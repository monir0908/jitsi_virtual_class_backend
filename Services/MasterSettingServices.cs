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
using System.Security.Principal;

namespace Commander.Services{

    public class MasterSettingServices : IMasterSettingServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<SignalHub> _notificationHubContext;

        public MasterSettingServices(ApplicationDbContext context, IHubContext<SignalHub> hubContext)
        {
            this._context = context;
            this._notificationHubContext = hubContext;
        }


        // Project Related Services
        public async Task<object> GetProjectList(int size, int pageNumber)
        {

            try
            {
                
                var query =  _context.Project.AsQueryable();
                var data = await query
                .Select(x => new
                {
                    x.Id,
                    x.ProjectName
                })
                .Skip(pageNumber * size).Take(size)
                .ToListAsync();

                var count = data.Count;

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
                    ex.Message
                };
            }
        }

        public async Task<object> GetProjectDetailById(long projectId)
        {

            try
            {
                
                var query =  _context.Project.Where(x => x.Id == projectId).AsQueryable();
                var data = await query
                .Select(x => new
                {
                    x.Id,
                    x.ProjectName
                })
                .FirstOrDefaultAsync();

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

        public async Task<object> CreateOrUpdateProject(Project model)
        {
            
            Project item = null;
            bool isEdit = true;
            try
            {
                if (await _context.Project.AnyAsync(x => x.Id != model.Id && x.ProjectName == model.ProjectName))
                    throw new Exception("Project already exists !");

                

                if (model.Id >0)
                {
                    item = await _context.Project.FindAsync(model.Id);
                    if (item == null) throw new Exception("Project not found to update !");
                }
                else
                {
                    item = new Project { ProjectName = model.ProjectName };
                    isEdit = false;
                }                

                if (!isEdit)
                {                    
                    _context.Project.Add(item);
                }
                else
                {
                    item.ProjectName = model.ProjectName;
                    _context.Entry(item).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();
                
                return new
                {
                    Success = true,
                    Message = "Successfully " + (isEdit ? "Updated" : "Created"),
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

        
        // Batch Related Services
        public async Task<object> GetBatchList(int size, int pageNumber)
        {

            try
            {
                
                var query =  _context.Batch.AsQueryable();
                var data = await query
                .Select(x => new
                {
                    x.Id,
                    x.BatchName
                })
                .Skip(pageNumber * size).Take(size)
                .ToListAsync();

                var count = data.Count;

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
                    ex.Message
                };
            }
        }

        public async Task<object> GetBatchDetailById(long batchId)
        {

            try
            {
                
                var query =  _context.Batch.Where(x => x.Id == batchId).AsQueryable();
                var data = await query
                .Select(x => new
                {
                    x.Id,
                    x.BatchName
                })
                .FirstOrDefaultAsync();

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

        public async Task<object> CreateOrUpdateBatch(Batch model)
        {
            
            Batch item = null;
            bool isEdit = true;
            try
            {
                if (await _context.Batch.AnyAsync(x => x.Id != model.Id && x.BatchName == model.BatchName))
                    throw new Exception("Batch already exists !");

                

                if (model.Id >0)
                {
                    item = await _context.Batch.FindAsync(model.Id);
                    if (item == null) throw new Exception("Batch not found to update !");
                }
                else
                {
                    item = new Batch { BatchName = model.BatchName };
                    isEdit = false;
                }                

                if (!isEdit)
                {                    
                    _context.Batch.Add(item);
                }
                else
                {
                    item.BatchName = model.BatchName;
                    _context.Entry(item).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();
                
                return new
                {
                    Success = true,
                    Message = "Successfully " + (isEdit ? "Updated" : "Created"),
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

        

        //Project Batch Related Services
        public async Task<object> GetMergeableBatchListByProjectId(long projectId)
        {

            try
            {
                long[] batchIds = _context.ProjectBatch
                .Where(x => x.ProjectId == projectId)                
                .Select(x => x.BatchId).ToArray();



                var query =  _context.Batch.Where(x => !batchIds.Contains(x.Id)).AsQueryable();
                var data = await query
                .Select(x => new
                {
                    x.Id,
                    x.BatchName
                })
                .ToListAsync();

                var count = data.Count;

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
                    ex.Message
                };
            }
        }

        public async Task<object> MergeProjectBatch(IEnumerable<ProjectBatch> models)
        {
            
            try
            {
                foreach (var item in models)
                {
                    item.CreatedDateTime = DateTime.UtcNow;
                    _context.ProjectBatch.Add(item);                    
                }

                await _context.SaveChangesAsync();

                
                
                return new
                {
                    Success = true,
                    Message = "Successfully Merged",
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

        public async Task<object> MergeProjectBatchHost(IEnumerable<ProjectBatchHost> models)
        {
            
            
            try
            {
                long projectId = models.Select(x => x.ProjectId).FirstOrDefault();
                long batchId = models.Select(x => x.BatchId).FirstOrDefault();
                long projectBatchId = _context.ProjectBatch.Where(x => x.ProjectId == projectId && x.BatchId == batchId).Select(x => x.Id).FirstOrDefault();
                

                foreach (var item in models)
                {
                    item.ProjectBatchId = projectBatchId;
                    item.CreatedDateTime = DateTime.UtcNow;
                    _context.ProjectBatchHost.Add(item); 
                }

                await _context.SaveChangesAsync();

                return new
                {
                    Success = true,
                    Message = "Successfully Merged",
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

        public async Task<object> MergeProjectBatchHostParticipant(long projectId, long batchId, string hostId, IEnumerable<ParticipantList> participantList)
        {
            
            
            try
            {
                
                long projectBatchhostId = _context.ProjectBatchHost.Where(x => x.ProjectId == projectId && x.BatchId == batchId && x.HostId == hostId).Select(x => x.Id).FirstOrDefault();
                
                
                foreach (var item in participantList)
                {
                    ProjectBatchHostParticipant obj = new ProjectBatchHostParticipant();
                    obj.ProjectBatchHostId = projectBatchhostId;
                    obj.CreatedDateTime = DateTime.UtcNow;
                    obj.ParticipantId = item.Id;
                    _context.ProjectBatchHostParticipant.Add(obj); 
                }

                await _context.SaveChangesAsync();

                return new
                {
                    Success = true,
                    Message = "Successfully Merged",
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

        public async Task<object> GetHostList(int size, int pageNumber)
        {

            try
            {
                
                var data = await _context.Users
                .Where(x => x.UserType == "Host")              
                .Select(x => new
                {
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.PhoneNumber,
                    x.Email,
                })
                .Skip(pageNumber * size).Take(size)
                .ToListAsync();

                var count = data.Count;

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
                    ex.Message
                };
            }
        }

        public async Task<object> GetHostListByProjectId(long projectId)
        {

            try
            {
                string [] hostIds = _context.ProjectBatchHost.Where(x => x.ProjectId == projectId).Distinct().Select(x => x.HostId).ToArray();
                var data = await _context.Users
                .Where(x => hostIds.Contains(x.Id) && x.UserType == "Host")              
                .Select(x => new
                {
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.PhoneNumber,
                    x.Email,
                })
                .ToListAsync();

                var count = data.Count;

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
                    ex.Message
                };
            }
        }

        public async Task<object> GetMergeableHostList(long projectId, long batchId, int size, int pageNumber)
        {

            try
            {
                string [] hostIds = _context.ProjectBatchHost.Where(x => x.ProjectId == projectId && x.BatchId == batchId).Select(x => x.HostId).ToArray();
                
                var data = await _context.Users
                .Where(x => !hostIds.Contains(x.Id) && x.UserType == "Host")              
                .Select(x => new
                {
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.PhoneNumber,
                    x.Email,
                    x.ImagePath
                })
                .OrderBy(x => x.Id)
                .Skip(pageNumber * size).Take(size)
                .ToListAsync();

                var count = data.Count;

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
                    ex.Message
                };
            }
        }

        public async Task<object> GetAlreadyMergedHostList(long projectId, long batchId, int size, int pageNumber)
        {

            try
            {
                string [] hostIds = _context.ProjectBatchHost.Where(x => x.ProjectId == projectId && x.BatchId == batchId).Select(x => x.HostId).ToArray();
                
                var data = await _context.Users
                .Where(x => hostIds.Contains(x.Id) && x.UserType == "Host")              
                .Select(x => new
                {
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.PhoneNumber,
                    x.Email,
                    x.ImagePath
                })
                .OrderBy(x => x.Id)
                .Skip(pageNumber * size).Take(size)
                .ToListAsync();

                var count = data.Count;

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
                    ex.Message
                };
            }
        }

        public async Task<object> GetParticipantList(int size, int pageNumber)
        {

            try
            {
                
                var data = await _context.Users
                .Where(x => x.UserType == "Participant")              
                .Select(x => new
                {
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.PhoneNumber,
                    x.Email,
                })
                .Skip(pageNumber * size).Take(size)
                .ToListAsync();

                var count = data.Count;

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
                    ex.Message
                };
            }
        }

        public async Task<object> GetMergeableParticipantList(long projectId, long batchId, string hostId, int size, int pageNumber)
        {

            try
            {
                long [] projectBatchHostIds = _context.ProjectBatchHost.Where(x => x.ProjectId == projectId && x.BatchId == batchId && x.HostId == hostId).Select(x => x.Id).ToArray();
                string [] participantIds = 
                _context.
                ProjectBatchHostParticipant
                .Where(x => projectBatchHostIds.Contains(x.ProjectBatchHostId))
                .Select(x => x.ParticipantId)
                .ToArray();
                
                var data = await _context.Users
                .Where(x => !participantIds.Contains(x.Id) && x.UserType == "Participant")              
                .Select(x => new
                {
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.PhoneNumber,
                    x.Email,
                    x.ImagePath
                })
                .OrderBy(x => x.Id)
                .Skip(pageNumber * size).Take(size)
                .ToListAsync();

                var count = data.Count;

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
                    ex.Message
                };
            }
        }

        public async Task<object> GetAlreadyMergedParticipantList(long projectId, long batchId, string hostId, int size, int pageNumber)
        {

            try
            {
                long [] projectBatchHostIds = _context.ProjectBatchHost.Where(x => x.ProjectId == projectId && x.BatchId == batchId && x.HostId == hostId).Select(x => x.Id).ToArray();
                string [] participantIds = 
                _context.
                ProjectBatchHostParticipant
                .Where(x => projectBatchHostIds.Contains(x.ProjectBatchHostId))
                .Select(x => x.ParticipantId)
                .ToArray();
                
                var data = await _context.Users
                .Where(x => participantIds.Contains(x.Id) && x.UserType == "Participant")              
                .Select(x => new
                {
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.PhoneNumber,
                    x.Email,
                    x.ImagePath
                })
                .OrderBy(x => x.Id)
                .Skip(pageNumber * size).Take(size)
                .ToListAsync();

                var count = data.Count;

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
                    ex.Message
                };
            }
        }

        
    }
}


