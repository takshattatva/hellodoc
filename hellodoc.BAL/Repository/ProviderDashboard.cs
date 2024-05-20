using hellodoc.BAL.Interface;
using hellodoc.DAL.ViewModels;
using hellodoc.DAL.Models;
using System.Collections;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.BAL.Repository
{
    public class ProviderDashboard : IProviderDashboard
    {
        private readonly HellodocDbContext _context;

        public ProviderDashboard(HellodocDbContext context)
        {
            _context = context;
        }

        #region Provider Dashboard

        //***************************************************************************************************************************************************
        /// <summary>
        /// Count data based on status 
        /// </summary>
        /// <param name="physicianId"></param>
        /// <returns></returns>
        public CountRequest GetCountRequest(int physicianId)
        {
            var request = _context.Requests;

            var countData = new CountRequest()
            {
                NewRequest = request.Where(i => i.Status == 1 && i.Physicianid == physicianId).Count(),
                PendingRequest = request.Where(i => i.Status == 2 && i.Physicianid == physicianId).Count(),
                ActiveRequest = request.Where(i => (i.Status == 4 || i.Status == 5) && i.Physicianid == physicianId).Count(),
                ConcludeRequest = request.Where(i => i.Status == 6 && i.Physicianid == physicianId).Count()
            };
            return countData;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Table Data Fetch Records
        /// </summary>
        /// <param name="Status"></param>
        /// <param name="requesttypeid"></param>
        /// <param name="physicinId"></param>
        /// <returns></returns>
        public List<RequestListAdminDash> getRequestData(int[] Status, string requesttypeid, int physicinId)
        {
            var requestList = _context.Requests.Where(i => Status.Contains(i.Status) && i.Physicianid == physicinId);

            if (requesttypeid != null)
            {
                requestList = requestList.Where((i => i.Requesttypeid.ToString() == requesttypeid));
            }

            var tableData = requestList.Select(r => new RequestListAdminDash()
            {
                Name = r.Requestclients.Select(x => x.Firstname).First() + " " + r.Requestclients.Select(x => x.Lastname).First(),
                Email = r.Requestclients.Select(x => x.Email).First(),
                Mobile = r.Phonenumber == null ? "-" : r.Phonenumber,
                Phone = r.Requestclients.Select(x => x.Phonenumber).First(),
                Address = r.Requestclients.Select(x => x.Street).First() + ", " + r.Requestclients.Select(x => x.City).First() + ", " + r.Requestclients.Select(x => x.State).First(),
                DateOfBirth = r.Requestclients.Select(x => x.Intdate).First() == null ? null : new DateTime?(new DateTime(r.Requestclients.Select(x => (int)x.Intyear).First(), r.Requestclients.Select(x => int.Parse(x.Strmonth)).First(), r.Requestclients.Select(x => (int)x.Intdate).First())),
                RequestDate = r.Createddate.ToShortDateString(),
                RequestTypeId = r.Requesttypeid,
                RequestId = r.Requestid,
                callType = r.Calltype == null ? 0 : (int)r.Calltype,
                isFinalized = r.Encounters.Select(x => x.IsFinalized).First() == new BitArray(1, true),
                PhysicianId = r.Physician.Physicianid,
                PatientAspId = r.User.Aspnetuserid,
                UserId = r.Userid,
                AdminAspId = "3bdd2ffd-041a-46b9-984c-24e7b01f8fd4",

            }).ToList();
            return tableData;
        }

        #endregion


        #region Accept Case

        //***************************************************************************************************************************************************
        /// <summary>
        /// Accept Case Update Records
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="physicianId"></param>
        public void SetAcceptCaseData(int requestId, int physicianId)
        {
            var request = _context.Requests.FirstOrDefault(i => i.Requestid == requestId);

            var physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == physicianId);

            string acceptNote = "Case is Accepted By " + physician.Firstname + " " + physician.Lastname + " on " + DateTime.Now.ToString("MMM dd, yyyy") + " at " + DateTime.Now.ToString("hh:mm tt");

            var acceptCase = new Requeststatuslog()
            {
                Requestid = requestId,
                Status = 2,
                Physicianid = physicianId,
                Createddate = DateTime.Now,
                Notes = acceptNote,
            };
            _context.Requeststatuslogs.Add(acceptCase);

            request.Status = 2;
            request.Modifieddate = DateTime.Now;
            _context.SaveChanges();
        }

        #endregion


        #region Transfer Case

        //***************************************************************************************************************************************************
        /// <summary>
        /// Transfer Case Update Records
        /// </summary>
        /// <param name="transferCaseModal"></param>
        public void TransferCaseData(TransferCaseModal transferCaseModal)
        {
            var request = _context.Requests.FirstOrDefault(i => i.Requestid == transferCaseModal.RequestId);

            var physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == transferCaseModal.PhysicianId);

            string transferNote = physician.Firstname + " Transferred to Admin on " + DateTime.Now.ToString("MMM dd, yyyy") + " at " + DateTime.Now.ToString("hh:mm tt") + " : " + transferCaseModal.TransferNotes;

            var transferData = new Requeststatuslog()
            {
                Requestid = transferCaseModal.RequestId,
                Status = 1,
                Createddate = DateTime.Now,
                Transtoadmin = new BitArray(1, true),
                Notes = transferNote,
                Physicianid = request.Physicianid,
            };
            request.Status = 1;
            request.Physicianid = null;
            _context.Requeststatuslogs.Add(transferData);
            _context.SaveChanges();
        }

        #endregion


        #region Encounter Case

        //***************************************************************************************************************************************************
        /// <summary>
        /// House Call Modal Update Records
        /// </summary>
        /// <param name="requestId"></param>
        public void HouseCallConclude(int requestId)
        {
            var requestData = _context.Requests.FirstOrDefault(i => i.Requestid == requestId);

            if (requestData != null)
            {
                requestData.Status = 6;
                requestData.Modifieddate = DateTime.Now;
                _context.SaveChanges();

                var physician = _context.Physicians.FirstOrDefault(i => i.Physicianid == requestData.Physicianid);

                string houseCallNote = physician.Firstname + " House Called care on " + DateTime.Now.ToString("MMM dd, yyyy") + " at " + DateTime.Now.ToString("hh:mm tt");

                var statusData = new Requeststatuslog()
                {
                    Requestid = requestData.Requestid,
                    Status = 6,
                    Physicianid = requestData.Physicianid,
                    Createddate = DateTime.Now,
                    Notes = houseCallNote,
                };
                _context.Requeststatuslogs.Add(statusData);
                _context.SaveChanges();
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Encounter Modal Update Records
        /// </summary>
        /// <param name="encounterVm"></param>
        public void SetEncounterCareType(EncounterVm encounterVm)
        {
            var request = _context.Requests.FirstOrDefault(i => i.Requestid == encounterVm.reqid);

            if (request != null)
            {
                var physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == encounterVm.physicianId);
                var care = "";

                if (encounterVm.Option == 1)
                {
                    care = "HOUSE CALL";
                    request.Calltype = 1;
                    request.Status = 5;
                }
                else if (encounterVm.Option == 2)
                {
                    care = "CONSULT";
                    request.Calltype = 2;
                    request.Status = 6;
                }
                request.Modifieddate = DateTime.Now;
                _context.SaveChanges();

                string encounterNote = physician.Firstname + " Selected " + care + " on " + DateTime.Now.ToString("MMM dd, yyyy") + " at " + DateTime.Now.ToString("hh:mm tt");

                var transferData = new Requeststatuslog()
                {
                    Requestid = encounterVm.reqid,
                    Status = request.Status,
                    Createddate = DateTime.Now,
                    Notes = encounterNote,
                    Physicianid = encounterVm.physicianId,
                };
                _context.Requeststatuslogs.Add(transferData);
                _context.SaveChanges();
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Encounter Form Finalize Update Records
        /// </summary>
        /// <param name="requestId"></param>
        public void FinalizeEncounterCase(int requestId)
        {
            var encounter = _context.Encounters.FirstOrDefault(i => i.RequestId == requestId);

            if (encounter != null)
            {
                encounter.IsFinalized = new BitArray(1, true);
                _context.SaveChanges();
            }
            else
            {
                Encounter enc = new Encounter();
                enc.RequestId = requestId;
                enc.IsFinalized = new BitArray(1, true);
                _context.Encounters.Add(enc);
                _context.SaveChanges();
            }
        }

        #endregion


        #region Conclude Case

        //***************************************************************************************************************************************************
        /// <summary>
        /// Conclude Case Fetch Records ( data )
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public AdminViewDocumentsVm GetViewDocumentsData(int requestid)
        {
            var patient = _context.Requestclients.FirstOrDefault(i => i.Requestid == requestid);

            string confirmation = _context.Requests.FirstOrDefault(i => i.Requestid == requestid).Confirmationnumber;
            var userId = _context.Requests.FirstOrDefault(i => i.Requestid == requestid).Userid;

            var casedata = _context.Requestclients.FirstOrDefault(i => i.Requestid == requestid);
            var requestList = _context.Requests.Where(i => i.Requestid == requestid);

            var documentData = new AdminViewDocumentsVm()
            {
                UserId = userId == null ? 0 : (int)userId,
                requestId = requestid,
                patientName = patient.Firstname + " " + patient.Lastname,
                ConfirmationNumber = confirmation,
            };
            return documentData;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Conclude Case Fetch Records ( docs )
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public List<ViewDocuments> GetViewDocumentsList(int requestid)
        {
            var document = _context.Requestwisefiles.Where(i => i.Requestid == requestid && i.Isdeleted != new BitArray(1, true));

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
        /// Conclude Case Update Records ( upload file )
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
                Createddate = DateTime.Now,
            };
            _context.Add(fileData);
            _context.SaveChanges();
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Conclude Case Update Records ( conclude the case )
        /// </summary>
        /// <param name="adminViewDocumentsVm"></param>
        public void ConfirmConcludeCare(AdminViewDocumentsVm adminViewDocumentsVm)
        {
            var request = _context.Requests.FirstOrDefault(i => i.Requestid == adminViewDocumentsVm.requestId);

            if (request != null)
            {
                request.Status = 8;
                request.Modifieddate = DateTime.Now;
                request.Completedbyphysician = new BitArray(1, true);
                _context.SaveChanges();

                var physician = _context.Physicians.FirstOrDefault(i => i.Physicianid == request.Physicianid);

                var noteData = new Requestnote()
                {
                    Requestid = request.Requestid,
                    Physiciannotes = adminViewDocumentsVm.ProviderNote,
                    Createdby = physician.Aspnetuserid,
                    Createddate = DateTime.Now,
                };
                _context.Requestnotes.Add(noteData);
                _context.SaveChanges();

                string concludeNote = physician.Firstname + " Concluded care on " + DateTime.Now.ToString("MMM dd, yyyy") + " at " + DateTime.Now.ToString("hh:mm tt");

                var statusData = new Requeststatuslog()
                {
                    Requestid = request.Requestid,
                    Status = 8,
                    Physicianid = request.Physicianid,
                    Createddate = DateTime.Now,
                    Notes = concludeNote,
                };
                _context.Requeststatuslogs.Add(statusData);
                _context.SaveChanges();
            }
        }

        #endregion


        #region Request To Admin

        //***************************************************************************************************************************************************
        /// <summary>
        /// Request to admin Update Records
        /// </summary>
        /// <param name="providerProfileVm"></param>
        public void RequestForEdit(ProviderProfileVm providerProfileVm)
        {
            var physician = _context.Physicians.FirstOrDefault(i => i.Physicianid == providerProfileVm.PhysicianId);

            if (physician != null)
            {
                var aspData = _context.Aspnetusers.FirstOrDefault(i => i.Id == physician.Createdby);

                if (aspData != null)
                {
                    var mail = "tatva.dotnet.takshgadhiya@outlook.com";
                    var password = "12!@Taksh";
                    var email = aspData.Email;
                    var subject = "Request For Edit";
                    var message = $"Hey <b>{aspData?.Username}(Admin)</b>, We trust this message finds you in good spirits. <br><br> Message : " + providerProfileVm.RequestMessage + "<br> Request For : " + physician.Email;

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
                        Roleid = 1,
                        Createdate = DateTime.Now,
                        Sentdate = DateTime.Now,
                        Isemailsent = new BitArray(1, true),
                        Senttries = 1,
                        Physicianid = physician.Physicianid
                    };
                    _context.Emaillogs.Add(emailLog);
                    _context.SaveChanges();
                }
            }
        }

        #endregion


        #region Invoicing

        //***************************************************************************************************************************************************
        /// <summary>
        /// Provider Invoicing Fetch Records
        /// </summary>
        /// <param name="phyid"></param>
        /// <param name="dateSelected"></param>
        /// <returns></returns>
        public List<TimesheetDetail> GetTimeSheetDetails(int phyid, string dateSelected)
        {
            var data = _context.TimesheetDetails.Include(i => i.Timesheet).Where(i => i.Timesheet.PhysicianId == phyid).OrderBy(i => i.TimesheetDetailId).ToList();

            if (dateSelected == null)
            {
                var currentMonth = DateTime.Now.Month.ToString();
                var currentYear = DateTime.Now.Year;
                if (currentMonth.Length == 1)
                {
                    currentMonth = "0" + currentMonth;
                }
                var startDate = "01" + "-" + currentMonth + "-" + currentYear.ToString();
                var endDate = 15 + "-" + currentMonth + "-" + currentYear.ToString();

                data = data.Where(i => i.Timesheet.StartDate.ToString() == startDate && i.Timesheet.EndDate.ToString() == endDate).ToList();
            }
            if (data.Count > 0 && dateSelected != null)
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
                    data = data.Where(i => i.Timesheet.StartDate.ToString() == startDate && i.Timesheet.EndDate.ToString() == endDate).ToList();
                }
                else if (splitedDate[1] == "2")
                {
                    var startDate = 16 + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    var endDate = daysInMonth.ToString() + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    data = data.Where(i => i.Timesheet.StartDate.ToString() == startDate && i.Timesheet.EndDate.ToString() == endDate).ToList();
                }
            }
            return data;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Provider Invoicing Fetch Records
        /// </summary>
        /// <param name="phyid"></param>
        /// <param name="dateSelected"></param>
        /// <returns></returns>
        public List<TimesheetDetailReimbursement> GetTimeSheetDetailsReimbursements(int phyid, string dateSelected)
        {
            var data = _context.TimesheetDetailReimbursements.Include(i => i.TimesheetDetail.Timesheet).Where(i => i.TimesheetDetail.Timesheet.PhysicianId == phyid && i.IsDeleted != true).OrderBy(i => i.TimesheetDetailId).ToList();

            if (dateSelected == null)
            {
                var currentMonth = DateTime.Now.Month.ToString();
                var currentYear = DateTime.Now.Year;
                if (currentMonth.Length == 1)
                {
                    currentMonth = "0" + currentMonth;
                }
                var startDate = "01" + "-" + currentMonth + "-" + currentYear.ToString();
                var endDate = 15 + "-" + currentMonth + "-" + currentYear.ToString();

                data = data.Where(i => i.TimesheetDetail.Timesheet.StartDate.ToString() == startDate && i.TimesheetDetail.Timesheet.EndDate.ToString() == endDate).ToList();
            }
            if (data.Count > 0 && dateSelected != null)
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
                    data = data.Where(i => i.TimesheetDetail.Timesheet.StartDate.ToString() == startDate && i.TimesheetDetail.Timesheet.EndDate.ToString() == endDate).ToList();
                }
                else if (splitedDate[1] == "2")
                {
                    var startDate = 16 + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    var endDate = daysInMonth.ToString() + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    data = data.Where(i => i.TimesheetDetail.Timesheet.StartDate.ToString() == startDate && i.TimesheetDetail.Timesheet.EndDate.ToString() == endDate).ToList();
                }
            }
            return data;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Finalize Timesheet Fetch Records
        /// </summary>
        /// <param name="phyid"></param>
        /// <param name="dateSelected"></param>
        /// <returns></returns>
        public List<ProviderTimesheetDetails> GetFinalizeTimeSheetDetails(int phyid, string dateSelected)
        {
            var currentYear = DateTime.Now.Year;
            var data = _context.TimesheetDetails
                                .Include(i => i.Timesheet)
                                .Where(i => i.Timesheet.PhysicianId == phyid)
                                .OrderBy(i => i.TimesheetDetailId)
                                .ToList();

            if (!string.IsNullOrEmpty(dateSelected))
            {
                var splitedDate = dateSelected.Split('-');
                var month = Convert.ToInt32(splitedDate[0]);
                var daysInMonth = DateTime.DaysInMonth(currentYear, month);
                var startDate = splitedDate[1] == "1" ? new DateOnly(currentYear, month, 1) : new DateOnly(currentYear, month, 16);
                var endDate = splitedDate[1] == "1" ? new DateOnly(currentYear, month, 15) : new DateOnly(currentYear, month, daysInMonth);

                data = data.Where(i => i.Timesheet.StartDate == startDate && i.Timesheet.EndDate == endDate).ToList();

                if (data.Count == 0)
                {
                    var newShift = new Timesheet
                    {
                        PhysicianId = phyid,
                        StartDate = startDate,
                        EndDate = endDate,
                        IsFinalize = false,
                        IsApproved = false,
                        CreatedBy = _context.Physicians.FirstOrDefault(i => i.Physicianid == phyid).Aspnetuserid,
                    };
                    _context.Timesheets.Add(newShift);
                    _context.SaveChanges();

                    for (int i = splitedDate[1] == "1" ? 1 : 16; i <= (splitedDate[1] == "1" ? 15 : daysInMonth); i++)
                    {
                        var newShiftData = new TimesheetDetail
                        {
                            Timesheet = newShift,
                            TimesheetDate = new DateOnly(currentYear, month, i),
                        };
                        _context.TimesheetDetails.Add(newShiftData);
                    }
                    _context.SaveChanges();

                    data = _context.TimesheetDetails
                                   .Include(i => i.Timesheet)
                                   .Where(i => i.Timesheet.PhysicianId == phyid &&
                                               i.Timesheet.StartDate == startDate &&
                                               i.Timesheet.EndDate == endDate)
                                   .OrderBy(i => i.TimesheetDetailId)
                                   .ToList();
                }
            }

            var timeSheetData = data.Select(i => new ProviderTimesheetDetails
            {
                TimeSheetId = i.TimesheetId,
                TimeSheetDetailId = i.TimesheetDetailId,
                ShiftDetailDate = i.TimesheetDate,
                Hours = (int?)i.TotalHours,
                NoOfConsults = i.NumberOfPhoneCall,
                NoOfHouseCalls = i.NumberOfHouseCall,
                IsWeekend = i.IsWeekend,
            }).ToList();
            return timeSheetData;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Finalize Timesheet Update Records
        /// </summary>
        /// <param name="providerTimesheetDetails"></param>
        /// <returns></returns>
        public bool PostFinalizeTimesheet(List<ProviderTimesheetDetails> providerTimesheetDetails)
        {
            try
            {
                if (providerTimesheetDetails.Count > 0)
                {

                    for (int i = 0; i < providerTimesheetDetails.Count; i++)
                    {
                        var data = _context.TimesheetDetails.FirstOrDefault(x => x.TimesheetDetailId == providerTimesheetDetails[i].TimeSheetDetailId);

                        if (data != null)
                        {
                            data.TotalHours = providerTimesheetDetails[i].Hours;
                            data.NumberOfHouseCall = providerTimesheetDetails[i].NoOfHouseCalls;
                            data.NumberOfPhoneCall = providerTimesheetDetails[i].NoOfConsults;
                            data.IsWeekend = providerTimesheetDetails[i].IsWeekend;
                        }
                    }
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Add Receipt Fetch Records
        /// </summary>
        /// <param name="timeSheetDetailId"></param>
        /// <param name="AspId"></param>
        /// <returns></returns>
        public List<AddReceiptsDetails> GetAddReceiptsDetails(int[] timeSheetDetailId, string AspId)
        {
            var data = _context.TimesheetDetailReimbursements
                                .Where(x => timeSheetDetailId.Contains(x.TimesheetDetailId) && x.IsDeleted!= true)
                                .OrderBy(x => x.TimesheetDetailReimbursementId)
                                .ToList();

            if (data.Count == 0)
            {
                for (int i = 0; i < timeSheetDetailId.Length; i++)
                {
                    var newReimbursementsData = new TimesheetDetailReimbursement
                    {
                        TimesheetDetailId = timeSheetDetailId[i],
                        TimesheetDate = _context.TimesheetDetails.FirstOrDefault(x => x.TimesheetDetailId == timeSheetDetailId[i])?.TimesheetDate,
                        CreatedDate = DateTime.Now,
                        CreatedBy = AspId,
                    };
                    _context.TimesheetDetailReimbursements.Add(newReimbursementsData);
                }
                _context.SaveChanges();

                data = _context.TimesheetDetailReimbursements
                                .Where(x => timeSheetDetailId.Contains(x.TimesheetDetailId))
                                .ToList();
            }

            var reimbursementsData = data.Select(i => new AddReceiptsDetails
            {
                TimeSheetDetailId = i.TimesheetDetailId,
                ShiftDetailDate = i.TimesheetDate,
                Item = i.ItemName,
                Amount = i.Amount,
                BillValue = i.Bill,
            }).ToList();
            return reimbursementsData;
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Add Receipt Update Records
        /// </summary>
        /// <param name="aspId"></param>
        /// <param name="timeSheetDetailId"></param>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool EditReceipt(string aspId, int timeSheetDetailId, string item, int amount, IFormFile file)
        {
            try
            {
                var reimbursementsData = _context.TimesheetDetailReimbursements.FirstOrDefault(x => x.TimesheetDetailId == timeSheetDetailId);
                var physician = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == aspId);

                reimbursementsData.ItemName = item;
                reimbursementsData.Amount = amount;
                reimbursementsData.ModifiedBy = aspId;
                reimbursementsData.ModifiedDate = DateTime.Now;


                string directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content");

                if (file != null)
                {
                    string path = Path.Combine(directory, file.FileName);

                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    reimbursementsData.Bill = file.FileName;
                }
                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Delete Receipt
        /// </summary>
        /// <param name="aspId"></param>
        /// <param name="timeSheetDetailId"></param>
        /// <returns></returns>
        public bool DeleteReceipt(string aspId, int timeSheetDetailId)
        {
            try
            {
                var reimbursementsData = _context.TimesheetDetailReimbursements.FirstOrDefault(x => x.TimesheetDetailId == timeSheetDetailId);

                reimbursementsData.IsDeleted = true;
                reimbursementsData.ModifiedBy = aspId;
                reimbursementsData.ModifiedDate = DateTime.Now;
                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Finalize the Timesheet
        /// </summary>
        /// <param name="timeSheetId"></param>
        /// <returns></returns>
        public bool FinalizeTimeSheet(int timeSheetId)
        {
            try
            {
                var timeSheetData = _context.Timesheets.FirstOrDefault(x => x.TimesheetId == timeSheetId);

                timeSheetData.IsFinalize = true;
                timeSheetData.ModifiedDate = DateTime.Now;
                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion


        //***************************************************************************************************************************************************
        //***************************************************************************************************************************************************
        //***************************************************************************************************************************************************
    }
}
