namespace Descartes.Wms2.HowToUse.Mvc.Shared.DTOs
{
	public class ProposalPositionOutputModel
	{
		public long Id { get; set; }
		public decimal Allocation { get; set; }
		public long CurrencyId { get; set; }
		public long SecurityId { get; set; }
		public long Order { get; set; }
	}

	public class ProposalOutputModel
	{
		public long Id { get; set; }
		public IList<long> InvestmentCategoryIds { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Color { get; set; }
		public string Code { get; set; }
		public long RiskCategorizationId { get; set; }
		public bool IsDeleted { get; set; }
		public int Order { get; set; }
		public decimal Volatility { get; set; }
		public decimal ExpectedReturn { get; set; }

		public IList<ProposalPositionOutputModel> Positions { get; set; }
	}
}
