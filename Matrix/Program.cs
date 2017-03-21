

using System;
using System.Collections.Generic;
using System.IO;

namespace matrix
{
    class Program
    {
        static int Counter;
        static Random rand = new Random();

        static int Interval = 100; // Normal Flowing of Matrix Rain
        static int FullFlow = Interval + 230; // Fast Flowing of Matrix Rain
        static int Blacking = FullFlow + 50; // Displaying the Test Alone

        static ConsoleColor NormalColor = ConsoleColor.DarkGreen;
        static ConsoleColor GlowColor = ConsoleColor.Green;
        static ConsoleColor FancyColor = ConsoleColor.White;
        static String TextInput = "Hello.";



        static char AsciiCharacter(int x, int y, char[][] art)//Randomised Inputs
        {
            if (y >= art.Length || x >= art[0].Length)
            {
                return '*';
            }
            return art[y][x];
        }
        static void Main()
        {

            var art = GetAsciiArt();

            
            Console.ForegroundColor = NormalColor;
            Console.WindowLeft = Console.WindowTop = 0;
            //Console.WindowHeight = Console.BufferHeight = Console.LargestWindowHeight;
            //Console.WindowWidth = Console.BufferWidth = Console.LargestWindowWidth;

            Console.SetWindowSize(art[0].Length, art.Length);

            Console.WindowHeight = Console.BufferHeight = art.Length;
            Console.WindowWidth = Console.BufferWidth = art[0].Length;

            Console.SetWindowPosition(0, 0);
            Console.CursorVisible = false;

            int width, height;
            int[] y;
            Initialize(out width, out height, out y);//Setting the Starting Point
           
            while (true)
            {
                Counter = Counter + 1;
                UpdateAllColumns(width, height, y, art);
                if (Counter > (5 * Interval))
                    Counter = 0;

            }
        }
        private static void UpdateAllColumns(int width, int height, int[] y, char[][] art)
        {
            int x;
            if (Counter < Interval)
            {
                for (x = 0; x < width; ++x)
                {
                    if (x % 10 == 1)//Randomly setting up the White Position
                        Console.ForegroundColor = FancyColor;
                    else
                        Console.ForegroundColor = GlowColor;
                    Console.SetCursorPosition(x, y[x]);
                    Console.Write(AsciiCharacter(x,y[x],art));

                    if (x % 10 == 9)
                        Console.ForegroundColor = FancyColor;
                    else
                        Console.ForegroundColor = NormalColor;
                    int temp = y[x] - 2;
                    Console.SetCursorPosition(x, inScreenYPosition(temp, height));
                    Console.Write(AsciiCharacter(x, y[x], art));

                    int temp1 = y[x] - 20;
                    Console.SetCursorPosition(x, inScreenYPosition(temp1, height));
                    Console.Write(' ');
                    y[x] = inScreenYPosition(y[x] + 1, height);
                   
                }
            }
            else if (Counter > Interval && Counter < FullFlow)
            {
                for (x = 0; x < width; ++x)
                {

                    Console.SetCursorPosition(x, y[x]);
                    //if (x % 10 == 9)
                    //    Console.ForegroundColor = FancyColor;
                    //else
                        Console.ForegroundColor = NormalColor;

                    Console.Write(AsciiCharacter(x, y[x], art));//Printing the Character Always at Fixed position

                    y[x] = inScreenYPosition(y[x] + 1, height);
                }
            }
            else if (Counter > FullFlow)
            {
                for (x = 0; x < width; ++x)
                {
                    Console.SetCursorPosition(x, y[x]);
                    Console.Write(' ');//Slowly blacking out the Screen
                    int temp1 = y[x] - 20;
                    Console.SetCursorPosition(x, inScreenYPosition(temp1, height));
                    Console.Write(' ');
                    if (Counter > FullFlow && Counter < Blacking)// Clearing the Entire screen to get the Darkness
                    {
                        if (x % 10 == 9)
                            Console.ForegroundColor = FancyColor;
                        else
                            Console.ForegroundColor = NormalColor;
                        int temp = y[x] - 2;
                        Console.SetCursorPosition(x, inScreenYPosition(temp, height));
                        Console.Write(AsciiCharacter(x, y[x], art));//The Text is printed Always

                    }
                    Console.SetCursorPosition(width / 2, height / 2);
                    Console.Write(TextInput);
                    y[x] = inScreenYPosition(y[x] + 1, height);
                }

            }
        }
        public static int inScreenYPosition(int yPosition, int height)
        {
            if (yPosition < 0)//When there is negative value
                return yPosition + height;
            else if (yPosition < height)//Normal 
                return yPosition;
            else// When y goes out of screen when autoincremented by 1
                return 0;

        }
        private static void Initialize(out int width, out int height, out int[] y)
        {
            height = Console.WindowHeight;
            width = Console.WindowWidth - 1;
            y = new int[width];
            Console.Clear();

            for (int x = 0; x < width; ++x)//Setting the cursor at random at program startup
            {
                y[x] = rand.Next(height);
            }
        }

        static char[][] GetAsciiArt()
        {
            string filePath = @".\art.txt";
            string separator = "";

            var output = CsvToArray(filePath,separator);
            return output;
        }

        public static char[][] CsvToArray(string path, string separator = ";")
        {
            // check if the file exists
            if (!File.Exists(path))
                throw new FileNotFoundException("The CSV specified was not found.");

            // temporary list to store information
            List<char[]> temp = new List<char[]>();

            using (var reader = new StreamReader(File.OpenRead(path)))
            {
                while (!reader.EndOfStream)
                {
                    // read the line
                    string line = reader.ReadLine();

                    // if you need to give some changes on the inforation
                    // do it here!
                    //string[] values = line.Split(separator.ToCharArray());
                    char[] values = line.ToCharArray();

                    // add to the temporary list
                    temp.Add(values);
                }
            }

            // convert te list to array, which it will be a string[][]
            return temp.ToArray();
        }
    }
}