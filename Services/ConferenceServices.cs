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

namespace Commander.Services{

    

    public class ConferenceServices : IConferenceServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<SignalHub> _notificationHubContext;

        public ConferenceServices(ApplicationDbContext context, IHubContext<SignalHub> hubContext)
        {
            this._context = context;
            this._notificationHubContext = hubContext;
        }
        

        private bool IsSameVirtualClassAleadyCreated(string roomId)
        {
            return _context.VClass.Any(x => x.RoomId == roomId);
        }
        private bool IsThereAnyOnGoingVirtualClass(string hostId)
        {
            return _context.VClass.Any(c => c.HostId == hostId && c.Status == "On-Going");
        }
        private bool IsParticipantCurrentlyInVirtualClass(string participantId)
        {
            return _context.VClassDetail.Any(c => c.ParticipantId == participantId && c.LeaveTime == null);
        }
        private bool IsVirtualClassJoinable(long vclassId){            

            return _context.VClass.Any(c => c.Id == vclassId && c.Status == "On-Going");
        }
        private bool HasHostJoinedTheVirtualClass(VClassDetail obj)
        {
            return _context.VClassDetail.Any(c => c.HostId == obj.HostId && c.VClassId == obj.VClassId && c.RoomId == obj.RoomId && c.LeaveTime == null);
        }
        private bool HasParticipantJoinedTheVirtualClass(VClassDetail obj)
        {
            return _context.VClassDetail.Any(c => c.ParticipantId == obj.ParticipantId && c.VClassId == obj.VClassId && c.RoomId == obj.RoomId && c.LeaveTime == null);
        }
        private void SendInvitationForVirtualClass(VClassDetail vClassDetail, IEnumerable<ParticipantList> participantList)
        {
            
            foreach (var item in participantList)
            {
                VClassInvitation viObj = new VClassInvitation();
                viObj.VClassId = vClassDetail.VClassId;
                viObj.RoomId = vClassDetail.RoomId ;
                viObj.BatchId = vClassDetail.BatchId ;
                viObj.HostId = vClassDetail.HostId ;
                viObj.ProjectId = vClassDetail.ProjectId ;
                viObj.InvitationDateTime = DateTime.UtcNow;
                viObj.Status = "Invited";
                viObj.ParticipantId = item.Id;

                _context.VClassInvitation.Add(viObj);
                _context.SaveChanges();
            }
            return;
            
        }
        private void RemoveInvitationList(long vClassId)
        {
            
            _context.VClassInvitation.RemoveRange(_context.VClassInvitation.Where(x => x.VClassId == vClassId && x.Status == "Invited"));
            _context.SaveChanges();   
            return;
            
        }
        


