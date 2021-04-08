using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Commander.Models;

namespace Commander.Models{

    public class ProjectBatchHostParticipant : NumberEntityField
    {

        [ForeignKey("ProjectBatchHostId")]
        public virtual ProjectBatchHost ProjectBatchHost { get; set; }
        public long ProjectBatchHostId { get; set; }        

        [ForeignKey("ParticipantId")]
        public virtual ApplicationUser Participant { get; set; }
        public string ParticipantId { get; set; }

        public DateTime? CreatedDateTime { get; set; }
    }
}