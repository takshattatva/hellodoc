using System.ComponentModel.DataAnnotations;

namespace hellodoc.DAL.ViewModels
{
    public class EncounterVm
    {
        public int callId { get; set; }

        public int physicianId { get; set; }

        public int userId { get; set; }

        public int statusForName { get; set; }

        public int reqid { get; set; }

        public int callType { get; set; }

        [Required(ErrorMessage = "Please Select Any Care Type")]
        public int? Option { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Location { get; set; }

        public DateTime? BirthDate { get; set; }

        public string? Date { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Only 150 Characaters are Accepted")]
        public string? HistoryIllness { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Only 150 Characaters are Accepted")]
        public string? MedicalHistory { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Only 150 Characaters are Accepted")]
        public string? Medications { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Only 150 Characaters are Accepted")]
        public string? Allergies { get; set; }

        [Required]
        [Range(34.0, 42.0, ErrorMessage = "Temperature must be between 34°C and 42°C")]
        public decimal? Temp { get; set; }

        [Required]
        [Range(40, 200, ErrorMessage = "Heart rate must be between 40 and 200 beats per minute")]
        public decimal? Hr { get; set; }

        [Required]
        [Range(12, 20, ErrorMessage = "Respiratory rate must be between 12 and 20 breaths per minute")]
        public decimal? Rr { get; set; }

        [Required]
        [Range(90, 140, ErrorMessage = "Systolic blood pressure must be between 90 and 140 mmHg")]
        public int? BpS { get; set; }

        [Required]
        [Range(60, 90, ErrorMessage = "Diastolic blood pressure must be between 60 and 90 mmHg")]
        public int? BpD { get; set; }

        [Required]
        [Range(90, 100, ErrorMessage = "Oxygen saturation must be between 90% and 100%")]
        public decimal? O2 { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Only 150 Characaters are Accepted")]
        public string? Pain { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Only 150 Characaters are Accepted")]
        public string? Heent { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Only 150 Characaters are Accepted")]
        public string? Cv { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Only 150 Characaters are Accepted")]
        public string? Chest { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Only 150 Characaters are Accepted")]
        public string? Abd { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Only 150 Characaters are Accepted")]
        public string? Extr { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Only 150 Characaters are Accepted")]
        public string? Skin { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Only 150 Characaters are Accepted")]
        public string? Neuro { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Only 150 Characaters are Accepted")]
        public string? Other { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Only 150 Characaters are Accepted")]
        public string? Diagnosis { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Only 150 Characaters are Accepted")]
        public string? TreatmentPlan { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Only 150 Characaters are Accepted")]
        public string? MedicationDispensed { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Only 150 Characaters are Accepted")]
        public string? Procedures { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Only 150 Characaters are Accepted")]
        public string? FollowUp { get; set; }

        public string? PhyFname { get; set; }

        public string? PhyLname { get; set; }

        public string? PhyLocation { get; set; }

        public DateTime? FinalizeDate { get; set; }

        public string? PhyEmail { get; set; }

        public string? PhyPhoneNumber { get; set; }
    }
}
