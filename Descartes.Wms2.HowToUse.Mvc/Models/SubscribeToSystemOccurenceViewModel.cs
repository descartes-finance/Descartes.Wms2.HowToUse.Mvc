using Descartes.Wms2.HowToUse.Mvc.Shared.DTOs;

namespace Descartes.Wms2.HowToUse.Mvc.Models
{
	public class SubscribeToSystemOccurenceViewModel
	{
		public List<SystemOccurenceOutputModel> SystemOccurences { get; set; }

		public ClientOutputModel Admin { get; set; }

		public long AdminUserId { get; set; }
		public List<long> Selected {  get; set; }
	}
}
