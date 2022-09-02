using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;

namespace ExchangeSharp.API.Exchanges.Poloniex
{
	public class ExchangePoloniexAPIV2 : ExchangeAPI
	{
		public sealed override string BaseUrl { get; set; } = "https://api.poloniex.com";
		public override string BaseUrlWebSocket { get; set; } =
			"wss://ws.poloniex.com/ws/public";
		public override string BaseUrlPrivateWebSocket { get; set; } =
			"wss://ws.poloniex.com/ws/private";
		private readonly HttpClient client;

		public ExchangePoloniexAPIV2() =>
			client = new HttpClient { BaseAddress = new Uri(BaseUrl) };

		public async Task<IWebSocket> SubscribeToOrderBook(
			Action<List<PoloniexBook>> callback, int depth = 5, params string[] symbols) =>
			await ConnectPublicWebSocketAsync(string.Empty, (_socket, message) =>
				{
					if (message != null)
					{
						var token = message.ToStringFromUTF8();
						if (token.Contains("event"))
							return Task.CompletedTask;
						var response =
							JsonConvert.DeserializeObject<PoloniexBookResponse>(token);
						if (response is { Data: { } })
							callback(response.Data);
					}
					return Task.CompletedTask;
				},
				async _socket =>
				{
					await _socket.SendMessageAsync(new
					{
						@event = "subscribe", channel = new[] { "book" }, symbols, depth
					});
				});

		//TODO: Refactor into smaller functions maybe
		public async Task<IWebSocket> Subscribe(Channel channel,
			Action<Balance> balanceCallback = null,
			Action<WebSocketOrderResponse> ordersCallback = null)
		{
			var authModel = PoloniexHelper.GetSignedAuthenticationModel("GET", "/ws",
				PrivateApiKey.ToUnsecureString(), PublicApiKey.ToUnsecureString());
			if (channel == Channel.Balances)
				return await AuthenticateAndSubscribe(authModel, channel, balance =>
				{
					if (balance != null && (balance.Count > 0) & (balanceCallback != null))
						balanceCallback(balance.First());
				});
			if (channel == Channel.Orders)
				return await AuthenticateAndSubscribe(authModel, channel,
					onOrdersUpdate: order =>
					{
						if (order != null && (order.Count > 0) & (balanceCallback != null))
							ordersCallback(order.First());
					});
			//If Channel.All
			return await AuthenticateAndSubscribe(authModel, channel, balance =>
			{
				if (balance != null && (balance.Count > 0) & (balanceCallback != null))
					balanceCallback(balance.First());
			}, order =>
			{
				if (order != null && (order.Count > 0) & (balanceCallback != null))
					ordersCallback(order.First());
			});
		}

		private async Task<IWebSocket> AuthenticateAndSubscribe(AuthenticationModel authModel,
			Channel channel, Action<List<Balance>> onBalanceUpdate = null,
			Action<List<WebSocketOrderResponse>> onOrdersUpdate = null) =>
			await ConnectPrivateWebSocketAsync(string.Empty, async (socket, message) =>
				{
					if (message != null)
					{
						var jsonMessage = message.ToStringFromUTF8();
						if (jsonMessage.Contains("auth"))
						{
							var response =
								JsonConvert.
									DeserializeObject<
										BaseWebSocketResponse<
											WebSocketAuthenticationResponse>>(
										jsonMessage);
							if (response is { Data: { } } && response.Data.Success)
								if (channel == Channel.Balances)
									await socket.SendMessageAsync(new
									{
										@event = "subscribe", channel = new[] { "balances" }
									});
								else if (channel == Channel.Orders)
									await socket.SendMessageAsync(new
									{
										@event = "subscribe", channel = new[] { "orders" }
									});
						}
						else if (jsonMessage.Contains("orders") & (onOrdersUpdate != null))
						{
							if (jsonMessage.Contains("event"))
								//receipt
								//{
								// "channel": "orders",
								// "event": "subscribe"
								// }
								return;
							var response =
								JsonConvert.
									DeserializeObject<
										BaseWebSocketResponse<List<WebSocketOrderResponse>>>(
										jsonMessage);
							onOrdersUpdate(response.Data);
						}
						else if (jsonMessage.Contains("balances") & (onBalanceUpdate != null))
						{
							if (jsonMessage.Contains("event"))
								//receipt
								//{
								// "channel": "balances",
								// "event": "subscribe"
								// }
								return;
							var response =
								JsonConvert.
									DeserializeObject<BaseWebSocketResponse<List<Balance>>>(
										jsonMessage);
							onBalanceUpdate(response.Data);
						}
					}
					else
					{
						throw new AuthenticationException("Failed to authenticate");
					}
				},
				async _socket =>
				{
					await _socket.SendMessageAsync(new
					{
						@event = "subscribe",
						channel = new[] { "auth" },
						@params = JsonConvert.SerializeObject(authModel)
					});
				});

		public async Task<List<AllAccountBalancesResponse>> GetAllAccountBalancesAsync(
			string id = null, string accountType = null)
		{
			var parameters = new Dictionary<string, object>
			{
				{ "id", id }, { "accountType", accountType }
			};
			var request = PoloniexHelper.GetAuthenticatedRequest(HttpMethod.Get,
				"/accounts/balances", parameters, PrivateApiKey.ToUnsecureString(),
				PublicApiKey.ToUnsecureString());
			return await MakeRequestAsync<List<AllAccountBalancesResponse>>(request);
		}

