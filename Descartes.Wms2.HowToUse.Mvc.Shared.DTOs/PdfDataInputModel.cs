namespace Descartes.Wms2.HowToUse.Mvc.Shared.DTOs
{
	public class FormField
	{
		public string Name { get; }
		public string Value { get; }

		public FormField(string name, string value)
		{
			this.Name = name;
			this.Value = value;
		}
	}

	public class PdfDataInputModel
	{
		public string Password { get; set; }
		public string InvestmentCategoryCode { get; set; }
		public string DocumentLanguageCode { get; set; }
		public string AdditionalPageIdentificationInfo { get; set; }
		public IList<FormField> PlaceHolderValues { get; set; }
	}
}
