﻿using Newtonsoft.Json;
using PTGI_Remastered.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTGI_UI
{
    public class SettingsView
    {
        public void Default()
        {
            UseCUDA = true;
            DrawGrid = false;
            DrawObjectsOverline = true;
            RenderFlag_IgnoreObstacleInterior = true;
            BounceLimitControlValue = "7";
            GridDividerControlValue = "16";
            SamplesPerPixelControlValue = "20";
            RenderHeightControlValue = "640";
            RenderWidthControlValue = "800";
        }

        public void Save()
        {
            File.WriteAllText(@".\settings.json", JsonConvert.SerializeObject(this));
        }

        public bool UseCUDA { get; set; }
        public bool DrawGrid { get; set; }
        public bool DrawObjectsOverline { get; set; }
        public bool RenderFlag_IgnoreObstacleInterior { get; set; }
        public int RenderWidth { 
            get 
            {
                int.TryParse(RenderWidthControlValue, out int result);
                return result;
            } 
        }
        public int RenderHeight {
            get
            {
                int.TryParse(RenderHeightControlValue, out int result);
                return result;
            }
        }
        public int SamplesPerPixel { 
            get
            {
                int.TryParse(SamplesPerPixelControlValue, out int result);
                return result;
            }
        }
        public int BounceLimit { 
            get
            {
                int.TryParse(BounceLimitControlValue, out int result);
                return result;
            } 
        }
        public int GridDivider { 
            get
            {
                int.TryParse(GridDividerControlValue, out int result);
                return result;
            }
        }
        public Color ObjectColor { get; set; }

        public string BounceLimitControlValue { get; set; }
        public string GridDividerControlValue { get; set; }
        public string SamplesPerPixelControlValue { get; set; }
        public string RenderHeightControlValue { get; set; }
        public string RenderWidthControlValue { get; set; }
    }
}