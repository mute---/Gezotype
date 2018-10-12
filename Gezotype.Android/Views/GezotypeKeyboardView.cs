using Android.Graphics;
using Android.Content;
using Android.InputMethodServices;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using Gezotype.PCL;

namespace Gezotype.Android.Views
{
    public class GezotypeKeyboardView : View
    {
        public event EventHandler<CharRecognizedEventArgs> CharRecongnized;

        private readonly Path _touchPath = new Path();

        public GezotypeKeyboardView(Context context, IAttributeSet set) : base(context, set) { }

        protected override void OnDraw(Canvas canvas)
        {
            var cx = MeasuredWidth / 2;
            var cy = MeasuredHeight / 2;

            var paint = new Paint
            {
                Color = Color.Gray
            };
            paint.SetStyle(Paint.Style.Stroke);

            canvas.DrawPath(_touchPath, paint);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            SetMeasuredDimension(widthMeasureSpec, widthMeasureSpec);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    _touchPath.Reset();
                    _touchPath.MoveTo(e.GetX(), e.GetY());
                    Invalidate();
                    return true;

                case MotionEventActions.Move:
                    _touchPath.LineTo(e.GetX(), e.GetY());
                    Invalidate();
                    return true;

                case MotionEventActions.Up:
                    _touchPath.Reset();
                    Invalidate();
                    return true;

                default:
                    return base.OnTouchEvent(e);
            }
        }

        private void RaiseCharRecongnized(string @char)
        {
            CharRecongnized?.Invoke(this, new CharRecognizedEventArgs(@char));
        }
    }
}
