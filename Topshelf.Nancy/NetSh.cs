using System;

namespace Topshelf.Nancy
{
    public static class NetSh
    {
        private const string NETSH_COMMAND = "netsh";

        public static bool DeleteUrlAcl(string url)
        {
            try
            {
                var arguments = GetDeleteParameters(url);

                string output;

                if (UacHelper.RunElevated(NETSH_COMMAND, arguments, out output))
                    return true;

                return FailedBecauseUrlReservationDidNotExist(output);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetDeleteParameters(string url)
        {
            return string.Format("http delete urlacl url={0}", url);
        }

        private static bool FailedBecauseUrlReservationDidNotExist(string netshProcessOutput)
        {
            return netshProcessOutput.Contains("Error: 2");
        }

    }
}
