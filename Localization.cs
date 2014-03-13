using Microsoft.AspNet.FriendlyUrls;
using System;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.UI;


public class Localization
{

    public string LoadMasterPage(string masterPagePath)
    {
		string localIdentifier = Thread.CurrentThread.CurrentUICulture.Name;
        string masterPage = "";
        if (localIdentifier != "en-US" & localIdentifier.Length > 0)
        {
            localIdentifier = "." + localIdentifier;
        }
        //// CHECK TO SEE WHAT SKINS EXIST AND APPLY CORRECT SKIN
        bool localizedSkinExists = File.Exists(HttpContext.Current.Server.MapPath(masterPagePath.Replace(".master", localIdentifier + ".master")));
        if (localizedSkinExists == true)
        {
            masterPage = masterPagePath.Replace(".master", localIdentifier + ".master");
        }
        else
        {
            masterPage = masterPagePath;
        }
        // HttpContext.Current.Response.Write(masterPage + "<br />")
        return masterPage;
    }




    public string LocalizeText(Page CurrentPage, string resourceKey)
    {
        string localizedText = "";
		
		// LOOK FOR LOCALIZED TEXT
		String filePath = "";
		if (!String.IsNullOrEmpty(CurrentPage.Request.GetFriendlyUrlFileVirtualPath()))
		{
			filePath = CurrentPage.Request.GetFriendlyUrlFileVirtualPath();  // FOR FRIENDLY URLS
		}
		else
		{
			filePath = CurrentPage.Request.CurrentExecutionFilePath; // FOR "UNFRIENDLY" URLS (THOSE WITH A FILE EXTENSION VISIBLE)
		}

        try
        {
			localizedText = Convert.ToString(HttpContext.GetLocalResourceObject(filePath, resourceKey, System.Globalization.CultureInfo.CurrentCulture)).Trim();
        }
        catch //(Exception ex)
        {
             //HttpContext.Current.Response.Write(ex.ToString() + "<br />" + filePath);
        }

		if (PublishContent(Convert.ToString(HttpContext.GetLocalResourceObject(filePath, resourceKey + ".PublishDate", System.Globalization.CultureInfo.CurrentCulture)).Trim(), Convert.ToString(HttpContext.GetLocalResourceObject(filePath, resourceKey + ".ExpirationDate", System.Globalization.CultureInfo.CurrentCulture)).Trim()) == true)
		{
			return localizedText;
		}
		else
		{
			return "";
		}
    }


	
	public Boolean PublishContent(string publishDateString, string expirationDateString)
	{
		
		// HANDLE DATES FOR NAVIGATION MENU INCLUSION
		DateTime CurrentDate = DateTime.Now;
		DateTime publishDate = new DateTime();
		DateTime expirationDate = new DateTime();

		// SET PUBLISH DATE, IF STRING IS EMPTY OR CAN'T BE CONVERTED TO DATE TIME SET TO MINIMUM DATE
		try
		{
			publishDate = Convert.ToDateTime(publishDateString);
		}
		catch
		{
			publishDate = Convert.ToDateTime(DateTime.MinValue);
		}

		// SET PUBLISH DATE, IF STRING IS EMPTY OR CAN'T BE CONVERTED TO DATE TIME SET TO MAXIMUM DATE, WATCH OUT FOR THE Y10K BUG! :-) 
		try
		{
			expirationDate = Convert.ToDateTime(expirationDateString);
		}
		catch
		{
			expirationDate = Convert.ToDateTime(DateTime.MaxValue);
		}
			
		if (CurrentDate.Ticks > publishDate.Ticks && CurrentDate.Ticks < expirationDate.Ticks)
		{
			return true;
		}
		else
		{
			return false;
		}
	}


}
