using Acc.Api.Models;
using Newtonsoft.Json.Linq;
using Acc.Api.Enum;

namespace Acc.Api.Interface
{
    interface IDynamicService
    {
        Output execute(int? id,int lastupdatestamp, int? line_no, string option_url, bool isDelete = false);
        Output execute(JObject JModel, SQL.Method.Aggregate Method);
        DTResultListDyn<dynamic> executeList(ParamList JModel, SQL.Method.Aggregate Method);
        Output executeDataList(JObject JModel, SQL.Method.Aggregate Method);
        Output executeMultiDelete(DynamicMultiParam JModel, SQL.Method.Aggregate Method);
        Output executeMultiPost(DynamicMultiParam JModel, SQL.Method.Aggregate Method);
        Output executeMulti(DynamicMultiParam JModel, SQL.Method.Aggregate Method);
        Output ProsesDataPostMulti(DynamicMultiParam JModel, SQL.Method.Aggregate Method);
        DTResultListLookup<dynamic> GetlistLookUp(ParamLookupList JModel, SQL.Method.Aggregate Method);
        Output GetDataLookUp(JObject JModel, SQL.Method.Aggregate Method);
        Output GetDataByLookUp(JObject JModel, SQL.Method.Aggregate Method);
        Output executeFunction(JObject JModel);
        Output GetDataSpec(JObject JModel);
    }
}
