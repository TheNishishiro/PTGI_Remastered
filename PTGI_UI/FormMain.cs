using Cyotek.Windows.Forms;
using ILGPU.Runtime;
using MaterialSkin2DotNet.Controls;
using PTGI_Remastered.Inputs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace PTGI_UI
{
    public class FormMain : MaterialForm
    {
        protected const int DefaultZoomDelta = 1000;

        protected PTGI_Denoiser.Denoiser Denoiser { get; set; }
        protected PTGI_Remastered.PTGI PathTracer { get; set; }
        protected SettingsView Settings { get; set; }
        protected AcceleratorId GpuId { get; set; }
        protected int popupTime { get; set; } = 0;
        protected List<PTGI_Remastered.Structs.Polygon> Polygons { get; set; }

        protected MaterialCard PopupMessage { get; set; }
        protected MaterialLabel RenderTimeMessageText { get; set; }
        protected MaterialLabel ProcessTimeMessageText { get; set; }
        protected MaterialLabel DenoiserTimeMessageText { get; set; }
        protected MaterialLabel TotalTimeMessageText { get; set; }

        protected MaterialListView QueuedVerticiesList { get; set; }
        protected PictureBox RenderedPictureBox { get; set; }
        protected Image RenderedImage { get; set; }
        protected bool IsObjectEmittingLight { get; set; }

        protected string SelectedObjectMaterial { get; set; }
        protected Color ObjectColor { get; set; }
        protected float ObjectEmissionStrength { get; set; }
        protected float ObjectDensity { get; set; }
        protected string ObjectName { get; set; }

        protected bool IsRenderingInProgress { get; set; }

        protected int SelectedPolygon { get; set; }
        protected float ZoomFactor { get; set; }
        protected int CurrentZoomDelta { get; set; }

        protected void PathTraceThread()
        {
            try
            {
                IsRenderingInProgress = true;
                var pathTraceResult = PathTracer.PathTraceRender(
                    new RenderSpecification()
                    {
                        BounceLimit = Settings.BounceLimit,
                        GpuId = GpuId,
                        GridSize = Settings.GridDivider,
                        UseCUDARenderer = Settings.UseCUDA,
                        ImageHeight = Settings.RenderHeight,
                        ImageWidth = Settings.RenderWidth,
                        Objects = Polygons.ToArray(),
                        SampleCount = Settings.SamplesPerPixel,
                        IgnoreEnclosedPixels = Settings.RenderFlag_IgnoreObstacleInterior
                    });

                var pixels = pathTraceResult.Pixels;

                var denoiseResult = new PTGI_Denoiser.DenoiseResult();
                if (Settings.UseDenoiser)
                {
                    denoiseResult = Denoiser.Denoise(new PTGI_Denoiser.DenoiseRequest()
                    {
                        bitmap = pathTraceResult.bitmap,
                        GpuId = GpuId,
                        Pixels = pathTraceResult.Pixels,
                        KernelSize = Settings.DenoiserKernelSize
                    });
                    pixels = denoiseResult.Pixels;
                }

                this.Invoke(new MethodInvoker(delegate () {
                    ShowPopupMessage($"Render time: {pathTraceResult.RenderTime} ms;" +
                        $"Allocation time: {pathTraceResult.ProcessTime - pathTraceResult.RenderTime} ms;" +
                        $"Denoiser time: {denoiseResult.DenoiseTime} ms;" +
                        $"Total time: {pathTraceResult.ProcessTime + denoiseResult.DenoiseTime} ms", 10);
                    RenderedPictureBox.BackgroundImage = RenderedImage = ApplyBitmap(pixels, pathTraceResult.bitmap.Size);
                    ZoomImage();
                }));
            }
            catch (Exception ex)
            {
                Settings.IsLivePreview = false;
                this.Invoke(new MethodInvoker(delegate ()
                {
                    MessageBox.Show(this, ex.Message, "Render failed");
                }));
            }
            IsRenderingInProgress = false;
        }

        protected Bitmap ApplyBitmap(PTGI_Remastered.Structs.Color[] pixels, int bitmapSize)
        {
            Bitmap bmp = new Bitmap(Settings.RenderWidth, Settings.RenderHeight, PixelFormat.Format24bppRgb);
            var rect = new Rectangle(0, 0, Settings.RenderWidth, Settings.RenderHeight);
            var data = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
            var depth = Bitmap.GetPixelFormatSize(data.PixelFormat) / 8; //bytes per pixel

            var buffer = new byte[data.Width * data.Height * depth];

            //copy pixels to buffer
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);

            Parallel.For(0, bitmapSize, (i) =>
            {
                int row = i % Settings.RenderWidth;
                int col = i / Settings.RenderWidth;
                Color c = Color.FromArgb((int)(pixels[i].R * 255.0), (int)(pixels[i].G * 255.0), (int)(pixels[i].B * 255.0));

                Process(buffer, row, col, Settings.RenderWidth, depth, c);
            });

            //Copy the buffer back to image
            Marshal.Copy(buffer, 0, data.Scan0, buffer.Length);

            bmp.UnlockBits(data);
            return bmp;
        }

        private void Process(byte[] buffer, int x, int y, int width, int depth, Color c)
        {
            var offset = ((y * width) + x) * depth;
            buffer[offset + 0] = c.B;
            buffer[offset + 1] = c.G;
            buffer[offset + 2] = c.R;
        }

        protected void ZoomImage()
        {
            if (RenderedImage == null || ZoomFactor == 1)
            {
                RenderedPictureBox.Size = new Size(Settings.RenderWidth, Settings.RenderHeight);
                return;
            }

            var bitmap = new Bitmap(RenderedImage, Convert.ToInt32(RenderedImage.Width * ZoomFactor), Convert.ToInt32(RenderedImage.Height * ZoomFactor));
            var graphics = Graphics.FromImage(bitmap);
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
            RenderedPictureBox.BackgroundImage = bitmap;
            RenderedPictureBox.Size = new Size(bitmap.Width, bitmap.Height);
        }

        protected void ResetZoom()
        {
            ZoomFactor = 1;
            CurrentZoomDelta = DefaultZoomDelta;
        }

        protected void ShowPopupMessage(string text, int popupTime)
        {
            if (text.StartsWith("Render in progress..."))
            {
                PopupMessage.Visible = true;
                RenderTimeMessageText.Visible = true;
                RenderTimeMessageText.Text = text;
                return;
            }

            PopupMessage.Visible = true;
            RenderTimeMessageText.Visible = true;
            RenderTimeMessageText.Text = text.Split(';')[0];
            ProcessTimeMessageText.Visible = true;
            ProcessTimeMessageText.Text = text.Split(';')[1];
            DenoiserTimeMessageText.Visible = true;
            DenoiserTimeMessageText.Text = text.Split(';')[2];
            TotalTimeMessageText.Visible = true;
            TotalTimeMessageText.Text = text.Split(';')[3];
            this.popupTime = popupTime;
        }

        protected void HidePopupMessage()
        {
            PopupMessage.Visible = false;
            RenderTimeMessageText.Visible = false;
            ProcessTimeMessageText.Visible = false;
            DenoiserTimeMessageText.Visible = false;
            TotalTimeMessageText.Visible = false;
        }

        protected void PopupMessageTimeout()
        {
            if (popupTime > 0)
            {
                popupTime--;

                if (popupTime == 0)
                {
                    HidePopupMessage();
                }
            }
        }

        protected virtual void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < QueuedVerticiesList.Items.Count; i++)
            {
                var pointZoomFactor = float.Parse(QueuedVerticiesList.Items[i].Text.Split(';')[2]);
                e.Graphics.DrawEllipse(
                    new Pen(Color.Red, 2f),
                    float.Parse(QueuedVerticiesList.Items[i].Text.Split(';')[0]) * (ZoomFactor / pointZoomFactor),
                    float.Parse(QueuedVerticiesList.Items[i].Text.Split(';')[1]) * (ZoomFactor / pointZoomFactor),
                    2,
                    2);
            }

            if(Settings.DrawObjectsOverline)
            {
                int polygonId = 0;
                foreach(var polygon in Polygons)
                {
                    for(int i = 0; i < polygon.Walls.Length; i++)
                    {
                        e.Graphics.DrawLine(
                            new Pen(Color.Green, 1f),
                            polygon.Walls[i].Source.X * ZoomFactor,
                            polygon.Walls[i].Source.Y * ZoomFactor,
                            polygon.Walls[i].Destination.X * ZoomFactor,
                            polygon.Walls[i].Destination.Y * ZoomFactor);
                    }
                    polygonId++;
                }
            }

            if (SelectedPolygon != -1 && Polygons.Count > 0 && Settings.DrawObjectsOverline)
            {
                var polygon = Polygons[SelectedPolygon];
                foreach (var wall in polygon.Walls)
                {
                    e.Graphics.DrawLine(
                            new Pen(Color.Red, 1f),
                            wall.Source.X * ZoomFactor,
                            wall.Source.Y * ZoomFactor,
                            wall.Destination.X * ZoomFactor,
                            wall.Destination.Y * ZoomFactor);
                }
            }

            if (Settings.DrawGrid)
            {
                PTGI_Remastered.Structs.Bitmap bitmap = new PTGI_Remastered.Structs.Bitmap();
                bitmap.SetBitmapSettings(Settings.RenderWidth, Settings.RenderHeight, 0);
                PTGI_Remastered.Structs.Grid cellGrid = new PTGI_Remastered.Structs.Grid();
                cellGrid.Create(bitmap, Settings.GridDivider);
                for (int i = 0; i < cellGrid.GridSize; i++)
                {
                    int row = (int)Math.Floor(i % (float)Settings.GridDivider);
                    int col = (int)Math.Floor(i / (float)Settings.GridDivider);

                    e.Graphics.DrawRectangle(
                        new Pen(Color.Blue, 1f),
                        (row * cellGrid.CellWidth) * ZoomFactor,
                        (col * cellGrid.CellHeight) * ZoomFactor,
                        cellGrid.CellWidth * ZoomFactor,
                        cellGrid.CellHeight * ZoomFactor);
                    
                }
            }
        }

        protected virtual void saveSceneButton_Click(object sender, EventArgs e)
        {
            try
            {
                var aSerializer = new XmlSerializer(typeof(List<PTGI_Remastered.Structs.Polygon>));
                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                aSerializer.Serialize(sw, Polygons);
                string xmlResult = sw.GetStringBuilder().ToString();

                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Title = "Save Scene";
                    saveFileDialog.Filter = "XML Files|*.xml";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        using (var streamWriter = new StreamWriter(saveFileDialog.FileName))
                        {
                            streamWriter.Write(xmlResult);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Save error");
            }
        }

        protected virtual void loadSceneButton_Click(object sender, EventArgs e)
        {
            try 
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Title = "Open scene file";
                    openFileDialog.Filter = "XML Files|*.xml";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        using (var streamReader = new StreamReader(openFileDialog.FileName))
                        {
                            var aSerializer = new XmlSerializer(typeof(List<PTGI_Remastered.Structs.Polygon>));
                            Polygons = (List<PTGI_Remastered.Structs.Polygon>)aSerializer.Deserialize(streamReader);
                            RenderedPictureBox.Refresh();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Load error");
            }
        }

        protected void AddPolygonToObjects()
        {
            if (QueuedVerticiesList.Items.Count < 2)
                return;

            PTGI_Remastered.Structs.Polygon polygon = new PTGI_Remastered.Structs.Polygon();
            PTGI_Remastered.Structs.Color color = new PTGI_Remastered.Structs.Color();
            color.SetColor(ObjectColor.R, ObjectColor.G, ObjectColor.B);

            List<PTGI_Remastered.Structs.Point> Vertices = new List<PTGI_Remastered.Structs.Point>();

            for (int i = 0; i < QueuedVerticiesList.Items.Count; i++)
            {
                var pointZoomFactor = float.Parse(QueuedVerticiesList.Items[i].Text.Split(';')[2]);
                float x = float.Parse(QueuedVerticiesList.Items[i].Text.Split(';')[0]) * (1 / pointZoomFactor);
                float y = float.Parse(QueuedVerticiesList.Items[i].Text.Split(';')[1]) * (1 / pointZoomFactor);

                PTGI_Remastered.Structs.Point point = new PTGI_Remastered.Structs.Point();
                point.SetCoords(x, y);

                Vertices.Add(point);
            }
            polygon.Name = ObjectName;
            polygon.Setup(Vertices.ToArray(),
                IsObjectEmittingLight ? PTGI_Remastered.Utilities.PTGI_ObjectTypes.LightSource : PTGI_Remastered.Utilities.PTGI_ObjectTypes.Solid,
                (PTGI_Remastered.Utilities.PTGI_MaterialReflectivness)Enum.Parse(typeof(PTGI_Remastered.Utilities.PTGI_MaterialReflectivness), SelectedObjectMaterial), 
                color,
                ObjectEmissionStrength, 
                ObjectDensity);

            Polygons.Add(polygon);
            QueuedVerticiesList.Items.Clear();
        }

        protected void SelectObject(Point mouseLocation)
        {
            if (Polygons.Count <= 0)
                return;

            var mousePoint = new PTGI_Remastered.Structs.Point();
            mousePoint.SetCoords(mouseLocation.X, mouseLocation.Y);

            for (int i = 0; i < Polygons.Count; i++)
            {
                if (mousePoint.LiesInObject(Polygons[i]))
                {
                    SelectedPolygon = i;
                    break;
                }
            }
        }

        protected void SaveRender()
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Title = "Save an Image File";
                    saveFileDialog.Filter = "PNG Files|*.png";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        RenderedPictureBox.BackgroundImage.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Save render error");
            }
        }

        protected void DeleteObject()
        {
            if (Polygons.Count <= 0 && SelectedPolygon < 0)
                return;

            try
            {
                Polygons.RemoveAt(SelectedPolygon);
                SelectedPolygon = -1; 
            }
            catch(Exception ex)
            {
                MessageBox.Show("Object you are trying to remove no longer exists");
            }
        }

        protected void ClearScene()
        {
            Polygons.Clear();
            RenderedPictureBox.Refresh();
        }

        protected void TerrariaWorldGenerator()
        {
            Random rnd = new Random();
            var blockSize = Settings.TerrariaWorldCellSize;
            int maxY = Settings.RenderHeight / 2;
            var generatorStartingPoints = new List<PTGI_Remastered.Structs.Point>();

            for (int x = 0; x <= Settings.RenderWidth - (blockSize+1); x += blockSize)
            {
                for(int y = maxY; y <= Settings.RenderHeight - (blockSize+1); y += blockSize)
                {
                    PTGI_Remastered.Structs.Point topRight = new PTGI_Remastered.Structs.Point();
                    topRight.SetCoords(x, y);
                    PTGI_Remastered.Structs.Point rightBottom = new PTGI_Remastered.Structs.Point();
                    rightBottom.SetCoords(x + blockSize, y + blockSize);

                    PTGI_Remastered.Structs.Polygon block = new PTGI_Remastered.Structs.Polygon();
                    PTGI_Remastered.Structs.Color color = new PTGI_Remastered.Structs.Color();
                    color.SetColor(255, 255, 255);
                    block.Setup(new PTGI_Remastered.Structs.Point[] { topRight, rightBottom }, PTGI_Remastered.Utilities.PTGI_ObjectTypes.Solid, PTGI_Remastered.Utilities.PTGI_MaterialReflectivness.Rough, color, 1, 1);
                    block.Name = $"{x};{y}";
                    Polygons.Add(block);

                    if (rnd.NextDouble() < 0.3)
                    {
                        generatorStartingPoints.Add(new PTGI_Remastered.Structs.Point()
                        {
                            X = x,
                            Y = y
                        });
                    }
                }
                int yDirection = rnd.Next(2) == 0 ? -blockSize : blockSize;
                maxY += yDirection;
            }

            for(int x = 0; x < generatorStartingPoints.Count; x++)
            {
                var lifetime = rnd.Next(10, 20);
                for(int i = 0; i < lifetime; i++)
                {
                    Polygons.RemoveAll(z => z.Name == $"{generatorStartingPoints[x].X};{generatorStartingPoints[x].Y}");

                    switch (rnd.Next(4))
                    {
                        case 0:
                            generatorStartingPoints[x].SetCoords(generatorStartingPoints[x].X + blockSize, generatorStartingPoints[x].Y);
                            break;
                        case 1:
                            generatorStartingPoints[x].SetCoords(generatorStartingPoints[x].X - blockSize, generatorStartingPoints[x].Y);
                            break;
                        case 2:
                            generatorStartingPoints[x].SetCoords(generatorStartingPoints[x].X, generatorStartingPoints[x].Y + blockSize);
                            break;
                        case 3:
                            generatorStartingPoints[x].SetCoords(generatorStartingPoints[x].X, generatorStartingPoints[x].Y - blockSize);
                            break;
                    }
                }
            }
        }

        protected void RenderImageWithMouseAsLight(int mouseX, int mouseY)
        {
            if (Settings.IsLivePreview)
            {
                mouseX = (int)(mouseX / ZoomFactor);
                mouseY = (int)(mouseY / ZoomFactor);

                PTGI_Remastered.Structs.Polygon polygon = new PTGI_Remastered.Structs.Polygon();
                PTGI_Remastered.Structs.Color color = new PTGI_Remastered.Structs.Color();
                color.SetColor(ObjectColor.R, ObjectColor.G, ObjectColor.B);

                var verticies = new PTGI_Remastered.Structs.Point[]
                {
                    new PTGI_Remastered.Structs.Point()
                    {
                        X = mouseX,
                        Y = mouseY,
                        HasValue = 1
                    },
                    new PTGI_Remastered.Structs.Point()
                    {
                        X = mouseX+5,
                        Y = mouseY+5,
                        HasValue = 1
                    }
                };

                polygon.Name = "MouseCursor";
                polygon.Setup(verticies,
                    IsObjectEmittingLight ? PTGI_Remastered.Utilities.PTGI_ObjectTypes.LightSource : PTGI_Remastered.Utilities.PTGI_ObjectTypes.Solid,
                    (PTGI_Remastered.Utilities.PTGI_MaterialReflectivness)Enum.Parse(typeof(PTGI_Remastered.Utilities.PTGI_MaterialReflectivness), SelectedObjectMaterial),
                    color,
                    ObjectEmissionStrength,
                    ObjectDensity);

                Polygons.Add(polygon);
                PathTraceThread();
                Polygons.RemoveAll(x => x.Name == "MouseCursor");
                RenderedPictureBox.Refresh();
            }
        }
    }
}
