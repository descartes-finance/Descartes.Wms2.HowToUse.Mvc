namespace Descartes.Wms2.HowToUse.Mvc.Models
{
	public class PdfPlaceHolder
	{
		public string Name { get; }
		public string Value { get; }

		public PdfPlaceHolder(string name, string value) { this.Name = name; this.Value = value; }
	}
}
