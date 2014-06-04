namespace Topshelf.Nancy
{
    public class NetShResult
    {
        public string Message { get; protected set; }
        public NetShResultCode ResultCode { get; set; }
        public string CommandRan { get; set; }

        public NetShResult(NetShResultCode resultCode, string message, string commandRan)
        {
            ResultCode = resultCode;
            Message = message;
            CommandRan = commandRan;
        }
    }
}