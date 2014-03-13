using framework.StrikeIron;
using System;
using System.Collections.Generic;
using System.Web;

public class AddressVerification : IDisposable
{

	private NorthAmericanAddressVerificationServiceSoapClient client = new NorthAmericanAddressVerificationServiceSoapClient();
	private String addressStreet01;
	private String addressStreet02;
	private String addressCity;
	private String addressRegion; // STATE OR PROVINCE
	private String addressCountry; 
	private String addressPostalCode; // NOT CALLED ZIP CODE, BECAUSE "ZIP" IS US-SPECIFIC
	private String addressUrbanization; // USED IN PUERTO RICO
	private String firm;
	private String casing; // WE WILL ASSUME UPPER UNLESS SPECIFIED

	public Dictionary<String, String> resultCollection = new Dictionary<String, String>();



	#region "Get / Set Parameters"

		public String AddressStreet01
		{
			get
			{
				return addressStreet01;
			}
			set
			{
				addressStreet01 = value;
			}
		}


		public String AddressStreet02
		{
			get
			{
				return addressStreet02;
			}
			set
			{
				addressStreet02 = value;
			}
		}

		public String AddressCity
		{
			get
			{
				return addressCity;
			}
			set
			{
				addressCity = value;
			}
		}

		public String AddressRegion
		{
			get
			{
				return addressRegion;
			}
			set
			{
				addressRegion = value;
			}
		}


		public String AddressCountry
		{
			get
			{
				return addressCountry;
			}
			set
			{
				addressCountry = value;
			}
		}


		public String AddressPostalCode
		{
			get
			{
				return addressPostalCode;
			}
			set
			{
				addressPostalCode = value;
			}
		}


		public String AddressUrbanization
		{
			get
			{
				return addressUrbanization;
			}
			set
			{
				addressUrbanization = value;
			}
		}


		public String Firm
		{
			get
			{
				return firm;
			}
			set
			{
				firm = value;
			}
		}


		public String Casing
		{
			get
			{
				return casing;
			}
			set
			{
				casing = value;
			}
		}

