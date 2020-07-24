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
        
        //讀取庫資料中的資料 => select
        public SqlDataReader Query(string strCommand, SqlParameter[] sqlParameter = null)
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

        //寫資料入庫資料 => insert、update、delete
        public int Excute(string strCommand, SqlParameter[] sqlParameter = null)
        {
            SqlConnection conn = new SqlConnection(CONN_STRIN);
            conn.Open();
            SqlCommand cmd = new SqlCommand(strCommand, conn);
            //若有參數要塞在執行
            if (sqlParameter != null)
            {
                cmd.Parameters.AddRange(sqlParameter);
            }
            int count = cmd.ExecuteNonQuery();
            cmd.Cancel();
            conn.Close();
            return count;
        }
    }
}
