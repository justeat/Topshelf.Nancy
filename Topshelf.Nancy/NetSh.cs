using System;

namespace Topshelf.Nancy
{
    public static class NetSh
    {
        private const string NetshCommand = "netsh";

        public static NetShResult DeleteUrlAcl(string url)
        {
            var arguments = GetDeleteParameters(url);

            try
            {
                string output;

                if (UacHelper.RunElevated(NetshCommand, arguments, out output))
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

                if (UacHelper.RunElevated(NetshCommand, arguments, out output))
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

        public static NetShResult OpenFirewallPorts(string portList, string username, string firewallRuleName)
        {
            var arguments = GetFirewallParameters(portList, firewallRuleName);

            try
            {
                string output;

                if (UacHelper.RunElevated(NetshCommand, arguments, out output))
                    return new NetShResult(NetShResultCode.Success, output, arguments);

                return new NetShResult(NetShResultCode.Error, output, arguments);

            }
            catch (Exception ex)
            {
                return new NetShResult(NetShResultCode.Error, ex.Message, arguments);
            };
        }

        private static string GetFirewallParameters(string portList, string firewallRuleName)
        {
            return string.Format(
                "advfirewall firewall add rule name=\"{0}\" dir=in protocol=TCP localport=\"{1}\" action=allow",
                firewallRuleName, portList);
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
