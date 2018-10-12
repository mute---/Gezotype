using Android.Graphics;
using Android.Content;
using Android.InputMethodServices;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Gezotype.Android.Views
{
    public class GezotypeKeyboardView : View
    {
        public GezotypeKeyboardView(Context context, IAttributeSet set) : base(context, set) { }

        protected override void OnDraw(Canvas canvas)
        {
            var cx = MeasuredWidth / 2;
            var cy = MeasuredHeight / 2;

            var paint = new Paint();
            paint.SetStyle(Paint.Style.Fill);
            paint.Color = Color.Green;

            canvas.DrawRect(cx - 50, cy - 50, cx + 50, cy + 50, paint);

            base.OnDraw(canvas);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            SetMeasuredDimension(widthMeasureSpec, widthMeasureSpec);
        }
    }
}
