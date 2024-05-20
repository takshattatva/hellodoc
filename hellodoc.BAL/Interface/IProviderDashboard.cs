using hellodoc.DAL.Models;
using hellodoc.DAL.ViewModels;
using Microsoft.AspNetCore.Http;

namespace hellodoc.BAL.Interface
{
    public interface IProviderDashboard
    {
        #region Dashboard Request

        CountRequest GetCountRequest(int physicianId);
        List<RequestListAdminDash> getRequestData(int[] Status, string requesttypeid, int physicianId);

        #endregion


        #region Accept Case

        void SetAcceptCaseData(int requestId, int physicianId);

        #endregion


        #region Transfer Case 

        void TransferCaseData(TransferCaseModal transferCaseModal);

        #endregion


        #region Encounter Case

        void HouseCallConclude(int requestId);
        void SetEncounterCareType(EncounterVm encounterVm);
        void FinalizeEncounterCase(int requestId);

        #endregion


        #region Conclude Case

        AdminViewDocumentsVm GetViewDocumentsData(int requestid);
        List<ViewDocuments> GetViewDocumentsList(int requestid);
        void SetViewDocumentData(AdminViewDocumentsVm adminViewDocumentsVm);
        void ConfirmConcludeCare(AdminViewDocumentsVm adminViewDocumentsVm);

        #endregion


        #region Request To Admin

        void RequestForEdit(ProviderProfileVm providerProfileVm);

        #endregion


        #region Invoicing

        List<TimesheetDetail> GetTimeSheetDetails(int phyid, string dateSelected);

        List<TimesheetDetailReimbursement> GetTimeSheetDetailsReimbursements(int phyid, string dateSelected);

        List<ProviderTimesheetDetails> GetFinalizeTimeSheetDetails(int phyid, string dateSelected);

        bool PostFinalizeTimesheet(List<ProviderTimesheetDetails> providerTimesheetDetails);

        List<AddReceiptsDetails> GetAddReceiptsDetails(int[] timeSheetDetailId, string AspId);

        bool EditReceipt(string aspId, int timeSheetDetailId, string item, int amount, IFormFile file);

        bool DeleteReceipt(string aspId, int timeSheetDetailId);

        bool FinalizeTimeSheet(int timeSheetId);

        #endregion
    }
}
