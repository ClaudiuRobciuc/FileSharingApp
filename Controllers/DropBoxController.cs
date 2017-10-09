using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net;
using ConsoleApplication.Models.Entities;
using ConsoleApplication.Models.Repositories;
using ConsoleApplication.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Dropbox.Api;
using Microsoft.Extensions.Configuration;
using System.Text;
using Dropbox.Api.Files;
using System.Net.Http;

namespace ConsoleApplication.Controllers
{
    [Authorize]
    public class DropBoxController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private IHostingEnvironment hostingEnv;
        private ICategoryRepositoryDB categoryRepository;
        private IItemsRepositoryDB itemsRepository;
        
        private CategoriesItemsViewModelDB civm = new CategoriesItemsViewModelDB(); 
        private static CategoriesItemsViewModelDB auxCivm = new CategoriesItemsViewModelDB(); 
        private static CategoriesItemsViewModelDB auxCivmRemove = new CategoriesItemsViewModelDB(); 

        private ItemViewModelDB ivm = new ItemViewModelDB();
        private static DropboxClient dropbox = new DropboxClient("MgX6Ia7UK1AAAAAAAAAACUxEgjuhsT4DtHykvshYVhkO5EtLqHoZOvPVuG4NJ-L2");
        
        public DropBoxController(ICategoryRepositoryDB categoryRepository, IItemsRepositoryDB itemsRepository, IHostingEnvironment env)
        {
            this.categoryRepository = categoryRepository;
            this.itemsRepository = itemsRepository;
            //auxCivm.Categories = categoryRepository.GetAll().ToList();
            auxCivm.Categories = categoryRepository.GetAll().ToList();
            auxCivm.Items = itemsRepository.GetAll().ToList();
            var task = Task.Run((Func<Task>)DropBoxController.Run);
            civm.GroupTags = new List<String>();
            task.Wait();           
            if(task.IsCompleted)
            {   try{
               
                    foreach(DropBoxCategory c in auxCivm.Categories)
                    {
                        categoryRepository.Save(c);
                    }
                    foreach(DropBoxItems i in auxCivm.Items)
                    {   
                        itemsRepository.Save(i);
                    } 
                    foreach(DropBoxCategory c in auxCivmRemove.Categories)
                    {
                        categoryRepository.Delete(c);
                    }
                    foreach(DropBoxItems i in auxCivmRemove.Items)
                    {   
                        itemsRepository.Delete(i);
                    }
                
                }catch(Exception e)
                {

                }
            civm.Categories = categoryRepository.GetAll().ToList();
            civm.Items = itemsRepository.GetAll();
            ivm.Categories = categoryRepository.GetAll().ToList();      
            this.hostingEnv = env;
            }
        }
        static async Task Run()
        {
            HttpClientHandler handler = new HttpClientHandler();
            using (var client = new HttpClient(handler, false))
            {
                CategoriesItemsViewModelDB aux = await ListRootFolder(dropbox);
                List<DropBoxItems> items = new List<DropBoxItems>();
                List<DropBoxCategory> categories = new List<DropBoxCategory>();
                List<DropBoxItems> auxI = new List<DropBoxItems>();
                List<DropBoxCategory> auxC = new List<DropBoxCategory>();
                List<DropBoxItems> auxIRemove = new List<DropBoxItems>();
                List<DropBoxCategory> auxCRemove = new List<DropBoxCategory>();
                var auxItems = aux.Items.Select(DropBoxItems => DropBoxItems);
                var auxCategories = aux.Categories.Select(DropBoxCategory => DropBoxCategory);
                
                foreach(DropBoxItems i in auxItems)
                {
                    items.Add(i);
                }
                foreach(DropBoxCategory c in auxCategories)
                {  
                    categories.Add(c);
                }
                if(auxCivm.Items.Count()>0)
                {
                    foreach(DropBoxItems i in auxCivm.Items)
                    {   
                        bool exist=false;
                        int j=0;
                        for(j=0;j<items.Count()&&exist==false;j++)
                        {
                            if(items[j].Title.Equals(i.Title))
                            {
                                exist=true;
                            }
                        }
                        if(exist==false)
                        {
                            auxIRemove.Add(i);
                        }  
                        
                    }
                    foreach(DropBoxCategory c in auxCivm.Categories)
                    {   
                        bool exist=false;
                        int j=0;
                        for(j=0;j<categories.Count()&&exist==false;j++)
                        {
                            if(categories[j].CategoryType.Equals(c.CategoryType))
                            {
                                exist=true;
                            }
                        }
                        if(exist==false)
                        {
                            auxCRemove.Add(c);
                        }  
                        
                    }
                    auxCivmRemove.Items = auxIRemove;
                    auxCivmRemove.Categories = auxCRemove;
                    for(int j=0; j<items.Count();j++)
                    {   
                        bool exist=false;
                        foreach(DropBoxItems i in auxCivm.Items)
                        {
                            
                            if(items[j].Title.Equals(i.Title))
                            {
                                exist=true;
                            }
                        }
                        if(exist==false)
                        {
                            auxI.Add(items[j]);

                        }   
                    }       
                    for(int j=0; j<categories.Count();j++)
                    {   bool exist= false;
                        foreach(DropBoxCategory c in auxCivm.Categories)
                        {
                            if(categories[j].CategoryType.Equals(c.CategoryType))
                            {
                                exist=true;
                            }
                        }
                    
                        if(exist==false)
                        {
                            auxC.Add(categories[j]);
                        }
                    }
                auxCivm.Categories = auxC;
                auxCivm.Items = auxI;
                }
                else
                {
                    auxCivm.Categories = categories;
                    auxCivm.Items = items;
                }

                
                //await Download(dbx,"","Get Started with Dropbox.pdf");
            }   
        }    
        static async Task<CategoriesItemsViewModelDB> ListRootFolder(DropboxClient dbx)
        {
            HttpClientHandler handler = new HttpClientHandler();
            using (var client = new HttpClient(handler, false))
            {
                CategoriesItemsViewModelDB catItems = new CategoriesItemsViewModelDB();
                List<DropBoxItems> items = new List<DropBoxItems>();
                List<DropBoxCategory> categories = new List<DropBoxCategory>();
                var list = await dbx.Files.ListFolderAsync(string.Empty);
                foreach (var folder in list.Entries.Where(i => i.IsFolder))
                {

                    var cFolder = await dbx.Files.ListFolderAsync("/"+folder.Name);
                    foreach (var file in cFolder.Entries.Where(i => i.IsFile))
                    {
                        bool cExist= false;
                        DropBoxItems item = new DropBoxItems();
                        String fName = file.Name;
                        String aux = fName.Substring(0,(fName.IndexOf('.')));
                        String form = fName.Substring(fName.IndexOf('.'),(fName.Length-fName.IndexOf('.')));
                        //item = new Items{Title=aux,Format=form, Path=Path.Combine(uploads,file.FileName),date=System.DateTime.UtcNow.ToString()};
                        item.Title=aux;
                        item.Format=form;
                        item.date = System.DateTime.UtcNow.ToString();
                        item.Tags = folder.Name;
                        item.Path = file.PathDisplay;
                        //To be replaced
                        //var author = await dbx.Users.GetCurrentAccountAsync();
                        //item.Author = author.Email.ToString();
                        item.Author = "DropBox User";
                        items.Add(item);
                        DropBoxCategory category = new DropBoxCategory{CategoryType=form};
                        for(int i=0; i< (categories.Count())&&(cExist==false); i++)
                        {
                            if(categories[i].CategoryType.Equals(form))
                            {
                                cExist=true;
                            }
                        }
                        if(cExist==false)
                        {   
                            categories.Add(category);
                        }
                    }
                }
            foreach (var file in list.Entries.Where(i => i.IsFile))
            {
                bool cExist= false;
                DropBoxItems item = new DropBoxItems();
                String fName = file.Name;
                String aux = fName.Substring(0,(fName.IndexOf('.')));
                String form = fName.Substring(fName.IndexOf('.'),(fName.Length-fName.IndexOf('.')));
                //item = new Items{Title=aux,Format=form, Path=Path.Combine(uploads,file.FileName),date=System.DateTime.UtcNow.ToString()};
                item.Title=aux;
                item.Format=form;
                item.date = System.DateTime.UtcNow.ToString();
                item.Tags = file.PathDisplay.Substring(0,(file.PathDisplay.IndexOf(file.Name[0])));
                item.Path = file.PathDisplay;
                //var author = await dbx.Users.GetCurrentAccountAsync();
                //item.Author = author.Email.ToString();
                item.Author = "DropBox User";
                items.Add(item);
                DropBoxCategory category = new DropBoxCategory{CategoryType=form};
                for(int i=0; i< (categories.Count())&&(cExist==false); i++)
                {
                    if(categories[i].CategoryType.Equals(form))
                    {
                        cExist=true;
                    }
                }
                if(cExist==false)
                {   
                    categories.Add(category);
                }
                
            }
            catItems.Categories = categories;
            catItems.Items = items;
            return catItems;
            }
            
        }
       