		// ReSharper disable once TooManyArguments
		public async Task<List<OpenOrdersResponse>> GetOpenOrdersAsync(
			GetOpenOrdersRequestModel model)
		{
			var parameters = new Dictionary<string, object>
			{
				{ nameof(model.Symbol).ToLower(), model.Symbol },
				{ nameof(model.PoloSide).ToLower(), model.PoloSide },
				{ nameof(model.From).ToLower(), model.From.ToStringInvariant() },
				{ nameof(model.Direction).ToLower(), model.Direction },
				{ nameof(model.Limit).ToLower(), model.Limit.ToStringInvariant() }
			};
			var request = PoloniexHelper.GetAuthenticatedRequest(HttpMethod.Get, "/orders",
				parameters, PrivateApiKey.ToUnsecureString(),
				PublicApiKey.ToUnsecureString());
			return await MakeRequestAsync<List<OpenOrdersResponse>>(request);
		}

		private async Task<T> MakeRequestAsync<T>(HttpRequestMessage request)
		{
			var response = await client.SendAsync(request);
			if (!response.IsSuccessStatusCode)
				throw new HttpClientException(response.ReasonPhrase);
			var json = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<T>(json);
		}

		public async Task<List<CancelOrdersResponse>> CancelOrdersAsync(string[] orderIds,
			string[] clientOrderIds = null)
		{
			var parameters = new Dictionary<string, object>
			{
				{ nameof(orderIds), orderIds }, { nameof(clientOrderIds), clientOrderIds }
			};
			var request = PoloniexHelper.GetAuthenticatedRequest(HttpMethod.Delete,
				"/orders/cancelByIds", parameters, PrivateApiKey.ToUnsecureString(),
				PublicApiKey.ToUnsecureString());
			return await MakeRequestAsync<List<CancelOrdersResponse>>(request);
		}

		// ReSharper disable once TooManyArguments
		public async Task<List<OpenOrdersResponse>> CreateOrderAsync(
			CreateOrderRequestModel model)
		{
			ValidateAmount(model.Type, model.PoloSide, model.Amount);
			ValidatePrice(model.Type, model.Price.ToStringInvariant());
			ValidateQuantity(model.Type, model.PoloSide, model.Quantity);
			var parameters = new Dictionary<string, object>
			{
				{ nameof(model.Symbol).ToLower(), model.Symbol },
				{ "side", model.PoloSide },
				{ nameof(model.Type).ToLower(), model.Type },
				{ "accountType".ToLower(), model.AccountType },
				{ nameof(model.Quantity).ToLower(), model.Quantity?.ToStringInvariant() },
				{ nameof(model.Amount).ToLower(), model.Amount?.ToStringInvariant() },
				{ nameof(model.Price).ToLower(), model.Price?.ToStringInvariant() },
				{ "clientOrderId", model.ClientOrderId },
				{ nameof(model.TimeInForce).ToLower(), model.TimeInForce }
			};
			var request = PoloniexHelper.GetAuthenticatedRequest(HttpMethod.Post, "/orders",
				parameters, PrivateApiKey.ToUnsecureString(),
				PublicApiKey.ToUnsecureString());
			return await MakeRequestAsync<List<OpenOrdersResponse>>(request);
		}

		public async Task<List<OpenOrdersResponse>> GetOrderHistoryAsync(
			GetOrderHistoryRequestModel model)
		{
			var parameters = new Dictionary<string, object>
			{
				{ nameof(model.Symbol).ToLower(), model.Symbol },
				{ "side", model.PoloSide },
				{ nameof(model.Type).ToLower(), model.Type },
				{ "accountType".ToLower(), model.AccountType },
				{ nameof(model.Quantity).ToLower(), model.Quantity?.ToStringInvariant() },
				{ nameof(model.Amount).ToLower(), model.Amount?.ToStringInvariant() },
				{ nameof(model.Price).ToLower(), model.Price?.ToStringInvariant() },
				{ nameof(model.Direction), model.Direction },
				{
					nameof(model.HideCancel).ToLower(), model.HideCancel.ToStringInvariant()
				},
				{ nameof(model.States).ToLower(), model.States },
				{ nameof(model.From).ToLower(), model.From?.ToStringInvariant() },
				{ nameof(model.Limit).ToLower(), model.Limit?.ToStringInvariant() },
				{
					nameof(model.StartTime).ToLower(), model.StartTime?.ToStringInvariant()
				},
				{ nameof(model.EndTime).ToLower(), model.EndTime?.ToStringInvariant() }
			};
			var request = PoloniexHelper.GetAuthenticatedRequest(HttpMethod.Get,
				"/orders/history", parameters, PrivateApiKey.ToUnsecureString(),
				PublicApiKey.ToUnsecureString());
			return await MakeRequestAsync<List<OpenOrdersResponse>>(request);
		}

		private void ValidatePrice(string orderType, string price)
		{
			if (string.IsNullOrEmpty(price) && orderType != PoloOrderType.Market)
				throw new ArgumentNullException("Price must not be empty for limit orders");
		}

		private void ValidateQuantity(string orderType, string side, int? quantity)
		{
			if (quantity <= 0 && orderType == PoloOrderType.Market && side == PoloSide.Sell)
				throw new ArgumentNullException(
					"Quantity must not be empty for market orders");
		}

		private void ValidateAmount(string orderType, string side, double? amount)
		{
			if (amount <= 0 && orderType == PoloOrderType.Market && side == PoloSide.Buy)
				throw new ArgumentNullException("Amount must not be empty for market orders");
		}
	}
}
