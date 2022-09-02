using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExchangeSharp.Model;
using Newtonsoft.Json.Linq;

namespace ExchangeSharp.API.Exchanges.Blockchain
{
	public class ExchangeBlockchainAPI : ExchangeAPI
	{
		public override string BaseUrl { get; set; } =
			"https://api.blockchain.com/v3/exchange";

		protected override async Task<IEnumerable<ExchangeOrderResult>>
			OnGetOpenOrderDetailsAsync(string marketSymbol = null)
		{
			var query = "/orders?status=FILLED&limit=100";
			if (!string.IsNullOrEmpty(marketSymbol))
				query += $"&symbol={marketSymbol}";
			var orders = new List<ExchangeOrderResult>();
			var response = await MakeJsonRequestAsync<JArray>(
				query, BaseUrl, requestMethod: "GET");
			foreach (var token in response)
			{
				var order = new ExchangeOrderResult
				{
					ClientOrderId = token["clOrdId"].ToStringInvariant(),
					Result = ExchangeAPIOrderResult.Open,
					IsBuy = token["side"].ToStringInvariant() == "BUY",
					MarketSymbol = token["symbol"].ToStringInvariant(),
					Price = token["price"].ConvertInvariant<decimal>(),
					AveragePrice = token["avgPx"].ConvertInvariant<decimal>(),
					OrderDate = DateTimeOffset.
						FromUnixTimeMilliseconds(token["timestamp"].
							ConvertInvariant<long>()).DateTime
				};
				orders.Add(order);
			}
			return orders;
		}

		protected override async Task<Dictionary<string, decimal>> OnGetFeesAsync()
		{
			var response =
				await MakeJsonRequestAsync<JObject>("/fees", BaseUrl, requestMethod: "GET");
			return response.ToObject<Dictionary<string, decimal>>();
		}

		protected override async Task<AccountBalances> OnGetBalancesAsync()
		{
			var accountBalances = new AccountBalances();
			accountBalances.Balances = new List<AccountBalance>();
			var response =
				await MakeJsonRequestAsync<JObject>("/accounts", BaseUrl,
					requestMethod: "GET");
			accountBalances.Balance = response.First["balance"].ConvertInvariant<decimal>();
			accountBalances.AvailableBalance =
				response.First["available"].ConvertInvariant<decimal>();
			foreach (var token in response)
			{
				var exchangeBalance = new AccountBalance
				{
					AccountName = token.Key,
					LocalAvailableBalance =
						token.Value["available_local"].ConvertInvariant<decimal>(),
					BalanceLocal =
						token.Value["balance_local"].ConvertInvariant<decimal>(),
					Rate = token.Value["rate"].ConvertInvariant<decimal>()
				};
				accountBalances.Balances.Add(exchangeBalance);
			}
			return accountBalances;
		}

		protected override async Task<IEnumerable<ExchangeOrderResult>> OnGetFillsAsync(
			string marketSymbol = null, DateTime? fromDateTime = null,
			DateTime? toDateTime = null, long? fromExecId = null, long? toExecId = null,
			int limit = 100)
		{
			var orders = new List<ExchangeOrderResult>();
			var query = "/fills?";
			if (!string.IsNullOrEmpty(marketSymbol))
				query += $"symbol={marketSymbol}&";
			if (fromDateTime != null)
				query += $"from={fromDateTime}&";
			if (toDateTime != null)
				query += $"to={toDateTime}&";
			if (fromExecId != null)
				query += $"fromExecId={fromExecId}&";
			if (toExecId != null)
				query += $"toExecId={toExecId}&";
			query += $"limit={limit}";
			var response =
				await MakeJsonRequestAsync<JArray>(query, BaseUrl, requestMethod: "GET");
			foreach (var token in response)
			{
				var exchangeOrderResult = new ExchangeOrderResult
				{
					ExecutionOrderId = token["exOrdId"].ConvertInvariant<long>(),
					TradeId = token["tradeId"].ToStringInvariant(),
					ExecutionId = token["execId"].ConvertInvariant<long>(),
					IsBuy = token["side"].ToStringInvariant() == "BUY",
					Price = token["price"].ConvertInvariant<decimal>(),
					Fees = token["fee"].ConvertInvariant<decimal>(),
					OrderDate = DateTimeOffset.
						FromUnixTimeMilliseconds(token["timestamp"].
							ConvertInvariant<long>()).DateTime
				};
				orders.Add(exchangeOrderResult);
			}
			return orders;
		}

		protected override async Task<IEnumerable<ExchangeWhitelist>> OnGetWhiteListAsync()
		{
			var response =
				await MakeJsonRequestAsync<JArray>("/whitelist", BaseUrl,
					requestMethod: "GET");
			return response.ToObject<List<ExchangeWhitelist>>();
		}

		protected override async Task<ExchangeWithdrawalResponse> OnWithdrawAsync(
			decimal? amount, string currency, string beneficiary, bool sendMax = false)
		{
			var parameters = new Dictionary<string, object>
			{
				{ "amount", amount },
				{ "currency", currency },
				{ "beneficiary", beneficiary },
				{ "sendMax", sendMax }
			};
			var response =
				await MakeJsonRequestAsync<JToken>("/withdrawals", BaseUrl, parameters,
					"POST");
			return new ExchangeWithdrawalResponse
			{
				Id = response["withdrawalId"].ToStringInvariant(),
				Amount = response["amount"].ConvertInvariant<decimal>(),
				Fee = response["fee"].ConvertInvariant<decimal>(),
				Symbol = response["currency"].ToStringInvariant(),
				Beneficiary = response["beneficiary"].ToStringInvariant(),
				Message = response["state"].ToStringInvariant()
			};
		}

		protected override async Task OnCancelOrderAsync(string orderId,
			string marketSymbol = null, bool isClientOrderId = false) =>
			await MakeJsonRequestAsync<JToken>($"/orders/{orderId}", BaseUrl,
				requestMethod: "DELETE");

		protected override async Task ProcessRequestAsync(IHttpWebRequest request,
			Dictionary<string, object> payload)
		{
			request.AddHeader("X-Api-Token", PrivateApiKey.ToUnsecureString());
			await request.WriteToRequestAsync(null);
		}
	}
}
