using System;
using UnityEngine;
using Vuplex.WebView;

namespace Assets.Mechanics.Browser
{
    public class ControllableCanvasPointerInputDetector : CanvasPointerInputDetector
    {        
        public void RaiseBeganDragEvent(EventArgs<Vector2> eventArgs)
        {
            _raiseBeganDragEvent(eventArgs);
        }

        public void RaiseDraggedEvent(EventArgs<Vector2> eventArgs)
        {
            _raiseDraggedEvent(eventArgs);
        }

        public void RaisePointerDownEvent(PointerEventArgs eventArgs)
        {
            _raisePointerDownEvent(eventArgs);
        }

        public void RaisePointerExitedEvent(EventArgs eventArgs)
        {
            _raisePointerExitedEvent(new EventArgs<Vector2>(Vector2.zero));
        }

        public void RaisePointerUpEvent(PointerEventArgs eventArgs)
        {
            _raisePointerUpEvent(eventArgs);
        }

        public void RaiseScrolledEvent(ScrolledEventArgs eventArgs)
        {
            _raiseScrolledEvent(eventArgs);
        }
    }
}