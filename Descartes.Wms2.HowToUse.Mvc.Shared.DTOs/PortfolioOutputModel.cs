namespace Descartes.Wms2.HowToUse.Mvc.Shared.DTOs
{
	public class PortfolioOutputModel
	{
		public long Id { get; set; }
		public string ExternalId { get; set; }
		public long AccountHoldingInstitutionId { get; set; }
		public long InvestmentCategoryId { get; set; }
		public long UserId { get; set; }
		public long ProposalId { get; set; }
		public string DepositNumber { get; set; }
		public string Name { get; set; }
		public string SystemName { get; set; }
		public long CurrencyId { get; set; }
		public string Color { get; set; }
		public bool IsTrading { get; set; }
		public bool HasPendingOrder { get; set; }
		public long LastOrderStatusId { get; set; }
		public DateTime CreationDate { get; set; }
		public DateTime? AcceptanceDate { get; set; }
		public DateTime? ClosedDate { get; set; }
		public bool IsDeleted { get; set; }

		public decimal? InvestedAmount { get; set; }
		public decimal? Total { get; set; }
		public decimal? MarketValue { get; set; }
		public decimal? ProfitOrLost { get; set; }
		public decimal? PerformanceSinceInception { get; set; }
		public decimal? PerformanceYearToDate { get; set; }
		public DateTime? PortfolioValueDate { get; set; }

		public IList<PortfolioPositionOutputModel> Positions { get; set; }
	}
}