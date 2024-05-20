using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using hellodoc.DAL.Models;
using System.Collections;

namespace hellodoc.DAL.ViewModels
{
    public class ProviderProfileVm
    {
        public int callId { get; set; }

        public List<Region> Regions { get; set; }

        public List<Role> Roles { get; set; }

        public List<PhysicianRegionTable> PhysicianRegionTables { get; set; }

        public string AspId { get; set; }

        public int PhysicianId { get; set; }

        [Required(ErrorMessage = "Request Message Is Required")]
        public string? RequestMessage { get; set; }

        public string? Username { get; set; }

        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Minimum eight characters and at least one letter, one number and one special character is mandatory")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Password Is Required")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Minimum eight characters and at least one letter, one number and one special character is mandatory")]
        public string? CreatePhyPassword { get; set; }

        [Required(ErrorMessage = "Role Is Required")]
        public int? RoleId { get; set; }

        public short? Status { get; set; }

        [Required(ErrorMessage = "Firstname Is Required")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "FirstName Accepts Only Text Characters")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Lastname Is Required")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "FirstName Accepts Only Text Characters")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "PhoneNumber is Required")]
        [RegularExpression(@"^[0-9]{7,15}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string Phonenumber { get; set; }

        [Required(ErrorMessage = "Medical License Number is Required")]
        [StringLength(32, ErrorMessage = "Only 32 Characaters are Accepted")]
        public string? MedicalLicense { get; set; }

        public string? NPINumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? SyncEmail { get; set; }

        [Required(ErrorMessage = "Adress1 Is Required")]
        [StringLength(32, ErrorMessage = "Only 32 Characaters are Accepted")]
        public string? Address1 { get; set; }

        [Required(ErrorMessage = "Adress2 Is Required")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string? Address2 { get; set; }

        [Required(ErrorMessage = "City Is Required")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "City Accepts Only Text Characters")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Region Is Required")]
        public int? RegionId { get; set; }

        [Required(ErrorMessage = "Zipcode is Required")]
        [RegularExpression(@"^\d{5,10}(?:[-\s]\d{4})?$", ErrorMessage = "Invalid Zipcode")]
        public string? Zipcode { get; set; }

        [Required(ErrorMessage = "Alt PhoneNumber is Required")]
        [RegularExpression(@"^[0-9]{7,15}$", ErrorMessage = "Please Enter Valid Alt Phone Number")]
        public string? AltPhone { get; set; }

        [Required(ErrorMessage = "Business name is Required")]
        public string? BusinessName { get; set; }

        [Required(ErrorMessage = "Business Website is Required")]
        [RegularExpression(@"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$", ErrorMessage = "Please enter a valid website URL.")]
        public string? BusinessWebsite { get; set; }

        [Required(ErrorMessage = "Photo is required.")]
        public IFormFile? Photo { get; set; }

        public string? PhotoValue { get; set; }

        [Required(ErrorMessage = "Sign is required.")]
        public IFormFile? Signature { get; set; }

        public string? SignatureValue { get; set; }

        [Required(ErrorMessage = "Admin Notes is Required")]
        public string? AdminNotes { get; set; }

        [Required(ErrorMessage = "ContractorAgreement is required.")]
        public IFormFile? ContractorAgreement { get; set; }

        public bool IsContractorAgreement { get; set; }

        [Required(ErrorMessage = "BackgroundCheck is required.")]
        public IFormFile? BackgroundCheck { get; set; }

        public bool IsBackgroundCheck { get; set; }

        [Required(ErrorMessage = "HIPAA is required.")]
        public IFormFile? HIPAA { get; set; }

        public bool IsHIPAA { get; set; }

        [Required(ErrorMessage = "NonDisclosure is required.")]
        public IFormFile? NonDisclosure { get; set; }

        public bool IsNonDisclosure { get; set; }

        [Required(ErrorMessage = "LicenseDocument is required.")]
        public IFormFile? LicenseDocument { get; set; }

        public bool IsLicenseDocument { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public BitArray? Isdeleted { get; set; }
    }

    public class PhysicianRegionTable
    {
        public int PhysicianId { get; set; }

        public int Regionid { get; set; }

        public string Name { get; set; }

        public bool ExistsInTable { get; set; }
    }
}
