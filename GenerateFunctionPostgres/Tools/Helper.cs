using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GenerateFunctionPostgres.Tools
{
    public class Helper
    {
        private static bool isDebug = false;
        public static void writeConsole(string content)
        {
            if (isDebug)
            {
                Console.WriteLine(content);
            }
        }

        public static string[] readFile(string pathFile)
        {
            string[] lines;

            try
            {
                lines = System.IO.File.ReadAllLines(pathFile);
            }
            catch (Exception e)
            {
                return null;
            }
            //Console.WriteLine(pathFile);
            // The files used in this example are created in the topic
            // How to: Write to a Text File. You can change the path and
            // file name to substitute text files of your own.

            // Example #1
            // Read the file as one string.
            //string text = System.IO.File.ReadAllText(@"C:\Users\Public\TestFolder\WriteText.txt");

            // Display the file contents to the console. Variable text is a string.
            //System.Console.WriteLine("Contents of WriteText.txt = {0}", text);

            // Example #2
            // Read each line of the file into a string array. Each element
            // of the array is one line of the file.
            //string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Public\TestFolder\WriteLines2.txt");

            // Display the file contents by using a foreach loop.
            //System.Console.WriteLine("Contents of file");
            //foreach (string line in lines)
            //{
            //    // Use a tab to indent each line of the file.
            //    Console.WriteLine(line);
            //}
            return lines;
        }

        public static string readFileReturnString(string pathFile)
        {
            return File.ReadAllText(pathFile);
        }
        public static bool writeFile(string[] data, string pathFile)
        {
            bool status = false;
            // These examples assume a "C:\Users\Public\TestFolder" folder on your machine.
            // You can modify the path if necessary.


            // Example #1: Write an array of strings to a file.
            // Create a string array that consists of three lines.
            //string[] lines = { "First line", "Second line", "Third line" };
            // WriteAllLines creates a file, writes a collection of strings to the file,
            // and then closes the file.  You do NOT need to call Flush() or Close().
            //System.IO.File.WriteAllLines(@"C:\Users\Public\TestFolder\WriteLines.txt", lines);


            // Example #2: Write one string to a text file.
            //string text = "A class is the most powerful data type in C#. Like a structure, " +
            //               "a class defines the data and behavior of the data type. ";
            // WriteAllText creates a file, writes the specified string to the file,
            // and then closes the file.    You do NOT need to call Flush() or Close().
            //System.IO.File.WriteAllText(@"C:\Users\Public\TestFolder\WriteText.txt", text);

            // Example #3: Write only some strings in an array to a file.
            // The using statement automatically flushes AND CLOSES the stream and calls 
            // IDisposable.Dispose on the stream object.
            // NOTE: do not use FileStream for text files because it writes bytes, but StreamWriter
            // encodes the output as text.

            try
            {
                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(pathFile))
                {
                    foreach (string line in data)
                    {
                        // If the line doesn't contain the word 'Second', write the line to the file.
                        //if (!line.Contains("Second"))
                        //{
                        file.WriteLine(line);
                        //}
                    }
                }
                status = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //status = false;
            }

            // Example #4: Append new text to an existing file.
            // The using statement automatically flushes AND CLOSES the stream and calls 
            // IDisposable.Dispose on the stream object.
            //using (System.IO.StreamWriter file =
            //    new System.IO.StreamWriter(@"C:\Users\Public\TestFolder\WriteLines2.txt", true))
            //{
            //    file.WriteLine("Fourth line");
            //}
            return status;
        }

        //public static string regex (string value)
        //{
        //    //var regex = new Regex(@"(?<=%download%#)\d+");
        //    //var regex = new Regex(regexStr);
        //    //return regex.Matches(value).ToString();
        //    //@"ID=(\d+)"
        //    return Regex.Match(value, @"ID=(\d+)").Groups[1].Value;
        //}
        public static string getString(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                try
                {
                    return strSource.Substring(Start, End - Start);
                }
                catch (Exception ex)
                {
                    return null;
                }

            }
            else
            {
                return null;
            }
        }
        public static string getStringFromstrStartoEnd(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.Length;
                try
                {
                    return strSource.Substring(Start, End - Start);
                }
                catch (Exception ex)
                {
                    return null;
                }

            }
            else
            {
                return null;
            }
        }
        public static string getStringFirst(string strSource, string strEnd)
        {
            int Start = 0, End;
            if (strSource.Contains(strEnd))
            {
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return null;
            }
        }
        public static string getStringStartLast(string strSource, string strStartLastIndex)
        {
            int Start, End;
            if (strSource.Contains(strStartLastIndex))
            {
                Start = strSource.LastIndexOf(strStartLastIndex) + strStartLastIndex.Length;
                return strSource.Substring(Start, strSource.Length - Start);
            }
            else
            {
                return null;
            }
        }
        public static string getStringLast(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.LastIndexOf(strEnd);
                try
                {
                    return strSource.Substring(Start, End - Start);
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        public static string getStringLast(string strSource, string strStart)
        {
            int Start, End;
            if (strSource.Contains(strStart))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                return strSource.Substring(Start, strSource.Length - Start);
            }
            else
            {
                return null;
            }
        }

        public static string extractStringTag(string s, string tag)
        {
            // You should check for errors in real-world code, omitted for brevity
            var startTag = "<" + tag + ">";
            int startIndex = s.IndexOf(startTag) + startTag.Length;
            int endIndex = s.IndexOf("</" + tag + ">", startIndex);
            return s.Substring(startIndex, endIndex - startIndex);
        }
    }
}
