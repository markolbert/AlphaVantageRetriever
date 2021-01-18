using System;
using System.IO;

namespace J4JSoftware.AlphaVantageCSVRetriever
{
    public partial class Configuration
    {
        public static string GetDefaultOutputFile()
        {
            return Environment.OSVersion.Platform switch
            {
                PlatformID.Win32NT => get_windows_default(),
                PlatformID.Win32S => get_windows_default(),
                PlatformID.Win32Windows => get_windows_default(),
                PlatformID.WinCE => get_windows_default(),
                PlatformID.Xbox => get_windows_default(),
                _ => Path.Combine( Environment.CurrentDirectory, "AlphaVantage.csv" )
            };

            string get_windows_default()
            {
                return Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.Desktop ),
                    "AlphaVantage.csv" );
            }
        }
    }
}