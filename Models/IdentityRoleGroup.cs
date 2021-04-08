using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Commander.Models{

    public class HeadRoles : NumberAuditableEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int NoOfRoles { get; set; }
        
    }
    public class HeadRoles_Roles : NumberEntityField
    {
        
        [ForeignKey("HeadRoleId")]
        public virtual HeadRoles HeadRoles { get; set; }
        public long HeadRoleId { get; set; }


        [ForeignKey("RoleId")]
        public virtual IdentityRole IdentityRole { get; set; }
        public string RoleId { get; set; }

    }
}