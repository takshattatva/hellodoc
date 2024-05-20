using hellodoc.DAL.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace hellodoc.DAL.ViewModels
{
    public class ProviderInvoicingVm
    {
        public bool? TimesheetsFinalize { get; set; }

        public List<TimesheetDetail>? Timesheetdetails { get; set; }

        public List<TimesheetDetailReimbursement>? Timesheetdetailreimbursements { get; set; }

        public List<ProviderTimesheetDetails>? ProviderTimesheetDetails { get; set; }

        public List<AddReceiptsDetails>? AddReceiptsDetails { get; set; }

        public List<PayrateByProvider>? PayrateByProvider { get; set; }

        public int? callId { get; set; }
    }

    public class ProviderTimesheetDetails
    {
        public int? TimeSheetId { get; set; }

        public int? TimeSheetDetailId { get; set; }

        public int? Hours { get; set; }

        public bool? IsWeekend { get; set; }

        public int? NoOfHouseCalls { get; set; }

        public int? NoOfConsults { get; set; }

        public DateOnly? ShiftDetailDate { get; set; }
    }

    public class AddReceiptsDetails
    {
        public int? TimeSheetDetailId { get; set; }

        public int? Amount { get; set; }

        public string? Item { get; set; }

        public string? BillValue { get; set; }

        public IFormFile? Bill { get; set; }

        public DateOnly? ShiftDetailDate { get; set; }
    }
}