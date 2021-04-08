using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Commander.Models;
using Microsoft.EntityFrameworkCore;
using Commander.Common;
using Microsoft.Extensions.Configuration;

namespace Commander.Services{


    public interface IMasterSettingServices
    {
        // Project Related Interfaces
        Task<object> GetProjectList(int size, int pageNumber);
        Task<object> GetProjectDetailById(long projectId);
        Task<object> CreateOrUpdateProject(Project model);

        // Batch Related Interfaces
        Task<object> GetBatchList(int size, int pageNumber);
        Task<object> GetBatchDetailById(long batchId);
        Task<object> CreateOrUpdateBatch(Batch model);


        //Project Batch Related Interfaces
        Task<object> GetMergeableBatchListByProjectId(long projectId);
        Task<object> MergeProjectBatch(IEnumerable<ProjectBatch> models);
        Task<object> MergeProjectBatchHost(IEnumerable<ProjectBatchHost> models);
        Task<object> GetHostList(int size, int pageNumber);
        Task<object> GetHostListByProjectId(long projectId);
        Task<object> GetMergeableHostList(long projectId, long batchId, int size, int pageNumber);
        Task<object> GetAlreadyMergedHostList(long projectId, long batchId, int size, int pageNumber);
        Task<object> GetParticipantList(int size, int pageNumber);
        Task<object> GetMergeableParticipantList(long projectId, long batchId, string hostId, int size, int pageNumber);
        Task<object> GetAlreadyMergedParticipantList(long projectId, long batchId, string hostId, int size, int pageNumber);
        Task<object> MergeProjectBatchHostParticipant(long projectId, long batchId, string hostId, IEnumerable<ParticipantList> participantList);
        




    }
}