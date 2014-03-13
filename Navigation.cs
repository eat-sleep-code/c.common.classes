using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;


public class Navigation
{
    private Localization localizeSite = new Localization();
    private DataXML xmlData = new DataXML();

    public String GenerateNavigation(String displayIn = "both", String region = "")
    {
        StringBuilder stringNavigation = new StringBuilder("");
        DataTable dataTableNavigation = null;
        dataTableNavigation = xmlData.GetDataTable(HttpContext.Current.Server.MapPath("~/App_Data/navigation.xml"), "Item");

        if (dataTableNavigation.Rows.Count > 0)
        {
            stringNavigation.AppendLine(GenerateNavigationItem(dataTableNavigation, 0, displayIn));
            dataTableNavigation.Clear();
        }
        dataTableNavigation = null;
        return stringNavigation.ToString().Trim();
    }


    public String GenerateNavigationItem(DataTable dataTableNavigation, Int32 parentID = 0, String menuType = "Header")
    {
        if (dataTableNavigation.Select("ParentID = " + parentID.ToString() + " AND (DisplayIn = '" + menuType + "' OR DisplayIn = 'both')", "SortOrder").Count() == 0)
        {
            return "";
        }

        StringBuilder stringNavigationItem = new StringBuilder("");
        stringNavigationItem.AppendLine("<ul class=\"navigation-" + menuType.ToLower() + "\">");

        foreach (DataRow dataRowNavigation in dataTableNavigation.Select("ParentID = " + parentID.ToString() + " AND (DisplayIn = '" + menuType + "' OR DisplayIn = 'both')", "SortOrder"))
        {

			if (localizeSite.PublishContent(dataRowNavigation["PublishDate"].ToString(), dataRowNavigation["ExpirationDate"].ToString()) == true)
			{
				String cssClass = "";
				Int32 CurrentIndex = dataRowNavigation.Table.Rows.IndexOf(dataRowNavigation);

				if (Convert.ToInt32(dataRowNavigation["ParentID"]) == 0)
				{
					cssClass += " navigation-root";
				}
				else
				{
					cssClass += " navigation-sub";
				}

				if (Convert.ToString(dataRowNavigation["Url"]).Trim().ToLower() == HttpContext.Current.Request.Url.AbsolutePath.ToLower())
				{
					cssClass += " navigation-selected";
				}

				if (CurrentIndex == 0)
				{
					cssClass += " navigation-first";
				}

				if (CurrentIndex >= dataTableNavigation.Rows.Count)
				{
					cssClass += " navigation-last";
				}

				cssClass = cssClass.Trim();

				stringNavigationItem.Append("<li class=\"" + cssClass + "\">");
				stringNavigationItem.Append("<a href=\"" + dataRowNavigation["url"].ToString().Trim() + "\" target=\"" + dataRowNavigation["Target"].ToString().Trim() + "\" title=\"" + dataRowNavigation["Title"].ToString().Trim() + "\">" + dataRowNavigation["Text"].ToString().Trim());
				if (dataRowNavigation["MenuStyle"].ToString().Trim().ToLower() == "mega")
				{
					if (dataRowNavigation["MegaMenuImage"].ToString().Trim().Length > 0)
					{
						stringNavigationItem.Append("<img src=\"" + dataRowNavigation["MegaMenuImage"].ToString() + "\" class=\"navigation-mega-icon\" alt=\"" + dataRowNavigation["Alt"].ToString() + "\" title=\"" + dataRowNavigation["Title"].ToString() + "\">");
					}
					if (dataRowNavigation["MegaMenuText"].ToString().Trim().Length > 0)
					{
						stringNavigationItem.Append(dataRowNavigation["MegaMenuText"].ToString());
					}
				}
				stringNavigationItem.Append("</a>");

				string navigationItem = GenerateNavigationItem(dataTableNavigation, Convert.ToInt32(dataRowNavigation["ItemID"]), menuType);
				if (!String.IsNullOrEmpty(navigationItem))
				{
					if (dataRowNavigation["MenuStyle"].ToString().Trim().ToLower() == "mega")
					{
					
						stringNavigationItem.AppendLine("<div class=\"navigation-mega\">");
						stringNavigationItem.AppendLine(navigationItem);
						stringNavigationItem.AppendLine("</div>");
					}
					else
					{
						stringNavigationItem.AppendLine("<div class=\"navigation-standard\">");
						stringNavigationItem.AppendLine(navigationItem);
						stringNavigationItem.AppendLine("</div>");
					}
				}
				stringNavigationItem.AppendLine("</li>");
			}
        }
        stringNavigationItem.AppendLine("</ul>");
        return stringNavigationItem.ToString();
    }

}

