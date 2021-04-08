using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Commander{

    public class DateTimeParams
    {        
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }

    public class CallDurationHistory
    {        
        public TimeSpan? HostCallDuration { get; set; }
        public TimeSpan? ParticipantsCallDuration { get; set; }
        public TimeSpan? EmptySlotDuration { get; set; }
        public TimeSpan? ActualCallDuration { get; set; }
        public int ParticipantJoined { get; set; }
        public int UniqueParticipantCounts { get; set; }
    }

    public class ParticipantList
    {   
        public string Id{ get; set; }
    }


    public class Pagination
    {   
        public int Size{ get; set; }
        public int PageNumber{ get; set; }
    }

    public class HeadRolesModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual List<string> Roles { get; set; }
    }


    public class RoleList
    {   
        public string Name{ get; set; }
    }
}