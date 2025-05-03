using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;

class WiFiGrabber
{
    static void Main()
    {
        Console.Title = "📶 Wi-Fi Info Grabber | threadline_";
        RunGrabberLoop();
    }

    static void RunGrabberLoop()
    {
        while (true)
        {
            Console.Clear();
            ShowBanner();

            string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "wifi_passwords.txt");
            File.WriteAllText(outputPath, string.Empty); // Clear file

            string profiles = RunCmd("netsh wlan show profiles");
            MatchCollection ssids = Regex.Matches(profiles, @"(?:All User Profile|Nombre de perfil|Profil\s*:\s*|Perfil de todos los usuarios)\s*:\s*(.+)");

            if (ssids.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[!] No Wi-Fi profiles found.");
                Console.ResetColor();
                return;
            }

            foreach (Match match in ssids)
            {
                string ssid = match.Groups[1].Value.Trim().Trim('"');
                string password = "N/A";

                string profileInfo = RunCmd($"netsh wlan show profile name=\"{ssid}\" key=clear");

                Match keyMatch = Regex.Match(profileInfo, @"Key Content\s*:\s*(.+)");
                if (keyMatch.Success)
                    password = keyMatch.Groups[1].Value.Trim();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[+] SSID: {ssid}");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"    Password: {password}");
                Console.ResetColor();

                File.AppendAllText(outputPath, $"SSID: {ssid} | Password: {password}\n");
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n[!] Wi-Fi data saved to: {outputPath}");
            Console.Write("\n[?] Run scan again? (Y/N): ");
            Console.ResetColor();

            string choice = Console.ReadLine().Trim().ToLower();
            if (choice != "y")
            {
                Console.WriteLine("\n[✓] Exiting...");
                Thread.Sleep(1000);
                break;
            }
        }
    }

    static string RunCmd(string cmd)
    {
        ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/c " + cmd)
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        Process proc = Process.Start(psi);
        string output = proc.StandardOutput.ReadToEnd();
        proc.WaitForExit();
        return output;
    }

    static void ShowBanner()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("+======================================================================================================+");
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("|   ███████╗██╗    ██╗██╗███████╗██╗     ██╗███████╗ ██████╗ ██████╗ ██████╗ ██████╗  █████╗ ██████╗    |");
        Console.WriteLine("|   ██╔════╝██║    ██║██║██╔════╝██║     ██║██╔════╝██╔════╝ ██╔══██╗██╔══██╗██╔══██╗██╔══██╗██╔══██╗   |");
        Console.WriteLine("|   █████╗  ██║ █╗ ██║██║█████╗  ██║     ██║███████╗██║  ███╗██████╔╝██████╔╝██║  ██║███████║██████╔╝   |");
        Console.WriteLine("|   ██╔══╝  ██║███╗██║██║██╔══╝  ██║     ██║╚════██║██║   ██║██╔═══╝ ██╔═══╝ ██║  ██║██╔══██║██╔═══╝    |");
        Console.WriteLine("|   ███████╗╚███╔███╔╝██║███████╗███████╗██║███████║╚██████╔╝██║     ██║     ██████╔╝██║  ██║██║        |");
        Console.WriteLine("|   ╚══════╝ ╚══╝╚══╝ ╚═╝╚══════╝╚══════╝╚═╝╚══════╝ ╚═════╝ ╚═╝     ╚═╝     ╚═════╝ ╚═╝  ╚═╝╚═╝        |");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("+======================================================================================================+");
        Console.ResetColor();
        Console.WriteLine("🔥 Wi-Fi Profile & Password Info Grabber — by threadline_ | GitHub: https://github.com/threadline");
        Console.WriteLine("------------------------------------------------------------------------------------------------------\n");
    }
}
