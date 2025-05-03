using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class WifiGrabber
{
    static void Banner()
    {
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine(@"
/* +======================================================================================================+ */
/* |    ___       __   ___                 ________ ___          ___  ________   ________ ________        | */
/* |   |\  \     |\  \|\  \               |\  _____\\  \        |\  \|\   ___  \|\  _____\\   __  \       | */
/* |   \ \  \    \ \  \ \  \  ____________\ \  \__/\ \  \       \ \  \ \  \\ \  \ \  \__/\ \  \|\  \      | */
/* |    \ \  \  __\ \  \ \  \|\____________\ \   __\\ \  \       \ \  \ \  \\ \  \ \   __\\ \  \\\  \     | */
/* |     \ \  \|\__\_\  \ \  \|____________|\ \  \_| \ \  \       \ \  \ \  \\ \  \ \  \_| \ \  \\\  \    | */
/* |      \ \____________\ \__\              \ \__\   \ \__\       \ \__\ \__\\ \__\ \__\   \ \_______\   | */
/* |       \|____________|\|__|               \|__|    \|__|        \|__|\|__| \|__|\|__|    \|_______|   | */
/* +======================================================================================================+ */
");
    }

    static void Main(string[] args)
    {
        Console.Title = "Wi-Fi Information Grabber";
        Console.WriteLine("=== Wi-Fi Profile Information Grabber ===\n");

        try
        {
            string profiles = RunCmd("netsh wlan show profiles");
            Console.WriteLine("[*] Profiles fetched:\n" + profiles); // Debugging output to check if profiles are fetched

            MatchCollection ssids = Regex.Matches(profiles, @"All User Profile\s*:\s*(.+)");
            if (ssids.Count == 0)
            {
                Console.WriteLine("No Wi-Fi profiles found.");
                return;
            }

            string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "wifi_passwords.txt");
            using (StreamWriter sw = new StreamWriter(outputPath, false))
            {
                foreach (Match match in ssids)
                {
                    string ssid = match.Groups[1].Value.Trim();
                    Console.WriteLine($"[+] Checking SSID: {ssid}");

                    string profileInfo = RunCmd($"netsh wlan show profile name=\"{ssid}\" key=clear");
                    Console.WriteLine("[*] Profile Info:\n" + profileInfo); // Debugging output to check profile info

                    string password = "N/A";
                    Match keyMatch = Regex.Match(profileInfo, @"Key Content\s*:\s*(.+)");
                    if (keyMatch.Success)
                    {
                        password = keyMatch.Groups[1].Value.Trim();
                    }

                    Console.WriteLine($"[+] SSID: {ssid}");
                    Console.WriteLine($"    Password: {password}\n");

                    sw.WriteLine($"SSID: {ssid}\nPassword: {password}\n");
                }
            }

            Console.WriteLine($"[!] Wi-Fi data saved to: {outputPath}");

            // Ask for re-run option
            Console.Write("\nWould you like to run the scan again? (Y/N): ");
            string input = Console.ReadLine();
            if (input.ToUpper() == "Y")
            {
                Main(null);  // restart the program
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    static string RunCmd(string cmd)
    {
        ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/c " + cmd)
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8
        };

        using (Process proc = Process.Start(psi))
        {
            return proc.StandardOutput.ReadToEnd();
        }
    }
}
