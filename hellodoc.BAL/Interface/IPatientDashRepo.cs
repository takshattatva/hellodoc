using hellodoc.DAL.Models;
using hellodoc.DAL.ViewModels;

namespace hellodoc.BAL.Interface
{
    public interface IPatientDashRepo 
    {
        #region Get Patient Request Data 

        List<DashboardData> RequestList(int userId);

        #endregion


        #region Get Documents Data

        List<DocumentData> DocumentList(int reqId);

        #endregion


        #region View Doc Upload File Post

        bool DashboardUpload(PatientDashData patientDashData, int reqId);

        #endregion


        #region Patient Profile

        ProfileData GetProfileData(int userId);
        void SetProfileData(ProfileData updatedProfileData, int userId);

        #endregion
    }
}
