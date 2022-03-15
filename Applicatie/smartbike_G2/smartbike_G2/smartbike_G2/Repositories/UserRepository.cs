using System.Diagnostics;

namespace smartbike_G2.Repositories
{
    public class UserRepository
    {
        public static int Score { get; set; } = 0;
        public static float Distance { get; set; } = 0;
        public static void UpdateScore(float speed, float curTime, float prevTime)
        {
            float time = (curTime - prevTime);
            if (time < 0)
            {
                time += 65536;
                Debug.WriteLine("\tWarning: Integer overflow on 'time',\n\trecalculated to:" + time);
            }
            float increment = speed *time / 1000;

            Distance += increment / 3600F*1000;            
        }
    }
}
