using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parking_lib
{
    public class DBHelper
    {
        private string CONN_STRIN = string.Empty;
        public DBHelper(string strConnString)
        {
            CONN_STRIN = strConnString;
        }

        public SqlDataReader Excute(string strCommand, SqlParameter[] sqlParameter = null)
        {
            SqlConnection conn = new SqlConnection(CONN_STRIN);
            conn.Open();
            SqlCommand cmd = new SqlCommand(strCommand, conn);
            //若有參數要塞在執行
            if (sqlParameter != null)
            {
                cmd.Parameters.AddRange(sqlParameter);
            }
            SqlDataReader dr = cmd.ExecuteReader();
            return dr;
        }
    }
}
