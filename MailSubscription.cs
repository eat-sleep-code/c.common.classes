using framework.ExactTarget;
using System;
using System.Collections.Generic;
using System.Web;


public class MailSubscription: IDisposable
{
	// FOR ANY OF THIS TO WORK, YOU MUST CREATE A "SERVICE REFERENCE" TO: https://webservice.exacttarget.com/etframework.wsdl 
	// YOU MUST ALSO ENSURE BINDING FOR SERVICE REFERENCE IN THE WEB.CONFIG HAS SECURITY MODE SET TO "TransportWithMessageCredential"

	private SoapClient client = new SoapClient();



	// === CLEARS SESSION VARIABLES USED WHEN ADDING/UPDATING SUBSCRIBERS =========================================================================================================
	public void initialize()
	{
		try
		{
			HttpContext.Current.Session.Remove("attributeCollection");
		}
		catch
		{
			// SUPPRESS ERROR
		}
	}



	// === ADDS/UPDATES SPECIFIED SUBSCRIBER TO A SPECIFIED LIST OR LISTS =========================================================================================================
	public String AddSubscriber(String email = "", String list = "", String service = "exacttarget")
	{
		if (service.Trim().ToLower() == "exacttarget")
		{
			client.ClientCredentials.UserName.UserName = System.Configuration.ConfigurationManager.AppSettings["exacttarget_username"];
			client.ClientCredentials.UserName.Password = System.Configuration.ConfigurationManager.AppSettings["exacttarget_password"];

			try
			{
				//String str_guid = System.Guid.NewGuid().ToString();
				Subscriber sub = new Subscriber();
				sub.SubscriberKey = email.Trim();
				sub.EmailAddress = email.Trim();

				if (list.Trim().Length > 0)
				{
					sub.Lists = new SubscriberList[1];
					String[] listArray = list.Split(';');
					
					Int32 listCount = 0;
					foreach (String list_id in listArray)
					{
						if (list_id.Trim().Length > 0)
						{
							sub.Lists[listCount] = new SubscriberList();
							sub.Lists[listCount].ID = Convert.ToInt32(list_id);
							sub.Lists[listCount].IDSpecified = true;
							listCount = listCount + 1;
						}
					}
				}

				if (HttpContext.Current.Session["attributeCollection"] != null)
				{
					Dictionary<String, String> attributeCollection = (Dictionary<String, String>) HttpContext.Current.Session["attributeCollection"];
					sub.Attributes = new framework.ExactTarget.Attribute[attributeCollection.Count];
					Int32 attributeCount = 0;
					foreach (KeyValuePair<String, String> attribute in attributeCollection)
					{
						sub.Attributes[attributeCount] = new framework.ExactTarget.Attribute();
						sub.Attributes[attributeCount].Name = attribute.Key;
						sub.Attributes[attributeCount].Value = attribute.Value;
						attributeCount = attributeCount + 1;
					}
				}

				CreateOptions createOptions = new CreateOptions();
				createOptions.SaveOptions = new SaveOption[1];
				createOptions.SaveOptions[0] = new SaveOption();
				createOptions.SaveOptions[0].SaveAction = SaveAction.UpdateAdd;
				createOptions.SaveOptions[0].PropertyName = "*";

				string clientRequestID = String.Empty;
				string clientStatus = String.Empty;
				try
				{
					CreateResult[] clientResults = client.Create(createOptions, new APIObject[] { sub }, out clientRequestID, out clientStatus);
				}
				catch (Exception ex)
				{
					return ex.ToString();
				}
				try
				{
					HttpContext.Current.Session.Remove("attributeCollection");
				}
				catch
				{
					// SUPPRESS ERROR
				}
			}
			catch (Exception ex)
			{
				return ex.ToString();
			}
		}
		return "";
	}

	



	// === ADDS PARAMETERS FOR USE BY AddSubscriber ==============================================================================================================================
	public void AddParameter(String key, String value = "")
	{
		Dictionary<String, String> attributeCollection = new Dictionary<String, String>();
		if (HttpContext.Current.Session["attributeCollection"] != null)
		{
			attributeCollection = (Dictionary<String, String>) HttpContext.Current.Session["attributeCollection"];
		}
		attributeCollection.Add(key, value);
		HttpContext.Current.Session["attributeCollection"] = attributeCollection;
	}





