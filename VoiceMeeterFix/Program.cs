using Microsoft.Win32.TaskScheduler;
using System.Configuration;
using System.Collections.Specialized;
using System;
using System.Diagnostics;
using System.Security.Principal;

namespace VoiceMeeterFix
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (IsAdmin() == false)
                Console.WriteLine("[ERROR]: Process needs to be ran as ADMIN! >:C | Curious/Unsure? Contact Fifi#2000 for more info.\n");

            string setup;
            setup = ConfigurationManager.AppSettings.Get("setup");

            try
            {
                if (setup == "false")
                {
                    using (TaskService ts = new TaskService())
                    {
                        TaskDefinition td = ts.NewTask();
                        td.RegistrationInfo.Description = "Runs the VoiceMeeter mic fix | Fifi#2000";
                        td.RegistrationInfo.Author = "Fifi#2000";
                        td.Triggers.Add(new LogonTrigger { Enabled = true });
                        td.Actions.Add(new ExecAction($"C:\\Users\\{Environment.UserName}\\Documents\\VmFix\\VoiceMeeterFix.exe", null, null));
                        td.Principal.LogonType = TaskLogonType.S4U;
                        td.Principal.RunLevel = TaskRunLevel.Highest;
                        td.Settings.Compatibility = TaskCompatibility.V2_3;
                        ts.RootFolder.RegisterTaskDefinition(@"VoiceMeeterFix", td);

                        Console.WriteLine("[INFO]: Task Schedule Created");
                    }

                    // Change setup value to true
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["setup"].Value = "true";
                    config.Save(ConfigurationSaveMode.Full, true);
                    ConfigurationManager.RefreshSection("appSettings");

                    Console.WriteLine("[SUCCESS]: Setup Finished\n[WARN]: Make sure to set the \"VoiceMeeterFix.exe\" file in Documents\\VmFix! Press any key to continue...");
                    Console.ReadKey(); 
                } else if (setup == "true")
                {
                    // Make sure the audiodg.exe process is running.
                    Process[] audiodg = Process.GetProcessesByName("audiodg");

                    // Retrieve the audiodg process (Windows starts this automatically).
                    audiodg = Process.GetProcessesByName("audiodg");
                    using (Process app = audiodg[0])
                    {
                        // Set the properties to audiodg
                        Process.GetProcessesByName("audiodg")[0].ProcessorAffinity = (IntPtr)1;

                        //System.Threading.Thread.Sleep(15000);
                        //Process.GetProcessesByName("voicemeeterpro")[0].ProcessorAffinity = (IntPtr)4;

                        Console.WriteLine("[INFO]: Process Affinity Set!");
                        Console.WriteLine("[SUCCESS]: Finished VoiceMeeter fix! Press any key to continue...");
                        // Console.ReadKey();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR]: {e.Message}\n[INFO]: Questions? Contact Fifi#2000 | Press any key to continue...");
                Console.ReadKey();
            }
        }

        public static bool IsAdmin()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
