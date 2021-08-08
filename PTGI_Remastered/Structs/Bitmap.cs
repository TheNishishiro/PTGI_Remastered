using ILGPU;
using ILGPU.Algorithms;
using ILGPU.Runtime;
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
        public float R;
        public float G;
        public float B;

        public void SetColor(float R, float G, float B)
        {
            this.R = R;
            this.G = G;
            this.B = B;
        }

        public void SetColor(Color color, float mixFactor)
        {
            R = color.R * mixFactor;
            G = color.G * mixFactor;
            B = color.B * mixFactor;
        }

        public void TintWith(Color color, float mixFactor)
        {
            R *= color.R * mixFactor;
            G *= color.G * mixFactor;
            B *= color.B * mixFactor;
        }

        public void ApplyGammaCorrection(float scale)
        {
            R = XMath.Sqrt(R * scale);
            G = XMath.Sqrt(G * scale);
            B = XMath.Sqrt(B * scale);
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

        public void Rescale(float factor)
        {
            R /= factor;
            G /= factor;
            B /= factor;
        }

        public Color GetRescaled(float factor)
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
        public int Size;
        public int Width;
        public int Height;
        public int WallsCount;

        public MemoryBuffer<Color> CreateColorArrayView(int Width, int Height, int wallsCount, Accelerator accelerator)
        {
            this.Width = Width;
            this.Height = Height;
            Size = Width * Height;
            WallsCount = wallsCount;
            return accelerator.Allocate<Color>(Size);
        }

        public void SetPixel(int id, Color pixelColor, float gammaCorrectionScale, ArrayView<Color> pixels)
        {
            pixelColor.ApplyGammaCorrection(gammaCorrectionScale);
            pixelColor.Clip();
            pixels[id] = pixelColor;
        }
    }
}
