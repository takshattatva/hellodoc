using hellodoc.DAL.Models;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace hellodoc.DAL.ViewModels
{
    public class ProvidersVm
    {
        public int RegionId { get; set; }

        public int physicianId { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessage = "Message Is Required")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Message Accepts Only Alphabets")]
        public string ContactMessage { get; set; }

        public List<Region> Regions { get; set; }

        public List<Provider> Providers { get; set; }
    }

    public class Provider
    {
        public string aspId { get; set; }

        public int physicianId { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Role { get; set; }

        public string CallStatus { get; set; }

        public short Status { get; set; }

        public BitArray? Isdeleted { get; set; }

        public bool IsNotificationStopped { get; set; }
    }
}
