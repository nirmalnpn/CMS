using CMS.DAL.Interface;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Repository
{
    public class DapperDaoRepo : IDapperDao
    {
        private readonly string connectionString;
        public DapperDaoRepo(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<T0> ExecuteQuery<T0>(string sqlQuery, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(connectionString.ToString()))
            {
                try
                {
                    sqlConnection.Open();
                    var result = sqlConnection.QueryMultiple(sqlQuery, commandTimeout: 30000,
                        commandType: queryType);
                    var res = result.Read<T0>().ToList();
                    return res;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
        public List<T0> ExecuteQuery<T0>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(connectionString.ToString()))
            {
                try
                {
                    sqlConnection.Open();
                    var result = sqlConnection.Query<T0>(sqlQuery, sqlParam, commandTimeout: 30000,
                        commandType: queryType).ToList();
                    //var res = result.Read<T0>().ToList();
                    return result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
        public T0 ExecuteQueryFirst<T0>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(connectionString.ToString()))
            {
                sqlConnection.Open();
                var result = sqlConnection.QueryMultiple(sqlQuery, sqlParam, commandTimeout: 30000,
                    commandType: queryType);
                var res = result.Read<T0>().FirstOrDefault();
                if (res == null)
                {

                    return res;
                }
                return res;
            }
        }
        public string ExecuteQueryJson<T0>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(connectionString.ToString()))
            {
                try
                {
                    sqlConnection.Open();
                    var result = sqlConnection.QueryMultiple(sqlQuery, sqlParam, commandTimeout: 30000,
                        commandType: queryType);
                    var res = result.Read<T0>().ToString();
                    return res;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
        public List<object> ExecuteQuery<T0, T1>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(connectionString.ToString()))
            {
                try
                {
                    sqlConnection.Open();
                    var result = sqlConnection.QueryMultiple(sqlQuery, sqlParam, commandTimeout: 30000, commandType: queryType);
                    var res = new List<object>();
                    res.Add(result.Read<T0>().ToList());
                    res.Add(result.Read<T1>().ToList());
                    return res;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
        public string ExecuteQueryString<T0>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(connectionString.ToString()))
            {
                try
                {
                    sqlConnection.Open();
                    var trans = sqlConnection.BeginTransaction();
                    var result = sqlConnection.Query<string>(sqlQuery, sqlParam, commandTimeout: 30000,
                        commandType: queryType, transaction: trans).FirstOrDefault();
                    if (result == null)
                    {
                        trans.Rollback();
                        sqlConnection.Close();
                        return result;
                    }
                    else
                    {
                        trans.Commit();
                        return result;
                    }
                    //var res = result.Read<T0>().ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        public Tuple<List<T0>, T1> ExecuteQueryTupleList<T0, T1>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(connectionString.ToString()))
            {
                try
                {
                    sqlConnection.Open();
                    var result = sqlConnection.QueryMultiple(sqlQuery, sqlParam, commandTimeout: 30000, commandType: queryType);
                    var list1 = result.Read<T0>().ToList();
                    var list2 = result.Read<T1>().SingleOrDefault();
                    return Tuple.Create(list1, list2);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
        public Tuple<T0, T1> ExecuteQueryTuple<T0, T1>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(connectionString.ToString()))
            {
                try
                {
                    sqlConnection.Open();
                    var result = sqlConnection.QueryMultiple(sqlQuery, sqlParam, commandTimeout: 30000, commandType: queryType);
                    var item1 = result.Read<T0>().SingleOrDefault();
                    var item2 = result.Read<T1>().SingleOrDefault();
                    return Tuple.Create(item1, item2);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        public async Task<List<T0>> ExecuteQueryAsync<T0>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(connectionString.ToString()))
            {
                try
                {
                    await sqlConnection.OpenAsync().ConfigureAwait(false);
                    var result = (await sqlConnection.QueryAsync<T0>(sqlQuery, sqlParam, commandTimeout: 30000, commandType: queryType)).ToList();
                    return result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
        public async Task<Tuple<T0, T1>> ExecuteQueryTupleAsync<T0, T1>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(connectionString.ToString()))
            {
                try
                {
                    await sqlConnection.OpenAsync().ConfigureAwait(false); // Open connection asynchronously
                    var result = await sqlConnection.QueryMultipleAsync(sqlQuery, sqlParam, commandTimeout: 30000, commandType: queryType).ConfigureAwait(false); // Execute query asynchronously

                    var item1 = await result.ReadSingleOrDefaultAsync<T0>().ConfigureAwait(false); // Read first result set asynchronously
                    var item2 = await result.ReadSingleOrDefaultAsync<T1>().ConfigureAwait(false); // Read second result set asynchronously

                    return Tuple.Create(item1, item2); // Return tuple
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
        public async Task<Tuple<List<T0>, T1>> ExecuteQueryTupleListAsync<T0, T1>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(connectionString.ToString()))
            {
                try
                {
                    await sqlConnection.OpenAsync().ConfigureAwait(false); // Open connection asynchronously
                    var result = await sqlConnection.QueryMultipleAsync(sqlQuery, sqlParam, commandTimeout: 30000, commandType: queryType).ConfigureAwait(false); // Execute query asynchronously

                    var item1 = (await result.ReadAsync<T0>().ConfigureAwait(false)).ToList(); // Read first result set asynchronously
                    var item2 = await result.ReadSingleOrDefaultAsync<T1>().ConfigureAwait(false); // Read second result set asynchronously

                    return Tuple.Create(item1, item2); // Return tuple
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close(); // Close connection
                }
            }
        }
        public async Task<Tuple<List<T0>, List<T1>, List<T2>>> ExecuteQueryTupleListAsync1<T0, T1, T2>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(connectionString.ToString()))
            {
                try
                {
                    await sqlConnection.OpenAsync().ConfigureAwait(false); // Open connection asynchronously
                    var result = await sqlConnection.QueryMultipleAsync(sqlQuery, sqlParam, commandTimeout: 30000, commandType: queryType).ConfigureAwait(false); // Execute query asynchronously

                    var item1 = (await result.ReadAsync<T0>().ConfigureAwait(false)).ToList(); // Read first result set asynchronously
                    var item2 = (await result.ReadAsync<T1>().ConfigureAwait(false)).ToList(); // Read first result set asynchronously
                    var item3 = (await result.ReadAsync<T2>().ConfigureAwait(false)).ToList(); // Read first result set asynchronously
                    //var item2 = await result.ReadSingleOrDefaultAsync<T1>().ConfigureAwait(false); // Read second result set asynchronously

                    return Tuple.Create(item1, item2, item3); // Return tuple
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close(); // Close connection
                }
            }
        }
        public async Task<T0> ExecuteQueryFirstAsync<T0>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(connectionString.ToString()))
            {
                try
                {
                    await sqlConnection.OpenAsync().ConfigureAwait(false);
                    var result = await sqlConnection.QueryMultipleAsync(sqlQuery, sqlParam, commandTimeout: 30000, commandType: queryType).ConfigureAwait(false);
                    var res = await result.ReadFirstOrDefaultAsync<T0>().ConfigureAwait(false);
                    return res;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
    }
}
