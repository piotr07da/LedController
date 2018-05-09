using LedController3Client.Ui.Core;
using SkiaSharp;

namespace LedController3Client.Ui
{
    public class SliderTouchHandlerComponent : Component, ITouchHandlerComponent
    {
        private readonly Slider _slider;
        private readonly bool _isSelectable;
        private long? _currentTouchId;
        private bool _isPressed;
        private bool _isMoved;

        public SliderTouchHandlerComponent(Slider slider, bool isSelectable)
        {
            _slider = slider;
            _isSelectable = isSelectable;
        }

        public bool Handle(long touchId, SKPoint touchLocation, TouchAction touchAction)
        {
            // Only single active touch allowed. Ignore all new touches if current touch hasn't been released.
            if (_currentTouchId.HasValue && touchId != _currentTouchId.Value)
            {
                return false;
            }

            switch (touchAction)
            {
                case TouchAction.Pressed:

                    _currentTouchId = touchId;

                    _isPressed = _slider.HitTest(touchLocation);
                    _isMoved = false;

                    return _isPressed;

                case TouchAction.Moved:

                    if (!_isPressed)
                        return false;
                    
                    _isMoved = true;
                    _slider.Drag(touchLocation);

                    return true;

                case TouchAction.Released:

                    _currentTouchId = null;

                    if (!_isPressed)
                        return false;

                    if (_isSelectable && !_isMoved)
                    {
                        _slider.IsSelected = !_slider.IsSelected;
                    }

                    _isPressed = false;

                    return true;
            }

            return false;
        }
    }
}
