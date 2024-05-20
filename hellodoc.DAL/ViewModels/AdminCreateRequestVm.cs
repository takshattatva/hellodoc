using hellodoc.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace hellodoc.DAL.ViewModels
{
    public class AdminCreateRequestVm
    {
        public int StatusForName { get; set; }
        public int callId { get; set; }
        public int requestId { get; set; }

        [Required(ErrorMessage = "First Name Is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{0,15}$", ErrorMessage = "Firstname Accepts Only Alphabets max of 16 Characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name Is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{0,15}$", ErrorMessage = "Lastname Accept Only Alphabets max of 16 Characters")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Birthdate is required")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number Is Required")]
        [RegularExpression(@"^[0-9]{7,15}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Phone Number Is Required")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string? Street { get; set; }

        [Required(ErrorMessage = "City Is Required")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "City Accepts Only Text Characters")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string? City { get; set; }

        [Required(ErrorMessage = "State Is Required")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "State Accepts Only Text Characters")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string? State { get; set; }

        [Required(ErrorMessage = "Zipcode Is Required")]
        [RegularExpression(@"^\d{5,10}(?:[-\s]\d{4})?$", ErrorMessage = "Invalid Zipcode")]
        public string Zipcode { get; set; }

        [RegularExpression(@"^[0-9]{1,7}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string? Room { get; set; }

        [Required(ErrorMessage = "Admin Note Is Required")]
        public string? AdminNotes { get; set; }

        [Required(ErrorMessage = "Region Is Required")]
        public int? RegionId { get; set; }

        public List<Region> Regions { get; set; }
    }
}
