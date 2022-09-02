using System.Collections.Generic;

namespace ExchangeSharp.Model
{
	public class AccountBalances
	{
		/// <summary>
		///     Account balance
		/// </summary>
		public decimal Balance { get; set; }
		/// <summary>
		///     Available balance
		/// </summary>
		public decimal AvailableBalance { get; set; }
		/// <summary>
		///     Available balance
		/// </summary>
		public ICollection<AccountBalance> Balances { get; set; }
	}

	public class AccountBalance
	{
		/// <summary>
		///     Account name of the given balance
		/// </summary>
		public string AccountName { get; set; }
		/// <summary>
		///     Currency of the balance
		/// </summary>
		public string Currency { get; set; }
		/// <summary>
		///     Local account balance
		/// </summary>
		public decimal BalanceLocal { get; set; }
		/// <summary>
		///     Local available balance
		/// </summary>
		public decimal LocalAvailableBalance { get; set; }
		/// <summary>
		///     Rate
		/// </summary>
		public decimal Rate { get; set; }
	}
}
