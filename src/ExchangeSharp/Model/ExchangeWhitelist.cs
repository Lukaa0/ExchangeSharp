using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeSharp.Model
{
		public class ExchangeWhitelist
		{
			/// <summary>
			/// Whitelist ID
			/// </summary>
			public string WhitelistId { get; set; }
			/// <summary>
			/// Name of the withdrawal
			/// </summary>
			public string Name { get; set; }
			/// <summary>
			/// Name of the currency
			/// </summary>
			public string Currency { get; set; }
		}
}
