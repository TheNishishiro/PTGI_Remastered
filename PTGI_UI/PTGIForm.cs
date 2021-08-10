using MaterialSkin;
using MaterialSkin.Controls;
using PTGI_Remastered.Inputs;
using PTGI_Remastered.Structs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bitmap = System.Drawing.Bitmap;
using Color = System.Drawing.Color;

namespace PTGI_UI
{
    public partial class PTGIForm : FormMain
    {
        public PTGIForm()
        {
            InitializeComponent();
        }

        private bool _isRendering = false;
        public void PathTraceThread()
        {
            try
            {
                _isRendering = true;
                var pathTraceResult = PathTracer.PathTraceRender(
                    new RenderSpecification()
                    {
                        BounceLimit = BounceLimit,
                        GpuId = GpuId,
                        GridSize = GridDivider,
                        UseCUDARenderer = UseCUDA,
                        ImageHeight = RenderHeight,
                        ImageWidth = RenderWidth,
                        Objects = Polygons.ToArray(),
                        SampleCount = SamplesPerPixel,
                        IgnoreEnclosedPixels = RenderFlag_IgnoreObstacleInterior
                    });
               

                this.Invoke(new MethodInvoker(delegate () {
                    ShowPopupMessage($"Render/Process time: {pathTraceResult.RenderTime}/{pathTraceResult.ProcessTime - pathTraceResult.RenderTime} ms", 5);
                    RenderedPictureBox.Size = new Size(RenderWidth, RenderHeight);
                    RenderedPictureBox.BackgroundImage = ApplyBitmap(pathTraceResult);
                }));
            }
            catch(Exception ex)
            {
                isLiveRender = false;
                this.Invoke(new MethodInvoker(delegate ()
                {
                    MessageBox.Show(this, ex.Message, "Render failed");
                }));
            }
            _isRendering = false;
        }

        private Bitmap ApplyBitmap(RenderResult pathTraceResult)
        {
            Bitmap bmp = new Bitmap(RenderWidth, RenderHeight, PixelFormat.Format24bppRgb);
            var rect = new Rectangle(0, 0, RenderWidth, RenderHeight);
            var data = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
            var depth = Bitmap.GetPixelFormatSize(data.PixelFormat) / 8; //bytes per pixel

            var buffer = new byte[data.Width * data.Height * depth];

            //copy pixels to buffer
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);

