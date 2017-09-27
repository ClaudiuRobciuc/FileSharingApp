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
            
            var task = Task.Run((Func<Task>)Program.Run);
            task.Wait();
          
            
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();
            host.Run();
        }

        static async Task Run()
        {
            using (var dbx = new DropboxClient("MgX6Ia7UK1AAAAAAAAAACUxEgjuhsT4DtHykvshYVhkO5EtLqHoZOvPVuG4NJ-L2"))
            {
                var full = await dbx.Users.GetCurrentAccountAsync();
                Console.WriteLine(full.Email);
                await ListRootFolder(dbx);
                
            }
        }
        static async Task ListRootFolder(DropboxClient dbx)
        {
            var list = await dbx.Files.ListFolderAsync(string.Empty);

            // show folders then files
            foreach (var item in list.Entries.Where(i => i.IsFolder))
            {

                var folder = await dbx.Files.ListFolderAsync("/"+item.Name);
                Console.WriteLine("D  {0}/", item.Name);
                foreach (var item2 in folder.Entries.Where(i => i.IsFile))
                {
                    Console.WriteLine("F{0,8} {1}", item2.AsFile.Size, item2.Name);
                    Console.WriteLine(item2.PathDisplay);
                }
            }
            foreach (var item in list.Entries.Where(i => i.IsFile))
            {
                    Console.WriteLine("F{0,8} {1}", item.AsFile.Size, item.Name);
            }

            
        }
        static async Task Download(DropboxClient dbx, string folder, string file)
        {
            using (var response = await dbx.Files.DownloadAsync(folder + "/" + file))
            {
                Console.WriteLine(await response.GetContentAsStringAsync());
            }
        }
        static async Task Upload(DropboxClient dbx, string folder, string file, string content)
        {
            using (var mem = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                var updated = await dbx.Files.UploadAsync(
                folder + "/" + file,
                WriteMode.Overwrite.Instance,
                body: mem);
                Console.WriteLine("Saved {0}/{1} rev {2}", folder, file, updated.Rev);
            }
        }
    }
}
