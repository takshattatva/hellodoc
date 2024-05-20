using hellodoc.DAL.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace hellodoc.DAL.ViewModels
{
    public class PatientReqData
    {
        // *********** Patient Details ************

        [Required(ErrorMessage = "Region Is Required")]
        public int? RegionId { get; set; }

        public List<Region> Regions { get; set; }

        public int Requestclientid { get; set; }

        public int Requestid { get; set; }

        [Required(ErrorMessage = "FirstName is Required")]
        [RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Invalid First Name")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "LastName is Required")]
        [RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Invalid Last Name")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string? Lastname { get; set; }

        [Required(ErrorMessage = "PhoneNumber is Required")]
        [RegularExpression(@"^[0-9]{7,15}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string? Phonenumber { get; set; }

        public string? Location { get; set; }

        [Required(ErrorMessage = "Hotelname is Required")]
        [StringLength(32, ErrorMessage = "Only 32 Characaters are Accepted")]
        public string? Hotelname { get; set; }

        public int? CaseNo { get; set; }

        public string? Notes { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Birthdate is Required")]
        public DateTime Birthdate { get; set; }

        [Required(ErrorMessage = "Streetname is Required")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string? Street { get; set; }

        [Required(ErrorMessage = "City is Required")]
        [RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Invalid City Name")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string? City { get; set; }

        [Required(ErrorMessage = "State is Required")]
        [RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Invalid State Name")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string? State { get; set; }

        [Required(ErrorMessage = "Zipcode is Required")]
        [RegularExpression(@"^\d{5,10}(?:[-\s]\d{4})?$", ErrorMessage = "Invalid Zipcode")]
        public string? Zipcode { get; set; }

        [StringLength(250, ErrorMessage = "Only 250 Characaters are Accepted")]
        public string? Filename { get; set; }

        [Required(ErrorMessage = "FirstName is Required")]
        [RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Invalid First Name")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string? OtherFirstName { get; set; }

        [Required(ErrorMessage = "LastName is Required")]
        [RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Invalid Last Name")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string? OtherLastName { get; set; }

        [Required(ErrorMessage = "PhoneNumber is Required")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string? OtherPhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? OtherEmail { get; set; }

        [Required(ErrorMessage = "RelationName is Required")]
        [RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Invalid RelationName")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string? OtherRelation { get; set; }

        public string AspNetUserId { get; set; }

        public int UserId { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{6,}$", ErrorMessage = "Minimum six characters, at least one letter, one number and one special character is mandatory")]
        [Display(Name = "PasswordHash")]
        public string PasswordHash { get; set; }

        [Required(ErrorMessage = "Confirm Password is Required")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{6,}$", ErrorMessage = "Minimum six characters, at least one letter, one number and one special character is mandatory")]
        [Display(Name = "Confirm Password")]
        [Compare("PasswordHash", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConformPassword { get; set; }

        public int Conciergeid { get; set; }

        public int ConciergeReqid { get; set; }

        public int Businessid { get; set; }

        public IFormFile? Upload { get; set; }

        public int Requesttypeid { get; set; }
    }
}





        