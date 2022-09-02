namespace ExchangeSharp.API.Exchanges.Poloniex
{
	public class CreateOrderRequestModel
	{
		public string Symbol { get; set; }
		public string PoloSide { get; set; }
		public string TimeInForce { get; set; }
		public string Type { get; set; }
		public string AccountType { get; set; }
		public double? Price { get; set; }
		public int? Quantity { get; set; }
		public double? Amount { get; set; }
		public string ClientOrderId { get; set; }
	}
}
