using System;
namespace Gezotype.PCL
{
    public class Keyboard
    {
        private KeyboardState _currentState = KeyboardState.Normal;
        public KeyboardLayout _currentLayout { get; } = new KeyboardLayout();

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
            _currentState = KeyboardState.In;
        }

        public void SetOut() 
        {
            _currentState = KeyboardState.Out;
        }

        public void SetInOut() 
        {
            _currentState = KeyboardState.InOut;
        }

        private char GetR(int idx)
        {
            switch (_currentState)
            {
                case KeyboardState.Normal:
                    return _currentLayout.R[idx];
                case KeyboardState.In:
                    return _currentLayout.RIn[idx];
                case KeyboardState.Out:
                    return _currentLayout.ROut[idx];
                case KeyboardState.InOut:
                    return _currentLayout.RInOut[idx];
                default:
                    throw new Exception("Invalid keyboard state");
            }
        }
        
        private char GetL(int idx)
        {
            switch (_currentState)
            {
                case KeyboardState.Normal:
                    return _currentLayout.L[idx];
                case KeyboardState.In:
                    return _currentLayout.LIn[idx];
                case KeyboardState.Out:
                    return _currentLayout.LOut[idx];
                case KeyboardState.InOut:
                    return _currentLayout.LInOut[idx];
                default:
                    throw new Exception("Invalid keyboard state");
            }
        }
    }
}
