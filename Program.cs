using System;
using System.IO;

namespace chdate
{
    internal class Program
    {
        private const string CREATED_DATE_OPTION = "/c";
        private const string MODIFIED_DATE_OPTION = "/m";

        private string _path;
        private DateTimeOffset _creationDate;
        private DateTimeOffset _modifiedDate;
        private bool _changeCreationDate;
        private bool _changeModifiedDate;

        private int Main(string[] args)
        {
            try
            {
                ParseCommandline(args);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return -1;
            }

            return 0;
        }

        private void ParseCommandline(string[] args)
        {
            if (args.Length < 4)
            {
                ShowUsage();
                throw new Exception();
            }

            _path = args[1];

            for(int position = 2; position < args.Length; position++)
            {
                if(CREATED_DATE_OPTION == args[position])
                {
                    if ((position + 1) == args.Length)
                        throw new Exception("Expected missing parameter <creation date> after " + CREATED_DATE_OPTION + ".");

                    _creationDate = DateTimeOffset.Parse(args[position + 1]);
                    _changeCreationDate = true;
                }
                else if(MODIFIED_DATE_OPTION == args[position])
                {
                    if ((position + 1) == args.Length)
                        throw new Exception("Expected missing parameter <modified date> after " + MODIFIED_DATE_OPTION + ".");

                    _modifiedDate= DateTimeOffset.Parse(args[position + 1]);
                    _changeModifiedDate = true;
                }
            }
        }

        private void ShowUsage()
        {
            // TODO:
        }
    }
}