        [HttpGet]
        public IActionResult Index(String sortOrder, String searchString, String[] format)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["FormatSortParm"] = sortOrder == "Format" ? "format_desc" : "Format";
            ViewData["DescriptionSortParm"] = sortOrder == "Description" ? "description_desc" : "Description";
            ViewData["TagsSortParm"] = sortOrder == "Tags" ? "tags_desc" : "Tags";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["CurrentFilter"] = searchString;
            List<Boolean> madeTrue = new List<Boolean>();
            foreach(DropBoxItems i in civm.Items)
            {
                civm.GroupTags.Add(i.Tags);
            }
            civm.GroupTags = civm.GroupTags.Distinct().ToList();
            foreach(DropBoxCategory c in categoryRepository.GetAll())
                madeTrue.Add(false);
            try{
            if(!String.IsNullOrEmpty(format[0]))
            {
                int k=0;
                int n=0;
                ViewData["FormatFilter"] = format;
                List<DropBoxItems> aux = new List<DropBoxItems>();
                List<DropBoxCategory> category = categoryRepository.GetAll().ToList();
                foreach(var selection in format)
                {
                    foreach(DropBoxItems i in itemsRepository.GetAll())
                    {
                        if(i.Format.Equals(selection))
                            aux.Add(i);
                    }
                    k=categoryRepository.Size();
                    for(int j=n; j<k&&madeTrue[n]==false; j++)
                    {
                        if(category[j].CategoryType.Equals(selection))
                        {
                            categoryRepository.Get(category[j].CategoryID).Selected=true;
                            madeTrue[n]=true;        
                        }
                        else
                        {
                            categoryRepository.Get(category[j].CategoryID).Selected=false;
                            n++;
                        }
                    }
                    n++;
                }
                for(int j=0; j<categoryRepository.Size(); j++)
                {
                    categoryRepository.Get(category[j].CategoryID).Selected=madeTrue[j];
                }
                civm.Items = aux;  
            }
            else{
                foreach(DropBoxCategory c in categoryRepository.GetAll())
                    categoryRepository.Get(c.CategoryID).Selected=false;
            } 
            }catch(Exception e){
                
            }
            
