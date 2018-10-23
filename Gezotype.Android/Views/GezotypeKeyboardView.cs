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
        public event EventHandler<CharRecognizedEventArgs> CharRecognized;
        public event EventHandler<KeyboardActionEventArgs> Action;

        private readonly Dictionary<int, Path> _touchPaths = new Dictionary<int, Path>();
        private readonly Dictionary<int, int> _touchPrevX = new Dictionary<int, int>();
        private readonly Paint _paint = new Paint();
        private readonly TextPaint _labelPaint = new TextPaint();
        private readonly Keyboard _keyboard = new Keyboard();

        int cx;
        int cy;
        int centerBarHeight;
        float sideXOffset;
        private float _sideBarHeight;
        float labelYOffset;
        float labelXOffset;
        float textSize;
        float baselineShift;
        float centerLabelYOffset;

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
            _labelPaint.TextSize = textSize;

            // Draw left in.
            _labelPaint.TextAlign = Paint.Align.Center;
            var lInLabels = _keyboard.GetLInLabels();
            if (!string.IsNullOrEmpty(lInLabels))
                for (int i = 1; i < 5; ++i)
                {
                    canvas.DrawText(lInLabels[i - 1].ToString(), 
                        sideXOffset - labelXOffset, 
                        _sideBarHeight + labelYOffset * i - baselineShift,
                        _labelPaint);
                }

            // Draw left out.
            var lOutLabels = _keyboard.GetLOutLabels();
            if (!string.IsNullOrEmpty(lOutLabels))
                for (var i = 1; i < 5; ++i)
                {
                    canvas.DrawText(lOutLabels[i - 1].ToString(),
                        sideXOffset + labelXOffset,
                        _sideBarHeight + labelYOffset * i - baselineShift,
                        _labelPaint);
                }

            // Draw left inout.
            var lInOutLabels = _keyboard.GetLInOutLabels();
            if (!string.IsNullOrEmpty(lInOutLabels))
                for (var i = 1; i < 5; ++i)
                {
                    canvas.DrawText(lInOutLabels[i - 1].ToString(),
                        sideXOffset + labelXOffset * 2.5f,
                        _sideBarHeight + labelYOffset * i - baselineShift,
                        _labelPaint);
                }

            // Draw right in.
            _labelPaint.TextAlign = Paint.Align.Center;
            var rInLabels = _keyboard.GetRInLabels();
            if (!string.IsNullOrEmpty(rInLabels))
                for (var i = 1; i < 5; ++i)
                {
                    canvas.DrawText(rInLabels[i - 1].ToString(),
                        MeasuredWidth - sideXOffset + labelXOffset,
                        _sideBarHeight + labelYOffset * i - baselineShift,
                        _labelPaint);
                }

            // Draw left out.
            var rOutLabels = _keyboard.GetROutLabels();
            if (!string.IsNullOrEmpty(rOutLabels))
                for (var i = 1; i < 5; ++i)
                {
                    canvas.DrawText(rOutLabels[i - 1].ToString(),
                        MeasuredWidth - sideXOffset - labelXOffset,
                        _sideBarHeight + labelYOffset * i - baselineShift,
                        _labelPaint);
                }

            // Draw left inout.
            var rInOutLabels = _keyboard.GetRInOutLabels();
            if (!string.IsNullOrEmpty(rInOutLabels))
                for (var i = 1; i < 5; ++i)
                {
                    canvas.DrawText(rInOutLabels[i - 1].ToString(),
                        MeasuredWidth - sideXOffset - labelXOffset * 2.5f,
                        _sideBarHeight + labelYOffset * i - baselineShift,
                        _labelPaint);
                }

            // Draw center.
            var l = _keyboard.GetLLabels();
            var r = _keyboard.GetRLabels();
            for (var i = 1; i < 5; ++i)
            {
                canvas.DrawText(l[i - 1].ToString().ToUpper(),
                    cx - labelXOffset,
                    centerBarHeight * i + centerLabelYOffset,
                    _labelPaint);
                canvas.DrawText(r[i - 1].ToString().ToUpper(),
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
            _paint.Color = Color.Black;

            // Draw center bar cluster.
            canvas.DrawLine(cx, centerBarHeight, cx, centerBarHeight * 5, _paint);

            _paint.SetStyle(Paint.Style.FillAndStroke);
            for (var i = 1; i < 6; ++i)
            {
                canvas.DrawCircle(cx, centerBarHeight * i, 3 * Resources.DisplayMetrics.Density, _paint);
            }
            _paint.SetStyle(Paint.Style.Stroke);


            // Draw side bars.
            canvas.DrawLine(sideXOffset, _sideBarHeight, sideXOffset, _sideBarHeight * 2, _paint);
            canvas.DrawLine(MeasuredWidth - sideXOffset, _sideBarHeight, MeasuredWidth - sideXOffset, _sideBarHeight * 2, _paint);
        }

        private bool DetectedCollision(MotionEvent e, int idx)
        {
            var x = (int)e.GetX(idx);
            var oldX = _touchPrevX[e.GetPointerId(idx)];
            var y = (int)e.GetY(idx);

            if (y >= centerBarHeight && y <= centerBarHeight * 5)
            {
                var barIdx = (y / centerBarHeight) - 1;
                if (x >= cx && x > oldX && oldX < cx)
                {
                    RaiseCharRecognized(_keyboard.GetL(barIdx).ToString());
                    return true;
                }
                else if (x <= cx && x < oldX && oldX > cx)
                {
                    RaiseCharRecognized(_keyboard.GetR(barIdx).ToString());
                    return true;
                }

                if (y >= _sideBarHeight && y <= _sideBarHeight * 2)
                {
                    if ((x >= sideXOffset && oldX < sideXOffset) || (x <= MeasuredWidth - sideXOffset && oldX > MeasuredWidth - sideXOffset))
                        _keyboard.DoIn();
                    else if ((x <= sideXOffset && oldX > sideXOffset) || (x >= MeasuredWidth - sideXOffset && oldX < MeasuredWidth - sideXOffset))
                    {
                        var action = _keyboard.DoOut();
                        if (action != KeyboardAction.None)
                            RaiseKeyboardAction(action);
                    }
                }
            }

            _touchPrevX[e.GetPointerId(idx)] = x;
            return false;
        }

        private void RaiseKeyboardAction(KeyboardAction action)
        {
            Action?.Invoke(this, new KeyboardActionEventArgs(action));
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            var width = MeasureSpec.GetSize(widthMeasureSpec);
            var height = MeasureSpec.GetSize(heightMeasureSpec);
            SetMeasuredDimension(width, height);

            cx = width / 2;
            cy = height / 2;

            centerBarHeight = MeasuredHeight / 6;

            sideXOffset = MeasuredWidth / 4.5f;
            _sideBarHeight = MeasuredHeight / 3;
            labelYOffset = _sideBarHeight / 4;
            labelXOffset = labelYOffset / 2;
            textSize = labelYOffset / 2.5f * Resources.DisplayMetrics.Density;
            baselineShift = textSize / 3;

            centerLabelYOffset = centerBarHeight / 2 + baselineShift;
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
                    _touchPrevX.Add(id, (int)e.GetX(e.ActionIndex));
                    Invalidate();
                    return true;

                case MotionEventActions.Move:
                    for (int i = 0; i < e.PointerCount; ++i)
                    {
                        if ((int)e.GetX(i) == _touchPrevX[e.GetPointerId(i)])
                            continue;
                        _touchPaths[e.GetPointerId(i)].LineTo(e.GetX(i), e.GetY(i));
                        if (!DetectedCollision(e, i)) continue;
                        _touchPrevX[e.GetPointerId(i)] = (int)e.GetX(i);
                        _touchPaths[e.GetPointerId(i)].Reset();
                        _touchPaths[e.GetPointerId(i)].MoveTo(e.GetX(i), e.GetY(i));
                        _keyboard.ResetState();
                    }
                    Invalidate();
                    return true;

                case MotionEventActions.Up:
                case MotionEventActions.PointerUp:
                    _touchPaths.Remove(id);
                    _touchPrevX.Remove(id);
                    _keyboard.ResetState();
                    Invalidate();
                    return true;

                case MotionEventActions.Cancel:
                    _touchPaths.Clear();
                    _touchPrevX.Clear();
                    _keyboard.ResetState();
                    Invalidate();
                    return true;

                default:
                    return base.OnTouchEvent(e);
            }
        }

        private void RaiseCharRecognized(string @char)
        {
            CharRecognized?.Invoke(this, new CharRecognizedEventArgs(@char));
        }
    }
}
