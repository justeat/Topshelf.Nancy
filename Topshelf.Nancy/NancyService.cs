using System;
using System.Linq;
using Nancy;
using Nancy.Hosting.Self;
using Topshelf.Logging;

namespace Topshelf.Nancy
{
    internal class NancyService
    {
        private NancyHost NancyHost { get; set; }

        private HostConfiguration NancyHostConfiguration { get; set; }

        private NancyServiceConfiguration NancyServiceConfiguration { get; set; }

        private static readonly LogWriter Logger = HostLogger.Get(typeof(NancyService));
        private UrlReservationsHelper _urlReservationsHelper;

        public NancyHost Configure(NancyServiceConfiguration nancyServiceConfiguration)
        {

            var nancyHostConfiguration = new HostConfiguration();

            if (nancyServiceConfiguration.NancyHostConfigurator != null)
            {
                nancyServiceConfiguration.NancyHostConfigurator(nancyHostConfiguration);
            }

            NancyServiceConfiguration = nancyServiceConfiguration;
            NancyHostConfiguration = nancyHostConfiguration;

            _urlReservationsHelper = new UrlReservationsHelper(NancyServiceConfiguration.Uris, NancyHostConfiguration);

            NancyHost = new NancyHost(NancyHostConfiguration, NancyServiceConfiguration.Uris.ToArray());
            return NancyHost;
        }

        public void Start()
        {
            Logger.Info("[Topshelf.Nancy] Starting NancyHost");
            NancyHost.Start();
            Logger.Info("[Topshelf.Nancy] NancyHost started");
        }

        public void Stop()
        {
            Logger.Info("[Topshelf.Nancy] Stopping NancyHost");
            NancyHost.Stop();
            Logger.Info("[Topshelf.Nancy] NancyHost stopped");
        }

        public void BeforeInstall()
        {
            if (NancyServiceConfiguration.ShouldCreateUrlReservationsOnInstall)
            {
                _urlReservationsHelper.TryDeleteUrlReservations();

                if (NancyServiceConfiguration.ShouldOpenFirewallPorts)
                {
                    var ports = NancyServiceConfiguration.Uris.Select(x => x.Port).ToList();
                    _urlReservationsHelper.OpenFirewallPorts(ports, NancyServiceConfiguration.FirewallRuleName);
                }

                _urlReservationsHelper.AddUrlReservations();

            }
        }

        public void BeforeUninstall()
        {
            if (NancyServiceConfiguration.ShouldDeleteReservationsOnUnInstall)
            {
                _urlReservationsHelper.TryDeleteUrlReservations();
            }
        }
    }
}
