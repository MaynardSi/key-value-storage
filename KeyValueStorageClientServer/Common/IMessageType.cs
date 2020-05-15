namespace Common
{
    public class IMessageType
    {
        public string MessageID { get; set; }
        public string MessageType { get; set; } // "REQUEST"/"RESPONSE"
        public string Message { get; set; }
    }
}