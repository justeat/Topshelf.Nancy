using System;
using System.Collections.Generic;
using System.IO;
using Nancy.Hosting.Self;
using Topshelf.Logging;

namespace Topshelf.Nancy
{
    public class NancyConfigurator
	{
		
        internal List<Uri> Uris { get; set; }
        internal Action<HostConfiguration> NancyHostConfigurator { get; set; }


        public NancyConfigurator()
        {
            Uris = new List<Uri>();
        }

        public void Configure(Action<HostConfiguration> nancyHostConfigurator)
		{
            NancyHostConfigurator = nancyHostConfigurator;
		}

		public void AddHost(string scheme = "http", string domain = "localhost", int port = 8080)
		{
            Uris.Add(new UriBuilder(scheme, domain, port).Uri);
		}
	}
}
