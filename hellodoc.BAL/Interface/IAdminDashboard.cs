using hellodoc.DAL.Models;
using hellodoc.DAL.ViewModels;
using Microsoft.AspNetCore.Http;

namespace hellodoc.BAL.Interface
{
    public interface IAdminDashboard
    {
        #region Dashboard

        // ***************************** Table fetch Records *****************************
        List<RequestListAdminDash> getRequestData(int[] Status, string requesttypeid, int regionid);


        // ***************************** View Case fetch and update Records *****************************
        AdminViewCaseData getViewCaseData(int requestid);
        void setViewCaseData(AdminViewCaseData updatedViewCaseData, int requestid);


        // ***************************** Region fetch Records *****************************
        List<Region> GetRegions();
        List<Region> GetPhysicianRegions(int physicianId);


        // ***************************** fetch Count of Request as per StatusForName *****************************
        CountRequest GetCountRequest();


        // ***************************** View Notes fetch and update Records *****************************
        ViewNotesData GetViewNotesData(int requestid);
        void setViewNotesData(ViewNotesData updatedViewNotesData, int requestid);


        // ***************************** CaseTag fetch Records *****************************
        List<Casetag> GetCasetags();


        // ***************************** Cancel Case fetch and update Records *****************************
        CancelCaseModal getCancelCaseData(int requestid);
        void setCancelCaseData(CancelCaseModal updatedCancelCaseData);


        // ***************************** Physician's Table Records *****************************
        List<Physician> GetPhysicians(int regionid);
        List<Physician> GetPhysicians();
        List<Physician> GetPhysiciansForAssign(int regionid, int requestid, string aspId);


        // ***************************** Assign Case fetch and update Records *****************************
        AssignCaseModal GetAssignCaseData(int requestid);
        void SetAssignCaseData(AssignCaseModal updatedAssignCaseModal);


        // ***************************** Block Case fetch and update Records *****************************
        BlockCaseModal GetBlockCaseModal(int requestid);
        void SetBlockCaseData(BlockCaseModal updatedBlockCaseData);


        // ***************************** View Documents fetch and update Records *****************************
        AdminViewDocumentsVm GetViewDocumentsData(int requestid);
        List<ViewDocuments> GetViewDocumentsList(int requestid);
        void SetViewDocumentData(AdminViewDocumentsVm adminViewDocumentsVm);
        void DeleteFileData(int requestwisefileid);
        public Task SendEmailWithFile(int[] requestwisefileid, int requestid, int adminId);


        // ***************************** Send Order fetch and update Records *****************************
        public SendOrderModel getOrderData(int requestid);
        public List<Healthprofessionaltype> getHealthProfessionalTypes();
        public List<Healthprofessional> getHealthProfessionals(int health_professional_id); 
        public SendOrderModel GetVendordata(int vendorid);
        public void setOrderData(SendOrderModel sendOrderModel);


        // ***************************** Transfer Case fetch and update Records *****************************
        TransferCaseModal GetTransferCaseData(int requestid);
        void SetTransferCaseData(TransferCaseModal updatedTransferCaseModal);


        // ***************************** Clear Case fetch and update Records *****************************
        public ClearCaseModel getClearCaseData(int requestid);
        public void setClearCaseData(ClearCaseModel updatedClearCaseData);


        // ***************************** Send Agreement fetch and update Records *****************************
        SendAgreementModal GetSendAgreementModal(int requestid, int reuqesttypeid);
        void SendAgreementEmail(SendAgreementModal sendAgreementModal);


        // ***************************** Send Link update Records *****************************
        void SendSubmitRequestLink(SendLink sendLink, int senderId);


        // ***************************** Create Request By Admin/Physician update Records *****************************
        void SendCreateRequestData(AdminCreateRequestVm adminCreateRequestVm, int senderId);
        void InsertRequestData(AdminCreateRequestVm adminCreateRequestVm, int reqTypeId, int senderId);
        void InsertRequestClientData(AdminCreateRequestVm adminCreateRequestVm, int requestId);
        void InsertNotesData(AdminCreateRequestVm adminCreateRequestVm, int requestId, string aspId);


        // ***************************** Close Case fetch and update Records *****************************
        AdminCloseCaseVm GetCloseCaseData(int requestId);
        List<Documents> GetCloseCaseDocuments(int requestId);
        void UpdateCloseCaseData(AdminCloseCaseVm adminCloseCaseVm);
        void SetClosedCase(int requestId);


        // ***************************** Encounter form fetch and update Records *****************************
        EncounterVm GetEncounterData(int requestid, int status);
        void SetEncounterData(EncounterVm encounterVm);


        // ***************************** Request Duty Support Update Records *****************************
        bool RequestSupportViaEmail(string message, int adminId);

        #endregion


        #region Profiles ( Admin & Physician )

        // ***************************** Admin Edit Profile fetch and update Records *****************************
        List<Role> GetRolesForAdmin(string aspId);
        List<AdminregionTable> GetAdminregions(string aspId);
        AdminProfileVm GetProfileData(string aspId);
        void AdminResetPassword(string password, string aspId);
        void AdminAccountUpdate(short status, int roleId, string aspId);
        void AdministratorDetail(AdminProfileVm adminProfileVm, List<int> regions);
        void MailingDetail(AdminProfileVm adminProfileVm);
        void RemoveAdmin(int adminId);


