using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Discount
{
    /// <summary>
    /// Static method containing all utilities
    /// </summary>
    static class Utilities
    {
        /// <summary>
        /// Method used to calculate the SHA256 hash of the input string
        /// </summary>
        /// <param name="input">Input to calculate hash from</param>
        /// <returns>The SHA256 hash value</returns>
        public static string CalculateSHA256Hash(string input)
        {
            // https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.sha256?view=netframework-4.8

            SHA256 mySHA256 = SHA256.Create();

            // Create an array of bytes from the string and calculate the SHA256 hash value 
            byte[] sessionBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = mySHA256.ComputeHash(sessionBytes);

            // Convert byte array back into string with a StringBuilder
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                // Format as Hexadecimal value (X2) as expected by the Shopgun API
                strBuilder.Append(hashBytes[i].ToString("X2"));
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Calculate the amount of Experience required for the next level increase 
        /// </summary>
        /// <param name="nextLevel">The target level</param>
        /// <returns>Required amount of Experience required to level up</returns>
        public static int CalculateRequiredExperienceForNextLevel(int nextLevel)
        {
            // return (a * x) ^ b
            int a = 22;
            double b = 1.6;
            int x = nextLevel - 1;

            return (int)Math.Floor(Math.Pow((a * x), b));
        }
    }
}
