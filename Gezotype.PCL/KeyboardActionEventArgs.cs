using System;
namespace Gezotype.PCL
{
    public class KeyboardActionEventArgs
    {
        public KeyboardAction Action { get; set; }

        public KeyboardActionEventArgs(KeyboardAction action)
        {
            Action = action;
        }
    }
}
