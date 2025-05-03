using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

class WifiGrabber
{
    static void Banner()
    {
        string[] bannerLines = new string[]
        {
            "+======================================================================================================+",
            "|    ___       __   ___                 ________ ___          ___  ________   ________ ________        |",
            "|   |\\  \\     |\\  \\|\\  \\               |\\  _____\\\\  \\        |\\  \\|\\   ___  \\|\\  _____\\\\   __  \\       |",
            "|   \\ \\  \\    \\ \\  \\ \\  \\  ____________\\ \\  \\__/\\ \\  \\       \\ \\  \\ \\  \\\\ \\  \\ \\  \\__/\\ \\  \\|\\  \\      |",
            "|    \\ \\  \\  __\\ \\  \\ \\  \\|\\____________\\ \\   __\\\\ \\  \\       \\ \\  \\ \\  \\\\ \\  \\ \\   __\\\\ \\  \\\\\\  \\     |",
            "|     \\ \\  \\|\\__\\_\\  \\ \\  \\|____________|\\ \\  \\_| \\ \\  \\       \\ \\  \\ \\  \\\\ \\  \\ \\  \\_| \\ \\  \\\\\\  \\    |",
            "|      \\ \\____________\\ \\__\\              \\ \\__\\   \\ \\__\\       \\ \\__\\ \\__\\\\ \\__\\ \\__\\   \\ \\_______\\   |",
            "|       \\|____________|\\|__|               \\|__|    \\|__|        \\|__|\\|__| \\|__|\\|__|    \\|_______|   |",
            "|                                                                                                      |",
            "|    ________  ________  ________  ________  ________  _______   ________                              |",
            "|   |\\   ____\\|\\   __  \\|\\   __  \\|\\   __  \\|\\   __  \\|\\  ___ \\ |\\   __  \\                             |",
            "|   \\ \\  \\___|\\ \\  \\|\\  \\ \\  \\|\\  \\ \\  \\|\\ /\\ \\  \\|\\ /\\ \\   __/|\\ \\  \\|\\  \\                            |",
            "|    \\ \\  \\  __\\ \\   _  _\\ \\   __  \\ \\   __  \\ \\   __  \\ \\  \\_|/_\\ \\   _  _\\                           |",
            "|     \\ \\  \\|\\  \\ \\  \\\\  \\\\ \\  \\ \\  \\ \\  \\|\\  \\ \\  \\|\\  \\ \\  \\_|\\ \\ \\  \\\\  \\|                          |",
            "|      \\ \\_______\\ \\__\\\\ _\\\\ \\__\\ \\__\\ \\_______\\ \\_______\\ \\_______\\ \\__\\\\ _\\                          |",
            "|       \\|_______|\\|__|\\|__|\\|__|\\|__|\\|_______|\\|_______|\\|_______|\\|__|\\|__|                         |",
            "+======================================================================================================+ " +
            "                                Add Discord - threadline_"
        };

        ConsoleColor[] colors = new ConsoleColor[]
        {
            ConsoleColor.Blue,
            ConsoleColor.Cyan,
            ConsoleColor.Magenta,
            ConsoleColor.Yellow,
            ConsoleColor.Green,
            ConsoleColor.DarkCyan,
            ConsoleColor.Red
        };

        int colorIndex = 0;
        foreach (string line in bannerLines)
        {
            Console.ForegroundColor = colors[colorIndex];
            Console.WriteLine(line);
            colorIndex = (colorIndex + 1) % colors.Length;
        }

        Console.ResetColor();
    }

    static void Main(string[] args)
    {
        Console.Title = "Wi-Fi Information Grabber";

        while (true)
        {
            Console.Clear();
            Banner();
            Console.WriteLine("=== Wi-Fi Profile Information Grabber ===\n");

            try
            {
                string profiles = RunCmd("netsh wlan show profiles");

                MatchCollection ssids = Regex.Matches(profiles, @"All User Profile\s*:\s*(.+)");
                if (ssids.Count == 0)
                {
                    Console.WriteLine("No Wi-Fi profiles found.");
                }
                else
                {
                    string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "wifi_passwords.txt");
                    using (StreamWriter sw = new StreamWriter(outputPath, false))
                    {
                        foreach (Match match in ssids)
                        {
                            string ssid = match.Groups[1].Value.Trim();
                            string profileInfo = RunCmd($"netsh wlan show profile name=\"{ssid}\" key=clear");

                            string password = "N/A";
                            Match keyMatch = Regex.Match(profileInfo, @"Key Content\\s*:\\s*(.+)");
                            if (keyMatch.Success)
                                password = keyMatch.Groups[1].Value.Trim();

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"[+] SSID: {ssid}");
                            Console.WriteLine($"    Password: {password}\n");
                            Console.ResetColor();

                            sw.WriteLine($"SSID: {ssid}\nPassword: {password}\n");
                        }
                    }

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[!] Wi-Fi data saved to: {outputPath}");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: " + ex.Message);
                Console.ResetColor();
            }

            Console.WriteLine("\nWould you like to run the scan again? (Y/N)");
            Console.Write(">>> ");
            string input = Console.ReadLine().ToLower();

            if (input != "y" && input != "yes")
            {
                Console.WriteLine("\nExiting... press any key to close.");
                Console.ReadKey();
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
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8
        };

        using (Process proc = Process.Start(psi))
        {
            return proc.StandardOutput.ReadToEnd();
        }
    }
}
