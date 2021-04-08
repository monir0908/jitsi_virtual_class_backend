using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Commander.Models;
using Microsoft.EntityFrameworkCore;
using Commander.Common;
using Microsoft.Extensions.Configuration;

namespace Commander.Services{


    public interface IConferenceServices
    {
        
        // Host Side : VClass Related Interfaces
        Task<object> GetProjectListByHostId(string hostId);       
        Task<object> GetBatchListByProjectIdAndHostId(long projectId, string hostId);
        Task<object> GetParticipantListByProjectIdBatchIdAndHostId(long projectId, long batchId, string hostId);
        Task<object> GetCurrentOnGoingVirtualClassListByHostId(string hostId);
        Task<object> CreateVirtualClass(VClass vClassObj);
        Task<object> JoinVirtualClassByHost(VClassDetail vClassDetail, IEnumerable<ParticipantList> participantList);
        Task<object> EndVirtualClassByHost(VClass vClassObj);


        // Host Side: Miscellaneous Interfaces
        Task<object> GetBatchListByProjectId(long projectId);
        Task<object> GetVirtualClassCallingDetailByDaterange(DateTimeParams obj);
        Task<object> GetVirtualClassCallingDetailByHostId(string hostId);
        Task<object> GetVirtualClassCallingDetail(long projectId, long batchId, string hostId, DateTime startDate, DateTime endDate);
        Task<object> GetVirtualClassDetailById(long vclassId);

        
        // Participant Side : VClass Related Interfaces
        Task<object> GetInvitationListByParticipantId(string participantId);
        Task<object> JoinVirtualClassByParticipant(VClassDetail vClassDetail);
        Task<object> EndVirtualClassByParticipant(VClassDetail vClassDetail);


        // Test Services
        Task<object> TestApi(long projectId, long batchId, string hostId, DateTime startDate, DateTime endDate);




    }
}