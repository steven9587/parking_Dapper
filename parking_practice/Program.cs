using System;
using System.Timers;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace parking_practice
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //DailyTimer
            Timer dailyTimer = new Timer(24 * 60 * 60 * 1000);
            //Timer dailyTimer = new Timer(5 * 1000);
            dailyTimer.Elapsed += DailyEvent;
            dailyTimer.Enabled = true;
            //FiveMinsTimer
            Timer fiveMinsTimer = new Timer(5 * 60 * 1000);
            //Timer fiveMinsTimer = new Timer(7 * 1000);
            fiveMinsTimer.Elapsed += FiveMinsEvent;
            fiveMinsTimer.Enabled = true;
            //
            Console.ReadLine();
            dailyTimer.Stop();
            dailyTimer.Dispose();
            fiveMinsTimer.Stop();
            fiveMinsTimer.Dispose();


        }
        private static async void DailyEvent(object sender, ElapsedEventArgs e)
        {
            //Console.WriteLine("{0:HH:mm:ss.fff}",
            //             e.SignalTime);
            await Task.Run(async () => { await DailyProcess.DoWork(); });
        }

        private static async void FiveMinsEvent(object sender, ElapsedEventArgs e)
        {
            //Console.WriteLine("===================================================================");
            await Task.Run(async () => { await FiveMinsProcess.DoWork(); });
        }
    }
}