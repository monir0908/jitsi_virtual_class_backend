using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Commander.Models{

    public class VClass : NumberEntityField
    {
        [StringLength(750)]
        public string RoomId { get; set; }


        [ForeignKey("HostId")]
        public virtual ApplicationUser Host { get; set; }
        public string HostId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
        public long ProjectId { get; set; }

        [ForeignKey("BatchId")]
        public virtual Batch Batch { get; set; }
        public long BatchId { get; set; }
        

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? ScheduleDateTime { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }  //On-Going, Finished

        [NotMapped]
        public string ConnectionId { get; set;}

        public TimeSpan? HostCallDuration {get;set;}
        public TimeSpan? ParticipantsCallDuration {get;set;}
        public TimeSpan? EmptySlotDuration {get;set;}
        public TimeSpan? ActualCallDuration {get;set;}
        public int ParticipantJoined {get;set;}
        public int UniqueParticipantCounts {get;set;}



    }
}