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
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Principal;
using System.Transactions;

namespace Commander.Services{

    

    public class RoleServices : IRoleServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<SignalHub> _notificationHubContext;
        private RoleManager<IdentityRole> _roleManager;

        public RoleServices(ApplicationDbContext context, IHubContext<SignalHub> hubContext, RoleManager<IdentityRole> roleManager)
        {
            this._context = context;
            this._notificationHubContext = hubContext;
            this._roleManager = roleManager;
        }
        public async Task<object> CreateOrUpdateHeadRole(HeadRoles model, IIdentity identity)
        {
            
            HeadRoles item = null;
            bool isEdit = true;
            try
            {
                if (await _context.HeadRoles.AnyAsync(x => x.Id != model.Id && x.Name == model.Name))
                    throw new Exception("Head Role Name already exists !");

                

                if (model.Id >0)
                {
                    item = await _context.HeadRoles.FindAsync(model.Id);
                    if (item == null) throw new Exception("Head Role not found to update !");
                }
                else
                {
                    item = new HeadRoles();
                    isEdit = false;                    
                }


               
                item.Name = model.Name;
                item.Description = model.Description;

                item.SetAuditTrailEntity(identity);    

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        if (!isEdit)
                        {
                            _context.HeadRoles.Add(item);
                        }
                        else
                        {
                            _context.Entry(item).State = EntityState.Modified;
                        }
                        await _context.SaveChangesAsync();
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();
                        throw (ex);
                    }

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
                    Message = ex.InnerException?.Message ?? ex.Message
                };
            }
        }


        

        public async Task<object> GetHeadRoleList()
        {
            try
            {
                // var data = await _context.HeadRoles.OrderByDescending(x => x.Name)
                //     .GroupJoin(_context.HeadRoles_Roles, x => x.Id, y => y.HeadRoleId, (x, y) => new
                //     {
                //         HeadRoles = x,
                //         Roles = y
                //     }).Select(x => new { x.HeadRoles.Id, x.HeadRoles.Name, x.HeadRoles.Description, x.HeadRoles.NoOfRoles, Roles = x.Roles.Select(y => y.Id).ToList() }).Skip(pageNumber * size).Take(size).ToListAsync();
                
                // HeadRoles, HeadRoles_Roles, Roles
                var data = 
                _context.HeadRoles
                .Select( hr => new{
                    hr.Id,
                    hr.Name,
                    hr.Description,
                    hr.NoOfRoles,

                    Roles = _context.HeadRoles_Roles
                    .Where(x => x. HeadRoleId == hr.Id)
                    .Join(_context.Roles,
                    x => x.RoleId,
                    y => y.Id,
                    (x, y)=> new {HeadRoles = x, Roles = y})
                    .Select(x => x.Roles.Name).ToList(),

                })
                .ToList();
            
            
                
                var count = await _context.HeadRoles.CountAsync();

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

        public async Task<object> GetHeadRolesById(long id)
        {
            try
            {
                var data = await _context.HeadRoles.Where(t => t.Id == id)                     
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.Description,
                }).FirstOrDefaultAsync();
                return new
                {
                    Success = true,
                    Record = data
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

        public async Task<object> GetHeadRolesDropDownList()
        {
            try
            {
                var data = await _context.HeadRoles.OrderBy(o => o.Name)
                .Select(t => new
                {
                    t.Id,
                    t.Name
                }).ToListAsync();

                return new
                {
                    Success = true,
                    Records = data
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

        public async Task<object> CreateRole(IdentityRole model)
        {
            bool isEdit = true;
            IdentityRole item = null;
            try
            {

                if (model.Id == null)
                {
                    item = await _roleManager.FindByNameAsync(model.Name);
                    if (item != null) throw new Exception("Role exists !");
                }


                if (model.Id != null)
                {
                    item = await _roleManager.FindByIdAsync(model.Id);
                    if (item == null) throw new Exception("Role not found to update !");
                }                
                else
                {
                    item = new IdentityRole();
                    isEdit = false;                    
                }

                item.Name = model.Name;

                if (!isEdit)
                {
                    await _roleManager.CreateAsync(new IdentityRole(model.Name));
                }
                else
                {
                    await _roleManager.UpdateAsync(new IdentityRole(model.Name));
                    // _context.Entry(item).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();            
                

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
                    Message = ex.InnerException?.Message ?? ex.Message
                };
            }
        }


        public async Task<object> GetRoleList()
        {
            try
            {
                var data = await _context.Roles.OrderBy(o => o.Name)
                .Select(t => new { t.Name, t.Id}).ToListAsync();

                // var data = list.Select(x => new { 
                //     Id = x, 
                //     Name = x.Replace("_", " ") 
                // })
                // .ToList();

                return new
                {
                    Success = true,
                    Records = data
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
        
        public async Task<object> GetRoleById(string id)
        {
            try
            {
                var data = await _context.Roles.Where(w => w.Id == id)
                .Select(t => new{
                    t.Id,
                    t.Name,
                }).FirstOrDefaultAsync();

                return new
                {
                    Success = true,
                    Record = data
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
        public async Task<object> GetRolesByHeadId(long id)
        {
            try
            {
                var data = await _context.HeadRoles_Roles.Where(w => w.HeadRoleId == id)
                .Join(_context.Roles,
                x => x.RoleId,
                y => y.Id,
                (x,y) => new{ HeadRoles_Roles = x, Roles = y})
                .Select(t => new{
                    t.Roles.Id,
                    t.Roles.Name
                }).ToListAsync();

                return new
                {
                    Success = true,
                    Records = data
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
        
        public async Task<object> MergeHeadRoleWithRoles(IEnumerable<HeadRoles_Roles> objs){

            long headRoleId = objs.Select(x => x.HeadRoleId).FirstOrDefault();
            
            try
            {
                // Removing first
                _context.HeadRoles_Roles.RemoveRange(_context.HeadRoles_Roles.Where(x => x.HeadRoleId == headRoleId));
                _context.SaveChanges(); 

                // Then, adding
                foreach (var item in objs)
                {
                    _context.HeadRoles_Roles.Add(item);                   
                    await _context.SaveChangesAsync();
                }

                return new
                {
                    Success = true,
                    Message = "Successfully merged",
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


