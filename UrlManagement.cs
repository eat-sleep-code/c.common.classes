using Framework.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Framework.Extensions
{
	public class UrlManagement
	{
		public static bool IsLocalizedUrl()
		{
			try
			{
				return IsLocalizedUrl(new Context().AbsoluteUri);
			}
			catch
			{
				return false;
			}
		}

		public static bool IsLocalizedUrl(Uri url)
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
			string[] hostSegments = host.Split('.');
			if (hostSegments.Length > 2)
			{
				return hostSegments[0];
			}
			return string.Empty;
		}


		public CultureInfo GetCultureInfoFromUrl()
		{
			return GetCultureInfoFromUrl(new Context().AbsoluteUri);
		}

		public static CultureInfo GetCultureInfoFromUrl(HttpRequest httpRequest)
		{
			try
			{
				UriBuilder uriBuilder = new UriBuilder()
				{
					Scheme = httpRequest.Scheme,
					Host = httpRequest.Host.Host
				};
				return GetCultureInfoFromUrl(uriBuilder.Uri);
			}
			catch
			{
				return new CultureInfo("en-us");
			}
		}

		public static CultureInfo GetCultureInfoFromUrl(Uri uri)
		{
			string subDomain = GetSubDomain(uri);
			if (!string.IsNullOrWhiteSpace(subDomain))
			{
				string fileContents = File.ReadAllText(Startup.ContentRootPath + "/Data/Localization.json");
				List<LocalizationItem> localizationItemList = JsonConvert.DeserializeObject<List<LocalizationItem>>(fileContents);
				string cultureInfo = localizationItemList.Where(l => l.LocalIdentifier == subDomain).Select(l => l.CultureInfo).FirstOrDefault();
				if (!string.IsNullOrEmpty(cultureInfo))
				{
					return new CultureInfo(cultureInfo); 	
				}
			}
			return new CultureInfo("en-us");
		}
	}
}