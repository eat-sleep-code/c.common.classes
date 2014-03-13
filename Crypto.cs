using System;
using System.Text;

public class Crypto
{
	public string encrypt(string stringToEncrypt, string salt)
	{
		//// ENCRYPT THE SPECIFIED STRING USING SHA512 USING THE SPECIFIED SALT
		UTF8Encoding utf8Encoder = new UTF8Encoding();
		byte[] hashedString = null;

		try 
        {
			System.Security.Cryptography.SHA512CryptoServiceProvider cryptoService = new System.Security.Cryptography.SHA512CryptoServiceProvider();
			hashedString = cryptoService.ComputeHash(utf8Encoder.GetBytes(salt.Trim() + stringToEncrypt.Trim()));
		} 
        catch 
        {
			System.Security.Cryptography.SHA512Managed cryptoService = new System.Security.Cryptography.SHA512Managed();
			hashedString = cryptoService.ComputeHash(utf8Encoder.GetBytes(salt.Trim() + stringToEncrypt.Trim()));
		}

		return Convert.ToBase64String(hashedString).Trim();
	}

}
