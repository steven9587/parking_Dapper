using Newtonsoft.Json;
using parking_lib;
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
            await Task.Run(async () => { await DailyProcess.DoWork(); });
            await Task.Run(async () => { await FiveMinsProcess.DoWork(); });
        }
    }
}