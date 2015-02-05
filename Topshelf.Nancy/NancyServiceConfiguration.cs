using System;
using System.Collections.Generic;
using Nancy.Hosting.Self;

namespace Topshelf.Nancy
{
    public class NancyServiceConfiguration
	{
		
        internal List<Uri> Uris { get; set; }
        internal Action<HostConfiguration> NancyHostConfigurator { get; set; }

        internal bool ShouldCreateUrlReservationsOnInstall { get; set; }
        internal bool ShouldDeleteReservationsOnUnInstall { get; set; }

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
		public void AddHost(string scheme = "http", string domain = "localhost", int port = 8080)
		{
            Uris.Add(new UriBuilder(scheme, domain, port).Uri);
		}
	}
}
