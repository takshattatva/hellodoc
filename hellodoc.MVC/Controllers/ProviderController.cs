using Microsoft.AspNetCore.Mvc;
using hellodoc.DAL.Models;
using hellodoc.DAL.ViewModels;
using hellodoc.BAL.Interface;
using hellodoc.MVC.Auth;
using Rotativa.AspNetCore;

namespace hellodoc.DAL.Controllers
{
    [CustomAuthorize("Physician", "Admin")]
    public class ProviderController : Controller
    {
        private readonly HellodocDbContext _context;
        private readonly IProviderDashboard _providerDashboard;
        private readonly IAdminDashboard _adminDashboard;

        public ProviderController(HellodocDbContext context, IProviderDashboard providerDashboard, IAdminDashboard adminDashboard)
        {
            _context = context;
            _providerDashboard = providerDashboard;
            _adminDashboard = adminDashboard;
        }

        #region Provider Dashboard

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Provider Dashboard Page
        /// </summary>
        /// <returns></returns>
        [CustomAuthorize("Physician")]
        public IActionResult ProviderDashboard()
        {
            int[] arr = { 1 };

            string aspId = HttpContext.Session.GetString("aspNetUserId");
            int physicianId = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == aspId).Physicianid;

            AdminDashboardVm adminDashboardVm = new AdminDashboardVm();
            adminDashboardVm.StatusForName = 1;
            adminDashboardVm.RequestListAdminDash = _providerDashboard.getRequestData(arr, null, physicianId);
            adminDashboardVm.countRequest = _providerDashboard.GetCountRequest(physicianId);
            adminDashboardVm.sessionId = physicianId;

