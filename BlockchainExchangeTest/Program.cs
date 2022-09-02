using ExchangeSharp;
using ExchangeSharp.API.Exchanges.Blockchain;

var api =
	await ExchangeAPI.GetExchangeAPIAsync<ExchangeBlockchainAPI>() as ExchangeBlockchainAPI;
api.LoadAPIKeysUnsecure("", "secret");
//Open orders
var openOrders = await api.GetOpenOrderDetailsAsync("BTC-USD");
//Fills
var fills = await api.GetFillsAsync("BTC-USD");
//Balances of all accounts
var accountBalances = (await api.GetBalancesAsync()).Balances;
//Cancel order by ID
await api.CancelOrderAsync("someId", "someSymbolPair");
// Fees
var fees = await api.GetFeesAsync();
//To make a withdrawal request, you must get a beneficiary ID from /whitelist endpoint:
var whitelist = await api.GetWhitelistedAccountsAsync();
//Make a withdrawal request - Beneficiary ID is retrieved through api.GetWhitelistedAccountsAsyc()
var withdrawalResponse =
	await api.WithdrawAsync(2, "someSymbolPair", whitelist.First().WhitelistId, true);
