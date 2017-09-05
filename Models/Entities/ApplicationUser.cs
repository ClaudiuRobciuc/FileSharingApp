using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ConsoleApplication.Models.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
    }
}