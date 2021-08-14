using MaterialSkin2DotNet;
using Newtonsoft.Json;
using PTGI_Remastered.Inputs;
using PTGI_Remastered.Structs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
            Settings = new SettingsView();
            pictureBox1.MouseWheel += pictureBox1_MouseWheel;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists(@".\settings.json"))
                Settings = JsonConvert.DeserializeObject<SettingsView>(File.ReadAllText(@".\settings.json"));
            else
                Settings.Default();
            settingsViewBindingSource.DataSource = Settings;

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);

            objectMaterialControl.Items.AddRange(Enum.GetNames(typeof(PTGI_Remastered.Utilities.PTGI_MaterialReflectivness)));
            objectMaterialControl.SelectedIndex = 0;

            PopupMessage = renderReportBox;
            RenderTimeMessageText = renderReportTime;
            ProcessTimeMessageText = processReportTime;
            DenoiserTimeMessageText = denoiserReportTime;
            TotalTimeMessageText = totalReportTime;
            RenderedPictureBox = pictureBox1;
            QueuedVerticiesList = verticiesListControl;

            HidePopupMessage();

            UpdateObjectSettings(null, null);

            PathTracer = new PTGI_Remastered.PTGI();
            Denoiser = new PTGI_Denoiser.Denoiser();
            Polygons = new List<PTGI_Remastered.Structs.Polygon>();
            ResetZoom();

            GpuId = null;
            var gpus = PathTracer.GetAvaiableHardwareAccelerators().ToArray();
            gpuSelectorControl.Items.AddRange(gpus);
            gpuSelectorControl.DisplayMember = "Name";
            gpuSelectorControl.ValueMember = "Id";
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

        private void startRenderButton_Click(object sender, EventArgs e)
        {
            if (IsRenderingInProgress)
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
                QueuedVerticiesList.Items.Add($"{clickCoordinates.X};{clickCoordinates.Y};{ZoomFactor}");
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

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            CurrentZoomDelta += e.Delta;
            ZoomFactor = CurrentZoomDelta / (float)DefaultZoomDelta;
            ZoomImage();
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
            else if(e.KeyCode == Keys.Space)
            {
                ResetZoom();
                ZoomImage();
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

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Settings.IsLivePreview)
                RenderImageWithMouseAsLight(e.X, e.Y);
        }

        private void resetSceneButton_Click(object sender, EventArgs e)
        {
            ClearScene();
        }

        private void buttonSaveSettings_Click(object sender, EventArgs e)
        {
            Settings.Save();
        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            pictureBox1.Focus();
        }
    }
}
