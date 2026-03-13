using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

class WinCDInstaller
{
    static string espLetter;
    static string winLetter;
    static string cdLetter;

    static void Main()
    {
        Console.Title = "Windows 10 1809 CD Setup";

        Welcome();

        RunDiskPart();

        AskPartitionLetters();

        CopyDiscs();

        int index = SelectEdition();

        ApplyWindows(index);

        InstallBootloader();

        Finish();
    }

    static void Welcome()
    {
        Console.WriteLine("Windows 10 1809 (x64) Setup");
        Console.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=\n");
        Console.WriteLine("Welcome to Windows CD setup. Press any key to begin...");
        Console.ReadKey();
    }

    static void RunDiskPart()
    {
        Console.Clear();

        Console.WriteLine("[*] Partition and format your drive using DiskPart.");
        Console.WriteLine("[*] Please identify your CD/DVD drive using list vol.\n");
        Console.WriteLine("Close DiskPart when finished.\n");

        Process p = new Process();
        p.StartInfo.FileName = "diskpart.exe";
        p.Start();
        p.WaitForExit();
    }

    static void AskPartitionLetters()
    {
        Console.Write("\n[-] Specify the ESP letter: ");
        espLetter = Console.ReadLine().ToUpper();

        Console.Write("[-] Specify the primary partition letter: ");
        winLetter = Console.ReadLine().ToUpper();

        Console.Write("[-] Specify the CD/DVD drive letter: ");
        cdLetter = Console.ReadLine().ToUpper();

        Directory.CreateDirectory($"{winLetter}:\\sources");
    }

    static void CopyDiscs()
    {
        long totalWritten = 0;

        for (int i = 1; i <= 8; i++)
        {
            Console.WriteLine($"\n[-] Insert disc #{i} into drive {cdLetter}: and press any key to continue.");
            Console.ReadLine();

            string src = $"{cdLetter}:\\install{i}.swm";

            if (!File.Exists(src))
            {
                Console.WriteLine("Required file not found on disc.");
                i--;
                continue;
            }

            string dst;

            if (i == 1)
                dst = $"{winLetter}:\\sources\\install.swm";
            else
                dst = $"{winLetter}:\\sources\\install{i}.swm";

            CopyWithProgress(src, dst, ref totalWritten);

            Console.WriteLine($"\nProcessed disc {i}/8. Written {totalWritten / (1024 * 1024)} MB.");
        }
    }

    static void CopyWithProgress(string source, string dest, ref long totalWritten)
    {
        const int bufferSize = 1024 * 1024;
        byte[] buffer = new byte[bufferSize];

        using FileStream src = new FileStream(source, FileMode.Open, FileAccess.Read);
        using FileStream dst = new FileStream(dest, FileMode.Create, FileAccess.Write);

        long size = src.Length;
        long copied = 0;

        int read;

        while ((read = src.Read(buffer, 0, buffer.Length)) > 0)
        {
            dst.Write(buffer, 0, read);
            copied += read;
            totalWritten += read;

            DrawProgressBar(copied, size);
        }
    }

    static void DrawProgressBar(long progress, long total)
    {
        int width = 30;
        double percent = (double)progress / total;

        int filled = (int)(percent * width);

        Console.CursorLeft = 0;
        Console.Write("[");

        for (int i = 0; i < width; i++)
        {
            if (i < filled) Console.Write("█");
            else Console.Write("-");
        }

        Console.Write($"] {(percent * 100):0}%   {progress / (1024 * 1024)}MB");
    }

    static int SelectEdition()
    {
        Console.Clear();

        string[] editions =
        {
            "[1] Windows 10 Education",
            "[2] Windows 10 Education N",
            "[3] Windows 10 Enterprise",
            "[4] Windows 10 Enterprise N",
            "[5] Windows 10 Pro",
            "[6] Windows 10 Pro N",
            "[7] Windows 10 Pro Education",
            "[8] Windows 10 Pro Education N",
            "[9] Windows 10 Pro for Workstations",
            "[10] Windows 10 Pro N for Workstations"
        };

        Console.WriteLine("Installation successful!");
        Console.WriteLine("[*] Which version should be deployed?\n");

        Console.WriteLine($"{editions}");

        Console.Write("\n[-] Enter your choice: ");

        return int.Parse(Console.ReadLine());
    }

    static void ApplyWindows(int index)
    {
        Console.WriteLine("\nApplying Windows image...\n");

        RunCommand(
            $"dism /apply-image /imagefile:{winLetter}:\\sources\\install.swm " +
            $"/swmfile:{winLetter}:\\sources\\install*.swm " +
            $"/index:{index} /applydir:{winLetter}:\\"
        );
    }

    static void InstallBootloader()
    {
        Console.WriteLine("\nCreating boot files...\n");

        RunCommand($"bcdboot {winLetter}:\\Windows /s {espLetter}:");
    }

    static void Finish()
    {
        Console.WriteLine("\nCongratulations! Windows 10 1809 has been installed on your computer.");
        Console.WriteLine("Press any key to reboot.");

        Console.ReadLine();

        RunCommand("wpeutil reboot");
    }

    static void RunCommand(string cmd)
    {
        Process p = new Process();
        p.StartInfo.FileName = "cmd.exe";
        p.StartInfo.Arguments = "/c " + cmd;
        p.StartInfo.UseShellExecute = false;

        p.Start();
        p.WaitForExit();
    }
}