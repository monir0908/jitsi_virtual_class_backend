using Commander.Common;
using Commander.Models;
using Commander.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Commander
{
    public class SignalHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private IConferenceServices _conferenceServices;
        private readonly IHubContext<SignalHub> _notificationHubContext;

        public SignalHub(ApplicationDbContext context, IHubContext<SignalHub> hubContext, IConferenceServices conferenceServices)
        {
            this._context = context;
            this._notificationHubContext = hubContext;
            this._conferenceServices = conferenceServices;
        }
        public void GetDataFromClient(string userId, string connectionId)
        {
            
            Clients.Client(connectionId).SendAsync("clientMethodName", $"Updated userid {userId}");
        }
        // public void ABCMethodCallableFromClient(string hostId, string participantId, string roomId, string connectionId)
        // {
        //     Clients.Client(connectionId).SendAsync("XYZMethodTobeListenedTo", $"Updated HOSTID {hostId}");
        // }

        public void ABCMethodCallableFromClient(string hostId, string connectionId)
        {
            Clients.Client(connectionId).SendAsync("XYZMethodTobeListenedTo", $"Updated HOSTID is :  {hostId}");
        }

        
        public override Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            Clients.Client(connectionId).SendAsync("WelcomeMethodName", connectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            Clients.All.SendAsync("BrowserRefreshedOrInternetInteruption", connectionId);

            //Console.WriteLine("NOTE:====================================================JUST DISCONNECTED SOCKETID : " + connectionId);

            VClassDetail vClassDetailObj = _context.VClassDetail.Where(x =>x.ConnectionId == connectionId ).Select(x => x).OrderByDescending(x=> x.Id).FirstOrDefault();

            

            if(vClassDetailObj == null){
                return base.OnDisconnectedAsync(exception);
            }

            // Note: if vClassObj is not null then we can find who got disconnected through its connectionid (i.e. socketId)
            else{                

                //Scenario 01: Host got disconnected
                if(vClassDetailObj.HostId !=null){                    

                    Console.WriteLine("######################################################################################");    
                    Console.WriteLine("JUST DISCONNECTED SOCKETID : " + connectionId);
                    Console.WriteLine("Host '" + vClassDetailObj.HostId + "' got disconnected");


                    //NOTE: 'LeaveTime' property is null means, an on-going meeting exits and need to update property.
                    if(vClassDetailObj.LeaveTime == null){

                        //Update 'LeaveTime' property on VClassHistory Table if 'LeaveTime' property is empty for Host
                        vClassDetailObj.LeaveTime = DateTime.UtcNow;
                        _context.SaveChanges();
                        Console.WriteLine("1. 'LeaveTime' updated for Host.");                        



                        //Update PARTICIPANT's 'LeaveTime' property on VClassHistory Table to end participant session too.
                        var participantList = 
                        _context.VClassDetail
                        .Where(c=> c.VClassId == vClassDetailObj.VClassId && c.ParticipantId !=null && c.LeaveTime ==null) //'LeaveTime == null : this is important !
                        .Select(c=> c).ToList();

                        
                        if(participantList.Count() >0){
                            Console.WriteLine("2. LeaveTime updated now for ");

                            foreach (var obj in participantList)
                            {
                                Console.WriteLine("## " + obj.ParticipantId);
                                obj.LeaveTime = DateTime.UtcNow;
                                _context.SaveChanges();                               
                            }                                                 
                            
                        }
                        else
                        {
                            Console.WriteLine("---NOTE: No participants found for 'LeaveTime' updates!");

                        } 

                        // Gathering all invitee list since class is disconnected
                        var invitees = _context.VClassInvitation.Where(c=> c.VClassId == vClassDetailObj.VClassId && c.Status == "Invited").Select(c=> new{
                            c.ParticipantId,
                            HostFullName = c.Host.FirstName + " " + c.Host.LastName,
                            c.RoomId,
                        }).ToList();

                        // Removing all invitations
                        _context.VClassInvitation
                        .RemoveRange(_context.VClassInvitation
                        .Where(x => x.VClassId == vClassDetailObj.VClassId && x.Status == "Invited"));
                        _context.SaveChanges();
                        Console.WriteLine("3. Invitation list removed.");

                        // Alerting Host that connection is lost
                        _notificationHubContext.Clients.All.SendAsync("LetHostKnowClassEnded", vClassDetailObj.HostId); //this is needed if multiple browsers opened
                        Console.WriteLine("4. Alerting Host that class has ended. It is needed in case Host has multiple tabs opened.");

                        // Alerting Participants that connection is lost
                        Console.WriteLine("5. Alerting (" + invitees.Count + ") invitee that connection has lost so that invite's browsers can be updated too.");
                        foreach (var item in invitees)
                        {           
                            
                            Console.WriteLine(item.ParticipantId);
                            _notificationHubContext.Clients.All.SendAsync("ConnectionLostFromHost", item.ParticipantId, item.RoomId, item.HostFullName);
                            
                        }

                        

                        //Update 'Status' property on VClass: Parent Table
                        VClass existingConf = _context.VClass.Where(x =>
                        x.Id == vClassDetailObj.VClassId && 
                        x.HostId == vClassDetailObj.HostId  && 
                        x.RoomId == vClassDetailObj.RoomId)
                        .Select(x => x)
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefault();


                        Helpers h = new Helpers(_context);
                        var res = h.GetActualCallDurationBetweenHostAndParticipant(existingConf.Id);
                        existingConf.Status = "Closed";

                        existingConf.HostCallDuration = res.HostCallDuration;
                        existingConf.ParticipantsCallDuration = res.ParticipantsCallDuration;
                        existingConf.EmptySlotDuration = res.EmptySlotDuration;
                        existingConf.ActualCallDuration = res.ActualCallDuration;
                        existingConf.ParticipantJoined = res.ParticipantJoined;
                        existingConf.UniqueParticipantCounts = res.UniqueParticipantCounts;
                        _context.SaveChanges();


                        Console.WriteLine("6. Finally, 'VClass Table' : Main table is updated with call-necessary details.");
                    }
                    
                    else
                    {
                        Console.WriteLine("Host 'LeaveTime' property is already set. No action taken!");
                    }
                }

                //Scenario 02: Participant got disconnected
                else if(vClassDetailObj.ParticipantId != null){                    

                    Console.WriteLine("######################################################################################");
                    Console.WriteLine("DISCONNECTED SOCKETID : " + connectionId);
                    Console.WriteLine("Participant '" + vClassDetailObj.ParticipantId + "' got disconnected");


                    
                    //Step 01: Update 'LeaveTime' property on VClassHistory Table if 'LeaveTime' property is empty
                    if(vClassDetailObj.LeaveTime == null)
                    {                
                        vClassDetailObj.LeaveTime = DateTime.UtcNow;
                        _context.SaveChanges();
                        Console.WriteLine("--- I am participant and my 'LeaveTime' is empty, so it is updated now.");
                    }
                    else
                    {                
                        Console.WriteLine("--- I am participant and my 'LeaveTime' property is already set. No action taken!");
                    }
                    
                }
            }

            return base.OnDisconnectedAsync(exception);
            




            
        }
    }
}
