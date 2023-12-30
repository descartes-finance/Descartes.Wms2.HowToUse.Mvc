namespace Descartes.Wms2.HowToUse.Mvc.Shared.DTOs
{
	public class AccountHoldingInstitutionOutputModel
	{
		public long Id { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public string CareOf { get; set; }
		public string Street { get; set; }
		public string City { get; set; }
		public long CountryId { get; set; }
		public bool Deleted { get; set; }
	}
}
