using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Commander.Models;
using Microsoft.EntityFrameworkCore;
using Commander.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.SignalR;
using Itenso.TimePeriod;
using Microsoft.AspNetCore.Identity;
using System.Security.Principal;

namespace Commander.Services{

    

    public class UserServices : IUserServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<SignalHub> _notificationHubContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager; 
       

        public UserServices(ApplicationDbContext context, IHubContext<SignalHub> hubContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this._context = context;
            this._notificationHubContext = hubContext;
            this.userManager = userManager; 
            this.roleManager = roleManager;
        }

        public async Task<object> GetUserList(int size, int pageNumber)
        {
            try
            {
                var data = await _context.Users.OrderByDescending(x => x.Id).Select(x => new 
                { 
                    x.Id, 
                    x.FirstName, 
                    x.LastName, 
                    x.Email, 
                    x.PhoneNumber,
                    x.UserName,
                    x.UserType,
                })
                .Skip(pageNumber * size)
                .Take(size)
                .ToListAsync();

                var count = await _context.Users.CountAsync();

                return new
                {
                    Success = true,
                    Records = data,
                    Total = count
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    Success = false,
                    Message = ex.InnerException?.Message ?? ex.Message
                };
            }
        }

        public async Task<object> CreateUser(ApplicationUser model, IEnumerable<RoleList> roleList)
        {
            string[] roles = roleList.Select(x => x.Name).ToArray();

            ApplicationUser item = null;
            ApplicationUser user = null;
            IdentityResult result = null;
            bool isEdit = true;
            try
            {
                if (await _context.Users.AnyAsync(x => x.Id != model.Id && x.Email == model.Email))
                    throw new Exception("Already exists: " + model.Email);

                if (model.Id != null)
                {
                    item = await _context.Users.FindAsync(model.Id);
                    if (item == null) throw new Exception("User not found");
                }
                else
                {
                    item = new ApplicationUser();
                    isEdit = false;
                }

                
                item.FirstName = model.FirstName;
                item.LastName = model.LastName;
                item.PhoneNumber = model.PhoneNumber;
                item.Email = model.Email;
                item.UserType = model.UserType;
                item.HeadRoleId = model.HeadRoleId;

                try
                    {
                        if (!isEdit)
                        {
                            item.Id = Guid.NewGuid().ToString();
                            user = new ApplicationUser() { 
                                UserName = item.Email, 
                                PhoneNumber = item.PhoneNumber, 
                                Email = item.Email, 
                                FirstName = item.FirstName, 
                                LastName = item.LastName, 
                                UserType = item.UserType, 
                                HeadRoleId = item.HeadRoleId, 
                                IsActive = true, 
                            };

                            
                            // _context.Users.Add(item);
                            var password = "BacBon@admin";

                            result = await userManager.CreateAsync(user,password);
                            if (!result.Succeeded) throw new Exception("Error on user creation: <br/>" + string.Join("<br/>", result.Errors));

                            if (roles.Any())
                            {
                                result = await userManager.AddToRolesAsync(user, roles);
                                if (!result.Succeeded) throw new Exception("Error on user roles asign: <br/>" + string.Join("<br/>", result.Errors));
                            }
                        }
                        else
                        {
                            user = await userManager.FindByIdAsync(item.Id);
                            if (user == null) throw new Exception("User not found for this employee");

                            user.UserName = item.Email;
                            user.Email = item.Email;
                            user.FirstName = item.FirstName;
                            user.LastName = item.LastName;
                            user.PhoneNumber = item.PhoneNumber;
                            user.HeadRoleId = item.HeadRoleId;
                            result = await userManager.UpdateAsync(user);
                            if (!result.Succeeded) throw new Exception("Error on user update: " + string.Join(", ", result.Errors));

                            var previousRoles = await userManager.GetRolesAsync(user);
                            result = await userManager.RemoveFromRolesAsync(user, previousRoles.ToArray());
                            if (!result.Succeeded) throw new Exception("Error on previous roles remove: " + string.Join(", ", result.Errors));

                            result = await userManager.AddToRolesAsync(user, roles);
                            if (!result.Succeeded) throw new Exception("Error on new roles assign: " + string.Join(", ", result.Errors));

                            _context.Entry(item).State = EntityState.Modified;
                        }
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        return new
                        {
                            Success = false,
                            Message = ex.InnerException?.Message ?? ex.Message
                        };
                    }

               

                return new
                {
                    Success = true,
                    Message = "Successfully " + (isEdit ? "Updated" : "Saved"),
                    item.Id
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    Success = false,
                    ex.Message
                };
            }
        }
        
        public async Task<object> GetUserDetailWithRoles(string id)
        {
            try
            {
                var user = await _context.Users
                .FindAsync(id);

                if (user == null) throw new Exception("No User Found");

                var roles = await userManager.GetRolesAsync(user);


                var data = new {
                    user.Id,
                    user.UserType,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.HeadRoleId,
                    user.PhoneNumber,

                };


                

                return new
                {
                    Success = true,
                    Record = new { User = data, Roles = roles.ToArray() }
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    Success = false,
                    Message = ex.InnerException?.Message ?? ex.Message
                };
            }
        }
        
    }
}