            return View(adminDashboardVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Provider Dashboard which contains only six status boxes
        /// </summary>
        /// <param name="physicianId"></param>
        /// <returns></returns>
        public IActionResult GetProviderDashboard(int physicianId)
        {
            byte[] byteArray = HttpContext.Session.Get("providerDashStatus");

            if (byteArray == null)
            {
                int[] arr = { 1 };

                AdminDashboardVm adminDashboardVm = new AdminDashboardVm()
                {
                    StatusForName = 1,
                    RequestListAdminDash = _providerDashboard.getRequestData(arr, null, physicianId),
                    countRequest = _providerDashboard.GetCountRequest(physicianId),
                    sessionId = physicianId,

                };
                return PartialView("Provider/Dashboard/_Provider_Dashboard", adminDashboardVm);
            }
            else
            {
                int[] statusArray = new int[byteArray.Length / sizeof(int)];
                Buffer.BlockCopy(byteArray, 0, statusArray, 0, byteArray.Length);

                AdminDashboardVm adminDashboardVm = new AdminDashboardVm();
                adminDashboardVm.StatusForName = statusArray[0];
                adminDashboardVm.RequestListAdminDash = _providerDashboard.getRequestData(statusArray, null, physicianId);
                adminDashboardVm.countRequest = _providerDashboard.GetCountRequest(physicianId);

                return PartialView("Provider/Dashboard/_Provider_Dashboard", adminDashboardVm);
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Provider Dashboard Table Records
        /// </summary>
        /// <param name="status"></param>
        /// <param name="requesttypeid"></param>
        /// <param name="physicianId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ProviderTableRecords(int[] status, string requesttypeid, int physicianId)
        {
            byte[] byteArray = new byte[status.Length * sizeof(int)];
            Buffer.BlockCopy(status, 0, byteArray, 0, byteArray.Length);

            HttpContext.Session.Set("providerDashStatus", byteArray);

            AdminDashboardVm adminDashboardVm = new AdminDashboardVm();
            adminDashboardVm.StatusForName = status[0];
            adminDashboardVm.countRequest = _providerDashboard.GetCountRequest(physicianId);
            adminDashboardVm.RequestListAdminDash = _providerDashboard.getRequestData(status, requesttypeid, physicianId);

            return PartialView("Provider/Dashboard/_ProviderTable", adminDashboardVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Filter Provider Dashboard Table using reqtypeid,region
        /// </summary>
        /// <param name="status"></param>
        /// <param name="requesttypeid"></param>
        /// <param name="physicianId"></param>
        /// <returns></returns>
        public IActionResult ProviderFilterTable(int[] status, string requesttypeid, int physicianId)
        {
            AdminDashboardVm adminDashboardVm = new AdminDashboardVm();
            adminDashboardVm.StatusForName = status[0];
            adminDashboardVm.RequestListAdminDash = _providerDashboard.getRequestData(status, requesttypeid, physicianId);
            adminDashboardVm.reqTypeId = requesttypeid;

            return PartialView("Provider/Dashboard/_ProviderRequestTable", adminDashboardVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Accept Case
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public IActionResult AcceptCaseModal(int requestid)
        {
            ModalVm modalVm = new ModalVm();
            modalVm.RequestId = requestid;

            return PartialView("Provider/Dashboard/_Provider_AcceptCase", modalVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Accept Case Records
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="physicianId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AcceptCase(int requestId, int physicianId)
        {
            _providerDashboard.SetAcceptCaseData(requestId, physicianId);
            return Ok();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Transfer Case
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="physicianId"></param>
        /// <returns></returns>
        public IActionResult Transfer(int requestid, int physicianId)
        {
            TransferCaseModal transferCaseModal = new TransferCaseModal();
            transferCaseModal.RequestId = requestid;
            transferCaseModal.PhysicianId = physicianId;

            return PartialView("Provider/Dashboard/_Provider_TransferCase", transferCaseModal);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Transfer Case
        /// </summary>
        /// <param name="transferCaseModal"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult TransferCase(TransferCaseModal transferCaseModal)
        {
            if (transferCaseModal.RequestId != 0)
            {
                _providerDashboard.TransferCaseData(transferCaseModal);
                return Ok();
            }
            return Json(new { Error = "Returned in else" });
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch House call Modal to ask for conclude the request
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public IActionResult HouseCall(int requestId)
        {
            EncounterVm encounterVm = new EncounterVm();
            encounterVm.reqid = requestId;

            return PartialView("Provider/Dashboard/_Provider_EncounterModal", encounterVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update House call Modal to conclude the request
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult HouseCallPost(int requestId)
        {
            _providerDashboard.HouseCallConclude(requestId);
            return Ok();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Encounter Modal (House call / Consultant ?)
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="physicianId"></param>
        /// <returns></returns>
        public IActionResult EncounterModal(int requestid, int physicianId)
        {
            EncounterVm encounterVm = new EncounterVm();
            encounterVm.reqid = requestid;
            encounterVm.physicianId = physicianId;

            return PartialView("Provider/Dashboard/_Provider_EncounterModal", encounterVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Encounter Modal (House call / Consultant ?)
        /// </summary>
        /// <param name="encounterVm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PostEncounterCare(EncounterVm encounterVm)
        {
            _providerDashboard.SetEncounterCareType(encounterVm);

            if (encounterVm.Option == 1)
            {
                return Ok(true);
            }
            return Ok(false);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Finalize Modal 
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public IActionResult finalizeEncounterModal(int requestId)
        {
            EncounterVm encounterVm = new EncounterVm();
            encounterVm.reqid = requestId;

            return PartialView("Provider/Dashboard/_Provider_EncounterModal", encounterVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Finalize Encounter Form
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult finalizeEncounter(int requestId)
        {
            _providerDashboard.FinalizeEncounterCase(requestId);
            return Ok();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Download Encounter form Modal
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public IActionResult DownloadEncounter(int requestId, int callId)
        {
            EncounterVm encounterVm = new EncounterVm();
            encounterVm.reqid = requestId;
            encounterVm.callId = callId;

            return PartialView("Provider/Dashboard/_Provider_EncounterModal", encounterVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Download Encounter form Modal
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        
        public IActionResult GeneratePDF([FromQuery] int requestid)
        {
            var encounterFormView = _adminDashboard.GetEncounterData(requestid, 0);
            if (encounterFormView == null)
            {
                return NotFound();
            }
            return new ViewAsPdf("FinalizeForm", encounterFormView)
            {
                FileName = "EncounterForm.pdf"
            };
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Conclude Case
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public IActionResult GetConcludeCare(int requestid)
        {
            AdminViewDocumentsVm adminViewDocumentsVm = new AdminViewDocumentsVm();
            adminViewDocumentsVm = _providerDashboard.GetViewDocumentsData(requestid);
            adminViewDocumentsVm.viewDocuments = _providerDashboard.GetViewDocumentsList(requestid);

            return PartialView("Provider/Dashboard/_Provider_ConcludeCare", adminViewDocumentsVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update (upload file) Conclude Case
        /// </summary>
        /// <param name="adminViewDocumentsVm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UploadConcludeDocument(AdminViewDocumentsVm adminViewDocumentsVm)
        {
            _providerDashboard.SetViewDocumentData(adminViewDocumentsVm);
            return Ok(adminViewDocumentsVm.requestId);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update (delete file) Conclude Case
        /// </summary>
        /// <param name="requestwisefileid"></param>
        /// <returns></returns>
        public IActionResult DeleteConcludeFile(int requestwisefileid)
        {
            _adminDashboard.DeleteFileData(requestwisefileid);
            return Ok();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update (conclude the case) Conclude Case
        /// </summary>
        /// <param name="adminViewDocumentsVm"></param>
        /// <returns></returns>
        [HttpPost]
        public int ConcludeCare(AdminViewDocumentsVm adminViewDocumentsVm)
        {
            var encounter = _context.Encounters.FirstOrDefault(x => x.RequestId == adminViewDocumentsVm.requestId);
            if (encounter == null)
            {
                return 1;
            }
            else if (encounter != null && encounter.IsFinalized.Length > 0 && !encounter.IsFinalized[0])
            {
                return 1;
            }
            else
            {
                _providerDashboard.ConfirmConcludeCare(adminViewDocumentsVm);
                return 0;
            }
        }

        #endregion


        #region Provider Profile

        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Provider Request to Admin for changes in him/her Profile
        /// </summary>
        /// <param name="providerProfileVm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SendRequestToAdmin(ProviderProfileVm providerProfileVm)
        {
            _providerDashboard.RequestForEdit(providerProfileVm);
            return Ok();
        }

        #endregion


        #region Provider Invoicing

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Provider Invoicing Page
        /// </summary>
        /// <param name="dateSelected"></param>
        /// <returns></returns>
        public IActionResult GetProviderInvoicing(string dateSelected)
        {
            string aspId = HttpContext.Session.GetString("aspNetUserId");
            int phyid = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == aspId).Physicianid;

            ProviderInvoicingVm providerInvoicingVm = new ProviderInvoicingVm();
            providerInvoicingVm.Timesheetdetails = _providerDashboard.GetTimeSheetDetails(phyid, dateSelected);
            providerInvoicingVm.Timesheetdetailreimbursements = _providerDashboard.GetTimeSheetDetailsReimbursements(phyid, dateSelected);
            providerInvoicingVm.TimesheetsFinalize = providerInvoicingVm.Timesheetdetails.Any(x => x.Timesheet.IsFinalize == true);

            return PartialView("Provider/Invoicing/_Provider_Invoicing", providerInvoicingVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Finalize Timesheet
        /// </summary>
        /// <returns></returns>
        public IActionResult OpenFinalizeTimeSheet(string dateSelected)
        {
            string aspId = HttpContext.Session.GetString("aspNetUserId");
            int phyid = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == aspId).Physicianid;

            ProviderInvoicingVm providerInvoicingVm = new ProviderInvoicingVm();
            providerInvoicingVm.ProviderTimesheetDetails = _providerDashboard.GetFinalizeTimeSheetDetails(phyid, dateSelected);
            providerInvoicingVm.callId = 3;

            return PartialView("Provider/Invoicing/_Provider_FinalizeTimeSheet", providerInvoicingVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Finalize Timesheet
        /// </summary>
        /// <param name="providerTimesheetDetails"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PostFinalizeTimesheet(List<ProviderTimesheetDetails> providerTimesheetDetails)
        {
            if (_providerDashboard.PostFinalizeTimesheet(providerTimesheetDetails))
            {
                return Ok(true);
            };
            return Ok(false);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Add Receipt
        /// </summary>
        /// <param name="timeSheetDetailId"></param>
        /// <returns></returns>
        public IActionResult GetAddReceipts(int[] timeSheetDetailId)
        {
            string AspId = HttpContext.Session.GetString("aspNetUserId");

            ProviderInvoicingVm providerInvoicingVm = new ProviderInvoicingVm();
            providerInvoicingVm.AddReceiptsDetails = _providerDashboard.GetAddReceiptsDetails(timeSheetDetailId, AspId);

            return PartialView("Provider/Invoicing/_Provider_AddReceipts", providerInvoicingVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeSheetDetailId"></param>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PostAddReceipt(int timeSheetDetailId, string item, int amount, IFormFile file)
        {
            string aspId = HttpContext.Session.GetString("aspNetUserId");

            if (_providerDashboard.EditReceipt(aspId, timeSheetDetailId, item, amount, file))
            {
                return Ok(true);
            }
            return Ok(false);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeSheetDetailId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteReceipt(int timeSheetDetailId)
        {
            string aspId = HttpContext.Session.GetString("aspNetUserId");

            if (_providerDashboard.DeleteReceipt(aspId, timeSheetDetailId))
            {
                return Ok(true);
            }
            return Ok(false);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeSheetId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ConfirmFinalizeTimeSheet(int timeSheetId)
        {
            if (_providerDashboard.FinalizeTimeSheet(timeSheetId))
            {
                return Ok(true);
            }
            return Ok(false);
        }

        #endregion

        //***************************************************************************************************************************************************
        //***************************************************************************************************************************************************
        //***************************************************************************************************************************************************
    }
}
