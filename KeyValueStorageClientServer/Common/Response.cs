using static Common.RequestResponseEnum;

namespace Common
{
    public class Response : IMessageType
    {
        public Response(RequestResponseTypes responseType, string message)
        {
            MessageID = GenerateUniqueID.GetShortID();
            MessageType = "RESPONSE";
            ResponseType = responseType;
            Message = message;
        }

        public RequestResponseTypes ResponseType { get; set; }
    }
}