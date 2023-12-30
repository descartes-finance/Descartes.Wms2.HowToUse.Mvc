namespace Descartes.Wms2.HowToUse.Mvc.Shared.DTOs
{
	public class CashAccountMovementOutputModel
	{
		public long CashAccountId { get; set; }

		public DateTime BookingDate { get; set; }

		public decimal Amount { get; set; }

		public decimal Saldo { get; set; }

		public string TransactionId { get; set; }

		public long BookingCodeId { get; set; }

		public string BookingCode { get; set; }

		public string BookingCodeType { get; set; }

		public string BookingDescription { get; set; }

		public string Isin { get; set; }

		public bool IsFirstPayment { get; set; }

		public string UsetText { get; set; }

		public decimal? UnitPrice { get; set; }

		public decimal? Quantity { get; set; }
	}
}