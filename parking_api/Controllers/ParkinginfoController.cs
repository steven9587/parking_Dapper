using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace parking_api.Controllers
{
    public class ParkinginfoController : ApiController
    {
        public List<DbPark> Get(string area)
        {
            List<DbPark> conditionPark = new List<DbPark>();
            SqlConnection conn = new SqlConnection("data source=LAPTOP-9CB9PVPB\\SQLEXPRESS; initial catalog = SYSTEX; user id = sa; password = 0000");
            conn.Open();
            string strSelect = "SELECT * FROM Parking where area ='"+ area +"'";
            SqlCommand cmd = new SqlCommand(strSelect, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            try
            {
                while (dr.Read())
                {
                    //將所讀取到的該筆資料存入list中
                    conditionPark.Add(new DbPark() { 
                        Twd97X = (float)Convert.ToDouble(dr[0]),
                        Twd97Y = (float)Convert.ToDouble(dr[1]),
                        Name = dr[2].ToString(),
                        Area = dr[3].ToString(),
                        Address = dr[4].ToString(),
                        ServiceTime = dr[5].ToString(),
                        PayEx = dr[6].ToString(),
                        TotalCar = dr[7].ToString(),
                        TotalMotor = dr[8].ToString(),
                        Summary = dr[9].ToString(),
                        Id = dr[10].ToString(),
                        Tel = dr[11].ToString(),
                        UpdateTime = dr[12].ToString()
                    });
                }
            }
            finally
            {
                dr.Close();
            }  
            return conditionPark;
        }


        public string Get(string lat, string lng)
        {
            return "value";
        }

        public class DbPark
        {
            public float Twd97X { get; set; }
            public float Twd97Y { get; set; }
            public string Name { get; set; }
            public string Area { get; set; }
            public string Address { get; set; }
            public string ServiceTime { get; set; }
            public string PayEx { get; set; }
            public string TotalCar { get; set; }
            public string TotalMotor { get; set; }
            public string Summary { get; set; }
            public string Id { get; set; }
            public string Tel { get; set; }
            public string UpdateTime { get; set; }
        }


    }
}
