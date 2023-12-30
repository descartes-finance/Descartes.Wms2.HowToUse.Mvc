namespace Descartes.Wms2.HowToUse.Mvc.Shared.DTOs
{
	public class CashAccountOutputModel
	{
		public long Id { get; set; }

		public string Number { get; set; }

		public string Iban { get; set; }

		public decimal Balance { get; set; }

		public DateTime BalanceDate { get; set; }

		public DateTime OpeningDate { get; set; }

		public long AccountTypeId { get; set; }

		public long CurrencyId { get; set; }

		public IList<long> ConnectedPortfolioIds { get; set; }
	}
}
