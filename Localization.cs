using Framework.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Framework.Extensions
{
	public class Localization
    {

		public string LocalizeText(string ResourceValue)
		{
			string localizedText = string.Empty;
			if (ResourceValue != null)
			{
				localizedText = ResourceValue.Trim();
			}
			return localizedText;
		}



		public string LocalizeText(string ResourceValue, string PublishDateString, string ExpirationDateString)
		{
			DateTime publishDate = new DateTime();
			DateTime expirationDate = new DateTime();

			try
			{
				publishDate = Convert.ToDateTime(PublishDateString);
			}
			catch
			{
				publishDate = Convert.ToDateTime(DateTime.MinValue);
			}

			try
			{
				expirationDate = Convert.ToDateTime(ExpirationDateString);
			}
			catch
			{
				expirationDate = Convert.ToDateTime(DateTime.MaxValue);
			}

			return LocalizeText(ResourceValue, publishDate, expirationDate);
		}



		public string LocalizeText(string ResourceValue, DateTime PublishDate, DateTime ExpirationDate)
		{
			string localizedText = string.Empty;
			if (ResourceValue != null)
			{
				localizedText = ResourceValue.Trim();
			}

			if (PublishContent(PublishDate, ExpirationDate))
			{
				return localizedText;
			}
			else
			{
				return string.Empty;
			}
		}



		public Boolean PublishContent(string PublishDateString, string ExpirationDateString)
		{
			DateTime publishDate = new DateTime();
			DateTime expirationDate = new DateTime();

			try
			{
				publishDate = Convert.ToDateTime(PublishDateString);
			}
			catch
			{
				publishDate = Convert.ToDateTime(DateTime.MinValue);
			}

			try
			{
				expirationDate = Convert.ToDateTime(ExpirationDateString);
			}
			catch
			{
				expirationDate = Convert.ToDateTime(DateTime.MaxValue);
			}

			return PublishContent(publishDate, expirationDate);
		}



		public Boolean PublishContent(DateTime PublishDate, DateTime ExpirationDate)
		{
			DateTime currentDate = DateTime.Now;
			if (currentDate.Ticks > PublishDate.Ticks && currentDate.Ticks < ExpirationDate.Ticks)
			{
				return true;
			}
			else
			{
				return false;
			}
		}


		public string GetLocalizedFilePath(string FilePath)
		{
			string filePath = FilePath.Trim();
			string currentCulture = CultureInfo.CurrentCulture.ToString().ToLower();
			if ((currentCulture != "en-us" & !String.IsNullOrWhiteSpace(currentCulture)))
			{
				string fileExtension = Path.GetExtension(FilePath);
				string localizedFilePath = filePath.Substring(0, filePath.Length - fileExtension.Length) + "." + currentCulture + fileExtension;
				if (File.Exists(localizedFilePath) == true)
				{
					filePath = localizedFilePath;
				}
			}
			return filePath;
		}


		public string GetLocalizedHost(CultureInfo cultureInfo)
		{
			string fileContents = File.ReadAllText(Startup.ContentRootPath + "/Data/Localization.json");
			List<LocalizationItem> localizationItemList = JsonConvert.DeserializeObject<List<LocalizationItem>>(fileContents);
			if (localizationItemList.Where(x => x.CultureInfo == cultureInfo.Name).Count() > 0)
			{
				return localizationItemList.FirstOrDefault().Host.Trim().ToLower();
			}
			else
			{
				return string.Empty;
			}
		}


		public bool IsCultureImplemented(CultureInfo cultureInfo)
		{
			string fileContents = File.ReadAllText(Startup.ContentRootPath + "/Data/Localization.json");
			List<LocalizationItem> localizationItemList = JsonConvert.DeserializeObject<List<LocalizationItem>>(fileContents);
			if (localizationItemList.Where(x => x.CultureInfo == cultureInfo.Name).Count() > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
