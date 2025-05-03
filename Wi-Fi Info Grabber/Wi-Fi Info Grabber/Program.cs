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
            File.WriteAllText(outputPath, ""); // Reset file

            string profilesOutput = RunCmd("netsh wlan show profiles");
            MatchCollection ssids = Regex.Matches(profilesOutput, @"All User Profile\s*:\s*(.+)", RegexOptions.IgnoreCase);

            if (ssids.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[!] No Wi-Fi profiles found.");
                Console.ResetColor();
                Pause();
                return;
            }

            foreach (Match match in ssids)
            {
                string ssid = match.Groups[1].Value.Trim();
                string password = "N/A";

                try
                {
                    string profileDetails = RunCmd($"netsh wlan show profile name=\"{ssid}\" key=clear");

                    // Works in EN, ES, FR, DE, JP, RU, etc.
                    Match keyMatch = Regex.Match(profileDetails,
                        @"(?:Key Content|Contenido de la clave|Contenu de la clé|Schlüsselinhalt|Clef du contenu|キーの内容|Содержимое ключа)\s*:\s*(.+)",
                        RegexOptions.IgnoreCase);

                    if (keyMatch.Success)
                        password = keyMatch.Groups[1].Value.Trim();
                }
                catch (Exception ex)
                {
                    password = $"[Error: {ex.Message}]";
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[+] SSID: {ssid}");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"    Password: {password}");
                Console.ResetColor();

                File.AppendAllText(outputPath, $"SSID: {ssid} | Password: {password}\n");
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n[✓] Wi-Fi data saved to: {outputPath}");
            Console.ResetColor();

            Console.Write("\n[?] Run scan again? (Y/N): ");
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

        using (Process proc = Process.Start(psi))
        {
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            return output;
        }
    }

    static void ShowBanner()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("+==================== Wi-Fi Grabber by threadline_ ====================+");
        Console.ResetColor();
    }

    static void Pause()
    {
        Console.Write("\nPress any key to return...");
        Console.ReadKey();
    }
}
