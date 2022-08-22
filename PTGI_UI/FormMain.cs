using Cyotek.Windows.Forms;
using ILGPU.Runtime;
using MaterialSkin2DotNet.Controls;
using PTGI_Remastered.Inputs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using PTGI_Remastered.Classes;
using PTGI_Remastered.Structs;
using PTGI_Remastered.Utilities;
using Bitmap = System.Drawing.Bitmap;
using Color = System.Drawing.Color;
using SystemPoint = System.Drawing.Point;
using Point = PTGI_Remastered.Classes.Point;

namespace PTGI_UI
{
    public class FormMain : MaterialForm
    {
        protected const int DefaultZoomDelta = 1000;

        protected PTGI_Denoiser.Denoiser Denoiser { get; set; }
        protected PTGI_Remastered.PTGI PathTracer { get; set; }
        protected SettingsView Settings { get; set; }
        protected int popupTime { get; set; } = 0;
        protected List<Polygon> Polygons { get; set; }

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

        protected Polygon SelectedPolygon { get; set; }
        protected float ZoomFactor { get; set; }
        protected int CurrentZoomDelta { get; set; }

        protected void PathTraceThread(RenderSpecification renderSpecification = null)
        {
            try
            {
                IsRenderingInProgress = true;
                if (renderSpecification == null)
                {
                    renderSpecification = new RenderSpecification()
                    {
                        BounceLimit = Settings.BounceLimit,
                        DeviceId = Settings.DeviceId,
                        GridSize = Settings.GridDivider,
                        AcceleratorType = Settings.AcceleratorType,
                        ImageHeight = Settings.RenderHeight,
                        ImageWidth = Settings.RenderWidth,
                        Objects = Polygons.ToArray(),
                        SampleCount = Settings.SamplesPerPixel,
                        IgnoreEnclosedPixels = Settings.RenderFlag_IgnoreObstacleInterior
                    };
                }

                var pathTraceResult = PathTracer.PathTraceRender(renderSpecification);

                var pixels = pathTraceResult.Pixels;

                var denoiseResult = new PTGI_Denoiser.DenoiseResult();
                if (Settings.UseDenoiser)
                {
                    denoiseResult = Denoiser.Denoise(new PTGI_Denoiser.DenoiseRequest()
                    {
                        bitmap = pathTraceResult.bitmap,
                        DeviceId = Settings.DeviceId,
                        Pixels = pathTraceResult.Pixels,
                        KernelSize = Settings.DenoiserKernelSize,
                        IterationCount = Settings.DenoiserIterationCount
                    });
                    pixels = denoiseResult.Pixels;
                }

                this.Invoke(new MethodInvoker(delegate ()
                {
                    var stopwatch = Stopwatch.StartNew();
                    RenderedPictureBox.BackgroundImage = RenderedImage = ApplyBitmap(pixels, pathTraceResult.bitmap.Size);
                    stopwatch.Stop();
                    ShowPopupMessage($"Render time: {pathTraceResult.RenderTime} ms;" +
                        $"Allocation time: {pathTraceResult.ProcessTime - pathTraceResult.RenderTime} ms;" +
                        $"Denoiser time: {denoiseResult.DenoiseTime} ms;" +
                        $"Apply bitmap: {stopwatch.ElapsedMilliseconds} ms;" +
                        $"Total time: {pathTraceResult.ProcessTime + denoiseResult.DenoiseTime + stopwatch.ElapsedMilliseconds} ms", 10);
                    ZoomImage();
                }));
            }
            catch (Exception ex)
            {
                Settings.IsLivePreview = false;
                this.Invoke(new MethodInvoker(delegate ()
                {
                    if (ex.InnerException == null)
                        MessageBox.Show(this, ex.Message, "Render failed");
                    else if (ex.InnerException.InnerException == null)
                        MessageBox.Show(this, ex.InnerException.Message, "Render failed");
                    else
                        MessageBox.Show(this, ex.InnerException.InnerException.Message, "Render failed");
                }));
            }
            IsRenderingInProgress = false;
        }

        private Bitmap ApplyBitmap(PTGI_Remastered.Structs.Color[] pixels, int bitmapSize)
        {
            var bmp = new Bitmap(Settings.RenderWidth, Settings.RenderHeight, PixelFormat.Format24bppRgb);
            var rect = new Rectangle(0, 0, Settings.RenderWidth, Settings.RenderHeight);
            var data = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
            var depth = Image.GetPixelFormatSize(data.PixelFormat) / 8; //bytes per pixel

            var buffer = new byte[data.Width * data.Height * depth];

            //copy pixels to buffer
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);

