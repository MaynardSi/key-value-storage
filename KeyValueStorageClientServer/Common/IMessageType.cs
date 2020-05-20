namespace Common
{
    public class IMessageType
    {
        public string Id { get; set; }

        public string Command { get; set; } // "REQUEST"/"RESPONSE"

        public string Message { get; set; }
    }
}