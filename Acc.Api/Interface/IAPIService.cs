using Newtonsoft.Json.Linq;
using Acc.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Interface
{
    public interface IApiDynamicService<T>
    {
        Output execute(JObject Model);
        DTResultListDyn<dynamic> executeList(JObject Model);
    }
}
