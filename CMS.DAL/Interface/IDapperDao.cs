using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Interface
{
    public interface IDapperDao
    {
        List<T0> ExecuteQuery<T0>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
        T0 ExecuteQueryFirst<T0>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
        List<T0> ExecuteQuery<T0>(string sqlQuery, CommandType queryType = CommandType.StoredProcedure);
        List<object> ExecuteQuery<T0, T1>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
        string ExecuteQueryJson<T0>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
        string ExecuteQueryString<T0>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);

        Tuple<T0, T1> ExecuteQueryTuple<T0, T1>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
        Tuple<List<T0>, T1> ExecuteQueryTupleList<T0, T1>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);

        Task<List<T0>> ExecuteQueryAsync<T0>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
        Task<T0> ExecuteQueryFirstAsync<T0>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
        Task<Tuple<T0, T1>> ExecuteQueryTupleAsync<T0, T1>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
        Task<Tuple<List<T0>, T1>> ExecuteQueryTupleListAsync<T0, T1>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
        Task<Tuple<List<T0>, List<T1>, List<T2>>> ExecuteQueryTupleListAsync1<T0, T1, T2>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
    }
}
