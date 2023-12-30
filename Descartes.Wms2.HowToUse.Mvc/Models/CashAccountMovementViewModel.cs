namespace Descartes.Wms2.HowToUse.Mvc.Models
{
	public class CashAccountMovementViewModel
	{
		public string TransactionId { get; set; }
		public string BookingCode { get; set; }
		public string BookingCodeDescription { get; set; }
		public DateTime BookingDate { get; set; }
		public string MovementDescription { get; set; }
		public decimal MovementAmount { get; set; }
		public decimal Saldo { get; set; }
		public string UsetText { get; set; }
	}
}
