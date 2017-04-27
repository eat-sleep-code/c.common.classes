using Framework.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Framework.Extensions
{

	public class Navigation
	{
		Localization localization = new Localization();
		private static readonly MemoryCache NavigationCache = new MemoryCache(new MemoryCacheOptions() { });

		public string GenerateNavigation(String displayIn = "both", String region = "")
		{
			string cacheName = "navigation" + "-" + displayIn + "-" + region;
            var cachedNavigation = (string)NavigationCache.Get(cacheName);
			if (cachedNavigation != null)
			{
				return cachedNavigation;
			}
			else
			{
				string fileContents = File.ReadAllText(localization.GetLocalizedFilePath(Startup.ContentRootPath + "/Data/Navigation.json"));
				List<NavigationItem> navigationItemList = JsonConvert.DeserializeObject<List<NavigationItem>>(fileContents);
				IQueryable<NavigationItem> navigationItemQueryable = navigationItemList.AsQueryable().OrderByDescending(x => x.SortOrder);

				StringBuilder stringNavigation = new StringBuilder(string.Empty);
				stringNavigation.AppendLine(GenerateNavigationItem(navigationItemQueryable, 0, displayIn));
				NavigationCache.Set(cacheName, stringNavigation.ToString().Trim(), DateTimeOffset.UtcNow.AddHours(2));
				return stringNavigation.ToString().Trim();
			}
		}


		public string GenerateNavigationItem(IQueryable<NavigationItem> navigationItemList, Int32 parentID = 0, String menuType = "Header")
		{
			int navigationItemCount = navigationItemList.Where(x => x.ParentID == parentID && (x.DisplayIn.Trim().ToLower() == menuType.Trim().ToLower() || x.DisplayIn.Trim().ToLower() == "both")).OrderBy(x => x.SortOrder).Count();
			if (navigationItemCount == 0)
			{
				return string.Empty;
			}
			int navigationItemIndex = 0;

			StringBuilder navigationMenu = new StringBuilder(string.Empty);
			navigationMenu.AppendLine("<ul class=\"navigation-" + menuType.ToLower() + "\">");

			foreach (NavigationItem navigationItem in navigationItemList.Where(x => x.ParentID == parentID && (x.DisplayIn.Trim().ToLower() == menuType.Trim().ToLower() || x.DisplayIn.Trim().ToLower() == "both")).OrderBy(x => x.SortOrder))
			{
				navigationItemIndex++;
				if (localization.PublishContent(navigationItem.PublishDate.ToString(), navigationItem.ExpirationDate.ToString()) == true)
				{
					string cssClass = string.Empty;

					if (Convert.ToInt32(navigationItem.ParentID) == 0)
					{
						cssClass += " navigation-root";
					}
					else
					{
						cssClass += " navigation-sub";
					}

					if (navigationItem.Url.Trim().ToLower() == new Context().Url)
					{
						cssClass += " navigation-selected";
					}

					if (navigationItemIndex == 1)
					{
						cssClass += " navigation-first";
					}

					if (navigationItemIndex >= navigationItemCount)
					{
						cssClass += " navigation-last";
					}

					cssClass = cssClass.Trim();

					navigationMenu.Append("<li class=\"" + cssClass + "\">");
					navigationMenu.Append("<a href=\"" + navigationItem.Url.ToString().Trim() + "\" target=\"" + navigationItem.Target.ToString().Trim() + "\" title=\"" + navigationItem.Title.ToString().Trim() + "\">" + navigationItem.Text.ToString().Trim());
					if (navigationItem.MenuStyle.ToString().Trim().ToLower() == "mega")
					{
						if (navigationItem.MegaMenuImage.ToString().Trim().Length > 0)
						{
							navigationMenu.Append("<img src=\"" + Startup.CDNUrl + navigationItem.MegaMenuImage.ToString() + "\" class=\"navigation-mega-icon\" alt=\"" + navigationItem.Alt.ToString() + "\" title=\"" + navigationItem.Title.ToString() + "\">");
						}
						if (navigationItem.MegaMenuText.ToString().Trim().Length > 0)
						{
							navigationMenu.Append(navigationItem.MegaMenuText.ToString());
						}
					}
					navigationMenu.Append("</a>");

					string navigationSubItem = GenerateNavigationItem(navigationItemList, Convert.ToInt32(navigationItem.ItemID), menuType);
					if (!String.IsNullOrEmpty(navigationSubItem))
					{
						if (navigationItem.MenuStyle.ToString().Trim().ToLower() == "mega")
						{

							navigationMenu.AppendLine("<div class=\"navigation-mega\">");
							navigationMenu.AppendLine(navigationSubItem);
							navigationMenu.AppendLine("</div>");
						}
						else
						{
							navigationMenu.AppendLine("<div class=\"navigation-standard\">");
							navigationMenu.AppendLine(navigationSubItem);
							navigationMenu.AppendLine("</div>");
						}
					}
					navigationMenu.AppendLine("</li>");
				}
			}
			navigationMenu.AppendLine("</ul>");
			return navigationMenu.ToString();
		}



		public string GenerateLocalizationMenu()
		{
			Uri url = new Context().AbsoluteUri;
			string subdomain = UrlManagement.GetSubDomain(url);
			string localizedUrl = url.ToString();

			string cacheName = "navigation-localization-" + url.AbsolutePath + "-" + CultureInfo.CurrentCulture.Name;
			var cachedNavigation = (string)NavigationCache.Get(cacheName);
			if (cachedNavigation != null)
			{
				return cachedNavigation;
			}
			else
			{
				string fileContents = File.ReadAllText(Startup.ContentRootPath + "/Data/Localization.json");
				List<LocalizationItem> localizationItemList = JsonConvert.DeserializeObject<List<LocalizationItem>>(fileContents);
				StringBuilder localizationMenuCurrent = new StringBuilder(string.Empty);
				StringBuilder localizationMenu = new StringBuilder(string.Empty);

				localizationMenu.AppendLine("<ul>");
				foreach (LocalizationItem localizationItem in localizationItemList)
				{
					string localIdentifier = localizationItem.LocalIdentifier.ToLower();
					if (localIdentifier == "us" || localIdentifier == "en-us")
					{
						//localIdentifier = "www";
						localIdentifier = "";
					}

					if (!String.IsNullOrEmpty(subdomain))
					{
						localizedUrl = url.ToString().Replace(subdomain, localIdentifier);
					}
					else
					{
						localizedUrl = url.ToString().Replace("://", "://" + localIdentifier + ".");
					}

					// REMOVE EXTRANEOUS DOT FROM THE URLS
					localizedUrl = localizedUrl.Replace("://.", "://");

					localizationMenu.Append("<li>");

					localizationMenu.Append("<a href=\"" + localizedUrl + "\" title=\"" + localizationItem.DisplayName.Trim() + "\" target=\"_self\">");
					if (!string.IsNullOrWhiteSpace(localizationItem.Image.Trim()))
					{
						localizationMenu.Append("<img alt=\"" + localizationItem.DisplayName.Trim() + "\" src=\"" + Startup.CDNUrl + localizationItem.Image.Trim() + "\" class=\"item-image\">");

					}
					localizationMenu.Append(localizationItem.DisplayName.Trim());
					localizationMenu.Append("</a>");
					localizationMenu.AppendLine("</li>");

					// IF THIS IS THE CURRENT CULTURE, THEN PREPEND IT TO THE LIST
					if (new Context().Culture.Name == localizationItem.CultureInfo.Trim())
					{
						localizationMenuCurrent.Append("<a>");
						localizationMenuCurrent.Append(localizationItem.DisplayName.Trim());
						localizationMenuCurrent.Append("<img alt=\"\" src=\"" + Startup.CDNUrl + "/images/arrow-down.png\" class=\"select-image\" />");
						localizationMenuCurrent.AppendLine("</a>");
					}
				}
				localizationMenu.AppendLine("</ul>");
				string localizationMenuFull = "<ul class=\"localization-dropdown\"><li>" + localizationMenuCurrent.ToString().Trim() + localizationMenu.ToString().Trim() + "</li></ul>";
				NavigationCache.Set(cacheName, localizationMenuFull, DateTimeOffset.UtcNow.AddHours(2));
				return localizationMenuFull;
			}
		}
	}
}
