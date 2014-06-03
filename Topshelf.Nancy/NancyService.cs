using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Principal;
using Nancy;
using Nancy.Hosting.Self;
using Topshelf.Logging;

namespace Topshelf.Nancy
{
    internal class NancyService
    {
        private NancyHost NancyHost { get; set; }

        private HostConfiguration HostConfiguration { get; set; }

        private NancyConfigurator NancyConfigurator { get; set; }

        private static readonly LogWriter Logger = HostLogger.Get(typeof(NancyService));

        public NancyHost Configure(NancyConfigurator nancyConfigurator)
        {

            var nancyHostConfiguration = new HostConfiguration();

            if (nancyConfigurator.NancyHostConfigurator != null)
            {
                nancyConfigurator.NancyHostConfigurator(nancyHostConfiguration);
            }

            NancyConfigurator = nancyConfigurator;
            HostConfiguration = nancyHostConfiguration;

            StaticConfiguration.DisableErrorTraces = false;

            NancyHost = new NancyHost(HostConfiguration, NancyConfigurator.Uris.ToArray());

            return NancyHost;
        }

        public void Start()
        {

            Logger.Info("[Topshelf.Nancy] Starting NanacyHost");

            NancyHost.Start();

            Logger.Info("[Topshelf.Nancy] NanacyHost started");
        }

        public void Stop()
        {
            NancyHost.Stop();
        }

        public bool TryDeleteUrlReservations()
        {
            Logger.Info("[Topshelf.Nancy] Deleting URL Reservations");
            
            foreach (var prefix in GetPrefixes())
            {
                NetSh.DeleteUrlAcl(prefix);
            }

            Logger.Info("[Topshelf.Nancy] URL Reservations deleted");

            return true;
        }

        public bool AddUrlReservations()
        {
            Logger.Info("[Topshelf.Nancy] Adding URL Reservations");

            var user = GetUser();

            foreach (var prefix in GetPrefixes())
            {
                if (!global::Nancy.Hosting.Self.NetSh.AddUrlAcl(prefix, user))
                {
                    Logger.Error("[Topshelf.Nancy] Error adding URL Reservations");
                    return false;
                }
            }

            Logger.Info("[Topshelf.Nancy] URL Reservations added");

            return true;
        }

        private string GetUser()
        {
            if (!string.IsNullOrWhiteSpace(HostConfiguration.UrlReservations.User))
            {
                return HostConfiguration.UrlReservations.User;
            }

            return WindowsIdentity.GetCurrent().Name;
        }

        private IEnumerable<string> GetPrefixes()
        {
            foreach (var baseUri in NancyConfigurator.Uris)
            {
                var prefix = baseUri.ToString();

                if (HostConfiguration.RewriteLocalhost && !baseUri.Host.Contains("."))
                {
                    prefix = prefix.Replace("localhost", "+");
                }

                yield return prefix;
            }
        }
    }
}