using static Common.RequestResponseEnum;

namespace Common
{
    public class IMessageType
    {
        public string MessageID { get; set; }

        public RequestResponseTypes MessageType { get; set; } // "REQUEST"/"RESPONSE"

        public string Message { get; set; }
    }
}