using hellodoc.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace hellodoc.DAL.ViewModels
{
    public class ModalVm
    {
        public int RequestId { get; set; }

        public int StatusForName { get; set; }

        public int callId { get; set; }

        // *************** Cancel Case ***************
        public CancelCaseModal? cancelCaseModal {  get; set; }

        public List<Casetag> casetags { get; set; }


        // *************** Asign Case ***************
        public AssignCaseModal? assignCaseModal { get; set; }

        public List<Region> regions { get; set; }

        public List<Physician> physicians { get; set; }


        // *************** Block Case ***************
        public BlockCaseModal? blockCaseModal { get; set; }


        // *************** Asign Case ***************
        public TransferCaseModal? transferCaseModal { get; set; }


        // *************** Clear Case ***************
        public ClearCaseModel? clearCaseModel { get; set; }


        // *************** Send Agreement ***************
        public SendAgreementModal? sendAgreementModal { get; set; }


        // *************** Request DTY Support ***************
        [Required(ErrorMessage = "Message is Required")]
        public string? RequestSupportMessage { get; set; }
    }

    public class CancelCaseModal
    {
        public int RequestId { get; set; }

        public string Name { get; set; }

        [Required(ErrorMessage = "Cancellation note is Required")]
        public string CancellationNotes { get; set; }

        [Required(ErrorMessage = "Cancellation reason is Required")]
        public int CasetagId { get; set; }
    }

    public class AssignCaseModal
    {
        public int RequestId { get; set; }

        [Required(ErrorMessage = "Region is Required")]
        public int RegionId { get; set; }

        [Required(ErrorMessage = "Physician is Required")]
        public int PhysicianId { get; set; }

        [Required(ErrorMessage = "Assign note is Required")]
        public string AssignNotes { get; set; }
    }

    public class BlockCaseModal
    {
        public int RequestId { get; set; }

        public string Name { get; set; }

        [Required(ErrorMessage = "Reason for Block Request is Required")]
        public string BlockReason { get; set; }
    }

    public class TransferCaseModal
    {
        public int statusForName { get; set; }

        public int RequestId { get; set; }

        [Required(ErrorMessage = "Region is Required")]
        public int RegionId { get; set; }

        [Required(ErrorMessage = "Physician is Required")]
        public int PhysicianId { get; set; }

        [Required(ErrorMessage = "Physician is Required")]
        public int TransfertoPhysicianId { get; set; }
         
        [Required(ErrorMessage = "Reason to tranfer request is Required")]
        public string TransferNotes { get; set; } 
    }

    public class ClearCaseModel
    {
        public int RequestId { get; set; }
    }

    public class SendAgreementModal
    {
        public int RequestId { get; set; }

        public int RequestTypeId { get; set; }

        [Required(ErrorMessage = "Phone number is Required")]
        [RegularExpression(@"^[0-9]{7,15}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        public string Email { get; set; }

        public int? adminid { get; set; }
    }
}
