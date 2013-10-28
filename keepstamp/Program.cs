using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace keepstamp
{
    class Program
    {
        static void PrintHelp ()
        {
            Console.WriteLine ("Usage:\t keepstamp [MODE] \"C:\\folder\" \"C:\\logfile.txt\"");
            Console.WriteLine ();
            Console.WriteLine ("Modes:");
            Console.WriteLine ("\t-b, --backup");
            Console.WriteLine ();
            Console.WriteLine ("\t\tSave timestamps of the given folder to the given logfile");
            Console.WriteLine ();
            Console.WriteLine ("\t-r, --restore");
            Console.WriteLine ();
            Console.WriteLine ("\t\tRestore timestamps in the given folder from the given logfile");
        }

        static void PrintError (string errorText)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine (errorText);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        static void Main (string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Gray;

            if (args.Length != 3)
            {
                PrintHelp ();
                return;
            }

            Int64 tickcount;
            string logentry;
            string path = args[1];
            string logpath = args[2];

            if (args[0].Equals ("-b") || args[0].Equals ("--backup"))
            {
                if (File.Exists(logpath))
                {
                    PrintError("Logfile already exists, use a different filename!");
                }
                else if (!Directory.Exists(path))
                {
                    PrintError("Folder not found!");
                }
                else
                {
                    string[] list = Directory.EnumerateFileSystemEntries(path, "*", SearchOption.AllDirectories).ToArray<string>();
                    foreach (string item in list)
                    {
                        DateTime date = File.GetLastWriteTime(item);
                        tickcount = date.Ticks;
                        logentry = tickcount + " " + item;
                        Console.WriteLine(logentry);
                        File.AppendAllText(logpath, logentry);
                        File.AppendAllText(logpath, "\r\n");
                    }
                }
            }
            else if (args[0].Equals ("-r") || args[0].Equals ("--restore"))
            {
                if (File.Exists(logpath))
                {


                    foreach (string line in File.ReadLines(logpath))
                    {
                        tickcount = Convert.ToInt64(line.Substring(0, 18));
                        DateTime date = new DateTime(tickcount);

                        string filename = line.Substring(19);

                        if (File.Exists(filename))
                        {
                            File.SetLastWriteTime(filename, date);
                            Console.WriteLine(filename);
                        }
                        else if (Directory.Exists(filename))
                        {
                            Directory.SetLastWriteTime(filename, date);
                            Console.WriteLine(filename);
                        }
                        else
                        {
                            PrintError (filename + " not found!");
                        }

                    }
                }
                else
                {
                    PrintError ("Logfile not found!");
                }
            }
            else
            {
                PrintHelp ();
                return;
            }
        }
    }
}
