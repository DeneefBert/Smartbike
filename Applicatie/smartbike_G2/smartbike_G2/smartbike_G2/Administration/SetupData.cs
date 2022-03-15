using System;
using System.Collections.Generic;
using System.Text;

namespace smartbike_G2.Administration
{
    public class SetupData
    {
        // Het youtube kanaal ID, pas het gedeelte tussen de "" aan
        public static string ChannelID = "UCbsvZtmBI1ORoOjpmll7F1g";

        // De initiële minimum snelheid
        public static int MinimumSpeed = 10;

        // Het interval waarmee de snelheid tijdens videos omhoog gaat (in km/u)
        public static int SpeedInterval = 1;

        // Het tijdsinterval waarin de snelhijd tijdens videos omhoog gaat (in seconden)
        public static int TimeInterval = 60;

        // Maximum level snelheid - De hometrainer waar we mee testen ging af als de snelheid boven 30km/u ging
        public static int MaxSpeed = 25;

        // Standaard playlists voor videos en muziek, de eerste die wordt weergegeven als men op die tabs drukt
        public static string DefaultVideos = "PLnGa7tcaV-U9If08VEokt3vAlz-4gFy6l";
        public static string DefaultMusic = "PLnGa7tcaV-U-RPAsyhslu2nv8LfiDwv_9";

    }
}
