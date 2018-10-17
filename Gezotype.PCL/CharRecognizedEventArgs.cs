using System;
namespace Gezotype.PCL
{
    public class CharRecognizedEventArgs
    {
        public string Char { get; set; }

        public CharRecognizedEventArgs(string @char)
        {
            Char = @char;
        }
    }
}
