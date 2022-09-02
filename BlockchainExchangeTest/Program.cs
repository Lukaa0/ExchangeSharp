using ExchangeSharp;
using ExchangeSharp.API.Exchanges.Blockchain;

var api =
	await ExchangeAPI.GetExchangeAPIAsync<ExchangeBlockchainAPI>() as ExchangeBlockchainAPI;
api.LoadAPIKeysUnsecure("", "");
var balances = await api.GetBalancesAsync();
Console.ReadLine();
