Topshelf.Nancy
==========
_A Topshelf extension providing a Nancy endpoint for your Topshelf windows service_

---

* Introduction
* Installation
* Getting Started
* Documentation
* Contributing
* Copyright

Topshelf.Nancy is an extension to Topshelf that allows you to to serve content over http via Nanacy.  This is done by configuring a hosta nd port for nanacy to listen onto a NancyModule which serves your content.

Topshelf.Nancy was written to was written to provide an easily repeatable way to add a Nancy endpoint to Windows services configured via Topshelf.  This includes configuring URL reservations which would otherwise have to be manually configured.
		

## Installation

Pre-requisites: The project is built in .net v4.0.

* From source: https://github.com/justeat/Topshelf.Nancy
* By hand: https://www.nuget.org/packages/Topshelf.Nancy

Via NuGet:

		PM> Install-Package Topshelf.Nancy


## Getting Started

Once you have the package installed into your test project, a standard wireup will look like this.

```csharp
	var host = HostFactory.New(x =>
	{
	    x.UseNLog();
	    
	    x.Service<SampleService>(s =>
	    {
	        s.ConstructUsing(settings => new SampleService());
	        s.WhenStarted(service => service.Start());
	        s.WhenStopped(service => service.Stop());
	        s.WithNancyEndpoint(x, c =>
	        {
	            c.AddHost(port: 8080);
	        });
	    });
	    x.StartAutomatically();
	    x.SetServiceName("topshelf.nancy.sampleservice");
	    x.RunAsNetworkService();
	});
	
	host.Run();
```
A sample project is provided in the repository.

## Contributing

If you find a bug, have a feature request or even want to contribute an enhancement or fix, please follow the contributing guidelines included in the repository.


## Copyright

Copyright © JUST EAT PLC 2014
