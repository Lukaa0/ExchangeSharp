//TODO Convert to non-static class
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace ExchangeSharp.API.Exchanges.Poloniex
{
	public static class PoloniexHelper
	{
		private const string baseUrl = "https://api.poloniex.com";

		public static AuthenticationModel GetSignedAuthenticationModel(string methodType,
			string path, string secretKey, string apikey)
		{
			var timestamp = GetTimeStamp();
			var requestString = FormatWSRequestString(methodType, path, timestamp);
			var signature = Encode(requestString, secretKey);
			return new AuthenticationModel(apikey, timestamp, signature);
		}

		// ReSharper disable once TooManyArguments
		public static HttpRequestMessage GetAuthenticatedRequest(HttpMethod method, string path,
			Dictionary<string, object> parameters, string secretKey, string apiKey)
		{
			parameters = parameters.Where(param => param.Value != null).
				ToDictionary(x => x.Key, x => x.Value);
			var timestamp = GetTimeStamp();
			var requestString = method == HttpMethod.Get
				? FormatRequestString(method, path, timestamp, parameters)
				: FormatBodyString(method, path, timestamp, parameters);
			var signature = Encode(requestString, secretKey);
			parameters.Remove("signTimestamp");
			var normalizedPath = GetNormalizedData(method, path, parameters, timestamp);
			var requestMessage = new HttpRequestMessage(method, normalizedPath);
			requestMessage.Headers.Add("key", apiKey);
			requestMessage.Headers.Add("signatureMethod", "HmacSHA256");
			requestMessage.Headers.Add("signatureVersion", "2");
			requestMessage.Headers.Add("signTimestamp", timestamp);
			requestMessage.Headers.Add("signature", signature);
			if (parameters.Any() && method != HttpMethod.Get)
				requestMessage.Content = new StringContent(
					JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");
			return requestMessage;
		}

		private static string GetNormalizedData(HttpMethod method, string path,
			IDictionary<string, object> parameters, string timestamp)
		{
			var normalizedPath = new StringBuilder().Append(baseUrl).Append(path).Append('?');
			if (method == HttpMethod.Get)
				NormalizeQueryString(parameters, normalizedPath);
			else
				return path;
			normalizedPath.Length--;
			return normalizedPath.ToString();
		}

		private static string FormatRequestString(HttpMethod methodType, string path,
			string timestamp, IDictionary<string, object> parameters)
		{
			parameters.Add("signTimestamp", timestamp);
			var requestStringBuilder = new StringBuilder();
			requestStringBuilder.Append(methodType).Append("\n").Append(path).Append("\n");
			var sortedParameters = new SortedDictionary<string, object>(parameters);
			NormalizeQueryString(sortedParameters, requestStringBuilder);
			requestStringBuilder.Length--;
			return requestStringBuilder.ToString();
		}

		private static string FormatBodyString(HttpMethod methodType, string path,
			string timestamp, IDictionary<string, object> parameters)
		{
			var requestStringBuilder = new StringBuilder();
			requestStringBuilder.Append(methodType).Append("\n").Append(path).Append("\n");
			NormalizeBodyString(parameters, timestamp, requestStringBuilder);
			return requestStringBuilder.ToString();
		}

		private static void NormalizeQueryString(IDictionary<string, object> parameters,
			StringBuilder requestStringBuilder)
		{
			if (parameters == null || !parameters.Any())
				return;
			foreach (var parameter in parameters.Where(parameter => parameter.Value != null))
				requestStringBuilder.Append(parameter.Key + "=" + parameter.Value).Append("&");
		}

		private static void NormalizeBodyString(IDictionary<string, object> parameters,
			string timestamp, StringBuilder requestStringBuilder)
		{
			if (parameters != null && parameters.Count() > 0)
			{
				requestStringBuilder.Append("requestBody=").Append(
					JsonConvert.SerializeObject(parameters));
				requestStringBuilder.Append("&");
			}
			requestStringBuilder.Append("signTimestamp").Append("=").Append(timestamp);
		}

		private static string GetTimeStamp() =>
			((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).
				TotalMilliseconds).ToString();

		private static string FormatWSRequestString(string methodType, string path,
			string timeStamp) =>
			methodType + "\n" + path + "\n" + $"signTimestamp={timeStamp}";

		private static string Encode(string requestString, string secretKey)
		{
			var encoding = new ASCIIEncoding();
			using var hmacSha256 = new HMACSHA256(encoding.GetBytes(secretKey));
			var hashMessage = hmacSha256.ComputeHash(encoding.GetBytes(requestString));
			return Convert.ToBase64String(hashMessage);
		}
	}
}
