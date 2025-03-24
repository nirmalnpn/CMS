using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.DBConnection
{
    public class DBConnection : IDBConnect
    {
        private readonly IConfiguration _config;

        public DBConnection(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection connect()
        {
            return new SqlConnection(_config.GetConnectionString("DefaultConnection")); 
        }
    }
}
