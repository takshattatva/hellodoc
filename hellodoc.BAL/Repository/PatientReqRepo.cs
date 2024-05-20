using hellodoc.BAL.Interface;
using hellodoc.DAL.Models;
using hellodoc.DAL.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Net;
using System.Net.Mail;

namespace hellodoc.BAL.Repository
{
    public class PatientReqRepo : IPatientReqRepo
    {
        private readonly HellodocDbContext _db;
        private readonly IHostingEnvironment _environment;

        public PatientReqRepo(HellodocDbContext db, IHostingEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }

        #region Check Mail During Patient Request

        //***************************************************************************************************************************************************
        /// <summary>
        /// Check email for login
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public bool EmailCheck(string Email)
        {
            return _db.Users.Any(x => x.Email == Email);
        }

        #endregion


        #region Use Already Existed UserId @Patient Request

        //***************************************************************************************************************************************************
        /// <summary>
        /// Get userid for login
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public int GetUserId(String email)
        {
            var user = _db.Users.FirstOrDefault(x => x.Email == email);
            return user.Userid;
        }

        #endregion


        #region Get Regions for Request forms

        //***************************************************************************************************************************************************
        /// <summary>
        /// Get Region Table 
        /// </summary>
        /// <returns></returns>
        public List<Region> GetRegions()
        {
            return _db.Regions.ToList();
        }

        #endregion


        #region Set User's Data

        //***************************************************************************************************************************************************
        /// <summary>
        /// Add Data in Table(anu,u,anur) (4 requests)
        /// </summary>
        /// <param name="model"></param>
        public void AddToUser(PatientReqData model)
        {
            Aspnetuser anu = new Aspnetuser();
            Aspnetuserrole anur = new Aspnetuserrole();
            User u = new User();

            anu.Username = model.Firstname + model.Lastname;
            anu.Passwordhash = BCrypt.Net.BCrypt.HashPassword(model.PasswordHash);
            anu.Email = model.Email;
            anu.Phonenumber = model.Phonenumber;
            anu.Createddate = DateTime.Now;

            _db.Aspnetusers.Add(anu);
            _db.SaveChanges();
            model.AspNetUserId = anu.Id;

            anur.Userid = model.AspNetUserId;
            anur.Roleid = "3";
            _db.Aspnetuserroles.Add(anur);
            _db.SaveChanges();

            u.Aspnetuserid = model.AspNetUserId;
            u.Firstname = model.Firstname;
            u.Lastname = model.Lastname;
            u.Email = model.Email;
            u.Mobile = model.Phonenumber;
            u.Street = model.Street;
            u.City = model.City;
            u.State = model.State;
            u.Zipcode = model.Zipcode;
            u.Createdby = model.AspNetUserId;
            u.Createddate = DateTime.Now;
            var dt = model.Birthdate;
            u.Intdate = dt.Day;
            u.Strmonth = dt.Month.ToString();
            u.Intyear = dt.Year;
            _db.Users.Add(u);
            _db.SaveChanges();
            model.UserId = u.Userid;
        }

        #endregion


        #region Set Request's Data

