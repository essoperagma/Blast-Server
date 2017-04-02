using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Core.Utilities
{
    static class Chronos
    {
        public static float deltaTime; // son 2 frame arasindaki zaman farkini gosterir. saniye cinsinden.

        static long interval;
        static DateTime lastTickTime = DateTime.UtcNow;

        public static void setInterval(long msInterval)
        {
            interval = msInterval * 10000;
        }

        public static void waitForTheRightMoment()
        {
            float a = ((DateTime.UtcNow - lastTickTime).Ticks / 10000f);
            if (a > 10)
                Console.WriteLine(a.ToString("F2"));

            while ((DateTime.UtcNow - lastTickTime).Ticks < interval) System.Threading.Thread.Sleep(5);  // dogru zaman gelene kadar busy wait yapiyor. 
            // En onemli thread bu oldugu icin polling den kacinmadim.
            // gelistirilebilir.

            deltaTime = ((float)(DateTime.UtcNow - lastTickTime).TotalSeconds);//.Ticks) / (10000.0f * 1000.0f);

            lastTickTime = DateTime.UtcNow;

        }

    }
}
