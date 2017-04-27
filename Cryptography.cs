using System;
using System.Text;

namespace Framework.Extensions
{
	public class Cryptography
	{
		public string Encrypt(string stringToEncrypt, string salt)
		{
			//ENCRYPT THE SPECIFIED STRING USING SHA512 USING THE SPECIFIED SALT
			UTF8Encoding utf8Encoder = new UTF8Encoding();
			byte[] hashedString = null;

			System.Security.Cryptography.HMACSHA512 cryptoService = new System.Security.Cryptography.HMACSHA512();
			hashedString = cryptoService.ComputeHash(utf8Encoder.GetBytes(salt.Trim() + stringToEncrypt.Trim()));

			return Convert.ToBase64String(hashedString).Trim();
		}
	}
}
