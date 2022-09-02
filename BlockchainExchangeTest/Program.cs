using ExchangeSharp;
using ExchangeSharp.API.Exchanges.Blockchain;

var api =
	await ExchangeAPI.GetExchangeAPIAsync<ExchangeBlockchainAPI>() as ExchangeBlockchainAPI;
api.LoadAPIKeysUnsecure("",
	"secret");
var n = await api.GetFillsAsync("BTC-USD");
Console.ReadLine();
