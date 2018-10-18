using System;
using System.Linq;
using Android;
using Android.App;
using Android.InputMethodServices;
using Android.Views;
using Android.Views.InputMethods;
using Gezotype.Android.Views;
using Gezotype.PCL;

namespace Gezotype.Android.Services
{
    [Service(Permission = "android.permission.BIND_INPUT_METHOD")]
    [IntentFilter(new string[] { "android.view.InputMethod" })]
    [MetaData("android.view.im", Resource = "@xml/method")]
    public class GezotypeService : InputMethodService
    {
        public override View OnCreateInputView()
        {
            var keyboardView = LayoutInflater.Inflate(Resource.Layout.keyboard_view, null) as GezotypeKeyboardView;
            keyboardView.CharRecongnized += KeyboardView_CharRecongnized;
            keyboardView.Action += KeyboardView_Action;

            return keyboardView;
        }

        void KeyboardView_CharRecongnized(object sender, CharRecognizedEventArgs e)
        {
            CurrentInputConnection.CommitText(new Java.Lang.String(e.Char), 0);
        }

        void KeyboardView_Action(object sender, KeyboardActionEventArgs e)
        {
            switch (e.Action)
            {
                case KeyboardAction.Space:
                    CurrentInputConnection.CommitText(new Java.Lang.String(" "), 0);
                    break;

                case KeyboardAction.Delete:
                    var wordToDelete = CurrentInputConnection
                        .GetTextBeforeCursor(25, GetTextFlags.None)
                        .Split(' ').LastOrDefault(str => !string.IsNullOrEmpty(str));
                    if (!string.IsNullOrEmpty(wordToDelete))
                    {
                        CurrentInputConnection.DeleteSurroundingText(wordToDelete.Length, 0);
                    }
                    break;
            }
        }
    }
}
