using Framework.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Framework.Extensions
{
	public class GalleryRendering
	{
		Localization localization = new Localization();

		public string RenderGallery(string galleryID)
		{
			IQueryable<GalleryImage> galleryContents = GetGalleryContents(galleryID);
			string gallery = string.Empty;
			//RENDER GALLERY
			gallery += "<div class=\"connected-carousels\"><div class=\"stage\"><div class=\"carousel carousel-stage\" data-jcarousel=\"true\">" + Environment.NewLine;
			gallery += "<ul itemscope itemtype=\"http://schema.org/ImageGallery\">" + Environment.NewLine;
			foreach (var image in galleryContents)
			{
				gallery += "<li><span class=\"helper\"></span><a href=\"" + image.UrlFull+ "\" itemprop=\"url\"><img src=\"" + Startup.CDNUrl + image.UrlFull + "\"  alt=\"" + image.AlternateText + "\"  itemprop=\"image\"></a><span itemprop=\"name\"></span></li>" + Environment.NewLine;
			}
			gallery += "</ul>" + Environment.NewLine;
			gallery += "</div>" + Environment.NewLine;
			gallery += "<a class=\"prev prev-stage inactive\" href=\"#\" data-jcarouselcontrol=\"true\"><span>‹</span></a><a class=\"next next-stage\" href=\"#\" data-jcarouselcontrol=\"true\"><span>›</span></a>" + Environment.NewLine;
			gallery += "</div>" + Environment.NewLine;
			gallery += "<div class=\"navigation\"> " + Environment.NewLine;
			gallery += "<a class=\"prev prev-navigation inactive\" href=\"#\" data-jcarouselcontrol=\"true\">‹</a><a class=\"next next-navigation\" href=\"#\" data-jcarouselcontrol=\"true\">›</a>" + Environment.NewLine;
			gallery += "<div class=\"carousel carousel-navigation\" data-jcarousel=\"true\">" + Environment.NewLine;
			gallery += "<ul>" + Environment.NewLine;
			foreach (var image in galleryContents)
			{
				gallery += "<li><span class=\"helper\"></span><img src=\"" + image.UrlFull + "\"  alt=\"" + image.AlternateText + "\" data-pin-no-hover=\"true\"></li>" + Environment.NewLine;
			}
			gallery += "</ul>" + Environment.NewLine;
			gallery += "</div></div></div>" + Environment.NewLine;
			return gallery;
		}

		private IQueryable<GalleryImage> GetGalleryContents(string galleryID)
		{
			string fileContents = File.ReadAllText(Startup.WebRootPath + "/images/gallery/" + galleryID + "/config.json");
			List<GalleryImage> galleryImageList = JsonConvert.DeserializeObject<List<GalleryImage>>(fileContents);
			return galleryImageList.AsQueryable();
		}
	}
}
