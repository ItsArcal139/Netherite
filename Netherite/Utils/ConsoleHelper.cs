using System;

namespace Netherite.Utils
{
    internal static class ConsoleHelper
    {
        public static void WriteAscii(string line)
        {
            string content = "";
            bool isEscaped = false;
            string parameter = "";

            foreach (char s in line.ToCharArray())
            {
                if (!isEscaped)
                {
                    if (s == 0x1b)
                    {
                        // Escape character.
                        isEscaped = true;
                        Console.Write(content);
                        content = "";
                    }
                    else
                    {
                        content += s;
                    }
                }
                else
                {
                    if (s == '[') continue;

                    if (s == 'm')
                    {
                        isEscaped = false;

                        // Parse the parameter.
                        string[] p = parameter.Split(';');
                        try
                        {
                            bool isBright = false;
                            // bool isReset = false;
                            foreach (string str in p)
                            {
                                int n = int.Parse(str);
                                switch (n)
                                {
                                    case 0:
                                        isBright = false;
                                        Console.BackgroundColor = ConsoleColor.Black;
                                        Console.ForegroundColor = ConsoleColor.Gray;
                                        break;
                                    case 1:
                                        isBright = true;
                                        break;
                                    case 30:
                                        Console.ForegroundColor = !isBright ? ConsoleColor.Black : ConsoleColor.DarkGray;
                                        break;
                                    case 31:
                                        Console.ForegroundColor = !isBright ? ConsoleColor.DarkRed : ConsoleColor.Red;
                                        break;
                                    case 32:
                                        Console.ForegroundColor = !isBright ? ConsoleColor.DarkGreen : ConsoleColor.Green;
                                        break;
                                    case 33:
                                        Console.ForegroundColor = !isBright ? ConsoleColor.DarkYellow : ConsoleColor.Yellow;
                                        break;
                                    case 34:
                                        Console.ForegroundColor = !isBright ? ConsoleColor.DarkBlue : ConsoleColor.Blue;
                                        break;
                                    case 35:
                                        Console.ForegroundColor = !isBright ? ConsoleColor.DarkMagenta : ConsoleColor.Magenta;
                                        break;
                                    case 36:
                                        Console.ForegroundColor = !isBright ? ConsoleColor.DarkCyan : ConsoleColor.Cyan;
                                        break;
                                    case 37:
                                        Console.ForegroundColor = !isBright ? ConsoleColor.Gray : ConsoleColor.White;
                                        break;
                                }
                            }
                        }
                        catch (FormatException)
                        {

                        }
                        parameter = "";
                    }
                    else
                    {
                        parameter += s;
                    }
                }
            }

            Console.Write(content);
        }

        public static void WriteAsciiLine(string line)
        {
            WriteAscii(line + "\n");
        }
    }
}
