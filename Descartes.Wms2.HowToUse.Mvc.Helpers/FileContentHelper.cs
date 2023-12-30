namespace Descartes.Wms2.HowToUse.Mvc.Helpers
{
	public class FileContentHelper
	{
		public static byte[] GetFileContent(Type assembly, string fileName)
		{
			var pdfBytes = default(byte[]);

			using (var stream = assembly.Assembly.GetManifestResourceStream(fileName))
			{
				using var streamReader = new StreamReader(stream);
				using var memStream = new MemoryStream();
				streamReader.BaseStream.CopyTo(memStream);
				pdfBytes = memStream.ToArray();
			}

			return pdfBytes;
		}
	}
}
