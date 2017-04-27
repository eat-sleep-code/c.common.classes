using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;

namespace Framework.Extensions
{
	public class Context
	{
		public HttpContext Current { get; set; }
		public Uri AbsoluteUri { get; set; }
		public string AbsolutePath { get; set; }
		public string Url { get; set; }
		public CultureInfo Culture { get; set; }

		private static IOptions<RequestLocalizationOptions> RequestLocalizationOptions;
		public static void ConfigureLocalizationOptions(IOptions<RequestLocalizationOptions> requestLocalizationOptions)
		{
			RequestLocalizationOptions = requestLocalizationOptions;
		}

		private static IHttpContextAccessor HttpContextAccessor;
		public static void ConfigureHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
		{
			HttpContextAccessor = httpContextAccessor;
		}

		public Context()
		{
			Current = HttpContextAccessor.HttpContext;
			AbsoluteUri = GetAbsoluteUri();
			AbsolutePath = GetAbsoluteUri().AbsolutePath;
			Url = GetAbsoluteUri().ToString();
			Culture = GetCultureInfo(HttpContextAccessor.HttpContext);
		}

		private Uri GetAbsoluteUri()
		{
			UriBuilder uriBuilder = new UriBuilder();
			uriBuilder.Scheme = Current.Request.Scheme;
			uriBuilder.Host = Current.Request.Host.Host.ToString();
			if (Current.Request.Host.Port != null)
			{
				uriBuilder.Port = Current.Request.Host.Port ?? default(int);
			}
			uriBuilder.Path = Current.Request.Path.ToString();
			uriBuilder.Query = Current.Request.QueryString.ToString();
			Uri absoluteUri = uriBuilder.Uri;
			return uriBuilder.Uri;
		}

		private CultureInfo GetCultureInfo(HttpContext httpContext) {
			var requestCultureFeature = httpContext.Features.Get<IRequestCultureFeature>();
			return requestCultureFeature.RequestCulture.Culture;
		}
	}
}
