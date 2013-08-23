using System;
using System.Web;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace ByteSmith.WindowsAzure.Messaging
{
    public class SASTokenGenerator
    {
        public static string GetSASToken(string resourceUri, string keyName, string key, TimeSpan ttl)
        {
		    var expiry = GetExpiry(ttl);
            string stringToSign = HttpUtility.UrlEncode(resourceUri) + "\n" + expiry;
            string signature;
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
            }
            var sasToken = String.Format(CultureInfo.InvariantCulture, "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}",
                HttpUtility.UrlEncode(resourceUri), HttpUtility.UrlEncode(signature), expiry, keyName);
            return sasToken;
        }

        private static string GetExpiry(TimeSpan ttl)
        {
            TimeSpan expirySinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1) + ttl;
            return Convert.ToString((int)expirySinceEpoch.TotalSeconds);
        } 
    }
}
