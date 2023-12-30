namespace Descartes.Wms2.HowToUse.Mvc.Shared.DTOs
{
	public class ReservationOutputModel
	{
		public long Id { get; set; }
		public string ExternalReservationId { get; set; }

		public string DepositNumber { get; set; }
		public string PlanNumber { get; set; }

		public string AccountNumber { get; set; }
		public string Iban { get; set; }

		public string ClientNumber { get; set; }

		public DateTime RequestedDate { get; set; }
		public DateTime? UsedDate { get; set; }
	}
}