            Parallel.For(0, pathTraceResult.bitmap.Size, (i) =>
            {
                int row = i % RenderWidth;
                int col = i / RenderWidth;
                Color c = Color.FromArgb((int)(pathTraceResult.Pixels[i].R * 255.0), (int)(pathTraceResult.Pixels[i].G * 255.0), (int)(pathTraceResult.Pixels[i].B * 255.0));

                Process(buffer, row, col, RenderWidth, depth, c);
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

        private void Form1_Load(object sender, EventArgs e)
        {
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);

            ControlExtension.Draggable(renderReportBox, true);
            ControlExtension.Draggable(objectSpecificationCard, true);
            ControlExtension.Draggable(renderingButtonsPanel, true);

            objectMaterialControl.Items.AddRange(Enum.GetNames(typeof(PTGI_Remastered.Utilities.PTGI_MaterialReflectivness)));
            objectMaterialControl.SelectedIndex = 0;

            PopupMessage = renderReportBox;
            PopupMessageText = renderReportTime;
            RenderedPictureBox = pictureBox1;
            QueuedVerticiesList = verticiesListControl;
            WorldObjectList = objectListControl;
            WorldObjectList.DisplayMember = "Name";

            DrawGrid = drawCellGridControl.Checked;

            HidePopupMessage();

            RenderWidth = int.Parse(renderResolutionWidth.Text);
            RenderHeight = int.Parse(renderResolutionHeight.Text);
            SamplesPerPixel = int.Parse(samplePerPixelControl.Text);
            UseCUDA = useCudaCheckbox.Checked;
            BounceLimit = int.Parse(bounceLimitControl.Text);
            GridDivider = int.Parse(gridDividerControl.Text);
            DrawObjectsOverline = objectsOverlineControl.Checked;

            UpdateObjectSettings(null, null);

            PathTracer = new PTGI_Remastered.PTGI();
            Polygons = new List<PTGI_Remastered.Structs.Polygon>();

            RenderFlag_IgnoreObstacleInterior = renderFlagIgnoreObstacleInteriors.Checked;

            GpuId = null;
            try
            {
                var gpus = PathTracer.GetAvaiableHardwareAccelerators().ToArray();
                gpuSelectorControl.Items.AddRange(gpus);
                gpuSelectorControl.DisplayMember = "Name";
                gpuSelectorControl.ValueMember = "Id";
            }
            catch(Exception ex)
            {

            }
        }

        private void rendererSettingsApply_Click(object sender, EventArgs e)
        {
            UseCUDA = useCudaCheckbox.Checked;
            BounceLimit = int.Parse(bounceLimitControl.Text);
            GridDivider = int.Parse(gridDividerControl.Text);

            if(UseCUDA && gpuSelectorControl.SelectedIndex >= 0)
                GpuId = ((PTGI_Remastered.Structs.Gpu)gpuSelectorControl.SelectedItem).Id;
        }

        private void renderSettingsApply_Click(object sender, EventArgs e)
        {
            RenderWidth = int.Parse(renderResolutionWidth.Text);
            RenderHeight = int.Parse(renderResolutionHeight.Text);
            SamplesPerPixel = int.Parse(samplePerPixelControl.Text);
        }

        private void UpdateObjectSettings(object sender, EventArgs e)
        {
            IsObjectEmittingLight = emitsLightControl.Checked;
            SelectedObjectMaterial = (string)objectMaterialControl.SelectedItem;
            ObjectColor = colorEditor1.Color;
            ObjectEmissionStrength = float.Parse(objectEmissionStrengthControl.Text.Replace('.', ','));
            ObjectDensity = float.Parse(objectDensityControl.Text.Replace('.', ','));
            ObjectName = objectNameControl.Text;
            colorDisplayPictureBox.BackColor = ObjectColor;
        }

        private void applyDebugSettingsButton_Click(object sender, EventArgs e)
        {
            DrawGrid = drawCellGridControl.Checked;
            DrawObjectsOverline = objectsOverlineControl.Checked;
        }

        private void startRenderButton_Click(object sender, EventArgs e)
        {
            if (_isRendering)
                return;

            var renderThread = new Thread(() => PathTraceThread());
            ShowPopupMessage("Render in progress...", int.MaxValue);
            
            renderThread.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            PopupMessageTimeout();
        }

        private void applyObjectSpecification_Click(object sender, EventArgs e)
        {
            IsObjectEmittingLight = emitsLightControl.Checked;
        }

        private void deleteVerticie_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MouseEventArgs mouseEvent = (MouseEventArgs)e;

            if(mouseEvent.Button == MouseButtons.Left)
            {
                var clickCoordinates = mouseEvent.Location;
                QueuedVerticiesList.Items.Add($"{clickCoordinates.X};{clickCoordinates.Y}");
                Refresh();
            }
            else if(mouseEvent.Button == MouseButtons.Right)
            {
                SelectObject(mouseEvent.Location);
                Refresh();
            }
        }

        protected override void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            base.pictureBox1_Paint(sender, e);
        }

        private void PTGIForm_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.I)
            {
                AddPolygonToObjects();
                Refresh();
            }
            else if(e.KeyCode == Keys.R)
            {
                Refresh();
            }
            else if(e.KeyCode == Keys.Delete)
            {
                DeleteObject();
                Refresh();
            }
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            DeleteObject();
        }

        private void saveRenderButton_Click(object sender, EventArgs e)
        {
            SaveRender();
        }

        protected override void saveSceneButton_Click(object sender, EventArgs e)
        {
            base.saveSceneButton_Click(sender, e);
        }

        protected override void loadSceneButton_Click(object sender, EventArgs e)
        {
            base.loadSceneButton_Click(sender, e);
        }

        private void objectNameControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                ActiveControl = materialLabel1;
        }

        private void minecraftWorldGeneration_Click(object sender, EventArgs e)
        {
            TerrariaWorldGenerator();
        }

        private void renderFlagsApply_Click(object sender, EventArgs e)
        {
            RenderFlag_IgnoreObstacleInterior = renderFlagIgnoreObstacleInteriors.Checked;
        }

        private bool isLiveRender = false;
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                isLiveRender = true;
            else if (e.Button == MouseButtons.Middle)
                isLiveRender = false;

            if (isLiveRender)
            {
                PTGI_Remastered.Structs.Polygon polygon = new PTGI_Remastered.Structs.Polygon();
                PTGI_Remastered.Structs.Color color = new PTGI_Remastered.Structs.Color();
                color.SetColor(ObjectColor.R, ObjectColor.G, ObjectColor.B);

                List<PTGI_Remastered.Structs.Point> Vertices = new List<PTGI_Remastered.Structs.Point>();

                var verticies = new PTGI_Remastered.Structs.Point[]
                {
                new PTGI_Remastered.Structs.Point()
                {
                    X = e.X,
                    Y = e.Y,
                    HasValue = 1
                },
                new PTGI_Remastered.Structs.Point()
                {
                    X = e.X+5,
                    Y = e.Y+5,
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

        private void resetSceneButton_Click(object sender, EventArgs e)
        {
            ClearScene();
        }
    }
}
