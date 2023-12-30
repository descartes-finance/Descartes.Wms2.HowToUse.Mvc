using Descartes.Wms2.HowToUse.Mvc.Shared.DTOs;

namespace Descartes.Wms2.HowToUse.Mvc.Models
{
	public class PortfolioDetailViewModel
	{
		public string ClientName { get; set; }
		public string ClientSurname { get; set; }

		public PortfolioOutputModel Portfolio { get; set; }
    }
}
