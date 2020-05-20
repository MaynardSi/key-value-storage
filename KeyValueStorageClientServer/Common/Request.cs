using Newtonsoft.Json;

namespace Common
{
    public class Request : IMessageType
    {
        // Constructor chaining
        public Request(string requestType, string message) : this($"M-{GenerateUniqueID.GetShortID()}", requestType, message) { }

        [JsonConstructor]
        public Request(string id, string requestType, string message)
        {
            Id = id;
            Command = requestType;
            Message = message;
        }
    }
}