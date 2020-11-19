using Acc.Api.DataAccess;
using Acc.Api.Helper;
using Acc.Api.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Services
{
    public class TrackingService
    {
        private FunctionString fn;
        private TrackingRepo trackingRepo;
        public TrackingService(IConfiguration configuration)
        {
            fn = new FunctionString(Tools.ConnectionString(configuration));
            trackingRepo = new TrackingRepo(Tools.ConnectionString(configuration));
        }

        public Output GetDataTracking(string OrderNo)
        {
            Output _result = new Output();
            try
            {
                RowID dataRowID = trackingRepo.GetRowID(OrderNo);
                if (dataRowID == null || dataRowID.row_id == 0)
                {
                    throw new Exception("No Order yang anda masukan salah.");
                }
                _result.Data = trackingRepo.GetDataTracking(dataRowID.row_id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
    }
}
