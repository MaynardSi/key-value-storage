using static Common.RequestResponseEnum;

namespace Common
{
    public class Response : IMessageType
    {
        public Response(RequestResponseTypes responseType, string message)
        {
            MessageID = $"M-{GenerateUniqueID.GetShortID()}";
            MessageType = responseType;
            Message = message;
        }
    }
}