using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Commander.Models;
using Microsoft.EntityFrameworkCore;
using Commander.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Security.Principal;

namespace Commander.Services{


    public interface IRoleServices
    {
        // head Roles Related Interfaces        

        Task<object> CreateOrUpdateHeadRole(HeadRoles model, IIdentity identity);
        // Task<object> HeadRolesCreateOrUpdate(HeadRolesModel model, IIdentity identity);
        Task<object> GetHeadRoleList();
        Task<object> GetHeadRolesById(long id);
        Task<object> GetHeadRolesDropDownList();


        //Roles Related Interfaces
        Task<object> CreateRole(IdentityRole model);
        Task<object> GetRoleList();
        Task<object> GetRoleById(string id);
        Task<object> GetRolesByHeadId(long id);

        Task<object> MergeHeadRoleWithRoles(IEnumerable<HeadRoles_Roles> objs);

    }
}