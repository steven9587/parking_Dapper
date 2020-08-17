using Dapper;
using MongoDB.Bson;
using MongoDB.Driver;
using parking_lib;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;

namespace parking_api.Controllers
{
    public class ParkinginfoController : ApiController
    {
        SqlConnection conn = new SqlConnection(GetDBConnectionString());
        List<ParkData> conditionPark = new List<ParkData>();
        List<LocationData> location = new List<LocationData>();
        //利用area搜尋的API (Dapper寫法)
        public List<ParkData> Get(string area)
        {
            try
            {
                conn.Open();
                conditionPark = conn.Query<ParkData>("SELECT * FROM Parking where area = @Area",
                                        new { Area = area }).ToList();
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return conditionPark;
        }

        //利用經度、緯度、及距離搜尋的API
        public List<ParkData> Get(string lat, string lng, float distance)
        {
            try
            {
                //先撈出所有經度&緯度
                conn.Open();
                location = conn.Query<LocationData>("SELECT * FROM Parking;").ToList();
                //一一比對每組經度&緯度，若在範圍內就存入list中
                foreach (var loc in location)
                {
                    if (distance >= DistanceOfTwoPoints(Convert.ToSingle(lat), Convert.ToSingle(lng), (float)Convert.ToDouble(loc.Latitude), (float)Convert.ToDouble(loc.Longitude)))
                    {
                        conditionPark.AddRange(conn.Query<ParkData>("SELECT * FROM Parking where latitude = @Lat and longitude = @Lon",
                                            new { Lat = loc.Latitude, Lon = loc.Longitude }).ToList());
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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

        private static string GetDBConnectionString()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString.ToString();
        }

        //利用id查詢該停車場還剩下多少空車位
        public string ParkingSpace(string id)
        {
            string ParkingSpace = "NULL";
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("SYSTEX");
            var collec = database.GetCollection<BsonDocument>("Parking");
            var filter = Builders<BsonDocument>.Filter.Empty;
            var projection = Builders<BsonDocument>.Projection.Include("id").Include("availableCar").Exclude("_id");
            var documents = collec.Find(filter).Project(projection).ToList();

            //linQ寫法 => 效能upup
            ParkingSpace = documents
                .Where(x => x.GetElement(0).Value.ToString() == id)
                .Select(p => p.GetElement(1).Value.ToString())
                .FirstOrDefault();

            //迴圈寫法
            //foreach (var data in documents)
            //{
            //    string parkingId = data.GetElement(0).Value.ToString();
            //    string availableCar = data.GetElement(1).Value.ToString();
            //    //parkingSpaceData.Add(new ParkingSpaceData() { Id = parkingId, AvailableCar = availableCar });
            //    if (id == parkingId)
            //    {
            //        ParkingSpace = availableCar;
            //    }
            //}
            return ParkingSpace;
        }
    }
}
