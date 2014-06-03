using Nancy;

namespace Topshelf.Nancy.Sample
{
    public class SampleNancyModule : NancyModule
    {
        public SampleNancyModule()
        {
            Get["/status"] = _ => "I am alive!";
        }
    }
}