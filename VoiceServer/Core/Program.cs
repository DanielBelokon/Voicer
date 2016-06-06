using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Voicer.Common;

namespace VoiceServer
{
    class Program
    {
        static Server server;

        static void Main(string[] args)
        {  
            Console.CursorVisible = false;

            server = new Server();
            do
            {
                string command = "";
                ConsoleKeyInfo lastKey;

                do
                {
                    lastKey = Console.ReadKey(true);
                    // If key was delete, remove last char of string.
                    if (command != "" && (lastKey.Key == ConsoleKey.Backspace || lastKey.Key == ConsoleKey.Delete))
                    {
                        command = command.Remove(command.Length - 1);
                    }
                    else if (lastKey.Key != ConsoleKey.Enter)
                    {
                        command += lastKey.KeyChar;
                    }

                    WriteLastLine(command);
                } while (lastKey.Key != ConsoleKey.Enter);

                // Type the command in the 'normal' area, and clear the bottom line.
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(command);
                Console.ForegroundColor = ConsoleColor.Gray;

                WriteLastLine(new string(' ', Console.BufferWidth));

                //command = command.ToLower();
                string[] inputs = command.Split(' ');
                DoCommand(inputs[0], inputs);

            } while (server.IsListening);
            Administration.SaveServerKeys();
            Console.WriteLine("Exited");
            Console.ReadLine();
        }

        static void DoCommand(string command, string[] prms)
        {
            switch (command)
            {
                case "shutdown":
                    server.Stop();
                    break;

                case "kick":
                    try
                    {
                        short id = short.Parse(prms[1]);
                        server.Kick(id);

                    }
                    catch (FormatException)
                    {
                        server.Kick(prms[1]);
                    }

                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine("You must enter valid parameters.");
                    }

                    break;

                case "say":

                    break;

                case "restart":

                    break;

                case "setadmin":
                    ServerClient client;
                    try
                    {
                        short id = short.Parse(prms[1]);
                        client = server.FindClient(id);
                        client.SetAdmin(true);

                    }
                    catch (FormatException)
                    {
                        client = server.FindClient(prms[1]);
                        client.SetAdmin(true);
                    }

                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine("You must enter valid parameters.");
                        return;
                    }

                    
                    break;

                default:
                    Console.WriteLine("invalid command");
                    break;
            }
        }

        // Write text on the last line of console, used for input.
        static void WriteLastLine(string text)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            int x = Console.CursorLeft;
            int y = Console.CursorTop;
            Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;
            Console.Write(text);
            // Restore previous position
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(x, y);
        }
    }
}
