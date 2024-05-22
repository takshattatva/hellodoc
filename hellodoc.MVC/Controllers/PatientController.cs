using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hellodoc.DAL.Models;
using hellodoc.DAL.ViewModels;
using hellodoc.BAL.Interface;
using System.Diagnostics;
using hellodoc.MVC.Auth;
using System.Collections;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using System.Security.Cryptography;

namespace hellodoc.DAL.Controllers
{
    public class PatientController : Controller
    {
        private readonly HellodocDbContext _context;
        private readonly IPatientReqRepo _ipatientreq;
        private readonly IPatientDashRepo _ipatientdash;
        private readonly IRegisterRepo _iregister;
        private readonly IJwtServiceRepo _ijwtservice;


        public PatientController(HellodocDbContext context, IPatientReqRepo ipatientreq, IPatientDashRepo ipatientdash, IRegisterRepo iregister, IJwtServiceRepo ijwtservice)
        {
            _context = context;
            _ipatientreq = ipatientreq;
            _ipatientdash = ipatientdash;
            _iregister = iregister;
            _ijwtservice = ijwtservice;

        }
        
        #region Session X-pire Logout

        //***************************************************************************************************************************************************
        /// <summary>
        /// After concluding session redirecting to login page
        /// </summary>
        /// <returns></returns>
        public string AjaxLogout()
        {
            HttpContext.Session.Clear();
            return "<script>window.location.href = '/Patient'</script>";
        }

        #endregion


