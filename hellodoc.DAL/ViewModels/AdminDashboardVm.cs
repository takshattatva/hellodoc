using hellodoc.DAL.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hellodoc.DAL.ViewModels
{
    public class AdminDashboardVm
    {
        public List<RequestListAdminDash> RequestListAdminDash { get; set; } // dashboard data (table)

        public int StatusForName { get; set; } // all 10 status

        public AdminViewCaseData adminViewCaseData { get; set; } // view case data

        public List<Region> regions { get; set; } // region table data


        //**********for count request out of 9 **************************
        public CountRequest countRequest { get; set; }

        public ViewNotesData viewNotesData { get; set; } // view notes data

        public SendOrderModel? sendOrderModel { get; set; } // Order data

        public List<Healthprofessionaltype>? healthProfessionalTypes { get; set; } // health professional type table data

        public List<Healthprofessional>? healthProfessionals { get; set; } // health professional table data

        public SendLink? sendLink { get; set; } // Send link

        public int sessionId { get; set; }

        public string reqTypeId { get; set; }

        public int callId { get; set; }
    }

    public class RequestListAdminDash
    {
        public int? UserId { get; set; }

        public string? PatientAspId { get; set; }

        public string? AdminAspId { get; set; }

        public string? PhyAspId { get; set; }

        public int? PhysicianId { get; set; }

        public string Name { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Requestor { get; set; }

        public string RequestDate { get; set; } //Createddate


        //********************* for count how much hr,mn,sec passed *******************
        public int? totalHours { get; set; }
        public int? totalMinutes { get; set; }
        public int? totalSeconds { get; set; }


        public string Phone { get; set; }

        public string Mobile { get; set; }

        public string? Notes { get; set; }

        public string? Address { get; set; }

        public string ChatWith { get; set; }

        public string Physician { get; set; }

        public DateTime DateOfService { get; set; }

        public string Region { get; set; }

        public int Status { get; set; }

        public int RequestTypeId { get; set; }

        public string Email { get; set; }

        public int? RequestId { get; set; }

        public int callType { get; set; }

        public bool isFinalized { get; set; }
    }

    public class AdminViewCaseData
    {
        public int? callId { get; set; }

        public int? UserId { get; set; }

        public string Symptoms { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "PhoneNumber is Required")]
        [RegularExpression(@"^[0-9]{7,15}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(30, ErrorMessage = "Only 30 Characaters are Accepted")]
        public string Email { get; set; }

        public string Region { get; set; }

        public string BusinessAddress { get; set; }

        public string Room { get; set; }

        public int RequestTypeId { get; set; }

        public string ConfirmationNumber { get; set; }

        public int RequestId { get; set; }

    }

    public class ViewNotesData
    {
        public int RequestId { get; set; }

        public string?  aspId { get; set; }

        public int callId { get; set; }

        [Required(ErrorMessage = "Admin Note is Required")]
        public string? AdminNotes { get; set; }

        public string? PhysicianNotes { get; set; }

        public List<Requeststatuslog> TransferNotes { get; set; }

        [ForeignKey("Physicianid")]
        public virtual Physician Physician { get; set; }
    }

    public class CountRequest
    {
        public int NewRequest { get; set; }

        public int PendingRequest { get; set; }

        public int ActiveRequest { get; set; }

        public int ConcludeRequest { get; set; }

        public int ToCloseRequest { get; set; }

        public int UnpaidRequest { get; set; }
    }

    public class SendOrderModel
    {
        public string? aspId { get; set; }

        public int statusForName { get; set; }

        public int callId { get; set; }

        public int RequestId { get; set; }

        [Required(ErrorMessage = "Health Profession Type is Required")]
        public int HealthPressionallId { get; set; }

        [Required(ErrorMessage = "Vendor is Required")]
        public int VendorId { get; set; }

        [Required(ErrorMessage = "Phone number is Required")]
        [RegularExpression(@"^[0-9]{7,15}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string? BusinessContact { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Faxnumber is Required")]
        [RegularExpression(@"^[0-9]{6,9}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string? FaxNum { get; set; }

        [Required(ErrorMessage = "Order details is mandatory")]
        public string? Prescription { get; set; }

        [Required(ErrorMessage = "No of Refill is mandatory")]
        public int Refil { get; set; }
    }

    public class SendLink
    {
        [Required(ErrorMessage = "First Name Is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{0,15}$", ErrorMessage = "Only alphabets max of 16 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name Is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{0,15}$", ErrorMessage = "Only alphabets max of 16 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Is Required")]
        [RegularExpression(@"^[0-9]{7,15}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string Phone { get; set; }
    }
}