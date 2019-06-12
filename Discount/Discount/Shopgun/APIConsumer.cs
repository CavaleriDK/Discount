using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Discount.Shopgun
{
    /// <summary>
    /// The main class used to consume APIs
    /// </summary>
    static class APIConsumer
    {
        /// <summary>
        /// Token used for authorisations
        /// </summary>
        public static string XToken { get => xToken; set => xToken = value; }

        /// <summary>
        /// Signature used for added protection on API calls
        /// </summary>
        public static string XSignature { get => xSignature; set => xSignature = value; }

        private static string xToken;
        private static string xSignature;

        /// <summary>
        /// Method used to send POST requests with a JSON object response
        /// </summary>
        /// <typeparam name="T">The expected JSON model returned by the API call</typeparam>
        /// <param name="path">The full path to the API endpoint</param>
        /// <param name="json_body">The JSON serialized body to send with the request</param>
        /// <returns>The JSON model parsed from the response</returns>
        public static T SendJSONPostRequest<T>(string path, string json_body)
        {
            try
            {
                // Converts the JSON string to byte array to be consumed by WebRequest
                byte[] bodyArray = Encoding.UTF8.GetBytes(json_body);

                // Create the request and set up the proper content paramaters
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = bodyArray.Length;

                // Write the JSON bytes to the request datastream
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bodyArray, 0, bodyArray.Length);

                // Get a reference to the response
                WebResponse response = request.GetResponse();

                // Read the response stream and convert to a string
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);

                // Read the content
                string responseFromServer = reader.ReadToEnd();

                // Create a memory stream from the response
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(responseFromServer));

                // Create an appropriate serializer for the object type
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

                // Deserialize back into a Model
                T model = (T)serializer.ReadObject(ms);

                // Close all the connections and streams
                requestStream.Close();
                responseStream.Close();
                ms.Close();
                response.Close();

                return model;
            }
            catch (Exception e)
            {
                // Show error notification to the user in case anything goes wrong
                MessageBox.Show("Error: " + e.Message, "Connection error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            // Return default value if anything goes wrong
            return default(T);
        }

        /// <summary>
        /// Method used to send GET requests with a JSON object response
        /// </summary>
        /// <typeparam name="T">The expected JSON model(s) returned by the API call</typeparam>
        /// <param name="path">The full path to the API endpoint</param>
        /// <returns>The JSON model(s) parsed from the response</returns>
        public static T SendJSONGetRequest<T>(string path)
        {
            try
            {
                // Create the request and set up the proper content paramaters
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path);
                request.Method = "GET";
                request.Headers.Add("X-Token", XToken);
                request.Headers.Add("X-Signature", XSignature);

                // Get a reference to the response
                WebResponse response = request.GetResponse();

                // Read the response stream and convert to a string
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);

                // Read the content  
                string responseFromServer = reader.ReadToEnd();

                // Create a memory stream from the response
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(responseFromServer));

                // Create an appropriate serializer for the object type
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

                // Deserialize back into a Model
                T model = (T)serializer.ReadObject(ms);

                // Close all the connections and streams
                responseStream.Close();
                ms.Close();
                response.Close();

                return model;
            }
            catch (Exception e)
            {
                // Show error notification to the user in case anything goes wrong
                MessageBox.Show("Error: " + e.Message, "Connection error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            // Return default value if anything goes wrong
            return default(T);
        }
    }
}
