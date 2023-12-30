using Newtonsoft.Json;

namespace Descartes.Wms2.HowToUse.Mvc.Extensions
{
	public static class SessionExtensions
	{
		public static void Set<T>(this ISession session, string key, T value)
		{
			session.SetString(key, JsonConvert.SerializeObject(value));
		}

		public static T Get<T>(this ISession session, string key)
		{
			var value = session.GetString(key);

			return value == null ? default : JsonConvert.DeserializeObject<T>(value);
		}
	}
}