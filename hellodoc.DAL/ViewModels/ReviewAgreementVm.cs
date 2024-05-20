using System.ComponentModel.DataAnnotations;

namespace hellodoc.DAL.ViewModels
{
    public class ReviewAgreementVm
    {
        public int RequestId { get; set; } 

        public string PatientName { get; set; }

        [Required(ErrorMessage = "Cancelation Reason is Required")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Only letters, numbers, and spaces are allowed")]
        public string CancellationNotes { get; set; }
    }
}
