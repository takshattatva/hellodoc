using hellodoc.DAL.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace hellodoc.DAL.ViewModels
{
    public class PatientDashData
    {
        public List<DashboardData> dashboardData { get; set; }

        public List<DocumentData> documentData { get; set; }

        public IFormFile Upload { get; set; }

        public ProfileData? profileData { get; set; }
    }

    public class DashboardData
    {
        public int RequestId { get; set; }

        public int? PhysicianId { get; set; }

        public string? AdminAspId { get; set; }

        public string? PhyAspId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int Status { get; set; }

        public int DocumentCount { get; set; }

        public string? ConfirmationNumber { get; set; }

        public string? physicianname { get; set; }
    }

    public class DocumentData
    {
        public readonly object RequestsId;

        public DateTime CreatedDate { get; set; }

        public string DocumentName { get; set; }
    }

    public class ProfileData
    {
        [Required(ErrorMessage = "FirstName is Required")]
        [RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Invalid First Name")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "LastName is Required")]
        [RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Invalid Last Name")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string? Lastname { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        public DateTime Birthdate { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "PhoneNumber is Required")]
        [RegularExpression(@"^[0-9]{7,15}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Street Name is Required")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string? Street { get; set; }

        [RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Invalid City Name")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string? City { get; set; }

        [RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Invalid State Name")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string? State { get; set; }

        [Required(ErrorMessage = "Zipcode is mandatory")]
        [RegularExpression(@"^\d{6}(?:[-\s]\d{4})?$", ErrorMessage = "Invalid Zipcode")]
        public string? Zipcode { get; set; }

        [Required(ErrorMessage = "Region Is Required")]
        public int? RegionId { get; set; }

        public List<Region> Regions { get; set; }
    }
}