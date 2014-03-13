using System;
using System.Configuration;
using System.Net.Configuration;
using System.Net.Mail;
using System.Web.Configuration;

public class Mail
{

	public string Send(string recipient = "", string sender = "", string body = "", string subject = "", string cc = "", string bcc = "", string replyTo = "", string priority = "Normal", string smtpServer = "", int smtpPort = 25)
	{
		//// == GET PRECONFIGURED SETTINGS == 
		string configuredSender = "";
		string configuredHost = "";
		int configuredPort = 25;
		string configuredUsername = "";
		string configuredPassword = "";

		try 
        {
			Configuration webConfigFile = WebConfigurationManager.OpenWebConfiguration("~");
			MailSettingsSectionGroup mailSettings = (MailSettingsSectionGroup)webConfigFile.GetSectionGroup("system.net/mailSettings");
			if ((mailSettings != null)) {
				configuredSender = mailSettings.Smtp.From;
				configuredHost = mailSettings.Smtp.Network.Host;
				configuredPort = mailSettings.Smtp.Network.Port;
				configuredUsername = mailSettings.Smtp.Network.UserName;
				configuredPassword = mailSettings.Smtp.Network.Password;
			}
		} 
        catch //(Exception ex) 
        {
			//// IN DEFAULT MEDIUM TRUST ENVIRONMENTS (SUCH AS GODADDY) THE ABOVE MAY FAIL, LETS GO LOOK FOR AppSettings VALUES IN THE WEB.CONFIG - NOT IDEAL, BUT CurrentLY A LIMITATION OF ASP.NET FRAMEWORK
			configuredSender = WebConfigurationManager.AppSettings["mail_from"];
			configuredHost = WebConfigurationManager.AppSettings["mail_host"];
			configuredPort = Convert.ToInt32(WebConfigurationManager.AppSettings["mail_port"]);
			configuredUsername = WebConfigurationManager.AppSettings["mail_username"];
			configuredPassword = WebConfigurationManager.AppSettings["mail_password"];
		}

		//// == CREATE MAIL OBJECTS ==
		MailMessage mailMessage = new MailMessage();
		SmtpClient mailClient = new SmtpClient();
		System.Net.NetworkCredential mailCredentials = new System.Net.NetworkCredential();

		//// === SET THE SENDER & REPLY TO ADDRESSES === 
		if (sender.Trim().Length > 0) 
        {
			mailMessage.From = new MailAddress(sender.Trim());
			if (replyTo.Trim().Length > 0) 
            {
				mailMessage.ReplyToList.Add(new MailAddress(replyTo.Trim()));
			} 
            else 
            {
				mailMessage.ReplyToList.Add(new MailAddress(sender.Trim()));
			}
		} 
        else 
        {
			mailMessage.From = new MailAddress(configuredSender.Trim());
			if (replyTo.Trim().Length > 0) 
            {
				mailMessage.ReplyToList.Add(new MailAddress(replyTo.Trim()));
			} 
            else 
            {
				mailMessage.ReplyToList.Add(new MailAddress(configuredSender.Trim()));
			}
		}

		//// === SET THE RECIPIENTS ===
		if (recipient.Trim().Length > 0) 
        {
			foreach (string recipientAddress in recipient.Trim().Split(';')) 
            {
				if (recipientAddress.Trim().Length >= 5) 
                {
					mailMessage.To.Add(recipientAddress.Trim());
				}
			}
		} 
        else 
        {
			mailMessage.To.Add(new MailAddress(configuredSender.Trim()));
		}

		foreach (string ccAddress in cc.Trim().Split(';')) 
        {
			if (ccAddress.Trim().Length >= 5) 
            {
				mailMessage.CC.Add(ccAddress.Trim());
			}
		}

		foreach (string bccAddress in bcc.Trim().Split(';')) 
        {
			if (bccAddress.Trim().Length >= 5) 
            {
				mailMessage.Bcc.Add(bccAddress.Trim());
			}
		}

		//// === SET THE SUBJECT ===
		mailMessage.Subject = subject.Trim();

		//// === SET THE MESSAGE BODY TEXT ===
		mailMessage.Body = body.Trim();

		//// === SET PRIORITY OF MESSAGE (DEFAULT IS NORMAL) ===
		if (priority.Trim() == "High") 
        {
			mailMessage.Priority = MailPriority.High;
		} 
        else if (priority.Trim() == "Low") 
        {
			mailMessage.Priority = MailPriority.Low;
		} 
        else 
        {
			mailMessage.Priority = MailPriority.Normal;
		}

		//// === SET FORMAT OF THE MAIL ===
		mailMessage.IsBodyHtml = true;

		//// === SET THE SMTP SERVER ===
		if (smtpServer.Trim().Length > 0) 
        {
			mailClient.Host = smtpServer.Trim();
		} 
        else 
        {
			mailClient.Host = configuredHost.Trim();
		}

		//// === SET THE SMTP SERVER PORT ===
		if (smtpPort > 0) 
        {
			mailClient.Port = smtpPort;
		} 
        else 
        {
			mailClient.Port = configuredPort;
		}

		if ((configuredUsername != null) & (configuredPassword != null)) 
        {
			if (configuredUsername.Trim().Length > 0) 
            {
				mailCredentials.UserName = configuredUsername;
				mailCredentials.Password = configuredPassword;
				mailClient.Credentials = mailCredentials;
			}
		}

		//// === SEND THE MESSAGE, RETURN ANY ERRORS ===
		try 
        {
			mailClient.Send(mailMessage);
			return "";
		} 
        catch (Exception mail_exception) 
        {
			return mail_exception.ToString();
		}

	}

}
