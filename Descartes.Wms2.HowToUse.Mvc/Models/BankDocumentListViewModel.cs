namespace Descartes.Wms2.HowToUse.Mvc.Models
{
	public class BankDocumentViewModel
	{
		public DateTime Created { get; set; }
		public string Reference { get; set; }
		public string Name { get; set; }
		public string DocumentType { get; set; }
	}

	public class BankDocumentListViewModel
	{
		public long PortfolioId { get; set; }

		public IList<BankDocumentViewModel> BankDocuments { get; set; } = new List<BankDocumentViewModel>();
	}
}
