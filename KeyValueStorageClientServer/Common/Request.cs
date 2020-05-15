using static Common.RequestResponseEnum;

namespace Common
{
    public class Request : IMessageType
    {
        public Request(RequestResponseTypes requestType, string message)
        {
            MessageID = GenerateUniqueID.GetShortID();
            MessageType = "REQUEST";
            RequestType = requestType;
            Message = message;
        }

        public RequestResponseTypes RequestType { get; set; }
    }
}