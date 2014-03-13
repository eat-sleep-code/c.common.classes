using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace framework
{
	public class UrlManagement
	{

		public static Boolean IsLocalizedUrl(Uri url)
		{
			string subDomain = GetSubDomain(url).ToLower();
			if (subDomain == "www" || subDomain == "en-us" || subDomain == "")
			{
				return false;
			}
			else
			{
				return true;
			}
		}


		public static string GetSubDomain(Uri url)
		{
			string host = url.Host;
			if (host.Split('.').Length > 2)
			{
				int lastIndex = host.LastIndexOf(".");
				int index = host.LastIndexOf(".", lastIndex - 1);
				return host.Substring(0, index).ToLower();
			}
			return "";
		}


		public static CultureInfo GetCultureInfoFromUrl(Uri url)
		{
			String subDomain = GetSubDomain(url);

			//TODO: ADD LOGIC TO MATCH SUBDOMAIN TO LOCAL IDENTIFIER IN XML FILE
			XDocument xdoc = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/localization.xml"));
			String cultureInfo = xdoc.Root.Elements("Language")
				.Where(i => (string)i.Element("LocalIdentifier") == subDomain)
				.Select(i => (string) i.Element("CultureInfo"))
				.FirstOrDefault();
			if (String.IsNullOrEmpty(cultureInfo) == true)
			{
				return new CultureInfo("en-us");
			}
			else
			{
				return new CultureInfo(cultureInfo);
			}
		}
	}
}