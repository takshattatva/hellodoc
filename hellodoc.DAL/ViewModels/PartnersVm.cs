using hellodoc.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace hellodoc.DAL.ViewModels
{
    public class PartnersVm
    {
        public List<Region> regions { get; set; }

        public List<Healthprofessionaltype> Professions { get; set; }

        public List<Partnersdata> Partnersdata { get; set; }

        [Required(ErrorMessage = "Street Is Required")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string Street { get; set; }

        [Required(ErrorMessage = "City Is Required")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        public string City { get; set; }

        [Required(ErrorMessage = "Region Is Required")]
        public int? RegionId { get; set; }

        [Required(ErrorMessage = "Please Select Profession")]
        public int? SelectedhealthprofID { get; set; }

        [Required(ErrorMessage = "Zip Is Required")]
        [RegularExpression(@"^\d{5,10}(?:[-\s]\d{4})?$", ErrorMessage = "Invalid Zipcode")]
        public string Zip { get; set; }

        [Required(ErrorMessage = "Business Name Is Required")]
        public string? BusinessName { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "PhoneNumber Is Required")]
        [RegularExpression(@"^[0-9]{7,15}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string Phonenumber { get; set; }

        [Required(ErrorMessage = "FAX NO. Is Required")]
        [RegularExpression(@"^[0-9]{7,15}$", ErrorMessage = "Please Enter Valid Fax Number")]
        public string FAXNumber { get; set; }

        [Required(ErrorMessage = "Business Contact Is Required")]
        [RegularExpression(@"^[0-9]{7,15}$", ErrorMessage = "Please Enter Valid Business Contact")]
        public string? BusinessContact { get; set; }

        public int? vendorID { get; set; }
    }

    public class Partnersdata
    {
        public string VendorName { get; set; }

        public string ProfessionName { get; set; }

        public int? VendorId { get; set; }

        public string PhoneNo { get; set; }

        public string? FaxNo { get; set; }

        public string? VendorEmail { get; set; }

        public string Businesscontact { get; set; }
    }
}
