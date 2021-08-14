using ILGPU;
using ILGPU.Algorithms;
using ILGPU.Runtime;
using PTGI_Remastered.Utilities;
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
        public byte Skip;

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

        public float GetSimpleLuminance()
        {
            return XMath.Sqrt(0.299f * PTGI_Math.Pow(R, 2) + 0.587f * PTGI_Math.Pow(G, 2) + 0.114f * PTGI_Math.Pow(B, 2));
        }

        public float GetPerceivedLightness()
        {
            var luminance = GetLuminance();

            if (luminance <= (216 / 24389))
            {     
                return luminance * (24389 / 27); 
            }
            else
            {
                return PTGI_Math.PowFloat(luminance, (1 / 3)) * 116 - 16;
            }
        }

        public float GetLuminance()
        {
            return 0.2126f * SrgbToLin(R) + 0.7152f * SrgbToLin(G) + 0.0722f * SrgbToLin(B);
        }

        public float SrgbToLin(float colorChannel)
        {
            if (colorChannel <= 0.04045f)
            {
                return colorChannel / 12.92f;
            }
            else
            {
                return PTGI_Math.PowFloat(((colorChannel + 0.055f) / 1.055f), 2.4f);
            }
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
        public int GridSize;

        public void SetBitmapSettings(int Width, int Height, int wallsCount)
        {
            this.Width = Width;
            this.Height = Height;
            Size = Width * Height;
            WallsCount = wallsCount;
        }

        public void SetPixel(int id, Color pixelColor, float gammaCorrectionScale, ArrayView<Color> pixels)
        {
            pixelColor.ApplyGammaCorrection(gammaCorrectionScale);
            pixelColor.Clip();
            pixels[id] = pixelColor;
        }
    }
}
