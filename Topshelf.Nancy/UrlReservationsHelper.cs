using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Nancy.Hosting.Self;
using Topshelf.Logging;

namespace Topshelf.Nancy
{
    internal class UrlReservationsHelper
    {
        private readonly List<Uri> _uris;
        private readonly HostConfiguration _nancyHostConfiguration;

        private static readonly LogWriter Logger = HostLogger.Get(typeof(UrlReservationsHelper));

        public UrlReservationsHelper(List<Uri> uris, HostConfiguration nancyHostConfiguration)
        {
            _uris = uris;
            _nancyHostConfiguration = nancyHostConfiguration;
        }

        public bool TryDeleteUrlReservations()
        {
            Logger.Info("[Topshelf.Nancy] Deleting URL Reservations");

            foreach (var prefix in GetPrefixes())
            {
                var result = NetSh.DeleteUrlAcl(prefix);

                if (result.ResultCode == NetShResultCode.Error)
                {
                    var message = string.Format("[Topshelf.Nancy] Error deleting URL Reservation {0} with command: netsh {1}. {2}", prefix, result.CommandRan, result.Message);
                    Logger.Error(message);
                    return false;
                }

                if (result.ResultCode == NetShResultCode.UrlReservationDoesNotExist)
                {
                    var message = string.Format("[Topshelf.Nancy] Could not delete URL Reservation {0} because it does not exist. Treating as a success.", prefix);
                    Logger.Warn(message);
                }
            }

            Logger.Info("[Topshelf.Nancy] URL Reservations deleted");

            return true;
        }

        public bool OpenFirewallPorts(IEnumerable<int> ports, string firewallRuleName)
        {
            Logger.Info("[Topshelf.Nancy] Opening firewall ports");

            var user = GetUser();

            var portList = string.Join(",", ports);

            var result = NetSh.OpenFirewallPorts(portList, user, firewallRuleName);
            if (result.ResultCode == NetShResultCode.Error)
            {
                var message = string.Format("[Topshelf.Nancy] Error opening firewall ports {0}: netsh {1}. {2}", portList, result.CommandRan, result.Message);
                Logger.Error(message);
                return false;
            }

            Logger.Info(string.Format("[Topshelf.Nancy] Firewall ports opened: {0}", portList));
            return true;
        }

        public bool AddUrlReservations(bool shouldOpenFirewallPorts = false)
        {
            var prefixes = GetPrefixes().ToList();
            var urlText = prefixes.Count == 1 ? "url" : "urls";
            var startMessage = string.Format("[Topshelf.Nancy] Adding Reservations for {0} {1}", prefixes.Count, urlText);
            Logger.Info(startMessage);

            var user = GetUser();

            foreach (var prefix in prefixes)
            {
                var result = NetSh.AddUrlAcl(prefix, user);
                if (result.ResultCode == NetShResultCode.Error)
                {
                    var message = string.Format("[Topshelf.Nancy] Error adding URL Reservation {0} with command: netsh {1}. {2}", prefix, result.CommandRan, result.Message);
                    Logger.Error(message);
                    return false;
                }

                if (result.ResultCode == NetShResultCode.UrlReservationAlreadyExists)
                {
                    var message = string.Format("[Topshelf.Nancy] Could not add URL Reservation {0} because it already exists. Treating as a success.", prefix);
                    Logger.Warn(message);
                    return true;
                }
            }

            Logger.Info("[Topshelf.Nancy] URL Reservations added");

            return true;
        }

        private string GetUser()
        {
            if (!string.IsNullOrWhiteSpace(_nancyHostConfiguration.UrlReservations.User))
            {
                return _nancyHostConfiguration.UrlReservations.User;
            }

            return WindowsIdentity.GetCurrent().Name;
        }

        private IEnumerable<string> GetPrefixes()
        {
            foreach (var baseUri in _uris)
            {
                var prefix = baseUri.ToString();

                if (baseUri.IsDefaultPort)
                {
                    prefix = prefix.Replace(baseUri.Host, string.Format("{0}:{1}", baseUri.Host, baseUri.Port));

                }

                if (_nancyHostConfiguration.RewriteLocalhost && !baseUri.Host.Contains("."))
                {
                    prefix = prefix.Replace("localhost", "+");
                }

                yield return prefix;
            }
        }
    }
}
