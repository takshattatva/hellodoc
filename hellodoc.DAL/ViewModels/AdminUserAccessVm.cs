using hellodoc.DAL.Models;

namespace hellodoc.DAL.ViewModels
{
    public class AdminUserAccessVm
    {
        public List<Aspnetrole> Aspnetroles { get; set; }

        public List<UserAccess> UserAccesses { get; set; }
    }

    public class UserAccess
    {
        public string AspId { get; set; }

        public int AccountTypeId { get; set; }

        public string AccountType { get; set; }

        public string AccountHolderName { get; set; }

        public string AccountPhone { get; set; }

        public short AccountStatus { get; set; }

        public int AccountRequests { get; set; }
    }
}
