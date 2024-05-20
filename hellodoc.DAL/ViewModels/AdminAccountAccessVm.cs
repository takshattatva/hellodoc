using hellodoc.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace hellodoc.DAL.ViewModels
{
    public class AdminAccountAccessVm
    {
        public List<AccountAccess> AccountAccess { get; set; }

        public AccountAccess CreateAccountAccess { get; set; }

        public List<Aspnetrole> Aspnetroles { get; set; }

        public List<Menu> Menus { get; set; }

        public List<AccountMenu> AccountMenu { get; set; }
    }

    public class AccountAccess
    {
        public int roleid { get; set; }

        public int Adminid { get; set; }

        [Required(ErrorMessage = "Role Name Is Required")]
        public string name { get; set; }

        public string accounttype { get; set; }

        [Required(ErrorMessage = "Account Type Is Required")]
        public int accounttypeid { get; set; }
    }

    public class AccountMenu
    {
        public int menuid { get; set; }

        public int roleid { get; set; }

        public string name { get; set; }

        public int accounttype { get; set; }

        public bool ExistsInTable { get; set; }
    }
}
