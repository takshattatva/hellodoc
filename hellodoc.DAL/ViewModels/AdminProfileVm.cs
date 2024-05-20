using hellodoc.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace hellodoc.DAL.ViewModels
{
    public class AdminProfileVm
    {
        public int callId { get; set; }

        public List<Role> Roles { get; set; }

        [Required(ErrorMessage = "Regions Is Required")]
        public List<Region> Regions { get; set; }

        public List<AdminregionTable> AdminRegions { get; set; }

        public string AspId { get; set; }

        public int AdminId { get; set; }

        public string Username { get; set; }

        [Required(ErrorMessage = "Password Is Required")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{6,}$", ErrorMessage = "Minimum six characters, at least one letter, one number and one special character is mandatory")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Password Is Required")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Minimum eight characters and at least one letter, one number and one special character is mandatory")]
        public string? CreateAdminPassword { get; set; }

        public short Status { get; set; }

        [Required(ErrorMessage = "Role Is Required")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "FirstName Is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{0,15}$", ErrorMessage = "First Name Accepts Only Alphabets ( Max. 16 )")]
        public string Firstname { get; set; }

        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{0,15}$", ErrorMessage = "Last Name Accepts Only Alphabets ( Max. 16 )")]
        public string? Lastname { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Confirm Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Confirm Email")]
        [Compare("Email", ErrorMessage = "The Email and confirm Email do not match.")]
        public string? ConfirmEmail { get; set; }

        [Required(ErrorMessage = "PhoneNumber is Required")]
        [RegularExpression(@"^[0-9]{7,15}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string Phonenumber { get; set; }

        [Required(ErrorMessage = "PhoneNumber is Required")]
        [RegularExpression(@"^[0-9]{7,15}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string AltPhonenumber { get; set; }

        [Required(ErrorMessage = "Adress Is Required")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string Address1 { get; set; }

        [Required(ErrorMessage = "Adress2 Is Required")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string Address2 { get; set; }

        [Required(ErrorMessage = "City Is Required")]
        [RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Invalid City Name")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string? City { get; set; }

        [Required(ErrorMessage = "State Is Required")]
        public int RegionId { get; set; }

        [Required(ErrorMessage = "Postal Code Is Required")]
        [RegularExpression(@"^\d{5,10}(?:[-\s]\d{4})?$", ErrorMessage = "Invalid Zipcode")]
        public string? Zipcode { get; set; }
    }

    public class AdminregionTable
    {
        public int Adminid { get; set; }

        public int Regionid { get; set; }

        public string Name { get; set; }

        public bool ExistsInTable { get; set; }
    }
}
