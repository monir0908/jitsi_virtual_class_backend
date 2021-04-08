using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Commander.Models{

    public class Conference : NumberEntityField
    {
        [StringLength(750)]
        public string RoomId { get; set; }


        [ForeignKey("HostId")]
        public virtual ApplicationUser Host { get; set; }
        public string HostId { get; set; }

        [ForeignKey("ParticipantId")]
        public virtual ApplicationUser Participant { get; set; }
        public string ParticipantId { get; set; }

        [ForeignKey("BatchId")]
        public virtual Batch Batch { get; set; }
        public long BatchId { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }  //On-Going, Finished
        
        public bool HasJoinedByHost { get; set;}
        public bool HasJoinedByParticipant { get; set;}

        [NotMapped]
        public string ConnectionId { get; set;}


    }
}