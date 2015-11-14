using System;
using System.Collections.Generic;
using Nancy.Bootstrapper;
using Nancy.Hosting.Self;

namespace Topshelf.Nancy
{
    public class NancyServiceConfiguration
    {
        internal List<Uri> Uris { get; set; }
        internal Action<HostConfiguration> NancyHostConfigurator { get; set; }

        internal bool ShouldCreateUrlReservationsOnInstall { get; set; }
        internal bool ShouldDeleteReservationsOnUnInstall { get; set; }
        public bool ShouldOpenFirewallPorts { get; set; }
        public string FirewallRuleName { get; set; }
        public INancyBootstrapper Bootstrapper { get; set; }

        /// <summary>
        /// Determines if URL Resverations should be created automatically when the services installs.  
        /// Else Nancy will try and create the URL Reservation when it starts.  
        /// Adding URL Reservations require Adminstrative priviliages, so this is useful
        /// for when the service runs with a more restrictive use (i.e. as the Network Service).
        /// </summary>
        public void CreateUrlReservationsOnInstall()
        {
            ShouldCreateUrlReservationsOnInstall = true;
            ShouldDeleteReservationsOnUnInstall = true;
        }

        /// <summary>
        /// Opens the firewall ports on install. The ports are opened only when the CreateUrlReservationsOnInstall is called as well.
        /// </summary>
        /// <param name="firewallRuleName">The name of the firewall rule</param>
        public void OpenFirewallPortsOnInstall(string firewallRuleName)
        {
            ShouldOpenFirewallPorts = true;
            FirewallRuleName = firewallRuleName;
        }

        /// <summary>
        /// Allows Topshelf.Nancy to delete any URL Reservations it has created. 
        /// This is automatically set to true if CreateUrlReservationsOnInstall() has been previously called
        /// </summary>
        public void DeleteReservationsOnUnInstall()
        {
            ShouldDeleteReservationsOnUnInstall = true;
        }

        public NancyServiceConfiguration()
        {
            Uris = new List<Uri>();
        }

        /// <summary>
        /// Configure NancyHost via Nancy.Hosting.Self.HostConfiguration.
        /// </summary>
        /// <param name="nancyHostConfigurator">An action which configures Nancy.Hosting.Self.HostConfiguration</param>
        public void ConfigureNancy(Action<HostConfiguration> nancyHostConfigurator)
        {
            NancyHostConfigurator = nancyHostConfigurator;
        }

        /// <summary>
        /// Adds a new Host for Nancy to listen on.
        /// </summary>
        /// <param name="scheme">http or https. Defaults to http.</param>
        /// <param name="domain">The domain to listen on e.g. www.mydomain.com. Defaults to localhost.</param>
        /// <param name="port">The port to listen on. Defaults to 8080.</param>
        /// <param name="path">The path component. Defaults to an empty string.</param>
        public void AddHost(string scheme = "http", string domain = "localhost", int port = 8080, string path = "")
        {
            Uris.Add(new UriBuilder(scheme, domain, port, path).Uri);
        }

        /// <summary>
        /// Set the INancyBootstrapper instance that Nancy will use.
        /// If a bootstrapper is not configured here, Nancy will attempt to automatically locate one.
        /// </summary>
        /// <param name="bootstrapper">The bootstrapper to use.</param>
        public void UseBootstrapper (INancyBootstrapper bootstrapper)
        {
            Bootstrapper = bootstrapper;
        }
    }
}
