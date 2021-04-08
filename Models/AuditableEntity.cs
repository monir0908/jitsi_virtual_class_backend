using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public class AuditableEntity : EntityField
    {
        public DateTime CreatedDate { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public void SetAuditTrailEntity(IIdentity identity)
        {
            if (this.Id == Guid.Empty)
            {
                this.CreatedDate = DateTime.UtcNow;
                this.CreatedBy = identity.Name;
            }
            else
            {
                this.UpdatedDate = DateTime.UtcNow;
                this.UpdatedBy = identity.Name;
            }
        }
    }

    public class NumberAuditableEntity : NumberEntityField
    {
        public DateTime CreatedDate { get; set; }

        
        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public void SetAuditTrailEntity(IIdentity identity)
        {
            if (this.Id == 0)
            {
                this.CreatedDate = DateTime.UtcNow;
                this.CreatedBy = identity.Name;
            }
            else
            {
                this.UpdatedDate = DateTime.UtcNow;
                this.UpdatedBy = identity.Name;
            }
        }
    }
}
