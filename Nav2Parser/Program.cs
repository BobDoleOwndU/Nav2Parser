using System;
using System.IO;

namespace Nav2Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (Path.GetExtension(args[0]) == ".nav2")
                {
                    Nav2 nav2 = new Nav2();
                    nav2.Read(args[0]);

                    bool exit = false;

                    while (!exit)
                    {
                        Console.WriteLine("Select an option:");
                        Console.WriteLine("1 - Header Info");
                        Console.WriteLine("2 - Manifest Info");
                        Console.WriteLine("3 - NavWorld Info");
                        Console.WriteLine("0 - Exit");

                        switch (Console.ReadLine())
                        {
                            case "0":
                                exit = true;
                                break;
                            case "1":
                                nav2.DisplayHeaderInfo();
                                break;
                            case "2":
                                nav2.DisplayManifestInfo();
                                break;
                            case "3":
                                nav2.DisplayNavWorldInfo();
                                break;
                            default:
                                Console.WriteLine("\nInvalid option!\n");
                                break;
                        } //switch
                    } //while
                } //if
                else
                {
                    Console.WriteLine("Invalid File");
                } //else
            } //if
        } //Main
    } //class
} //namespace
