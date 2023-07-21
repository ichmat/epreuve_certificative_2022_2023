using FreshTech.Tools;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreshTech.Views.Dashboard
{
    public class GraphicsDrawableGraph : IDrawable
    {
        public Dictionary<string, double> Values = new Dictionary<string, double>();

        public const double SPACE_LEFT_LABEL = 50;
        public const double SPACE_BOTTOM_LABEL = 100;

        public const float SIZE_STROKE_FRAME = 2;

        public const double SIZE_HORIZONTAL_AXIS = 40;

        public const double SIZE_CUBE = 20;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            double min = 0;
            double max = 10;
            if(Values.Count > 0)
            {
                min = Values.Values.OrderBy(x => x).First() - 1;
                max = Values.Values.OrderBy(x => x).Last() + 1;

                if (min < 0) min = 0;
            }

            DrawHorizontalAxix(canvas, dirtyRect, min, max, out double pixelForOne, out int tickValue);

            DrawingCube(canvas, dirtyRect, min, max, pixelForOne);
            
            DrawFrame(canvas, dirtyRect);
        }

        private void DrawFrame(ICanvas canvas, RectF dirtyRect)
        {
            canvas.StrokeColor = ColorsTools.Gray500;
            canvas.StrokeSize = SIZE_STROKE_FRAME;

            canvas.DrawLine(
                (float)SPACE_LEFT_LABEL, 0,
                (float)SPACE_LEFT_LABEL, (float)(dirtyRect.Height - SPACE_BOTTOM_LABEL)
                );

            canvas.DrawLine(
               (float)SPACE_LEFT_LABEL, (float)(dirtyRect.Height - SPACE_BOTTOM_LABEL),
               (float)dirtyRect.Width, (float)(dirtyRect.Height - SPACE_BOTTOM_LABEL)
               );
        }

        private void DrawHorizontalAxixOld(ICanvas canvas, RectF dirtyRect, double min, double max)
        {
            canvas.StrokeColor = ColorsTools.Gray200;
            canvas.StrokeSize = 1;

            double sizeY = dirtyRect.Height - SPACE_BOTTOM_LABEL;
            double sizeX = dirtyRect.Width - SPACE_LEFT_LABEL;

            // récupère la taille minimum de la largeur de la ligne
            int tickPixelMin = (int)Math.Ceiling(sizeY / SIZE_HORIZONTAL_AXIS);
            // récupère le nombre de ligne maximum possible 
            int nbTickMax = (int)Math.Floor(Math.Floor(sizeY) / tickPixelMin);
            // nombre de ligne demandées
            int nbTickRequest = (int)Math.Round(max- min);

            int tickValue = nbTickMax >= nbTickRequest ? nbTickRequest : nbTickMax;

            double pixelTick = (int)Math.Ceiling(sizeY / tickValue);

            double y = sizeY;
            while(y >= 0)
            {
                canvas.DrawLine(
                    (float)SPACE_LEFT_LABEL, (float)y,
                    (float)dirtyRect.Width, (float)y
                    );

                y -= pixelTick;
            }
        }

        private void DrawHorizontalAxix(ICanvas canvas, RectF dirtyRect, double min, double max, out double pixelForOne, out int tickValue)
        {

            canvas.StrokeColor = ColorsTools.Gray200;
            canvas.StrokeSize = 1;

            int nb = (int)Math.Ceiling(max - min);
            double sizeY = dirtyRect.Height - SPACE_BOTTOM_LABEL;

            tickValue = 1;

            double pixelTick = sizeY / nb;

            while (pixelTick < SIZE_HORIZONTAL_AXIS)
            {
                tickValue *= 2;
                pixelTick = sizeY / Math.Ceiling(nb / (double)tickValue);
            }

            double y = sizeY;
            double value = min;
            while (y >= 0)
            {
                canvas.DrawLine(
                    (float)SPACE_LEFT_LABEL, (float)y,
                    (float)dirtyRect.Width, (float)y
                    );

                canvas.DrawString(value.ToString()
                    , 0, (float)(y - pixelTick /2)
                    , (float)(SPACE_LEFT_LABEL / 1.5), (float)pixelTick
                    , HorizontalAlignment.Right, VerticalAlignment.Center);

                y -= pixelTick;
                value += tickValue;
            }

            pixelForOne = pixelTick / tickValue;
        }
    
        private void DrawingCube(ICanvas canvas, RectF dirtyRect, double min, double max, double pixelForOne)
        {
            if (Values.Count() == 0) return;

            canvas.StrokeColor = ColorsTools.Primary;
            canvas.FillColor = ColorsTools.Primary;
            canvas.StrokeSize = 1;
            double sizeX = dirtyRect.Width - SPACE_LEFT_LABEL;
            double sizeY = dirtyRect.Height - SPACE_BOTTOM_LABEL;

            double tickPixel = ((sizeX - SIZE_CUBE) / Values.Count());

            double x = SPACE_LEFT_LABEL + SIZE_CUBE / 2;

            foreach (var item in Values)
            {
                double sizeYCube = (item.Value - min) * pixelForOne;

                canvas.FillRoundedRectangle(
                    (float)x, (float)(sizeY - sizeYCube),
                    (float)SIZE_CUBE, (float)sizeYCube,
                    8,8,0,0
                    );

                canvas.Rotate(90, (float)x, (float)sizeY);
                
                canvas.DrawString(
                    item.Key,
                    (float)(x), (float)(sizeY - SIZE_CUBE),
                    (float)SPACE_BOTTOM_LABEL, (float)SIZE_CUBE,
                    HorizontalAlignment.Center,
                    VerticalAlignment.Center
                    );

                canvas.Rotate(-90, (float)x, (float)sizeY);

                x += tickPixel;
            }
        }
    }
}
