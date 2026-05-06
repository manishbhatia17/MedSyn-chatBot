using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace MedGyn.MedForce.Common.Helpers
{
	public static class EncryptionHelper
	{
		// https://xkpasswd.net/s/
		private static string KEY = "||58;rush;FANTASTIC;escaped;55||"; // must be 32 characters
		private static string IV = "!MANY-lake-land!"; // must be 16 characters

		public static string EncryptString(string value)
		{
			if(value.IsNullOrEmpty()) {
				return null;
			}

			var key = Encoding.UTF8.GetBytes(KEY);
			var iv = Encoding.UTF8.GetBytes(IV);
			byte[] encrypted;

			using (var aes = Aes.Create())
			using (var encryptor = aes.CreateEncryptor(key, iv))
			using (var msEncrypt = new MemoryStream())
			using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
			{
				using (var swEncrypt = new StreamWriter(csEncrypt))
				{
					swEncrypt.Write(value);
				}
				encrypted = msEncrypt.ToArray();
			}

			return Convert.ToBase64String(encrypted);
		}

		public static string DecryptString(string value)
		{
			if(value.IsNullOrEmpty()) {
				return null;
			}

			var key = Encoding.UTF8.GetBytes(KEY);
			var iv = Encoding.UTF8.GetBytes(IV);
			var encrypted = Convert.FromBase64String(value);
			string decrypted;

			using (var aes = Aes.Create())
			using (var decryptor = aes.CreateDecryptor(key, iv))
			using (var msDecrypt = new MemoryStream(encrypted))
			using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
			using (var srEncrypt = new StreamReader(csDecrypt))
			{
				decrypted = srEncrypt.ReadToEnd();
			}

			return decrypted;
		}
	}
}
