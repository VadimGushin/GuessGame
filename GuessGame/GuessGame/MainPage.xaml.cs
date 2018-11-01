﻿using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GuessGame
{
    public partial class MainPage : ContentPage
    {
        private Dictionary<long, SKPath> temporaryPaths = new Dictionary<long, SKPath>();
        private List<SKPath> paths = new List<SKPath>();
        private SKCanvas canvas;
        private SKSurface surface;
        SKImage skimage;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnPainting(object sender, SKPaintSurfaceEventArgs e)
        {
            // CLEARING THE SURFACE

            // we get the current surface from the event args
            surface = e.Surface;
            // then we get the canvas that we can draw on
            canvas = surface.Canvas;
            // clear the canvas / view
            canvas.Clear(SKColors.White);
            
            // DRAWING TOUCH PATHS

            // create the paint for the touch path
            var touchPathStroke = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Black,
                StrokeWidth = 5
            };

            // draw the paths
            foreach (var touchPath in temporaryPaths)
            {
                canvas.DrawPath(touchPath.Value, touchPathStroke);
            }
            foreach (var touchPath in paths)
            {
                canvas.DrawPath(touchPath, touchPathStroke);
            }

                skimage = surface.Snapshot();
                SKBitmap bitmap = SKBitmap.FromImage(skimage);
            var scaledBitmap = bitmap.Resize(new SKImageInfo(100, 100), SKBitmapResizeMethod.Triangle);
                skimage = SKImage.FromBitmap(scaledBitmap);
                //SKImage snapI = e.Surface.Snapshot();

            //var x = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "FolderName");
            //var fullpath = x + "PicName.png";
            //File.WriteAllBytes(fullpath, pngImage.ToArray());
            //SKBitmap bitmap = SKBitmap.Decode(fullpath);
            //var dstInfo = new SKImageInfo(1060, 550);

            //bitmap.Resize(dstInfo, SKBitmapResizeMethod.Hamming);
            ////image.
            //SKBitmap bitmap = SKBitmap.FromImage(skimage);
            ////bitmap.Resize(new SKImageInfo(100, 100), SKBitmapResizeMethod.Lanczos3);
            //var byteArray = bitmap.Bytes;
            //Stream stream = new MemoryStream(byteArray);
            //ImageDim.Source = ImageSource.FromStream(()=> { return stream; });
            //using (var scaledBitmap = bitmap.Resize(new SKImageInfo(500, 100), SKBitmapResizeMethod.Lanczos3))
            //{
            //    using (var image = SKImage.FromBitmap(scaledBitmap))
            //    {
            //        using (var png = image.Encode(SKEncodedImageFormat.Png, 100))
            //        {
            //            File.Create("C:\\AZ\\scaled.png");
            //            using (var filestream = File.OpenWrite("C:\\AZ\\scaled.png"))
            //            {
            //                png.SaveTo(filestream);
            //            }
            //        }
            //    }
            //}
        }

        private void OnTouch(object sender, SKTouchEventArgs e)
        {
            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    // start of a stroke
                    var p = new SKPath();
                    p.MoveTo(e.Location);
                    temporaryPaths[e.Id] = p;
                    break;
                case SKTouchAction.Moved:
                    // the stroke, while pressed
                    if (e.InContact)
                        temporaryPaths[e.Id].LineTo(e.Location);
                    break;
                case SKTouchAction.Released:
                    // end of a stroke
                    paths.Add(temporaryPaths[e.Id]);
                    temporaryPaths.Remove(e.Id);
                    break;
                case SKTouchAction.Cancelled:
                    // we don't want that stroke
                    temporaryPaths.Remove(e.Id);
                    break;
            }

            // we have handled these events
            e.Handled = true;
           
          
            ((SKCanvasView)sender).InvalidateSurface();
        }

        private void Cleane_Click(object sender, EventArgs e)
        {
            temporaryPaths.Clear();
            paths.Clear();
            GridCanvas.Children.Clear();
            GridCanvas.Children.Add(CanvasView);
        }
        private void Save_Click(object sender, EventArgs e)
        {
            if (skimage == null)
            {
                return;
            }
            SKData pngImage = skimage.Encode();
            var byteArray = pngImage.ToArray();
            Stream stream = new MemoryStream(byteArray);
            ImageDim.Source = ImageSource.FromStream(() => { return stream; });

            SKBitmap bitmap = SKBitmap.FromImage(skimage);
            var arrColors = bitmap.Pixels;
        }
    }
}
