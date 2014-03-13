using System;
using System.Web;


public class Edition
{

	public string GetEdition(string CurrentFileName)
	{
		CurrentFileName = CurrentFileName.Replace("~/", "");
		CurrentFileName = HttpContext.Current.Server.MapPath(CurrentFileName);

		try 
        {
			//HttpContext.Current.Response.Write(CurrentFileName)
			DateTime editionDate = System.IO.File.GetLastWriteTime(CurrentFileName);
			string editionText = "Last Modified: " + editionDate.ToString();

			return editionText;
		} 
        catch
        {
			return "";
		}
	}

}
