🚀 Wi-Fi Information Grabber



🎯 What is This Tool?
The Wi-Fi Information Grabber is a lightweight but powerful C# console application that scans your system for all saved Wi-Fi profiles and cleanly extracts their stored passwords (if available) — all formatted and saved to your Desktop in seconds.

It’s simple, sleek, effective, and perfect for personal recovery, network audits, or system backups.

✨ Features
✅ Displays all stored SSIDs (Wi-Fi names)
✅ Reveals stored plaintext passwords (if available)
✅ Saves everything to your Desktop (wifi_passwords.txt)
✅ Clean, looped UI — run continuously until you exit
✅ 🔥 Custom multi-colored ASCII banner
✅ 💬 Console-based interface — no bloat, no GUI needed
✅ Written 100% in pure C#, no external libraries

🖼️ Screenshot
🧠 Wanna know what it looks like in action? Check it out below:

![image](https://github.com/user-attachments/assets/e26d9507-3764-4454-b0b9-85b4c0f75b5a)

![image](https://github.com/user-attachments/assets/4f5bb422-13dc-4324-84ce-c8fb9e09a324)


📂 Output Example

SSID: Home_Network
Password: helloworld123

SSID: WorkWiFi_5G
Password: WorkStrongPass!

SSID: School_Guest
Password: N/A
File gets saved at:

C:\Users\<YourName>\Desktop\wifi_passwords.txt
⚙️ How It Works
Behind the scenes, the tool uses Windows’ built-in netsh wlan commands to:

List all saved wireless profiles.

Loop through each profile and reveal the Key Content (password).

Format and save the results cleanly.

No 3rd-party dependencies. 100% native.

💻 System Requirements
🪟 Windows 10/11
🛠 .NET 6.0 or newer (or use compiled .exe)
📡 Admin permissions (for accurate output)

🔐 Disclaimer
⚠️ This tool is intended for ethical use only.

It’s designed for:

Recovering passwords from your own system

Assisting authorized IT professionals

Performing audits on trusted environments

DO NOT use this on any system or network you do not have explicit permission to access. Misuse may be illegal depending on your jurisdiction.

👨‍💻 Author
Created by Threadline
📎 Add me on Discord: threadline_
🌐 GitHub: Threadlinee
🧠 More tools coming soon...

