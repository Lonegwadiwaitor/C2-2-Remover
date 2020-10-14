using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Net;
namespace C2_2
{
    class Program
    {
        /* 
         * The first simple C# project by Gladiator that isn't written in tardcode!
         * Report any issues on Discord, Glad#8906 (ID: 582558971722727424)
         */
        static void Main(string[] args)
        {
            Console.Title = "C2-2 Fixer by Gladiator#8906 cus im bored as fuck";
            Thread.Sleep(2000);
            var discorddesktopcorepath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Discord\\0.0.308\\modules\\discord_desktop_core";
            if (!File.ReadAllText($"{discorddesktopcorepath}\\index.js").Equals("module.exports = require('./core.asar');"))
            {
                Console.WriteLine("C2-2 detected, would you like to attempt removal? (Y/N)");
                var reply = Console.ReadLine();
                if (reply.StartsWith("y", StringComparison.CurrentCultureIgnoreCase))
                {
                    Console.WriteLine("Killing Discord...");
                    foreach (var proc in Process.GetProcessesByName("Discord")) proc.Kill();
                    Console.WriteLine("Starting Removal...");
                    Remover();
                }
                else if (reply.StartsWith("n", StringComparison.CurrentCultureIgnoreCase)) Environment.Exit(1);
            }
            else
            {
                Console.WriteLine("You don't seem to be infected by C2-2");
                Thread.Sleep(5000);
                Environment.Exit(1);
            }
            
        }

        public static void Remover()
        {
            bool AnarchyGrabber = false;
            // whitelisted files that wont get deleted
            var whitelisted = new List<string>
            {
                "core.asar",
                "index.js",
                "package.json"
            };
            // lists of files to delete
            List<string> filestodelete = new List<string>();
            List<string> dirstodelete = new List<string>();
            var discorddesktopcorepath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Discord\\0.0.308\\modules\\discord_desktop_core";
            // check for retarded anarchygrabber bullshit
            if (Directory.Exists($"{discorddesktopcorepath}\\4n4rchy") && File.ReadAllText($"{discorddesktopcorepath}\\index.js").Contains("process.env.anarchyHook"))
            {
                Console.WriteLine("AnarchyGrabber detected");
                AnarchyGrabber = true;
            }
            // check for random files that shouldnt be there
            foreach(var files in Directory.GetFiles(discorddesktopcorepath))
            {
                if (!whitelisted.Contains(Path.GetFileName(files)))
                {
                    filestodelete.Add(files);
                }
            }
            // check for random directories that shouldnt be there
            foreach (var directory in Directory.GetDirectories(discorddesktopcorepath))
            {
                dirstodelete.Add(directory);
            }
            Console.WriteLine("Done!\n");
            // are there files to delete?
            if (filestodelete.Count > 0)
            {
                Console.WriteLine("The following files will be deleted:");
                foreach (var files in filestodelete)
                {
                    Console.WriteLine($"- {files}");
                }
            }
            // are there directories to delete?
            if (dirstodelete.Count > 0)
            {
                Console.WriteLine("The following directories will be deleted:");
                foreach(var dirs in dirstodelete)
                {
                    Console.WriteLine($"- {dirs}");
                }
            }
            // do you want to delete them?
            Console.WriteLine("Press Y to continue or N to quit");
            var read = Console.ReadLine();
            if (read.StartsWith("y", StringComparison.CurrentCultureIgnoreCase))
            {
                foreach (var files in filestodelete)
                {
                    try
                    {
                        File.Delete(files);
                    }
                    catch { }
                }
                foreach (var dirs in dirstodelete)
                {
                    try
                    {
                        foreach(var filesindir in Directory.GetFiles(dirs))
                        {
                            File.Delete(filesindir);
                        }
                        Directory.Delete(dirs);
                    }
                    catch { }
                }
                var discordwebhookendpoint = "https://discordapp.com/api/webhooks/";
                // delete retarded anarchygrabber webhooks
                if (AnarchyGrabber)
                {
                    foreach (var line in File.ReadLines(discorddesktopcorepath + "\\index.js"))
                    {
                        if (line.Contains("process.env.anarchyHook"))
                        {
                            var token = line.Substring(27, 87);
                            var request = (HttpWebRequest)WebRequest.Create(discordwebhookendpoint + token);
                            request.Method = "DELETE";
                            // this throws an exception upon error code so its wrapped
                            try
                            {
                                request.GetResponse();
                            }
                            catch { }
                        }
                    }
                }
                File.WriteAllText(discorddesktopcorepath + "\\index.js", "module.exports = require('./core.asar');");
                Console.WriteLine("All done! You should now be able to use Synapse.");
                if (AnarchyGrabber)
                {
                    Console.WriteLine("Please change your Discord password, you will not be unblacklisted if your account is hijacked and used maliciously.");
                }
                // user has to close application, this is done to give the user a chance to read everything
                Thread.Sleep(-1);
            }
            // kill program if user specifies no
            else Environment.Exit(1);
        }
    }
}
