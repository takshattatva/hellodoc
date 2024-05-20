using hellodoc.DAL.Models;
using hellodoc.DAL.ViewModels;

namespace hellodoc.BAL.Interface
{
    public interface IPatientReqRepo
    {
        #region Get Regions For Request Forms

        List<Region> GetRegions();

        #endregion


        #region Email Check For Request Form

        bool EmailCheck(String Email);

        #endregion


        #region Get UserId

        int GetUserId(String email);

        #endregion


        #region Post Data of Requests

        void AddToUser(PatientReqData model);
        
        void AddToRequest(PatientReqData model);

        void AddToReqClient(PatientReqData model);

        void UploadFile(PatientReqData model);

        void AddToReqConcierge(PatientReqData model);

        void AddToConcierge(PatientReqData model);

        void AddToReqBusiness(PatientReqData model);

        void AddToBusiness(PatientReqData model);

        void AddToEmailLog(PatientReqData model);

        void UploadFileByMeAndOther(PatientReqData model, int reqId);

        #endregion


        #region Send Mail to Create Acc

        public Task EmailSender(string email, string subject, string message);

        #endregion


        #region Review Agreement

        public ReviewAgreementVm GetReviewAgreement(int reqId, int userId);
        public void AgreeReview(int reqId);
        public void CancelReview(ReviewAgreementVm reviewAgreementVm);

        #endregion
    }
}