	// === REMOVES SPECIFIED SUBSCRIBER FROM A SPECIFIED LIST OR LISTS ============================================================================================================
	public String RemoveSubscriber(String email = "", String list = "", String service = "exacttarget")
	{
		if (service.Trim().ToLower() == "exacttarget")
		{
			client.ClientCredentials.UserName.UserName = System.Configuration.ConfigurationManager.AppSettings["exacttarget_username"];
			client.ClientCredentials.UserName.Password = System.Configuration.ConfigurationManager.AppSettings["exacttarget_password"];

			try
			{
				//String str_guid = System.Guid.NewGuid().ToString();
				Subscriber sub = new Subscriber();
				sub.SubscriberKey = email.Trim();
				sub.EmailAddress = email.Trim();

				if (list.Trim().Length > 0)
				{
					sub.Lists = new SubscriberList[1];
					String[] listArray = list.Split(';');

					Int32 listCount = 0;
					foreach (String list_id in listArray)
					{
						if (list_id.Trim().Length > 0)
						{
							sub.Lists[listCount] = new SubscriberList();
							sub.Lists[listCount].ID = Convert.ToInt32(list_id);
							sub.Lists[listCount].IDSpecified = true;
							listCount = listCount + 1;
						}
					}
				}

				CreateOptions createOptions = new CreateOptions();
				createOptions.SaveOptions = new SaveOption[1];
				createOptions.SaveOptions[0] = new SaveOption();
				createOptions.SaveOptions[0].SaveAction = SaveAction.Delete;
				createOptions.SaveOptions[0].PropertyName = "*";

				string clientRequestID = String.Empty;
				string clientStatus = String.Empty;
				try
				{
					CreateResult[] clientResults = client.Create(createOptions, new APIObject[] { sub }, out clientRequestID, out clientStatus);
				}
				catch (Exception ex)
				{
					return ex.ToString();
				}
			}
			catch (Exception ex)
			{
				return ex.ToString();
			}
		}
		return "";
	}





	// === REMOVES SPECIFIED SUBSCRIBER FROM ALL LISTS AND REMOVES USER FROM SYSTEM ===============================================================================================
	public String PurgeSubscriber(String email = "", String list = "", String service = "exacttarget")
	{
		if (service.Trim().ToLower() == "exacttarget")
		{
			client.ClientCredentials.UserName.UserName = System.Configuration.ConfigurationManager.AppSettings["exacttarget_username"];
			client.ClientCredentials.UserName.Password = System.Configuration.ConfigurationManager.AppSettings["exacttarget_password"];

			try
			{
				Subscriber sub = new Subscriber();
				sub.SubscriberKey = email.Trim();

                string clientRequestID = String.Empty;
                string clientStatus = String.Empty;

				DeleteResult[] clientResults = client.Delete(new DeleteOptions(), new APIObject[] { sub }, out clientRequestID, out clientStatus);
			}
			catch (Exception ex)
			{
				return ex.ToString();
			}
		}

		return "";
	}





	// === RETURNS A DICTIONARY OBJECT CONTAINING LIST IDs AND SUBSCRIBER STATUS FOR A SPECIFIED USER =============================================================================
	public Dictionary<String, String> GetSubscriberStatus(String email = "", String service="exacttarget")
	{
		if (service.Trim().ToLower() == "exacttarget")
		{
			client.ClientCredentials.UserName.UserName = System.Configuration.ConfigurationManager.AppSettings["exacttarget_username"];
			client.ClientCredentials.UserName.Password = System.Configuration.ConfigurationManager.AppSettings["exacttarget_password"];

			Dictionary<String, String> listCollection = new Dictionary<String,String>();
				
			try
			{
				SimpleFilterPart simpleFilterPart = new SimpleFilterPart();
				simpleFilterPart.Property = "SubscriberKey";
				simpleFilterPart.SimpleOperator = SimpleOperators.equals;
				simpleFilterPart.Value = new string[] {email.Trim()};

				RetrieveRequest retrieveRequest = new RetrieveRequest();
				retrieveRequest.ObjectType = "ListSubscriber";
				retrieveRequest.Properties = new String[] { "ListID", "Status" };
				retrieveRequest.Filter = simpleFilterPart;

				APIObject[] results;
				string requestID;
				string status = client.Retrieve(retrieveRequest, out requestID, out results);

				for (int i = 0; i < results.Length; i++)
				{
					ListSubscriber ls = (ListSubscriber)results[i];
					listCollection.Add(ls.ListID.ToString(), ls.Status.ToString());
				}
				return listCollection;
			}
			catch //(Exception ex)
			{
				// SUPPRESS ERRORS
			}
		}
		return null;
	}



