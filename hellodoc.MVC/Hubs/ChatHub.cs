using hellodoc.DAL.Models;
using Microsoft.AspNetCore.SignalR;
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
        }

        public override Task OnConnectedAsync()
        {
            //string? aspnetID = _sessionUtils.GetUser(_httpContextAccessor.HttpContext!.Session) == null ? null : _sessionUtils.GetUser(_httpContextAccessor.HttpContext!.Session).AspNetUserId.ToString();
            string? aspnetID = _httpContextAccessor.HttpContext.Session.GetString("aspNetUserId");

            string Requestid = Context.GetHttpContext().Request.Query["Reqid"];

            if (!aspnetID.IsNullOrEmpty() && !Requestid.IsNullOrEmpty())
            {
                UserConnection connectedUSerId = _context.UserConnections.Where(x => x.UserId == aspnetID && x.RequestId==Requestid).FirstOrDefault();
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
                    _context.SaveChanges();
                }
            }
            else
            {
                Console.WriteLine("Warning: UserId is null on connection.");
            }
            return base.OnConnectedAsync();
        }
    }
}