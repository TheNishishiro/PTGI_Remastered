using Alea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTGI_Remastered.Structs
{
    [Serializable]
    public struct Color
    {
        public double R;
        public double G;
        public double B;

        public void SetColor(double R, double G, double B)
        {
            this.R = R;
            this.G = G;
            this.B = B;
        }

        public void SetColor(Color color, double mixFactor)
        {
            R = color.R * mixFactor;
            G = color.G * mixFactor;
            B = color.B * mixFactor;
        }

        public void TintWith(Color color, double mixFactor)
        {
            R *= color.R * mixFactor;
            G *= color.G * mixFactor;
            B *= color.B * mixFactor;
        }

        public void ApplyGammaCorrection(double scale)
        {
            R = DeviceFunction.Sqrt(R * scale);
            G = DeviceFunction.Sqrt(G * scale);
            B = DeviceFunction.Sqrt(B * scale);
        }

        public void Clip()
        {
            if (R > 1)
                R = 1;
            else if (R < 0)
                R = 0;

            if (G > 1)
                G = 1;
            else if (G < 0)
                G = 0;

            if (B > 1)
                B = 1;
            else if (B < 0)
                B = 0;
        }

        public void Rescale(double factor)
        {
            R /= factor;
            G /= factor;
            B /= factor;
        }

        public Color GetRescaled(double factor)
        {
            Color newColor = new Color();
            newColor.SetColor(R / factor, G / factor, B / factor);
            return newColor;
        }

        public void Multiply(Color color)
        {
            R *= color.R;
            G *= color.G;
            B *= color.B;
        }

        public void Add(Color color)
        {
            R += color.R;
            G += color.G;
            B += color.B;
        }
    }

    /// <summary>
    /// Custom bitmap to contain colors of pixel and basic information about the render
    /// </summary>
    public struct Bitmap
    {
        public Color[] pixels;
        public int Size;
        public int Width;
        public int Height;

        public void CopyPixels(Color[] pixels, int Width, int Height)
        {
            this.pixels = pixels;
            this.Width = Width;
            this.Height = Height;
            Size = Width * Height;
        }

        public void Create(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            Size = Width * Height;

            pixels = new Color[Size];
        }

        public void SetPixel(int id, Color pixelColor, double gammaCorrectionScale)
        {
            pixelColor.ApplyGammaCorrection(gammaCorrectionScale);
            pixelColor.Clip();
            pixels[id] = pixelColor;
        }
    }
}
