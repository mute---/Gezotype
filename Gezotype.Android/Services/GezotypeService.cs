using System;
using Android;
using Android.App;
using Android.InputMethodServices;
using Android.Views;
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

            return keyboardView;
        }

        void KeyboardView_CharRecongnized(object sender, CharRecognizedEventArgs e)
        {
            CurrentInputConnection.CommitText(new Java.Lang.String(e.Char), 0);
        }
    }
}
