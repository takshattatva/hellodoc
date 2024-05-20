using hellodoc.BAL.Interface;
using hellodoc.DAL.Models;
using hellodoc.DAL.ViewModels;
using hellodoc.MVC.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Rotativa.AspNetCore;

namespace hellodoc.DAL.Controllers
{
    //[CustomAuthorize("Admin", "Physician")]
    public class AdminController : Controller
    {
        private readonly HellodocDbContext _context;
        private readonly IAdminDashboard _adminDashboard;
        private readonly IProviderDashboard _providerDashboard;

        public AdminController(HellodocDbContext context, IAdminDashboard adminDashboard, IProviderDashboard providerDashboard)
        {
            _context = context;
            _adminDashboard = adminDashboard;
            _providerDashboard = providerDashboard;
        }

        #region Admin Dashboard

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Admin Dashboard Page
        /// </summary>
        /// <returns></returns>
        [CustomAuthorize("Admin")]
        public IActionResult admin_dashboard()
        {
            int[] arr = { 1 };

            AdminDashboardVm adminDashboardVm = new AdminDashboardVm();
            adminDashboardVm.StatusForName = 1;
            adminDashboardVm.RequestListAdminDash = _adminDashboard.getRequestData(arr, null, 0);
            adminDashboardVm.regions = _adminDashboard.GetRegions();
            adminDashboardVm.countRequest = _adminDashboard.GetCountRequest();

            return View(adminDashboardVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Admin Dashboard which contains only six status boxes
        /// </summary>
        /// <returns></returns>
        public IActionResult GetDashboard()
        {
            byte[] byteArray = HttpContext.Session.Get("statusArray");

            if (byteArray == null)
            {
                int[] arr = { 1 };

                AdminDashboardVm adminDashboardVm = new AdminDashboardVm();
                adminDashboardVm.StatusForName = 1;
                adminDashboardVm.RequestListAdminDash = _adminDashboard.getRequestData(arr, null, 0);
                adminDashboardVm.regions = _adminDashboard.GetRegions();
                adminDashboardVm.countRequest = _adminDashboard.GetCountRequest();

                return PartialView("Admin/Dashboard/_Admin_Dashboard", adminDashboardVm);
            }
            else
            {
                int[] statusArray = new int[byteArray.Length / sizeof(int)];
                Buffer.BlockCopy(byteArray, 0, statusArray, 0, byteArray.Length);

                AdminDashboardVm adminDashboardVm = new AdminDashboardVm();
                adminDashboardVm.StatusForName = statusArray[0];
                adminDashboardVm.RequestListAdminDash = _adminDashboard.getRequestData(statusArray, null, 0);
                adminDashboardVm.regions = _adminDashboard.GetRegions();
                adminDashboardVm.countRequest = _adminDashboard.GetCountRequest();

                return PartialView("Admin/Dashboard/_Admin_Dashboard", adminDashboardVm);
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Admin Dashboard Table Records 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="requesttypeid"></param>
        /// <param name="regionid"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult TableRecords(int[] status, string requesttypeid, int regionid)
        {
            byte[] byteArray = new byte[status.Length * sizeof(int)];
            Buffer.BlockCopy(status, 0, byteArray, 0, byteArray.Length);

            HttpContext.Session.Set("statusArray", byteArray);

            AdminDashboardVm adminDashboardVm = new AdminDashboardVm();
            adminDashboardVm.StatusForName = status[0];
            adminDashboardVm.RequestListAdminDash = _adminDashboard.getRequestData(status, requesttypeid, regionid);
            adminDashboardVm.regions = _adminDashboard.GetRegions();

            return PartialView("Admin/Dashboard/_AdminTable", adminDashboardVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Filter Admin Dashboard Table using reqtypeid,region
        /// </summary>
        /// <param name="status"></param>
        /// <param name="requesttypeid"></param>
        /// <param name="regionid"></param>
        /// <returns></returns>
        public IActionResult FilterTableRecords(int[] status, string requesttypeid, int regionid)
        {
            AdminDashboardVm adminDashboardVm = new AdminDashboardVm();
            adminDashboardVm.StatusForName = status[0];
            adminDashboardVm.RequestListAdminDash = _adminDashboard.getRequestData(status, requesttypeid, regionid);
            adminDashboardVm.regions = _adminDashboard.GetRegions();

            return PartialView("Admin/Dashboard/_AdminRequestTable", adminDashboardVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch View Case 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public IActionResult ViewCaseRecords(int status, int requestid, int callId)
        {
            if (requestid != 0)
            {
                var viewCaseData = _adminDashboard.getViewCaseData(requestid);

                AdminDashboardVm adminDashboardVm = new AdminDashboardVm();
                adminDashboardVm.StatusForName = status;
                adminDashboardVm.adminViewCaseData = viewCaseData;
                adminDashboardVm.adminViewCaseData.callId = callId;

                return PartialView("Admin/Dashboard/_Admin_ViewCase", adminDashboardVm);
            }
            return Json(new { Error = "Returned in else" }); ;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update View Case Records and fetch view case   
        /// </summary>
        /// <param name="adminDashboardVm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateViewCaseRecords(AdminDashboardVm adminDashboardVm)
        {
            _adminDashboard.setViewCaseData(adminDashboardVm.adminViewCaseData, adminDashboardVm.adminViewCaseData.RequestId);

            return Json(new { status = adminDashboardVm.StatusForName, requestid = adminDashboardVm.adminViewCaseData.RequestId });
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch View Notes 
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public IActionResult ViewNotes(int requestid, int status, int callId)
        {
            AdminDashboardVm adminDashboardVm = new AdminDashboardVm();
            adminDashboardVm.viewNotesData = _adminDashboard.GetViewNotesData(requestid);
            adminDashboardVm.StatusForName = status;
            adminDashboardVm.viewNotesData.callId = callId;

            return PartialView("Admin/Dashboard/_Admin_ViewNotes", adminDashboardVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update View Notes Records and fetch view notes
        /// </summary>
        /// <param name="adminDashboardVm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateViewNotes(AdminDashboardVm adminDashboardVm, int callId)
        {
            adminDashboardVm.viewNotesData.callId = callId;
            _adminDashboard.setViewNotesData(adminDashboardVm.viewNotesData, adminDashboardVm.viewNotesData.RequestId);
            adminDashboardVm.StatusForName = adminDashboardVm.StatusForName;

            return Json(new { requestid = adminDashboardVm.viewNotesData.RequestId, status = adminDashboardVm.StatusForName, callId = callId });
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Cancel Case Modal 
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public IActionResult CancelModal(int requestid, int status, int callId)
        {
            ModalVm modalVm = new ModalVm();
            modalVm.cancelCaseModal = _adminDashboard.getCancelCaseData(requestid);
            modalVm.casetags = _adminDashboard.GetCasetags();
            modalVm.StatusForName = status;
            modalVm.callId = callId;

            return PartialView("Admin/Dashboard/_Admin_CancelCase", modalVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Cancel the Request and GetDashboard() 
        /// </summary>
        /// <param name="modalVm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CancelCase(ModalVm modalVm)
        {
            if (modalVm.cancelCaseModal.RequestId != 0)
            {
                _adminDashboard.setCancelCaseData(modalVm.cancelCaseModal);

                return Ok();
            }
            return Json(new { Error = "Returned in else" });
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Assign Case Modal 
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public IActionResult AssignModal(int requestid, int status, int callId)
        {
            ModalVm modalVm = new ModalVm();
            modalVm.assignCaseModal = _adminDashboard.GetAssignCaseData(requestid);
            modalVm.regions = _adminDashboard.GetRegions();
            modalVm.physicians = _adminDashboard.GetPhysicians();
            modalVm.StatusForName = status;
            modalVm.callId = callId;

            return PartialView("Admin/Dashboard/_Admin_AssignCase", modalVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Filter Assign Modal by Region Selected
        /// </summary>
        /// <param name="regionid"></param>
        /// <returns></returns>
        public IActionResult FilterAssignModal(int regionid, int requestId)
        {
            string aspId = HttpContext.Session.GetString("aspNetUserId");

            ModalVm modalVm = new ModalVm();
            modalVm.physicians = _adminDashboard.GetPhysicians(regionid);
            modalVm.physicians = _adminDashboard.GetPhysiciansForAssign(regionid, requestId, aspId);
            return Json(new { success = modalVm.physicians });
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Assign Case and GetDashboard()
        /// </summary>
        /// <param name="modalVm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AssignCase(ModalVm modalVm)
        {
            if (modalVm.assignCaseModal.RequestId != 0)
            {
                _adminDashboard.SetAssignCaseData(modalVm.assignCaseModal);

                return Ok();
            }
            return Json(new { Error = "Returned in else" });
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Block Case Modal
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public IActionResult BlockModal(int requestid, int status)
        {
            ModalVm modalVm = new ModalVm();
            modalVm.blockCaseModal = _adminDashboard.GetBlockCaseModal(requestid);
            modalVm.StatusForName = status;

            return PartialView("Admin/Dashboard/_Admin_BlockCase", modalVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Block Request and GetDashboard()
        /// </summary>
        /// <param name="modalVm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult BlockCase(ModalVm modalVm)
        {
            if (modalVm.blockCaseModal.RequestId != 0)
            {
                _adminDashboard.SetBlockCaseData(modalVm.blockCaseModal);

                return Ok();
            }
            return Json(new { Error = "Returned in else" });
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch view docs records
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public IActionResult ViewDocuments(int requestid, int status, int callId)
        {
            AdminViewDocumentsVm adminViewDocumentsVm = _adminDashboard.GetViewDocumentsData(requestid);
            adminViewDocumentsVm.viewDocuments = _adminDashboard.GetViewDocumentsList(requestid);
            adminViewDocumentsVm.statusForName = status;
            adminViewDocumentsVm.callId = callId;

            return PartialView("Admin/Dashboard/_Admin_ViewDocument", adminViewDocumentsVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Upload docs and fetch view docs records
        /// </summary>
        /// <param name="adminViewDocumentsVm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UploadDocument(AdminViewDocumentsVm adminViewDocumentsVm)
        {
            _adminDashboard.SetViewDocumentData(adminViewDocumentsVm);

            return Ok(adminViewDocumentsVm.requestId);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Delete docs and fetch view docs records
        /// </summary>
        /// <param name="requestwisefileid"></param>
        /// <param name="requestid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public IActionResult DeleteFile(int requestwisefileid, int requestid, int status, int callId)
        {
            _adminDashboard.DeleteFileData(requestwisefileid);

            return Json(new { requestid = requestid, status = status, callId = callId });
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Send Mails with files and fetch view docs records
        /// </summary>
        /// <param name="requestwisefileid"></param>
        /// <param name="requestid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public IActionResult SendFile(int[] requestwisefileid, int requestid, int status, int callId)
        {
            string aspnetuserid = HttpContext.Session.GetString("aspNetUserId");
            if (callId == 1 || callId == 2)
            {
                var senderId = _context.Admins.FirstOrDefault(i => i.Aspnetuserid == aspnetuserid).Adminid;
                _adminDashboard.SendEmailWithFile(requestwisefileid, requestid, senderId);

                return Json(new { requestid = requestid, status = status, callId = callId });
            }
            else
            {
                var senderId = _context.Physicians.FirstOrDefault(i => i.Aspnetuserid == aspnetuserid).Physicianid;
                _adminDashboard.SendEmailWithFile(requestwisefileid, requestid, senderId);

                return Json(new { requestid = requestid, status = status, callId = callId });
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Send Order Records
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public IActionResult SendOrder(int requestid, int status, int callId)
        {
            AdminDashboardVm adminDashboardVm = new AdminDashboardVm();
            adminDashboardVm.sendOrderModel = _adminDashboard.getOrderData(requestid);
            adminDashboardVm.healthProfessionalTypes = _adminDashboard.getHealthProfessionalTypes();
            adminDashboardVm.healthProfessionals = _adminDashboard.getHealthProfessionals(0);
            adminDashboardVm.sendOrderModel.RequestId = requestid;
            adminDashboardVm.sendOrderModel.statusForName = status;
            adminDashboardVm.sendOrderModel.callId = callId;

            return PartialView("Admin/Dashboard/_Admin_SendOrder", adminDashboardVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Filter healthprofessionals by healthprofessionaltypes 
        /// </summary>
        /// <param name="health_professional_id"></param>
        /// <returns></returns>
        public IActionResult FilterSendOrder(int health_professional_id)
        {
            AdminDashboardVm adminDashboardVm = new AdminDashboardVm();
            adminDashboardVm.healthProfessionals = _adminDashboard.getHealthProfessionals(health_professional_id);

            return Json(new { success = adminDashboardVm.healthProfessionals });
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Get vender records as per selected healthprofessional
        /// </summary>
        /// <param name="vendorid"></param>
        /// <returns></returns>
        public IActionResult VendorData(int vendorid)
        {
            AdminDashboardVm adminDashboardVm = new AdminDashboardVm();
            adminDashboardVm.sendOrderModel = _adminDashboard.GetVendordata(vendorid);

            return Json(new { success = adminDashboardVm.sendOrderModel });
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Add Order Details 
        /// </summary>
        /// <param name="adminDashboardVm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SendOrder(AdminDashboardVm adminDashboardVm)
        {
            string aspId = HttpContext.Session.GetString("aspNetUserId");
            adminDashboardVm.sendOrderModel.aspId = aspId;

            if (adminDashboardVm.sendOrderModel.RequestId != 0)
            {
                _adminDashboard.setOrderData(adminDashboardVm.sendOrderModel);
                return Json(new { requestid = adminDashboardVm.sendOrderModel.RequestId, status = adminDashboardVm.sendOrderModel.statusForName, callId = adminDashboardVm.sendOrderModel.callId });
            }
            return Json(new { Error = "error returned in else" });
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Transfer Modal
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public IActionResult TransferModal(int requestid, int status)
        {
            ModalVm modalVm = new ModalVm();
            modalVm.transferCaseModal = _adminDashboard.GetTransferCaseData(requestid);
            modalVm.regions = _adminDashboard.GetRegions();
            modalVm.physicians = _adminDashboard.GetPhysicians();
            modalVm.StatusForName = status;

            return PartialView("Admin/Dashboard/_Admin_TransferCase", modalVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Filter Transfer Modal as per selected Region
        /// </summary>
        /// <param name="regionid"></param>
        /// <returns></returns>
        public IActionResult FilterTransferModal(int regionid, int requestId)
        {
            string aspId = HttpContext.Session.GetString("aspNetUserId");

            ModalVm modalVm = new ModalVm();
            modalVm.physicians = _adminDashboard.GetPhysiciansForAssign(regionid, requestId, aspId);

            return Json(new { success = modalVm.physicians });
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Transfer Case and GetDashboard()
        /// </summary>
        /// <param name="modalVm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult TransferCase(ModalVm modalVm)
        {
            if (modalVm.transferCaseModal.RequestId != 0)
            {
                _adminDashboard.SetTransferCaseData(modalVm.transferCaseModal);

                return Ok();
            }
            return Json(new { Error = "Returned in else" });
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Clear Cae Modal
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public IActionResult ClearModal(int requestid, int status)
        {
            ModalVm modalVm = new ModalVm();
            modalVm.clearCaseModel = _adminDashboard.getClearCaseData(requestid);
            modalVm.StatusForName = status;

            return PartialView("Admin/Dashboard/_Admin_ClearCase", modalVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Clear Request and GetDashboard()
        /// </summary>
        /// <param name="modalVm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ClearCase(ModalVm modalVm)
        {
            if (modalVm.clearCaseModel.RequestId != 0)
            {
                _adminDashboard.setClearCaseData(modalVm.clearCaseModel);

                return Ok();
            }
            return Json(new { Eroor = "error in update clearcase" });
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Send Agreement Modal
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="requesttypeid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public IActionResult SendAgreementModal(int requestid, int requesttypeid, int status, int callID)
        {
            ModalVm modalVm = new ModalVm();
            modalVm.sendAgreementModal = _adminDashboard.GetSendAgreementModal(requestid, requesttypeid);
            modalVm.StatusForName = status;
            modalVm.callId = callID;

            return PartialView("Admin/Dashboard/_Admin_SendAgreement", modalVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Send Agreement to Patient
        /// </summary>
        /// <param name="modalVm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SendAgreement(ModalVm modalVm, int callId)
        {
            string aspnetuserid = HttpContext.Session.GetString("aspNetUserId");
            if (callId == 1)
            {
                var senderId = _context.Admins.FirstOrDefault(i => i.Aspnetuserid == aspnetuserid).Adminid;
                modalVm.sendAgreementModal.adminid = senderId;
                _adminDashboard.SendAgreementEmail(modalVm.sendAgreementModal);

                return Ok();
            }
            else
            {
                var senderId = _context.Physicians.FirstOrDefault(i => i.Aspnetuserid == aspnetuserid).Physicianid;
                modalVm.sendAgreementModal.adminid = senderId;
                _adminDashboard.SendAgreementEmail(modalVm.sendAgreementModal);

                return Ok();
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Admin Encounter Form
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public IActionResult AdminEncounter(int requestid, int status, int callId)
        {
            EncounterVm encounterVm = _adminDashboard.GetEncounterData(requestid, status);
            encounterVm.callId = callId;
            encounterVm.statusForName = status;

            return PartialView("Admin/Dashboard/_Admin_Encounter", encounterVm);
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
        /// Update Admin Encounter and fetch admin encounter form
        /// </summary>
        /// <param name="encounterVm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SubmitEncounter(EncounterVm encounterVm, int callId)
        {
            _adminDashboard.SetEncounterData(encounterVm);
            encounterVm.callId = callId;

            return Json(new { reqId = encounterVm.reqid, status = encounterVm.statusForName, callId = encounterVm.callId });
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Close Case Records
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public IActionResult CloseCase(int requestid, int status)
        {
            AdminCloseCaseVm adminCloseCaseVm = _adminDashboard.GetCloseCaseData(requestid);
            adminCloseCaseVm.file = _adminDashboard.GetCloseCaseDocuments(requestid);
            adminCloseCaseVm.statusForName = status;

            return PartialView("Admin/Dashboard/_Admin_CloseCase", adminCloseCaseVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Close Case Records and fetch close case records
        /// </summary>
        /// <param name="adminCloseCaseVm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult updateCloseCase(AdminCloseCaseVm adminCloseCaseVm)
        {
            _adminDashboard.UpdateCloseCaseData(adminCloseCaseVm);

            return Json(new { reqId = adminCloseCaseVm.RequestId, status = adminCloseCaseVm.statusForName });
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Close the Request and Request goes to unpaid status and GetDashboard()
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PostCloseCase(int requestId)
        {
            _adminDashboard.SetClosedCase(requestId);

            return Ok();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Send Link Modal
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public IActionResult sendLinkModal(int status, int callId)
        {
            AdminDashboardVm adminDashboardVm = new AdminDashboardVm();
            adminDashboardVm.StatusForName = status;
            adminDashboardVm.callId = callId;

            return PartialView("Admin/Dashboard/_Admin_SendLink", adminDashboardVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Send Link to Given Email
        /// </summary>
        /// <param name="adminDashboardVm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SendLink(AdminDashboardVm adminDashboardVm, int callId)
        {
            string aspnetuserid = HttpContext.Session.GetString("aspNetUserId");
            if (callId == 1)
            {
                var adminId = _context.Admins.FirstOrDefault(i => i.Aspnetuserid == aspnetuserid).Adminid;
                _adminDashboard.SendSubmitRequestLink(adminDashboardVm.sendLink, adminId);
            }
            if (callId == 2)
            {
                var physicianId = _context.Physicians.FirstOrDefault(i => i.Aspnetuserid == aspnetuserid).Physicianid;
                _adminDashboard.SendSubmitRequestLink(adminDashboardVm.sendLink, physicianId);
            }
            return Ok();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Create Request By Admin
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public IActionResult adminCreateRequest(int status, int callId)
        {
            AdminCreateRequestVm adminCreateRequestVm = new AdminCreateRequestVm();
            adminCreateRequestVm.Regions = _adminDashboard.GetRegions();
            adminCreateRequestVm.StatusForName = status;
            adminCreateRequestVm.callId = callId;

            return PartialView("Admin/Dashboard/_Admin_CreateRequest", adminCreateRequestVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Records of Request Created by admin/provider
        /// </summary>
        /// <param name="adminCreateRequestVm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult sendAdminCreateRequest(AdminCreateRequestVm adminCreateRequestVm, int callId)
        {
            User? user = _context.Users.FirstOrDefault(x => x.Email == adminCreateRequestVm.Email);
            if (user == null)
            {
                string aspnetuserid = HttpContext.Session.GetString("aspNetUserId");
                if (callId == 1)
                {
                    var adminId = _context.Admins.FirstOrDefault(i => i.Aspnetuserid == aspnetuserid).Adminid;
                    adminCreateRequestVm.callId = callId;
                    _adminDashboard.SendCreateRequestData(adminCreateRequestVm, adminId);
                }
                if (callId == 2)
                {
                    var physicianId = _context.Physicians.FirstOrDefault(i => i.Aspnetuserid == aspnetuserid).Physicianid;
                    adminCreateRequestVm.callId = callId;
                    _adminDashboard.SendCreateRequestData(adminCreateRequestVm, physicianId);
                }
                return Ok();
            }
            else
            {
                TempData["error"] = "Email Already Existed! Patient have to just add request by login";
                return Ok();
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Export and Exports All Request Records
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="requesttypeid"></param>
        /// <param name="regionid"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Export(int[] arr, string requesttypeid, int regionid)
        {
            var requestData = _adminDashboard.getRequestData(arr, requesttypeid, regionid);

            // Set LicenseContext to suppress the LicenseException
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Create a new Excel package
            using (ExcelPackage package = new ExcelPackage())
            {
                // Add a new worksheet to the Excel package
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("RequestData");

                // Add headers to the worksheet
                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "Requestor";
                worksheet.Cells[1, 3].Value = "Requested Date";
                worksheet.Cells[1, 4].Value = "Phone";
                worksheet.Cells[1, 5].Value = "Address";
                worksheet.Cells[1, 6].Value = "Notes";
                worksheet.Cells[1, 7].Value = "Status";
                worksheet.Cells[1, 8].Value = "Physician";
                worksheet.Cells[1, 9].Value = "Birth Date";
                worksheet.Cells[1, 10].Value = "RequestTypeId";
                worksheet.Cells[1, 11].Value = "Email";
                worksheet.Cells[1, 12].Value = "RequestId";

                // Populate the worksheet with table data
                for (int i = 0; i < requestData.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = requestData[i].Name;
                    worksheet.Cells[i + 2, 2].Value = requestData[i].Requestor;
                    worksheet.Cells[i + 2, 3].Value = requestData[i].RequestDate;
                    worksheet.Cells[i + 2, 4].Value = requestData[i].Phone;
                    worksheet.Cells[i + 2, 5].Value = requestData[i].Address;
                    worksheet.Cells[i + 2, 6].Value = requestData[i].Notes;
                    worksheet.Cells[i + 2, 7].Value = requestData[i].Status;
                    worksheet.Cells[i + 2, 8].Value = requestData[i].Physician;
                    worksheet.Cells[i + 2, 9].Value = requestData[i].DateOfBirth;
                    worksheet.Cells[i + 2, 10].Value = requestData[i].RequestTypeId;
                    worksheet.Cells[i + 2, 11].Value = requestData[i].Email;
                    worksheet.Cells[i + 2, 12].Value = requestData[i].RequestId;
                }
                // Convert the Excel package to a byte array
                byte[] excelBytes = package.GetAsByteArray();

                // Return the Excel file as a download
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Request DTY Support
        /// </summary>
        /// <returns></returns>
        public IActionResult RequestSupport()
        {
            return PartialView("Admin/Dashboard/_Admin_RequestSupport");
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Request DTY Support
        /// </summary>
        /// <param name="modalVm"></param>
        /// <returns></returns>
        public IActionResult RequestDTYPost(ModalVm modalVm)
        {
            string adminAspId = HttpContext.Session.GetString("aspNetUserId");
            int adminId = _context.Admins.FirstOrDefault(x => x.Aspnetuserid == adminAspId).Adminid;

            if (_adminDashboard.RequestSupportViaEmail(modalVm.RequestSupportMessage, adminId))
            {
                return Ok(true);
            }
            return Ok(false);
        }

        #endregion


        #region Admin Profie

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Admin Profile Tab
        /// </summary>
        /// <param name="aspnetId"></param>
        /// <param name="callId"></param>
        /// <returns></returns>
        public IActionResult GetAdminProfile(string aspnetId, int callId)
        {
            //string aspId = HttpContext.Session.GetString("aspNetUserId");
            string aspId = aspnetId == 0.ToString() ? HttpContext.Session.GetString("aspNetUserId") : aspnetId;

            AdminProfileVm adminProfileVm = _adminDashboard.GetProfileData(aspId);
            adminProfileVm.Regions = _adminDashboard.GetRegions();
            adminProfileVm.Roles = _adminDashboard.GetRolesForAdmin(aspId);
            adminProfileVm.AdminRegions = _adminDashboard.GetAdminregions(aspId);
            adminProfileVm.callId = callId;

            return PartialView("Admin/Profile/_Admin_Profile", adminProfileVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Admin Account Password and fetch admin profile 
        /// </summary>
        /// <param name="adminProfileVm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AdminResetPassword(AdminProfileVm adminProfileVm)
        {
            _adminDashboard.AdminResetPassword(adminProfileVm.Password, adminProfileVm.AspId);
            return Ok();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Admin Account Records and fetch admin profile
        /// </summary>
        /// <param name="adminProfileVm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AdminAccountEdit(AdminProfileVm adminProfileVm)
        {
            _adminDashboard.AdminAccountUpdate((short)adminProfileVm.Status, (int)adminProfileVm.RoleId, adminProfileVm.AspId);
            return Ok();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Admin Administrator Records and fetch admin profile
        /// </summary>
        /// <param name="adminProfileVm"></param>
        /// <param name="regions"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AdministratorDetail(AdminProfileVm adminProfileVm, List<int> regions)
        {
            if (adminProfileVm != null)
            {
                if (adminProfileVm.Email == adminProfileVm.ConfirmEmail)
                {
                    _adminDashboard.AdministratorDetail(adminProfileVm, regions);
                    return Json(new { success = true });
                }
                return Json(new { success = false });
            }
            return Json(new { Error = "Returned in else" });
        }

        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Ademin Mailing Records and fetch admin profile 
        /// </summary>
        /// <param name="adminProfileVm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult MailingDetail(AdminProfileVm adminProfileVm)
        {
            if (adminProfileVm != null)
            {
                _adminDashboard.MailingDetail(adminProfileVm);
                return Ok();
            }
            return Json(new { Error = "Returned in else" });
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Delete Admin Account and fetch User Access
        /// </summary>
        /// <param name="adminId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteAdminAccount(int adminId)
        {
            _adminDashboard.RemoveAdmin(adminId);
            return Ok();
        }

        #endregion


        #region Provider's Tab

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Providers provider tab
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public IActionResult GetProvider(int regionId)
        {
            ProvidersVm providersVm = new ProvidersVm();
            providersVm.Regions = _adminDashboard.GetRegions();
            providersVm.Providers = _adminDashboard.GetProviders(regionId);

            return PartialView("Admin/Providers/_Providers", providersVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Stop/Unstop notification and fetch providers provider tab
        /// </summary>
        /// <param name="physicianId"></param>
        /// <returns></returns>
        public IActionResult stopNotification(int physicianId)
        {
            if (_adminDashboard.StopNotificationPhysician(physicianId) == 1)
            {
                return Json(new { Success = true });
            }
            else
            {
                return Json(new { Success = true });
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Contact Provider Modal
        /// </summary>
        /// <returns></returns>
        public IActionResult ContactProvider()
        {
            return Ok();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Send email to provider with message
        /// </summary>
        /// <param name="providersVm"></param>
        /// <returns></returns>
        public IActionResult SendEmailToProvider(ProvidersVm providersVm)
        {
            var user = HttpContext.Session.GetString("aspNetUserId");
            string aspId = user;

            _adminDashboard.ContactProviderViaEmail(providersVm, aspId);

            return Ok();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Send sms to provider with message
        /// </summary>
        /// <param name="providersVm"></param>
        /// <returns></returns>
        public IActionResult SendContactToProvider(ProvidersVm providersVm)
        {
            var user = HttpContext.Session.GetString("aspNetUserId");
            string aspId = user;

            _adminDashboard.ContactProviderViaContact(providersVm, aspId);

            return Ok();
        }

        #endregion


        #region Provider Profile

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Edit Provider
        /// </summary>
        /// <param name="aspId"></param>
        /// <param name="callId"></param>
        /// <returns></returns>
        public IActionResult GetEditProvider(string aspId, int callId)
        {
            ProviderProfileVm providerProfileVm = _adminDashboard.GetProviderProfileData(aspId);
            providerProfileVm.Regions = _adminDashboard.GetRegions();
            providerProfileVm.Roles = _adminDashboard.GetRolesForPhysicians();
            providerProfileVm.PhysicianRegionTables = _adminDashboard.GetPhysicianRegionTables(aspId);
            providerProfileVm.callId = callId;

            return PartialView("Admin/Providers/_Provider_ProfileEdit", providerProfileVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Physician Account Password
        /// </summary>
        /// <param name="providerProfileVm"></param>
        /// <returns></returns>
        public IActionResult PhysicianProfileResetPassword(ProviderProfileVm providerProfileVm, int callId)
        {
            if (providerProfileVm.Password != null)
            {
                _adminDashboard.PhysicianResetPassword(providerProfileVm.Password, providerProfileVm.AspId);

                return Json(new { Success = true, aspId = providerProfileVm.AspId });
            }
            return Json(new { Success = false });
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Physician Account Records and fetch Edit Provider
        /// </summary>
        /// <param name="providerProfileVm"></param>
        /// <returns></returns>
        public IActionResult PhysicianAccountEdit(ProviderProfileVm providerProfileVm)
        {
            _adminDashboard.PhysicianAccountUpdate((short)providerProfileVm.Status, (int)providerProfileVm.RoleId, providerProfileVm.AspId);

            return Ok(providerProfileVm.AspId);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Physician Administrator Records and fetch Edit Provider
        /// </summary>
        /// <param name="providerProfileVm"></param>
        /// <param name="physicianRegions"></param>
        /// <returns></returns>
        public IActionResult PhysicianAdministratorEdit(ProviderProfileVm providerProfileVm, List<int> physicianRegions)
        {
            _adminDashboard.PhysicianAdministratorDataUpdate(providerProfileVm, physicianRegions);

            return Ok(providerProfileVm.AspId);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Physician Mailing Records and fetch Edit Provider
        /// </summary>
        /// <param name="providerProfileVm"></param>
        /// <returns></returns>
        public IActionResult PhysicianMailingEdit(ProviderProfileVm providerProfileVm)
        {
            _adminDashboard.PhysicianMailingDataUpdate(providerProfileVm);

            return Ok(providerProfileVm.AspId);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Physician Business Records and fetch Edit Provider
        /// </summary>
        /// <param name="providerProfileVm"></param>
        /// <returns></returns>
        public IActionResult PhysicianBusinessInfoEdit(ProviderProfileVm providerProfileVm)
        {
            _adminDashboard.PhysicianBusinessInfoUpdate(providerProfileVm);

            return Ok(providerProfileVm.AspId);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Physician Onboarding Records and fetch Edit Provider
        /// </summary>
        /// <param name="providerProfileVm"></param>
        /// <returns></returns>
        public IActionResult UpdateOnBoarding(ProviderProfileVm providerProfileVm)
        {
            _adminDashboard.EditOnBoardingData(providerProfileVm);

            return Ok(providerProfileVm.AspId);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Delete Physician Account and fetch Providers provider tab
        /// </summary>
        /// <param name="physicianId"></param>
        /// <returns></returns>
        public IActionResult DeletePhysicianAccount(int physicianId)
        {
            if (_adminDashboard.RemovePhysician(physicianId))
            {
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }

        #endregion


        #region Create Provider Acc

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Create Physician Account 
        /// </summary>
        /// <param name="callId"></param>
        /// <returns></returns>
        public IActionResult CreateProviderAccount(int callId)
        {
            var user = HttpContext.Session.GetString("aspNetUserId");
            string aspId = user;

            ProviderProfileVm providerProfileVm = new ProviderProfileVm();
            providerProfileVm.Roles = _adminDashboard.GetRolesForPhysicians();
            providerProfileVm.Regions = _adminDashboard.GetRegions();
            providerProfileVm.AspId = aspId;
            providerProfileVm.callId = callId;

            return PartialView("Admin/Providers/_Provider_CreateProviderAccount", providerProfileVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Check Entered Email at Physician Cretae Account 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> EmailCheckForPhysician(string email)
        {
            var physician = await _context.Physicians.FirstOrDefaultAsync(x => x.Email == email);
            return physician != null;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Create Provider Account and Save the records and fetch Providers Provider Tab or User Access Tab
        /// </summary>
        /// <param name="providerProfileVm"></param>
        /// <param name="physicianRegions"></param>
        /// <returns></returns>
        public IActionResult CreateProviderAccountPost(ProviderProfileVm providerProfileVm, List<int> physicianRegions)
        {
            if (_adminDashboard.CreatePhysicianAccount(providerProfileVm, physicianRegions))
            {
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }

        #endregion


        #region Account Access

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Access Tab's Account Access
        /// </summary>
        /// <returns></returns>
        public IActionResult GetAccountAccess()
        {
            AdminAccountAccessVm adminAccessVm = new AdminAccountAccessVm();
            adminAccessVm.AccountAccess = _adminDashboard.GetAccountAccessData();

            return PartialView("Admin/Access/_Admin_AccountAccess", adminAccessVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Create Account Access
        /// </summary>
        /// <returns></returns>
        public IActionResult GetCreateAccess()
        {
            AdminAccountAccessVm adminAccessVm = new AdminAccountAccessVm();
            adminAccessVm.Aspnetroles = _adminDashboard.GetAccountType();
            adminAccessVm.Menus = _adminDashboard.GetMenu(0);

            return PartialView("Admin/Access/_Admin_AccountAccessCreate", adminAccessVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Filter Roles as per selected role at Create Account Access
        /// </summary>
        /// <param name="accounttype"></param>
        /// <returns></returns>
        public IActionResult FilterRolesMenu(int accounttype)
        {
            var menu = _adminDashboard.GetMenu(accounttype);
            return Json(menu);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Check Entered Role Name isExisted or not at Create Account Access
        /// </summary>
        /// <param name="RoleName"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> RoleNamecheck(string RoleName)
        {
            var Rolename = await _context.Roles.FirstOrDefaultAsync(x => x.Name == RoleName);
            return Rolename != null;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Create Account Access and Save Records and fetch Account Access
        /// </summary>
        /// <param name="adminAccessVm"></param>
        /// <param name="AccountMenu"></param>
        /// <returns></returns>
        [HttpPost]
        public bool SetCreateAccessAccount(AdminAccountAccessVm adminAccessVm, List<int> AccountMenu)
        {
            Role? rolename = _context.Roles.FirstOrDefault(x => x.Name == adminAccessVm.CreateAccountAccess.name);
            if (rolename == null)
            {
                _adminDashboard.SetCreateAccessAccount(adminAccessVm.CreateAccountAccess, AccountMenu);
                return true;
            }
            else
            {
                return false;
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Edit Account Access
        /// </summary>
        /// <param name="accounttypeid"></param>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public IActionResult GetEditAccess(int accounttypeid, int roleid)
        {
            var roledata = _adminDashboard.GetEditAccessData(roleid);
            var Accounttype = _adminDashboard.GetAccountType();
            var menu = _adminDashboard.GetAccountMenu(accounttypeid, roleid);

            AdminAccountAccessVm adminAccessVm = new AdminAccountAccessVm();
            adminAccessVm.Aspnetroles = Accounttype;
            adminAccessVm.AccountMenu = menu;
            adminAccessVm.CreateAccountAccess = roledata;

            return PartialView("Admin/Access/_Admin_AccountAccessEdit", adminAccessVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Filter Roles as per selected role at Edit Account Access
        /// </summary>
        /// <param name="accounttypeid"></param>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public IActionResult FilterEditRolesMenu(int accounttypeid, int roleid)
        {
            var menu = _adminDashboard.GetAccountMenu(accounttypeid, roleid);
            var htmlcontent = "";
            foreach (var obj in menu)
            {
                htmlcontent += $"<div class='form-check form-check-inline px-2 mx-3'><input class='form-check-input d2class' {(obj.ExistsInTable ? "checked" : "")} name='AccountMenu' type='checkbox' id='{obj.menuid}' value='{obj.menuid}'/><label class='form-check-label' for='{obj.menuid}'>{obj.name}</label></div>";
            }
            return Content(htmlcontent);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Edit Account Access
        /// </summary>
        /// <param name="adminAccessVm"></param>
        /// <param name="AccountMenu"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SetEditAccessAccount(AdminAccountAccessVm adminAccessVm, List<int> AccountMenu)
        {
            _adminDashboard.SetEditAccessAccount(adminAccessVm.CreateAccountAccess, AccountMenu);
            return Ok();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Delete Account Access 
        /// </summary>
        /// <param name="roleid"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteAccountAccess(int roleid)
        {
            _adminDashboard.DeleteAccountAccess(roleid);
            return Ok();
        }

        #endregion


        #region User Access

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Access Tab's User Access
        /// </summary>
        /// <param name="accountTypeId"></param>
        /// <returns></returns>
        public IActionResult GetUserAccess(int accountTypeId)
        {
            AdminUserAccessVm adminUserAccessVm = new AdminUserAccessVm();
            adminUserAccessVm.Aspnetroles = _adminDashboard.GetAccountType();
            adminUserAccessVm.UserAccesses = _adminDashboard.GetUserAccessData(accountTypeId);

            return PartialView("Admin/Access/_Admin_UserAccess", adminUserAccessVm);
        }

        #endregion


        #region Create Admin Acc

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Admin Create Account
        /// </summary>
        /// <param name="callId"></param>
        /// <returns></returns>
        public IActionResult GetCreateAdminAccount(int callId)
        {
            string aspId = HttpContext.Session.GetString("aspNetUserId");

            AdminProfileVm adminProfileVm = new AdminProfileVm();
            adminProfileVm.Roles = _adminDashboard.GetRolesForAdmin(aspId);
            adminProfileVm.Regions = _adminDashboard.GetRegions();
            adminProfileVm.AspId = aspId;
            adminProfileVm.callId = callId;

            return PartialView("Admin/Access/_Admin_CreateAccount", adminProfileVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Create Admin Account and Update Reecords and fetch User Access
        /// </summary>
        /// <param name="adminProfileVm"></param>
        /// <param name="adminRegions"></param>
        /// <returns></returns>
        public IActionResult CreateAdminAccountPost(AdminProfileVm adminProfileVm, List<int> adminRegions)
        {
            if (_adminDashboard.CreateAdminAccount(adminProfileVm, adminRegions))
            {
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }

        #endregion


        #region Schedulling

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Provider Tab's Schedulling Upper Part
        /// </summary>
        /// <returns></returns>
        public IActionResult GetScheduling(int callId)
        {
            SchedulingVm schedulingVm = new SchedulingVm();
            schedulingVm.regions = _adminDashboard.GetRegions();
            schedulingVm.callId = callId;

            return PartialView("Admin/Scheduling/_Scheduling", schedulingVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch schedulling table records as per day,week,month
        /// </summary>
        /// <param name="datestring"></param>
        /// <param name="sundaystring"></param>
        /// <param name="saturdaystring"></param>
        /// <param name="shifttype"></param>
        /// <param name="regionid"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult loadshift(string datestring, string sundaystring, string saturdaystring, string shifttype, int regionid)
        {
            DateTime date = DateTime.Parse(datestring);
            DateTime sunday = DateTime.Parse(sundaystring);
            DateTime saturday = DateTime.Parse(saturdaystring);

            string aspId = HttpContext.Session.GetString("aspNetUserId");

            switch (shifttype)
            {
                case "month":
                    MonthShiftModal monthShift = new MonthShiftModal();

                    var totalDays = DateTime.DaysInMonth(date.Year, date.Month);
                    var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                    var startDayIndex = (int)firstDayOfMonth.DayOfWeek;

                    var dayceiling = (int)Math.Ceiling((float)(totalDays + startDayIndex) / 7);

                    monthShift.daysLoop = (int)dayceiling * 7;
                    monthShift.daysInMonth = totalDays;
                    monthShift.firstDayOfMonth = firstDayOfMonth;
                    monthShift.startDayIndex = startDayIndex;
                    monthShift.Physicians = _adminDashboard.GetPhysicians(regionid);

                    if (regionid == 0)
                    {
                        monthShift.shiftDetailsmodals = _adminDashboard.ShiftDetailsmodal(date, sunday, saturday, "month", aspId);
                    }
                    else
                    {
                        monthShift.shiftDetailsmodals = _adminDashboard.ShiftDetailsmodal(date, sunday, saturday, "month", aspId).Where(i => i.Regionid == regionid).ToList();
                    }
                    return PartialView("Admin/Scheduling/_MonthWiseShift", monthShift);

                case "week":

                    WeekShiftModal weekShift = new WeekShiftModal();

                    weekShift.Physicians = regionid == 0 ? _adminDashboard.GetPhysicians() : _adminDashboard.GetPhysicians(regionid);
                    weekShift.shiftDetailsmodals = _adminDashboard.ShiftDetailsmodal(date, sunday, saturday, "week", aspId);

                    List<int> dlist = new List<int>();

                    for (var i = 0; i < 7; i++)
                    {
                        var date12 = sunday.AddDays(i);
                        dlist.Add(date12.Day);
                    }

                    weekShift.datelist = dlist.ToList();
                    weekShift.dayNames = new string[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

                    return PartialView("Admin/Scheduling/_WeekWiseShift", weekShift);

                case "day":

                    DayShiftModal dayShift = new DayShiftModal();

                    dayShift.Physicians = regionid == 0 ? _adminDashboard.GetPhysicians() : _adminDashboard.GetPhysicians(regionid);
                    dayShift.shiftDetailsmodals = _adminDashboard.ShiftDetailsmodal(date, sunday, saturday, "day", aspId);

                    return PartialView("Admin/Scheduling/_DayWiseShift", dayShift);

                default:
                    return PartialView();
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Create Shift Modal
        /// </summary>
        /// <returns></returns>
        public IActionResult CreateNewShift(int callId)
        {
            SchedulingVm schedulingVm = new SchedulingVm();
            if (callId == 1)
            {
                schedulingVm.regions = _adminDashboard.GetRegions();
                schedulingVm.callId = 1;
            }
            else if (callId == 3)
            {
                string aspId = HttpContext.Session.GetString("aspNetUserId");
                int physicianId = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == aspId).Physicianid;

                schedulingVm.regions = _adminDashboard.GetPhysicianRegions(physicianId);
                schedulingVm.callId = 3;
                schedulingVm.physicianId = physicianId;
            }
            return PartialView("Admin/Scheduling/_CreateShift", schedulingVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Create Shift and fetch schedulling table records as per day,week,month
        /// </summary>
        /// <param name="schedulingVm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult createShiftPost(SchedulingVm schedulingVm, int callId)
        {
            string aspId = HttpContext.Session.GetString("aspNetUserId");

            if (_adminDashboard.createShift(schedulingVm.ScheduleModel, aspId))
            {
                return Json(new { callId = callId });
            }
            return Ok(false);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch View Shift Modal
        /// </summary>
        /// <param name="viewShiftModal"></param>
        /// <returns></returns>
        public IActionResult OpenScheduledModal(ViewShiftModal viewShiftModal)
        {
            string aspId = HttpContext.Session.GetString("aspNetUserId");

            switch (viewShiftModal.actionType)
            {
                case "shiftdetails":
                    ShiftDetailsmodal shift = _adminDashboard.GetShift(viewShiftModal.shiftdetailsid);
                    return PartialView("Admin/Scheduling/_ViewShift", shift);

                case "moreshifts":
                    DateTime date = DateTime.Parse(viewShiftModal.datestring);
                    ShiftDetailsmodal ScheduleModel = new ShiftDetailsmodal();
                    var list = ScheduleModel.ViewAllList = _adminDashboard.ShiftDetailsmodal(date, DateTime.Now, DateTime.Now, "month", aspId).Where(i => i.Shiftdate.Day == viewShiftModal.columnDate.Day).ToList();
                    ViewBag.TotalShift = list.Count();
                    return PartialView("Admin/Scheduling/_MoreShift", ScheduleModel);

                default:

                    return PartialView();
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch view shift modal for week
        /// </summary>
        /// <param name="sundaystring"></param>
        /// <param name="saturdaystring"></param>
        /// <param name="datestring"></param>
        /// <param name="shiftdate"></param>
        /// <param name="physicianid"></param>
        /// <returns></returns>
        public IActionResult OpenScheduledModalWeek(string sundaystring, string saturdaystring, string datestring, DateTime shiftdate, int physicianid)
        {
            string aspId = HttpContext.Session.GetString("aspNetUserId");

            DateTime sunday = DateTime.Parse(sundaystring);
            DateTime saturday = DateTime.Parse(saturdaystring);

            DateTime date1 = DateTime.Parse(datestring);
            ShiftDetailsmodal ScheduleModel = new ShiftDetailsmodal();
            var list = ScheduleModel.ViewAllList = _adminDashboard.ShiftDetailsmodal(date1, sunday, saturday, "week", aspId).Where(i => i.Shiftdate.Day == shiftdate.Day && i.Physicianid == physicianid).ToList();
            ViewBag.TotalShift = list.Count();

            return PartialView("Admin/Scheduling/_MoreShift", ScheduleModel);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Return Shift (approve/disapprove) and fetch schedulling table records
        /// </summary>
        /// <param name="status"></param>
        /// <param name="shiftdetailid"></param>
        /// <returns></returns>
        public IActionResult ReturnShift(int status, int shiftdetailid)
        {
            string aspId = HttpContext.Session.GetString("aspNetUserId");

            _adminDashboard.SetReturnShift(status, shiftdetailid, aspId);
            return Ok();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Delete Shift and fetch schedulling table records
        /// </summary>
        /// <param name="shiftdetailid"></param>
        /// <returns></returns>
        public IActionResult deleteShift(int shiftdetailid)
        {
            string aspId = HttpContext.Session.GetString("aspNetUserId");

            _adminDashboard.SetDeleteShift(shiftdetailid, aspId);
            return Ok();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Edit Shift Records and fetch schedulling table records
        /// </summary>
        /// <param name="shiftDetailsmodal"></param>
        /// <returns></returns>
        public IActionResult EditShiftDetails(ShiftDetailsmodal shiftDetailsmodal)
        {
            string aspId = HttpContext.Session.GetString("aspNetUserId");

            _adminDashboard.SetEditShift(shiftDetailsmodal, aspId);
            return Ok();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Shift Review 
        /// </summary>
        /// <param name="regionId"></param>
        /// <param name="callId"></param>
        /// <returns></returns>
        public IActionResult ShiftReview(int regionId, int callId)
        {
            SchedulingVm schedulingVm = new SchedulingVm();
            schedulingVm.regions = _adminDashboard.GetRegions();
            schedulingVm.ShiftReview = _adminDashboard.GetShiftReview(regionId, callId);
            schedulingVm.regionId = regionId;
            schedulingVm.callId = callId;

            return PartialView("Admin/Scheduling/_ShiftReview", schedulingVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Approve Selected Shifts and fetch shift review
        /// </summary>
        /// <param name="shiftDetailsId"></param>
        /// <returns></returns>
        public IActionResult ApproveShift(int[] shiftDetailsId)
        {
            string aspId = HttpContext.Session.GetString("aspNetUserId");

            _adminDashboard.ApproveSelectedShift(shiftDetailsId, aspId);
            return Ok();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Delete Selected shifts and fetch shift review
        /// </summary>
        /// <param name="shiftDetailsId"></param>
        /// <returns></returns>
        public IActionResult DeleteSelectedShift(int[] shiftDetailsId)
        {
            string aspId = HttpContext.Session.GetString("aspNetUserId");

            _adminDashboard.DeleteShiftReview(shiftDetailsId, aspId);

            return Ok();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch MdOnCall records
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public IActionResult MDOnCall(int regionId)
        {
            OnCallModal onCallModal = _adminDashboard.GetOnCallDetails(regionId);

            return PartialView("Admin/Scheduling/_MDsOnCall", onCallModal);
        }

        #endregion


        #region Provider's Location

        //***************************************************************************************************************************************************
        /// <summary>
        /// Get Physician Location Table's Records
        /// </summary>
        /// <returns></returns>
        public IActionResult GetLocations()
        {
            List<Physicianlocation> getLocation = _adminDashboard.GetPhysicianlocations();
            return Ok(getLocation);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Provider's Location Tab
        /// </summary>
        /// <returns></returns>
        public IActionResult ProviderLocation()
        {
            return PartialView("Admin/Provider_Location/_Provider_Location");
        }

        #endregion


        #region Partner's Tab

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Partner's Tab
        /// </summary>
        /// <param name="professionid"></param>
        /// <returns></returns>
        public IActionResult Partners(int professionid)
        {
            var Partnersdata = _adminDashboard.GetPartnersdata(professionid);

            PartnersVm partnersVm = new PartnersVm();
            partnersVm.Partnersdata = Partnersdata;
            partnersVm.Professions = _adminDashboard.getHealthProfessionalTypes();

            return PartialView("Admin/Partners/_Partners", partnersVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Add Business OR Edit Business
        /// </summary>
        /// <param name="vendorID"></param>
        /// <returns></returns>
        public IActionResult AddBusiness(int vendorID)
        {
            if (vendorID == 0)
            {
                PartnersVm partnersVm = new PartnersVm();
                partnersVm.Professions = _adminDashboard.getHealthProfessionalTypes();
                partnersVm.regions = _adminDashboard.GetRegions();
                partnersVm.vendorID = vendorID;

                return PartialView("Admin/Partners/_Partners_Create_Edit", partnersVm);
            }
            else
            {
                PartnersVm partnersVm = _adminDashboard.GetEditBusinessData(vendorID);
                partnersVm.Professions = _adminDashboard.getHealthProfessionalTypes();
                partnersVm.regions = _adminDashboard.GetRegions();
                partnersVm.vendorID = vendorID;

                return PartialView("Admin/Partners/_Partners_Create_Edit", partnersVm);
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Add Business (healthprofessional) and fetch Partner's Tab
        /// </summary>
        /// <param name="partnersVm"></param>
        /// <returns></returns>
        public IActionResult CreateNewBusiness(PartnersVm partnersVm)
        {
            string aspId = HttpContext.Session.GetString("aspNetUserId");

            if (_adminDashboard.CreateNewBusiness(partnersVm, aspId))
            {
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Business Records (healthprofessional) and fetch Edit Business
        /// </summary>
        /// <param name="partnersVm"></param>
        /// <returns></returns>
        public IActionResult UpdateBusiness(PartnersVm partnersVm)
        {
            if (_adminDashboard.UpdateBusiness(partnersVm))
            {
                return Json(new { Success = true, vendorid = partnersVm.vendorID });
            }
            return Json(new { Success = false, vendorid = partnersVm.vendorID });
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Delete Business (healthprofessional) and fetch Partner's tab
        /// </summary>
        /// <param name="VendorID"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DelPartner(int VendorID)
        {
            _adminDashboard.DelPartner(VendorID);
            return Ok();
        }

        #endregion


        #region Records Tab

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Patient History
        /// </summary>
        /// <param name="getRecordsModel"></param>
        /// <returns></returns>
        public IActionResult GetRecordsTab(GetRecordsModel getRecordsModel)
        {
            getRecordsModel = _adminDashboard.GetRecordsTab(0, getRecordsModel);

            return PartialView("Admin/Records/_GetRecordsTab", getRecordsModel);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Explore in Patient History
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public IActionResult GetPatientRecordExplore(int UserId)
        {
            GetRecordsModel model = new GetRecordsModel();
            model = _adminDashboard.GetRecordsTab(UserId, model);
            return PartialView("Admin/Records/_ExploreRecords", model);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Block History
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IActionResult GetBlockedRequest(BlockedRequestModel model)
        {
            model = _adminDashboard.GetBlockedRequest(model);

            return PartialView("Admin/Records/_BlockedHistory", model);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update (unblock) Block History
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public IActionResult UnblockRequest(int requestId)
        {
            _adminDashboard.UnblockRequest(requestId);
            return Ok();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Email/SMS Log
        /// </summary>
        /// <param name="tempId"></param>
        /// <returns></returns>
        public IActionResult GetEmailSmsLog(int tempId)
        {
            EmailSmsLogModel model = new EmailSmsLogModel();
            model = _adminDashboard.GetEmailSmsLog(tempId, model);

            return PartialView("Admin/Records/_EmailSmsLog", model);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Filter Email/SMS lOG
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IActionResult emailSmsLogFilter(EmailSmsLogModel model)
        {
            model = _adminDashboard.GetEmailSmsLog((int)model.tempid, model);
            return PartialView("Admin/Records/_EmailSmsLog", model);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Search Records
        /// </summary>
        /// <param name="searchRecordsModel"></param>
        /// <returns></returns>
        public IActionResult GetSearchRecords(SearchRecordsModel searchRecordsModel)
        {
            searchRecordsModel = _adminDashboard.GetSearchRecords(searchRecordsModel);
            searchRecordsModel.requestType = _adminDashboard.GetRequesttypes();

            return PartialView("Admin/Records/_SearchRecords", searchRecordsModel);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Exports in Search Records
        /// </summary>
        /// <param name="searchRecordsModel"></param>
        /// <returns></returns>
        public IActionResult ExportSearchRecords(SearchRecordsModel searchRecordsModel)
        {
            var requests = _adminDashboard.GetSearchRecords(searchRecordsModel);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var excelPackage = new ExcelPackage())
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("SearchRecords");

                // Add headers
                worksheet.Cells[1, 1].Value = "Request ID";
                worksheet.Cells[1, 2].Value = "Patient Name";
                worksheet.Cells[1, 3].Value = "Requestor";
                worksheet.Cells[1, 4].Value = "Date of Service";
                worksheet.Cells[1, 5].Value = "Close Case Date";
                worksheet.Cells[1, 6].Value = "Email";
                worksheet.Cells[1, 7].Value = "Contact";
                worksheet.Cells[1, 8].Value = "Address";
                worksheet.Cells[1, 9].Value = "Zip";
                worksheet.Cells[1, 10].Value = "Status";
                worksheet.Cells[1, 11].Value = "Physician";
                worksheet.Cells[1, 12].Value = "Physician Note";
                worksheet.Cells[1, 13].Value = "Provider Note";
                worksheet.Cells[1, 14].Value = "Admin Note";
                worksheet.Cells[1, 15].Value = "Patient Note";
                worksheet.Cells[1, 16].Value = "Request Type ID";
                worksheet.Cells[1, 17].Value = "User ID";

                // Populate data
                for (int i = 0; i < requests.requestList.Count; i++)
                {
                    var rowData = requests.requestList[i];
                    worksheet.Cells[i + 2, 1].Value = rowData.requestid;
                    worksheet.Cells[i + 2, 2].Value = rowData.patientname;
                    worksheet.Cells[i + 2, 3].Value = rowData.requestor;
                    worksheet.Cells[i + 2, 4].Value = rowData.dateOfService;
                    worksheet.Cells[i + 2, 5].Value = rowData.closeCaseDate;
                    worksheet.Cells[i + 2, 6].Value = rowData.email;
                    worksheet.Cells[i + 2, 7].Value = rowData.contact;
                    worksheet.Cells[i + 2, 8].Value = rowData.address;
                    worksheet.Cells[i + 2, 9].Value = rowData.zip;
                    worksheet.Cells[i + 2, 10].Value = rowData.status;
                    worksheet.Cells[i + 2, 11].Value = rowData.physician;
                    worksheet.Cells[i + 2, 12].Value = rowData.physicianNote;
                    worksheet.Cells[i + 2, 13].Value = rowData.providerNote;
                    worksheet.Cells[i + 2, 14].Value = rowData.AdminNote;
                    worksheet.Cells[i + 2, 15].Value = rowData.pateintNote;
                    worksheet.Cells[i + 2, 16].Value = rowData.requestTypeId;
                    worksheet.Cells[i + 2, 17].Value = rowData.userid;
                }
                // Convert the Excel package to a byte array
                byte[] excelBytes = excelPackage.GetAsByteArray();

                // Return the Excel file as a download
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Delete in Search Records
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public IActionResult deletRequest(int requestId)
        {
            _adminDashboard.deletRequest(requestId);
            return Ok();
        }

        #endregion


        #region  Pay Rate

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Enter Payrate
        /// </summary>
        /// <param name="aspId"></param>
        /// <param name="phyid"></param>
        /// <returns></returns>
        public IActionResult GetEnterPayrate(string aspId, int phyid)
        {
            string adminAspid = HttpContext.Session.GetString("aspNetUserId");

            PayrateVm payrateVm = new PayrateVm();
            payrateVm.AspId = aspId;
            payrateVm.Phyid = phyid;
            payrateVm.PayrateForProvider = _adminDashboard.GetPayRateForProvider(phyid, adminAspid);

            return PartialView("Admin/Providers/_Provider_Payrate", payrateVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Enter Payrate
        /// </summary>
        /// <param name="category"></param>
        /// <param name="payrate"></param>
        /// <param name="phyid"></param>
        /// <returns></returns>
        public IActionResult PostPayrate(int category, int payrate, int phyid)
        {
            string adminAspId = HttpContext.Session.GetString("aspNetUserId");

            if (_adminDashboard.PostPayrate(category, payrate, phyid, adminAspId))
            {
                return Ok(true);
            }
            return Ok(false);
        }

        #endregion


        #region Invoicing

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Admin Invocing
        /// </summary>
        /// <returns></returns> 
        public IActionResult GetAdminInvoicing()
        {
            AdminInvoicingVm adminInvoicingVm = new AdminInvoicingVm();
            adminInvoicingVm.physiciansList = _adminDashboard.GetPhysicians();
            adminInvoicingVm.Physicianname = "";

            return PartialView("Admin/Invoicing/_Invoicing", adminInvoicingVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Admin Invocing for Particular Physician's Selected Time Duration
        /// </summary>
        /// <param name="phyid"></param>
        /// <param name="dateSelected"></param>
        /// <returns></returns>
        public IActionResult GetTimeSheetDetail(int phyid, string dateSelected)
        {
            if (phyid == 0 || dateSelected == null)
            {
                return GetAdminInvoicing();
            }
            else
            {
                AdminInvoicingVm adminInvoicingVm = new AdminInvoicingVm();
                adminInvoicingVm.PhysicianId = phyid;
                adminInvoicingVm.Physicianname = "Dr. " + _context.Physicians.FirstOrDefault(x => x.Physicianid == phyid).Firstname + " " + _context.Physicians.FirstOrDefault(x => x.Physicianid == phyid).Lastname;
                adminInvoicingVm.physiciansList = _adminDashboard.GetPhysicians();
                adminInvoicingVm.timesheetsList = _adminDashboard.GetTimeSheetDetail(phyid, dateSelected);

                return PartialView("Admin/Invoicing/_Invoicing", adminInvoicingVm);
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Admin Finalize Timesheet
        /// .Where(x => timeSheetDetailId.Contains(x.TimesheetDetailId)
        /// </summary>
        /// <param name="dateSelected"></param>
        /// <param name="phyid"></param>
        /// <returns></returns>
        public IActionResult GetAdminFinalizeTimeSheet(string dateSelected, int phyid)
        {
            ProviderInvoicingVm? providerInvoicingVm = new ProviderInvoicingVm();
            providerInvoicingVm.ProviderTimesheetDetails = _providerDashboard.GetFinalizeTimeSheetDetails(phyid, dateSelected);
            providerInvoicingVm.callId = 1;
            providerInvoicingVm.PayrateByProvider = _adminDashboard.GetPayRateForProviderByPhyId(phyid);

            return PartialView("Provider/Invoicing/_Provider_FinalizeTimeSheet", providerInvoicingVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Approve Timesheet
        /// </summary>
        /// <param name="timeSheetId"></param>
        /// <param name="bonus"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ConfirmApproveTimeSheet(int timeSheetId, int bonus, string notes)
        {
            if (_adminDashboard.ApproveTimeSheet(timeSheetId, bonus, notes))
            {
                return Ok(true);
            }
            return Ok(false);
        }

        #endregion


        #region ChatWith

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch ChatWith 
        /// </summary>
        /// <param name="Reqid"></param>
        /// <param name="ReciverAspid"></param>
        /// <returns></returns>
        public IActionResult ChatPersonDetails(int Reqid, string ReciverAspid)
        {
            var senderId = HttpContext.Session.GetString("aspNetUserId");

            ChatVm chatVm = _adminDashboard.GetChatData(Reqid, senderId, ReciverAspid);

            return Ok(chatVm);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update ChatWith
        /// </summary>
        /// <param name="Reqid"></param>
        /// <param name="receiverId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public IActionResult AddChatDetails(int Reqid, string receiverId, string message)
        {
            var senderId = HttpContext.Session.GetString("aspNetUserId");

            _adminDashboard.AddChatHistory(Reqid, senderId, receiverId, message);

            return Ok();
        }

        #endregion

        //***************************************************************************************************************************************************
        //***************************************************************************************************************************************************
        //***************************************************************************************************************************************************
    }
}