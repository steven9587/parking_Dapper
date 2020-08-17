using Dapper;
using Newtonsoft.Json;
using parking_lib;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace parking_practice
{
    public class DailyProcess
    {
        public static async Task DoWork()
        {
            //讀取原本DB資料的經度&緯度   
            SqlConnection conn = new SqlConnection(GetDBConnectionString());
            List<LocationData> location = new List<LocationData>();
            try
            {
                conn.Open();
                location = conn.Query<LocationData>("SELECT * FROM Parking;").ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            for (int i = 0; i < 2; i++)
            {
                //DBHelper dBHelper = new DBHelper(GetDBConnectionString());
                //conn = new SqlConnection(GetDBConnectionString());
                string strCommand = string.Empty;
                var temp = await GetRequest("https://data.ntpc.gov.tw/api/datasets/B1464EF0-9C7C-4A6F-ABF7-6BDF32847E68/json?page=" + i + "&size=1000");
                //將一大串json字串切割成一筆一筆的
                Console.WriteLine(temp);
                List<Park> parkdata = JsonConvert.DeserializeObject<List<Park>>(temp);
                foreach (var data in parkdata)
                {
                    float latitude = Convert.ToSingle(Cal_TWD97_To_lonlat(data.Twd97X, data.Twd97Y, "lat"));
                    float longitude = Convert.ToSingle(Cal_TWD97_To_lonlat(data.Twd97X, data.Twd97Y, "lon"));
                    string name = data.Name;
                    string area = data.Area;
                    string address = data.Address;
                    string serviceTime = data.ServiceTime;
                    string payEx = data.PayEx;
                    string totalCar = data.TotalCar;
                    string totalMotor = data.TotalMotor;
                    string summary = data.Summary;
                    string id = data.Id;
                    string tel = data.Tel;
                    string time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//資料更新時間
                    Console.WriteLine("停車場緯度:" + latitude + " " + "停車場經度:" + longitude + " " + "停車場名稱:" + name + " " + "停車場所在區域:" + area + " " +
                        "停車場地址:" + address + " " + "停車場服務時間:" + serviceTime + " " + "停車場收費資訊:" + payEx + " " + "汽車停車格總數:" + totalCar + " " +
                        "機車停車格總數:" + totalMotor + " " + "停車場基本描述:" + summary + " " + "停車場唯一ID:" + id + " " + "停車場連絡電話:" + tel + " " +
                        "資料更新時間:" + time);

                    //判斷資料庫是否有該筆資料(經度、緯度)
                    Boolean exist = false;
                    foreach (var loc in location)
                    {
                        if (latitude == loc.Latitude && longitude == loc.Longitude)
                        {
                            exist = true;
                        }
                    }

                    try
                    {
                        if (payEx == null)
                        {
                            payEx = "NULL";
                        }
                        if (summary == null)
                        {
                            summary = "NULL";
                        }
                        if (tel == null)
                        {
                            tel = "NULL";
                        }
                        //將該筆資料放入DB中(更新or新增)
                        if (exist)
                        {
                            //資料已存在 => 更新
                            strCommand = @"UPDATE Parking SET name = @name, area = @area, address = @address,serviceTime = @serviceTime,payInfo = @payInfo,
                                        totalCar = @totalCar,totalMotor = @totalMotor,summary = @summary,id = @id,tel = @tel,updateTime = @updateTime 
                                        WHERE latitude=@latitude and longitude = @longitude;";
                        }
                        else
                        {
                            //資料不存在 => 加入資料庫
                            strCommand = @"INSERT INTO Parking(latitude, longitude, name,area,address,serviceTime,payInfo,totalCar,totalMotor,summary,id,tel,updateTime)
                                        VALUES (@latitude, @longitude, @name,@area,@address,@serviceTime,@payInfo,@totalCar,@totalMotor,@summary,@id,@tel,@updateTime)";
                        }
                        SqlParameter[] sqlParameter = new SqlParameter[] {
                            new SqlParameter("@latitude",latitude),
                            new SqlParameter("@longitude",longitude),
                            new SqlParameter("@name",name),
                            new SqlParameter("@area",area),
                            new SqlParameter("@address",address),
                            new SqlParameter("@serviceTime",serviceTime),
                            new SqlParameter("@payInfo",payEx),
                            new SqlParameter("@totalCar",totalCar),
                            new SqlParameter("@totalMotor",totalMotor),
                            new SqlParameter("@summary",summary),
                            new SqlParameter("@id",id),
                            new SqlParameter("@tel",tel),
                            new SqlParameter("@updateTime",time)
                        };
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(strCommand, conn);
                        cmd.Parameters.AddRange(sqlParameter);
                        cmd.ExecuteNonQuery();
                        //dBHelper.Excute(strCommand, sqlParameter);
                    }
                    catch (SqlException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
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

        //TWD97坐標轉WGS84
        private static string Cal_TWD97_To_lonlat(double x, double y, string back)
        {
            double a = 6378137.0;
            double b = 6356752.314245;
            double k0 = 0.9999;
            int dx = 250000;
            double dy = 0;
            double lon0 = 121 * Math.PI / 180;
            double e = Math.Pow((1 - Math.Pow(b, 2) / Math.Pow(a, 2)), 0.5);

            x -= dx;
            y -= dy;

            // Calculate the Meridional Arc
            double M = y / k0;

            // Calculate Footprint Latitude
            double mu = M / (a * (1.0 - Math.Pow(e, 2) / 4.0 - 3 * Math.Pow(e, 4) / 64.0 - 5 * Math.Pow(e, 6) / 256.0));
            double e1 = (1.0 - Math.Pow((1.0 - Math.Pow(e, 2)), 0.5)) / (1.0 + Math.Pow((1.0 - Math.Pow(e, 2)), 0.5));

            double J1 = (3 * e1 / 2 - 27 * Math.Pow(e1, 3) / 32.0);
            double J2 = (21 * Math.Pow(e1, 2) / 16 - 55 * Math.Pow(e1, 4) / 32.0);
            double J3 = (151 * Math.Pow(e1, 3) / 96.0);
            double J4 = (1097 * Math.Pow(e1, 4) / 512.0);

            double fp = mu + J1 * Math.Sin(2 * mu) + J2 * Math.Sin(4 * mu) + J3 * Math.Sin(6 * mu) + J4 * Math.Sin(8 * mu);

            // Calculate Latitude and Longitude

            double e2 = Math.Pow((e * a / b), 2);
            double C1 = Math.Pow(e2 * Math.Cos(fp), 2);
            double T1 = Math.Pow(Math.Tan(fp), 2);
            double R1 = a * (1 - Math.Pow(e, 2)) / Math.Pow((1 - Math.Pow(e, 2) * Math.Pow(Math.Sin(fp), 2)), (3.0 / 2.0));
            double N1 = a / Math.Pow((1 - Math.Pow(e, 2) * Math.Pow(Math.Sin(fp), 2)), 0.5);

            double D = x / (N1 * k0);

            // 計算緯度
            double Q1 = N1 * Math.Tan(fp) / R1;
            double Q2 = (Math.Pow(D, 2) / 2.0);
            double Q3 = (5 + 3 * T1 + 10 * C1 - 4 * Math.Pow(C1, 2) - 9 * e2) * Math.Pow(D, 4) / 24.0;
            double Q4 = (61 + 90 * T1 + 298 * C1 + 45 * Math.Pow(T1, 2) - 3 * Math.Pow(C1, 2) - 252 * e2) * Math.Pow(D, 6) / 720.0;
            double lat = fp - Q1 * (Q2 - Q3 + Q4);

            // 計算經度
            double Q5 = D;
            double Q6 = (1 + 2 * T1 + C1) * Math.Pow(D, 3) / 6;
            double Q7 = (5 - 2 * C1 + 28 * T1 - 3 * Math.Pow(C1, 2) + 8 * e2 + 24 * Math.Pow(T1, 2)) * Math.Pow(D, 5) / 120.0;
            double lon = lon0 + (Q5 - Q6 + Q7) / Math.Cos(fp);

            lat = (lat * 180) / Math.PI; //緯
            lon = (lon * 180) / Math.PI; //經

            if (back == "lon")
            {
                return lon.ToString();
            }
            else
            {
                return lat.ToString();
            }
        }

        private static string GetDBConnectionString()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString.ToString();
        }
    }
}
