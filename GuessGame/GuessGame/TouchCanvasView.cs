using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace GuessGame
{
    public class TouchCanvasView : SKCanvasView, ITouchCanvasViewController
    {
        public event Action<SKPoint> Touched;

        public virtual void OnTouch(SKPoint point)
        {
            Touched?.Invoke(point);
        }
    }

    public interface ITouchCanvasViewController : IViewController
    {
        void OnTouch(SKPoint point);
    }
}
