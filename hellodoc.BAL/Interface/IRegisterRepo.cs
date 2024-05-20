using hellodoc.DAL.Models;
using hellodoc.DAL.ViewModels;

namespace hellodoc.BAL.Interface
{
    public interface IRegisterRepo
    {
        #region Create Account

        public void RegisterUser(RegisterVm registerVm);

        #endregion


        #region Reset Password

        public void ResetPassword(RegisterVm registerVm);

        #endregion


        #region Get Role

        public Aspnetuser GetUserRole(string Email);

        #endregion


        #region Encrypt-Decrypt

        string Encrypt(string clearText);

        string Decrypt(string cipherText);

        #endregion
    }
}

  