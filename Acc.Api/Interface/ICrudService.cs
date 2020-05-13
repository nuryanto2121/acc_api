using Newtonsoft.Json.Linq;
using Acc.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Interface
{
    public interface ICrudService<T, K>
    {
        Output Insert(T Model);
        Output Update(T Model);
        Output Delete(K Key,K Timestamp);
        Output GetDataBy(K Key, K Timestamp);
        DTResultListDyn<dynamic> GetList(ParamList JModel);
        Output GetDataList(string Parameter);
    }
}
