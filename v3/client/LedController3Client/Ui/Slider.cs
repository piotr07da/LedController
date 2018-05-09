using LedController3Client.Ui.Core;
using SkiaSharp;
using System;

namespace LedController3Client.Ui
{
    public class Slider : Component
    {
        private float _value;
        private SKColor _color;
        private float _radius;
        private bool _isSelected;
        private readonly ISliderBody _body;

        private SliderDrawerComponent _drawer;
        private SliderTouchHandlerComponent _touchHandler;

        public Slider(ColorTimeLineDrawingConfig drawingConfig, float value, SKColor color, float radius, bool isSelected, bool isSelectable, ISliderBody body)
        {
            _value = value;
            _color = color;
            _radius = radius;
            _isSelected = isSelected;
            _body = body;

            RecalculatePosition();

            _drawer = new SliderDrawerComponent(drawingConfig, this);
            _touchHandler = new SliderTouchHandlerComponent(this, isSelectable);

            AddChild(_drawer);
            AddChild(_touchHandler);
        }

        public event EventHandler<EventArgs<float>> ValueChanged;
        public event EventHandler<EventArgs<bool>> IsSelectedChanged;

        public float Value
        {
            get { return _value; }
            set
            {
                if (value != _value)
                {

                    ResetValue(value);
                    ValueChanged?.Invoke(this, new EventArgs<float>(_value));
                }
            }
        }

        public SKColor Color { get => _color; set => _color = value; }
        public float Radius { get => _radius; set => _radius = value; }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value != _isSelected)
                {
                    ResetIsSelected(value);
                    IsSelectedChanged?.Invoke(this, new EventArgs<bool>(_isSelected));
                }
            }
        }
        
        public SKPoint Position { get; private set; }

        public void ResetValue(float value)
        {
            _value = value;
            RecalculatePosition();
        }

        public void ResetIsSelected(bool isSelected)
        {
            _isSelected = isSelected;
        }

        public bool HitTest(SKPoint hitPoint)
        {
            var xd = hitPoint.X - Position.X;
            var yd = hitPoint.Y - Position.Y;
            return xd * xd + yd * yd < Radius * Radius * 1.5f; // 1.5 ratio is just for making collision circle bigger than real shape.
        }

        public void Drag(SKPoint dragPoint)
        {
            Value = _body.PositionToValue(dragPoint);
        }

        private void RecalculatePosition()
        {
            Position = _body.ValueToPosition(Value);
        }
    }
}
