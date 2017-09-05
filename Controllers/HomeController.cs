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


namespace ConsoleApplication.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
       
        private IHostingEnvironment hostingEnv;
        private ICategoryRepository categoryRepository;
        private IItemsRepository itemsRepository;
        //multiple items with multiple categories
        private CategoriesItemsViewModel civm = new CategoriesItemsViewModel(); 
        
        //Single Item with the possibility of more categories
        private ItemViewModel ivm = new ItemViewModel();
        
        public HomeController(ICategoryRepository categoryRepository, IItemsRepository itemsRepository, IHostingEnvironment env)
        {
            this.categoryRepository = categoryRepository;
            this.itemsRepository = itemsRepository;  
            civm.Categories = categoryRepository.GetAll().ToList();
            civm.Items = itemsRepository.GetAll();  
            ivm.Categories = categoryRepository.GetAll().ToList();      
            this.hostingEnv = env;
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
            
            foreach(Category c in categoryRepository.GetAll())
                madeTrue.Add(false);
            try{
            if(!String.IsNullOrEmpty(format[0]))
            {
                int k=0;
                int n=0;

                ViewData["FormatFilter"] = format;
                List<Items> aux = new List<Items>();
                List<Category> category = categoryRepository.GetAll().ToList();
                foreach(var selection in format)
                {
                    foreach(Items i in itemsRepository.GetAll())
                    {
                        if(i.Format.Equals(selection))
                            aux.Add(i);
                    }
                    /*Category c = categoryRepository.GetByName(selection);
                    categoryRepository.Delete(c);
                    c.Selected=true;
                    categoryRepository.Save(c);*/
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
                foreach(Category c in categoryRepository.GetAll())
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

        public FileResult Download(int id)
        {
            Items item = itemsRepository.Get(id);
            byte[] fileBytes = System.IO.File.ReadAllBytes(item.Path);
            string fileName = item.Title+item.Format;
            return File(fileBytes,"application/x-msdownload", fileName);
        }

        [HttpGet]
        // Update
        public IActionResult Update(int id)
        {
            ivm.Items = itemsRepository.Get(id);
            return View(ivm);
        }
    
        [HttpPost]
         public IActionResult Update(ItemViewModel c)
        {
            if (ModelState.IsValid)
            {
                c.Items.date=System.DateTime.Now.ToString();
                itemsRepository.Update(c.Items);
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        //Delete
        [HttpGet]
        
        public IActionResult Upload()
        {
            return View();
        }

        public IActionResult AddLink()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddLink(ItemViewModel i)
        {
                bool cExist = false;
                i.Items.Format="Link";
                i.Items.date=System.DateTime.Now.ToString();
                itemsRepository.Save(i.Items);
                foreach(Category c in categoryRepository.GetAll())
                        {
                            if(c.CategoryType.Equals(i.Items.Format))
                            {   
                                cExist=true;
                                break;
                            }
                        }
                        if(cExist==false)
                        {   
                            Category category = new Category{CategoryType=i.Items.Format, Selected=false};
                            categoryRepository.Save(category);
                        }
                return RedirectToAction("Index");
        }
      
       [HttpPost]  
        public async Task<IActionResult> Post(ICollection<IFormFile> files, ItemViewModel item)
        {   
            
            var uploads = Path.Combine(hostingEnv.WebRootPath, "Files");
            int id=0;
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
                    {   
                        bool cExist=false;
                        bool fExist=false;
                        String fName = file.FileName;
                        String aux = fName.Substring(0,(fName.IndexOf('.')));
                        String form = fName.Substring(fName.IndexOf('.'),(fName.Length-fName.IndexOf('.')));
                        //item = new Items{Title=aux,Format=form, Path=Path.Combine(uploads,file.FileName),date=System.DateTime.Now.ToString()};
                        item.Items.Title=aux;
                        item.Items.Format=form;
                        item.Items.Path = Path.Combine(uploads,file.FileName);
                        item.Items.date = System.DateTime.Now.ToString();
                        foreach(Category c in categoryRepository.GetAll())
                        {
                            if(c.CategoryType.Equals(form))
                            {   
                                cExist=true;
                                break;
                            }
                        }
                        if(cExist==false)
                        {   
                            Category category = new Category{CategoryType=form};
                            categoryRepository.Save(category);
                        }
                        //itemsRepository.Save(item);
                        
                        await file.CopyToAsync(fileStream);
                        foreach(Items i in itemsRepository.GetAll())
                        {   
                            if(i.Title.Equals(item.Items.Title))
                            {
                                fExist=true;
                                itemsRepository.Delete(i);
                                itemsRepository.Save(item.Items);
                                return RedirectToAction("Index");
                            }
                        }
                        if(fExist==false)
                        {   
                            itemsRepository.Save(item.Items);
                            id=itemsRepository.GetLastItem().ItemID;
                        }
                    }
                }
            }
            //return RedirectToAction("Update", new{ id });
            
            return RedirectToAction("Index");
        }
    }
}   

