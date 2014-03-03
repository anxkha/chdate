// Author: Lucas Suggs (Anxkha)
// Last Updated: 2014-02-28
//
// This code is public domain. Refer to the LICENSE file for details.

using System;
using System.IO;

namespace chdate
{
    internal class Program
    {
        private const string CREATED_DATE_OPTION = "/c";
        private const string MODIFIED_DATE_OPTION = "/m";
        private const string HELP_OPTION = "/?";

        private static string _path;
        private static DateTimeOffset _creationDate;
        private static DateTimeOffset _modifiedDate;
        private static bool _changeCreationDate;
        private static bool _changeModifiedDate;

        private static int Main(string[] args)
        {
            try
            {
                ParseCommandline(args);
                Run();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return -1;
            }

            return 0;
        }

        private static void ParseCommandline(string[] args)
        {
            if (0 == args.Length)
                ShowUsage();

            if (HELP_OPTION == args[0])
                ShowUsage();

            if (args.Length < 3)
                ShowUsage();

            _path = args[0];

            for(int position = 1; position < args.Length; position++)
            {
                if(HELP_OPTION == args[position])
                    ShowUsage();
                else if(CREATED_DATE_OPTION == args[position])
                {
                    if ((position + 1) == args.Length)
                        throw new Exception("Expected missing parameter <creation date> after " + CREATED_DATE_OPTION + ".");

                    try
                    {
                        _creationDate = DateTimeOffset.Parse(args[position + 1]);
                    }
                    catch(FormatException)
                    {
                        Console.Error.WriteLine("The creation date specified is of the wrong date format.\n");
                        ShowUsage();
                    }

                    _changeCreationDate = true;
                    position++;
                }
                else if(MODIFIED_DATE_OPTION == args[position])
                {
                    if ((position + 1) == args.Length)
                        throw new Exception("Expected missing parameter <modified date> after " + MODIFIED_DATE_OPTION + ".");

                    try
                    {
                        _modifiedDate = DateTimeOffset.Parse(args[position + 1]);
                    }
                    catch(FormatException)
                    {
                        Console.Error.WriteLine("The modified date specified is of the wrong date format.\n");
                        ShowUsage();
                    }

                    _changeModifiedDate = true;
                    position++;
                }
            }

            if ((false == _changeCreationDate) && (false == _changeModifiedDate))
                throw new Exception("One of either the /m or /c parameters is required.");
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("chdate.exe <path to file or directory> [/m <modified date> | /c <creation date>]");
            Console.WriteLine("One of either /m or /c is required. Both can be specified at the same time.\n");
            Console.WriteLine("Example date formats:\n");
            Console.WriteLine("\"2014-02-28\"");
            Console.WriteLine("\"2014-02-28 10:54\"");
            Console.WriteLine("\"2014-02-28 10:54 -8\"");
            throw new Exception("");
        }

        private static void Run()
        {
            if (Directory.Exists(_path))
                RunRecursiveForDirectory(_path);
            else if (File.Exists(_path))
                RunForSingleFile(_path);
            else
                throw new Exception(@"""" + _path + @""" is not a valid file or directory.");
        }

        private static void RunRecursiveForDirectory(string root)
        {
            string[] childDirectories = Directory.GetDirectories(root);
            string[] files = Directory.GetFiles(root);

            foreach (string file in files)
                RunForSingleFile(file);

            foreach (string directory in childDirectories)
                RunRecursiveForDirectory(directory);
        }

        private static void RunForSingleFile(string path)
        {
            Console.WriteLine("Setting dates for " + path);

            try
            {
                if (_changeCreationDate)
                    File.SetCreationTimeUtc(path, _creationDate.UtcDateTime);

                if (_changeModifiedDate)
                    File.SetLastWriteTimeUtc(path, _modifiedDate.UtcDateTime);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.Error.WriteLine(e.Message);
            }
            catch (PathTooLongException e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }
    }
}