        // ***************************** Physician Profile Edit fetch and update Records *****************************
        List<Role> GetRolesForPhysicians();
        List<Provider> GetProviders(int regionId);
        int StopNotificationPhysician(int physicianId);
        void ContactProviderViaEmail(ProvidersVm providersVm, string aspId);
        void ContactProviderViaContact(ProvidersVm providersVm, string aspId);
        List<PhysicianRegionTable> GetPhysicianRegionTables(string aspId);
        ProviderProfileVm GetProviderProfileData(string aspId);
        void PhysicianResetPassword(string password, string aspId);
        void PhysicianAccountUpdate(short status, int roleId, string aspId);
        void PhysicianAdministratorDataUpdate(ProviderProfileVm providerProfileVm, List<int> physicianRegions);
        void PhysicianMailingDataUpdate(ProviderProfileVm providerProfileVm);
        void PhysicianBusinessInfoUpdate(ProviderProfileVm providerProfileVm);
        void AddProviderBusinessPhotos(IFormFile photo, IFormFile signature, string aspId);
        void EditOnBoardingData(ProviderProfileVm providerProfileVm);
        bool RemovePhysician(int physicianId);


        // ***************************** Physician Crete Account update Records *****************************
        bool CreatePhysicianAccount(ProviderProfileVm providerProfileVm, List<int> physicianRegions);
        void AddProviderDocuments(int Physicianid, IFormFile Photo, IFormFile ContractorAgreement, IFormFile BackgroundCheck, IFormFile HIPAA, IFormFile NonDisclosure);

        #endregion


        #region Access

        // ***************************** Account Access Create/Edit fetch and update Records *****************************
        List<AccountAccess> GetAccountAccessData();
        List<Aspnetrole> GetAccountType();
        List<Menu> GetMenu(int accounttype);
        List<AccountMenu> GetAccountMenu(int accounttype, int roleid);
        void SetCreateAccessAccount(AccountAccess accountAccess, List<int> AccountMenu);
        void SetEditAccessAccount(AccountAccess accountAccess, List<int> AccountMenu);
        void DeleteAccountAccess(int roleid);
        AccountAccess GetEditAccessData(int roleid);


        // ***************************** User Access fetch Records *****************************
        List<UserAccess> GetUserAccessData(int accountTypeId);


        // ***************************** Admin Create Account update Records *****************************
        bool CreateAdminAccount(AdminProfileVm adminProfileVm, List<int> adminRegions);

        #endregion


        #region Schedulling

        // ***************************** Scheduling fetch and update Records *****************************
        public List<ShiftDetailsmodal> ShiftDetailsmodal(DateTime date, DateTime sunday, DateTime saturday, string type, string aspId);


        // ***************************** Create Shift update Records *****************************
        bool createShift(ScheduleModel scheduleModel, string Aspid);


        // ***************************** View Shift fetch and update Records *****************************
        ShiftDetailsmodal GetShift(int shiftdetailsid);
        void SetReturnShift(int status, int shiftdetailid, string Aspid);
        public void SetDeleteShift(int shiftdetailid, string Aspid);
        public void SetEditShift(ShiftDetailsmodal shiftDetailsmodal, string Aspid);


        // ***************************** Shift Review fetch and update Records *****************************
        public List<ShiftReview> GetShiftReview(int regionId, int callId);
        public void ApproveSelectedShift(int[] shiftDetailsId, string Aspid);
        public void DeleteShiftReview(int[] shiftDetailsId, string Aspid);


        // ***************************** MDsOnCall fetch Records *****************************
        public OnCallModal GetOnCallDetails(int regionId);

        #endregion


        #region Provider Location

        // ***************************** Provider Location fetch Records *****************************
        public List<Physicianlocation> GetPhysicianlocations();

        #endregion


        #region Partners

        // ***************************** Partners fetch and update Records  *****************************
        public List<Partnersdata> GetPartnersdata(int professionid);
        void DelPartner(int VendorID);


        // ***************************** Create Business update Records *****************************
        public bool CreateNewBusiness(PartnersVm partnersVm, string LoggerAspnetuserId);


        // ***************************** Edit Business fetch and update Records *****************************
        public PartnersVm GetEditBusinessData(int vendorID);
        public bool UpdateBusiness(PartnersVm partnersVm);

        #endregion


        #region Records Tab

        // ***************************** Records *****************************
        GetRecordsModel GetRecordsTab(int UserId, GetRecordsModel model);
        BlockedRequestModel GetBlockedRequest(BlockedRequestModel model);
        void UnblockRequest(int requestId);
        EmailSmsLogModel GetEmailSmsLog(int tempId, EmailSmsLogModel model);
        SearchRecordsModel GetSearchRecords(SearchRecordsModel model);
        List<Requesttype> GetRequesttypes();
        void deletRequest(int requestId);

        #endregion


        #region Pay Rate

        List<PayRateForProviderVm> GetPayRateForProvider(int phyid, string adminAspId);

        bool PostPayrate(int category, int payrate, int phyid, string adminAspId);

        #endregion


        #region Invoicing

        List<Timesheet> GetTimeSheetDetail(int phyid, string dateSelected);

        List<PayrateByProvider> GetPayRateForProviderByPhyId(int phyid);

        bool ApproveTimeSheet(int timeSheetId, int bonus, string notes);

        #endregion


        #region ChatWith

        ChatVm GetChatData(int Reqid, string Senderid, string ReciverAspid);

        void AddChatHistory(int Reqid, string senderId, string Receiverid, string Message);

        #endregion
    }
}
 