        //***************************************************************************************************************************************************
        /// <summary>
        /// Add Data in Table(r) (4 requests)
        /// </summary>
        /// <param name="model"></param>
        public void AddToRequest(PatientReqData model)
        {
            User? user = _db.Users.FirstOrDefault(i => i.Email == model.Email);
            Region? region = _db.Regions.FirstOrDefault(x => x.Regionid == model.RegionId);
            Request r = new Request();

            r.Userid = user == null ? null : model.UserId;
            r.Firstname = model.OtherFirstName;
            r.Lastname = model.OtherLastName;
            r.Email = model.Email;
            r.Phonenumber = model.OtherPhoneNumber;
            r.Requesttypeid = model.Requesttypeid;
            r.Status = 1;
            r.Createddate = DateTime.Now;
            r.Relationname = model.OtherRelation;
            r.Isurgentemailsent = new BitArray(new bool[1] { true });
            r.Requestid = model.Requestid;
            int count = _db.Requests.Where(x => x.Createddate.Date == DateTime.Now.Date).Count() + 1;
            string? abbr = _db.Regions.FirstOrDefault(x => x.Regionid == model.RegionId).Abbreviation;
            r.Confirmationnumber = abbr + DateTime.Now.Day.ToString("D2") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Year.ToString().Substring(2, 2) + r.Lastname.Remove(2).ToUpper() + r.Firstname.Remove(2).ToUpper() + count.ToString("D4");
            _db.Requests.Add(r);
            _db.SaveChanges();
            model.Requestid = r.Requestid;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Add Data in Table(rc) (4 requests)
        /// </summary>
        /// <param name="model"></param>
        public void AddToReqClient(PatientReqData model)
        {
            Requestclient rc = new Requestclient();

            rc.Requestid = model.Requestid;
            rc.Email = model.Email;
            rc.Firstname = model.Firstname;
            rc.Lastname = model.Lastname;
            rc.Phonenumber = model.Phonenumber;
            rc.Notes = model.Notes;
            rc.Street = model.Street;
            rc.City = model.City;
            rc.State = model.State;
            rc.Zipcode = model.Zipcode;
            rc.Address = model.Hotelname;
            rc.Location = model.City;
            var dt = model.Birthdate;
            rc.Intdate = dt.Day;
            rc.Strmonth = dt.Month.ToString();
            rc.Intyear = dt.Year;
            rc.Regionid = model.RegionId;
            _db.Requestclients.Add(rc);
            _db.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Uplode file (4 requests)
        /// </summary>
        /// <param name="model"></param>
        public void UploadFile(PatientReqData model)
        {
            string path = _environment.WebRootPath;
            string? filePath = "content/" + model.Filename;
            string fullPath = Path.Combine(path, filePath);

            IFormFile file1 = model.Upload;
            FileStream stream = new FileStream(fullPath, FileMode.Create);

            if (file1 != null && stream != null)
            {
                file1.CopyTo(stream);
            }

            Requestwisefile rwf = new Requestwisefile();

            rwf.Requestid = model.Requestid;
            rwf.Filename = !string.IsNullOrEmpty(model.Filename) ? model.Filename : null;
            rwf.Createddate = DateTime.Now;
            _db.Add(rwf);
            _db.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Add Data in Table(con) (1 request)
        /// </summary>
        /// <param name="model"></param>
        public void AddToConcierge(PatientReqData model)
        {
            Concierge con = new Concierge();

            con.Conciergename = model.OtherFirstName + " " + model.OtherLastName;
            con.Address = model.Hotelname;
            con.Street = model.Street;
            con.State = model.State;
            con.City = model.City;
            con.State = model.State;
            con.Zipcode = model.Zipcode;
            con.Createddate = DateTime.Now;
            con.Regionid = 1;
            _db.Concierges.Add(con);
            _db.SaveChanges();
            model.Conciergeid = con.Conciergeid;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Add Data in Table(rcon) (1 request)
        /// </summary>
        /// <param name="model"></param>
        public void AddToReqConcierge(PatientReqData model)
        {
            Requestconcierge rcon = new Requestconcierge();

            rcon.Conciergeid = model.Conciergeid;
            rcon.Requestid = model.Requestid;
            _db.Requestconcierges.Add(rcon);
            _db.SaveChanges();
            model.ConciergeReqid = rcon.Id;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Add Data in Table(bus) (1 request)
        /// </summary>
        /// <param name="model"></param>
        public void AddToBusiness(PatientReqData model)
        {
            var bus = new Business();
            bus.Name = model.OtherFirstName + " " + model.OtherLastName;
            bus.Createddate = DateTime.Now;
            bus.Regionid = 1;
            bus.Address1 = model.Hotelname;
            _db.Businesses.Add(bus);
            _db.SaveChanges();
            model.Businessid = bus.Businessid;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Add Data in Table(rbus) (1 request)
        /// </summary>
        /// <param name="model"></param>
        public void AddToReqBusiness(PatientReqData model)
        {
            var rbus = new Requestbusiness();
            rbus.Businessid = model.Businessid;
            rbus.Requestid = model.Requestid;
            _db.Requestbusinesses.Add(rbus);
            _db.SaveChanges();
            model.Businessid = rbus.Businessid;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Send Mail for creating Acc(3 requests) or reset passwd(Reset for all)
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task EmailSender(string email, string subject, string message)
        {
            var mail = "tatva.dotnet.takshgadhiya@outlook.com";
            var password = "12!@Taksh";

            var client = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, password)
            };

            var mailMessage = new MailMessage(from: mail, to: email)
            {
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };
            return client.SendMailAsync(mailMessage);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Add dtat in emaillog table (3 requests)
        /// </summary>
        /// <param name="model"></param>
        public void AddToEmailLog(PatientReqData model)
        {
            var emailLog = new Emaillog()
            {
                Subjectname = "Create Patient Account !!!",
                Emailid = model.Email,
                Roleid = 3,
                Requestid = model.Requestid,
                Filepath = "There no file uploaded yet",
                Createdate = DateTime.Now,
                Sentdate = DateTime.Now,
                Isemailsent = new BitArray(1, true),
                Senttries = 1,
                Confirmationnumber = _db.Requests.FirstOrDefault(x => x.Requestid == model.Requestid).Confirmationnumber,
            };
            _db.Emaillogs.Add(emailLog);
            _db.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Uploade File (submit for me/other)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="reqId"></param>
        public void UploadFileByMeAndOther(PatientReqData model, int reqId)
        {
            string path = _environment.WebRootPath;
            string filePath = "content/" + model.Upload.FileName;
            string fullPath = Path.Combine(path, filePath);

            IFormFile file1 = model.Upload;
            FileStream stream = new FileStream(fullPath, FileMode.Create);

            if (file1 != null && stream != null)
            {
                file1.CopyTo(stream);
            }

            var fileName = model.Upload?.FileName;
            var doctType = model.Upload?.ContentType;

            var reqWiseFileData = new Requestwisefile()
            {
                Requestid = reqId,
                Filename = fileName,
                Ip = doctType,
            };
            _db.Add(reqWiseFileData);
            _db.SaveChanges();
        }

        #endregion


        #region Review Agreement

        //***************************************************************************************************************************************************
        /// <summary>
        /// To show Review Agreement
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ReviewAgreementVm GetReviewAgreement(int reqId, int userId)
        {
            if (_db.Requests.Any(i => i.Requestid == reqId && i.Userid == userId))
            {
                var patient = _db.Requestclients.FirstOrDefault(i => i.Requestid == reqId);

                var reviewData = new ReviewAgreementVm()
                {
                    RequestId = reqId,
                    PatientName = patient.Firstname + " " + patient.Lastname,
                };
                return reviewData;
            }
            return null;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// I agree ??????????
        /// </summary>
        /// <param name="reqId"></param>
        public void AgreeReview(int reqId)
        {
            var request = _db.Requests.FirstOrDefault(i => i.Requestid == reqId);

            var agreeData = new Requeststatuslog()
            {
                Requestid = reqId,
                Status = 4,
                Physicianid = request.Physicianid,
                Transtophysicianid = request.Physicianid,
                Notes = "Review is Agreed",
                Createddate = DateTime.Now,
            };
            request.Status = 4;
            _db.Requeststatuslogs.Add(agreeData);
            _db.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Canceled by patient ??????
        /// </summary>
        /// <param name="reviewAgreementCm"></param>
        public void CancelReview(ReviewAgreementVm reviewAgreementVm)
        {
            var request = _db.Requests.FirstOrDefault(i => i.Requestid == reviewAgreementVm.RequestId);

            var cancelData = new Requeststatuslog()
            {
                Requestid = request.Requestid,
                Status = 7,
                Notes = reviewAgreementVm.CancellationNotes,
                Createddate = DateTime.Now,
            };
            request.Status = 7;
            _db.Requeststatuslogs.Add(cancelData);
            _db.SaveChanges();
        }

        #endregion

        //***************************************************************************************************************************************************
        //***************************************************************************************************************************************************
        //***************************************************************************************************************************************************
    }
}