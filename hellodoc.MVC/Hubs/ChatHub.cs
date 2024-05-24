using hellodoc.DAL.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HellodocDbContext _context;

        public ChatHub(IHttpContextAccessor httpContextAccessor, HellodocDbContext context)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Send Message OnConnection

        public async Task SendMessage(string requestId, string receiverId, string message)
        {
            var senderConnectionId = Context.ConnectionId;
            var senderId = _context.UserConnections.Where(x => x.ConnectionId == senderConnectionId).Select(x => x.UserId).FirstOrDefault();

            // Get the connection ID of the receiver
            var receiverConnectionId = _context.UserConnections
                .Where(x => x.UserId == receiverId && x.RequestId == requestId)
                .Select(x => x.ConnectionId)
                .FirstOrDefault();

            if (receiverConnectionId != null)
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", senderId, message);
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiverNotAvailable", receiverId);
            }

            // Send the message to the sender as well
            await Clients.Client(senderConnectionId).SendAsync("ReceiveMessage", "You", message);

            // Send Notification
            var NotiConnectionid = _context.UserConnections.Where(x => x.UserId == receiverId && x.RequestId == "0").Select(x => x.ConnectionId).FirstOrDefault();

            await SendPushNotification(NotiConnectionid, message);
        }

        #endregion


        #region Send Notification

        public async Task SendPushNotification(string receiverConnectionId, string message)
        {
            string? aspId = _httpContextAccessor.HttpContext!.Session.GetString("aspNetUserId");
            //var senderName = _context.Aspnetusers.FirstOrDefault(x => x.Id == aspId).Username;
            var anu = _context.Aspnetusers.Include(x => x.Users).Include(x => x.Admins).Include(x => x.PhysicianAspnetusers).FirstOrDefault(x => x.Id == aspId);
            if (anu.Users != null) 
            {
                var senderName = anu.Users.FirstOrDefault().Firstname + " " + anu.Users.FirstOrDefault().Lastname;
                await Clients.Client(receiverConnectionId).SendAsync("ReceivePushNotification", message, senderName);
            }
            else if (anu.Admins != null)
            {
                var senderName = anu.Admins.FirstOrDefault().Firstname + " " + anu.Admins.FirstOrDefault().Lastname;
                await Clients.Client(receiverConnectionId).SendAsync("ReceivePushNotification", message, senderName);
            }
            else if (anu.PhysicianAspnetusers != null)
            {
                var senderName = anu.PhysicianAspnetusers.FirstOrDefault().Firstname + " " + anu.PhysicianAspnetusers.FirstOrDefault().Lastname;
                await Clients.Client(receiverConnectionId).SendAsync("ReceivePushNotification", message, senderName);
            }
            else
            {
                var senderName = anu.Username;
                await Clients.Client(receiverConnectionId).SendAsync("ReceivePushNotification", message, senderName);
            }
        }

        #endregion


        #region OnConnection

        public override Task OnConnectedAsync()
        {
            string? aspnetID = _httpContextAccessor.HttpContext.Session.GetString("aspNetUserId");

            string Requestid = Context.GetHttpContext().Request.Query["Reqid"];

            if (!aspnetID.IsNullOrEmpty() && !Requestid.IsNullOrEmpty())
            {
                UserConnection connectedUSerId = _context.UserConnections.Where(x => x.UserId == aspnetID && x.RequestId == Requestid).FirstOrDefault();

                UserConnection connectedUSerId2 = _context.UserConnections.Where(x => x.UserId == aspnetID && x.RequestId == "0").FirstOrDefault();

                if (connectedUSerId == null)
                {
                    UserConnection userConnection = new UserConnection();
                    userConnection.ConnectionId = Context.ConnectionId;
                    userConnection.UserId = aspnetID;
                    userConnection.RequestId=Requestid;
                    _context.UserConnections.Add(userConnection);
                    _context.SaveChanges();
                }
                else
                {
                    connectedUSerId.ConnectionId = Context.ConnectionId;

                    if (connectedUSerId2 != null)
                    {
                        connectedUSerId2.ConnectionId = Context.ConnectionId;
                    }
                    _context.SaveChanges();
                }
            }
            else
            {
                Console.WriteLine("Warning: UserId is null on connection.");
            }
            return base.OnConnectedAsync();
        }

        #endregion
    }
}