        #region Common Pages

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Landing Page
        /// </summary>
        /// <returns></returns>
        public IActionResult patient_site()
        {
            return View();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Submit Request
        /// </summary>
        /// <returns></returns>
        public IActionResult submit_request()
        {
            return View();
        }

        #endregion


        #region Email Check For Request Form

        //***************************************************************************************************************************************************
        /// <summary>
        /// Check email for patient request form only
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> EmailCheck(string email)
        {
            var user = await _context.Aspnetusers.FirstOrDefaultAsync(x => x.Email == email);
            return user != null;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Check Email for family,concierge,business requests  only
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> EmailCheckforOtherReq(string email)
        {
            var user = await _context.Requestclients.FirstOrDefaultAsync(x => x.Email == email);
            return user != null;
        }

        #endregion


        #region Request Forms

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Patient Request Form
        /// </summary>
        /// <returns></returns>
        public IActionResult patient_request_form()
        {
            PatientReqData patientReqData = new PatientReqData();
            patientReqData.Regions = _ipatientreq.GetRegions();

            return View(patientReqData);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Patient Request Form
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult patient_request_form(PatientReqData model)
        {
            if (!_ipatientreq.EmailCheck(model.Email))
            {
                if (model.PasswordHash != model.ConformPassword)
                {
                    return View();
                }
                _ipatientreq.AddToUser(model);
            }
            else
            {
                model.UserId = _ipatientreq.GetUserId(model.Email);
            }
            model.OtherFirstName = model.Firstname;
            model.OtherLastName = model.Lastname;
            model.OtherEmail = model.OtherEmail;
            model.OtherPhoneNumber = model.Phonenumber;
            model.Requesttypeid = 1;
            model.OtherRelation = "Self";

            _ipatientreq.AddToRequest(model);

            _ipatientreq.AddToReqClient(model);

            if (model.Filename != null)
            {
                _ipatientreq.UploadFile(model);
            }
            TempData["success"] = "Request Added";
            return RedirectToAction("submit_request");
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Family Request Form
        /// </summary>
        /// <returns></returns>
        public IActionResult family_request_form()
        {
            PatientReqData patientReqData = new PatientReqData();
            patientReqData.Regions = _ipatientreq.GetRegions();

            return View(patientReqData);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Family Request Form
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult family_request_form(PatientReqData model)
        {
            model.Requesttypeid = 2;
            model.OtherRelation = "Family/Friend";

            _ipatientreq.AddToRequest(model);

            _ipatientreq.AddToReqClient(model);

            if (model.Filename != null)
            {
                _ipatientreq.UploadFile(model);
            }

            var myUser = _context.Aspnetusers.Where(x => x.Email == model.Email).FirstOrDefault();

            if (myUser == null)
            {
                var reciever = model.Email;
                var subject = "Create Patient Account !!!";
                var here = "https://localhost:7052/Patient/patient_create_account?pid=" + _iregister.Encrypt(model.Requestid.ToString());
                var message = $"We trust this message finds you in good spirits.To Create Account click <a href=\"{here}\">here</a>";

                _ipatientreq.AddToEmailLog(model);
                _ipatientreq.EmailSender(reciever, subject, message);
            }
            TempData["success"] = "Request Added";
            return RedirectToAction("patient_site");
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Concierge Request Form
        /// </summary>
        /// <returns></returns>
        public IActionResult concierge_request_form()
        {
            PatientReqData patientReqData = new PatientReqData();
            patientReqData.Regions = _ipatientreq.GetRegions();

            return View(patientReqData);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Concierge Request Form
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult concierge_request_form(PatientReqData model)
        {
            model.Requesttypeid = 3;
            model.OtherRelation = "Concierge";

            _ipatientreq.AddToRequest(model);

            _ipatientreq.AddToReqClient(model);

            _ipatientreq.AddToConcierge(model);

            _ipatientreq.AddToReqConcierge(model);

            var myUser = _context.Aspnetusers.Where(x => x.Email == model.Email).FirstOrDefault();

            if (myUser == null)
            {
                var reciever = model.Email;
                var subject = "Create Patient Account !!!";
                var here = "https://localhost:7052/Patient/patient_create_account?pid=" + _iregister.Encrypt(model.Requestid.ToString());
                var message = $"We trust this message finds you in good spirits.To Create Account click <a href=\"{here}\">here</a>";

                _ipatientreq.AddToEmailLog(model);
                _ipatientreq.EmailSender(reciever, subject, message);
            }
            TempData["success"] = "Request Added";
            return RedirectToAction("patient_site");
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Business Request Form
        /// </summary>
        /// <returns></returns>
        public IActionResult business_request_form()
        {
            PatientReqData patientReqData = new PatientReqData();
            patientReqData.Regions = _ipatientreq.GetRegions();

            return View(patientReqData);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Business Request Form
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult business_request_form(PatientReqData model)
        {
            model.Requesttypeid = 4;
            model.OtherRelation = "Business Partner";

            _ipatientreq.AddToRequest(model);

            _ipatientreq.AddToReqClient(model);

            _ipatientreq.AddToBusiness(model);

            _ipatientreq.AddToReqBusiness(model);

            var myUser = _context.Aspnetusers.Where(x => x.Email == model.Email).FirstOrDefault();

            if (myUser == null)
            {
                var reciever = model.Email;
                var subject = "Create Patient Account !!!";
                var here = "https://localhost:7052/Patient/patient_create_account?pid=" + _iregister.Encrypt(model.Requestid.ToString());
                var message = $"We trust this message finds you in good spirits.To Create Account click <a href=\"{here}\">here</a>";

                _ipatientreq.AddToEmailLog(model);
                _ipatientreq.EmailSender(reciever, subject, message);
            }
            TempData["success"] = "Request Added";
            return RedirectToAction("patient_site");
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Submit For Me Page
        /// </summary>
        /// <returns></returns>
        [CustomAuthorize("Patient")]
        public IActionResult submit_for_me()
        {
            int userId = (int)(HttpContext.Session.GetInt32("Userid"));
            var users = _context.Users.FirstOrDefault(i => i.Userid == userId);

            var BirthDay = Convert.ToInt32(users.Intdate);
            var BirthMonth = Convert.ToInt32(users.Strmonth);
            var BirthYear = Convert.ToInt32(users.Intyear);

            PatientReqData model = new PatientReqData();
            model.Firstname = users.Firstname;
            model.Lastname = users.Lastname;
            model.Email = users.Email;
            model.Phonenumber = users.Mobile;
            model.Birthdate = new DateTime(BirthYear, BirthMonth, BirthDay);
            model.Regions = _ipatientreq.GetRegions();

            return View(model);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Submit For Me
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult submit_for_me(PatientReqData model)
        {
            if (_ipatientreq.SubmitForMe(model))
            {
                TempData["success"] = "Request Created";
                return RedirectToAction("patient_dashboard");
            }
            else
            {
                TempData["success"] = "Request Not Created";
                return RedirectToAction("patient_dashboard");
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Submit For Other
        /// </summary>
        /// <returns></returns>
        [CustomAuthorize("Patient")]
        public IActionResult submit_for_other()
        {
            PatientReqData patientReqData = new PatientReqData();
            patientReqData.Regions = _ipatientreq.GetRegions();

            return View(patientReqData);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Submit For Other
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult submit_for_other(PatientReqData model)
        {
            if (_ipatientreq.SubmitForOther(model))
            {
                TempData["success"] = "Request Created and Mail Sent";
                return RedirectToAction("patient_dashboard");
            }
            else
            {
                TempData["success"] = "Request Not Created";
                return RedirectToAction("patient_dashboard");
            }
        }

        #endregion


        #region Patient Create Acc

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Patient Create Account
        /// </summary>
        /// <returns></returns>
        public IActionResult patient_create_account(string pid)
        {
            int requestId = Int32.Parse(_iregister.Decrypt(pid.ToString()));
            var user = _context.Requests.FirstOrDefault(x => x.Requestid == requestId);
            if (pid == 0.ToString() || user == null || user.Userid != null)
            {
                return RedirectToAction("Accessdenied");
            }
            else
            {
                RegisterVm registervm = new RegisterVm();
                registervm.RequestId = requestId;
                return View(registervm);
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Patient Create Account
        /// </summary>
        /// <param name="registervm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult patient_create_account(RegisterVm registervm, int pid)
        {
            if (registervm.Email != _context.Requestclients.FirstOrDefault(x => x.Requestid == pid).Email)
            {
                TempData["error"] = "Enter valid Email!!!";
                return View(registervm);
            }
            else
            {
                if (registervm.Password == registervm.ConfirmPassword)
                {
                    _iregister.RegisterUser(registervm);
                    TempData["success"] = "Account Created Successfully";
                    return RedirectToAction("patient_site");
                }
                else
                {
                    TempData["error"] = "Password Not Matched"; ;
                    return View();
                }
            }
        }

        #endregion


        #region Login

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Login Page
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Login Page
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Index(Aspnetuser user)
        {
            if (user.Email == null || user.Passwordhash == null || user == null)
            {
                TempData["error"] = "Enter Email or Password first!";
                return View();
            }
            if (_context.Aspnetusers.FirstOrDefault(x => x.Email == user.Email) == null)
            {
                TempData["error"] = "Email not Regisered!!!";
                return View();
            }
            var passwd = _context.Aspnetusers.FirstOrDefault(x => x.Email == user.Email).Passwordhash;
            bool verify = BCrypt.Net.BCrypt.Verify(user.Passwordhash, passwd);
            var myuser = _context.Aspnetusers.FirstOrDefault(x => x.Email == user.Email && verify);

            var us = _context.Users.FirstOrDefault(x => x.Email.Trim() == user.Email);

            int? userid = us == null ? null : us.Userid;
            var req = _context.Requests.FirstOrDefault(x => x.Userid == userid & x.Confirmationnumber != null);

            var admin = _context.Admins.FirstOrDefault(x => x.Email == user.Email);
            var physician = _context.Physicians.FirstOrDefault(x => x.Email == user.Email);

            if (physician != null)
            {
                if (physician.Status != 1 || physician.Isdeleted != null)
                {
                    TempData["error"] = "Your Account Not Active Or deleted by Admin";
                    return View();
                }
            }

            if (myuser != null)
            {
                HttpContext.Session.SetString("UserSession", myuser.Email);
                HttpContext.Session.SetString("aspNetUserId", myuser.Id);
                if (us != null)
                {
                    HttpContext.Session.SetInt32("Userid", us.Userid);
                    HttpContext.Session.SetInt32("_sessionUserId", us.Userid);
                    HttpContext.Session.SetString("_sessionUserName", us.Firstname + " " + us.Lastname);
                }
                if (req != null)
                {
                    HttpContext.Session.SetString("_sessionConfirmationNumber", req.Confirmationnumber);
                }
                if (admin != null)
                {
                    HttpContext.Session.SetString("_AdminName", admin.Firstname + " " + admin.Lastname);
                }
                if (physician != null)
                {
                    HttpContext.Session.SetString("_PhysicianName", physician.Firstname + " " + physician.Lastname);
                }

                TempData["success"] = "Login Successfull!!";

                Aspnetuser aspnetuser = _iregister.GetUserRole(user.Email);
                string token = _ijwtservice.GenerateJwtToken(aspnetuser);
                HttpContext.Session.SetString("token", token);

                if (aspnetuser.Aspnetuserrole.Roleid == "1")
                {
                    return RedirectToAction("admin_dashboard", "Admin");
                }
                else if (aspnetuser.Aspnetuserrole.Roleid == "3")
                {
                    return RedirectToAction("patient_dashboard");
                }
                else if (aspnetuser.Aspnetuserrole.Roleid == "2")
                {
                    return RedirectToAction("ProviderDashboard", "Provider");
                }
            }
            else
            {
                TempData["error"] = "Invalid Credentials!";
            }
            return View();
        }

        #endregion


        #region Forgot Passwd

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Patient Forgot Password
        /// </summary>
        /// <returns></returns>
        public IActionResult patient_forgot_passwd()
        {
            return View();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Patient Forgot Password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult patient_forgot_passwd(PatientReqData model)
        {
            var myUser = _context.Aspnetusers.Where(x => x.Email == model.Email).FirstOrDefault();

            if (myUser != null)
            {
                var anur = _context.Aspnetuserroles.FirstOrDefault(x => x.Userid == myUser.Id);
                var reciever = model.Email;
                var subject = "Reset Account Password !!!";
                var here = "https://localhost:7052/Patient/patient_reset_passwd";
                var otp = GenerateOTP(model.Email);
                var message = $"We trust this message finds you in good spirits.To Reset your Account Password click <a href=\"{here}\">here</a><p>Security Code : {otp}";

                var emailLog = new Emaillog()
                {
                    Subjectname = subject,
                    Emailid = reciever,
                    Roleid = int.Parse(anur.Roleid),
                    Filepath = "There no file uploaded yet",
                    Createdate = DateTime.Now,
                    Sentdate = DateTime.Now,
                    Isemailsent = new BitArray(1, true),
                    Senttries = 1,
                };
                _context.Emaillogs.Add(emailLog);
                _context.SaveChanges();

                _ipatientreq.EmailSender(reciever, subject, message);

                TempData["success"] = "Email Sent";
                return RedirectToAction("patient_reset_passwd");
            }
            else
            {
                TempData["error"] = "Email isn't Register";
            }
            return View();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Generate Otp for reset passwd
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private string GenerateOTP(string email)
        {
            const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@#$%^&*()";
            var random = new Random();
            var otp = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());

            var aspnetuser = _context.Aspnetusers.FirstOrDefault(x => x.Email == email);
            aspnetuser.Otp = otp;
            _context.SaveChanges();

            return otp;
        }

        #endregion


        #region Reset Passwd

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Patient Reset Password
        /// </summary>
        /// <returns></returns>
        public IActionResult patient_reset_passwd()
        {
            return View();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Check Otp for reset passwd
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult OtpCheck(string email, string otp)
        {
            var aspnetuser = _context.Aspnetusers.FirstOrDefault(x => x.Email == email);
            if (aspnetuser == null)
            {
                return Json(new { Success = false });
            }
            if (aspnetuser.Otp == null)
            {
                return Json(new { Success = false });
            }
            else
            {
                if (aspnetuser.Otp == otp)
                {
                    return Json(new { Success = true });
                }
                else
                {
                    return Json(new { Success = false });
                }
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Patient Reset Password
        /// </summary>
        /// <param name="registervm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult patient_reset_passwd(RegisterVm registervm)
        {
            Aspnetuser? aspnetuser = _context.Aspnetusers.FirstOrDefault(x => x.Email == registervm.Email);
            if (aspnetuser == null)
            {
                TempData["error"] = "Email Note Registered";
            }
            else
            {
                if (registervm.Password == registervm.ConfirmPassword)
                {
                    _iregister.ResetPassword(registervm);

                    TempData["success"] = "Password Updated";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["error"] = "Please enter same password";
                }
            }
            return View();
        }

        #endregion


        #region Patient Dashboard

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Patient Dashboard
        /// </summary>
        /// <returns></returns>
        [CustomAuthorize("Patient")]
        public IActionResult patient_dashboard()
        {
            int userId = (int)(HttpContext.Session.GetInt32("Userid"));

            PatientDashData patientDashData = new PatientDashData();
            patientDashData.dashboardData = _ipatientdash.RequestList(userId);
            patientDashData.profileData = _ipatientdash.GetProfileData(userId);

            return View(patientDashData);
        }

        #endregion


        #region Patient Profile

        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Patient Profile Data
        /// </summary>
        /// <param name="patientDashData"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult patient_dashboard(PatientDashData patientDashData)
        {
            int userId = (int)HttpContext.Session.GetInt32("_sessionUserId");

            _ipatientdash.SetProfileData(patientDashData.profileData, userId);

            User? user = _context.Users.FirstOrDefault(i => i.Userid == userId);
            HttpContext.Session.SetString("_sessionUserName", user.Firstname + " " + user.Lastname);

            TempData["success"] = "Patient Details Updated";
            return RedirectToAction("patient_dashboard");
        }

        #endregion


        #region Session Clear

        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Logout
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            TempData["success"] = "User has been Logged out";
            return RedirectToAction("Index", "Patient");
        }

        #endregion


        #region Patient View Doc

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Patient View Docs 
        /// </summary>
        /// <param name="rid"></param>
        /// <returns></returns>
        [CustomAuthorize("Patient")]
        public IActionResult view_doc(int rid)
        {
            HttpContext.Session.SetInt32("_sessionReqId", rid);
            int userId = (int)(HttpContext.Session.GetInt32("_sessionUserId"));

            PatientDashData patientDashData = new PatientDashData();
            patientDashData.documentData = _ipatientdash.DocumentList(rid);
            patientDashData.profileData = _ipatientdash.GetProfileData(userId);

            return View(patientDashData);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Patient View Docs
        /// </summary>
        /// <param name="patientDashData"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult view_doc(PatientDashData patientDashData)
        {
            int reqId = (int)(HttpContext.Session.GetInt32("_sessionReqId"));

            if (_ipatientdash.DashboardUpload(patientDashData, reqId))
            {
                patientDashData.documentData = _ipatientdash.DocumentList(reqId);

                return View(patientDashData);
            }
            return View();
        }

        #endregion


        #region Review Agreement

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Review Agreement
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        [CustomAuthorize("Patient", "Admin", "Physician")]
        public IActionResult review_agreement(string pid)
        {
            int userId = (int)(HttpContext.Session.GetInt32("Userid"));
            int requestId = Int32.Parse(_iregister.Decrypt(pid.ToString()));

            ReviewAgreementVm reviewAgreementVm = new ReviewAgreementVm();
            reviewAgreementVm = _ipatientreq.GetReviewAgreement(requestId, userId);

            if (reviewAgreementVm != null)
            {
                return View(reviewAgreementVm);
            }
            return RedirectToAction("Accessdenied", "Patient");
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update (AGREE) Review Agreement
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        [CustomAuthorize("Patient")]
        public IActionResult Review_Agree(int pid)
        {
            _ipatientreq.AgreeReview(pid);

            TempData["success"] = "Agreement Agreed!!!";
            return RedirectToAction("Index", "Patient");
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Update (DISAGREE) Review Agreement
        /// </summary>
        /// <param name="reviewAgreementVm"></param>
        /// <returns></returns>
        [CustomAuthorize("Patient")]
        [HttpPost]
        public IActionResult Review_Cancel(ReviewAgreementVm reviewAgreementVm)
        {
            _ipatientreq.CancelReview(reviewAgreementVm);

            TempData["success"] = "Agreement Cancelled!!!";
            return RedirectToAction("Index", "Patient");
        }

        #endregion


        #region AccessDenied

        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Access Denied
        /// </summary>
        /// <returns></returns>
        public IActionResult Accessdenied()
        {
            return View();
        }

        #endregion

        //***************************************************************************************************************************************************
        //***************************************************************************************************************************************************
        //***************************************************************************************************************************************************
        #region Home

        [CustomAuthorize("Patient")]
        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion
    }
}