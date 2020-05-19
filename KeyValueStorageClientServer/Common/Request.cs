using static Common.RequestResponseEnum;

namespace Common
{
    public class Request : IMessageType
    {
        public Request(RequestResponseTypes requestType, string message)
        {
            MessageID = $"M-{GenerateUniqueID.GetShortID()}";
            MessageType = requestType;
            Message = message;
        }
    }
}