namespace Descartes.Wms2.HowToUse.Mvc.Shared.DTOs
{
	public class InvestmentCategoryOutputModel
	{
		public long Id { get; set; }
		public string AvailableOnlyInCountry { get; set; }
		public long BusinessLineId { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}
}