            Parallel.For(0, bitmapSize, (i) =>
            {
                var row = i % Settings.RenderWidth;
                var col = i / Settings.RenderWidth;
                var c = Color.FromArgb((int)(pixels[i].R * 255.0), (int)(pixels[i].G * 255.0), (int)(pixels[i].B * 255.0));

                Process(buffer, row, col, Settings.RenderWidth, depth, c);
            });

            //Copy the buffer back to image
            Marshal.Copy(buffer, 0, data.Scan0, buffer.Length);

            bmp.UnlockBits(data);
            return bmp;
        }

        private static void Process(IList<byte> buffer, int x, int y, int width, int depth, Color c)
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
            if (popupTime <= 0) return;
            popupTime--;

            if (popupTime == 0)
            {
                HidePopupMessage();
            }
        }

        protected virtual void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            for (var i = 0; i < QueuedVerticiesList.Items.Count; i++)
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
                foreach(var polygon in Polygons)
                {
                    for(var i = 0; i < polygon.Walls.Length; i++)
                    {
                        e.Graphics.DrawLine(
                            new Pen(Color.Green, 1f),
                            polygon.Walls[i].Source.X * ZoomFactor,
                            polygon.Walls[i].Source.Y * ZoomFactor,
                            polygon.Walls[i].Destination.X * ZoomFactor,
                            polygon.Walls[i].Destination.Y * ZoomFactor);
                    }
                }
            }

            if (SelectedPolygon != null && Polygons.Count > 0)
            {
                foreach (var wall in SelectedPolygon.Walls)
                {
                    e.Graphics.DrawLine(
                            new Pen(Color.Red, 1f),
                            wall.Source.X * ZoomFactor,
                            wall.Source.Y * ZoomFactor,
                            wall.Destination.X * ZoomFactor,
                            wall.Destination.Y * ZoomFactor);
                }
            }

            if (!Settings.DrawGrid) return;
            
            var bitmap = new PTGI_Remastered.Structs.Bitmap();
            bitmap.SetBitmapSettings(Settings.RenderWidth, Settings.RenderHeight, 0);
            var cellGrid = new SGrid();
            cellGrid.Create(bitmap, Settings.GridDivider);
            for (var i = 0; i < cellGrid.GridSize; i++)
            {
                var row = (int)Math.Floor(i % (float)Settings.GridDivider);
                var col = (int)Math.Floor(i / (float)Settings.GridDivider);

                e.Graphics.DrawRectangle(
                    new Pen(Color.Blue, 1f),
                    (row * cellGrid.CellWidth) * ZoomFactor,
                    (col * cellGrid.CellHeight) * ZoomFactor,
                    cellGrid.CellWidth * ZoomFactor,
                    cellGrid.CellHeight * ZoomFactor);
                
            }
            
        }

        protected virtual void saveSceneButton_Click(object sender, EventArgs e)
        {
            try
            {
                var aSerializer = new XmlSerializer(typeof(List<Polygon>));
                var sb = new StringBuilder();
                var sw = new StringWriter(sb);
                aSerializer.Serialize(sw, Polygons);
                var xmlResult = sw.GetStringBuilder().ToString();

                using var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Save Scene";
                saveFileDialog.Filter = "XML Files|*.xml";
                
                if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
                
                using var streamWriter = new StreamWriter(saveFileDialog.FileName);
                streamWriter.Write(xmlResult);
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
                using var openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Open scene file";
                openFileDialog.Filter = "XML Files|*.xml";
                if (openFileDialog.ShowDialog() != DialogResult.OK) return;
                
                using var streamReader = new StreamReader(openFileDialog.FileName);
                var aSerializer = new XmlSerializer(typeof(List<Polygon>));
                Polygons = (List<Polygon>)aSerializer.Deserialize(streamReader);
                RenderedPictureBox.Refresh();
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
            
            var color = new PTGI_Remastered.Structs.Color();
            color.SetColor(ObjectColor.R, ObjectColor.G, ObjectColor.B);

            var vertices = new List<Point>();

            for (var i = 0; i < QueuedVerticiesList.Items.Count; i++)
            {
                var pointZoomFactor = float.Parse(QueuedVerticiesList.Items[i].Text.Split(';')[2]);
                var x = float.Parse(QueuedVerticiesList.Items[i].Text.Split(';')[0]) * (1 / pointZoomFactor);
                var y = float.Parse(QueuedVerticiesList.Items[i].Text.Split(';')[1]) * (1 / pointZoomFactor);

                var point = new Point(x, y);

                vertices.Add(point);
            }
            var polygon = new Polygon(vertices.ToArray(),
                IsObjectEmittingLight ? PTGI_ObjectTypes.LightSource : PTGI_ObjectTypes.Solid,
                (PTGI_MaterialReflectivness)Enum.Parse(typeof(PTGI_MaterialReflectivness), SelectedObjectMaterial), 
                color,
                ObjectEmissionStrength, 
                ObjectDensity);
            polygon.Name = ObjectName;

            Polygons.Add(polygon);
            QueuedVerticiesList.Items.Clear();
        }

        protected void SelectObject(System.Drawing.Point mouseLocation)
        {
            if (Polygons.Count <= 0)
                return;

            var mousePoint = new Point(mouseLocation.X, mouseLocation.Y);
            var selectedPolygon = Polygons.FirstOrDefault(polygon => mousePoint.LiesInObject(polygon));
            SelectedPolygon = selectedPolygon == SelectedPolygon ? null : selectedPolygon;
            
            UpdateControlsBySelectedPolygon();
        }

        protected virtual void UpdateControlsBySelectedPolygon()
        {
            
        }

        protected void SaveRender()
        {
            try
            {
                using var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Save an Image File";
                saveFileDialog.Filter = "PNG Files|*.png";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    RenderedPictureBox.BackgroundImage.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
            catch(Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Save render error");
            }
        }

        protected void DeleteObject()
        {
            if (Polygons.Count <= 0 && SelectedPolygon is null)
                return;

            try
            {
                Polygons.Remove(SelectedPolygon);
                SelectedPolygon = null; 
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
            var rnd = new Random();
            var blockSize = Settings.TerrariaWorldCellSize;
            var maxY = Settings.RenderHeight / 2;
            var generatorStartingPoints = new List<Point>();

            for (var x = 0; x <= Settings.RenderWidth - (blockSize+1); x += blockSize)
            {
                for(var y = maxY; y <= Settings.RenderHeight - (blockSize+1); y += blockSize)
                {
                    var topRight = new Point(x, y);
                    var rightBottom = new Point(x + blockSize, y + blockSize);
                    
                    var color = new PTGI_Remastered.Structs.Color();
                    color.SetColor(255, 255, 255);
                    var block = new Polygon(new Point[] { topRight, rightBottom }, PTGI_ObjectTypes.Solid, PTGI_MaterialReflectivness.Rough, color, 1, 1);
                    block.Name = $"{x};{y}";
                    Polygons.Add(block);

                    if (rnd.NextDouble() < 0.3)
                    {
                        generatorStartingPoints.Add(new Point()
                        {
                            X = x,
                            Y = y
                        });
                    }
                }
                var yDirection = rnd.Next(2) == 0 ? -blockSize : blockSize;
                maxY += yDirection;
            }

            for(var x = 0; x < generatorStartingPoints.Count; x++)
            {
                var lifetime = rnd.Next(10, 20);
                for(var i = 0; i < lifetime; i++)
                {
                    Polygons.RemoveAll(z => z.Name == $"{generatorStartingPoints[x].X};{generatorStartingPoints[x].Y}");

                    switch (rnd.Next(4))
                    {
                        case 0:
                            generatorStartingPoints[x] = new Point(generatorStartingPoints[x].X + blockSize, generatorStartingPoints[x].Y);
                            break;
                        case 1:
                            generatorStartingPoints[x] = new Point(generatorStartingPoints[x].X - blockSize, generatorStartingPoints[x].Y);
                            break;
                        case 2:
                            generatorStartingPoints[x] = new Point(generatorStartingPoints[x].X, generatorStartingPoints[x].Y + blockSize);
                            break;
                        case 3:
                            generatorStartingPoints[x] = new Point(generatorStartingPoints[x].X, generatorStartingPoints[x].Y - blockSize);
                            break;
                    }
                }
            }
        }

        private int _previousObjectCount = 0;
        protected void GenerateScene()
        {
            Polygons.Clear();
            var rnd = new Random();

            var objectCount = _previousObjectCount;
            while (objectCount == _previousObjectCount)
                objectCount = rnd.Next(10, 20);

            const int vertexSpread = 200;
            _previousObjectCount = objectCount;
            var generatedLight = false;
            var forceLightGeneration = false;
            for (var i = 0; i < objectCount; i++)
            {
                var vertexCount = rnd.Next(2, 5);
                var objectType = rnd.NextDouble();
                var vertices = new List<Point>();
                for (var v = 0; v < vertexCount; v++)
                {
                    var point = new Point();
                    while (true)
                    {
                        if (!vertices.Any())
                            point = new Point(rnd.Next(Settings.RenderWidth), rnd.Next(Settings.RenderHeight));
                        else
                            point = new Point(
                                rnd.Next(Math.Max(0, (int)vertices.LastOrDefault().X - vertexSpread),
                                    Math.Min(Settings.RenderWidth, (int)vertices.LastOrDefault().X + vertexSpread)),
                                rnd.Next(Math.Max(0, (int)vertices.LastOrDefault().Y - vertexSpread),
                                    Math.Min(Settings.RenderHeight, (int)vertices.LastOrDefault().Y + vertexSpread)));

                        var liesInPolygon = Polygons.Any(tempPolygons => point.LiesInObject(tempPolygons));

                        if (!liesInPolygon)
                            break;
                    }
                    vertices.Add(point);
                }
                var color = new PTGI_Remastered.Structs.Color();
                color.SetColor(rnd.Next(10, 255), rnd.Next(10, 255), rnd.Next(10, 255));
                if (objectType > 0.8)
                    generatedLight = true;
                if (!generatedLight && i == objectCount - 1)
                    forceLightGeneration = true;

                var polygon = new Polygon(vertices.ToArray(),
                    objectType > 0.8 || forceLightGeneration ? PTGI_ObjectTypes.LightSource : PTGI_ObjectTypes.Solid,
                    PTGI_MaterialReflectivness.Rough,
                    color,
                    (float)(rnd.NextDouble()+0.7),
                    ObjectDensity);

                Polygons.Add(polygon);
            }
        }

        protected void RenderImageWithMouseAsLight(int mouseX, int mouseY)
        {
            if (!Settings.IsLivePreview) return;
            mouseX = (int) (mouseX / ZoomFactor);
            mouseY = (int) (mouseY / ZoomFactor);
            
            var color = new PTGI_Remastered.Structs.Color();
            color.SetColor(ObjectColor.R, ObjectColor.G, ObjectColor.B);

            var verticies = new Point[]
            {
                new(mouseX, mouseY),
                new(mouseX + 5, mouseY + 5)
            };

            var polygon = new Polygon(verticies,
                IsObjectEmittingLight
                    ? PTGI_ObjectTypes.LightSource
                    : PTGI_ObjectTypes.Solid,
                (PTGI_MaterialReflectivness) Enum.Parse(
                    typeof(PTGI_MaterialReflectivness), SelectedObjectMaterial),
                color,
                ObjectEmissionStrength,
                ObjectDensity);
            polygon.Name = "MouseCursor";
            Polygons.Add(polygon);
            PathTraceThread();
            Polygons.RemoveAll(x => x.Name == "MouseCursor");
            RenderedPictureBox.Refresh();
        }

        protected void MoveSelected(Keys keyPress)
        {
            if (SelectedPolygon is null)
                return;

            var movementSpeed = 3;
            switch (keyPress)
            {
                case Keys.Up:
                    SelectedPolygon.Move(0, -movementSpeed);
                    break;
                case Keys.Down:
                    SelectedPolygon.Move(0, movementSpeed);
                    break;
                case Keys.Left:
                    SelectedPolygon.Move(-movementSpeed, 0);
                    break;
                case Keys.Right:
                    SelectedPolygon.Move(movementSpeed, 0);
                    break;
            }
            PathTraceThread();
            RenderedPictureBox.Refresh();
        }

        protected void UpdateSelected()
        {
            if (SelectedPolygon is null)
                return;

            var color = new PTGI_Remastered.Structs.Color();
            color.SetColor(ObjectColor.R, ObjectColor.G, ObjectColor.B);
            SelectedPolygon.Color = color;
            SelectedPolygon.objectType = IsObjectEmittingLight ? PTGI_ObjectTypes.LightSource : PTGI_ObjectTypes.Solid;
            SelectedPolygon.reflectivnessType = (PTGI_MaterialReflectivness) Enum.Parse(typeof(PTGI_MaterialReflectivness), SelectedObjectMaterial);
            SelectedPolygon.EmissionStrength = ObjectEmissionStrength;
            SelectedPolygon.Density = ObjectDensity;
            SelectedPolygon.Name = ObjectName;
            SelectedPolygon.IsUpdated = true;
        }
    }
}
