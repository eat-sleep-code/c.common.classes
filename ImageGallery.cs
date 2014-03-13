using System;

public class ImageGallery
{
	public string RenderGalleryScripts(String elementID)
	{
		String gallery = "";
		//// RENDER JQUERY PLUGIN AND CONFIGURATION	
		gallery += "<script src=\"/scripts/jquery.jcarousel.min.js\" type=\"text/javascript\"></script>" + Environment.NewLine;
		gallery += "<script src=\"/scripts/jquery.pikachoose.min.js\" type=\"text/javascript\"></script>" + Environment.NewLine;
		gallery += "<script src=\"/scripts/jquery.touchwipe.min.js\" type=\"text/javascript\"></script>" + Environment.NewLine;
		gallery += "<script type=\"text/javascript\">" + Environment.NewLine;
		gallery += "$(document).ready(" + Environment.NewLine;
		gallery += "function (){" + Environment.NewLine;
		gallery += "var preventStageHoverEffect = function(self){" + Environment.NewLine;
		gallery += "self.wrap.unbind('mouseenter').unbind('mouseleave');" + Environment.NewLine;
		gallery += "self.imgNav.append('<a class=\"tray\">');" + Environment.NewLine;
		gallery += "self.imgNav.show();" + Environment.NewLine;
		gallery += "self.hiddenTray = true;" + Environment.NewLine;
		gallery += "self.imgNav.find('.tray').bind('click',function(){" + Environment.NewLine;
		gallery += "if(self.hiddenTray){" + Environment.NewLine;
		gallery += "self.list.parents('.jcarousel-container').animate({height:\"80px\"});" + Environment.NewLine;
		gallery += "}else{" + Environment.NewLine;
		gallery += "self.list.parents('.jcarousel-container').animate({height:\"1px\"});" + Environment.NewLine;
		gallery += "}" + Environment.NewLine;
		gallery += "self.hiddenTray = !self.hiddenTray;" + Environment.NewLine;
		gallery += "});" + Environment.NewLine;
		gallery += "}" + Environment.NewLine;
		gallery += "$(\"#" + elementID + "\").PikaChoose({bindsFinished: preventStageHoverEffect, carousel:true});" + Environment.NewLine;
		gallery += "});" + Environment.NewLine;
		gallery += "</script>" + Environment.NewLine;

		return gallery;

	}
}
