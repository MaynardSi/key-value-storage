using Newtonsoft.Json;
using System.Collections.Generic;

namespace Common
{
    public class MessageWrapper
    {
        public static string WrapRequest(string request)
        {
            string id;
            string command;
            string message = "";
            Dictionary<string, string> requestDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(request);
            if (requestDictionary.ContainsKey("commandID"))
            {
                id = requestDictionary["commandID"];
                command = requestDictionary["commandType"];
                if (requestDictionary.ContainsKey("messageTarget"))
                {
                    message += $"{ requestDictionary["messageTarget"] },";
                    if (requestDictionary.ContainsKey("messageValue"))
                    {
                        message += $"{ requestDictionary["messageValue"] }";
                    }
                }
                return CreateRequest(command, message);
            }
            return request;
        }

        // Duplicated area of code because eventually response and requests may differ

        /// <summary>
        /// Returns a JSON string built from the serialized Response class.
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string CreateResponse(string requestType, string responseMessage)
        {
            Response responseObj = new Response(requestType, responseMessage);
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(responseObj);
            return jsonString;
        }

        /// <summary>
        /// Returns a JSON string built from the serialized Response class and process response.
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string CreateRequest(string requestType, string requestMessage)
        {
            Request requestObj = new Request(requestType, requestMessage);
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(requestObj);
            return jsonString;
        }
    }
}