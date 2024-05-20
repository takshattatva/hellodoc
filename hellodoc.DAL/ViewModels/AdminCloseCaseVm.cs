using System.ComponentModel.DataAnnotations;

namespace hellodoc.DAL.ViewModels
{
    public class AdminCloseCaseVm
    {
        public int statusForName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Confirmationnumber { get; set; }

        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Phone Is Required")]
        [RegularExpression(@"^[0-9]{7,15}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public int RequestId { get; set; }

        public List<Documents> file { get; set; }

        public string? Address { get; set; }

        public string? Physicianname { get; set; }

        public string? Physiciancontact { get; set; }

        public string? Physicianemail { get; set; }
    }

    public class Documents
    {
        public int requestWiseFileId { get; set; }

        public int requestId { get; set; }

        public string documentName { get; set; }

        public DateTime uploadDate { get; set; }
    }
}
