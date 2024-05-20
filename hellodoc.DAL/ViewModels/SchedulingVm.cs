using hellodoc.DAL.Models;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace hellodoc.DAL.ViewModels
{
    public class SchedulingVm
    {
        public ScheduleModel ScheduleModel { get; set; }

        public List<Region> regions { get; set; }

        public DayShiftModal DayShiftModal { get; set; }

        public MonthShiftModal MonthShiftModal { get; set; }

        public ViewShiftModal ViewShiftModal { get; set; }

        public ShiftDetailsmodal ShiftDetailsmodal { get; set; }

        public WeekShiftModal WeekShiftModal { get; set; }

        public List<ShiftReview> ShiftReview { get; set; }

        public int regionId { get; set; }

        public int callId { get; set; }

        public int physicianId { get; set; }
    }

    public class ScheduleModel
    {
        public int? Shiftid { get; set; }

        [Required(ErrorMessage = "Please Select the Physician")]
        public int Physicianid { get; set; }

        public string? PhysicianName { get; set; }

        public string? PhysicianPhoto { get; set; }

        [Required(ErrorMessage = "Please Select the region")]
        public int Regionid { get; set; }

        public string? RegionName { get; set; }

        [Required(ErrorMessage = "Please Select the Start Date")]
        public DateOnly Startdate { get; set; }

        public DateTime Shiftdate { get; set; }

        [Required(ErrorMessage = "Please Select the Start Time")]
        public TimeOnly Starttime { get; set; }

        [Required(ErrorMessage = "Please Select the End Date")]
        public TimeOnly Endtime { get; set; }

        public bool Isrepeat { get; set; }

        public string? checkWeekday { get; set; }

        public int? Repeatupto { get; set; }

        public short Status { get; set; }

        public List<ScheduleModel> DayList { get; set; }
    }

    public class DayShiftModal
    {
        public DateTime currentDate { get; set; }

        public int year { get; set; }

        public int month { get; set; }

        public int daysInMonth { get; set; }

        public DateTime firstDayOfMonth { get; set; }

        public int startDayIndex { get; set; }

        public string[] dayNames { get; set; }

        public List<ShiftDetailsmodal> shiftDetailsmodals { get; set; }

        public List<Physician> Physicians { get; set; }
    }

    public class MonthShiftModal
    {
        public DateTime currentDate { get; set; }

        public int year { get; set; }

        public int month { get; set; }

        public int daysInMonth { get; set; }

        public int daysLoop { get; set; }

        public DateTime firstDayOfMonth { get; set; }

        public int startDayIndex { get; set; }

        public string[] dayNames { get; set; }

        public List<ShiftDetailsmodal> shiftDetailsmodals { get; set; }

        public List<Physician> Physicians { get; set; }
    }

    public class ViewShiftModal
    {
        public string actionType { get; set; }

        public int requestid { get; set; }

        public int requestwisefileid { get; set; }

        public string bcolor { get; set; }

        public string btext { get; set; }

        public string patientName { get; set; }

        public string email { get; set; }

        public long? phonenumber { get; set; }

        public int physicianid { get; set; }

        public int shiftdetailsid { get; set; }

        public string datestring { get; set; }

        public DateTime columnDate { get; set; }
    }

    public class ShiftDetailsmodal
    {
        public int Shiftid { get; set; }

        public int Physicianid { get; set; }

        public string PhysicianName { get; set; }

        public DateOnly Startdate { get; set; }

        public BitArray Isrepeat { get; set; } = null!;

        public string? Weekdays { get; set; }

        public int? Repeatupto { get; set; }

        public int Shiftdetailid { get; set; }

        [Required(ErrorMessage = "Please Select the Shift Time")]
        public DateTime Shiftdate { get; set; }

        public int? Regionid { get; set; }

        public string region { get; set; }

        [Required(ErrorMessage = "Please Select the Start Time")]
        public TimeOnly Starttime { get; set; }

        [Required(ErrorMessage = "Please Select the End Time")]
        public TimeOnly Endtime { get; set; }

        public short Status { get; set; }

        public BitArray Isdeleted { get; set; }

        public string? Eventid { get; set; }

        public string? Abberaviation { get; set; }

        public string regionname { get; set; }

        public List<Region> regions { get; set; }

        public List<Physician> Physicians { get; set; }

        public List<ShiftDetailsmodal> ViewAllList { get; set; }
    }

    public class WeekShiftModal
    {
        public int startdate { get; set; }

        public int enddate { get; set; }

        public string[] dayNames { get; set; }

        public List<int> datelist { get; set; }

        public List<ShiftDetailsmodal> shiftDetailsmodals { get; set; }

        public List<Physician> Physicians { get; set; }
    }

    public class ShiftReview
    {
        public int shiftDetailId { get; set; }

        public string PhysicianName { get; set; }

        public string ShiftDate { get; set; }

        public string ShiftTime { get; set; }

        public string ShiftRegion { get; set; }
    }

    public class OnCallModal
    {
        public List<Physician> OnCall { get; set; }

        public List<Physician> OffDuty { get; set; }

        public List<Region> regions { get; set; }
    }
}