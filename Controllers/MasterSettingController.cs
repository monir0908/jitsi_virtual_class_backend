using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Commander.Services;
using Commander.Models;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Commander.Controllers
{
    [Route("api/[controller]")]
    
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class MasterSettingController : ControllerBase
    {
        private IMasterSettingServices _services;

        public MasterSettingController(IMasterSettingServices services)
        {
            this._services = services;
        }

        // Project Related Endpoints

        [HttpGet, Route("GetProjectList")]
        public async Task<IActionResult> GetProjectList(int size, int pageNumber)
        {
            return Ok(await _services.GetProjectList(size,pageNumber));
        }
        
        [HttpGet, Route("GetHostListByProjectId/{projectId:long}")]
        public async Task<IActionResult> GetHostListByProjectId(long projectId)
        {
            return Ok(await _services.GetHostListByProjectId(projectId));
        }

        [HttpGet, Route("GetProjectDetailById/{projectId:long}")]
        public async Task<IActionResult> GetProjectDetailById(long projectId)
        {
            return Ok(await _services.GetProjectDetailById(projectId));
        }

        [HttpPost, Route("CreateOrUpdateProject")]
        public async Task<IActionResult> CreateOrUpdateProject(Project model)
        {
            return Ok(await _services.CreateOrUpdateProject(model));
        }


        // Batch Related Endpoints

        [HttpGet, Route("GetBatchList")]
        public async Task<IActionResult> GetBatchList(int size, int pageNumber)
        {
            return Ok(await _services.GetBatchList(size,pageNumber));
        }

        [HttpGet, Route("GetBatchDetailById/{batchId:long}")]
        public async Task<IActionResult> GetBatchDetailById(long batchId)
        {
            return Ok(await _services.GetBatchDetailById(batchId));
        }        

        [HttpPost, Route("CreateOrUpdateBatch")]
        public async Task<IActionResult> CreateOrUpdateBatch(Batch model)
        {
            return Ok(await _services.CreateOrUpdateBatch(model));
        }


        //Project Batch Related Endpoints

        [HttpGet, Route("GetMergeableBatchListByProjectId/{projectId:long}")]
        public async Task<IActionResult> GetMergeableBatchListByProjectId(long projectId)
        {
            return Ok(await _services.GetMergeableBatchListByProjectId(projectId));
        }

        [HttpPost, Route("MergeProjectBatch")]
        public async Task<IActionResult> MergeProjectBatch(IEnumerable<ProjectBatch> models)
        {
            return Ok(await _services.MergeProjectBatch(models));
        }

        [HttpPost, Route("MergeProjectBatchHost")]
        public async Task<IActionResult> MergeProjectBatchHost(IEnumerable<ProjectBatchHost> models)
        {
            return Ok(await _services.MergeProjectBatchHost(models));
        }

        [HttpPost, Route("MergeProjectBatchHostParticipant")]
        public async Task<IActionResult> JoinVirtualClassByHost(JObject objData)
        {
            dynamic jsonData = objData;
            

            long projectId = jsonData.projectId;
            long batchId = jsonData.batchId;
            string hostId = jsonData.hostId;

            JArray participantListJson = jsonData.participantList;
            var participantList = participantListJson.Select(item => item.ToObject<ParticipantList>()).ToList();
            return Ok(await _services.MergeProjectBatchHostParticipant(projectId, batchId, hostId, participantList));
        }

        [HttpGet, Route("GetHostList")]
        public async Task<IActionResult> GetHostList(int size, int pageNumber)
        {
            return Ok(await _services.GetHostList(size,pageNumber));
        }

        [HttpGet, Route("GetMergeableHostList/{projectId:long}/{batchId:long}")]
        public async Task<IActionResult> GetMergeableHostList(long projectId, long batchId, int size, int pageNumber)
        {
            return Ok(await _services.GetMergeableHostList(projectId, batchId, size,pageNumber));
        }

        [HttpGet, Route("GetAlreadyMergedHostList/{projectId:long}/{batchId:long}")]
        public async Task<IActionResult> GetAlreadyMergedHostList(long projectId, long batchId, int size, int pageNumber)
        {
            return Ok(await _services.GetAlreadyMergedHostList(projectId, batchId, size,pageNumber));
        }

        [HttpGet, Route("GetParticipantList")]
        public async Task<IActionResult> GetParticipantList(int size, int pageNumber)
        {
            return Ok(await _services.GetParticipantList(size,pageNumber));
        }

        [HttpGet, Route("GetMergeableParticipantList/{projectId:long}/{batchId:long}/{hostId}")]
        public async Task<IActionResult> GetMergeableParticipantList(long projectId, long batchId, string hostId, int size, int pageNumber)
        {
            return Ok(await _services.GetMergeableParticipantList(projectId, batchId, hostId, size,pageNumber));
        }

        [HttpGet, Route("GetAlreadyMergedParticipantList/{projectId:long}/{batchId:long}/{hostId}")]
        public async Task<IActionResult> GetAlreadyMergedParticipantList(long projectId, long batchId, string hostId, int size, int pageNumber)
        {
            return Ok(await _services.GetAlreadyMergedParticipantList(projectId, batchId, hostId, size,pageNumber));
        }


    }
}
