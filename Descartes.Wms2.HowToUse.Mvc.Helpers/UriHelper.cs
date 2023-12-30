namespace Descartes.Wms2.HowToUse.Mvc.Helpers
{
	public static class UriHelper
	{
		public static long GetIdFromLocationUri(Uri uri)
		{
			var theUriAsString = uri.GetLeftPart(UriPartial.Path);
			return long.Parse(theUriAsString.Split('/').ToList().Last());
		}
	}
}