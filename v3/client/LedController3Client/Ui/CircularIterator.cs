using System;

namespace LedController3Client.Ui
{
    public class CircularIterator
    {
        private static readonly float FullCircleAngle = (float)Math.PI * 2f;

        private readonly int _resolution;
        private readonly float _alphaDelta;

        private int _step;

        public CircularIterator(int resolution)
        {
            _resolution = resolution;
            _alphaDelta = FullCircleAngle / resolution;

            Alpha1 = 0f;
            Cos1 = 1f;
            Sin1 = 0f;
        }

        public float Alpha0 { get; private set; }
        public float Alpha1 { get; private set; }
        public float Alpha0Ratio { get { return Alpha0 / FullCircleAngle; } }
        public float Alpha1Ratio { get { return Alpha1 / FullCircleAngle; } }
        public float Cos0 { get; private set; }
        public float Sin0 { get; private set; }
        public float Cos1 { get; private set; }
        public float Sin1 { get; private set; }

        public bool Next()
        {
            if (_step == _resolution)
                return false;

            ++_step;

            Alpha0 = Alpha1;
            Alpha1 = _step * _alphaDelta;
            Cos0 = Cos1;
            Sin0 = Sin1;
            Cos1 = (float)Math.Cos(Alpha1);
            Sin1 = (float)Math.Sin(Alpha1);

            return true;
        }
    }
}
