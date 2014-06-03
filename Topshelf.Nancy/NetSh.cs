using System;

namespace Topshelf.Nancy
{
    public static class NetSh
    {
        private const string NETSH_COMMAND = "netsh";

        public static NetShResultCode DeleteUrlAcl(string url)
        {
            try
            {
                var arguments = GetDeleteParameters(url);

                string output;

                if (UacHelper.RunElevated(NETSH_COMMAND, arguments, out output))
                    return NetShResultCode.Success;
                ;

                if (FailedBecauseUrlReservationDidNotExist(output))
                {
                    return NetShResultCode.UrlReservationDoesNotExist;
                }
            }
            catch (Exception)
            {
                return NetShResultCode.Error;
            }

            return NetShResultCode.Error;
        }

        public static NetShResultCode AddUrlAcl(string url, string user)
        {
            try
            {
                var arguments = GetAddParameters(url, user);

                string output;

                if (UacHelper.RunElevated(NETSH_COMMAND, arguments, out output))
                    return NetShResultCode.Success;


                if (FailedBecauseUrlReservationAlreadyExists(output))
                {
                    return NetShResultCode.UrlReservationAlreadyExists;
                }

            }
            catch (Exception)
            {
                return NetShResultCode.Error;
            }

            return NetShResultCode.Error;
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
}