	// === RETURNS A DICTIONARY OBJECT CONTAINING ALL PROFILE ATTRIBUTES (AND RESPECTIVE VALUES) FOR A SPECIFIED USER =============================================================
	public Dictionary<String, String> GetSubscriberProfile(String email = "", String service = "exacttarget")
	{
		if (service.Trim().ToLower() == "exacttarget")
		{
			client.ClientCredentials.UserName.UserName = System.Configuration.ConfigurationManager.AppSettings["exacttarget_username"];
			client.ClientCredentials.UserName.Password = System.Configuration.ConfigurationManager.AppSettings["exacttarget_password"];

			Dictionary<String, String> profileCollection = new Dictionary<String, String>();

			try
			{
				SimpleFilterPart simpleFilterPart = new SimpleFilterPart();
				simpleFilterPart.Property = "SubscriberKey";
				simpleFilterPart.SimpleOperator = SimpleOperators.equals;
				simpleFilterPart.Value = new string[] { email.Trim() };

				RetrieveRequest retrieveRequest = new RetrieveRequest();
				retrieveRequest.ObjectType = "Subscriber";
				retrieveRequest.Properties = new String[] { "ID" };
				retrieveRequest.Filter = simpleFilterPart;

				APIObject[] results;
				string requestID;
				string status = client.Retrieve(retrieveRequest, out requestID, out results);
				foreach (Subscriber s in results)
				{
					foreach (framework.ExactTarget.Attribute att in s.Attributes)
					{
						profileCollection.Add(att.Name.ToString(), att.Value.ToString());
					}
				}
				return profileCollection;
			}
			catch //(Exception ex)
			{
				// SUPPRESS ERRORS
			}
		}
		return null;
	}


	// === RETURNS A DICTIONARY OBJECT CONTAINING ALL SUBSCRIBERS FOR THE SPECIFIED LIST ==========================================================================================
	public Dictionary<String, String> GetListSubscribers(Int32 list_id, String service = "exacttarget")
	{
		if (service.Trim().ToLower() == "exacttarget")
		{
			client.ClientCredentials.UserName.UserName = System.Configuration.ConfigurationManager.AppSettings["exacttarget_username"];
			client.ClientCredentials.UserName.Password = System.Configuration.ConfigurationManager.AppSettings["exacttarget_password"];

			Dictionary<String, String> subscriberCollection = new Dictionary<String, String>();

			try
			{
				SimpleFilterPart simpleFilterPart = new SimpleFilterPart();
				simpleFilterPart.Property = "ListID";
				simpleFilterPart.SimpleOperator = SimpleOperators.equals;
				simpleFilterPart.Value = new string[] { list_id.ToString() };

				RetrieveRequest retrieveRequest = new RetrieveRequest();
				retrieveRequest.ObjectType = "ListSubscriber";
				retrieveRequest.Properties = new string[] { "ListID", "SubscriberKey", "Status" };
				retrieveRequest.Filter = simpleFilterPart;

				APIObject[] results;
				string requestID;
				string status = client.Retrieve(retrieveRequest, out requestID, out results);

				// Iterate over the results
				Console.WriteLine("List Subscriber Details:\tList ID\tSubscriberKey\tStatus");
				for (int i = 0; i < results.Length; i++)
				{
					ListSubscriber ls = (ListSubscriber)results[i];
					subscriberCollection.Add(ls.SubscriberKey.ToString(), ls.Status.ToString());
				}
				return subscriberCollection;
			}
			catch //(Exception ex)
			{
				// SUPPRESS ERRORS
			}
		}
		return null;
	}



	#region "Dispose"

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~MailSubscription()
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





