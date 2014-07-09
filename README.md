[![Build status](https://ci.appveyor.com/api/projects/status/pa8wocs17k5ts2c7)](https://ci.appveyor.com/project/justeattech/topshelf-nancy)

Topshelf.Nancy
==========
_A Topshelf extension providing a Nancy endpoint for your Windows service_

---

* Introduction
* Installation
* Getting Started
* Documentation
* Contributing
* Copyright

Topshelf.Nancy is an extension to Topshelf that allows you to to serve content over http via Nancy.  This is done by configuring a host and port for Nancy to listen onto via a NancyModule which serves your content.

## Installation

Pre-requisites: The project is built in .net v4.0.

* From source: https://github.com/justeat/Topshelf.Nancy
* By hand: https://www.nuget.org/packages/Topshelf.Nancy

Via NuGet:

		PM> Install-Package Topshelf.Nancy


## Getting Started

Once you have the package installed into your test project, a standard wire-up will look like this.

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

## Documentation

### URL Reservations

When you host a website within IIS, reserving a URL so that traffic is directed to your IIS website and not a different website on the machine is done for you.  It's something you don't event think about. This is called a URL Reservation, and a new URL Reservation is added via the command `netsh http add url=url user=user`

The Nancy Self Host does this for you too, however `netsh` needs Administrator privileges. Normally Windows services run in a less permissive context (as Network Service or Local Service) so the only time to add a URL Reservation is during install as installing a service requires Administrator privileges.  

If your service is running with restrictive permissions then you can specify that URL Reservations be created at install time like so. 

```csharp
s.WithNancyEndpoint(x, c =>
{
    c.AddHost(port: 8080);
	c.CreateUrlReservationsOnInstall();
});
	
```

By default any URL Reservations added will also be deleted when the service is uninstalled.

## Contributing

If you find a bug, have a feature request or even want to contribute an enhancement or fix, please follow the contributing guidelines included in the repository.


## Copyright

Copyright © JUST EAT PLC 2014
