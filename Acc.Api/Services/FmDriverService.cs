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
    public class FmDriverService
    {
        private FunctionString fn;
        private IConfiguration config;
        private FmDriverRepo FmDriverRepo;
        public FmDriverService(IConfiguration configuration)
        {
            fn = new FunctionString(Tools.ConnectionString(configuration));
            config = configuration;
            FmDriverRepo = new FmDriverRepo(Tools.ConnectionString(configuration));
        }

        public Output Save(VmFMDriver Model)
        {
            Output _result = new Output();
            try
            {
                Model.SsPortfolioId = fn.DecryptString(Model.SsPortfolioId);
                Model.UserInput = fn.DecryptString(Model.UserInput);

                Model.FMDriver.Password = EncryptionLibrary.EncryptText(Model.FMDriver.Password);
                var dtRow = FmDriverRepo.SaveHeader(Model);

                Model.DriverDocument.ForEach(delegate (DriverDocument dt)
                {
                    FmDriverRepo.SaveDocument(dt, dtRow.row_id, Model.UserInput);
                });
               
                _result.Data = dtRow;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
        public Output Update(VmFMDriver Model)
        {
            Output _result = new Output();
            try
            {
                Model.SsPortfolioId = fn.DecryptString(Model.SsPortfolioId);
                Model.UserInput = fn.DecryptString(Model.UserInput);

                if (!string.IsNullOrEmpty(Model.FMDriver.Password))
                {
                    Model.FMDriver.Password = EncryptionLibrary.EncryptText(Model.FMDriver.Password);
                }
                
                var dtRow = FmDriverRepo.UpdateHeader(Model);

                //delete document
                var isOk = FmDriverRepo.DeleteDocument(Model.FMDriver.FMDriverId);

                Model.DriverDocument.ForEach(delegate (DriverDocument dt)
                {
                    FmDriverRepo.SaveDocument(dt, Model.FMDriver.FMDriverId, Model.UserInput);
                });

                _result.Data = dtRow;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
        public Output SaveWoUser(MMWorkshopUser Model)
        {
            Output _result = new Output();
            try
            {
                Model.SsPortfolioId = fn.DecryptString(Model.SsPortfolioId);
                Model.UserInput = fn.DecryptString(Model.UserInput);

                Model.Password = EncryptionLibrary.EncryptText(Model.Password);
                var dtRow = FmDriverRepo.SaveWoUser(Model);
               

                _result.Data = dtRow;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
        public Output UpdateWoUser(MMWorkshopUser Model)
        {
            Output _result = new Output();
            try
            {
                Model.SsPortfolioId = fn.DecryptString(Model.SsPortfolioId);
                Model.UserInput = fn.DecryptString(Model.UserInput);
                Model.UserEdit = fn.DecryptString(Model.UserEdit);

                if (!string.IsNullOrEmpty(Model.Password))
                {
                    Model.Password = EncryptionLibrary.EncryptText(Model.Password);
                }

                var dtRow = FmDriverRepo.UpdateWoUser(Model);
               

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
