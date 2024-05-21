using hellodoc.DAL.Models;
using hellodoc.DAL.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Collections;
using File = System.IO.File;
using hellodoc.BAL.Interface;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;


namespace hellodoc.BAL.Repository
{
    public class AdminDashboard : IAdminDashboard
    {
        private readonly HellodocDbContext _context;
        private readonly IRegisterRepo _iregisterRepo;

        public AdminDashboard(HellodocDbContext context, IRegisterRepo registerRepo)
        {
            _context = context;
            _iregisterRepo = registerRepo;
        }

        #region Admin Dashboard

        //***************************************************************************************************************************************************
        /// <summary>
        /// Get Count data based on status & Get Dashboard data fetch (updated count)
        /// </summary>
        /// <returns></returns>
        public CountRequest GetCountRequest()
        {
            var request = _context.Requests;

            CountRequest countData = new CountRequest();
            countData.NewRequest = request.Where(i => i.Status == 1).Count();
            countData.PendingRequest = request.Where(i => i.Status == 2).Count();
            countData.ActiveRequest = request.Where(i => i.Status == 4 || i.Status == 5).Count();
            countData.ConcludeRequest = request.Where(i => i.Status == 6).Count();
            countData.ToCloseRequest = request.Where(i => i.Status == 7 || i.Status == 8 || i.Status == 3).Count();
            countData.UnpaidRequest = request.Where(i => i.Status == 9).Count();

            return countData;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Table Data Fetch Records
        /// </summary>
        /// <param name="Status"></param>
        /// <param name="requesttypeid"></param>
        /// <param name="regionid"></param>
        /// <returns></returns>
        public List<RequestListAdminDash> getRequestData(int[] Status, string requesttypeid, int regionid)
        {
            var requestList = _context.Requests.Where(i => Status.Contains(i.Status));

            if (requesttypeid != null)
            {
                requestList = requestList.Where((i => i.Requesttypeid.ToString() == requesttypeid));
            }

            if (regionid != 0)
            {
                requestList = requestList.Where(i => i.Requestclients.Select(rc => rc.Regionid.ToString()).Contains(regionid.ToString()));
            }

            var GetRequestData = requestList.Select(r => new RequestListAdminDash()
            {
                Name = r.Requestclients.Select(x => x.Firstname).First() + " " + r.Requestclients.Select(x => x.Lastname).First(),
                Requestor = r.Firstname + " " + r.Lastname,
                RequestDate = r.Createddate.ToString("MMM") + " " + r.Createddate.Day + ", " + r.Createddate.Year, // date of request
                totalHours = (int)(DateTime.Now - r.Createddate).TotalMinutes / 60,
                totalMinutes = (int)(DateTime.Now - r.Createddate).TotalMinutes % 60,
                totalSeconds = (int)(DateTime.Now - r.Createddate).TotalSeconds % 60,
                Phone = r.Requestclients.Select(x => x.Phonenumber).First(),
                Address = r.Requestclients.Select(x => x.Street).First() + ", " + r.Requestclients.Select(x => x.City).First() + ", " + r.Requestclients.Select(x => x.State).First() + " - " + r.Requestclients.Select(x => x.Zipcode).First(),
                Notes = r.Requeststatuslogs.Where(x => x.Transtophysicianid != null && x.Physicianid != x.Transtophysicianid).First() == null ? "-" : "Admin transferred to " + r.Requeststatuslogs.Where(x => x.Transtophysicianid != null).First().Physician.Firstname + " on " + r.Requeststatuslogs.Where(x => x.Transtophysicianid != null).First().Createddate.ToShortDateString() + " at " + r.Requeststatuslogs.Where(x => x.Transtophysicianid != null).First().Createddate.ToShortTimeString() + " : " + r.Requeststatuslogs.Where(x => x.Transtophysicianid != null).First().Notes,
                Status = r.Status,
                ChatWith = r.Physicianid.ToString(),
                Physician = r.Physician.Firstname + " " + r.Physician.Lastname,
                DateOfBirth = r.Requestclients.Select(x => new DateTime((int)x.Intyear, int.Parse(x.Strmonth), (int)x.Intdate)).FirstOrDefault(),
                RequestTypeId = r.Requesttypeid,
                Email = r.Requestclients.Select(x => x.Email).First(),
                RequestId = r.Requestid,
                Region = r.Requestclients.Select(x => x.State) == null ? "-" : r.Requestclients.Select(x => x.State).FirstOrDefault(),
                Mobile = r.Phonenumber == null ? null : r.Phonenumber,
                isFinalized = r.Encounters.Select(x => x.IsFinalized).First() == new BitArray(1, true),
                PhysicianId = r.Physician.Physicianid,
                PhyAspId = r.Physician.Aspnetuserid,
                PatientAspId = r.User.Aspnetuserid,
                UserId = r.Userid,

            }).ToList();
            return GetRequestData;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// View Case Fetch Records
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public AdminViewCaseData getViewCaseData(int requestid)
        {
            var casedata = _context.Requestclients.FirstOrDefault(i => i.Requestid == requestid);
            var requestList = _context.Requests.Where(i => i.Requestid == requestid);

            var BirthDay = Convert.ToInt32(casedata.Intdate);
            var BirthMonth = Convert.ToInt32(casedata.Strmonth);
            var BirthYear = Convert.ToInt32(casedata.Intyear);

            AdminViewCaseData AdminViewCaseModel = new AdminViewCaseData();
            AdminViewCaseModel.ConfirmationNumber = requestList.Select(X => X.Confirmationnumber).First();
            AdminViewCaseModel.Symptoms = casedata.Notes;
            AdminViewCaseModel.FirstName = casedata.Firstname;
            AdminViewCaseModel.LastName = casedata.Lastname;
            AdminViewCaseModel.Mobile = casedata.Phonenumber;
            AdminViewCaseModel.Email = casedata.Email;
            AdminViewCaseModel.Region = casedata.Regionid.ToString();
            AdminViewCaseModel.BusinessAddress = casedata.Street + ", " + casedata.City + ", " + casedata.State;
            AdminViewCaseModel.Room = casedata.Address;
            AdminViewCaseModel.RequestTypeId = requestList.Select(x => x.Requesttypeid).First();
            AdminViewCaseModel.DateOfBirth = new DateTime(BirthYear, BirthMonth, BirthDay);
            AdminViewCaseModel.RequestId = requestid;
            AdminViewCaseModel.UserId = _context.Requests.FirstOrDefault(x => x.Requestid == requestid && x.Confirmationnumber == AdminViewCaseModel.ConfirmationNumber).Userid;

            return AdminViewCaseModel;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// View Case Update Records
        /// </summary>
        /// <param name="updatedViewCaseData"></param>
        /// <param name="requestid"></param>
        public void setViewCaseData(AdminViewCaseData updatedViewCaseData, int requestid)
        {
            var casedata = _context.Requestclients.FirstOrDefault(x => x.Requestid == requestid);

            if (casedata != null)
            {
                casedata.Email = updatedViewCaseData.Email;
                casedata.Phonenumber = updatedViewCaseData.Mobile;
            }
            _context.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Get Region Table Fetch Records 
        /// </summary>
        /// <returns></returns>
        public List<Region> GetRegions()
        {
            var regions = _context.Regions.ToList();
            return regions;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Get Regions for Provider Schedulling
        /// </summary>
        /// <param name="physicianId"></param>
        /// <returns></returns>
        public List<Region> GetPhysicianRegions(int physicianId)
        {
            var regions = _context.Regions.Include(p => p.Physicianregions).Where(p => p.Physicianregions.Any(x => x.Physicianid == physicianId)).ToList();
            return regions;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// View Notes Fetch Records
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public ViewNotesData GetViewNotesData(int requestid)
        {
            var notedata = _context.Requestnotes.FirstOrDefault(i => i.Requestid == requestid);
            var transferdata = _context.Requeststatuslogs.Where(i => i.Requestid == requestid && i.Status == 1).Include(i => i.Physician).ToList();

            if (notedata != null)
            {
                ViewNotesData ViewNotesModel = new ViewNotesData();
                ViewNotesModel.RequestId = requestid;
                ViewNotesModel.AdminNotes = notedata.Adminnotes;
                ViewNotesModel.PhysicianNotes = notedata.Physiciannotes;
                ViewNotesModel.TransferNotes = transferdata == null ? null : transferdata;

                return ViewNotesModel;
            }
            else
            {
                ViewNotesData ViewNotesModel = new ViewNotesData();
                ViewNotesModel.RequestId = requestid;
                ViewNotesModel.AdminNotes = null;
                ViewNotesModel.PhysicianNotes = null;
                ViewNotesModel.TransferNotes = transferdata == null ? null : transferdata;

                return ViewNotesModel;
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// View Notes Update Records
        /// </summary>
        /// <param name="updatedViewNotesData"></param>
        /// <param name="requestid"></param>
        public void setViewNotesData(ViewNotesData updatedViewNotesData, int requestid)
        {
            var notedata = _context.Requestnotes.FirstOrDefault(i => i.Requestid == requestid);
            if (updatedViewNotesData.callId == 1 || updatedViewNotesData.callId == 2)
            {
                if (notedata != null)
                {
                    notedata.Requestid = requestid;
                    notedata.Adminnotes = updatedViewNotesData.AdminNotes;
                    notedata.Modifieddate = DateTime.Now;
                    notedata.Modifiedby = "Admin";
                    _context.SaveChanges();
                }
                else
                {
                    var newnoteData = new Requestnote()
                    {
                        Requestid = requestid,
                        Adminnotes = updatedViewNotesData.AdminNotes,
                        Createdby = "Admin",
                        Createddate = DateTime.Now,
                    };
                    _context.Requestnotes.Add(newnoteData);
                    _context.SaveChanges();
                }
            }
            if (updatedViewNotesData.callId == 3 || updatedViewNotesData.callId == 4)
            {
                if (notedata != null)
                {
                    notedata.Requestid = requestid;
                    notedata.Physiciannotes = updatedViewNotesData.PhysicianNotes;
                    notedata.Modifieddate = DateTime.Now;
                    notedata.Modifiedby = "Physician";
                    _context.SaveChanges();
                }
                else
                {
                    var newnoteData = new Requestnote()
                    {
                        Requestid = requestid,
                        Physiciannotes = updatedViewNotesData.PhysicianNotes,
                        Createdby = "Physician",
                        Createddate = DateTime.Now,
                    };
                    _context.Requestnotes.Add(newnoteData);
                    _context.SaveChanges();
                }
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Get CaseTag Table Fetch Records
        /// </summary>
        /// <returns></returns>
        public List<Casetag> GetCasetags()
        {
            var reasons = _context.Casetags.ToList();
            return reasons;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Cancel Case Fetch Records 
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public CancelCaseModal getCancelCaseData(int requestid)
        {
            var cancelCase = _context.Requestclients.FirstOrDefault(i => i.Requestid == requestid);
            var status = _context.Requests.FirstOrDefault(i => i.Requestid == requestid);

            CancelCaseModal cancelData = new CancelCaseModal();
            cancelData.RequestId = requestid;
            cancelData.Name = cancelCase.Firstname + " " + cancelCase.Lastname;

            return cancelData;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Cancel Case Update Records  
        /// </summary>
        /// <param name="updatedCancelCaseData"></param>
        public void setCancelCaseData(CancelCaseModal updatedCancelCaseData)
        {
            var requestdata = _context.Requests.FirstOrDefault(i => i.Requestid == updatedCancelCaseData.RequestId);

            var addCancelData = new Requeststatuslog()
            {
                Requestid = requestdata.Requestid,
                Notes = updatedCancelCaseData.CancellationNotes,
                Status = 3,
                Createddate = DateTime.Now,
            };
            requestdata.Status = 3;
            requestdata.Casetag = updatedCancelCaseData.CasetagId.ToString();
            _context.Requeststatuslogs.Add(addCancelData);
            _context.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Get Physician List for Assign Case & Transfer Case where RegionId = 0 is given already 
        /// </summary>
        /// <param name="regionid"></param>
        /// <returns></returns>
        public List<Physician> GetPhysicians(int regionid)
        {
            var physicians = _context.Physicians.Where(i => i.Regionid == regionid && i.Isdeleted == null).ToList();
            return physicians;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Get Physician List For Assign & Transfer Case As par selected Region
        /// </summary>
        /// <param name="regionid"></param>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public List<Physician> GetPhysiciansForAssign(int regionid, int requestid, string aspId)
        {
            if (requestid == 0)
            {
                var physician = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == aspId);
                if (physician != null)
                {
                    var physicians = _context.Physicians.Include(x => x.Physicianregions).ToList();
                    var phyregion = _context.Physicianregions.Include(x => x.Physician).Where(x => x.Physicianid == physician.Physicianid).ToList();
                    if (regionid != 0)
                    {
                        physicians = phyregion.Where(x => x.Regionid == regionid).Select(x => x.Physician).ToList();
                    }
                    List<Physician> result = new List<Physician>();
                    foreach (var phy in physicians)
                    {
                        Physician model = new Physician();
                        model.Firstname = phy.Firstname;
                        model.Lastname = phy.Lastname;
                        model.Physicianid = phy.Physicianid;
                        result.Add(model);
                    }
                    return result;
                }
                else
                {
                    var physicians = _context.Physicians.Include(x => x.Physicianregions).ToList();
                    var phyregion = _context.Physicianregions.Include(x => x.Physician).ToList();
                    if (regionid != 0)
                    {
                        physicians = phyregion.Where(x => x.Regionid == regionid).Select(x => x.Physician).ToList();
                    }
                    List<Physician> result = new List<Physician>();
                    foreach (var phy in physicians)
                    {
                        Physician model = new Physician();
                        model.Firstname = phy.Firstname;
                        model.Lastname = phy.Lastname;
                        model.Physicianid = phy.Physicianid;
                        result.Add(model);
                    }
                    return result;
                }
            }
            else
            {
                var request = _context.Requests.FirstOrDefault(x => x.Requestid == requestid);
                var physician = _context.Physicians.Include(x => x.Physicianregions).Where(x => x.Physicianid != request.Physicianid).ToList();
                var phyregion = _context.Physicianregions.Include(x => x.Physician).ToList();
                if (regionid != 0)
                {
                    physician = phyregion.Where(x => x.Regionid == regionid).Select(x => x.Physician).Where(x => x.Physicianid != request.Physicianid).ToList();
                }
                List<Physician> result = new List<Physician>();
                foreach (var phy in physician)
                {
                    Physician model = new Physician();
                    model.Firstname = phy.Firstname;
                    model.Lastname = phy.Lastname;
                    model.Physicianid = phy.Physicianid;
                    result.Add(model);
                }
                return result;
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Assign Case Fetch Records
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public AssignCaseModal GetAssignCaseData(int requestid)
        {
            AssignCaseModal assignData = new AssignCaseModal();
            assignData.RequestId = requestid;

            return assignData;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Assign Case Update Records
        /// </summary>
        /// <param name="updatedAssignCaseModal"></param>
        public void SetAssignCaseData(AssignCaseModal updatedAssignCaseModal)
        {
            var request = _context.Requests.FirstOrDefault(i => i.Requestid == updatedAssignCaseModal.RequestId);

            var addAssignData = new Requeststatuslog()
            {
                Requestid = request.Requestid,
                Physicianid = updatedAssignCaseModal.PhysicianId,
                Status = 1,
                Notes = updatedAssignCaseModal.AssignNotes,
                Createddate = DateTime.Now,
            };
            request.Physicianid = updatedAssignCaseModal.PhysicianId;
            request.Status = 1;
            _context.Requeststatuslogs.Add(addAssignData);
            _context.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Block Case Fetch Records
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public BlockCaseModal GetBlockCaseModal(int requestid)
        {
            var blockCase = _context.Requests.FirstOrDefault(i => i.Requestid == requestid);

            BlockCaseModal blockData = new BlockCaseModal();
            blockData.RequestId = requestid;
            blockData.Name = blockCase.Firstname + " " + blockCase.Lastname;

            return blockData;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Block Case Update Records
        /// </summary>
        /// <param name="updatedBlockCaseData"></param>
        public void SetBlockCaseData(BlockCaseModal updatedBlockCaseData)
        {
            var request = _context.Requests.FirstOrDefault(i => i.Requestid == updatedBlockCaseData.RequestId);
            var requestClient = _context.Requestclients.FirstOrDefault(i => i.Requestid == updatedBlockCaseData.RequestId);

            var blockData = new Blockrequest()
            {
                Phonenumber = requestClient.Phonenumber,
                Email = requestClient.Email,
                Reason = updatedBlockCaseData.BlockReason,
                Requestid = request.Requestid,
                Createddate = DateTime.Now,
                Modifieddate = DateTime.Now,
                Isactive = new BitArray(1, true),
            };
            request.Status = 11;
            _context.Blockrequests.Add(blockData);
            _context.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// View Docs (document data) Fetch Records
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public AdminViewDocumentsVm GetViewDocumentsData(int requestid)
        {
            var patient = _context.Requestclients.FirstOrDefault(i => i.Requestid == requestid);
            var request = _context.Requests.FirstOrDefault(i => i.Requestid == requestid);

            var documentData = new AdminViewDocumentsVm()
            {
                requestId = requestid,
                patientName = patient == null ? null : patient.Firstname + " " + patient.Lastname,
                ConfirmationNumber = request == null ? null : request.Confirmationnumber,
                UserId = request == null ? null : (int)request.Userid,
            };
            return documentData;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// View Docs (document list) Fetch Records
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public List<ViewDocuments> GetViewDocumentsList(int requestid)
        {
            var document = _context.Requestwisefiles.Where(i => i.Requestid == requestid && i.Isdeleted == null);

            var viewDocuments = document.Select(r => new ViewDocuments()
            {
                requestWiseFileId = r.Requestwisefileid,
                requestId = requestid,
                documentName = r.Filename,
                uploadDate = r.Createddate,

            }).ToList();
            return viewDocuments;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// View Docs (Upload) Update Records
        /// </summary>
        /// <param name="adminViewDocumentsVm"></param>
        public void SetViewDocumentData(AdminViewDocumentsVm adminViewDocumentsVm)
        {
            IFormFile File1 = adminViewDocumentsVm.document;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content", File1.FileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                File1.CopyTo(fileStream);
            }

            var fileName = adminViewDocumentsVm.document?.FileName;

            var fileData = new Requestwisefile()
            {
                Requestid = adminViewDocumentsVm.requestId,
                Filename = fileName,
            };
            _context.Add(fileData);
            _context.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// View Docs (Delete) Update Records
        /// </summary>
        /// <param name="requestwisefileid"></param>
        public void DeleteFileData(int requestwisefileid)
        {
            var file = _context.Requestwisefiles.FirstOrDefault(i => i.Requestwisefileid == requestwisefileid);

            if (file != null)
            {
                file.Isdeleted = new BitArray(1, true);
            }
            _context.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// View Docs (Send Mail with File) Update Records
        /// </summary>
        /// <param name="requestwisefileid"></param>
        /// <param name="requestid"></param>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public async Task SendEmailWithFile(int[] requestwisefileid, int requestid, int senderId)
        {
            var mail = "tatva.dotnet.takshgadhiya@outlook.com";
            var password = "12!@Taksh";
            var email = _context.Requestclients.FirstOrDefault(i => i.Requestid == requestid).Email;
            var subject = "View Document";
            var message = "We trust this message finds you in good spirits. Please find the attached documents, In case of any confusions or queries, reach out to us";

            SmtpClient smtpClient = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mail, password)
            };

            MailMessage mailMessage = new MailMessage(from: mail, to: email, subject, message);

            foreach (var obj in requestwisefileid)
            {
                var file = _context.Requestwisefiles.FirstOrDefault(i => i.Requestwisefileid == obj);
                string filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content", file.Filename);

                mailMessage.Attachments.Add(new Attachment(filepath));
            }

            var emailLog = new Emaillog()
            {
                Filepath = "There are too many files",
                Subjectname = subject,
                Emailid = email,
                Roleid = 3,
                Requestid = requestid,
                Adminid = senderId,
                Createdate = DateTime.Now,
                Sentdate = DateTime.Now,
                Isemailsent = new BitArray(1, true),
                Senttries = 1,
                Confirmationnumber = _context.Requests.FirstOrDefault(x => x.Requestid == requestid).Confirmationnumber,
            };
            _context.Emaillogs.Add(emailLog);
            _context.SaveChanges();

            smtpClient.SendMailAsync(mailMessage);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Get Healthprofessionaltypes table fetch records for Send Order
        /// </summary>
        /// <returns></returns>
        public List<Healthprofessionaltype> getHealthProfessionalTypes()
        {
            var health_professional_types = _context.Healthprofessionaltypes.Where(x => x.Isdeleted == null).ToList();
            return health_professional_types;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Get Healthprofessional table as per selected healthprofessionaltype for send order
        /// </summary>
        /// <param name="health_professional_id"></param>
        /// <returns></returns>
        public List<Healthprofessional> getHealthProfessionals(int health_professional_id)
        {
            var heathProfessionals = _context.Healthprofessionals.Where(i => i.Profession == health_professional_id && i.Isdeleted == null).ToList();
            return heathProfessionals;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Send Order Fetch Records
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public SendOrderModel getOrderData(int requestid)
        {
            SendOrderModel sendorderdata = new SendOrderModel();
            sendorderdata.RequestId = requestid;

            return sendorderdata;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Get Business Records as per selected healthprofessional
        /// </summary>
        /// <param name="vendorid"></param>
        /// <returns></returns>
        public SendOrderModel GetVendordata(int vendorid)
        {
            var vendor = _context.Healthprofessionals.FirstOrDefault(i => i.Vendorid == vendorid);

            var vendordata = new SendOrderModel()
            {
                VendorId = vendorid,
                BusinessContact = vendor.Businesscontact,
                Email = vendor.Email,
                FaxNum = vendor.Faxnumber,
            };
            return vendordata;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Send Order Update Records
        /// </summary>
        /// <param name="sendOrderModel"></param>
        public void setOrderData(SendOrderModel sendOrderModel)
        {
            var request = _context.Requests.FirstOrDefault(i => i.Requestid == sendOrderModel.RequestId);
            var healthdata = _context.Healthprofessionals.FirstOrDefault(i => i.Vendorid == sendOrderModel.VendorId);

            Orderdetail od = new Orderdetail();
            od.Requestid = request.Requestid;
            od.Vendorid = healthdata.Vendorid;
            od.Faxnumber = healthdata.Faxnumber;
            od.Email = healthdata.Email;
            od.Businesscontact = healthdata.Businesscontact;
            od.Prescription = sendOrderModel.Prescription;
            od.Noofrefill = sendOrderModel.Refil;
            od.Createddate = DateTime.Now;
            od.Createdby = sendOrderModel.aspId;
            _context.Orderdetails.Add(od);
            _context.SaveChanges();

            var hpUpdateData = _context.Healthprofessionals.FirstOrDefault(i => i.Vendorid == sendOrderModel.VendorId);

            if (hpUpdateData != null)
            {
                hpUpdateData.Businesscontact = sendOrderModel.BusinessContact;
                hpUpdateData.Email = sendOrderModel.Email;
                hpUpdateData.Faxnumber = sendOrderModel.FaxNum;
                hpUpdateData.Modifieddate = DateTime.Now;
                _context.SaveChanges();
                _context.Update(hpUpdateData);
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Transfer Case Fetch Records
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public TransferCaseModal GetTransferCaseData(int requestid)
        {
            TransferCaseModal transferData = new TransferCaseModal();
            transferData.RequestId = requestid;

            return transferData;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Transfer Case Update Records
        /// </summary>
        /// <param name="updatedTransferCaseModal"></param>
        public void SetTransferCaseData(TransferCaseModal updatedTransferCaseModal)
        {
            var request = _context.Requests.FirstOrDefault(i => i.Requestid == updatedTransferCaseModal.RequestId);

            Requeststatuslog rsl = new Requeststatuslog();
            rsl.Requestid = request.Requestid;
            rsl.Status = 1;
            rsl.Physicianid = request.Physicianid;
            rsl.Transtophysicianid = updatedTransferCaseModal.PhysicianId;
            rsl.Notes = updatedTransferCaseModal.TransferNotes;
            rsl.Createddate = DateTime.Now;
            _context.Requeststatuslogs.Add(rsl);

            request.Physicianid = updatedTransferCaseModal.PhysicianId;
            request.Status = 1;
            _context.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Clear Case Fetch Records
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public ClearCaseModel getClearCaseData(int requestid)
        {
            ClearCaseModel clearCaseData = new ClearCaseModel();
            clearCaseData.RequestId = requestid;

            return clearCaseData;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Clear Case Update Records
        /// </summary>
        /// <param name="updatedClearCaseData"></param>
        public void setClearCaseData(ClearCaseModel updatedClearCaseData)
        {
            var request = _context.Requests.FirstOrDefault(i => i.Requestid == updatedClearCaseData.RequestId);

            Requeststatuslog rsl = new Requeststatuslog();
            rsl.Requestid = updatedClearCaseData.RequestId;
            rsl.Status = request.Status;
            rsl.Createddate = DateTime.Now;
            rsl.Notes = "Case is Cleared";
            _context.Requeststatuslogs.Add(rsl);

            request.Status = 10;
            _context.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Send Agreement Fetch Records
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="reuqesttypeid"></param>
        /// <returns></returns>
        public SendAgreementModal GetSendAgreementModal(int requestid, int reuqesttypeid)
        {
            var request = _context.Requestclients.FirstOrDefault(i => i.Requestid == requestid);

            SendAgreementModal agreementData = new SendAgreementModal();
            agreementData.RequestId = requestid;
            agreementData.RequestTypeId = reuqesttypeid;
            agreementData.Phone = request.Phonenumber;
            agreementData.Email = request.Email;

            return agreementData;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Send Agreement Update Records
        /// </summary>
        /// <param name="sendAgreementModal"></param>
        public void SendAgreementEmail(SendAgreementModal sendAgreementModal)
        {
            var mail = "tatva.dotnet.takshgadhiya@outlook.com";
            var password = "12!@Taksh";
            var email = sendAgreementModal.Email;
            var subject = "Review Agreement";
            var here = "https://localhost:7052/Patient/review_agreement?pid=" + _iregisterRepo.Encrypt(sendAgreementModal.RequestId.ToString());
            var message = $"We trust this message finds you in good spirits.First you have to login to your account and then click <a href=\"{here}\">here</a> to review your agreement";

            SmtpClient smtpClient = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mail, password)
            };

            var emailLog = new Emaillog()
            {
                Subjectname = subject,
                Emailid = email,
                Roleid = 3,
                Requestid = sendAgreementModal.RequestId,
                Adminid = sendAgreementModal.adminid,
                Createdate = DateTime.Now,
                Sentdate = DateTime.Now,
                Isemailsent = new BitArray(1, true),
                Senttries = 1,
                Confirmationnumber = _context.Requests.FirstOrDefault(x => x.Requestid == sendAgreementModal.RequestId).Confirmationnumber,
            };
            _context.Emaillogs.Add(emailLog);
            _context.SaveChanges();

            MailMessage mailMessage = new MailMessage(from: mail, to: email, subject, message);
            mailMessage.IsBodyHtml = true;

            smtpClient.SendMailAsync(mailMessage);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Send Link Update Records
        /// </summary>
        /// <param name="sendLink"></param>
        /// <param name="adminId"></param>
        public void SendSubmitRequestLink(SendLink sendLink, int senderId)
        {
            var mail = "tatva.dotnet.takshgadhiya@outlook.com";
            var password = "12!@Taksh";
            var email = sendLink.Email;
            var subject = "Submit Request Page Link Send By Admin/Physician";
            var link = "https://localhost:7052/Patient/submit_request";
            var message = $"We trust this message finds you in good spirits.Hey <b>{sendLink.FirstName}</b>, <br>" +
                $"Check below your details : <br>" +
                $"Firstname : {sendLink.FirstName} <br>" +
                $"Lastname : {sendLink.LastName} <br>" +
                $"Phonenumber : {sendLink.Phone} <br><br><br>" +
                $"Click <a href=\"{link}\">here</a> to submit request";

            SmtpClient smtpClient = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mail, password)
            };

            MailMessage mailMessage = new MailMessage(from: mail, to: email, subject, message);
            mailMessage.IsBodyHtml = true;

            smtpClient.SendMailAsync(mailMessage);

            var emailLog = new Emaillog()
            {
                Subjectname = subject,
                Emailid = email,
                Roleid = 0,
                Adminid = senderId,
                Createdate = DateTime.Now,
                Sentdate = DateTime.Now,
                Isemailsent = new BitArray(1, true),
                Senttries = 1,
            };
            _context.Emaillogs.Add(emailLog);
            _context.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Create Request By Admin Update Records in Tables(r,el,rc,rn)
        /// </summary>
        /// <param name="adminCreateRequestVm"></param>
        /// <param name="adminId"></param>
        public void SendCreateRequestData(AdminCreateRequestVm adminCreateRequestVm, int senderId)
        {
            int reqTypeId = 2;

            InsertRequestData(adminCreateRequestVm, reqTypeId, senderId);
        }
        public void InsertRequestData(AdminCreateRequestVm adminCreateRequestVm, int reqTypeId, int senderId)
        {
            if (adminCreateRequestVm.callId == 1)
            {
                var admin = _context.Admins.FirstOrDefault(i => i.Adminid == senderId);
                int count = _context.Requests.Where(x => x.Createddate.Date == DateTime.Now.Date).Count() + 1;
                string? abbr = _context.Regions.FirstOrDefault(x => x.Regionid == adminCreateRequestVm.RegionId).Abbreviation;
                User? user = _context.Users.FirstOrDefault(i => i.Email == adminCreateRequestVm.Email);
                var requestData = new Request()
                {
                    Userid = null,
                    Requesttypeid = reqTypeId,
                    Firstname = admin.Firstname,
                    Lastname = admin.Lastname,
                    Email = admin.Email,
                    Phonenumber = admin.Mobile,
                    Status = 1,
                    Createddate = DateTime.Now,
                    Confirmationnumber = abbr + DateTime.Now.Day.ToString("D2") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Year.ToString().Substring(2, 2) + adminCreateRequestVm.LastName.Remove(2).ToUpper() + adminCreateRequestVm.FirstName.Remove(2).ToUpper() + count.ToString("D4"),
                    Relationname = "Admin/Physician referral"
                };
                _context.Requests.Add(requestData);
                _context.SaveChanges();

                adminCreateRequestVm.requestId = requestData.Requestid;
                string aspId = admin.Aspnetuserid;

                Aspnetuser? aspnetuser = _context.Aspnetusers.FirstOrDefault(x => x.Email == adminCreateRequestVm.Email);

                if (aspnetuser == null)
                {
                    var mail = "tatva.dotnet.takshgadhiya@outlook.com";
                    var password = "12!@Taksh";
                    var email = adminCreateRequestVm.Email;
                    var subject = "Create Account";
                    var link = "https://localhost:7052/Patient/patient_create_account?pid=" + adminCreateRequestVm.requestId;
                    var message = $"Hey <b>{adminCreateRequestVm.FirstName}</b>, <br>" +
                        $"We trust this message finds you in good spirits.Your request is created, click <a href=\"{link}\">here</a> to create account and access it...";

                    SmtpClient smtpClient = new SmtpClient("smtp.office365.com", 587)
                    {
                        EnableSsl = true,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(mail, password)
                    };

                    MailMessage mailMessage = new MailMessage(from: mail, to: email, subject, message);
                    mailMessage.IsBodyHtml = true;

                    smtpClient.SendMailAsync(mailMessage);
                }

                var emailLog = new Emaillog()
                {
                    Subjectname = "Create Patient Account !!!",
                    Emailid = adminCreateRequestVm.Email,
                    Roleid = 3,
                    Requestid = adminCreateRequestVm.requestId,
                    Adminid = senderId,
                    Createdate = DateTime.Now,
                    Sentdate = DateTime.Now,
                    Isemailsent = new BitArray(1, true),
                    Senttries = 1,
                    Confirmationnumber = _context.Requests.FirstOrDefault(x => x.Requestid == adminCreateRequestVm.requestId).Confirmationnumber,
                };
                _context.Emaillogs.Add(emailLog);
                _context.SaveChanges();

                InsertRequestClientData(adminCreateRequestVm, adminCreateRequestVm.requestId);

                InsertNotesData(adminCreateRequestVm, adminCreateRequestVm.requestId, aspId);
            }
            if (adminCreateRequestVm.callId == 2)
            {
                var physician = _context.Physicians.FirstOrDefault(i => i.Physicianid == senderId);
                int count = _context.Requests.Where(x => x.Createddate.Date == DateTime.Now.Date).Count() + 1;
                User? user = _context.Users.FirstOrDefault(i => i.Email == adminCreateRequestVm.Email);
                var requestData = new Request()
                {
                    Userid = null,
                    Requesttypeid = reqTypeId,
                    Firstname = physician.Firstname,
                    Lastname = physician.Lastname,
                    Email = physician.Email,
                    Phonenumber = physician.Mobile,
                    Status = 1,
                    Createddate = DateTime.Now,
                    Confirmationnumber = "MD" + DateTime.Now.Day.ToString("D2") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Year.ToString().Substring(2, 2) + adminCreateRequestVm.LastName.Remove(2).ToUpper() + adminCreateRequestVm.FirstName.Remove(2).ToUpper() + count.ToString("D4"),
                    Relationname = "Admin/Physician referral"
                };
                _context.Requests.Add(requestData);
                _context.SaveChanges();

                adminCreateRequestVm.requestId = requestData.Requestid;
                string aspId = physician.Aspnetuserid;

                Aspnetuser? aspnetuser = _context.Aspnetusers.FirstOrDefault(x => x.Email == adminCreateRequestVm.Email);

                if (aspnetuser == null)
                {
                    var mail = "tatva.dotnet.takshgadhiya@outlook.com";
                    var password = "12!@Taksh";
                    var email = adminCreateRequestVm.Email;
                    var subject = "Create Account";
                    var link = "https://localhost:7052/Patient/patient_create_account?pid=" + adminCreateRequestVm.requestId;
                    var message = $"Hey <b>{adminCreateRequestVm.FirstName}</b>, <br>" +
                        $"We trust this message finds you in good spirits.Your request is created, click <a href=\"{link}\">here</a> to create account and access it...";

                    SmtpClient smtpClient = new SmtpClient("smtp.office365.com", 587)
                    {
                        EnableSsl = true,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(mail, password)
                    };

                    MailMessage mailMessage = new MailMessage(from: mail, to: email, subject, message);
                    mailMessage.IsBodyHtml = true;

                    smtpClient.SendMailAsync(mailMessage);
                }

                var emailLog = new Emaillog()
                {
                    Subjectname = "Create Patient Account !!!",
                    Emailid = adminCreateRequestVm.Email,
                    Roleid = 3,
                    Requestid = adminCreateRequestVm.requestId,
                    Adminid = senderId,
                    Createdate = DateTime.Now,
                    Sentdate = DateTime.Now,
                    Isemailsent = new BitArray(1, true),
                    Senttries = 1,
                    Confirmationnumber = _context.Requests.FirstOrDefault(x => x.Requestid == adminCreateRequestVm.requestId).Confirmationnumber,
                };
                _context.Emaillogs.Add(emailLog);
                _context.SaveChanges();

                InsertRequestClientData(adminCreateRequestVm, adminCreateRequestVm.requestId);

                InsertNotesData(adminCreateRequestVm, adminCreateRequestVm.requestId, aspId);
            }
        }
        public void InsertRequestClientData(AdminCreateRequestVm adminCreateRequestVm, int requestId)
        {
            var clientData = new Requestclient()
            {
                Requestid = requestId,
                Firstname = adminCreateRequestVm.FirstName,
                Lastname = adminCreateRequestVm.LastName,
                Email = adminCreateRequestVm.Email,
                Phonenumber = adminCreateRequestVm.PhoneNumber,
                Street = adminCreateRequestVm.Street,
                City = adminCreateRequestVm.City,
                State = adminCreateRequestVm.State,
                Zipcode = adminCreateRequestVm.Zipcode,
                Intyear = adminCreateRequestVm.DateOfBirth?.Year,
                Intdate = adminCreateRequestVm.DateOfBirth?.Day,
                Strmonth = adminCreateRequestVm.DateOfBirth?.Month.ToString(),
                Location = adminCreateRequestVm.City,
                Address = adminCreateRequestVm.Room,
                Notes = "Request Created by Admin/Physician",
                Regionid = adminCreateRequestVm.RegionId,
            };
            _context.Requestclients.Add(clientData);
            _context.SaveChanges();
        }
        public void InsertNotesData(AdminCreateRequestVm adminCreateRequestVm, int requestId, string aspId)
        {
            var notesData = new Requestnote()
            {
                Requestid = requestId,
                Adminnotes = adminCreateRequestVm.AdminNotes,
                Createdby = aspId,
                Createddate = DateTime.Now,
            };
            _context.Requestnotes.Add(notesData);
            _context.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Request Duty Support Update Records
        /// </summary>
        /// <param name="message"></param>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public bool RequestSupportViaEmail(string message, int adminId)
        {
            try
            {
                if (message != null)
                {
                    var currentTime = new TimeOnly(DateTime.Now.Hour, DateTime.Now.Minute);
                    BitArray deletedBit = new BitArray(new[] { false });

                    var offDutyQuery = _context.Physicians
                        .Where(p => !_context.Shiftdetails.Any(sd => sd.Shift.Physicianid == p.Physicianid &&
                                                                       sd.Shiftdate.Date == DateTime.Today &&
                                                                       currentTime >= sd.Starttime &&
                                                                       currentTime <= sd.Endtime &&
                                                                       sd.Isdeleted.Equals(deletedBit)) && 
                                                                       p.Isdeleted == null &&
                                                                       p.Physiciannotifications.FirstOrDefault().Isnotificationstopped == deletedBit).ToList();

                    foreach (var obj in offDutyQuery)
                    {
                        var mail = "tatva.dotnet.takshgadhiya@outlook.com";
                        var password = "12!@Taksh";
                        var email = obj.Email;
                        var subject = "We hope this message finds you in good spirits.Need Support for hellodoc, Please Contact Admin";

                        SmtpClient smtpClient = new SmtpClient("smtp.office365.com", 587)
                        {
                            EnableSsl = true,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(mail, password)
                        };

                        MailMessage mailMessage = new MailMessage(from: mail, to: email, subject, message);
                        smtpClient.SendMailAsync(mailMessage);

                        var emailLog = new Emaillog()
                        {
                            Subjectname = subject,
                            Emailid = obj.Email,
                            Roleid = 2,
                            Adminid = adminId,
                            Createdate = DateTime.Now,
                            Sentdate = DateTime.Now,
                            Isemailsent = new BitArray(1, true),
                            Senttries = 1,
                        };
                        _context.Emaillogs.Add(emailLog);
                        _context.SaveChanges();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Close Case Fetch Records
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public AdminCloseCaseVm GetCloseCaseData(int requestId)
        {
            var client = _context.Requestclients.FirstOrDefault(x => x.Requestid == requestId);
            var BirthDay = Convert.ToInt32(client.Intdate);
            var BirthMonth = Convert.ToInt32(client.Strmonth);
            var BirthYear = Convert.ToInt32(client.Intyear);

            var request = _context.Requests.FirstOrDefault(x => x.Requestid == requestId);

            var physician = _context.Physicians.FirstOrDefault(i => i.Physicianid == request.Physicianid);

            AdminCloseCaseVm caseData = new AdminCloseCaseVm();
            caseData.FirstName = client.Firstname;
            caseData.LastName = client.Lastname;
            caseData.Confirmationnumber = request.Confirmationnumber;
            caseData.BirthDate = new DateTime(BirthYear, BirthMonth, BirthDay);
            caseData.PhoneNumber = client.Phonenumber;
            caseData.Email = client.Email;
            caseData.RequestId = requestId;
            caseData.Address = client.Street + ", " + client.City + ", " + client.State + " - " + client.Zipcode;

            if (physician != null)
            {
                caseData.Physicianname = physician.Firstname + " " + physician.Lastname;
                caseData.Physiciancontact = physician.Mobile;
                caseData.Physicianemail = physician.Email;
            }
            else
            {
                caseData.Physicianname = null;
                caseData.Physiciancontact = null;
                caseData.Physicianemail = null;
            }
            return caseData;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Close Case (docs) Fetch Records
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public List<Documents> GetCloseCaseDocuments(int requestId)
        {
            var files = _context.Requestwisefiles.Where(x => x.Requestid == requestId);

            var documentList = files.Select(x => new Documents()
            {
                requestWiseFileId = x.Requestwisefileid,
                requestId = x.Requestid,
                documentName = x.Filename,
                uploadDate = x.Createddate

            }).ToList();
            return documentList;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Close Case (data changes) Update Records
        /// </summary>
        /// <param name="adminCloseCaseVm"></param>
        public void UpdateCloseCaseData(AdminCloseCaseVm adminCloseCaseVm)
        {
            var client = _context.Requestclients.FirstOrDefault(x => x.Requestid == adminCloseCaseVm.RequestId);

            client.Email = adminCloseCaseVm.Email;
            client.Phonenumber = adminCloseCaseVm.PhoneNumber;
            _context.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Close Case (clsing the case) Update Records
        /// </summary>
        /// <param name="requestId"></param>
        public void SetClosedCase(int requestId)
        {
            var request = _context.Requests.FirstOrDefault(x => x.Requestid == requestId);

            var logData = new Requeststatuslog()
            {
                Requestid = requestId,
                Status = request.Status,
                Notes = "Closed and unpaid",
                Createddate = DateTime.Now,
            };
            request.Status = 9;
            _context.Requeststatuslogs.Add(logData);
            _context.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Admin Encounter Fetch Records
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public EncounterVm GetEncounterData(int requestid, int status)
        {
            var rc = _context.Requestclients.FirstOrDefault(i => i.Requestid == requestid);
            var enc = _context.Encounters.FirstOrDefault(i => i.RequestId == requestid);
            var userId = (int)_context.Requests.FirstOrDefault(i => i.Requestid == requestid).Userid;

            var birthDate = Convert.ToInt32(rc.Intdate);
            var birthMonth = Convert.ToInt32(rc.Strmonth);
            var birthYear = Convert.ToInt32(rc.Intyear);

            if (enc != null)
            {
                var AdminEncounter = new EncounterVm()
                {
                    userId = userId == null ? 0 : userId,
                    statusForName = status,
                    reqid = requestid,
                    FirstName = rc.Firstname,
                    LastName = rc.Lastname,
                    Location = rc.Street + ", " + rc.City + ", " + rc.State + ", " + rc.Zipcode,
                    BirthDate = new DateTime(birthYear, birthMonth, birthDate),
                    Email = rc.Email,
                    PhoneNumber = rc.Phonenumber,
                    Date = enc.Date,
                    HistoryIllness = enc.HistoryIllness,
                    MedicalHistory = enc.MedicalHistory,
                    Medications = enc.Medications,
                    Allergies = enc.Allergies,
                    Temp = enc.Temp,
                    Hr = enc.Hr,
                    Rr = enc.Rr,
                    BpD = enc.BpD,
                    BpS = enc.BpS,
                    O2 = enc.O2,
                    Pain = enc.Pain,
                    Heent = enc.Heent,
                    Cv = enc.Cv,
                    Chest = enc.Chest,
                    Abd = enc.Abd,
                    Extr = enc.Extr,
                    Skin = enc.Skin,
                    Neuro = enc.Neuro,
                    Other = enc.Other,
                    Diagnosis = enc.Diagnosis,
                    TreatmentPlan = enc.TreatmentPlan,
                    MedicationDispensed = enc.MedicationDispensed,
                    Procedures = enc.Procedures,
                    FollowUp = enc.FollowUp,
                };
                return AdminEncounter;
            }
            else
            {
                var AdminEncounter = new EncounterVm()
                {
                    userId = userId == null ? 0 : userId,
                    statusForName = status,
                    reqid = requestid,
                    FirstName = rc.Firstname,
                    LastName = rc.Lastname,
                    Location = rc.Street + ", " + rc.City + ", " + rc.State + ", " + rc.Zipcode,
                    BirthDate = new DateTime(birthYear, birthMonth, birthDate),
                    Email = rc.Email,
                    PhoneNumber = rc.Phonenumber,
                };
                return AdminEncounter;
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Admin Encounter Update Records
        /// </summary>
        /// <param name="encounterVm"></param>
        public void SetEncounterData(EncounterVm encounterVm)
        {
            var enc = _context.Encounters.FirstOrDefault(i => i.RequestId == encounterVm.reqid);

            if (enc != null)
            {
                enc.HistoryIllness = encounterVm.HistoryIllness;
                enc.MedicalHistory = encounterVm.MedicalHistory;
                enc.Medications = encounterVm.Medications;
                enc.Date = DateTime.Now;
                enc.Allergies = encounterVm.Allergies;
                enc.Temp = encounterVm.Temp;
                enc.Hr = encounterVm.Hr;
                enc.Rr = encounterVm.Rr;
                enc.BpD = encounterVm.BpD;
                enc.BpS = encounterVm.BpS;
                enc.O2 = encounterVm.O2;
                enc.Pain = encounterVm.Pain;
                enc.Heent = encounterVm.Heent;
                enc.Cv = encounterVm.Cv;
                enc.Chest = encounterVm.Chest;
                enc.Abd = encounterVm.Abd;
                enc.Extr = encounterVm.Extr;
                enc.Skin = encounterVm.Skin;
                enc.Neuro = encounterVm.Neuro;
                enc.Other = encounterVm.Other;
                enc.Diagnosis = encounterVm.Diagnosis;
                enc.TreatmentPlan = encounterVm.TreatmentPlan;
                enc.MedicationDispensed = encounterVm.MedicationDispensed;
                enc.Procedures = encounterVm.Procedures;
                enc.FollowUp = encounterVm.FollowUp;
                _context.SaveChanges();
            }
            else
            {
                var encounterData = new Encounter()
                {
                    RequestId = encounterVm.reqid,
                    Date = DateTime.Now,
                    HistoryIllness = encounterVm.HistoryIllness,
                    MedicalHistory = encounterVm.MedicalHistory,
                    Medications = encounterVm.Medications,
                    Allergies = encounterVm.Allergies,
                    Temp = encounterVm.Temp,
                    Hr = encounterVm.Hr,
                    Rr = encounterVm.Rr,
                    BpD = encounterVm.BpD,
                    BpS = encounterVm.BpS,
                    O2 = encounterVm.O2,
                    Pain = encounterVm.Pain,
                    Heent = encounterVm.Heent,
                    Cv = encounterVm.Cv,
                    Chest = encounterVm.Chest,
                    Abd = encounterVm.Abd,
                    Extr = encounterVm.Extr,
                    Skin = encounterVm.Skin,
                    Neuro = encounterVm.Neuro,
                    Other = encounterVm.Other,
                    Diagnosis = encounterVm.Diagnosis,
                    TreatmentPlan = encounterVm.TreatmentPlan,
                    MedicationDispensed = encounterVm.MedicationDispensed,
                    Procedures = encounterVm.Procedures,
                    FollowUp = encounterVm.FollowUp,
                };
                _context.Encounters.Add(encounterData);
                _context.SaveChanges();
            }
        }

        #endregion


        #region Admin Profile

        //***************************************************************************************************************************************************
        /// <summary>
        /// As per selectedaccount type fetch records of check boxes at Admin Edit/Create Account Info
        /// </summary>
        /// <param name="aspId"></param>
        /// <returns></returns>
        public List<Role> GetRolesForAdmin(string aspId)
        {
            return _context.Roles.AsEnumerable().Where(x => !x.Isdeleted[0] && x.Accounttype == 1).ToList();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Get Admin Region from Adminregion Table which states that in how many region admin will available
        /// </summary>
        /// <param name="aspId"></param>
        /// <returns></returns>
        public List<AdminregionTable> GetAdminregions(string aspId)
        {
            var regions = _context.Regions.ToList();
            var adminRegion = _context.Adminregions.ToList();
            var adminId = _context.Admins.FirstOrDefault(x => x.Aspnetuserid == aspId).Adminid;

            var CheckdRegion = regions.Select(r1 => new AdminregionTable
            {
                Regionid = r1.Regionid,
                Name = r1.Name,
                ExistsInTable = adminRegion.Any(r2 => r2.Regionid == r1.Regionid && r2.Adminid == adminId)

            }).ToList();
            return CheckdRegion;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Admin Profile Fetch Records
        /// </summary>
        /// <param name="aspId"></param>
        /// <returns></returns>
        public AdminProfileVm GetProfileData(string aspId)
        {
            var aspData = _context.Aspnetusers.FirstOrDefault(i => i.Id == aspId);
            var adminData = _context.Admins.FirstOrDefault(i => i.Aspnetuserid == aspId);

            var adminProfileData = new AdminProfileVm()
            {
                AspId = aspId,
                AdminId = adminData.Adminid,
                Username = aspData.Username,
                Status = (short)adminData.Status,
                RoleId = (int)adminData.Roleid,
                Firstname = adminData.Firstname,
                Lastname = adminData.Lastname,
                Email = adminData.Email,
                ConfirmEmail = adminData.Email,
                Phonenumber = adminData.Mobile,
                AltPhonenumber = adminData.Altphone,
                Address1 = adminData.Address1,
                Address2 = adminData.Address2,
                City = adminData.City,
                RegionId = (int)adminData.Regionid,
                Zipcode = adminData.Zip,
            };
            return adminProfileData;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Admin Profile (Passwd) Update Records
        /// </summary>
        /// <param name="password"></param>
        /// <param name="aspId"></param>
        public void AdminResetPassword(string password, string aspId)
        {
            var Aspnetuser = _context.Aspnetusers.FirstOrDefault(i => i.Id == aspId);

            Aspnetuser.Passwordhash = BCrypt.Net.BCrypt.HashPassword(password);
            _context.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Admin Profile (Account info) Update Records
        /// </summary>
        /// <param name="status"></param>
        /// <param name="roleId"></param>
        /// <param name="aspId"></param>
        public void AdminAccountUpdate(short status, int roleId, string aspId)
        {
            var admin = _context.Admins.FirstOrDefault(x => x.Aspnetuserid == aspId);

            if (admin != null)
            {
                admin.Status = status;
                admin.Roleid = roleId;
                admin.Modifiedby = aspId;
                admin.Modifieddate = DateTime.Now;
                _context.SaveChanges();
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Admin Profile (Administrator info) Update Records
        /// </summary>
        /// <param name="adminProfileVm"></param>
        /// <param name="regions"></param>
        public void AdministratorDetail(AdminProfileVm adminProfileVm, List<int> regions)
        {
            var aspData = _context.Aspnetusers.FirstOrDefault(i => i.Id == adminProfileVm.AspId);
            var adminData = _context.Admins.FirstOrDefault(i => i.Adminid == adminProfileVm.AdminId);

            if (aspData != null)
            {
                aspData.Phonenumber = adminProfileVm.Phonenumber;
                aspData.Modifieddate = DateTime.Now;
            }

            if (adminData.Adminid != null)
            {
                var Adminregion = _context.Adminregions.Where(i => i.Adminid == adminProfileVm.AdminId).ToList();
                _context.Adminregions.RemoveRange(Adminregion);
            }

            if (adminData != null)
            {
                adminData.Firstname = adminProfileVm.Firstname;
                adminData.Lastname = adminProfileVm.Lastname;
                adminData.Mobile = adminProfileVm.Phonenumber;
                adminData.Modifiedby = adminProfileVm.AspId;
                adminData.Modifieddate = DateTime.Now;
                _context.SaveChanges();
            }

            foreach (int regionid in regions)
            {
                Region? region = _context.Regions.FirstOrDefault(r => r.Regionid == regionid);

                _context.Adminregions.Add(new Adminregion
                {
                    Adminid = adminProfileVm.AdminId,
                    Regionid = regionid,
                });
            }
            _context.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Admin Profile (Mailing info) Update Records
        /// </summary>
        /// <param name="adminProfileVm"></param>
        public void MailingDetail(AdminProfileVm adminProfileVm)
        {
            var Admindata = _context.Admins.FirstOrDefault(i => i.Adminid == adminProfileVm.AdminId);

            if (Admindata != null)
            {
                Admindata.Address1 = adminProfileVm.Address1;
                Admindata.Address2 = adminProfileVm.Address2;
                Admindata.City = adminProfileVm.City;
                Admindata.Regionid = adminProfileVm.RegionId;
                Admindata.Altphone = adminProfileVm.AltPhonenumber;
                Admindata.Zip = adminProfileVm.Zipcode;
                Admindata.Modifiedby = adminProfileVm.AspId;
                Admindata.Modifieddate = DateTime.Now;
                _context.SaveChanges();
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Admin Profile (Delete) Update Records
        /// </summary>
        /// <param name="adminId"></param>
        public void RemoveAdmin(int adminId)
        {
            var admin = _context.Admins.FirstOrDefault(x => x.Adminid == adminId);

            admin.Isdeleted = true;
            admin.Modifieddate = DateTime.Now;
            _context.SaveChanges();
        }

        #endregion


        #region Provider Profile

        //***************************************************************************************************************************************************
        /// <summary>
        /// Get Roles for Physician only 
        /// </summary>
        /// <returns></returns>
        public List<Role> GetRolesForPhysicians()
        {
            return _context.Roles.AsEnumerable().Where(x => !x.Isdeleted[0] && x.Accounttype == 2).ToList();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Fetch Records For Providers Tab's Provider Table
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public List<Provider> GetProviders(int regionId)
        {
            var physicians = _context.Physicians.ToList();

            physicians = physicians.Where(x => x.Isdeleted == null).ToList();

            if (regionId != 0)
            {
                physicians = _context.Physicians.Where(x => x.Regionid == regionId && x.Isdeleted == null).ToList();
            }
            var currentTime = new TimeOnly(DateTime.Now.Hour, DateTime.Now.Minute);
            BitArray deletedBit = new BitArray(new[] { false });

            var providerList = physicians.Select(x => new Provider()
            {
                Email = x.Email,
                aspId = x.Aspnetuserid,
                physicianId = x.Physicianid,
                Name = x.Firstname + " " + x.Lastname,
                Role = _context.Roles.FirstOrDefault(i => i.Roleid == x.Roleid).Name,
                CallStatus = _context.Physicians
                .Where(p => !_context.Shiftdetails.Any(sd => sd.Shift.Physicianid == x.Physicianid &&
                                                               sd.Shiftdate.Date == DateTime.Today &&
                                                               currentTime >= sd.Starttime &&
                                                               currentTime <= sd.Endtime &&
                                                               sd.Isdeleted.Equals(deletedBit)) &&
                                                               p.Isdeleted == null).Count() == 0 ? "Bussy" : "Available",
                Status = (short)x.Status,
                Isdeleted = x.Isdeleted == null ? null : x.Isdeleted,
                IsNotificationStopped = _context.Physiciannotifications.FirstOrDefault(i => i.Physicianid == x.Physicianid)?.Isnotificationstopped != null && _context.Physiciannotifications.FirstOrDefault(i => i.Physicianid == x.Physicianid)?.Isnotificationstopped?[0] == true,

            }).ToList();
            return providerList;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Get Physician Region from Adminregion Table which states that in how many region physician will available
        /// </summary>
        /// <param name="aspId"></param>
        /// <returns></returns>
        public List<PhysicianRegionTable> GetPhysicianRegionTables(string aspId)
        {
            var region = _context.Regions.ToList();
            var physicianRegion = _context.Physicianregions.ToList();
            var phycisianId = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == aspId).Physicianid;

            var checkedRegion = region.Select(r1 => new PhysicianRegionTable
            {
                Regionid = r1.Regionid,
                Name = r1.Name,
                ExistsInTable = physicianRegion.Any(r2 => r2.Regionid == r1.Regionid && r2.Physicianid == phycisianId)

            }).ToList();

            return checkedRegion;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Stop/Unstop Notification for Physician
        /// </summary>
        /// <param name="physicianId"></param>
        /// <returns></returns>
        public int StopNotificationPhysician(int physicianId)
        {
            var physiciannotification = _context.Physiciannotifications.FirstOrDefault(x => x.Physicianid == physicianId);

            if (physiciannotification == null)
            {
                var pn = new Physiciannotification();
                pn.Physicianid = physicianId;
                pn.Isnotificationstopped = new BitArray(1, true);
                _context.Physiciannotifications.Add(pn);
                _context.SaveChanges();
                return 1;
            }
            else
            {
                if (physiciannotification.Isnotificationstopped[0] == true)
                {
                    physiciannotification.Isnotificationstopped = new BitArray(1, false);
                    _context.SaveChanges();
                    return 0;
                }
                else
                {
                    physiciannotification.Isnotificationstopped = new BitArray(1, true);
                    _context.SaveChanges();
                    return 0;
                }
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Contact Provider Via Email Update Records
        /// </summary>
        /// <param name="providersVm"></param>
        /// <param name="aspId"></param>
        public void ContactProviderViaEmail(ProvidersVm providersVm, string aspId)
        {
            var admin = _context.Admins.FirstOrDefault(x => x.Aspnetuserid == aspId);
            var physician = _context.Physicians.FirstOrDefault(x => x.Email == providersVm.Email);

            var mail = "tatva.dotnet.takshgadhiya@outlook.com";
            var password = "12!@Taksh";
            var email = providersVm.Email;
            var subject = "Message From Admin To Physician";
            var message = $"We trust this message finds you in good spirits.Hey, {providersVm.Email.Substring(0, providersVm.Email.IndexOf('@'))} <br><br>" +
                $"Here is the message from {admin.Firstname} {admin.Lastname} : <br>" +
                $"{providersVm.ContactMessage}";

            SmtpClient smtpClient = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mail, password)
            };

            MailMessage mailMessage = new MailMessage(from: mail, to: email, subject, message);
            mailMessage.IsBodyHtml = true;

            smtpClient.SendMailAsync(mailMessage);
            var emailLog = new Emaillog()
            {
                Subjectname = subject,
                Emailid = email,
                Roleid = 2,
                Adminid = admin.Adminid,
                Physicianid = physician.Physicianid,
                Createdate = DateTime.Now,
                Sentdate = DateTime.Now,
                Isemailsent = new BitArray(1, true),
                Senttries = 1,
            };
            _context.Emaillogs.Add(emailLog);
            _context.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Contact Provider Via Email Update Records
        /// </summary>
        /// <param name="providersVm"></param>
        /// <param name="aspId"></param>
        public void ContactProviderViaContact(ProvidersVm providersVm, string aspId)
        {
            var admin = _context.Admins.FirstOrDefault(x => x.Aspnetuserid == aspId);
            var physician = _context.Physicians.FirstOrDefault(x => x.Email == providersVm.Email);

            var smsLog = new Smslog()
            {
                Smstemplate = "SMS",
                Mobilenumber = physician.Mobile,
                Roleid = 2,
                Adminid = admin.Adminid,
                Physicianid = physician.Physicianid,
                Createdate = DateTime.Now,
                Sentdate = DateTime.Now,
                Issmssent = new BitArray(1, true),
                Senttries = 1,
            };
            _context.Smslogs.Add(smsLog);
            _context.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Edit Physician Fetch Records
        /// </summary>
        /// <param name="aspId"></param>
        /// <returns></returns>
        public ProviderProfileVm GetProviderProfileData(string aspId)
        {
            var aspData = _context.Aspnetusers.FirstOrDefault(x => x.Id == aspId);

            if (aspData != null)
            {
                var provider = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == aspId);

                var providerData = new ProviderProfileVm()
                {
                    AspId = provider.Aspnetuserid,
                    PhysicianId = provider.Physicianid,
                    Username = aspData.Username,
                    Status = provider.Status,
                    RoleId = provider.Roleid,
                    FirstName = provider.Firstname,
                    LastName = provider.Lastname,
                    Email = provider.Email,
                    Phonenumber = provider.Mobile,
                    MedicalLicense = provider.Medicallicense,
                    NPINumber = provider.Npinumber,
                    SyncEmail = provider.Syncemailaddress,
                    Address1 = provider.Address1,
                    Address2 = provider.Address2,
                    City = provider.City,
                    RegionId = provider.Regionid,
                    Zipcode = provider.Zip,
                    AltPhone = provider.Altphone,
                    BusinessName = provider.Businessname,
                    BusinessWebsite = provider.Businesswebsite,
                    PhotoValue = provider.Photo,
                    SignatureValue = provider.Signature,
                    AdminNotes = provider.Adminnotes,
                    Isdeleted = provider.Isdeleted,
                };
                return providerData;
            }
            return null;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Edit Physician (Passwd) Update Records
        /// </summary>
        /// <param name="password"></param>
        /// <param name="aspId"></param>
        public void PhysicianResetPassword(string password, string aspId)
        {
            var aspUser = _context.Aspnetusers.FirstOrDefault(x => x.Id == aspId);

            if (aspUser != null)
            {
                aspUser.Passwordhash = BCrypt.Net.BCrypt.HashPassword(password);
                aspUser.Modifieddate = DateTime.Now;
                _context.SaveChanges();
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Edit Physician (Account Info) Update Records
        /// </summary>
        /// <param name="status"></param>
        /// <param name="roleId"></param>
        /// <param name="aspId"></param>
        public void PhysicianAccountUpdate(short status, int roleId, string aspId)
        {
            var physician = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == aspId);

            if (physician != null)
            {
                physician.Status = status;
                physician.Roleid = roleId;
                physician.Modifieddate = DateTime.Now;
                physician.Modifiedby = aspId;
                _context.SaveChanges();
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Edit Physician (Administrator Info) Update Records
        /// </summary>
        /// <param name="providerProfileVm"></param>
        /// <param name="physicianRegions"></param>
        public void PhysicianAdministratorDataUpdate(ProviderProfileVm providerProfileVm, List<int> physicianRegions)
        {
            var physician = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == providerProfileVm.AspId);

            if (physician != null)
            {
                physician.Firstname = providerProfileVm.FirstName;
                physician.Lastname = providerProfileVm.LastName;
                physician.Mobile = providerProfileVm.Phonenumber;
                physician.Medicallicense = providerProfileVm.MedicalLicense;
                physician.Npinumber = providerProfileVm.NPINumber;
                physician.Syncemailaddress = providerProfileVm.SyncEmail;
                physician.Modifieddate = DateTime.Now;

                if (_context.Physicianregions.Any(x => x.Physicianid == physician.Physicianid))
                {
                    var physicianRegion = _context.Physicianregions.Where(x => x.Physicianid == physician.Physicianid).ToList();
                    _context.Physicianregions.RemoveRange(physicianRegion);
                    _context.SaveChanges();
                }

                foreach (int regionId in physicianRegions)
                {
                    var region = _context.Regions.FirstOrDefault(x => x.Regionid == regionId);

                    _context.Physicianregions.Add(new Physicianregion
                    {
                        Physicianid = providerProfileVm.PhysicianId,
                        Regionid = region.Regionid,
                    });
                }
                _context.SaveChanges();
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Edit Physician (Mailing Info) Update Records
        /// </summary>
        /// <param name="providerProfileVm"></param>
        public void PhysicianMailingDataUpdate(ProviderProfileVm providerProfileVm)
        {
            var physician = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == providerProfileVm.AspId);
            var phylocation = _context.Physicianlocations.FirstOrDefault(x => x.Physicianid == providerProfileVm.PhysicianId);

            if (physician != null)
            {
                physician.Address1 = providerProfileVm.Address1;
                physician.Address2 = providerProfileVm.Address2;
                physician.City = providerProfileVm.City;
                physician.Regionid = providerProfileVm.RegionId;
                physician.Zip = providerProfileVm.Zipcode;
                physician.Altphone = providerProfileVm.AltPhone;
                physician.Modifieddate = DateTime.Now;
                _context.SaveChanges();
            }

            if (phylocation != null)
            {
                phylocation.Latitude = providerProfileVm.Latitude;
                phylocation.Longitude = providerProfileVm.Longitude;
                phylocation.Address = providerProfileVm.Address1 + ", " + providerProfileVm.City;
                _context.SaveChanges();
            }

            if (phylocation == null)
            {
                var physicianLocation = new Physicianlocation()
                {
                    Physicianid = providerProfileVm.PhysicianId,
                    Latitude = providerProfileVm.Latitude,
                    Longitude = providerProfileVm.Longitude,
                    Createddate = DateTime.Now,
                    Physicianname = _context.Physicians.FirstOrDefault(i => i.Physicianid == providerProfileVm.PhysicianId).Firstname + " " + _context.Physicians.FirstOrDefault(i => i.Physicianid == providerProfileVm.PhysicianId).Lastname,
                    Address = providerProfileVm.Address1 + ", " + providerProfileVm.City,
                };
                _context.Add(physicianLocation);
                _context.SaveChanges();
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Edit Physician (Business Info) Update Records
        /// </summary>
        /// <param name="providerProfileVm"></param>
        public void PhysicianBusinessInfoUpdate(ProviderProfileVm providerProfileVm)
        {
            var physician = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == providerProfileVm.AspId);

            if (physician != null)
            {
                physician.Businessname = providerProfileVm.BusinessName;
                physician.Businesswebsite = providerProfileVm.BusinessWebsite;
                physician.Adminnotes = providerProfileVm.AdminNotes;
                physician.Modifieddate = DateTime.Now;
                _context.SaveChanges();

                if (providerProfileVm.Photo != null || providerProfileVm.Signature != null)
                {
                    AddProviderBusinessPhotos(providerProfileVm.Photo, providerProfileVm.Signature, providerProfileVm.AspId);
                }
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Edit Physician (photo + sign) Update Records
        /// </summary>
        /// <param name="photo"></param>
        /// <param name="signature"></param>
        /// <param name="aspId"></param>
        public void AddProviderBusinessPhotos(IFormFile photo, IFormFile signature, string aspId)
        {
            var physician = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == aspId);

            string directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content", physician.Physicianid.ToString());

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (photo != null)
            {
                string path = Path.Combine(directory, "Profile" + Path.GetExtension(photo.FileName));

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    photo.CopyTo(fileStream);
                }
                physician.Photo = photo.FileName;
            }
            if (signature != null)
            {
                string path = Path.Combine(directory, "Signature" + Path.GetExtension(signature.FileName));

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    signature.CopyTo(fileStream);
                }
                physician.Signature = signature.FileName;
            }
            _context.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Edit Physician (Onbording Info) Update Records
        /// </summary>
        /// <param name="providerProfileVm"></param>
        public void EditOnBoardingData(ProviderProfileVm providerProfileVm)
        {
            var physicianData = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == providerProfileVm.AspId);

            string directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content", physicianData.Physicianid.ToString());

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (providerProfileVm.ContractorAgreement != null)
            {
                string path = Path.Combine(directory, "Independent_Contractor" + Path.GetExtension(providerProfileVm.ContractorAgreement.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    providerProfileVm.ContractorAgreement.CopyTo(fileStream);
                }
                physicianData.Isagreementdoc = new BitArray(1, true);
            }
            if (providerProfileVm.BackgroundCheck != null)
            {
                string path = Path.Combine(directory, "Background" + Path.GetExtension(providerProfileVm.BackgroundCheck.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    providerProfileVm.BackgroundCheck.CopyTo(fileStream);
                }
                physicianData.Isbackgrounddoc = new BitArray(1, true);
            }
            if (providerProfileVm.HIPAA != null)
            {
                string path = Path.Combine(directory, "HIPAA" + Path.GetExtension(providerProfileVm.HIPAA.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    providerProfileVm.HIPAA.CopyTo(fileStream);
                }
                physicianData.Istrainingdoc = new BitArray(1, true);
            }
            if (providerProfileVm.NonDisclosure != null)
            {
                string path = Path.Combine(directory, "Non_Disclosure" + Path.GetExtension(providerProfileVm.NonDisclosure.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    providerProfileVm.NonDisclosure.CopyTo(fileStream);
                }
                physicianData.Isnondisclosuredoc = new BitArray(1, true);
            }
            if (providerProfileVm.LicenseDocument != null)
            {
                string path = Path.Combine(directory, "Licence" + Path.GetExtension(providerProfileVm.LicenseDocument.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    providerProfileVm.LicenseDocument.CopyTo(fileStream);
                }
                physicianData.Islicensedoc = new BitArray(1, true);
            }
            _context.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Edit Physician (Delete Account) Update Records
        /// </summary>
        /// <param name="physicianId"></param>
        public bool RemovePhysician(int physicianId)
        {
            var requests = _context.Requests.Where(r => r.Physicianid == physicianId && (r.Status == 4 || r.Status == 5));

            if (requests.Any())
            {
                return false;
            }
            else
            {
                var physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == physicianId);

                physician.Isdeleted = new BitArray(1, true);
                physician.Modifieddate = DateTime.Now;
                _context.SaveChanges();
                return true;
            }
        }

        #endregion


        #region Create Provider Acc

        //***************************************************************************************************************************************************
        /// <summary>
        /// Create Physician Account (Data) Update Records
        /// </summary>
        /// <param name="providerProfileVm"></param>
        /// <param name="physicianRegions"></param>
        /// <returns></returns>
        public bool CreatePhysicianAccount(ProviderProfileVm providerProfileVm, List<int> physicianRegions)
        {
            if (!_context.Aspnetusers.Any(x => x.Email == providerProfileVm.Email))
            {
                var aspData = new Aspnetuser()
                {
                    Username = "MD" + providerProfileVm.LastName + providerProfileVm.FirstName.Substring(0, 1).ToUpper(),
                    Passwordhash = BCrypt.Net.BCrypt.HashPassword(providerProfileVm.CreatePhyPassword),
                    Email = providerProfileVm.Email,
                    Phonenumber = providerProfileVm.Phonenumber,
                    Createddate = DateTime.Now,
                };
                _context.Aspnetusers.Add(aspData);
                _context.SaveChanges();

                var physicianData = new Physician()
                {
                    Aspnetuserid = aspData.Id,
                    Firstname = providerProfileVm.FirstName,
                    Lastname = providerProfileVm.LastName,
                    Email = providerProfileVm.Email,
                    Mobile = providerProfileVm.Phonenumber,
                    Medicallicense = providerProfileVm.MedicalLicense,
                    Adminnotes = providerProfileVm.AdminNotes,
                    Address1 = providerProfileVm.Address1,
                    Address2 = providerProfileVm.Address2,
                    City = providerProfileVm.City,
                    Regionid = providerProfileVm.RegionId,
                    Zip = providerProfileVm.Zipcode,
                    Altphone = providerProfileVm.AltPhone,
                    Createddate = DateTime.Now,
                    Status = 1,
                    Businessname = providerProfileVm.BusinessName,
                    Businesswebsite = providerProfileVm.BusinessWebsite,
                    Roleid = providerProfileVm.RoleId,
                };
                _context.Physicians.Add(physicianData);
                _context.SaveChanges();

                Aspnetuserrole anur = new Aspnetuserrole();
                anur.Userid = aspData.Id;
                anur.Roleid = "2";
                _context.Aspnetuserroles.Add(anur);
                _context.SaveChanges();

                foreach (int regionId in physicianRegions)
                {
                    var region = _context.Regions.FirstOrDefault(x => x.Regionid == regionId);

                    _context.Physicianregions.Add(new Physicianregion
                    {
                        Physicianid = physicianData.Physicianid,
                        Regionid = region.Regionid,
                    });
                }
                _context.SaveChanges();

                var phyLocation = new Physicianlocation()
                {
                    Physicianid = physicianData.Physicianid,
                    Latitude = providerProfileVm.Latitude,
                    Longitude = providerProfileVm.Longitude,
                    Createddate = DateTime.Now.Date,
                    Physicianname = providerProfileVm.FirstName + " " + providerProfileVm.LastName,
                    Address = providerProfileVm.City + "," + _context.Regions.FirstOrDefault(i => i.Regionid == providerProfileVm.RegionId).Name,
                };
                _context.Add(phyLocation);
                _context.SaveChanges();

                AddProviderDocuments(physicianData.Physicianid, providerProfileVm.Photo, providerProfileVm.ContractorAgreement, providerProfileVm.BackgroundCheck, providerProfileVm.HIPAA, providerProfileVm.NonDisclosure);

                return true;
            }
            return false;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Create Physician Account (Docs) Update Records
        /// </summary>
        /// <param name="Physicianid"></param>
        /// <param name="Photo"></param>
        /// <param name="ContractorAgreement"></param>
        /// <param name="BackgroundCheck"></param>
        /// <param name="HIPAA"></param>
        /// <param name="NonDisclosure"></param>
        public void AddProviderDocuments(int Physicianid, IFormFile Photo, IFormFile ContractorAgreement, IFormFile BackgroundCheck, IFormFile HIPAA, IFormFile NonDisclosure)
        {
            var physicianData = _context.Physicians.FirstOrDefault(x => x.Physicianid == Physicianid);

            string directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content", Physicianid.ToString());

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (Photo != null)
            {
                string path = Path.Combine(directory, "Profile" + Path.GetExtension(Photo.FileName));

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    Photo.CopyTo(fileStream);
                }
                physicianData.Photo = Photo.FileName;
            }
            if (ContractorAgreement != null)
            {
                string path = Path.Combine(directory, "Independent_Contractor" + Path.GetExtension(ContractorAgreement.FileName));

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    ContractorAgreement.CopyTo(fileStream);
                }
                physicianData.Isagreementdoc = new BitArray(1, true);
            }
            if (BackgroundCheck != null)
            {
                string path = Path.Combine(directory, "Background" + Path.GetExtension(BackgroundCheck.FileName));

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    BackgroundCheck.CopyTo(fileStream);
                }
                physicianData.Isbackgrounddoc = new BitArray(1, true);
            }
            if (HIPAA != null)
            {
                string path = Path.Combine(directory, "HIPAA" + Path.GetExtension(HIPAA.FileName));

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    HIPAA.CopyTo(fileStream);
                }
                physicianData.Istrainingdoc = new BitArray(1, true);
            }
            if (NonDisclosure != null)
            {
                string path = Path.Combine(directory, "Non_Disclosure" + Path.GetExtension(NonDisclosure.FileName));

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    NonDisclosure.CopyTo(fileStream);
                }
                physicianData.Isnondisclosuredoc = new BitArray(1, true);
            }
            _context.SaveChanges();
        }

        #endregion


        #region Acc Access

        //***************************************************************************************************************************************************
        /// <summary>
        /// Account Access Fetch Records
        /// </summary>
        /// <returns></returns>
        public List<AccountAccess> GetAccountAccessData()
        {
            BitArray deletedBit = new BitArray(new[] { false });
            var Roles = _context.Roles.Where(i => i.Isdeleted.Equals(deletedBit));

            var Accessdata = Roles.Select(r => new AccountAccess()
            {
                name = r.Name,
                accounttype = _context.Aspnetroles.FirstOrDefault(x => x.Id == r.Accounttype.ToString()).Name,
                accounttypeid = r.Accounttype,
                roleid = r.Roleid,

            }).ToList();
            return Accessdata;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Get Accounttypes from Aspnetroles Table where Patient is excluded for both Edit/Create Account Access
        /// </summary>
        /// <returns></returns>
        public List<Aspnetrole> GetAccountType()
        {
            var role = _context.Aspnetroles.Where(x => x.Id != 3.ToString()).ToList();
            return role;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// As per selected account type fetch records of check boxes for create account access
        /// </summary>
        /// <param name="accounttype"></param>
        /// <returns></returns>
        public List<Menu> GetMenu(int accounttype)
        {
            if (accounttype != 0)
            {
                var menu = _context.Menus.Where(r => r.Accounttype == accounttype).ToList();
                return menu;
            }
            else
            {
                var menu = _context.Menus.ToList();
                return menu;
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Create Account Access Update Records
        /// </summary>
        /// <param name="accountAccess"></param>
        /// <param name="AccountMenu"></param>
        public void SetCreateAccessAccount(AccountAccess accountAccess, List<int> AccountMenu)
        {
            if (accountAccess != null)
            {
                var role = new Role()
                {
                    Name = accountAccess.name,
                    Accounttype = (short)accountAccess.accounttypeid,
                    Createdby = "Admin",
                    Createddate = DateTime.Now,
                    Isdeleted = new BitArray(1, false),
                };
                _context.Add(role);
                _context.SaveChanges();

                if (AccountMenu != null)
                {
                    foreach (int menuid in AccountMenu)
                    {
                        _context.Rolemenus.Add(new Rolemenu
                        {
                            Roleid = role.Roleid,
                            Menuid = menuid,
                        });
                    }
                    _context.SaveChanges();
                }
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Edit Account Access Fetch Records
        /// </summary>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public AccountAccess GetEditAccessData(int roleid)
        {
            var role = _context.Roles.FirstOrDefault(i => i.Roleid == roleid);
            if (role != null)
            {
                var roledata = new AccountAccess()
                {
                    name = role.Name,
                    roleid = roleid,
                    accounttypeid = role.Accounttype,
                };
                return roledata;
            }
            return null;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// As per selected account type fetch records of check boxes for Edit account access
        /// </summary>
        /// <param name="accounttype"></param>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public List<AccountMenu> GetAccountMenu(int accounttype, int roleid)
        {
            var menu = _context.Menus.Where(r => r.Accounttype == accounttype).ToList();

            var rolemenu = _context.Rolemenus.ToList();

            var checkedMenu = menu.Select(r1 => new AccountMenu
            {
                menuid = r1.Menuid,
                name = r1.Name,
                ExistsInTable = rolemenu.Any(r2 => r2.Roleid == roleid && r2.Menuid == r1.Menuid),

            }).ToList();

            return checkedMenu;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Edit Account Access Update Records
        /// </summary>
        /// <param name="accountAccess"></param>
        /// <param name="AccountMenu"></param>
        public void SetEditAccessAccount(AccountAccess accountAccess, List<int> AccountMenu)
        {
            var role = _context.Roles.FirstOrDefault(x => x.Roleid == accountAccess.roleid);
            if (role != null)
            {
                role.Accounttype = (short)accountAccess.accounttypeid;
                role.Createdby = "Admin";
                role.Modifieddate = DateTime.Now;

                _context.SaveChanges();

                var rolemenu = _context.Rolemenus.Where(i => i.Roleid == accountAccess.roleid).ToList();
                if (rolemenu != null)
                {
                    _context.Rolemenus.RemoveRange(rolemenu);
                }

                if (AccountMenu != null)
                {
                    foreach (int menuid in AccountMenu)
                    {
                        _context.Rolemenus.Add(new Rolemenu
                        {
                            Roleid = role.Roleid,
                            Menuid = menuid,
                        });
                    }
                    _context.SaveChanges();
                }
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Delete Account Access
        /// </summary>
        /// <param name="roleid"></param>
        public void DeleteAccountAccess(int roleid)
        {
            var role = _context.Roles.FirstOrDefault(x => x.Roleid == roleid);
            if (role != null)
            {
                role.Isdeleted = new BitArray(1, true);
                _context.SaveChanges();
            }

            var rolemenu = _context.Rolemenus.Where(i => i.Roleid == roleid);
            if (rolemenu != null)
            {
                _context.Rolemenus.RemoveRange(rolemenu);
                _context.SaveChanges();
            }
        }

        #endregion


        #region User Access

        //***************************************************************************************************************************************************
        /// <summary>
        /// User Access Fetch Records
        /// </summary>
        /// <param name="accountTypeId"></param>
        /// <returns></returns>
        public List<UserAccess> GetUserAccessData(int accountTypeId)
        {
            var admin = _context.Admins.Where(i => i.Isdeleted == null).ToList();
            var physician = _context.Physicians.Where(i => i.Isdeleted == null).ToList();

            var adminList = admin.Select(x => new UserAccess
            {
                AspId = x.Aspnetuserid,
                AccountTypeId = 1,
                AccountType = "Admin",
                AccountHolderName = x.Firstname + " " + x.Lastname,
                AccountPhone = x.Mobile,
                AccountStatus = (short)x.Status,
                AccountRequests = _context.Requests.Where(x => x.Status != 10 && x.Status != 11).Count(),

            }).ToList();

            var physicianList = physician.Select(x => new UserAccess
            {
                AspId = x.Aspnetuserid,
                AccountTypeId = 2,
                AccountType = "Physician",
                AccountHolderName = x.Firstname + " " + x.Lastname,
                AccountPhone = x.Mobile,
                AccountStatus = (short)x.Status,
                AccountRequests = _context.Requests.Where(y => y.Physicianid == x.Physicianid && (y.Status == 2 || y.Status == 4 || y.Status == 5 || y.Status == 6)).Count(),

            }).ToList();

            var finalList = adminList.Concat(physicianList).ToList();

            if (accountTypeId == 1)
            {
                return adminList;
            }
            else if (accountTypeId == 2)
            {
                return physicianList;
            }
            return finalList;
        }

        #endregion


        #region Create Admin Acc

        //***************************************************************************************************************************************************
        /// <summary>
        /// Create Admin Account Update Records
        /// </summary>
        /// <param name="adminProfileVm"></param>
        /// <param name="adminRegions"></param>
        /// <returns></returns>
        public bool CreateAdminAccount(AdminProfileVm adminProfileVm, List<int> adminRegions)
        {
            if (!_context.Aspnetusers.Any(x => x.Email == adminProfileVm.Email))
            {
                var aspData = new Aspnetuser()
                {
                    Username = adminProfileVm.Lastname + adminProfileVm.Firstname.Substring(0, 1).ToUpper(),
                    Passwordhash = BCrypt.Net.BCrypt.HashPassword(adminProfileVm.CreateAdminPassword),
                    Email = adminProfileVm.Email,
                    Phonenumber = adminProfileVm.Phonenumber,
                    Createddate = DateTime.Now,
                };
                _context.Aspnetusers.Add(aspData);
                _context.SaveChanges();

                var adminData = new Admin()
                {
                    Aspnetuserid = aspData.Id,
                    Firstname = adminProfileVm.Firstname,
                    Lastname = adminProfileVm.Lastname,
                    Email = adminProfileVm.Email,
                    Mobile = adminProfileVm.Phonenumber,
                    Address1 = adminProfileVm.Address1,
                    Address2 = adminProfileVm.Address2,
                    City = adminProfileVm.City,
                    Regionid = adminProfileVm.RegionId,
                    Zip = adminProfileVm.Zipcode,
                    Altphone = adminProfileVm.AltPhonenumber,
                    Createdby = adminProfileVm.AspId,
                    Createddate = DateTime.Now,
                    Status = 1,
                    Roleid = adminProfileVm.RoleId,
                    Modifiedby = adminProfileVm.AspId,
                };
                _context.Admins.Add(adminData);
                _context.SaveChanges();

                Aspnetuserrole anur = new Aspnetuserrole();
                anur.Userid = aspData.Id;
                anur.Roleid = "1";
                _context.Aspnetuserroles.Add(anur);
                _context.SaveChanges();

                foreach (int regionId in adminRegions)
                {
                    var region = _context.Regions.FirstOrDefault(x => x.Regionid == regionId);

                    _context.Adminregions.Add(new Adminregion
                    {
                        Adminid = adminData.Adminid,
                        Regionid = region.Regionid,
                    });
                }
                _context.SaveChanges();

                return true;
            }
            return false;
        }

        #endregion


        #region Schedulling

        //***************************************************************************************************************************************************
        /// <summary>
        /// Get Physician List for Schedulling
        /// </summary>
        /// <returns></returns>
        public List<Physician> GetPhysicians()
        {
            var physicians = _context.Physicians.Where(i => i.Isdeleted == null).ToList();
            return physicians;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Schedulling Fetch Records
        /// </summary>
        /// <param name="date"></param>
        /// <param name="sunday"></param>
        /// <param name="saturday"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<ShiftDetailsmodal> ShiftDetailsmodal(DateTime date, DateTime sunday, DateTime saturday, string type, string aspId)
        {
            var physician = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == aspId);

            var shiftdetails = _context.Shiftdetails.Where(u => u.Shiftdate.Month == date.Month && u.Shiftdate.Year == date.Year);

            BitArray deletedBit = new BitArray(new[] { true });

            switch (type)
            {
                case "month":
                    shiftdetails = _context.Shiftdetails.Where(u => u.Shiftdate.Month == date.Month && u.Shiftdate.Year == date.Year && !u.Isdeleted.Equals(deletedBit));
                    break;

                case "week":
                    shiftdetails = _context.Shiftdetails.Where(u => (u.Shiftdate >= sunday || u.Shiftdate <= saturday) && !u.Isdeleted.Equals(deletedBit));
                    break;

                case "day":
                    shiftdetails = _context.Shiftdetails.Where(u => u.Shiftdate.Month == date.Month && u.Shiftdate.Year == date.Year && u.Shiftdate.Day == date.Day && !u.Isdeleted.Equals(deletedBit));
                    break;
            }

            var list = shiftdetails.Select(s => new ShiftDetailsmodal
            {
                Shiftid = s.Shiftid,
                Shiftdetailid = s.Shiftdetailid,
                Shiftdate = s.Shiftdate,
                Startdate = s.Shift.Startdate,
                Starttime = s.Starttime,
                Endtime = s.Endtime,
                Physicianid = s.Shift.Physicianid,
                PhysicianName = s.Shift.Physician.Firstname,
                Status = s.Status,
                regionname = _context.Regions.FirstOrDefault(i => i.Regionid == s.Regionid).Name,
                Abberaviation = _context.Regions.FirstOrDefault(i => i.Regionid == s.Regionid).Abbreviation,
                Regionid = s.Regionid,
            });

            if (physician != null)
            {
                list = list.Where(i => i.Physicianid == physician.Physicianid);
            }
            return list.ToList();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Create Shift Update Records
        /// </summary>
        /// <param name="scheduleModel"></param>
        /// <param name="Aspid"></param>
        /// <returns></returns>
        public bool createShift(ScheduleModel scheduleModel, string Aspid)
        {
            if (_context.Shifts.Where(x => x.Physicianid == scheduleModel.Physicianid).Count() >= 1)
            {
                var shiftData = _context.Shifts.Where(i => i.Physicianid == scheduleModel.Physicianid).ToList();
                var shiftDetailData = new List<Shiftdetail>();

                foreach (var obj in shiftData)
                {
                    var details = _context.Shiftdetails.Where(x => x.Shiftid == obj.Shiftid).ToList();
                    shiftDetailData.AddRange(details);
                }

                foreach (var obj in shiftDetailData)
                {
                    var shiftDate = new DateTime(scheduleModel.Startdate.Year, scheduleModel.Startdate.Month, scheduleModel.Startdate.Day);

                    if (obj.Shiftdate.Date == shiftDate.Date)
                    {
                        if ((obj.Starttime <= scheduleModel.Starttime && obj.Endtime >= scheduleModel.Starttime) || (obj.Starttime <= scheduleModel.Endtime && obj.Endtime >= scheduleModel.Endtime) || (obj.Starttime >= scheduleModel.Starttime && obj.Endtime <= scheduleModel.Endtime))
                        {
                            return false;
                        }
                    }
                }
            }

            Shift shift = new Shift();
            shift.Physicianid = scheduleModel.Physicianid;
            shift.Repeatupto = scheduleModel.Repeatupto;
            shift.Startdate = scheduleModel.Startdate;
            shift.Createdby = Aspid;
            shift.Createddate = DateTime.Now;
            shift.Isrepeat = scheduleModel.Isrepeat == false ? new BitArray(1, false) : new BitArray(1, true);
            shift.Repeatupto = scheduleModel.Repeatupto;
            _context.Shifts.Add(shift);
            _context.SaveChanges();

            Shiftdetail sd = new Shiftdetail();
            sd.Shiftid = shift.Shiftid;
            sd.Shiftdate = new DateTime(scheduleModel.Startdate.Year, scheduleModel.Startdate.Month, scheduleModel.Startdate.Day);
            sd.Starttime = scheduleModel.Starttime;
            sd.Endtime = scheduleModel.Endtime;
            sd.Regionid = scheduleModel.Regionid;
            sd.Status = 1;
            sd.Isdeleted = new BitArray(1, false);
            _context.Shiftdetails.Add(sd);
            _context.SaveChanges();

            Shiftdetailregion sr = new Shiftdetailregion();
            sr.Shiftdetailid = sd.Shiftdetailid;
            sr.Regionid = scheduleModel.Regionid;
            sr.Isdeleted = new BitArray(1, false);
            _context.Shiftdetailregions.Add(sr);
            _context.SaveChanges();

            if (scheduleModel.Isrepeat != false)
            {

                List<int> day = scheduleModel.checkWeekday.Split(',').Select(int.Parse).ToList();

                foreach (int d in day)
                {
                    DayOfWeek desiredDayOfWeek = (DayOfWeek)d;
                    DateTime today = DateTime.Today;
                    DateTime nextOccurrence = new DateTime(scheduleModel.Startdate.Year, scheduleModel.Startdate.Month, scheduleModel.Startdate.Day);
                    int occurrencesFound = 0;
                    while (occurrencesFound < scheduleModel.Repeatupto)
                    {
                        if (nextOccurrence.DayOfWeek == desiredDayOfWeek && (nextOccurrence.Day != scheduleModel.Startdate.Day))
                        {
                            Shiftdetail sdd = new Shiftdetail();
                            sdd.Shiftid = shift.Shiftid;
                            sdd.Shiftdate = nextOccurrence;
                            sdd.Starttime = scheduleModel.Starttime;
                            sdd.Endtime = scheduleModel.Endtime;
                            sdd.Regionid = scheduleModel.Regionid;
                            sdd.Status = 1;
                            sdd.Isdeleted = new BitArray(1, false);
                            _context.Shiftdetails.Add(sdd);
                            _context.SaveChanges();

                            Shiftdetailregion srr = new Shiftdetailregion();
                            srr.Shiftdetailid = sdd.Shiftdetailid;
                            srr.Regionid = scheduleModel.Regionid;
                            srr.Isdeleted = new BitArray(1, false);
                            _context.Shiftdetailregions.Add(srr);
                            _context.SaveChanges();
                            occurrencesFound++;
                        }
                        nextOccurrence = nextOccurrence.AddDays(1);
                    }
                }
            }
            return true;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// View Shift Fetch Records
        /// </summary>
        /// <param name="shiftdetailsid"></param>
        /// <returns></returns>
        public ShiftDetailsmodal GetShift(int shiftdetailsid)
        {
            var shiftdetails = _context.Shiftdetails.FirstOrDefault(s => s.Shiftdetailid == shiftdetailsid);
            var physicianlist = _context.Physicianregions.Where(p => p.Regionid == shiftdetails.Regionid).Select(s => s.Physicianid).ToList();

            var sd = _context.Shiftdetails.FirstOrDefault(s => s.Shiftdetailid == shiftdetailsid);
            var s = _context.Shifts.FirstOrDefault(x => x.Shiftid == sd.Shiftid);

            ShiftDetailsmodal shiftModal = new ShiftDetailsmodal
            {
                Shiftdetailid = shiftdetailsid,
                Shiftdate = shiftdetails.Shiftdate,
                Shiftid = shiftdetails.Shiftid,
                Starttime = shiftdetails.Starttime,
                Endtime = shiftdetails.Endtime,
                Regionid = shiftdetails.Regionid,
                Abberaviation = _context.Regions.FirstOrDefault(i => i.Regionid == shiftdetails.Regionid).Abbreviation,
                Status = shiftdetails.Status,
                regions = _context.Regions.ToList(),
                Physicians = _context.Physicians.Where(x => x.Physicianid == s.Physicianid).ToList(),
            };
            return shiftModal;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// View Shift (Approve/disapprove) Update Records
        /// </summary>
        /// <param name="status"></param>
        /// <param name="shiftdetailid"></param>
        /// <param name="Aspid"></param>
        public void SetReturnShift(int status, int shiftdetailid, string Aspid)
        {
            var shiftdetails = _context.Shiftdetails.FirstOrDefault(s => s.Shiftdetailid == shiftdetailid);
            if (status == 1)
            {
                shiftdetails.Status = 2;
                shiftdetails.Modifieddate = DateTime.Now;
                shiftdetails.Modifiedby = Aspid;
            }
            else
            {
                shiftdetails.Status = 1;
                shiftdetails.Modifieddate = DateTime.Now;
                shiftdetails.Modifiedby = Aspid;
            }
            _context.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// View Shift (Delete) Update Records
        /// </summary>
        /// <param name="shiftdetailid"></param>
        /// <param name="Aspid"></param>
        public void SetDeleteShift(int shiftdetailid, string Aspid)
        {
            var shiftdetails = _context.Shiftdetails.FirstOrDefault(s => s.Shiftdetailid == shiftdetailid);

            shiftdetails.Isdeleted = new BitArray(1, true);
            shiftdetails.Modifieddate = DateTime.Now;
            shiftdetails.Modifiedby = Aspid;

            _context.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// View Shift (Edit Data) Update Records
        /// </summary>
        /// <param name="shiftDetailsmodal"></param>
        /// <param name="Aspid"></param>
        public void SetEditShift(ShiftDetailsmodal shiftDetailsmodal, string Aspid)
        {
            var shiftdetails = _context.Shiftdetails.FirstOrDefault(s => s.Shiftdetailid == shiftDetailsmodal.Shiftdetailid);

            if (shiftdetails != null)
            {
                shiftdetails.Shiftdate = shiftDetailsmodal.Shiftdate;
                shiftdetails.Starttime = shiftDetailsmodal.Starttime;
                shiftdetails.Endtime = shiftDetailsmodal.Endtime;
                shiftdetails.Modifieddate = DateTime.Now;
                shiftdetails.Modifiedby = Aspid;
                _context.SaveChanges();
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// SHift Review Fetch Records
        /// </summary>
        /// <param name="regionId"></param>
        /// <param name="callId"></param>
        /// <returns></returns>
        public List<ShiftReview> GetShiftReview(int regionId, int callId)
        {
            BitArray deletedBit = new BitArray(new[] { false });

            var shiftDetail = _context.Shiftdetails.Where(i => i.Isdeleted.Equals(deletedBit) && i.Status != 2).ToList();

            DateTime currentDate = DateTime.Now;

            if (regionId != 0 || callId == 1)
            {
                shiftDetail = _context.Shiftdetails.Where(i => i.Isdeleted.Equals(deletedBit) && i.Regionid == regionId && i.Status != 2).ToList();
            }

            var reviewList = shiftDetail.Select(x => new ShiftReview
            {
                shiftDetailId = x.Shiftdetailid,
                PhysicianName = _context.Physicians.FirstOrDefault(y => y.Physicianid == _context.Shifts.FirstOrDefault(z => z.Shiftid == x.Shiftid).Physicianid).Firstname + ", " + _context.Physicians.FirstOrDefault(y => y.Physicianid == _context.Shifts.FirstOrDefault(z => z.Shiftid == x.Shiftid).Physicianid).Lastname,
                ShiftDate = x.Shiftdate.ToString("MMM dd, yyyy"),
                ShiftTime = x.Starttime.ToString("hh:mm tt") + " - " + x.Endtime.ToString("hh:mm tt"),
                ShiftRegion = _context.Regions.FirstOrDefault(y => y.Regionid == x.Regionid).Name,

            }).ToList();
            return reviewList;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Shift Review (Selected Approve/disapprove) Update Reocrds
        /// </summary>
        /// <param name="shiftDetailsId"></param>
        /// <param name="Aspid"></param>
        public void ApproveSelectedShift(int[] shiftDetailsId, string Aspid)
        {
            foreach (var shiftId in shiftDetailsId)
            {
                var shift = _context.Shiftdetails.FirstOrDefault(i => i.Shiftdetailid == shiftId);

                shift.Status = 2;
                shift.Modifieddate = DateTime.Now;
                shift.Modifiedby = Aspid;
            }
            _context.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Shift Review (Selected Delete) Update Reocrds
        /// </summary>
        /// <param name="shiftDetailsId"></param>
        /// <param name="Aspid"></param>
        public void DeleteShiftReview(int[] shiftDetailsId, string Aspid)
        {
            foreach (var shiftId in shiftDetailsId)
            {
                var shift = _context.Shiftdetails.FirstOrDefault(i => i.Shiftdetailid == shiftId);

                shift.Isdeleted = new BitArray(1, true);
                shift.Modifieddate = DateTime.Now;
                shift.Modifiedby = Aspid;
            }
            _context.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// MDOnCall Fetch Records
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public OnCallModal GetOnCallDetails(int regionId)
        {
            var currentTime = new TimeOnly(DateTime.Now.Hour, DateTime.Now.Minute);
            BitArray deletedBit = new BitArray(new[] { false });

            var onDutyQuery = _context.Shiftdetails
                .Include(sd => sd.Shift.Physician)
                .Where(sd => (regionId == 0 || sd.Shift.Physician.Physicianregions.Any(pr => pr.Regionid == regionId)) &&
                             sd.Shiftdate.Date == DateTime.Today &&
                             currentTime >= sd.Starttime &&
                             currentTime <= sd.Endtime &&
                             sd.Isdeleted.Equals(deletedBit))
                .Select(sd => sd.Shift.Physician)
                .Distinct()
                .ToList();

            var offDutyQuery = _context.Physicians
                .Include(p => p.Physicianregions)
                .Where(p => (regionId == 0 || p.Physicianregions.Any(pr => pr.Regionid == regionId)) &&
                            !_context.Shiftdetails.Any(sd => sd.Shift.Physicianid == p.Physicianid &&
                                                               sd.Shiftdate.Date == DateTime.Today &&
                                                               currentTime >= sd.Starttime &&
                                                               currentTime <= sd.Endtime &&
                                                               sd.Isdeleted.Equals(deletedBit)) && p.Isdeleted == null)
                .ToList();

            var onCallModal = new OnCallModal
            {
                OnCall = onDutyQuery,
                OffDuty = offDutyQuery,
                regions = GetRegions()
            };
            return onCallModal;
        }

        #endregion


        #region Provider Location

        //***************************************************************************************************************************************************
        /// <summary>
        /// Get Physicianlocation Table for Provider's Location Tab
        /// </summary>
        /// <returns></returns>
        public List<Physicianlocation> GetPhysicianlocations()
        {
            var phyLocation = _context.Physicianlocations.ToList();
            return phyLocation;
        }

        #endregion


        #region Partners

        //***************************************************************************************************************************************************
        /// <summary>
        /// Partner's Tab Fetch Records
        /// </summary>
        /// <param name="professionid"></param>
        /// <returns></returns>
        public List<Partnersdata> GetPartnersdata(int professionid)
        {
            var vendor = _context.Healthprofessionals.Where(r => r.Isdeleted == null).ToList();
            if (professionid != 0)
            {
                vendor = vendor.Where(i => i.Profession == professionid).ToList();
            }
            var Partnersdata = vendor.Select(r => new Partnersdata()
            {
                VendorId = r.Vendorid,
                VendorName = r.Vendorname,
                ProfessionName = _context.Healthprofessionaltypes.FirstOrDefault(i => i.Healthprofessionalid == r.Profession).Professionname,
                VendorEmail = r.Email,
                FaxNo = r.Faxnumber,
                PhoneNo = r.Phonenumber,
                Businesscontact = r.Businesscontact,
            }).ToList();
            return Partnersdata;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Create New Business Update Records
        /// </summary>
        /// <param name="partnersVm"></param>
        /// <param name="LoggerAspnetuserId"></param>
        /// <returns></returns>
        public bool CreateNewBusiness(PartnersVm partnersVm, string LoggerAspnetuserId)
        {
            if (!_context.Healthprofessionals.Any(x => x.Email == partnersVm.Email))
            {
                var healthprof = new Healthprofessional()
                {
                    Vendorname = partnersVm.BusinessName,
                    Profession = partnersVm.SelectedhealthprofID,
                    Faxnumber = partnersVm.FAXNumber,
                    Address = partnersVm.Street,
                    City = partnersVm.City,
                    State = _context.Regions.FirstOrDefault(i => i.Regionid == partnersVm.RegionId).Name,
                    Zip = partnersVm.Zip,
                    Regionid = partnersVm.RegionId,
                    Createddate = DateTime.Now,
                    Phonenumber = partnersVm.Phonenumber,
                    Email = partnersVm.Email,
                    Businesscontact = partnersVm.BusinessContact,
                };
                _context.Healthprofessionals.Add(healthprof);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Edit Business Fetch Records
        /// </summary>
        /// <param name="vendorID"></param>
        /// <returns></returns>
        public PartnersVm GetEditBusinessData(int vendorID)
        {
            var vendorDetails = _context.Healthprofessionals.FirstOrDefault(i => i.Vendorid == vendorID);
            var partnersVm = new PartnersVm()
            {
                BusinessName = vendorDetails.Vendorname,
                SelectedhealthprofID = vendorDetails.Profession,
                FAXNumber = vendorDetails.Faxnumber,
                Phonenumber = vendorDetails.Phonenumber,
                Email = vendorDetails.Email,
                BusinessContact = vendorDetails.Businesscontact,
                Street = vendorDetails.Address,
                City = vendorDetails.City,
                RegionId = vendorDetails.Regionid,
                Zip = vendorDetails.Zip,
            };
            return partnersVm;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Edit Business Update Records
        /// </summary>
        /// <param name="partnersCVm"></param>
        /// <returns></returns>
        public bool UpdateBusiness(PartnersVm partnersCVm)
        {
            var vendorDetails = _context.Healthprofessionals.FirstOrDefault(i => i.Vendorid == partnersCVm.vendorID);
            if (partnersCVm.BusinessName != vendorDetails.Vendorname || partnersCVm.SelectedhealthprofID != vendorDetails.Profession || partnersCVm.FAXNumber != vendorDetails.Faxnumber
            || partnersCVm.Phonenumber != vendorDetails.Phonenumber || partnersCVm.BusinessContact != vendorDetails.Businesscontact
            || partnersCVm.Street != vendorDetails.Address || partnersCVm.City != vendorDetails.City || partnersCVm.RegionId != vendorDetails.Regionid || partnersCVm.Zip != vendorDetails.Zip)
            {
                vendorDetails.Vendorname = partnersCVm.BusinessName;
                vendorDetails.Profession = partnersCVm.SelectedhealthprofID;
                vendorDetails.Faxnumber = partnersCVm.FAXNumber;
                vendorDetails.Phonenumber = partnersCVm.Phonenumber;
                vendorDetails.Businesscontact = partnersCVm.BusinessContact;
                vendorDetails.Address = partnersCVm.Street;
                vendorDetails.City = partnersCVm.City;
                vendorDetails.Regionid = partnersCVm.RegionId;
                vendorDetails.Zip = partnersCVm.Zip;
                vendorDetails.State = _context.Regions.FirstOrDefault(i => i.Regionid == partnersCVm.RegionId).Name;
                vendorDetails.Modifieddate = DateTime.Now.Date;
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Delete Business
        /// </summary>
        /// <param name="VendorID"></param>
        public void DelPartner(int VendorID)
        {
            var vendor = _context.Healthprofessionals.FirstOrDefault(x => x.Vendorid == VendorID);
            if (vendor != null)
            {
                vendor.Modifieddate = DateTime.Now;
                vendor.Isdeleted = new BitArray(1, true);
                _context.SaveChanges();
            }
        }

        #endregion


        #region Records Tab

        //***************************************************************************************************************************************************
        /// <summary>
        /// Patient History Fetch Records
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public GetRecordsModel GetRecordsTab(int UserId, GetRecordsModel model)
        {
            var usersList = _context.Users.ToList(); // Initialize as IQueryable<User>

            if (model != null)
            {
                if (model.searchRecordOne != null)
                {
                    usersList = usersList.Where(r => r.Firstname != null && r.Firstname.Trim().ToLower().Contains(model.searchRecordOne.Trim().ToLower())).ToList();
                }

                if (model.searchRecordTwo != null)
                {
                    usersList = usersList.Where(r => r.Lastname != null && r.Lastname.Trim().ToLower().Contains(model.searchRecordTwo.Trim().ToLower())).ToList();
                }

                if (model.searchRecordThree != null)
                {
                    usersList = usersList.Where(r => r.Email != null && r.Email.Trim().ToLower().Contains(model.searchRecordThree.Trim().ToLower())).ToList();
                }

                if (model.searchRecordFour != null)
                {
                    usersList = usersList.Where(r => r.Mobile != null && r.Mobile.Trim().ToLower().Contains(model.searchRecordFour.Trim().ToLower())).ToList();
                }
            }
            model.users = usersList.ToList();

            if (UserId != null && UserId != 0)
            {
                var recordList = _context.Requests.Where(i => i.Userid == UserId).ToList();
                foreach (var request in recordList)
                {
                    var requestClient = _context.Requestclients.FirstOrDefault(rc => rc.Requestid == request.Requestid);
                    if (requestClient != null)
                    {
                        request.Firstname = requestClient.Firstname;
                        request.Lastname = requestClient.Lastname;
                    }

                    var physicianList = _context.Physicians.FirstOrDefault(x => x.Physicianid == request.Physicianid);
                    if (physicianList != null)
                    {
                        request.Physician.Firstname = physicianList.Firstname;
                        request.Physician.Lastname = physicianList.Lastname;
                    }
                }
                model.requestList = recordList;
            }
            return model;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Block History Fetch Records
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BlockedRequestModel GetBlockedRequest(BlockedRequestModel model)
        {
            var blockrequest = _context.Blockrequests.Where(i => i.Isactive == new BitArray(1, true)).ToList();

            var recordList = new List<blockedRequest>();
            foreach (var m in blockrequest)
            {
                var request = _context.Requestclients.Where(i => i.Requestid == m.Requestid);
                bool check;
                if (m.Isactive != null && m.Isactive.Length > 0)
                {
                    check = m.Isactive[0];
                }
                else
                {
                    check = false;
                }

                var newRecord = new blockedRequest
                {
                    patientname = request.Select(i => i.Firstname).FirstOrDefault() + " " + request.Select(i => i.Lastname).FirstOrDefault(),
                    contact = m.Phonenumber,
                    email = m.Email,
                    requestid = m.Requestid,
                    createddate = m.Createddate,
                    notes = m.Reason,
                    isactive = check,
                };
                recordList.Add(newRecord);
            }
            if (model != null)
            {
                if (model.searchRecordOne != null)
                {
                    recordList = recordList.Where(r => r.patientname != null && r.patientname.Trim().ToLower().Contains(model.searchRecordOne.Trim().ToLower())).ToList();
                }

                if (model.searchRecordTwo != null)
                {
                    recordList = recordList.Where(r => r.patientname != null && r.patientname.Trim().ToLower().Contains(model.searchRecordTwo.Trim().ToLower())).ToList();
                }

                if (model.searchRecordThree != null)
                {
                    recordList = recordList.Where(r => r.email != null && r.email.Trim().ToLower().Contains(model.searchRecordThree.Trim().ToLower())).ToList();
                }

                if (model.searchRecordFour != null)
                {
                    recordList = recordList.Where(r => r.contact != null && r.contact.Trim().ToLower().Contains(model.searchRecordFour.Trim().ToLower())).ToList();
                }
            }
            model.blockrequestList = recordList;
            return model;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// unblock History Update Records
        /// </summary>
        /// <param name="requestId"></param>
        public void UnblockRequest(int requestId)
        {
            var blockrequest = _context.Blockrequests.Where(i => i.Requestid == requestId).FirstOrDefault();
            var request = _context.Requests.Where(i => i.Requestid == requestId).FirstOrDefault();
            if (blockrequest != null && request != null)
            {
                request.Status = 1;
                blockrequest.Isactive = new BitArray(1, false);
            }
            _context.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// EmailLog Fetch Records
        /// </summary>
        /// <param name="tempId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public EmailSmsLogModel GetEmailSmsLog(int tempId, EmailSmsLogModel model)
        {
            model.tempid = tempId;
            var recordList = new List<emailSmsRecords>();

            if (tempId == 0)
            {
                var records = _context.Emaillogs.ToList();
                foreach (var item in records)
                {
                    var newRecord = new emailSmsRecords
                    {
                        roleid = item.Roleid,
                        email = item.Emailid,
                        createddate = item.Createdate.ToString("yyyy-MM-dd"),
                        sentdate = item.Sentdate?.ToString("yyyy-MM-dd"),
                        sent = item.Isemailsent[0] ? "Yes" : "No",
                        recipient = item.Emailid.Substring(0, item.Emailid.IndexOf("@")),
                        rolename = item.Roleid == 1 ? "Admin" : item.Roleid == 2 ? "Physician" : item.Roleid == 3 ? "Patient" : item.Roleid == 0 ? "Unknown" : null,
                        senttries = item.Senttries,
                        confirmationNumber = item.Confirmationnumber != null ? item.Confirmationnumber : "-",
                        action = item.Action != null ? item.Action.ToString() : "-",
                    };
                    recordList.Add(newRecord);
                }
                if (model != null)
                {
                    if (model.searchRecordOne != null && model.searchRecordOne != 0)
                    {
                        recordList = recordList.Where(r => r.roleid == model.searchRecordOne).Select(r => r).ToList();
                    }

                    if (model.searchRecordTwo != null)
                    {
                        recordList = recordList.Where(r => r.recipient != null && r.recipient.Trim().ToLower().Contains(model.searchRecordTwo.Trim().ToLower())).Select(r => r).ToList();
                    }

                    if (model.searchRecordThree != null)
                    {
                        recordList = recordList.Where(r => r.email.Trim().ToLower().Contains(model.searchRecordThree.Trim().ToLower())).Select(r => r).ToList();
                    }

                    if (model.searchRecordFour != null)
                    {
                        recordList = recordList.Where(r => r.createddate.Equals(model.searchRecordFour?.ToString("yyyy-MM-dd"))).Select(r => r).ToList();
                    }

                    if (model.searchRecordFive != null)
                    {

                        recordList = recordList.Where(r => r.sentdate.Equals(model.searchRecordFive?.ToString("yyyy-MM-dd"))).Select(r => r).ToList();
                    }
                }
                model.recordslist = recordList;
            }
            else
            {
                var records = _context.Smslogs.ToList();
                foreach (var item in records)
                {
                    var newRecord = new emailSmsRecords
                    {
                        contact = item.Mobilenumber,
                        createddate = item.Createdate.ToString("yyyy-MM-dd"),
                        sentdate = item.Sentdate?.ToString("yyyy-MM-dd"),
                        sent = item.Issmssent[0] ? "Yes" : "No",
                        recipient = item.Requestid == null ? _context.Physicians.Where(i => i.Physicianid == item.Physicianid).Select(i => i.Firstname).FirstOrDefault() : _context.Requestclients.Where(i => i.Requestid == item.Requestid).Select(i => i.Firstname).FirstOrDefault(),
                        rolename = item.Roleid == 1 ? "Admin" : item.Roleid == 2 ? "Physician" : item.Roleid == 3 ? "Patient" : item.Roleid == 0 ? "Unknown" : null,
                        senttries = item.Senttries,
                        confirmationNumber = item.Confirmationnumber,
                    };
                    recordList.Add(newRecord);
                }
                if (model != null)
                {
                    if (model.searchRecordOne != null && model.searchRecordOne != 0)
                    {
                        recordList = recordList.Where(r => r.roleid == model.searchRecordOne).Select(r => r).ToList();
                    }

                    if (model.searchRecordTwo != null)
                    {
                        recordList = recordList.Where(r => r.recipient.Trim().ToLower().Contains(model.searchRecordTwo.Trim().ToLower())).Select(r => r).ToList();
                    }

                    if (model.searchRecordThree != null)
                    {
                        recordList = recordList.Where(r => r.contact.Trim().ToLower().Contains(model.searchRecordThree.Trim().ToLower())).Select(r => r).ToList();
                    }

                    if (model.searchRecordFour != null)
                    {
                        recordList = recordList.Where(r => r.createddate.Equals(model.searchRecordFour?.ToString("yyyy-MM-dd"))).Select(r => r).ToList();
                    }

                    if (model.searchRecordFive != null)
                    {

                        recordList = recordList.Where(r => r.sentdate.Equals(model.searchRecordFive?.ToString("yyyy-MM-dd"))).Select(r => r).ToList();
                    }
                }
                model.recordslist = recordList;
            }
            return model;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Search Records Fetch Records
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public SearchRecordsModel GetSearchRecords(SearchRecordsModel model)
        {
            var requestQuery = _context.Requests.Where(i => i.Isdeleted != new BitArray(1, true)).ToList();
            var recordList = new List<requests>();

            foreach (var request in requestQuery)
            {
                var requestClient = _context.Requestclients.Where(i => i.Requestid == request.Requestid).FirstOrDefault();
                var physician = _context.Physicians.Where(i => i.Physicianid == request.Physicianid).FirstOrDefault();
                var requestNotes = _context.Requestnotes.Where(i => i.Requestid == request.Requestid).FirstOrDefault();

                var newRequest = new requests
                {
                    patientname = (requestClient?.Firstname ?? "") + " " + (requestClient?.Lastname ?? ""),
                    requestor = request.Firstname + " " + request.Lastname,
                    dateOfService = null,
                    closeCaseDate = null,
                    email = requestClient?.Email,
                    contact = requestClient?.Phonenumber,
                    address = requestClient?.Address,
                    zip = requestClient?.Zipcode,
                    status = (int)request.Status,
                    physician = (physician?.Firstname ?? "") + " " + (physician?.Lastname ?? ""),
                    physicianNote = requestNotes?.Physiciannotes,
                    providerNote = null,
                    AdminNote = requestNotes?.Adminnotes,
                    pateintNote = requestClient?.Notes,
                    requestid = request.Requestid,
                    userid = request.Userid,
                    requestTypeId = request.Requesttypeid,
                };
                recordList.Add(newRequest);
            }
            if (model != null)
            {
                if (model.searchRecordOne != null && model.searchRecordOne != 0)
                {
                    recordList = recordList.Where(r => r.status == model.searchRecordOne).Select(r => r).ToList();
                }

                if (model.searchRecordTwo != null)
                {
                    recordList = recordList.Where(r => r.patientname.Trim().ToLower().Contains(model.searchRecordTwo.Trim().ToLower())).Select(r => r).ToList();
                }

                if (model.searchRecordThree != null && model.searchRecordThree != 0)
                {
                    recordList = recordList.Where(r => r.requestTypeId == model.searchRecordThree).Select(r => r).ToList();
                }

                if (model.searchRecordSix != null)
                {
                    recordList = recordList.Where(r => r.physician != null && r.physician.Trim().ToLower().Contains(model.searchRecordSix.Trim().ToLower())).Select(r => r).ToList();
                }

                if (model.searchRecordSeven != null)
                {
                    recordList = recordList.Where(r => r.email != null && r.email.Trim().ToLower().Contains(model.searchRecordSeven.Trim().ToLower())).Select(r => r).ToList();
                }

                if (model.searchRecordEight != null)
                {
                    recordList = recordList.Where(r => r.contact != null && r.contact.Trim().ToLower().Contains(model.searchRecordEight.Trim().ToLower())).Select(r => r).ToList();
                }
            }
            model.requestList = recordList;
            return model;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Getc Requests type Table Fetch Records
        /// </summary>
        /// <returns></returns>
        public List<Requesttype> GetRequesttypes()
        {
            return _context.Requesttypes.ToList();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// delete Request Update Records
        /// </summary>
        /// <param name="requestId"></param>
        public void deletRequest(int requestId)
        {
            var request = _context.Requests.Where(i => i.Requestid == requestId).FirstOrDefault();

            if (request != null)
            {
                request.Isdeleted = new BitArray(1, true);
                request.Modifieddate = DateTime.Now;
                _context.SaveChanges();
            }
        }

        #endregion


        #region Pay Rate

        //***************************************************************************************************************************************************
        /// <summary>
        /// Enter Payrate Fetch Records
        /// </summary>
        /// <param name="phyid"></param>
        /// <param name="adminAspId"></param>
        /// <returns></returns>
        public List<PayRateForProviderVm> GetPayRateForProvider(int phyid, string adminAspId)
        {
            List<PayRateForProviderVm> dataList = new List<PayRateForProviderVm>();

            var payrate = _context.PayrateByProviders.Where(i => i.PhysicianId == phyid).OrderBy(i => i.PayrateCategoryId).ToList();

            if (payrate.Count() == 0)
            {
                for (int i = 1; i <= 7; i++)
                {
                    var data = new PayrateByProvider()
                    {
                        PayrateCategoryId = i,
                        PhysicianId = phyid,
                        Payrate = 0,
                        CreatedBy = adminAspId,
                    };
                    _context.PayrateByProviders.Add(data);

                    dataList.Add(new PayRateForProviderVm
                    {
                        Categoryid = data.PayrateCategoryId,
                        PayrateValue = (int?)data.Payrate,
                        Categoryname = _context.PayrateCategories.FirstOrDefault(x => x.PayrateCategoryId == data.PayrateCategoryId)!.CategoryName
                    });
                }
                _context.SaveChanges();
                return dataList;
            }
            else
            {
                var data = payrate.Select(i => new PayRateForProviderVm
                {
                    Categoryid = i.PayrateCategoryId,
                    PayrateValue = (int?)i.Payrate,
                    Categoryname = _context.PayrateCategories.FirstOrDefault(x => x.PayrateCategoryId == i.PayrateCategoryId)!.CategoryName,
                }).ToList();
                return data;
            }

        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Enter Payrate Update Records
        /// </summary>
        /// <param name="category"></param>
        /// <param name="payrate"></param>
        /// <param name="phyid"></param>
        /// <param name="adminAspId"></param>
        /// <returns></returns>
        public bool PostPayrate(int category, int payrate, int phyid, string adminAspId)
        {
            try
            {
                var categoryData = _context.PayrateByProviders.FirstOrDefault(i => i.PhysicianId == phyid && i.PayrateCategoryId == category);

                if (categoryData != null)
                {
                    categoryData.Payrate = payrate;
                }
                else
                {
                    var catData = new PayrateByProvider()
                    {
                        PayrateCategoryId = category,
                        PhysicianId = phyid,
                        Payrate = payrate,
                        CreatedBy = adminAspId,
                    };
                    _context.PayrateByProviders.Add(catData);
                }
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion


        #region Invoicing

        //***************************************************************************************************************************************************
        /// <summary>
        /// Admin Invocing (Filtered) Fetch Records
        /// </summary>
        /// <param name="phyid"></param>
        /// <param name="dateSelected"></param>
        /// <returns></returns>
        public List<Timesheet> GetTimeSheetDetail(int phyid, string dateSelected)
        {
            var timesheetData = _context.Timesheets.ToList();

            if (phyid != null)
            {
                timesheetData = timesheetData.Where(i => i.PhysicianId == phyid).ToList();
            }

            if (dateSelected != null)
            {
                var splitedDate = dateSelected.Split('-');
                var currentYear = DateTime.Now.Year;
                var daysInMonth = DateTime.DaysInMonth(currentYear, Convert.ToInt32(splitedDate[0]));
                if (splitedDate[0].Length == 1)
                {
                    splitedDate[0] = "0" + splitedDate[0];
                }
                if (splitedDate[1] == "1")
                {
                    var startDate = "01" + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    var endDate = 15 + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    timesheetData = timesheetData.Where(i => i.StartDate.ToString() == startDate && i.EndDate.ToString() == endDate).ToList();
                }
                else if (splitedDate[1] == "2")
                {
                    var startDate = 16 + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    var endDate = daysInMonth.ToString() + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    timesheetData = timesheetData.Where(i => i.StartDate.ToString() == startDate && i.EndDate.ToString() == endDate).ToList();
                }
            }
            return timesheetData;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Get Payrate Table Fetch Records
        /// </summary>
        /// <param name="phyid"></param>
        /// <returns></returns>
        public List<PayrateByProvider> GetPayRateForProviderByPhyId(int phyid)
        {
            return _context.PayrateByProviders.Where(i => i.PhysicianId == phyid).ToList();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Approve Timesheet Update Records
        /// </summary>
        /// <param name="timeSheetId"></param>
        /// <param name="bonus"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        public bool ApproveTimeSheet(int timeSheetId, int bonus, string notes)
        {
            try
            {
                var timeSheetData = _context.Timesheets.FirstOrDefault(x => x.TimesheetId == timeSheetId);

                timeSheetData.IsApproved = true;
                timeSheetData.ModifiedDate = DateTime.Now;

                if (bonus != null)
                {
                    timeSheetData.BonusAmount = bonus.ToString();
                }
                if (notes != null)
                {
                    timeSheetData.AdminNotes = notes;
                }
                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion


        #region ChatWith

        //***************************************************************************************************************************************************
        /// <summary>
        /// ChatWith Fetch Records
        /// </summary>
        /// <param name="Reqid"></param>
        /// <param name="Senderid"></param>
        /// <param name="ReciverAspid"></param>
        /// <returns></returns>
        public ChatVm GetChatData(int Reqid, string Senderid, string ReciverAspid)
        {
            var aspData = _context.Aspnetusers.Include(x => x.Users).Include(x => x.Aspnetuserrole).Include(x => x.Admins).Include(x => x.PhysicianAspnetusers).FirstOrDefault(x => x.Id == ReciverAspid);

            var chathistory = _context.ChatHistories.Where(i => ((i.SenderAspId == Senderid.ToString() && i.ReceiverAspId == ReciverAspid.ToString()) || (i.SenderAspId == ReciverAspid.ToString() && i.ReceiverAspId == Senderid.ToString())) && i.RequestId == Reqid).OrderBy(i => i.Id).ToList();

            if (aspData != null)
            {
                ChatVm chatData = new ChatVm();
                chatData.AspId = ReciverAspid;
                //chatData.ReceiverName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(aspData.Email.Split('@')[0]);
                if (aspData.Aspnetuserrole.Roleid == "3")
                {
                    chatData.ReceiverName = aspData.Users.FirstOrDefault().Firstname + " " + aspData.Users.FirstOrDefault().Lastname;
                }
                else if (aspData.Aspnetuserrole.Roleid == "1")
                {
                    chatData.ReceiverName = aspData.Admins.FirstOrDefault().Firstname + " " + aspData.Admins.FirstOrDefault().Lastname;
                }
                else
                {
                    chatData.ReceiverName = aspData.PhysicianAspnetusers.FirstOrDefault().Firstname + " " + aspData.PhysicianAspnetusers.FirstOrDefault().Lastname;
                }
                chatData.chatHistories = chathistory;
                return chatData;
            }
            return null;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// ChatWith Update Records
        /// </summary>
        /// <param name="Reqid"></param>
        /// <param name="senderId"></param>
        /// <param name="Receiverid"></param>
        /// <param name="Message"></param>
        public void AddChatHistory(int Reqid, string senderId, string Receiverid, string Message)
        {
            if (!Message.IsNullOrEmpty())
            {
                var chat = new ChatHistory()
                {
                    RequestId = Reqid,
                    SenderAspId = senderId.ToString(),
                    ReceiverAspId = Receiverid.ToString(),
                    Message = Message,
                    Time = TimeOnly.FromDateTime(DateTime.Now),
                };
                _context.ChatHistories.Add(chat);
                _context.SaveChanges();
            }
        }

        #endregion

        //***************************************************************************************************************************************************
        //***************************************************************************************************************************************************
        //***************************************************************************************************************************************************
    }
}