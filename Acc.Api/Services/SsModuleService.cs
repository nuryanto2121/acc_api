using Acc.Api.DataAccess;
using Acc.Api.Helper;
using Acc.Api.Interface;
using Acc.Api.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Services
{
    public class SsModuleService : ICrudService<SsModule, int>
    {
        private FunctionString fn;
        private SsModuleRepo moduleRepo;
        public SsModuleService(IConfiguration configuration)
        {
            fn = new FunctionString(Tools.ConnectionString(configuration));
            moduleRepo = new SsModuleRepo(Tools.ConnectionString(configuration));
        }
        public Output Delete(int Key)
        {
            Output _result = new Output();
            try
            {
                _result.Data = moduleRepo.Delete(Key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public Output GetDataBy(int Key)
        {
            Output _result = new Output();
            try
            {
                _result.Data = moduleRepo.GetById(Key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public Output GetDataList(string Parameter)
        {
            Output _result = new Output();
            try
            {
                Parameter = !string.IsNullOrEmpty(Parameter) ? "WHERE " + Parameter : string.Empty;
                _result.Data = moduleRepo.GetList(Parameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public Output GetList(JObject JModel)
        {
            throw new NotImplementedException();
            //Output _result = new Output();
            //try
            //{
            //    _result.Data = moduleRepo.GetById(Key);
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //return _result;
        }

        public Output Insert(SsModule Model)
        {
            Output _result = new Output();
            try
            {
                _result.Data = moduleRepo.Save(Model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public Output Update(SsModule Model)
        {
            Output _result = new Output();
            try
            {

                _result.Data = moduleRepo.Update(Model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
    }
}
