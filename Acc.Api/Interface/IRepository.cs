using Acc.Api.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Interface
{
    public interface IRepository<T, K>
    {
        bool Save(T domain);
        bool Update(T domain);
        bool Delete(K key,K timestamp);
        List<T> GetList();
        List<T> GetList(int pageSize, int currentPage, string sortName, string sortOrder,string Parameter);
        T GetById(K key,K timestamp);
        object SelectScalar(SQL.Function.Aggregate function, string column);

    }
}
