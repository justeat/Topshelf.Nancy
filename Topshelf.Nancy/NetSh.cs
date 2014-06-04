using System;

namespace Topshelf.Nancy
{
    public static class NetSh
    {
        private const string NETSH_COMMAND = "netsh";

        public static NetShResult DeleteUrlAcl(string url)
        {
            var arguments = GetDeleteParameters(url);

            try
            {
                string output;

                if (UacHelper.RunElevated(NETSH_COMMAND, arguments, out output))
                    return new NetShResult(NetShResultCode.Success, output, arguments);

                if (FailedBecauseUrlReservationDidNotExist(output))
                {
                    return new NetShResult(NetShResultCode.UrlReservationDoesNotExist, output, arguments);
                }


                return new NetShResult(NetShResultCode.Error, output, arguments);
            }
            catch (Exception ex)
            {
                return new NetShResult(NetShResultCode.Error, ex.Message, arguments);
            }
        }

        public static NetShResult AddUrlAcl(string url, string user)
        {
            var arguments = GetAddParameters(url, user);

            try
            {
                string output;

                if (UacHelper.RunElevated(NETSH_COMMAND, arguments, out output))
                    return new NetShResult(NetShResultCode.Success, output, arguments);


                if (FailedBecauseUrlReservationAlreadyExists(output))
                {
                    return new NetShResult(NetShResultCode.UrlReservationAlreadyExists, output, arguments);
                }

                return new NetShResult(NetShResultCode.Error, output, arguments);

            }
            catch (Exception ex)
            {
                return new NetShResult(NetShResultCode.Error, ex.Message, arguments);
            };
        }

        public static string GetDeleteParameters(string url)
        {
            return string.Format("http delete urlacl url={0}", url);
        }

        public static string GetAddParameters(string url, string user)
        {
            return string.Format("http add urlacl url={0} user={1}", url, user);
        }

        private static bool FailedBecauseUrlReservationDidNotExist(string netshProcessOutput)
        {
            return netshProcessOutput.Contains("Error: 2");
        }

        private static bool FailedBecauseUrlReservationAlreadyExists(string netshProcessOutput)
        {
            return netshProcessOutput.Contains("Error: 183");
        }
    }

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
