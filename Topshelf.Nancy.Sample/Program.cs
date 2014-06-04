namespace Topshelf.Nancy.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
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
                        c.AddHost(port: 20005);
                        c.AddHost(port: 20006);
                        c.CreateUrlReservationsOnInstall();
                    });
                });
                x.StartAutomatically();
                x.SetServiceName("topshelf.nancy.sampleservice");
                x.SetDisplayName("Topshelf.Nancy.SampleService");
                x.SetDescription("Sample Service for the Topshelf.Nancy project");
                x.RunAsNetworkService();
            });

            host.Run();
        }
    }
}
