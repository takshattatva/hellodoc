using hellodoc.BAL.Interface;
using hellodoc.DAL.Models;
using hellodoc.DAL.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;


namespace hellodoc.BAL.Repository
{
    public class RegisterRepo : IRegisterRepo
    {
        private readonly HellodocDbContext _context; 

        public RegisterRepo(HellodocDbContext context)
        {
            _context = context;
        }

        #region Create Patient Acc

        //***************************************************************************************************************************************************
        /// <summary>
        /// Add Data in Table(anu,u,r,rc,anur) During Creating Account
        /// </summary>
        /// <param name="registerVm"></param>
        public void RegisterUser(RegisterVm registerVm)
        {
            var rcname = _context.Requestclients.FirstOrDefault(x => x.Email == registerVm.Email);
            var data = new Aspnetuser()
            {
                Username = rcname.Firstname + rcname.Lastname,
                Email = registerVm.Email,
                Passwordhash = BCrypt.Net.BCrypt.HashPassword(registerVm.Password),
                Createddate = DateTime.Now,
            };
            _context.Aspnetusers.Add(data);
            _context.SaveChanges();

            Aspnetuser? aspnetuser = _context.Aspnetusers.FirstOrDefault(i => i.Email == registerVm.Email);
            string Aspnetusername = aspnetuser.Username;

            Requestclient? requestclient = _context.Requestclients.FirstOrDefault(i => i.Email == registerVm.Email);

            var data1 = new User()
            {
                Firstname = requestclient.Firstname,
                Lastname = requestclient.Lastname,
                Email = registerVm.Email,
                Mobile = requestclient.Phonenumber,
                Createddate = DateTime.Now,
                Strmonth = requestclient.Strmonth,
                Intyear = requestclient.Intyear,
                Intdate = requestclient.Intdate,
                Aspnetuserid = aspnetuser.Id,
                Createdby = requestclient.Firstname + " " + requestclient.Lastname,
                Modifiedby = requestclient.Firstname + " " + requestclient.Lastname,
                Modifieddate = DateTime.Now,
                Street= requestclient.Street,
                City = requestclient.City,
                State = requestclient.State,
                Zipcode = requestclient.Zipcode,
            };
            _context.Users.Add(data1);
            _context.SaveChanges();

            User? user = _context.Users.FirstOrDefault(i => i.Email == registerVm.Email);
            int userId = user.Userid;

            Requestclient? rc = _context.Requestclients.FirstOrDefault(i => i.Email == registerVm.Email);
            int Requestid = rc.Requestid;
            string PhoneNo = rc.Phonenumber;

            Request? request1 = _context.Requests.FirstOrDefault(i => i.Requestid == Requestid);
            request1.Userid = userId;
            _context.SaveChanges();
            _context.Update(request1);

            Aspnetuser? anu = _context.Aspnetusers.FirstOrDefault(x => x.Email == registerVm.Email);
            anu.Phonenumber = PhoneNo;
            _context.SaveChanges();
            _context.Update(anu);

            Aspnetuser? aspnetuserid = _context.Aspnetusers.FirstOrDefault(i => i.Email == registerVm.Email);
            Aspnetuserrole anur = new Aspnetuserrole();
            anur.Userid = aspnetuserid.Id;
            anur.Roleid = "3";
            _context.Aspnetuserroles.Add(anur);
            _context.SaveChanges();
        }

        #endregion


        #region Reset Passwd 

        //***************************************************************************************************************************************************
        /// <summary>
        /// Update Data in Table(anu) During Reseting Passwd
        /// </summary>
        /// <param name="registerVm"></param>
        public void ResetPassword(RegisterVm registerVm)
        {
            Aspnetuser? aspnetuser = _context.Aspnetusers.FirstOrDefault(f => f.Email == registerVm.Email);
            if (aspnetuser != null)
            {
               aspnetuser.Passwordhash = BCrypt.Net.BCrypt.HashPassword(registerVm.Password);
               aspnetuser.Otp = null;
               aspnetuser.Modifieddate = DateTime.Now;
            }
            _context.SaveChanges();
            _context.Update(aspnetuser);
        }

        #endregion


        #region Get Role for Login

        //***************************************************************************************************************************************************
        /// <summary>
        /// Get role from Table(Aspnetuserrole via 3 table(anu,anur,anr) include)
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public Aspnetuser GetUserRole(string Email)
        {
            var role = _context.Aspnetusers.Include(x => x.Aspnetuserrole).ThenInclude(y => y.Role).FirstOrDefault(x => x.Email == Email);
            return role;
        }

        #endregion


        #region Encrypt-Decrypt

        public string Encrypt(string clearText)
        {
            string EncryptionKey = "abc123";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "abc123";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        #endregion

        //***************************************************************************************************************************************************
        //***************************************************************************************************************************************************
        //***************************************************************************************************************************************************
    }
}