        // Host Side : VClass Related Services
        public async Task<object> GetProjectListByHostId(string hostId)
        {
            try
            {
               var query =  _context.ProjectBatchHost.Where(x => x.HostId == hostId).AsQueryable();
               var data = await query
               .Select(x => new
               {
                   Id = x.Project.Id,
                   ProjectName = x.Project.ProjectName
               }).Distinct().ToListAsync();

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
                    Message = ex.InnerException != null ? ex.InnerException.InnerException?.Message ?? ex.InnerException.Message : ex.Message
                };
            }
        }

        public async Task<object> GetBatchListByProjectIdAndHostId(long projectId, string hostId)
        {

            try
            {
                var data = await _context.ProjectBatchHost.Where(x => x.ProjectId == projectId && x.HostId == hostId)
                            
                .Select(x => new{
                    Id = x.Batch.Id,
                    BatchName = x.ProjectBatch.Batch.BatchName
                }).ToListAsync();


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
                    Message = ex.InnerException != null ? ex.InnerException.InnerException?.Message ?? ex.InnerException.Message : ex.Message
                };
            }

        }

        public async Task<object> GetParticipantListByProjectIdBatchIdAndHostId(long projectId, long batchId, string hostId)
        {

            try
            {
                long[] pbhIds = _context.ProjectBatchHost
                .Where(x => x.ProjectId == projectId && x.BatchId == batchId && x.HostId == hostId)
                .Select(x => x.Id)
                .ToArray();

                var data = 
                await _context.ProjectBatchHostParticipant
                .Where(x => pbhIds.Contains(x.ProjectBatchHostId))
                .Select(x => new
                {
                    x.ParticipantId,
                    x.Participant.FirstName,
                    x.Participant.LastName,
                }).ToListAsync();


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
                    Message = ex.InnerException != null ? ex.InnerException.InnerException?.Message ?? ex.InnerException.Message : ex.Message
                };
            }

        }

        public async Task<object> GetCurrentOnGoingVirtualClassListByHostId(string hostId){
            try
            {
                var query = 
                _context.VClass
                .Where(x => x.Status == "On-Going" && x.HostId == hostId)
                .AsQueryable();

                var data = await query.OrderByDescending(x => x.Id)
                .Select(x => new
                {
                    x.Id,
                    x.RoomId,
                    x.HostId,
                    x.BatchId,
                    x.Batch.BatchName,                    
                    x.Status,
                    HasJoinedByHost = _context.VClassDetail.Any(c => c.HostId == x.HostId && c.VClassId == x.Id && c.RoomId == x.RoomId && c.LeaveTime == null),

                }).ToListAsync();

                var count = await query.CountAsync();               


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
                    Message = ex.InnerException != null ? ex.InnerException.InnerException?.Message ?? ex.InnerException.Message : ex.Message
                };
            }
        }

        public async Task<object> CreateVirtualClass(VClass vClassObj)
        {
            Console.WriteLine(vClassObj.ProjectId);
            Console.WriteLine(vClassObj.BatchId);
            Console.WriteLine(vClassObj.HostId);
            
            try
            {
                bool isSameVirtualClassAleadyCreated = IsSameVirtualClassAleadyCreated(vClassObj.RoomId);

                if (isSameVirtualClassAleadyCreated)
                {
                    return new
                    {
                        Success = false,
                        Message = "A class with same name has been identified. Action Aborted ! Please contact system administrators."
                    };
                }

                bool isThereAnyOnGoingVirtualClass = IsThereAnyOnGoingVirtualClass(vClassObj.HostId);

                if (isThereAnyOnGoingVirtualClass)
                {
                    return new
                    {
                        Success = false,
                        Message = "An 'On-Going' detected in browser. You won't be able to start another one. Please join in previously created class."
                    };
                }


                
                Helpers h = new Common.Helpers(_context);

                string newlastRoomNumber = h.GenerateVirtualClassRoomNumber();

                vClassObj.RoomId = newlastRoomNumber;
                vClassObj.CreatedDateTime = DateTime.UtcNow;
                vClassObj.Status = "On-Going";
                _context.VClass.Add(vClassObj);
                await _context.SaveChangesAsync();

                return new
                {
                    Success = true,
                    Records = vClassObj,
                    Message = "Successfully " + Helpers.GlobalProperty +" created !"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);

                return new
                {
                    Success = false,
                    ex.Message
                };
            }
        }
        
        public async Task<object> JoinVirtualClassByHost(VClassDetail vClassDetail, IEnumerable<ParticipantList> participantList)
        {
            bool isVirtualClassJoinable = IsVirtualClassJoinable(vClassDetail.VClassId);
            if(isVirtualClassJoinable == false){
                    return new
                    {
                        Success = false,
                        Message = "It seems, the virtual class has been closed by you !"

                    };
                }



            try
            {   
                bool hasHostJoinedTheVirtualClass = HasHostJoinedTheVirtualClass(vClassDetail);
                // Console.WriteLine(hasHostJoinedTheVirtualClass);

                if(hasHostJoinedTheVirtualClass){
                    return new
                    {
                        Success = false,
                        Message = "It seems you joined the class in another browser, multiple joining NOT allowed !"

                    };
                }

                else
                {

                    vClassDetail.JoinTime = DateTime.UtcNow;
                    _context.VClassDetail.Add(vClassDetail);
                    await _context.SaveChangesAsync();

                    
                    var HostDetail = _context.VClassDetail.Where(x => x.VClassId == vClassDetail.VClassId && x.HostId == vClassDetail.HostId)
                    .Select(x => new{
                        HostFullName = x.Host.FirstName + " " + x.Host.LastName,
                        x.RoomId,
                    }).FirstOrDefault();

                    Console.WriteLine(HostDetail.HostFullName);

                    
                    foreach (var participant in participantList)
                    {
                        // Now, signalR comes into play
                        await _notificationHubContext.Clients.All.SendAsync("JoinedByHost", participant.Id, HostDetail.RoomId, HostDetail.HostFullName);
                        
                    }

                    SendInvitationForVirtualClass(vClassDetail, participantList);
                    return new
                    {
                        Success = true,
                        Records = vClassDetail,
                        Message = "You successfully joined class !"
                    };

                }   


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

        public async Task<object> EndVirtualClassByHost(VClass vClassObj)
        {

            try
            {
                VClass existingConf =
                    _context.VClass.Where(x =>
                    x.Id == vClassObj.Id && 
                    x.HostId == vClassObj.HostId  && 
                    x.RoomId == vClassObj.RoomId && 
                    x.Status == "On-Going")
                    .Select(x => x)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefault();                    

                if (existingConf != null)
                {

                    //Host LeaveTime updating
                    VClassDetail vClassDetail = _context.VClassDetail.Where(c=> c.VClassId == existingConf.Id && c.HostId == existingConf.HostId).Select(c=> c).OrderByDescending(c => c.Id).FirstOrDefault();
                    if(vClassDetail !=null){
                        vClassDetail.LeaveTime = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                    }

                    //Getting Participants and LeaveTime updating
                    var participantList = 
                    _context.VClassDetail
                    .Where(c=> c.VClassId == existingConf.Id && c.ParticipantId !=null && c.LeaveTime ==null)
                    .Select(c=> c)
                    .ToList();

                    
                    if(participantList.Count() >0){
                        foreach (var obj in participantList)
                        {
                            obj.LeaveTime = DateTime.UtcNow;
                            await _context.SaveChangesAsync();                            
                        }                       
                        
                    }

                    // Letting all invitess know if class has been ended by host
                    var invitees = 
                    _context.VClassInvitation
                    .Where(c=> c.VClassId == existingConf.Id && c.Status == "Invited")
                    .Select(c=> new{
                        c.ParticipantId,
                        HostFullName = c.Host.FirstName + " " + c.Host.LastName,
                        c.RoomId,
                    }).ToList();



                    // Removing all invitation

                    RemoveInvitationList(vClassDetail.VClassId);

                    // Alerting Host that connection is lost
                    await _notificationHubContext.Clients.All.SendAsync("LetHostKnowClassEnded", vClassObj.HostId); //this is needed if multiple browsers opened                   

                    // Alerting Participants that connection is lost
                    foreach (var item in invitees)
                    {
                        
                        await _notificationHubContext.Clients.All.SendAsync("EndedByHost", item.ParticipantId, item.RoomId, item.HostFullName);
                        
                    }

                    // Now, update 'VClass' table with call-details
                    Helpers h = new Helpers(_context);
                    var res = h.GetActualCallDurationBetweenHostAndParticipant(vClassObj.Id);
                    existingConf.Status = "Closed";

                    existingConf.HostCallDuration = res.HostCallDuration;
                    existingConf.ParticipantsCallDuration = res.ParticipantsCallDuration;
                    existingConf.EmptySlotDuration = res.EmptySlotDuration;
                    existingConf.ActualCallDuration = res.ActualCallDuration;
                    existingConf.ParticipantJoined = res.ParticipantJoined;
                    existingConf.UniqueParticipantCounts = res.UniqueParticipantCounts;
                    await _context.SaveChangesAsync();

                    return new
                    {
                        Success = true,
                        Message = "Successfully class ended !"
                    };
                }

                return new
                {
                    Success = false,
                    Message = "No on-going class found !"
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

        
        
        
        // Host Side: Miscellaneous Services
        public async Task<object> GetBatchListByProjectId(long projectId)
        {

            try
            {
                var query = _context.ProjectBatch.Where(x => x.ProjectId == projectId).AsQueryable();
                var data = await query.OrderBy(x => x.Id).Select(x => new
                {
                    Id = x.BatchId,
                    x.Batch.BatchName,
                }).ToListAsync();


                var count = await query.CountAsync();

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
                    Message = ex.InnerException != null ? ex.InnerException.InnerException?.Message ?? ex.InnerException.Message : ex.Message
                };
            }

        }
        
        public async Task<object> GetVirtualClassCallingDetailByDaterange(DateTimeParams obj)
        {
            Helpers h = new Helpers(_context);
            

            var startDate = obj.StartDate;
            var endDate = obj.EndDate.AddDays(1).AddTicks(-1);

            Console.WriteLine(startDate);
            Console.WriteLine(endDate);

            

            var data =  await _context.VClassDetail
            .Where(cs => cs.JoinTime >= startDate && cs.LeaveTime <= endDate && cs.HostId !=null)
            .Select(cs => new{
                    Id = cs.VClassId,
                    cs.RoomId,
                    cs.HostId,
                    HostFirstName = cs.Host.FirstName,
                    // cs.ParticipantId,
                    // ParticipantFirstName = cs.Participant.FirstName,
                    VirtualClassDetail = h.GetActualCallDurationBetweenHostAndParticipant(cs.VClassId),
            }).ToListAsync(); 
            
            

            return new
            {
                Success = true,
                Records = data
            };
            
        }
        
        public async Task<object> GetVirtualClassCallingDetailByHostId(string hostId)
        {
            Helpers h = new Helpers(_context);

            var data =  await _context.VClassDetail
            .Where(cs => cs.HostId == hostId)
            .Select(cs => new{
                    Id = cs.VClassId,
                    cs.RoomId,
                    cs.HostId,
                    HostFirstName = cs.Host.FirstName,
                    // cs.ParticipantId,
                    // ParticipantFirstName = cs.Participant.FirstName,
                    VirtualClassDetail = h.GetActualCallDurationBetweenHostAndParticipant(cs.VClassId),
            }).ToListAsync(); 
            
            

            return new
            {
                Success = true,
                Records = data
            };
            
        }

        public async Task<object> GetVirtualClassCallingDetail(long projectId, long batchId, string hostId, DateTime startDate, DateTime endDate)
        {

            // Console.WriteLine("startDate from front end :");
            // Console.WriteLine(startDate);

            // Console.WriteLine("endDate from front end :");
            // Console.WriteLine(endDate);


            var fromDate = startDate;
            var toDate = endDate;
            toDate = endDate.AddDays(1).AddTicks(-1);


            // Console.WriteLine("projectId : " + projectId);
            // Console.WriteLine("batchId : " + batchId);
            // Console.WriteLine("hostId : " + hostId);
            // Console.WriteLine("fromDate : " + fromDate);
            // Console.WriteLine("toDate : " + toDate);

            IQueryable<VClassDetail> query = _context.VClassDetail.Where(vcd => vcd.JoinTime >= fromDate && vcd.LeaveTime <= toDate).AsQueryable();
            if (projectId>0)
                query =  query.Where(vcd => vcd.ProjectId == projectId);
            if (batchId>0)
                query =  query.Where(vcd => vcd.BatchId == batchId);
            if (hostId != null)
                query = query.Where(vcd => vcd.HostId == hostId);
            
            // query.Where(vcd => vcd.JoinTime >= fromDate && vcd.LeaveTime <= toDate);
       


            
            Helpers h = new Helpers(_context);

            var data =  
            await query     
            .Select(cs => new{
                    Id = cs.VClassId,
                    cs.RoomId,
                    cs.HostId,
                    cs.ProjectId,
                    cs.Project.ProjectName,
                    cs.BatchId,
                    cs.Batch.BatchName,
                    HostFirstName = cs.Host.FirstName,
                    HostLastName = cs.Host.LastName,
                    cs.JoinTime,
                    cs.LeaveTime,
                    // cs.ParticipantId,
                    // ParticipantFirstName = cs.Participant.FirstName,
                    VirtualClassDetail = h.GetActualCallDurationBetweenHostAndParticipant(cs.VClassId),
            }).ToListAsync(); 
            var count = data.Count;
            
            

            return new
            {
                Success = true,
                Records = data,
                Total = count
            };
            
        }
        

        public async Task<object> GetVirtualClassDetailById(long vclassId)
        {

            Helpers h = new Helpers(_context);

            var data =  await _context.VClass
            .Where(v => v.Id == vclassId)
            .Select(v => new{
                    v.Id,
                    v.RoomId,
                    v.HostId,
                    HostFirstName = v.Host.FirstName,
                    HostLastName = v.Host.LastName,
                    HostEmail = v.Host.Email,
                    HostImagePath = v.Host.ImagePath,
                    
                    v.HostCallDuration,
                    v.ParticipantsCallDuration,
                    v.EmptySlotDuration,
                    v.ActualCallDuration,
                    v.ParticipantJoined,
                    v.UniqueParticipantCounts,

                    VirtualClassDetail = _context.VClassDetail.Where(vd => vd.VClassId == v.Id)
                    .Select(vd => new{
                        vd.Id,
                        vd.HostId,
                        HostFirstName = vd.Host.FirstName,
                        HostLastName = vd.Host.LastName,
                        ParticipantFirstName = vd.Participant.FirstName,
                        ParticipantLastName = vd.Participant.LastName,
                        vd.JoinTime,
                        vd.LeaveTime
                    }).ToList(),
            }).FirstOrDefaultAsync(); 

            return new
            {
                Success = true,
                Records = data
            };
            
        }
        



        
        // Participant Side : VClass Services     
        
        public async Task<object> GetInvitationListByParticipantId(string participantId)
        {
            try
            {
                var query = _context.VClassInvitation.Where(x => x.Status == "Invited" && x.ParticipantId == participantId).AsQueryable();
                var data = await query.OrderByDescending(x => x.Id)
                .Select(x => new
                {
                    x.Id,
                    x.VClassId,
                    x.RoomId,
                    x.ParticipantId,
                    x.ProjectId,
                    x.HostId,
                    HostFirstName= x.Host.FirstName,
                    HostLastName= x.Host.LastName,
                    x.BatchId,
                    x.Batch.BatchName,                    
                    x.Status,
                    HasJoinedByParticipant = _context.VClassDetail.Any(c => c.ParticipantId == participantId && c.VClassId == x.VClassId && c.RoomId == x.RoomId && c.LeaveTime == null),

                }).ToListAsync();

                var count = await query.CountAsync();               


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
                    Message = ex.InnerException != null ? ex.InnerException.InnerException?.Message ?? ex.InnerException.Message : ex.Message
                };
            }
        

        }
        
        public async Task<object> JoinVirtualClassByParticipant(VClassDetail vClassDetail)
        {
            bool isVirtualClassJoinable = IsVirtualClassJoinable(vClassDetail.VClassId);
            if(isVirtualClassJoinable == false){
                    return new
                    {
                        Success = false,
                        Message = "It seems, the virtual class has been closed by Teacher !"

                    };
                }

            try
            {                
                bool hasParticipantJoinedTheVirtualClass = HasParticipantJoinedTheVirtualClass(vClassDetail);
                Console.WriteLine(hasParticipantJoinedTheVirtualClass);

                if(hasParticipantJoinedTheVirtualClass){
                    return new
                    {
                        Success = false,
                        Message = "It seems you joined the class in another browser, multiple joining NOT allowed !"

                    };
                }
                else if(IsParticipantCurrentlyInVirtualClass(vClassDetail.ParticipantId))
                {
                    return new
                    {
                        Success = false,
                        Message = "It seems, you currently in a call. Therefore, you won't be able to join in another class !"

                    };
                    
                }

                else
                {

                    vClassDetail.JoinTime = DateTime.UtcNow;
                    _context.VClassDetail.Add(vClassDetail);
                    await _context.SaveChangesAsync();

                    return new
                    {
                        Success = true,
                        Records = vClassDetail,
                        Message = "Class joined successfully !"
                    };

                }   
                


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
        
        public async Task<object> EndVirtualClassByParticipant(VClassDetail vClassDetail)
        {
            try
            {


                    VClassDetail detailObj = 
                    _context.VClassDetail
                    .Where(c=> c.VClassId == vClassDetail.Id && c.ParticipantId == vClassDetail.ParticipantId)
                    .Select(c=> c)
                    .OrderByDescending(c => c.Id)
                    .FirstOrDefault();

                    

                    if(detailObj !=null && detailObj.LeaveTime == null){
                        detailObj.LeaveTime = DateTime.UtcNow;
                        await _context.SaveChangesAsync();

                    }
                    else{
                        return new
                        {
                            Success = false,
                            Message = "The virtual class has already been ended by your Teacher."
                        };
                    }                    


                    // Now, signalR comes into play
                    // This is needed if multiple browsers opened by this participant
                    await _notificationHubContext.Clients.All.SendAsync("LetParticipantKnowClassEnded", vClassDetail.ParticipantId); 


                    return new
                    {
                        Success = true,
                        Message = "Successfully class ended !"
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


        
        
        // Test Services
        
        public async Task<object> TestApi(long projectId, long batchId, string hostId, DateTime startDate, DateTime endDate)
        {


            // Console.WriteLine("startDate from front end :");
            // Console.WriteLine(startDate);

            // Console.WriteLine("endDate from front end :");
            // Console.WriteLine(endDate);


           

            var fromDate = startDate;
            var toDate = endDate;
            toDate = endDate.AddDays(1).AddTicks(-1);


            // Console.WriteLine("projectId : " + projectId);
            // Console.WriteLine("batchId : " + batchId);
            // Console.WriteLine("hostId : " + hostId);
            // Console.WriteLine("fromDate : " + fromDate);
            // Console.WriteLine("toDate : " + toDate);

            IQueryable<VClassDetail> query = _context.VClassDetail;
            if (projectId>0)
                query =  query.Where(vcd => vcd.ProjectId == projectId);
            if (batchId>0)
                query =  query.Where(vcd => vcd.BatchId == batchId);
            if (hostId != null)
                query = query.Where(vcd => vcd.HostId == hostId);
            
       


            
            Helpers h = new Helpers(_context);

            var data =  
            await query         
            .Select(cs => new{
                    Id = cs.VClassId,
                    cs.RoomId,
                    cs.HostId,
                    cs.ProjectId,
                    cs.BatchId,
                    HostFirstName = cs.Host.FirstName,
                    cs.JoinTime,
                    cs.LeaveTime,
                    // cs.ParticipantId,
                    // ParticipantFirstName = cs.Participant.FirstName,
                    VirtualClassDetail = h.GetActualCallDurationBetweenHostAndParticipant(cs.VClassId),
            }).ToListAsync(); 


            
            

            return new
            {
                Success = true,
                Records = data
            };
            
        }
        
        
    }
}


