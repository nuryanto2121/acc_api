using Acc.Api.DataAccess;
using Acc.Api.Helper;
using Acc.Api.Models;
using EncryptLibrary.AES256Encryption;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Services
{
    public class UserMgmService
    {
        private FunctionString fn;
        private IConfiguration config;
        private UserMgmRepo UserMgmRepo;
        public UserMgmService(IConfiguration configuration)
        {
            fn = new FunctionString(Tools.ConnectionString(configuration));
            UserMgmRepo = new UserMgmRepo(Tools.ConnectionString(configuration));
        }
        public Output Save(UserManagement Model)
        {
            Output _result = new Output();
            try
            {
                Model.portfolio_id = fn.DecryptString(Model.portfolio_id);
                Model.user_input = fn.DecryptString(Model.user_input);
                Model.subportfolio_id = fn.DecryptString(Model.subportfolio_id);

                Model.password = EncryptionLibrary.EncryptText(Model.password);
                var dtRow = UserMgmRepo.Save(Model);              

                _result.Data = dtRow;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
        public Output Update(UserManagement Model)
        {
            Output _result = new Output();
            try
            {
                Model.portfolio_id = fn.DecryptString(Model.portfolio_id);
                Model.user_edit = fn.DecryptString(Model.user_edit);
                Model.subportfolio_id = fn.DecryptString(Model.subportfolio_id);

                if (!string.IsNullOrEmpty(Model.password))
                {
                    Model.password = EncryptionLibrary.EncryptText(Model.password);
                }

                var dtRow = UserMgmRepo.Update(Model);                

                _result.Data = dtRow;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
    }
}
