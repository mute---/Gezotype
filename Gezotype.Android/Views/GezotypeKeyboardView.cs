﻿using Android.Graphics;
using Android.Content;
using Android.Util;
using Android.Views;
using System;
using Gezotype.PCL;
using System.Collections.Generic;
using Android.Text;

namespace Gezotype.Android.Views
{
    public class GezotypeKeyboardView : View
    {
        public event EventHandler<CharRecognizedEventArgs> CharRecongnized;

        private readonly Dictionary<int, Path> _touchPaths = new Dictionary<int, Path>();
        private readonly Paint _paint = new Paint();
        private readonly TextPaint _labelPaint = new TextPaint();
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

            _labelPaint.SetStyle(Paint.Style.Fill);
            _labelPaint.AntiAlias = true;
        }

        protected override void OnDraw(Canvas canvas)
        {
            DrawGrid(canvas);
            DrawLabels(canvas);
            DrawTouchPath(canvas);
        }

        private void DrawLabels(Canvas canvas)
        {
            _labelPaint.Color = Color.Black;

            var sideXOffset = MeasuredWidth / 4.5f;
            var sideBarHeight = MeasuredHeight / 3;
            var labelYOffset = sideBarHeight / 4;
            var labelXOffset = labelYOffset / 2;
            var textSize = labelYOffset / 2.5f * Resources.DisplayMetrics.Density;
            var baselineShift = textSize / 3;
            _labelPaint.TextSize = textSize;

            // Draw left in.
            _labelPaint.TextAlign = Paint.Align.Center;
            var lInLabels = _keyboard.GetLInLabels();
            
            for (int i = 1; i < 5; ++i)
            {
                canvas.DrawText(lInLabels[i - 1].ToString(), 
                    sideXOffset - labelXOffset, 
                    sideBarHeight + labelYOffset * i - baselineShift, 
                    _labelPaint);
            }

            // Draw left out.
            var lOutLabels = _keyboard.GetLOutLabels();
            for (int i = 1; i < 5; ++i)
            {
                canvas.DrawText(lOutLabels[i - 1].ToString(),
                    sideXOffset + labelXOffset,
                    sideBarHeight + labelYOffset * i - baselineShift,
                    _labelPaint);
            }

            // Draw left inout.
            var lInOutLabels = _keyboard.GetLInOutLabels();
            for (int i = 1; i < 5; ++i)
            {
                canvas.DrawText(lInOutLabels[i - 1].ToString(),
                    sideXOffset + labelXOffset * 2.5f,
                    sideBarHeight + labelYOffset * i - baselineShift,
                    _labelPaint);
            }

            // Draw right in.
            _labelPaint.TextAlign = Paint.Align.Center;
            var rInLabels = _keyboard.GetRInLabels();

            for (int i = 1; i < 5; ++i)
            {
                canvas.DrawText(rInLabels[i - 1].ToString(),
                    MeasuredWidth - sideXOffset + labelXOffset,
                    sideBarHeight + labelYOffset * i - baselineShift,
                    _labelPaint);
            }

            // Draw left out.
            var rOutLabels = _keyboard.GetROutLabels();
            for (int i = 1; i < 5; ++i)
            {
                canvas.DrawText(rOutLabels[i - 1].ToString(),
                    MeasuredWidth - sideXOffset - labelXOffset,
                    sideBarHeight + labelYOffset * i - baselineShift,
                    _labelPaint);
            }

            // Draw left inout.
            var rInOutLabels = _keyboard.GetRInOutLabels();
            for (int i = 1; i < 5; ++i)
            {
                canvas.DrawText(rInOutLabels[i - 1].ToString(),
                    MeasuredWidth - sideXOffset - labelXOffset * 2.5f,
                    sideBarHeight + labelYOffset * i - baselineShift,
                    _labelPaint);
            }

            // Draw center.
            var l = _keyboard.GetLLabels();
            var r = _keyboard.GetRLabels();

            var cx = MeasuredWidth / 2;
            var cy = MeasuredHeight / 2;
            var centerBarHeight = MeasuredHeight / 6;
            var centerLabelYOffset = centerBarHeight / 2 + baselineShift;

            for (int i = 1; i < 5; ++i)
            {
                canvas.DrawText(l[i - 1].ToString(),
                    cx - labelXOffset,
                    centerBarHeight * i + centerLabelYOffset,
                    _labelPaint);
                canvas.DrawText(r[i - 1].ToString(),
                    cx + labelXOffset,
                    centerBarHeight * i + centerLabelYOffset,
                    _labelPaint);
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
