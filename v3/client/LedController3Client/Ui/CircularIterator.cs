using System;

namespace LedController3Client.Ui
{
    public class CircularIterator
    {
        public static readonly float FullCircleAngle = (float)Math.PI * 2f;

        private readonly int _resolution;
        private readonly float _alphaDelta;
        private readonly bool _calculateSinCos;

        private int _step;

        public CircularIterator(int resolution, bool calculateSinCos)
        {
            _resolution = resolution;
            _alphaDelta = FullCircleAngle / resolution;
            _calculateSinCos = calculateSinCos;

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
            if (_step > _resolution)
                return false;

            ++_step;

            Alpha0 = Alpha1;
            Alpha1 = _step * _alphaDelta;

            if (_calculateSinCos)
            {
                Cos0 = Cos1;
                Sin0 = Sin1;
                Cos1 = (float)Math.Cos(Alpha1);
                Sin1 = (float)Math.Sin(Alpha1);
            }

            return true;
        }
    }
}
