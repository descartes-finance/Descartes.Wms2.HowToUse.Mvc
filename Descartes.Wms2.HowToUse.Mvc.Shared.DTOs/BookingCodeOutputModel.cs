namespace Descartes.Wms2.HowToUse.Mvc.Shared.DTOs
{
	public class BookingCodeOutputModel
	{
		public long Id { get; set; }
		public long AccountHoldingInstitutionId { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public long BookingCategoryId { get; set; }
	}
}
