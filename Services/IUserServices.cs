using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Commander.Models;
using Microsoft.EntityFrameworkCore;
using Commander.Common;
using Microsoft.Extensions.Configuration;

namespace Commander.Services{


    public interface IUserServices
    {
        // User Related Interfaces
        
        Task<object> GetUserList(int size, int pageNumber);
        Task<object> CreateUser(ApplicationUser model, IEnumerable<RoleList> roleList);
        Task<object> GetUserDetailWithRoles(string id);

    }
}