	#endregion


		
	public Dictionary<String, String> GetVerifiedNorthAmericanAddress() // RETURNS A DICTIONARY OF THE RESULTS FROM STRIKEIRON
	{
		// SETUP SOME GENERAL SETTINGS
		SIWsOutputOfNorthAmericanAddress Output = new SIWsOutputOfNorthAmericanAddress();
		LicenseInfo license = new LicenseInfo();
		RegisteredUser user = new RegisteredUser();
		user.UserID = System.Configuration.ConfigurationManager.AppSettings["strikeiron_username"];
		user.Password = System.Configuration.ConfigurationManager.AppSettings["strikeiron_password"];
		license.RegisteredUser = user;
		SubscriptionInfo subscriptionCheck = new SubscriptionInfo();
		String status = "";

		// MAKE SURE RESULT COLLECTION IS CLEAR
		resultCollection.Clear();
		
		// HANDLE CASING
		CasingEnum casingEnumeration = new CasingEnum();
		if (String.IsNullOrEmpty(casing))
		{
			casing = "UPPER";
		}

		switch (casing.Trim().ToUpper())
		{
			case "PROPER":
			{
				casingEnumeration = CasingEnum.PROPER;
				break;
			}
			case "LOWER":
			{
				casingEnumeration = CasingEnum.LOWER;
				break;
			}
			default:
			{
				casingEnumeration = CasingEnum.UPPER;
				break;
			}
		}
		

		// LET'S TRY TO CHECK THE ADDRESS
		try
		{
			subscriptionCheck = client.GetRemainingHits(license);
			if (subscriptionCheck.RemainingHits > 10) // LET'S NOT DO THIS IF WE ARE GOING TO GET CHARGED FOR OVERAGES
			{
				if (String.IsNullOrEmpty(addressCountry))
				{
					addressCountry = "US";
				}
				addressCountry = addressCountry.Trim().ToUpper();

				// STRIKEIRON USES TWO DISTINCT OBJECTS FOR UNITED STATES VS. CANADIAN ADDRESSES, SO WHAT MAY LOOK LIKE A DUPLICATE SET OF CODE IS NECESSARY
				// STRIKEIRON DID NOT MAKE THE ADDRESS OBJECTS ENUMERABLE, SO WE CAN'T JUST LOOP THROUGH THEM -- WE HAVE TO SET THEM ONE BY ONE
				if (addressCountry.StartsWith("C") == true)
				{
					SubscriptionInfo subscription = client.NorthAmericanAddressVerification(license, addressStreet01.Trim(), addressStreet02.Trim(), (addressCity + " " + addressRegion + " " + addressPostalCode).Trim().Replace(",", " "), CountryCode.CA, firm.Trim(), addressUrbanization, casingEnumeration, out Output);
					CanadaAddress result = Output.ServiceResult.CanadaAddress; // LET'S SHORTEN THE BULK OF OUR ENTRIES

					resultCollection.Add("LicenseStatusCode", subscription.LicenseStatusCode.ToString());  // 
					resultCollection.Add("LicenseStatus", subscription.LicenseStatus.ToString());  // 
					resultCollection.Add("LicenseActionCode", subscription.LicenseActionCode.ToString());  // 
					resultCollection.Add("LicenseAction", subscription.LicenseAction.ToString());  // 
					resultCollection.Add("RemainingHits", subscription.RemainingHits.ToString());  // 
					resultCollection.Add("Amount", subscription.Amount.ToString());  // 
					resultCollection.Add("StatusNbr", Output.ServiceStatus.StatusNbr.ToString());  // 
					if (Output.ServiceStatus.StatusNbr < 300)
					{
						resultCollection.Add("StatusDescription", Output.ServiceStatus.StatusDescription);  // 
						resultCollection.Add("Firm", result.Firm);  // 
						resultCollection.Add("AddressLine1", result.AddressLine1);  // 
						resultCollection.Add("AddressLine2", result.AddressLine2);  // 
						resultCollection.Add("StreetNumber", result.StreetNumber);  // 
						resultCollection.Add("PreDirection", result.PreDirection);  // 
						resultCollection.Add("StreetName", result.StreetName);  // 
						resultCollection.Add("StreetType", result.StreetType);  // 
						resultCollection.Add("PostDirection", result.PostDirection);  // 
						resultCollection.Add("Extension", result.Extension);  // 
						resultCollection.Add("ExtensionNumber", result.ExtensionNumber);  // 
						resultCollection.Add("Village", result.Village);  // 
						resultCollection.Add("City", result.City);  // 
						resultCollection.Add("State", "");  // US ADDRESSES ONLY
						resultCollection.Add("Urbanization", "");  // US (PUERTO RICO) ADDRESSES ONLY
						resultCollection.Add("ZIPPlus4", "");  // US ADDRESSES ONLY
						resultCollection.Add("ZIPCode", "");  // US ADDRESSES ONLY
						resultCollection.Add("ZIPAddOn", "");  // US ADDRESSES ONLY
						try
						{
							resultCollection.Add("Latitude", result.GeoCode.Latitude.ToString());  // 
							resultCollection.Add("Longitude", result.GeoCode.Longitude.ToString());  // 
						}
						catch
						{
							resultCollection.Add("Latitude", "56.1303660");  // CENTER OF CANADA
							resultCollection.Add("Longitude", "-106.3467710");  // CENTER OF CANADA
						}
						resultCollection.Add("CarrierRoute", "");  // US ADDRESSES ONLY
						resultCollection.Add("PMB", "");  // US ADDRESSES ONLY
						resultCollection.Add("PMBDesignator", "");  // US ADDRESSES ONLY
						resultCollection.Add("DeliveryPoint", "");  // US ADDRESSES ONLY
						resultCollection.Add("DPCheckDigit", "");  // US ADDRESSES ONLY
						resultCollection.Add("LACS", "");  // US ADDRESSES ONLY
						resultCollection.Add("CMRA", "");  // US ADDRESSES ONLY
						resultCollection.Add("DPV", "");  //  US ADDRESSES ONLY
						resultCollection.Add("DPVFootnote", "");  // US ADDRESSES ONLY
						resultCollection.Add("RDI", "");  // US ADDRESSES ONLY
						resultCollection.Add("RecordType", "");  // US ADDRESSES ONLY
						resultCollection.Add("CongressDistrict", "");  // US ADDRESSES ONLY
						resultCollection.Add("County", "");  // US ADDRESSES ONLY
						resultCollection.Add("CountyNumber", "");  // US ADDRESSES ONLY
						resultCollection.Add("StateNumber", "");  // US ADDRESSES ONLY
						resultCollection.Add("CensusTract", "");  // US ADDRESSES ONLY
						resultCollection.Add("BlockNumber", "");  // US ADDRESSES ONLY 
						resultCollection.Add("BlockGroup", "");  // US ADDRESSES ONLY
						resultCollection.Add("PostalCode", result.PostalCode);  // 
						resultCollection.Add("Province", result.Province);  // 
						resultCollection.Add("CivicNumber", result.CivicNumber);  // 
						resultCollection.Add("CivicSuffix", result.CivicSuffix);  // 
						resultCollection.Add("DeliveryModeType", result.DeliveryModeType);  // 
						resultCollection.Add("DeliveryModeNumber", result.DeliveryModeNumber);  // 
						resultCollection.Add("DeliveryInstallationArea", result.DeliveryInstallationArea);  // 
						resultCollection.Add("DeliveryInstallationType", result.DeliveryInstallationType);  // 
						resultCollection.Add("DeliveryInstallationQualifier", result.DeliveryInstallationQualifier);  // 
						resultCollection.Add("UnifiedPostal", result.PostalCode); //
						resultCollection.Add("UnifiedRegion", result.Province); //
						resultCollection.Add("Country", "CA"); //
						resultCollection.Add("AddressStatus", result.AddressStatus);  // 
						status = "VALID";
					}
					else
					{
						status = Output.ServiceStatus.StatusDescription;
					}
				}
				else
				{
					SubscriptionInfo subscription = client.NorthAmericanAddressVerification(license, addressStreet01, addressStreet02, (addressCity + " " + addressRegion + " " + addressPostalCode).Trim().Replace(",", " "), CountryCode.US, firm, addressUrbanization, casingEnumeration, out Output);
					USAddress result = Output.ServiceResult.USAddress; // LET'S SHORTEN THE BULK OF OUR ENTRIES

					resultCollection.Add("LicenseStatusCode", subscription.LicenseStatusCode.ToString());  // 
					resultCollection.Add("LicenseStatus", subscription.LicenseStatus.ToString());  // 
					resultCollection.Add("LicenseActionCode", subscription.LicenseActionCode.ToString());  // 
					resultCollection.Add("LicenseAction", subscription.LicenseAction.ToString());  // 
					resultCollection.Add("RemainingHits", subscription.RemainingHits.ToString());  // 
					resultCollection.Add("Amount", subscription.Amount.ToString());  // 
					resultCollection.Add("StatusNbr", Output.ServiceStatus.StatusNbr.ToString());  // 
					resultCollection.Add("StatusDescription", Output.ServiceStatus.StatusDescription);  // 
					if (Output.ServiceStatus.StatusNbr < 300)
					{
						resultCollection.Add("Firm", result.Firm);  // 
						resultCollection.Add("AddressLine1", result.AddressLine1);  // 
						resultCollection.Add("AddressLine2", result.AddressLine2);  // 
						resultCollection.Add("StreetNumber", result.StreetNumber);  // 
						resultCollection.Add("PreDirection", result.PreDirection);  // 
						resultCollection.Add("StreetName", result.StreetName);  // 
						resultCollection.Add("StreetType", result.StreetType);  // 
						resultCollection.Add("PostDirection", result.PostDirection);  // 
						resultCollection.Add("Extension", result.Extension);  // 
						resultCollection.Add("ExtensionNumber", result.ExtensionNumber);  // 
						resultCollection.Add("Village", result.Village);  // 
						resultCollection.Add("City", result.City);  // 
						resultCollection.Add("State", result.State);  // 
						resultCollection.Add("Urbanization", result.Urbanization);  // 
						resultCollection.Add("ZIPPlus4", result.ZIPPlus4);  // 
						resultCollection.Add("ZIPCode", result.ZIPCode);  // 
						resultCollection.Add("ZIPAddOn", result.ZIPAddOn);  // 
						try
						{
							resultCollection.Add("Latitude", result.GeoCode.Latitude.ToString());  // 
							resultCollection.Add("Longitude", result.GeoCode.Longitude.ToString());  // 
						}
						catch
						{
							resultCollection.Add("Latitude", "37.0902400");  // CENTER OF THE UNITED STATES
							resultCollection.Add("Longitude", "-95.7128910");  // CENTER OF THE UNITED STATES
						}
						resultCollection.Add("CarrierRoute", result.CarrierRoute);  // 
						resultCollection.Add("PMB", result.PMB);  // 
						resultCollection.Add("PMBDesignator", result.PMBDesignator);  // 
						resultCollection.Add("DeliveryPoint", result.DeliveryPoint);  // 
						resultCollection.Add("DPCheckDigit", result.DPCheckDigit);  // 
						resultCollection.Add("LACS", result.LACS);  // 
						resultCollection.Add("CMRA", result.CMRA);  // 
						resultCollection.Add("DPV", result.DPV);  // 
						resultCollection.Add("DPVFootnote", result.DPVFootnote);  // 
						resultCollection.Add("RDI", result.RDI);  // 
						resultCollection.Add("RecordType", result.RecordType);  // 
						resultCollection.Add("CongressDistrict", result.CongressDistrict);  // 
						resultCollection.Add("County", result.County);  // 
						resultCollection.Add("CountyNumber", result.CountyNumber);  // 
						resultCollection.Add("StateNumber", result.StateNumber);  // 
						resultCollection.Add("CensusTract", result.GeoCode.CensusTract);  // 
						resultCollection.Add("BlockNumber", result.GeoCode.BlockNumber);  // 
						resultCollection.Add("BlockGroup", result.GeoCode.BlockGroup);  // 
						resultCollection.Add("PostalCode", "");  // CANADA ADDRESSES ONLY
						resultCollection.Add("Province", "");  // CANADA ADDRESSES ONLY
						resultCollection.Add("CivicNumber", "");  // CANADA ADDRESSES ONLY
						resultCollection.Add("CivicSuffix", "");  // CANADA ADDRESSES ONLY
						resultCollection.Add("DeliveryModeType", "");  // CANADA ADDRESSES ONLY
						resultCollection.Add("DeliveryModeNumber", "");  // CANADA ADDRESSES ONLY
						resultCollection.Add("DeliveryInstallationArea", "");  // CANADA ADDRESSES ONLY
						resultCollection.Add("DeliveryInstallationType", "");  // CANADA ADDRESSES ONLY
						resultCollection.Add("DeliveryInstallationQualifier", "");  // CANADA ADDRESSES ONLY
						resultCollection.Add("UnifiedPostal", result.ZIPPlus4); //
						resultCollection.Add("UnifiedRegion", result.State); //
						resultCollection.Add("Country", "US"); //
						resultCollection.Add("AddressStatus", result.AddressStatus);  // 
						status = "VALID";
					}
					else
					{
						status = Output.ServiceStatus.StatusDescription;
					}
				}
			}
		}
		catch (Exception ex)
		{
			status = ex.ToString();
		}
		if (status != "VALID") 
		{
			// SOMETHING BAD HAPPENED, I SHOULDN'T BE IN HERE!
			resultCollection.Clear();
			resultCollection.Add("StatusNbr", "1000");
			resultCollection.Add("RemainingHits", subscriptionCheck.RemainingHits.ToString());
			resultCollection.Add("StatusDescription", status);
		}
		return resultCollection;
	}



	#region "Handle: Session"

		public Dictionary<String, String> GetCurrentAddressCollection()
		{
			Dictionary<String, String> AddressCollection = new Dictionary<String, String>();
			if (HttpContext.Current.Session["AddressVerificationCollection"] != null)
			{
				AddressCollection = (Dictionary<String, String>)HttpContext.Current.Session["AddressVerificationCollection"];
			}
			return AddressCollection;
		}


		public void SetCurrentAddressCollection(Dictionary<String, String> AddressCollection)
		{
			try
			{
				HttpContext.Current.Session.Remove("AddressVerificationCollection");
			}
			catch
			{
				// SUPPRESS ERRORS
			}
			HttpContext.Current.Session.Add("AddressVerificationCollection", AddressCollection);
		}


		public void RemoveCurrentAddressCollection()
		{
			try
			{
				HttpContext.Current.Session.Remove("AddressVerificationCollection");
			}
			catch
			{
				// SUPPRESS ERRORS
			}
		}

	#endregion



	#region "Dispose"

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~AddressVerification()
		{
			Dispose(false);
		}

		protected virtual void Dispose(Boolean disposing)
		{
			if (disposing == true)
			{
				client.Close();
				client = null;
			}
		}

	#endregion

}
