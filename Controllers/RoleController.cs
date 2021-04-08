using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Commander.Services;
using Commander.Models;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;
using Microsoft.AspNetCore.Identity;

namespace Commander.Controllers
{
    [Route("api/[controller]")]
    
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class RoleController : ControllerBase
    {
        private IRoleServices _services;

        public RoleController(IRoleServices services)
        {
            this._services = services;
        }


        [HttpPost, Route("CreateOrUpdateHeadRole")]
        public async Task<IActionResult> CreateOrUpdateHeadRole(HeadRoles model)
        {
            return Ok(await _services.CreateOrUpdateHeadRole(model, User.Identity));
        }

        // Roles Related Endpoints

        [HttpGet, Route("GetHeadRoleList")]
        public async Task<IActionResult> GetHeadRoleList()
        {
            return Ok(await _services.GetHeadRoleList());
        }

        [HttpGet, Route("GetHeadRolesById/{id}")]
        public async Task<IActionResult> GetHeadRolesById(long id)
        {
            return Ok(await _services.GetHeadRolesById(id));
        }

        [HttpGet, Route("dropdown-list")]
        public async Task<IActionResult> GetHeadRolesDropDownList()
        {
            return Ok(await _services.GetHeadRolesDropDownList());
        }

        //Roles Related Endpoints

        [HttpPost, Route("CreateRole")]
        public async Task<IActionResult> CreateRole(IdentityRole model)
        {
            return Ok(await _services.CreateRole(model));
        }

        [HttpGet, Route("GetRoleList")]
        public async Task<IActionResult> GetRoleList()
        {
            return Ok(await _services.GetRoleList());
        }

        [HttpGet, Route("GetRoleById/{id}")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            return Ok(await _services.GetRoleById(id));
        }

        [HttpGet, Route("GetRolesByHeadId/{id:long}")]
        public async Task<IActionResult> GetRolesByHeadId(long id)
        {
            return Ok(await _services.GetRolesByHeadId(id));
        }

        
        [HttpPost, Route("MergeHeadRoleWithRoles")]
        public async Task<IActionResult> MergeHeadRoleWithRoles(IEnumerable<HeadRoles_Roles> objs)
        {
            return Ok(await _services.MergeHeadRoleWithRoles(objs));
        }
        


    }
}
