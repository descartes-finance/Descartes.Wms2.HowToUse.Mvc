using Descartes.Wms2.HowToUse.Mvc.Shared.DTOs;

namespace Descartes.Wms2.HowToUse.Mvc.Models
{
	public class PlanChangeProposalSelectionViewModel
	{
		public long PortfolioId { get; set; }
		public long ClientId { get; set; }
		public long ProposalId { get; set; }

		public IList<ProposalOutputModel> Proposals { get; set; }
	}
}