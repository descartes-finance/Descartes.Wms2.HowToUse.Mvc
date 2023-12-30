namespace Descartes.Wms2.HowToUse.Mvc.Shared.DTOs
{
	public class ReasonToChangeProposalResponseOutputModel
	{
		public long Id { get; set; }
		public string Code { get; set; }
		public string Description { get; set; }
		public int Order { get; set; }
	}

	public class ReasonToChangeProposalOutputModel
	{
		public long Id { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public bool IsDeleted { get; set; }

		public IList<ReasonToChangeProposalResponseOutputModel> Responses { get; set; }
	}
}
