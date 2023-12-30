namespace Descartes.Wms2.HowToUse.Mvc.Shared.DTOs
{
	public class FileOutputModel
	{
		public string FileName { get; set; }

		public byte[] FileContent { get; set; }

		public string ContentType { get; set; }
	}
}
