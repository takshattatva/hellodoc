using hellodoc.BAL.Interface;
using hellodoc.DAL.Models;
using hellodoc.DAL.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace hellodoc.BAL.Repository
{
    public class PatientDashRepo : IPatientDashRepo
    {
        public readonly HellodocDbContext _Context;
        public readonly IHostingEnvironment _environment;

        public PatientDashRepo(HellodocDbContext context, IHostingEnvironment environment)
        {
            _Context = context;
            _environment = environment;

        }

        #region Patient Dashboard

        //***************************************************************************************************************************************************
        /// <summary>
        /// Get Request Records for Patient Dashboard
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<DashboardData> RequestList(int userid)
        {
            var request = _Context.Requests.Where(r => r.Userid == userid).AsNoTracking();

            var r = _Context.Requests.FirstOrDefault(r => r.Userid == userid);

            var RequestList = request.Select(r => new DashboardData()
            {
                RequestId = r.Requestid,
                CreatedDate = r.Createddate,
                Status = r.Status,
                DocumentCount = r.Requestwisefiles.Select(f => f.Filename).Count(),
                //RequestId = r.Requestwisefiles.Select(f => f.Requestid).FirstOrDefault(),
                ConfirmationNumber = r.Confirmationnumber,
                physicianname = r.Physician.Firstname + " " + r.Physician.Lastname,
                AdminAspId = "3bdd2ffd-041a-46b9-984c-24e7b01f8fd4",
                PhyAspId = r.Physician.Aspnetuserid,
                PhysicianId = r.Physicianid,

            }).ToList();
            return RequestList;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Get Document List for Patient View Doc
        /// </summary>
        /// <param name="reqId"></param>
        /// <returns></returns>
        public List<DocumentData> DocumentList(int reqId)
        {
            var requestWiseFile = _Context.Requestwisefiles.Where(rwf => rwf.Requestid == reqId).AsNoTracking();

            var DocumentList = requestWiseFile.Select(rwf => new DocumentData()
            {
                CreatedDate = rwf.Createddate,
                DocumentName = rwf.Filename,

            }).ToList();
            return DocumentList;
        }

        #endregion


        #region Patient View Doc

        //***************************************************************************************************************************************************
        /// <summary>
        /// Upload File for Patient View Doc
        /// </summary>
        /// <param name="patientDashData"></param>
        /// <param name="reqId"></param>
        /// <returns></returns>
        public bool DashboardUpload(PatientDashData patientDashData, int reqId)
        {
            IFormFile File1 = patientDashData.Upload;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content", File1.FileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                File1.CopyTo(fileStream);
            }

            var fileName = patientDashData.Upload?.FileName;

            var fileData = new Requestwisefile()
            {
                Requestid = reqId,
                Filename = fileName,
            };
            _Context.Add(fileData);
            _Context.SaveChanges();

            return true;
        }

        #endregion


        #region Patient Profile

        //***************************************************************************************************************************************************
        /// <summary>
        /// Get Records for Profile Data
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ProfileData GetProfileData(int userId)
        {
            var userData = _Context.Users.FirstOrDefault(u => u.Userid == userId);
            var region = _Context.Requestclients.FirstOrDefault(x => x.Email == userData.Email).Regionid;

            var BirthDay = Convert.ToInt32(userData.Intdate);
            var BirthMonth = Convert.ToInt32(userData.Strmonth);
            var BirthYear = Convert.ToInt32(userData.Intyear);

            ProfileData profileData = new ProfileData();
            profileData.Firstname = userData.Firstname;
            profileData.Lastname = userData.Lastname;
            profileData.PhoneNumber = userData.Mobile;
            profileData.Email = userData.Email;
            profileData.Street = userData.Street;
            profileData.City = userData.City;
            profileData.State = userData.State;
            profileData.Zipcode = userData.Zipcode;
            profileData.Birthdate = new DateTime(BirthYear, BirthMonth, BirthDay);
            profileData.RegionId = region;

            return profileData;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Profile Records
        /// </summary>
        /// <param name="updatedProfileData"></param>
        /// <param name="userId"></param>
        /// <param name="reqId"></param>
        public void SetProfileData(ProfileData updatedProfileData, int userId)
        {
            var u = _Context.Users.FirstOrDefault(u => u.Userid == userId);
            var rc = _Context.Requestclients.FirstOrDefault(x => x.Email == u.Email);

            if (u != null && rc != null)
            {
                u.Firstname = updatedProfileData.Firstname;
                u.Lastname = updatedProfileData.Lastname;
                u.Mobile = updatedProfileData.PhoneNumber;
                u.Street = updatedProfileData.Street;
                u.City = updatedProfileData.City;
                u.State = updatedProfileData.State;
                u.Zipcode = updatedProfileData.Zipcode;
                var dt = updatedProfileData.Birthdate;
                u.Intdate = dt.Day;
                u.Strmonth = dt.Month.ToString();
                u.Intyear = dt.Year;
                rc.Regionid = updatedProfileData.RegionId;
            }

            var aspUserRecord = _Context.Aspnetusers.FirstOrDefault(x => x.Email == u.Email);

            if (aspUserRecord != null)
            {
                aspUserRecord.Phonenumber = updatedProfileData.PhoneNumber;
                aspUserRecord.Modifieddate = DateTime.Now;
            }
            _Context.SaveChanges();
        }

        #endregion

        //***************************************************************************************************************************************************
        //***************************************************************************************************************************************************
        //***************************************************************************************************************************************************
    }
}