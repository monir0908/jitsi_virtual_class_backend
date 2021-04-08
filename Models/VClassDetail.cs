using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Commander.Models{

    public class VClassDetail : NumberEntityField
    {
        
        public long VClassId { get; set; }

        [StringLength(750)]
        public string RoomId { get; set; }


        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
        public long ProjectId { get; set; }


        [ForeignKey("BatchId")]
        public virtual Batch Batch { get; set; }
        public long BatchId { get; set; }


        [ForeignKey("HostId")]
        public virtual ApplicationUser Host { get; set; }
        public string HostId { get; set; }

        [ForeignKey("ParticipantId")]
        public virtual ApplicationUser Participant { get; set; }
        public string ParticipantId { get; set; }

        [MaxLength(150)]
        public string ConnectionId { get; set;}

        public DateTime? JoinTime { get; set; }
        public DateTime? LeaveTime { get; set; }

    }
}