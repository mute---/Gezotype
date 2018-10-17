using System;
namespace Gezotype.PCL
{
    public class Keyboard
    {
        private KeyboardState _currentState = KeyboardState.Normal;
        public Keymap _currentKeymap { get; } = new Keymap();

        public char GetL1() => GetL(0);
        public char GetL2() => GetL(1);
        public char GetL3() => GetL(2);
        public char GetL4() => GetL(3);

        public char GetR1() => GetR(0);
        public char GetR2() => GetR(1);
        public char GetR3() => GetR(2);
        public char GetR4() => GetR(3);

        public void ResetState()
        {
            _currentState = KeyboardState.Normal;
        }

        public void SetIn()
        {
            if (_currentState == KeyboardState.Normal)
                _currentState = KeyboardState.In;
        }

        public void SetOut()
        {
            if (_currentState == KeyboardState.Normal)
                _currentState = KeyboardState.Out;
        }

        public void SetInOut()
        {
            if (_currentState == KeyboardState.Out)
                _currentState = KeyboardState.InOut;
        }

        public string GetLLabels()
        {
            switch (_currentState)
            {
                case KeyboardState.Normal:
                    return _currentKeymap.L;
                case KeyboardState.In:
                    return _currentKeymap.LIn;
                case KeyboardState.Out:
                    return _currentKeymap.LOut;
                case KeyboardState.InOut:
                    return _currentKeymap.LInOut;
                default:
                    return "";
            }
        }

        public string GetRLabels()
        {
            switch (_currentState)
            {
                case KeyboardState.Normal:
                    return _currentKeymap.R;
                case KeyboardState.In:
                    return _currentKeymap.RIn;
                case KeyboardState.Out:
                    return _currentKeymap.ROut;
                case KeyboardState.InOut:
                    return _currentKeymap.RInOut;
                default:
                    return "";
            }
        }

        public string GetLInLabels()
        {
            return _currentState == KeyboardState.Normal ? _currentKeymap.LIn : "";
        }

        public string GetRInLabels()
        {
            return _currentState == KeyboardState.Normal ? _currentKeymap.RIn : "";
        }

        public string GetLOutLabels()
        {
            return _currentState == KeyboardState.Normal ? _currentKeymap.LOut : "";
        }

        public string GetROutLabels()
        {
            return _currentState == KeyboardState.Normal ? _currentKeymap.ROut : "";
        }

        public string GetLInOutLabels()
        {
            return _currentState == KeyboardState.Normal || _currentState == KeyboardState.Out ? _currentKeymap.LInOut : "";
        }

        public string GetRInOutLabels()
        {
            return _currentState == KeyboardState.Normal || _currentState == KeyboardState.Out ? _currentKeymap.RInOut : "";
        }

        public char GetR(int idx)
        {
            switch (_currentState)
            {
                case KeyboardState.Normal:
                    return _currentKeymap.R[idx];
                case KeyboardState.In:
                    return _currentKeymap.RIn[idx];
                case KeyboardState.Out:
                    return _currentKeymap.ROut[idx];
                case KeyboardState.InOut:
                    return _currentKeymap.RInOut[idx];
                default:
                    throw new Exception("Invalid keyboard state");
            }
        }

        public char GetL(int idx)
        {
            switch (_currentState)
            {
                case KeyboardState.Normal:
                    return _currentKeymap.L[idx];
                case KeyboardState.In:
                    return _currentKeymap.LIn[idx];
                case KeyboardState.Out:
                    return _currentKeymap.LOut[idx];
                case KeyboardState.InOut:
                    return _currentKeymap.LInOut[idx];
                default:
                    throw new Exception("Invalid keyboard state");
            }
        }
    }
}
