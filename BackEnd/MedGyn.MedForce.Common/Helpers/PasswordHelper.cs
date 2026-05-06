using System.Security.Cryptography;
using System.Text;

namespace MedGyn.MedForce.Common.Helpers
{
	public static class PasswordHelper
	{
		public static void HashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512())
			{
				passwordSalt = hmac.Key;
				passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
			}
		}

		public static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
		{
			if (password == null)
			{
				return false;
			}

			if (storedSalt != null && storedHash != null)
			{
				using (var hmac = new HMACSHA512(storedSalt))
				{
					var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
					for (int i = 0; i < passwordHash.Length; i++)
					{
						if (passwordHash[i] != storedHash[i])
						{
							return false;
						}
					}
				}
				return true;
			}

			return false;
		}

		public static string GenerateRandomPassword(int length)
		{
			string chars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
			using (var random = new RNGCryptoServiceProvider())
			{
				byte[] data = new byte[length];

				byte[] buffer = null;

				// Maximum random number that can be used without introducing a bias
				int maxRandom = byte.MaxValue - ((byte.MaxValue + 1) % chars.Length);

				random.GetBytes(data);

				char[] result = new char[length];

				for (int i = 0; i < length; i++)
				{
					byte value = data[i];

					while (value > maxRandom)
					{
						if (buffer == null)
						{
							buffer = new byte[1];
						}

						random.GetBytes(buffer);
						value = buffer[0];
					}

					result[i] = chars[value % chars.Length];
				}

				return new string(result);
			}
		}
	}
}
