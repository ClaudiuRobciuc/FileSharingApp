using System;
using Microsoft.AspNetCore.Hosting;
using  Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.IO;
using Dropbox.Api;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using Dropbox.Api.Files;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
           
          
            
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();
            host.Run();
        }

        
    }
}
