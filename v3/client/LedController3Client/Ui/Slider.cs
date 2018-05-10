using LedController3Client.Ui.Core;
using SkiaSharp;
using System;

namespace LedController3Client.Ui
{
    public interface ISlider : IComponent
    {
        SKColor Color { get; set; }
        SKPoint Position { get; set; }
        float Radius { get; set; }
        bool IsSelected { get; set; }
        bool HitTest(SKPoint hitPoint);
        void Drag(SKPoint dragPoint);
    }

    public class Slider<TValue> : Component, ISlider
    {
        private TValue _value;
        private SKColor _color;
        private float _radius;
        private bool _isSelected;
        private readonly ISliderBody<TValue> _body;

        private SliderDrawerComponent _drawer;
        private SliderTouchHandlerComponent _touchHandler;

        public Slider(ColorTimeLineDrawingConfig drawingConfig, TValue value, SKColor color, float radius, bool isSelected, bool isSelectable, ISliderBody<TValue> body)
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

        public event EventHandler<EventArgs<TValue>> ValueChanged;
        public event EventHandler<EventArgs<bool>> IsSelectedChanged;

        public TValue Value
        {
            get { return _value; }
            set
            {
                if (!value.Equals(_value))
                {
                    ResetValue(value);
                    ValueChanged?.Invoke(this, new EventArgs<TValue>(_value));
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
        
        public SKPoint Position { get; set; }

        public void ResetValue(TValue value)
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
