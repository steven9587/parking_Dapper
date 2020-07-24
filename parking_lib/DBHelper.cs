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
        private SqlConnection conn = null;
        private SqlCommand cmd = null;

        public DBHelper(string strConnString)
        {
            CONN_STRIN = strConnString;
        }
        
        //讀取庫資料中的資料 => select
        public SqlDataReader Query(string strCommand, SqlParameter[] sqlParameter = null)
        {
            conn = new SqlConnection(CONN_STRIN);
            conn.Open();
            cmd = new SqlCommand(strCommand, conn);
            //若有參數要塞在執行
            if (sqlParameter != null)
            {
                cmd.Parameters.AddRange(sqlParameter);
            }
            SqlDataReader dr = cmd.ExecuteReader();
            return dr;
        }

        public void Dispose()
        {
            cmd.Cancel();
            conn.Close();
        }

        //寫資料入庫資料 => insert、update、delete
        public int Excute(string strCommand, SqlParameter[] sqlParameter = null)
        {
            conn = new SqlConnection(CONN_STRIN);
            conn.Open();
            cmd = new SqlCommand(strCommand, conn);
            //若有參數要塞在執行
            if (sqlParameter != null)
            {
                cmd.Parameters.AddRange(sqlParameter);
            }
            int count = cmd.ExecuteNonQuery();
            //cmd.Cancel();
            //conn.Close();
            return count;
        }
    }
}
