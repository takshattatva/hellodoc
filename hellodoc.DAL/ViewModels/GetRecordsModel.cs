using hellodoc.DAL.Models;

namespace hellodoc.DAL.ViewModels
{
    public class GetRecordsModel
    {
        public List<User>? users { get; set; }
        public List<Request>? requestList { get; set; }
        public List<Requestclient>? requestClient { get; set; }
        public string? searchRecordOne { get; set; }
        public string? searchRecordTwo { get; set; }
        public string? searchRecordThree { get; set; }
        public string? searchRecordFour { get; set; }
    }
}
