using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Commander.Models
{
  public class ApplicationUser : IdentityUser
  {
      [Required]
        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]

        public string UserType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? LastLogOn { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DeActivateOn { get; set; }
        public string ImagePath { get; set; }
        // public bool IsAdmin { get; set; }
        // public bool IsSuperUser { get; set; }
        public long HeadRoleId { get; set; }

        [NotMapped]
        public virtual IIdentity Identity { get; set; }

        // public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        // {
        //     // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
        //     var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
        //     // Add custom user claims here
        //     return userIdentity;
        // }
  }
}