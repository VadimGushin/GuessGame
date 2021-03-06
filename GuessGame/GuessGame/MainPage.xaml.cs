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
            answers = new List<Answer>();
        }

        private void OnPainting(object sender, SKPaintSurfaceEventArgs e)
        {
            surface = e.Surface;
            canvas = surface.Canvas;
            canvas.Clear(SKColors.White);
            var touchPathStroke = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Black,
                StrokeWidth = 5
            };
            foreach (var touchPath in temporaryPaths)
            {
                canvas.DrawPath(touchPath.Value, touchPathStroke);
            }
            foreach (var touchPath in paths)
            {
                canvas.DrawPath(touchPath, touchPathStroke);
            }

            skimage = surface.Snapshot();

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
            SKBitmap bitmap = SKBitmap.FromImage(skimage);
            var scaledBitmap = bitmap.Resize(new SKImageInfo(400, 400), SKBitmapResizeMethod.Triangle);
            skimage = SKImage.FromBitmap(scaledBitmap);
            bitmap = SKBitmap.FromImage(skimage);

            SKData pngImage = skimage.Encode();
            var byteArray = pngImage.ToArray();
            Stream stream = new MemoryStream(byteArray);
            ImageDim.Source = ImageSource.FromStream(() => { return stream; });
            SKColor[] arrColors = bitmap.Pixels;
            List<bool> boolImage = new List<bool>();
            foreach (var item in arrColors)
            {
                boolImage.Add(item.ToFormsColor() == Color.Black);
            }

        }

        private List<Answer> answers;
        private void Leaning_Click(object sender, EventArgs e)
        {
            if (skimage == null)
            {
                return;
            }
            SKBitmap bitmap = SKBitmap.FromImage(skimage);
            var scaledBitmap = bitmap.Resize(new SKImageInfo(400, 400), SKBitmapResizeMethod.Triangle);
            skimage = SKImage.FromBitmap(scaledBitmap);
            bitmap = SKBitmap.FromImage(skimage);

            SKData pngImage = skimage.Encode();
            var byteArray = pngImage.ToArray();
            Stream stream = new MemoryStream(byteArray);
            ImageDim.Source = ImageSource.FromStream(() => { return stream; });
            SKColor[] arrColors = bitmap.Pixels;
            List<bool> boolImage = new List<bool>();
            foreach (var item in arrColors)
            {
                boolImage.Add(item.ToFormsColor() == Color.Black);
            }
            
            var namePicture = NamePicture.Text;
            if (string.IsNullOrEmpty(namePicture))
            {
                DisplayAlert("Error", "empty name", "ok");
                return;
            }

            var answer = new Answer();
            string check = "";
            do
            {
                if (answers.Any(x => x.Name == namePicture))
                {
                    answer = answers.FirstOrDefault(x => x.Name == namePicture);
                   

                    for (int i = 0; i < boolImage.Count; i++)
                    {
                        if (boolImage[i])
                        {
                            answer.Weights[i] += 0.02;
                        }
                        else
                        {
                            answer.Weights[i] -= 0.005;
                        }
                    }
                }
                else
                {
                    answer.Name = namePicture;
                    
                    foreach (var isBlackPic in boolImage)
                    {
                        if (isBlackPic)
                        {
                            answer.Weights.Add(0.02);
                        }
                        else
                        {
                            answer.Weights.Add(0.001);
                        }
                    }

                    answers.Add(answer);
                }
                check = ChekeResult();
            } while (check != namePicture);

            NamePicture.Text = string.Empty;
            Cleane_Click(null, null);
            DisplayAlert("Status", "Success", "ok");
        }

        private void Check_Click(object sender, EventArgs e)
        {

            string answer = ChekeResult();
            if (!string.IsNullOrEmpty(answer))
            {
                DisplayAlert("Answer", answer, "ok");
            }
        }

        private string ChekeResult()
        {
            if (skimage == null)
            {
                return null;
            }
            SKBitmap bitmap = SKBitmap.FromImage(skimage);
            var scaledBitmap = bitmap.Resize(new SKImageInfo(400, 400), SKBitmapResizeMethod.Triangle);
            skimage = SKImage.FromBitmap(scaledBitmap);
            bitmap = SKBitmap.FromImage(skimage);

            SKData pngImage = skimage.Encode();
            var byteArray = pngImage.ToArray();
            Stream stream = new MemoryStream(byteArray);
            ImageDim.Source = ImageSource.FromStream(() => { return stream; });
            SKColor[] arrColors = bitmap.Pixels;
            List<bool> boolImage = new List<bool>();
            foreach (var item in arrColors)
            {
                boolImage.Add(item.ToFormsColor() == Color.Black);
            }

            string answer = "no";
            double resultamountWeights = double.MinValue;
            foreach (var item in answers)
            {
                double amountWeights = 0.1;
                for (int i = 0; i < item.Weights.Count; i++)
                {
                    double valueImage = boolImage[i] ? 1 : 0;
                    amountWeights += valueImage * item.Weights[i];
                }
                if (resultamountWeights < amountWeights)
                {
                    resultamountWeights = amountWeights;
                    answer = item.Name;
                }
            }

            return answer;
        }
    }
    public class Answer
    {
        public Answer()
        {
            Weights = new List<double>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<double> Weights { get; set; }
    }
}
