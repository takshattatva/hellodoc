using hellodoc.DAL.Models;

namespace hellodoc.DAL.ViewModels
{
    public class AdminInvoicingVm
    {
        public List<Physician>? physiciansList {  get; set; }

        public List<Timesheet>? timesheetsList { get; set; }

        public int? PhysicianId { get; set; }

        public string? Physicianname { get; set; }
    }

    public class AdminTimeSheetList
    {
        public int? TimeSheetId { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public string? Status { get; set; }
    }
}