            var items = civm.Items;
            if (!String.IsNullOrEmpty(searchString))
            {
                
                items = items.Where(s => s.Title.ToLower().Contains(searchString.ToLower())
                               || s.Tags.ToLower().Contains(searchString.ToLower()));
               
            }
            switch (sortOrder)
            {
                case "name_desc":
                items = items.OrderByDescending(s => s.Title);
                break;
                case "Format":
                items = items.OrderBy(s => s.Format);
                break;
                case "format_desc":
                items = items.OrderByDescending(s => s.Format);
                break;
                case "Description":
                items = items.OrderBy(s => s.Description);
                break;
                case "description_desc":
                items = items.OrderByDescending(s => s.Description);
                break;
                case "Tags":
                items = items.OrderBy(s => s.Tags);
                break;
                case "tags_desc":
                items = items.OrderByDescending(s => s.Tags);
                break;
                case "Date":
                items = items.OrderBy(s => s.date);
                break;
                case "date_desc":
                items = items.OrderByDescending(s => s.date);
                break;
                default:
                items = items.OrderBy(s => s.Title);
                break;
            }  
            civm.Items = items;
            return View(civm);
        } 
        public async Task<FileResult> Download(int id)
        {
            DropBoxItems item = itemsRepository.Get(id);
            //byte[] fileBytes = System.IO.File.ReadAllBytes(item.Path);
            //string fileName = item.Title+item.Format;
            string path = item.Path;
            
            string fileName="";
            char[] aux = path.ToCharArray();
            for(int i=aux.Count()-1;i>0;i--)
            {
                    if(aux[i]=='/'||aux[i]=='\\')
                        i=0;
                    else
                    {
                        fileName=aux[i]+fileName;
                    }
            } 
            using (var response = await dropbox.Files.DownloadAsync(path))
            {
                //Console.WriteLine(await response.GetContentAsStringAsync());
                
                return File(await response.GetContentAsByteArrayAsync(),"application/x-msdownload", fileName);
            }
        }
        [HttpPost]  
        public async Task<IActionResult> Post(ICollection<IFormFile> files, DropBoxItems item)
        {   
            var uploads = Path.Combine(hostingEnv.WebRootPath, "Files");
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {   using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
                        {   
                            await file.CopyToAsync(fileStream);
                        }
                         var path = Path.Combine(uploads, file.FileName);
                            using (var mem = new FileStream(path, FileMode.Open))
                            {   bool cExist = false;
                                bool fExist = false;
                                
                                
                                String fName = file.FileName;
                                String aux = fName.Substring(0,(fName.IndexOf('.')));
                                String form = fName.Substring(fName.IndexOf('.'),(fName.Length-fName.IndexOf('.')));
                                
                                item.Title=aux;
                                item.Format=form;
                                item.date = System.DateTime.UtcNow.ToString();
                                item.Path = "/Uploads/"+file.FileName;
                                foreach(DropBoxCategory c in categoryRepository.GetAll())
                                {
                                    if(c.CategoryType.Equals(form))
                                    {   
                                        cExist=true;
                                        break;
                                    }
                                }
                                if(cExist==false)
                                {   
                                    DropBoxCategory category = new DropBoxCategory{CategoryType=form};
                                    categoryRepository.Save(category);
                                }
                                foreach(DropBoxItems i in itemsRepository.GetAll())
                                {   
                                    if(i.Title.Equals(item.Title))
                                    {
                                        fExist=true;
                                        itemsRepository.Delete(i);
                                        itemsRepository.Save(item);
                                        await dropbox.Files.DeleteV2Async(i.Path);
                                        break;
                                        
                                    }
                                }
                                if(fExist==false)
                                {   
                                    itemsRepository.Save(item);
                                  
                                }
                                await dropbox.Files.UploadAsync(
                                "/Uploads" + "/" + file.FileName,
                                WriteMode.Overwrite.Instance,
                                body: mem);
                            }
                            
                            System.IO.File.Delete(path);
                    }
                }
                
            return RedirectToAction("Index");
        }
        [HttpGet]
        // Update
        public IActionResult Update(int id)
        {
            DropBoxItems item = itemsRepository.Get(id);
            //ivm.Items = itemsRepository.Get(id);
            return View(item);
        }

        public IActionResult UpdateSuccess()
        {
            return View();
        }
    
        [HttpPost]
         public IActionResult Update(DropBoxItems c)
        {
            if (ModelState.IsValid)
            {
                itemsRepository.Delete(c);
                c.date=System.DateTime.UtcNow.ToString();
                itemsRepository.Save(c);
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        //Delete
        [HttpGet]
        public IActionResult Delete(int id)
        {
            DropBoxItems i = itemsRepository.Get(id);
            return View(i);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(DropBoxItems s)
        {
           bool formatExists = false;
           
            String format= s.Format;
            await dropbox.Files.DeleteV2Async(s.Path);
            itemsRepository.Delete(s);
            
            foreach(DropBoxItems i in itemsRepository.GetAll())
            {
                if(format.Equals(i.Format))
                {
                    formatExists = true;
                    break;
                }
            }
            if(formatExists==false)
            {
                foreach(DropBoxCategory c in categoryRepository.GetAll())
                {
                    if(format.Equals(c.CategoryType))
                        categoryRepository.Delete(c);
                }
            }
            
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        
        public IActionResult Upload()
        {   HttpClientHandler handler = new HttpClientHandler();
            using (var client = new HttpClient(handler, false))
            {
            return View();
            }
        }
       
        
    }
}   

