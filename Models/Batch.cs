using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Commander.Models{

    public class Batch : NumberEntityField
    {


        [Required, StringLength(750)]
        public string BatchName { get; set; }

        public DateTime? CreatedDateTime { get; set; }
    }
}