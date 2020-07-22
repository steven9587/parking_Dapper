using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;

namespace parking_practice
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //讀取原本DB資料   
            //List<float> DBlatitude = new List<float>();
            //List<float> DBlongitude = new List<float>();
            List<Location> location = new List<Location>();
            SqlConnection conn = new SqlConnection("data source=LAPTOP-9CB9PVPB\\SQLEXPRESS; initial catalog = SYSTEX; user id = sa; password = 0000");
            conn.Open();
            string strSelect = "SELECT latitude,longitude FROM parking";
            SqlCommand cmd = new SqlCommand(strSelect, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            try
            {
                while (dr.Read())
                {
                    //將所讀取到的經度&緯度存入list中
                    //DBlatitude.Add((float)Convert.ToDouble(dr[0]));
                    //DBlongitude.Add((float)Convert.ToDouble(dr[1]));
                    location.Add(new Location() { Latitude = (float)Convert.ToDouble(dr[0]), Longitude = (float)Convert.ToDouble(dr[1]) });
                }
            }
            finally
            {
                dr.Close();
            }
            //讀取線上即時資料
            for (int i = 0; i < 2; i++)
            {
                var temp = await GetRequest("https://data.ntpc.gov.tw/api/datasets/B1464EF0-9C7C-4A6F-ABF7-6BDF32847E68/json?page=" + i + "&size=1000");
                //將一大串json字串切割成一筆一筆的
                JArray jsondata = JsonConvert.DeserializeObject<JArray>(temp);
                foreach (JObject data in jsondata)
                {
                    float latitude = (float)data["twd97X"]; //停車場緯度
                    float longitude = (float)data["twd97Y"]; //停車場經度
                    string name = (string)data["name"];//停車場名稱
                    string area = (string)data["area"];//停車場所在區域
                    string address = (string)data["address"];//停車場地址
                    string serviceTime = (string)data["serviceTime"];//停車場服務時間
                    string payEx = (string)data["payEx"];//停車場收費資訊
                    string totalCar = (string)data["totalCar"];//汽車停車格總數
                    string totalMotor = (string)data["totalMotor"];//機車停車格總數
                    string summary = (string)data["summary"];//停車場基本描述
                    string id = (string)data["id"];//停車場唯一ID
                    string tel = (string)data["tel"]; //停車場連絡電話
                    string time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//資料更新時間
                    Console.WriteLine("停車場緯度:" + latitude + " " + "停車場經度:" + longitude + " " + "停車場名稱:" + name + " " + "停車場所在區域:" + area + " " +
                        "停車場地址:" + address + " " + "停車場服務時間:" + serviceTime + " " + "停車場收費資訊:" + payEx + " " + "汽車停車格總數:" + totalCar + " " +
                        "機車停車格總數:" + totalMotor + " " + "停車場基本描述:" + summary + " " + "停車場唯一ID:" + id + " " + "停車場連絡電話:" + tel + " " +
                        "資料更新時間:" + time);

                    //判斷資料庫是否有該筆資料(經度、緯度)
                    Boolean exist = false;
                    //foreach (float a in DBlatitude)//針對緯度檢查
                    //{
                    //    if (latitude == a)
                    //    {
                    //        foreach (float b in DBlongitude)//針對經度檢查
                    //        {
                    //            if (b == longitude)
                    //            {
                    //                exist = true;//經度&緯度都相等 => 該筆資料庫資料已存在
                    //            }
                    //        }
                    //    }
                    //}
                    foreach (var loc in location)
                    {
                        if (latitude == loc.Latitude && longitude == loc.Longitude)
                        {
                            exist = true;
                        }
                    }

                    //將該筆資料放入DB中(更新or新增)
                    conn = new SqlConnection("data source=LAPTOP-9CB9PVPB\\SQLEXPRESS; initial catalog = SYSTEX; user id = sa; password = 0000");
                    conn.Open();
                    if (exist)
                    {
                        //資料已存在 => 更新
                        string strUpdate = @"UPDATE Parking SET name = @name, area = @area, address = @address,serviceTime = @serviceTime,payInfo = @payInfo,
                                        totalCar = @totalCar,totalMotor = @totalMotor,summary = @summary,id = @id,tel = @tel,updateTime = @updateTime 
                                        WHERE latitude=@latitude and longitude = @longitude;";
                        cmd = new SqlCommand(strUpdate, conn);
                    }
                    else
                    {
                        //資料不存在 => 加入資料庫
                        string strInsert = @"INSERT INTO Parking(latitude, longitude, name,area,address,serviceTime,payInfo,totalCar,totalMotor,summary,id,tel,updateTime)
                                        VALUES (@latitude, @longitude, @name,@area,@address,@serviceTime,@payInfo,@totalCar,@totalMotor,@summary,@id,@tel,@updateTime)";
                        cmd = new SqlCommand(strInsert, conn);
                    }
                    try
                    {
                        cmd.Parameters.AddWithValue("@latitude", latitude);
                        cmd.Parameters.AddWithValue("@longitude", longitude);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@area", area);
                        cmd.Parameters.AddWithValue("@address", address);
                        cmd.Parameters.AddWithValue("@serviceTime", serviceTime);
                        if (payEx == null)
                        {
                            payEx = "NULL";
                        }
                        cmd.Parameters.AddWithValue("@payInfo", payEx);
                        cmd.Parameters.AddWithValue("@totalCar", totalCar);
                        cmd.Parameters.AddWithValue("@totalMotor", totalMotor);
                        if (summary == null)
                        {
                            summary = "NULL";
                        }
                        cmd.Parameters.AddWithValue("@summary", summary);
                        cmd.Parameters.AddWithValue("@id", id);
                        if (tel == null)
                        {
                            tel = "NULL";
                        }
                        cmd.Parameters.AddWithValue("@tel", tel);
                        cmd.Parameters.AddWithValue("@updateTime", time);
                        cmd.ExecuteNonQuery();
                    }
                    catch (SqlException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        cmd.Cancel();
                    }
                    conn.Close();
                }
                Console.WriteLine("======================================================================================================================");
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