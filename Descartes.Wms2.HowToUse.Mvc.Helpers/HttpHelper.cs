using Newtonsoft.Json;

using System.Net.Http.Headers;
using System.Text;

namespace Descartes.Wms2.HowToUse.Mvc.Helpers
{
	public static class HttpHelper
	{
		public static HttpContent CreateMultipartFormDataHttpContent(object payLoad, byte[] pdfFileContent, string multipartFormDataContentName)
		{
			var multipartFormDataContent = new MultipartFormDataContent();

			var modelStringContent = JsonConvert.SerializeObject(payLoad);
			multipartFormDataContent.Add(new StringContent(modelStringContent, Encoding.UTF8, "application/json"), "JsonPayload");

			var byteArrayContent = new ByteArrayContent(pdfFileContent);
			byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
			multipartFormDataContent.Add(byteArrayContent, multipartFormDataContentName, fileName: "Whatever.pdf");

			return multipartFormDataContent;
		}
	}
}
