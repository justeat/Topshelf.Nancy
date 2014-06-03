using System;
using Topshelf.ServiceConfigurators;
using Topshelf.HostConfigurators;

namespace Topshelf.Nancy
{
    public static class NancyServiceConfiguratorExtensions
    {
        public static ServiceConfigurator<T> WithNancyEndpoint<T>(this ServiceConfigurator<T> configurator, HostConfigurator hostconfigurator, Action<NancyConfigurator> nancyConfigurator) where T : class
        {
            var nancyWrapperConfiguration = new NancyConfigurator();

            nancyConfigurator(nancyWrapperConfiguration);

            var nancyService = new NancyService();

            nancyService.Configure(nancyWrapperConfiguration);

            configurator.AfterStartingService(t => nancyService.Start());

            configurator.BeforeStoppingService(t =>
            {
                nancyService.Stop();
            });

            hostconfigurator.BeforeInstall(x =>
            {
                nancyService.TryDeleteUrlReservations();
                nancyService.AddUrlReservations();
            });

            hostconfigurator.BeforeUninstall(() =>
            {
                nancyService.TryDeleteUrlReservations();
            });

            return configurator;
        }
    }
}