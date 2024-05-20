using Microsoft.AspNetCore.Http;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace hellodoc.DAL.ViewModels
{
    public class AdminViewDocumentsVm
    {
        public int statusForName { get; set; }

        public int callId { get; set; }

        public int? UserId { get; set; }

        public int requestId { get; set; }

        public string patientName { get; set; }

        public string ConfirmationNumber { get; set; }

        public List<ViewDocuments> viewDocuments { get; set; }

        public IFormFile document { get; set; }

        [Required(ErrorMessage = "Provider note is Required")]
        public string ProviderNote { get; set; }

        public bool? isFinalized { get; set; }
    }

    public class ViewDocuments
    {
        public int statusForName { get; set; }

        public int callId { get; set; }

        public int requestWiseFileId { get; set; }

        public int requestId { get; set; }

        public string documentName { get; set; }

        public DateTime uploadDate { get; set; }
    }
}
