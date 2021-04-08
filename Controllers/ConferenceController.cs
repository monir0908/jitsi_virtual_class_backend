using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Commander.Services;
using Commander.Models;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;

namespace Commander.Controllers
{
    [Route("api/[controller]")]
    
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class ConferenceController : ControllerBase
    {
        private IConferenceServices _services;

        public ConferenceController(IConferenceServices services)
        {
            this._services = services;
        }

        // Host Side : VClass Related Endpoints

        [HttpGet, Route("GetProjectListByHostId/{hostId}")]
        public async Task<IActionResult> GetProjectListByHostId(string hostId)
        {
            return Ok(await _services.GetProjectListByHostId(hostId));
        }

        [HttpGet, Route("GetBatchListByProjectIdAndHostId/{projectId:long}/{hostId}")]
        public async Task<IActionResult> GetBatchListByProjectIdAndHostId(long projectId, string hostId)
        {
            return Ok(await _services.GetBatchListByProjectIdAndHostId(projectId, hostId));
        }

        [HttpGet, Route("GetParticipantListByProjectIdBatchIdAndHostId/{projectId:long}/{batchId:long}/{hostId}")]
        public async Task<IActionResult> GetParticipantListByProjectIdBatchIdAndHostId(long projectId, long batchId, string hostId)
        {
            return Ok(await _services.GetParticipantListByProjectIdBatchIdAndHostId(projectId, batchId, hostId));
        }

        [HttpGet, Route("GetCurrentOnGoingVirtualClassListByHostId/{hostId}")]
        public async Task<IActionResult> GetCurrentOnGoingVirtualClassListByHostId(string hostId)
        {
            return Ok(await _services.GetCurrentOnGoingVirtualClassListByHostId(hostId));
        }

        [HttpPost, Route("CreateVirtualClass")]
        public async Task<IActionResult> CreateVirtualClass(VClass vClassObj)
        {
            return Ok(await _services.CreateVirtualClass(vClassObj));
        }

        [HttpPost, Route("JoinVirtualClassByHost")]
        public async Task<IActionResult> JoinVirtualClassByHost(JObject objData)
        {

            dynamic jsonData = objData;
            JObject vClassDetailJson = jsonData.vClassDetail;
            var vClassDetail = vClassDetailJson.ToObject<VClassDetail>();

            JArray participantListJson = jsonData.participantList;
            var participantList = participantListJson.Select(item => item.ToObject<ParticipantList>()).ToList();
            return Ok(await _services.JoinVirtualClassByHost(vClassDetail, participantList));
        }

        [HttpPost, Route("EndVirtualClassByHost")]
        public async Task<IActionResult> EndVirtualClassByHost(VClass vClassObj)
        {
            return Ok(await _services.EndVirtualClassByHost(vClassObj));
        }


        // Host Side: Miscellaneous Endpoints

        [HttpGet, Route("GetBatchListByProjectId/{pId:long}")]
        public async Task<IActionResult> GetBatchListByProjectId(long pId)
        {
            return Ok(await _services.GetBatchListByProjectId(pId));
        }

        [HttpGet, Route("GetVirtualClassCallingDetailByDaterange")]
        public async Task<IActionResult> GetVirtualClassCallingDetailByDaterange(DateTimeParams obj)
        {
            return Ok(await _services.GetVirtualClassCallingDetailByDaterange(obj));
        }

        [HttpGet, Route("GetVirtualClassCallingDetailByHostId/{hostId}")]
        public async Task<IActionResult> GetVirtualClassCallingDetailByHostId(string hostId)
        {
            return Ok(await _services.GetVirtualClassCallingDetailByHostId(hostId));
        }

        [HttpGet, Route("GetVirtualClassCallingDetail")]
        public async Task<IActionResult> GetVirtualClassCallingDetail(long projectId, long batchId, string hostId, DateTime startDate, DateTime endDate)
        {
            return Ok(await _services.GetVirtualClassCallingDetail(projectId, batchId, hostId, startDate, endDate));
        }

        [HttpGet, Route("GetVirtualClassDetailById/{vclassId}")]
        public async Task<IActionResult> GetVirtualClassDetailById(long vclassId)
        {
            return Ok(await _services.GetVirtualClassDetailById(vclassId));
        }



        // Participant Side : VClass Related Endpoints

        [HttpGet, Route("GetInvitationListByParticipantId/{participantId}")]
        public async Task<IActionResult> GetInvitationListByParticipantId(string participantId)
        {
            return Ok(await _services.GetInvitationListByParticipantId(participantId));
        }

        [HttpPost, Route("JoinVirtualClassByParticipant")]
        public async Task<IActionResult> JoinVirtualClassByParticipant(VClassDetail vClassDetail)
        {
            return Ok(await _services.JoinVirtualClassByParticipant(vClassDetail));
        }

        [HttpPost, Route("EndVirtualClassByParticipant")]
        public async Task<IActionResult> EndVirtualClassByParticipant(VClassDetail vClassDetail)
        {
            return Ok(await _services.EndVirtualClassByParticipant(vClassDetail));
        }
        
        
        
        // Test Endpoints

        [HttpGet, Route("TestApi")]
        public async Task<IActionResult> TestApi(long projectId, long batchId, string hostId, DateTime startDate, DateTime endDate)
        {
            return Ok(await _services.TestApi(projectId, batchId, hostId, startDate, endDate));
        }
        


    }
}
