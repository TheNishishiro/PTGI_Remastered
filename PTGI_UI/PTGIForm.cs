using MaterialSkin;
using MaterialSkin.Controls;
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

namespace PTGI_UI
{
    public partial class PTGIForm : FormMain
    {
        public PTGIForm()
        {
            InitializeComponent();
        }

        public void PathTraceThread()
        {
            Bitmap bitmap = new Bitmap(RenderWidth, RenderHeight);
            try
            {
                var pathTraceResult = PathTracer.PathTraceRender(Polygons.ToArray(), RenderWidth, RenderHeight, SamplesPerPixel, BounceLimit, GridDivider, UseCUDA, GpuId);
                var pathTracedBitmap = pathTraceResult.bitmap;
                for (int i = 0; i < pathTracedBitmap.Size; i++)
                {
                    int row = i % RenderWidth;
                    int col = i / RenderWidth;
                    Color c = Color.FromArgb((int)(pathTracedBitmap.pixels[i].R * 255.0), (int)(pathTracedBitmap.pixels[i].G * 255.0), (int)(pathTracedBitmap.pixels[i].B * 255.0));

                    bitmap.SetPixel(row, col, c);
                }

                this.Invoke(new MethodInvoker(delegate () {
                    ShowPopupMessage($"Render time: {pathTraceResult.RenderTime} ms", 5);
                    RenderedPictureBox.Size = new Size(RenderWidth, RenderHeight);
                    RenderedPictureBox.BackgroundImage = bitmap;
                }));
            }
            catch(Exception ex)
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    MessageBox.Show(this, ex.Message, "Render failed");
                }));
            }
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

            GpuId = 0;
            try
            {
                var gpus = PathTracer.GetAvailableGpus().ToArray();
                gpuSelectorControl.Items.AddRange(gpus);
                gpuSelectorControl.DisplayMember = "Name";
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
            ObjectEmissionStrength = double.Parse(objectEmissionStrengthControl.Text.Replace('.', ','));
            ObjectDensity = double.Parse(objectDensityControl.Text.Replace('.', ','));
            ObjectName = objectNameControl.Text;
        }

        private void applyDebugSettingsButton_Click(object sender, EventArgs e)
        {
            DrawGrid = drawCellGridControl.Checked;
            DrawObjectsOverline = objectsOverlineControl.Checked;
        }

        private void startRenderButton_Click(object sender, EventArgs e)
        {
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
                SendDebugRay();
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
    }
}
