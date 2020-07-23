﻿using System;
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
        //利用area搜尋的API
        public List<DbPark> Get(string area)
        {
            List<DbPark> conditionPark = new List<DbPark>();
            SqlConnection conn = new SqlConnection("data source=LAPTOP-9CB9PVPB\\SQLEXPRESS; initial catalog = SYSTEX; user id = sa; password = 0000");
            conn.Open();
            string strSelect = "SELECT * FROM Parking where area ='" + area + "'";
            SqlCommand cmd = new SqlCommand(strSelect, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            try
            {
                while (dr.Read())
                {
                    //將所讀取到的該筆資料存入list中
                    conditionPark.Add(new DbPark()
                    {
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

        //利用經度、緯度、及距離搜尋的API
        public List<DbPark> Get(string lat, string lng, float distance)
        {
            List<DbPark> conditionPark = new List<DbPark>();
            SqlConnection conn = new SqlConnection("data source=LAPTOP-9CB9PVPB\\SQLEXPRESS; initial catalog = SYSTEX; user id = sa; password = 0000");
            conn.Open();
            string strSelect = "SELECT * FROM Parking;";
            SqlCommand cmd = new SqlCommand(strSelect, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            try
            {
                while (dr.Read())
                {
                    if (distance >= DistanceOfTwoPoints(Convert.ToSingle(lat), Convert.ToSingle(lng), (float)Convert.ToDouble(dr[0]), (float)Convert.ToDouble(dr[1])))
                    {
                        //將所讀取到且符合距離的該筆資料存入list中
                        conditionPark.Add(new DbPark()
                        {
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
            }
            finally
            {
                dr.Close();
            }
            return conditionPark;
        }

        //透過經緯度求出兩點距離
        public double DistanceOfTwoPoints(float lat1, float lng1, float lat2, float lng2)
        {
            float radLng1 = (float)(lng1 * Math.PI / 180.0);
            float radLng2 = (float)(lng2 * Math.PI / 180.0);
            float a = radLng1 - radLng2;
            float b = (float)((lat1 - lat2) * Math.PI / 180.0);
            float s = (float)(2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
                Math.Cos(radLng1) * Math.Cos(radLng2) * Math.Pow(Math.Sin(b / 2), 2))
                ) * 6378.137);
            s = (float)(Math.Round(s * 10000) / 10000);
            return s;
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