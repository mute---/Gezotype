using System;
using Android;
using Android.App;
using Android.InputMethodServices;
using Android.Views;
using Gezotype.Android.Views;

namespace Gezotype.Android.Services
{
    [Service(Permission = "android.permission.BIND_INPUT_METHOD")]
    [IntentFilter(new string[] { "android.view.InputMethod" })]
    [MetaData("android.view.im", Resource = "@xml/method")]
    public class GezotypeService : InputMethodService
    {
        public override View OnCreateInputView()
        {
            var input = CurrentInputConnection.CommitText(new Java.Lang.String("Hello from custom keyboard!!!"), 1);
            var keyboardView = LayoutInflater.Inflate(Resource.Layout.keyboard_view, null);
            return keyboardView;
        }
    }
}
