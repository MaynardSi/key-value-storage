using Newtonsoft.Json;

namespace Common
{
    public class Response : IMessageType
    {
        // Constructor chaining
        public Response(string requestType, string message) : this($"M-{GenerateUniqueID.GetShortID()}", requestType, message) { }

        [JsonConstructor]
        public Response(string id, string responseType, string message)
        {
            Id = id;
            Command = responseType;
            Message = message;
        }
    }
}