using System;
using System.Linq;
using Nancy;
using Nancy.Hosting.Self;
using Topshelf.Logging;

namespace Topshelf.Nancy
{
    internal class NancyService
    {
        private Lazy<NancyHost> NancyHost { get; set; }

        private HostConfiguration NancyHostConfiguration { get; set; }

        private NancyServiceConfiguration NancyServiceConfiguration { get; set; }

        private static readonly LogWriter Logger = HostLogger.Get(typeof(NancyService));
        private UrlReservationsHelper _urlReservationsHelper;

        public void Configure(NancyServiceConfiguration nancyServiceConfiguration)
        {
            var nancyHostConfiguration = new HostConfiguration();

            if (nancyServiceConfiguration.NancyHostConfigurator != null)
            {
              nancyServiceConfiguration.NancyHostConfigurator(nancyHostConfiguration);
            }

            NancyServiceConfiguration = nancyServiceConfiguration;
            NancyHostConfiguration = nancyHostConfiguration;

            _urlReservationsHelper = new UrlReservationsHelper(NancyServiceConfiguration.Uris, NancyHostConfiguration);


            NancyHost = new Lazy<NancyHost>(() => {
                if (NancyServiceConfiguration.Bootstrapper != null)
                {
                  return new NancyHost(NancyServiceConfiguration.Bootstrapper, NancyHostConfiguration, NancyServiceConfiguration.Uris.ToArray());
                }
                else
                {
                  return new NancyHost(NancyHostConfiguration, NancyServiceConfiguration.Uris.ToArray());
                }
              });
        }

        public void Start()
        {
            Logger.Info("[Topshelf.Nancy] Starting NancyHost");
            NancyHost.Value.Start();
            Logger.Info("[Topshelf.Nancy] NancyHost started");
        }

        public void Stop()
        {
            Logger.Info("[Topshelf.Nancy] Stopping NancyHost");
            NancyHost.Value.Stop();
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
