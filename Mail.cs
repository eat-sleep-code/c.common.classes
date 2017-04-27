using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Framework.Extensions
{
	public class Mail
	{
		public string Result { get; private set; }
		public Options MailOptions { get; set; }

		public class Options
		{
			public string APIKey { get; private set; }
			public string DefaultFrom { get; private set; }
			public string From { get; set; }
			public string ReplyTo { get; set; }
			public string To { get; set; }
			public string CC { get; set; }
			public string BCC { get; set; }
			public string Subject { get; set; }
			public string Body { get; set; }
			public string Priority { get; set; }

			public Options()
			{
				APIKey = Startup.Configuration["Mail:APIKey"].Trim();
				DefaultFrom = Startup.Configuration["Mail:DefaultFrom"].Trim();
				From = string.Empty;
				ReplyTo = string.Empty;
				To = string.Empty;
				CC = string.Empty;
				BCC = string.Empty;
				Subject = string.Empty;
				Body = string.Empty;
				Priority = "Normal";
			}
		}

		public Mail()
		{
			MailOptions = new Options();
			Result = Send(MailOptions);
		}

		public string Send(Options options)
		{
			return SendAsync(options).Result;
		}

		private async Task<string> SendAsync(Options options)
		{
			//== CREATE MAIL OBJECTS ==
			SendGridClient apiClient = new SendGridClient(options.APIKey);
			SendGridMessage mailMessage = new SendGridMessage();

			mailMessage.SetFrom(new EmailAddress(string.IsNullOrWhiteSpace(options.From) ? options.DefaultFrom.Trim() : options.From.Trim()));
			mailMessage.SetReplyTo(new EmailAddress(string.IsNullOrWhiteSpace(options.ReplyTo) ? options.From.Trim() : options.ReplyTo.Trim()));


			// == SET THE RECIPIENTS ==
			List<EmailAddress> recipientAddressList = new List<EmailAddress>();
			foreach (string recipientAddress in options.To.Split(';').ToList())
			{
				if (!string.IsNullOrWhiteSpace(recipientAddress))
				{ 
					recipientAddressList.Add(new EmailAddress(recipientAddress.Trim()));
				}
			}
			if (recipientAddressList.Where(e => !string.IsNullOrWhiteSpace(e.Email)).ToList().Count > 0)
			{
				mailMessage.AddTos(recipientAddressList);
			}
			else
			{
				mailMessage.AddTo(new EmailAddress(options.DefaultFrom.Trim()));
			}


			List<EmailAddress> ccAddressList = new List<EmailAddress>();
			foreach (string ccAddress in options.CC.Split(';').ToList())
			{
				if (!string.IsNullOrWhiteSpace(ccAddress))
				{
					ccAddressList.Add(new EmailAddress(ccAddress.Trim()));
				}
			}
			if (ccAddressList.Where(e => !string.IsNullOrWhiteSpace(e.Email)).ToList().Count > 0)
			{
				mailMessage.AddCcs(ccAddressList);
			}


			List<EmailAddress> bccAddressList = new List<EmailAddress>();
			foreach (string bccAddress in options.BCC.Split(';').ToList())
			{
				if (!string.IsNullOrWhiteSpace(bccAddress))
				{
					bccAddressList.Add(new EmailAddress(bccAddress.Trim()));
				}
			}
			if (bccAddressList.Where(e => !string.IsNullOrWhiteSpace(e.Email)).ToList().Count > 0)
			{
				mailMessage.AddBccs(bccAddressList);
			}


			//=== SET THE PRIORITY OF MESSAGE (DEFAULT IS NORMAL) ===
			if (options.Priority.Trim().ToLower() == "high")
			{
				mailMessage.AddHeader("Priority", "Urgent");
				mailMessage.AddHeader("Importance", "High");
			}

			//=== SET THE SUBJECT ===
			mailMessage.Subject = options.Subject.Trim();

			//=== SET THE MESSAGE CONTENT ===
			mailMessage.AddContent("text/html", options.Body.Trim());

			Response response = await apiClient.SendEmailAsync(mailMessage);
			string result = response.Body.ReadAsStringAsync().Result;
			return result;
		}
	}
}
