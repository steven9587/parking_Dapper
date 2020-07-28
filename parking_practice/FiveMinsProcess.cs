using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace parking_practice
{
    public class FiveMinsProcess
    {
        public static async Task DoWork()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("SYSTEX");
            database.DropCollection("Parking");
            var collec = database.GetCollection<BsonDocument>("Parking");
            //讀取線上即時資料
            for (int i = 0; i < 2; i++)
            {
                var temp = await GetRequest("https://data.ntpc.gov.tw/api/datasets/E09B35A5-A738-48CC-B0F5-570B67AD9C78/json?page=" + i + "&size=1000");
                //將一大串json字串切割成一筆一筆的 => 存入list中
                List<ParkingSpace> parkspacedata = JsonConvert.DeserializeObject<List<ParkingSpace>>(temp);
                foreach (var spacedata in parkspacedata)
                {
                    string id = spacedata.Id;
                    string availableCar = spacedata.AvailableCar;
                    Console.WriteLine("停車場唯一ID:" + id + " " + "剩下車位:" + availableCar);

                    //將沒有值的欄位補"NULL" => 以解決NullExecption
                    if (availableCar == "-9")
                    {
                        availableCar = "NULL";
                    }

                    var document = new BsonDocument {
                            { "id",id},
                            { "availableCar",availableCar}
                         };

                    await collec.InsertOneAsync(document);

                }
                Console.WriteLine("======================================================================================================================");
                //conn.Close();
            }
            Console.ReadKey();
        }


        //讀取swagger上的Opendata(非同步)
        private static async Task<string> GetRequest(string url)
        {
            string mycontent = string.Empty;
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    using (HttpContent content = response.Content)
                    {
                        mycontent = await content.ReadAsStringAsync();
                    }
                }
            }
            return mycontent;
        }
    }
}
