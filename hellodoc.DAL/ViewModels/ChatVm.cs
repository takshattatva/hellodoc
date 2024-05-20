using hellodoc.DAL.Models;

namespace hellodoc.DAL.ViewModels
{
    public class ChatVm
    {
        public string? AspId { get; set; }

        public string? SenderName { get; set; }

        public string? ReceiverName { get; set; }

        public List<ChatHistory> chatHistories { get; set;}
    }
}