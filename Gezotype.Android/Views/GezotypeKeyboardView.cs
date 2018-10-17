using Android.Graphics;
using Android.Content;
using Android.Util;
using Android.Views;
using System;
using Gezotype.PCL;
using System.Collections.Generic;

namespace Gezotype.Android.Views
{
    public class GezotypeKeyboardView : View
    {
        public event EventHandler<CharRecognizedEventArgs> CharRecongnized;

        private readonly Dictionary<int, Path> _touchPaths = new Dictionary<int, Path>();
        private readonly Paint _paint = new Paint();
        private readonly Keyboard _keyboard = new Keyboard();

        public GezotypeKeyboardView(Context context, IAttributeSet set) : base(context, set) 
        {
            Init(); 
        }

        private void Init()
        {
            _paint.SetStyle(Paint.Style.Stroke);
            _paint.StrokeWidth = 2  * Resources.DisplayMetrics.Density;
            _paint.AntiAlias = true;
        }

        protected override void OnDraw(Canvas canvas)
        {
            DrawGrid(canvas);
            DrawLabels(canvas);
            DrawTouchPath(canvas);
        }

        private void DrawLabels(Canvas canvas)
        {
            _paint.Color = Color.Black;

            // Draw left in.
            foreach(var @char in _keyboard.GetLInLabels())
            {

            }
        }

        private void DrawTouchPath(Canvas canvas)
        {
            _paint.Color = Color.Gray;

            foreach(var path in _touchPaths.Values)
            {
                canvas.DrawPath(path, _paint);
            }
        }

        private void DrawGrid(Canvas canvas)
        {
            var cx = MeasuredWidth / 2;
            var cy = MeasuredHeight / 2;

            var centerBarHeight = MeasuredHeight / 6;

            _paint.Color = Color.Black;

            // Draw center bar cluster.
            canvas.DrawLine(cx, centerBarHeight, cx, centerBarHeight * 5, _paint);

            _paint.SetStyle(Paint.Style.FillAndStroke);
            for (int i = 1; i < 6; ++i)
            {
                canvas.DrawCircle(cx, centerBarHeight * i, 3 * Resources.DisplayMetrics.Density, _paint);
            }
            _paint.SetStyle(Paint.Style.Stroke);


            // Draw side bars.
            var sideXOffset = MeasuredWidth / 4.5f;
            var sideBarHeight = MeasuredHeight / 3;
            canvas.DrawLine(sideXOffset, sideBarHeight, sideXOffset, sideBarHeight * 2, _paint);
            canvas.DrawLine(MeasuredWidth - sideXOffset, sideBarHeight, MeasuredWidth - sideXOffset, sideBarHeight * 2, _paint);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            SetMeasuredDimension(widthMeasureSpec, widthMeasureSpec);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            var id = e.GetPointerId(e.ActionIndex);
            switch (e.ActionMasked)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    var newTouch = new Path();
                    newTouch.MoveTo(e.GetX(e.ActionIndex), e.GetY(e.ActionIndex));
                    _touchPaths.Add(id, newTouch);
                    System.Diagnostics.Debug.WriteLine($"Start touch {id}");

                    Invalidate();
                    return true;

                case MotionEventActions.Move:
                    for (int i = 0; i < e.PointerCount; ++i)
                    {
                        _touchPaths[e.GetPointerId(i)].LineTo(e.GetX(i), e.GetY(i));
                    }
                    //System.Diagnostics.Debug.WriteLine($"Touch {id} moved to {e.GetX(e.ActionIndex)}, {e.GetY(e.ActionIndex)}");

                    Invalidate();
                    return true;

                case MotionEventActions.Up:
                case MotionEventActions.PointerUp:
                    _touchPaths.Remove(id);
                    System.Diagnostics.Debug.WriteLine($"End touch {id}");

                    Invalidate();
                    return true;

                case MotionEventActions.Cancel:
                    _touchPaths.Clear();
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
