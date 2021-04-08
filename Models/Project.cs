using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Commander.Models{

    public class Project : NumberEntityField
    {

        [Required, StringLength(750)]
        public string ProjectName { get; set; }

        public DateTime? CreatedDateTime { get; set; }
    }
}