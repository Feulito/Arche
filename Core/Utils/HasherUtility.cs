using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utils
{
    public static class HasherUtility
    {
        private static string Pepper { get; set; } = "FILE-SHARE-2021-m2";

        public static HashObject Hash(string pass)
        {
            byte[] salt = new byte[128 / 8];
            using (RandomNumberGenerator numberGenerator = RandomNumberGenerator.Create())
            {
                numberGenerator.GetBytes(salt);
            }

            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: pass + Pepper,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
                ));
            return new HashObject() { Salt = Convert.ToBase64String(salt), Hash = hashedPassword };
        }

        public static bool CheckHash(string plainText, string hashedText, string saltString)
        {
            byte[] salt = Convert.FromBase64String(saltString);

            string pass = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: plainText + Pepper,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
                ));

            return pass == hashedText;
        }
    }

    public struct HashObject
    {
        public string Salt { get; set; }
        public string Hash { get; set; }
    }

}
