
namespace PTGI_UI
{
    partial class PTGIForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.materialTabControl1 = new MaterialSkin.Controls.MaterialTabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.renderingButtonsPanel = new MaterialSkin.Controls.MaterialCard();
            this.saveRenderButton = new MaterialSkin.Controls.MaterialButton();
            this.startRenderButton = new MaterialSkin.Controls.MaterialButton();
            this.renderReportBox = new MaterialSkin.Controls.MaterialCard();
            this.renderReportTime = new MaterialSkin.Controls.MaterialLabel();
            this.objectSpecificationCard = new MaterialSkin.Controls.MaterialCard();
            this.objectEmissionStrengthControl = new MaterialSkin.Controls.MaterialTextBox();
            this.materialLabel12 = new MaterialSkin.Controls.MaterialLabel();
            this.objectDensityControl = new MaterialSkin.Controls.MaterialTextBox();
            this.materialLabel11 = new MaterialSkin.Controls.MaterialLabel();
            this.colorEditor1 = new Cyotek.Windows.Forms.ColorEditor();
            this.objectMaterialControl = new MaterialSkin.Controls.MaterialComboBox();
            this.materialLabel9 = new MaterialSkin.Controls.MaterialLabel();
            this.emitsLightControl = new MaterialSkin.Controls.MaterialSwitch();
            this.objectNameControl = new MaterialSkin.Controls.MaterialTextBox();
            this.materialLabel13 = new MaterialSkin.Controls.MaterialLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.materialCard2 = new MaterialSkin.Controls.MaterialCard();
            this.materialLabel5 = new MaterialSkin.Controls.MaterialLabel();
            this.renderSettingsApply = new MaterialSkin.Controls.MaterialButton();
            this.samplePerPixelControl = new MaterialSkin.Controls.MaterialTextBox();
            this.materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
            this.materialLabel3 = new MaterialSkin.Controls.MaterialLabel();
            this.renderResolutionWidth = new MaterialSkin.Controls.MaterialTextBox();
            this.renderResolutionHeight = new MaterialSkin.Controls.MaterialTextBox();
            this.materialCard1 = new MaterialSkin.Controls.MaterialCard();
            this.gpuSelectorControl = new MaterialSkin.Controls.MaterialComboBox();
            this.materialLabel7 = new MaterialSkin.Controls.MaterialLabel();
            this.rendererSettingsApply = new MaterialSkin.Controls.MaterialButton();
            this.gridDividerControl = new MaterialSkin.Controls.MaterialTextBox();
            this.materialLabel6 = new MaterialSkin.Controls.MaterialLabel();
            this.bounceLimitControl = new MaterialSkin.Controls.MaterialTextBox();
            this.materialLabel4 = new MaterialSkin.Controls.MaterialLabel();
            this.useCudaCheckbox = new MaterialSkin.Controls.MaterialSwitch();
            this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.materialCard3 = new MaterialSkin.Controls.MaterialCard();
            this.objectListControl = new MaterialSkin.Controls.MaterialComboBox();
            this.materialLabel8 = new MaterialSkin.Controls.MaterialLabel();
            this.materialButton1 = new MaterialSkin.Controls.MaterialButton();
            this.materialCard4 = new MaterialSkin.Controls.MaterialCard();
            this.verticiesListControl = new MaterialSkin.Controls.MaterialListView();
            this.columnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.materialLabel10 = new MaterialSkin.Controls.MaterialLabel();
            this.deleteVerticie = new MaterialSkin.Controls.MaterialButton();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.materialCard5 = new MaterialSkin.Controls.MaterialCard();
            this.objectsOverlineControl = new MaterialSkin.Controls.MaterialSwitch();
            this.drawCellGridControl = new MaterialSkin.Controls.MaterialSwitch();
            this.applyDebugSettingsButton = new MaterialSkin.Controls.MaterialButton();
            this.materialDrawer1 = new MaterialSkin.Controls.MaterialDrawer();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.saveSceneButton = new MaterialSkin.Controls.MaterialButton();
            this.loadSceneButton = new MaterialSkin.Controls.MaterialButton();
            this.materialCard6 = new MaterialSkin.Controls.MaterialCard();
            this.minecraftWorldGeneration = new MaterialSkin.Controls.MaterialButton();
            this.panel1.SuspendLayout();
            this.materialTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.renderingButtonsPanel.SuspendLayout();
            this.renderReportBox.SuspendLayout();
            this.objectSpecificationCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.materialCard2.SuspendLayout();
            this.materialCard1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.materialCard3.SuspendLayout();
            this.materialCard4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.materialCard5.SuspendLayout();
            this.materialCard6.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.materialTabControl1);
            this.panel1.Controls.Add(this.materialDrawer1);
            this.panel1.Location = new System.Drawing.Point(0, 63);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1602, 840);
            this.panel1.TabIndex = 0;
            // 
            // materialTabControl1
            // 
            this.materialTabControl1.Controls.Add(this.tabPage1);
            this.materialTabControl1.Controls.Add(this.tabPage2);
            this.materialTabControl1.Controls.Add(this.tabPage3);
            this.materialTabControl1.Controls.Add(this.tabPage5);
            this.materialTabControl1.Controls.Add(this.tabPage4);
            this.materialTabControl1.Depth = 0;
            this.materialTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.materialTabControl1.Location = new System.Drawing.Point(250, 0);
            this.materialTabControl1.Margin = new System.Windows.Forms.Padding(3, 300, 3, 3);
            this.materialTabControl1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialTabControl1.Multiline = true;
            this.materialTabControl1.Name = "materialTabControl1";
            this.materialTabControl1.SelectedIndex = 0;
            this.materialTabControl1.Size = new System.Drawing.Size(1352, 840);
            this.materialTabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.Controls.Add(this.renderingButtonsPanel);
            this.tabPage1.Controls.Add(this.renderReportBox);
            this.tabPage1.Controls.Add(this.objectSpecificationCard);
            this.tabPage1.Controls.Add(this.pictureBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1344, 814);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Render view";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // renderingButtonsPanel
            // 
            this.renderingButtonsPanel.AutoScroll = true;
            this.renderingButtonsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.renderingButtonsPanel.Controls.Add(this.loadSceneButton);
            this.renderingButtonsPanel.Controls.Add(this.saveSceneButton);
            this.renderingButtonsPanel.Controls.Add(this.saveRenderButton);
            this.renderingButtonsPanel.Controls.Add(this.startRenderButton);
            this.renderingButtonsPanel.Depth = 0;
            this.renderingButtonsPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.renderingButtonsPanel.Location = new System.Drawing.Point(1051, 696);
            this.renderingButtonsPanel.Margin = new System.Windows.Forms.Padding(14);
            this.renderingButtonsPanel.MouseState = MaterialSkin.MouseState.HOVER;
            this.renderingButtonsPanel.Name = "renderingButtonsPanel";
            this.renderingButtonsPanel.Padding = new System.Windows.Forms.Padding(14);
            this.renderingButtonsPanel.Size = new System.Drawing.Size(297, 93);
            this.renderingButtonsPanel.TabIndex = 6;
            // 
            // saveRenderButton
            // 
            this.saveRenderButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.saveRenderButton.Depth = 0;
            this.saveRenderButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.saveRenderButton.DrawShadows = true;
            this.saveRenderButton.HighEmphasis = true;
            this.saveRenderButton.Icon = null;
            this.saveRenderButton.Location = new System.Drawing.Point(14, 50);
            this.saveRenderButton.Margin = new System.Windows.Forms.Padding(4, 60, 4, 6);
            this.saveRenderButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.saveRenderButton.Name = "saveRenderButton";
            this.saveRenderButton.Size = new System.Drawing.Size(252, 36);
            this.saveRenderButton.TabIndex = 5;
            this.saveRenderButton.Text = "Save rendered image";
            this.saveRenderButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.saveRenderButton.UseAccentColor = false;
            this.saveRenderButton.UseVisualStyleBackColor = true;
            this.saveRenderButton.Click += new System.EventHandler(this.saveRenderButton_Click);
            // 
            // startRenderButton
            // 
            this.startRenderButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.startRenderButton.Depth = 0;
            this.startRenderButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.startRenderButton.DrawShadows = true;
            this.startRenderButton.HighEmphasis = true;
            this.startRenderButton.Icon = null;
            this.startRenderButton.Location = new System.Drawing.Point(14, 14);
            this.startRenderButton.Margin = new System.Windows.Forms.Padding(4, 6, 4, 60);
            this.startRenderButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.startRenderButton.Name = "startRenderButton";
            this.startRenderButton.Size = new System.Drawing.Size(252, 36);
            this.startRenderButton.TabIndex = 4;
            this.startRenderButton.Text = "Render";
            this.startRenderButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.startRenderButton.UseAccentColor = false;
            this.startRenderButton.UseVisualStyleBackColor = true;
            this.startRenderButton.Click += new System.EventHandler(this.startRenderButton_Click);
            // 
            // renderReportBox
            // 
            this.renderReportBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.renderReportBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.renderReportBox.Controls.Add(this.renderReportTime);
            this.renderReportBox.Depth = 0;
            this.renderReportBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.renderReportBox.Location = new System.Drawing.Point(1055, 3);
            this.renderReportBox.Margin = new System.Windows.Forms.Padding(14);
            this.renderReportBox.MouseState = MaterialSkin.MouseState.HOVER;
            this.renderReportBox.Name = "renderReportBox";
            this.renderReportBox.Padding = new System.Windows.Forms.Padding(14);
            this.renderReportBox.Size = new System.Drawing.Size(297, 51);
            this.renderReportBox.TabIndex = 5;
            // 
            // renderReportTime
            // 
            this.renderReportTime.AutoSize = true;
            this.renderReportTime.Depth = 0;
            this.renderReportTime.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.renderReportTime.Location = new System.Drawing.Point(17, 14);
            this.renderReportTime.MouseState = MaterialSkin.MouseState.HOVER;
            this.renderReportTime.Name = "renderReportTime";
            this.renderReportTime.Size = new System.Drawing.Size(81, 19);
            this.renderReportTime.TabIndex = 0;
            this.renderReportTime.Text = "renderTime";
            // 
            // objectSpecificationCard
            // 
            this.objectSpecificationCard.AutoScroll = true;
            this.objectSpecificationCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.objectSpecificationCard.Controls.Add(this.objectEmissionStrengthControl);
            this.objectSpecificationCard.Controls.Add(this.materialLabel12);
            this.objectSpecificationCard.Controls.Add(this.objectDensityControl);
            this.objectSpecificationCard.Controls.Add(this.materialLabel11);
            this.objectSpecificationCard.Controls.Add(this.colorEditor1);
            this.objectSpecificationCard.Controls.Add(this.objectMaterialControl);
            this.objectSpecificationCard.Controls.Add(this.materialLabel9);
            this.objectSpecificationCard.Controls.Add(this.emitsLightControl);
            this.objectSpecificationCard.Controls.Add(this.objectNameControl);
            this.objectSpecificationCard.Controls.Add(this.materialLabel13);
            this.objectSpecificationCard.Depth = 0;
            this.objectSpecificationCard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.objectSpecificationCard.Location = new System.Drawing.Point(1047, 62);
            this.objectSpecificationCard.Margin = new System.Windows.Forms.Padding(14);
            this.objectSpecificationCard.MouseState = MaterialSkin.MouseState.HOVER;
            this.objectSpecificationCard.Name = "objectSpecificationCard";
            this.objectSpecificationCard.Padding = new System.Windows.Forms.Padding(14);
            this.objectSpecificationCard.Size = new System.Drawing.Size(296, 628);
            this.objectSpecificationCard.TabIndex = 3;
            // 
            // objectEmissionStrengthControl
            // 
            this.objectEmissionStrengthControl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.objectEmissionStrengthControl.Depth = 0;
            this.objectEmissionStrengthControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.objectEmissionStrengthControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.objectEmissionStrengthControl.Location = new System.Drawing.Point(14, 536);
            this.objectEmissionStrengthControl.MaxLength = 50;
            this.objectEmissionStrengthControl.MouseState = MaterialSkin.MouseState.OUT;
            this.objectEmissionStrengthControl.Multiline = false;
            this.objectEmissionStrengthControl.Name = "objectEmissionStrengthControl";
            this.objectEmissionStrengthControl.Size = new System.Drawing.Size(268, 50);
            this.objectEmissionStrengthControl.TabIndex = 13;
            this.objectEmissionStrengthControl.Text = "1";
            // 
            // materialLabel12
            // 
            this.materialLabel12.AutoSize = true;
            this.materialLabel12.Depth = 0;
            this.materialLabel12.Dock = System.Windows.Forms.DockStyle.Top;
            this.materialLabel12.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel12.Location = new System.Drawing.Point(14, 517);
            this.materialLabel12.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel12.Name = "materialLabel12";
            this.materialLabel12.Size = new System.Drawing.Size(128, 19);
            this.materialLabel12.TabIndex = 15;
            this.materialLabel12.Text = "Emission strength";
            // 
            // objectDensityControl
            // 
            this.objectDensityControl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.objectDensityControl.Depth = 0;
            this.objectDensityControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.objectDensityControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.objectDensityControl.Location = new System.Drawing.Point(14, 467);
            this.objectDensityControl.MaxLength = 50;
            this.objectDensityControl.MouseState = MaterialSkin.MouseState.OUT;
            this.objectDensityControl.Multiline = false;
            this.objectDensityControl.Name = "objectDensityControl";
            this.objectDensityControl.Size = new System.Drawing.Size(268, 50);
            this.objectDensityControl.TabIndex = 12;
            this.objectDensityControl.Text = "1.4";
            // 
            // materialLabel11
            // 
            this.materialLabel11.AutoSize = true;
            this.materialLabel11.Depth = 0;
            this.materialLabel11.Dock = System.Windows.Forms.DockStyle.Top;
            this.materialLabel11.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel11.Location = new System.Drawing.Point(14, 448);
            this.materialLabel11.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel11.Name = "materialLabel11";
            this.materialLabel11.Size = new System.Drawing.Size(54, 19);
            this.materialLabel11.TabIndex = 14;
            this.materialLabel11.Text = "Density";
            // 
            // colorEditor1
            // 
            this.colorEditor1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.colorEditor1.Dock = System.Windows.Forms.DockStyle.Top;
            this.colorEditor1.Location = new System.Drawing.Point(14, 188);
            this.colorEditor1.Margin = new System.Windows.Forms.Padding(4);
            this.colorEditor1.Name = "colorEditor1";
            this.colorEditor1.Size = new System.Drawing.Size(268, 260);
            this.colorEditor1.TabIndex = 11;
            this.colorEditor1.ColorChanged += new System.EventHandler(this.UpdateObjectSettings);
            // 
            // objectMaterialControl
            // 
            this.objectMaterialControl.AutoResize = false;
            this.objectMaterialControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.objectMaterialControl.Depth = 0;
            this.objectMaterialControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.objectMaterialControl.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.objectMaterialControl.DropDownHeight = 174;
            this.objectMaterialControl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.objectMaterialControl.DropDownWidth = 121;
            this.objectMaterialControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.objectMaterialControl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.objectMaterialControl.FormattingEnabled = true;
            this.objectMaterialControl.IntegralHeight = false;
            this.objectMaterialControl.ItemHeight = 43;
            this.objectMaterialControl.Location = new System.Drawing.Point(14, 139);
            this.objectMaterialControl.MaxDropDownItems = 4;
            this.objectMaterialControl.MouseState = MaterialSkin.MouseState.OUT;
            this.objectMaterialControl.Name = "objectMaterialControl";
            this.objectMaterialControl.Size = new System.Drawing.Size(268, 49);
            this.objectMaterialControl.TabIndex = 10;
            this.objectMaterialControl.SelectedIndexChanged += new System.EventHandler(this.UpdateObjectSettings);
            // 
            // materialLabel9
            // 
            this.materialLabel9.AutoSize = true;
            this.materialLabel9.Depth = 0;
            this.materialLabel9.Dock = System.Windows.Forms.DockStyle.Top;
            this.materialLabel9.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel9.Location = new System.Drawing.Point(14, 120);
            this.materialLabel9.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel9.Name = "materialLabel9";
            this.materialLabel9.Size = new System.Drawing.Size(108, 19);
            this.materialLabel9.TabIndex = 9;
            this.materialLabel9.Text = "Object material";
            // 
            // emitsLightControl
            // 
            this.emitsLightControl.AutoSize = true;
            this.emitsLightControl.Checked = true;
            this.emitsLightControl.CheckState = System.Windows.Forms.CheckState.Checked;
            this.emitsLightControl.Depth = 0;
            this.emitsLightControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.emitsLightControl.Location = new System.Drawing.Point(14, 83);
            this.emitsLightControl.Margin = new System.Windows.Forms.Padding(0);
            this.emitsLightControl.MouseLocation = new System.Drawing.Point(-1, -1);
            this.emitsLightControl.MouseState = MaterialSkin.MouseState.HOVER;
            this.emitsLightControl.Name = "emitsLightControl";
            this.emitsLightControl.Ripple = true;
            this.emitsLightControl.Size = new System.Drawing.Size(268, 37);
            this.emitsLightControl.TabIndex = 8;
            this.emitsLightControl.Text = "Emit light";
            this.emitsLightControl.UseVisualStyleBackColor = true;
            this.emitsLightControl.CheckedChanged += new System.EventHandler(this.UpdateObjectSettings);
            // 
            // objectNameControl
            // 
            this.objectNameControl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.objectNameControl.Depth = 0;
            this.objectNameControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.objectNameControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.objectNameControl.Location = new System.Drawing.Point(14, 33);
            this.objectNameControl.MaxLength = 50;
            this.objectNameControl.MouseState = MaterialSkin.MouseState.OUT;
            this.objectNameControl.Multiline = false;
            this.objectNameControl.Name = "objectNameControl";
            this.objectNameControl.Size = new System.Drawing.Size(268, 50);
            this.objectNameControl.TabIndex = 16;
            this.objectNameControl.Text = "ObjectName";
            this.objectNameControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.objectNameControl_KeyDown);
            // 
            // materialLabel13
            // 
            this.materialLabel13.AutoSize = true;
            this.materialLabel13.Depth = 0;
            this.materialLabel13.Dock = System.Windows.Forms.DockStyle.Top;
            this.materialLabel13.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel13.Location = new System.Drawing.Point(14, 14);
            this.materialLabel13.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel13.Name = "materialLabel13";
            this.materialLabel13.Size = new System.Drawing.Size(92, 19);
            this.materialLabel13.TabIndex = 17;
            this.materialLabel13.Text = "Object Name";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(2, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1003, 674);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.materialCard2);
            this.tabPage2.Controls.Add(this.materialCard1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1344, 814);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Render settings";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // materialCard2
            // 
            this.materialCard2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.materialCard2.Controls.Add(this.materialLabel5);
            this.materialCard2.Controls.Add(this.renderSettingsApply);
            this.materialCard2.Controls.Add(this.samplePerPixelControl);
            this.materialCard2.Controls.Add(this.materialLabel2);
            this.materialCard2.Controls.Add(this.materialLabel3);
            this.materialCard2.Controls.Add(this.renderResolutionWidth);
            this.materialCard2.Controls.Add(this.renderResolutionHeight);
            this.materialCard2.Depth = 0;
            this.materialCard2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialCard2.Location = new System.Drawing.Point(329, 17);
            this.materialCard2.Margin = new System.Windows.Forms.Padding(14);
            this.materialCard2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialCard2.Name = "materialCard2";
            this.materialCard2.Padding = new System.Windows.Forms.Padding(14);
            this.materialCard2.Size = new System.Drawing.Size(548, 438);
            this.materialCard2.TabIndex = 3;
            // 
            // materialLabel5
            // 
            this.materialLabel5.AutoSize = true;
            this.materialLabel5.Depth = 0;
            this.materialLabel5.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel5.Location = new System.Drawing.Point(55, 140);
            this.materialLabel5.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel5.Name = "materialLabel5";
            this.materialLabel5.Size = new System.Drawing.Size(126, 19);
            this.materialLabel5.TabIndex = 7;
            this.materialLabel5.Text = "Samples per pixel";
            // 
            // renderSettingsApply
            // 
            this.renderSettingsApply.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.renderSettingsApply.Depth = 0;
            this.renderSettingsApply.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.renderSettingsApply.DrawShadows = true;
            this.renderSettingsApply.HighEmphasis = true;
            this.renderSettingsApply.Icon = null;
            this.renderSettingsApply.Location = new System.Drawing.Point(14, 388);
            this.renderSettingsApply.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.renderSettingsApply.MouseState = MaterialSkin.MouseState.HOVER;
            this.renderSettingsApply.Name = "renderSettingsApply";
            this.renderSettingsApply.Size = new System.Drawing.Size(520, 36);
            this.renderSettingsApply.TabIndex = 7;
            this.renderSettingsApply.Text = "Apply";
            this.renderSettingsApply.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.renderSettingsApply.UseAccentColor = false;
            this.renderSettingsApply.UseVisualStyleBackColor = true;
            this.renderSettingsApply.Click += new System.EventHandler(this.renderSettingsApply_Click);
            // 
            // samplePerPixelControl
            // 
            this.samplePerPixelControl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.samplePerPixelControl.Depth = 0;
            this.samplePerPixelControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.samplePerPixelControl.Location = new System.Drawing.Point(58, 162);
            this.samplePerPixelControl.MaxLength = 50;
            this.samplePerPixelControl.MouseState = MaterialSkin.MouseState.OUT;
            this.samplePerPixelControl.Multiline = false;
            this.samplePerPixelControl.Name = "samplePerPixelControl";
            this.samplePerPixelControl.Size = new System.Drawing.Size(440, 50);
            this.samplePerPixelControl.TabIndex = 8;
            this.samplePerPixelControl.Text = "20";
            // 
            // materialLabel2
            // 
            this.materialLabel2.AutoSize = true;
            this.materialLabel2.Depth = 0;
            this.materialLabel2.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel2.Location = new System.Drawing.Point(17, 14);
            this.materialLabel2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel2.Name = "materialLabel2";
            this.materialLabel2.Size = new System.Drawing.Size(112, 19);
            this.materialLabel2.TabIndex = 2;
            this.materialLabel2.Text = "Render Settings";
            // 
            // materialLabel3
            // 
            this.materialLabel3.AutoSize = true;
            this.materialLabel3.Depth = 0;
            this.materialLabel3.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel3.Location = new System.Drawing.Point(55, 46);
            this.materialLabel3.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel3.Name = "materialLabel3";
            this.materialLabel3.Size = new System.Drawing.Size(124, 19);
            this.materialLabel3.TabIndex = 5;
            this.materialLabel3.Text = "Render resolution";
            // 
            // renderResolutionWidth
            // 
            this.renderResolutionWidth.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.renderResolutionWidth.Depth = 0;
            this.renderResolutionWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.renderResolutionWidth.Location = new System.Drawing.Point(58, 74);
            this.renderResolutionWidth.MaxLength = 50;
            this.renderResolutionWidth.MouseState = MaterialSkin.MouseState.OUT;
            this.renderResolutionWidth.Multiline = false;
            this.renderResolutionWidth.Name = "renderResolutionWidth";
            this.renderResolutionWidth.Size = new System.Drawing.Size(201, 50);
            this.renderResolutionWidth.TabIndex = 3;
            this.renderResolutionWidth.Text = "800";
            // 
            // renderResolutionHeight
            // 
            this.renderResolutionHeight.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.renderResolutionHeight.Depth = 0;
            this.renderResolutionHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.renderResolutionHeight.Location = new System.Drawing.Point(297, 74);
            this.renderResolutionHeight.MaxLength = 50;
            this.renderResolutionHeight.MouseState = MaterialSkin.MouseState.OUT;
            this.renderResolutionHeight.Multiline = false;
            this.renderResolutionHeight.Name = "renderResolutionHeight";
            this.renderResolutionHeight.Size = new System.Drawing.Size(201, 50);
            this.renderResolutionHeight.TabIndex = 4;
            this.renderResolutionHeight.Text = "600";
            // 
            // materialCard1
            // 
            this.materialCard1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.materialCard1.Controls.Add(this.gpuSelectorControl);
            this.materialCard1.Controls.Add(this.materialLabel7);
            this.materialCard1.Controls.Add(this.rendererSettingsApply);
            this.materialCard1.Controls.Add(this.gridDividerControl);
            this.materialCard1.Controls.Add(this.materialLabel6);
            this.materialCard1.Controls.Add(this.bounceLimitControl);
            this.materialCard1.Controls.Add(this.materialLabel4);
            this.materialCard1.Controls.Add(this.useCudaCheckbox);
            this.materialCard1.Controls.Add(this.materialLabel1);
            this.materialCard1.Depth = 0;
            this.materialCard1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialCard1.Location = new System.Drawing.Point(13, 17);
            this.materialCard1.Margin = new System.Windows.Forms.Padding(14);
            this.materialCard1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialCard1.Name = "materialCard1";
            this.materialCard1.Padding = new System.Windows.Forms.Padding(14);
            this.materialCard1.Size = new System.Drawing.Size(297, 438);
            this.materialCard1.TabIndex = 1;
            // 
            // gpuSelectorControl
            // 
            this.gpuSelectorControl.AutoResize = false;
            this.gpuSelectorControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.gpuSelectorControl.Depth = 0;
            this.gpuSelectorControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.gpuSelectorControl.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.gpuSelectorControl.DropDownHeight = 174;
            this.gpuSelectorControl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.gpuSelectorControl.DropDownWidth = 121;
            this.gpuSelectorControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.gpuSelectorControl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.gpuSelectorControl.FormattingEnabled = true;
            this.gpuSelectorControl.IntegralHeight = false;
            this.gpuSelectorControl.ItemHeight = 43;
            this.gpuSelectorControl.Location = new System.Drawing.Point(14, 227);
            this.gpuSelectorControl.MaxDropDownItems = 4;
            this.gpuSelectorControl.MouseState = MaterialSkin.MouseState.OUT;
            this.gpuSelectorControl.Name = "gpuSelectorControl";
            this.gpuSelectorControl.Size = new System.Drawing.Size(269, 49);
            this.gpuSelectorControl.TabIndex = 9;
            // 
            // materialLabel7
            // 
            this.materialLabel7.AutoSize = true;
            this.materialLabel7.Depth = 0;
            this.materialLabel7.Dock = System.Windows.Forms.DockStyle.Top;
            this.materialLabel7.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel7.Location = new System.Drawing.Point(14, 208);
            this.materialLabel7.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel7.Name = "materialLabel7";
            this.materialLabel7.Size = new System.Drawing.Size(32, 19);
            this.materialLabel7.TabIndex = 10;
            this.materialLabel7.Text = "GPU";
            // 
            // rendererSettingsApply
            // 
            this.rendererSettingsApply.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.rendererSettingsApply.Depth = 0;
            this.rendererSettingsApply.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.rendererSettingsApply.DrawShadows = true;
            this.rendererSettingsApply.HighEmphasis = true;
            this.rendererSettingsApply.Icon = null;
            this.rendererSettingsApply.Location = new System.Drawing.Point(14, 388);
            this.rendererSettingsApply.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.rendererSettingsApply.MouseState = MaterialSkin.MouseState.HOVER;
            this.rendererSettingsApply.Name = "rendererSettingsApply";
            this.rendererSettingsApply.Size = new System.Drawing.Size(269, 36);
            this.rendererSettingsApply.TabIndex = 6;
            this.rendererSettingsApply.Text = "Apply";
            this.rendererSettingsApply.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.rendererSettingsApply.UseAccentColor = false;
            this.rendererSettingsApply.UseVisualStyleBackColor = true;
            this.rendererSettingsApply.Click += new System.EventHandler(this.rendererSettingsApply_Click);
            // 
            // gridDividerControl
            // 
            this.gridDividerControl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridDividerControl.Depth = 0;
            this.gridDividerControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.gridDividerControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.gridDividerControl.Location = new System.Drawing.Point(14, 158);
            this.gridDividerControl.MaxLength = 50;
            this.gridDividerControl.MouseState = MaterialSkin.MouseState.OUT;
            this.gridDividerControl.Multiline = false;
            this.gridDividerControl.Name = "gridDividerControl";
            this.gridDividerControl.Size = new System.Drawing.Size(269, 50);
            this.gridDividerControl.TabIndex = 8;
            this.gridDividerControl.Text = "16";
            // 
            // materialLabel6
            // 
            this.materialLabel6.AutoSize = true;
            this.materialLabel6.Depth = 0;
            this.materialLabel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.materialLabel6.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel6.Location = new System.Drawing.Point(14, 139);
            this.materialLabel6.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel6.Name = "materialLabel6";
            this.materialLabel6.Size = new System.Drawing.Size(81, 19);
            this.materialLabel6.TabIndex = 7;
            this.materialLabel6.Text = "Grid divider";
            // 
            // bounceLimitControl
            // 
            this.bounceLimitControl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.bounceLimitControl.Depth = 0;
            this.bounceLimitControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.bounceLimitControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.bounceLimitControl.Location = new System.Drawing.Point(14, 89);
            this.bounceLimitControl.MaxLength = 50;
            this.bounceLimitControl.MouseState = MaterialSkin.MouseState.OUT;
            this.bounceLimitControl.Multiline = false;
            this.bounceLimitControl.Name = "bounceLimitControl";
            this.bounceLimitControl.Size = new System.Drawing.Size(269, 50);
            this.bounceLimitControl.TabIndex = 6;
            this.bounceLimitControl.Text = "20";
            // 
            // materialLabel4
            // 
            this.materialLabel4.AutoSize = true;
            this.materialLabel4.Depth = 0;
            this.materialLabel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.materialLabel4.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel4.Location = new System.Drawing.Point(14, 70);
            this.materialLabel4.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel4.Name = "materialLabel4";
            this.materialLabel4.Size = new System.Drawing.Size(131, 19);
            this.materialLabel4.TabIndex = 6;
            this.materialLabel4.Text = "Max light bounces";
            // 
            // useCudaCheckbox
            // 
            this.useCudaCheckbox.AutoSize = true;
            this.useCudaCheckbox.Checked = true;
            this.useCudaCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useCudaCheckbox.Depth = 0;
            this.useCudaCheckbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.useCudaCheckbox.Location = new System.Drawing.Point(14, 33);
            this.useCudaCheckbox.Margin = new System.Windows.Forms.Padding(0);
            this.useCudaCheckbox.MouseLocation = new System.Drawing.Point(-1, -1);
            this.useCudaCheckbox.MouseState = MaterialSkin.MouseState.HOVER;
            this.useCudaCheckbox.Name = "useCudaCheckbox";
            this.useCudaCheckbox.Ripple = true;
            this.useCudaCheckbox.Size = new System.Drawing.Size(269, 37);
            this.useCudaCheckbox.TabIndex = 0;
            this.useCudaCheckbox.Text = "Use CUDA";
            this.useCudaCheckbox.UseVisualStyleBackColor = true;
            // 
            // materialLabel1
            // 
            this.materialLabel1.AutoSize = true;
            this.materialLabel1.Depth = 0;
            this.materialLabel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.materialLabel1.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel1.Location = new System.Drawing.Point(14, 14);
            this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel1.Name = "materialLabel1";
            this.materialLabel1.Size = new System.Drawing.Size(123, 19);
            this.materialLabel1.TabIndex = 2;
            this.materialLabel1.Text = "Renderer settings";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.materialCard3);
            this.tabPage3.Controls.Add(this.materialCard4);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1344, 814);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Object options";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // materialCard3
            // 
            this.materialCard3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.materialCard3.Controls.Add(this.objectListControl);
            this.materialCard3.Controls.Add(this.materialLabel8);
            this.materialCard3.Controls.Add(this.materialButton1);
            this.materialCard3.Depth = 0;
            this.materialCard3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialCard3.Location = new System.Drawing.Point(338, 14);
            this.materialCard3.Margin = new System.Windows.Forms.Padding(14);
            this.materialCard3.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialCard3.Name = "materialCard3";
            this.materialCard3.Padding = new System.Windows.Forms.Padding(14);
            this.materialCard3.Size = new System.Drawing.Size(310, 438);
            this.materialCard3.TabIndex = 10;
            // 
            // objectListControl
            // 
            this.objectListControl.AutoResize = false;
            this.objectListControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.objectListControl.Depth = 0;
            this.objectListControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.objectListControl.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.objectListControl.DropDownHeight = 174;
            this.objectListControl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.objectListControl.DropDownWidth = 121;
            this.objectListControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.objectListControl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.objectListControl.FormattingEnabled = true;
            this.objectListControl.IntegralHeight = false;
            this.objectListControl.ItemHeight = 43;
            this.objectListControl.Location = new System.Drawing.Point(14, 33);
            this.objectListControl.MaxDropDownItems = 4;
            this.objectListControl.MouseState = MaterialSkin.MouseState.OUT;
            this.objectListControl.Name = "objectListControl";
            this.objectListControl.Size = new System.Drawing.Size(282, 49);
            this.objectListControl.TabIndex = 10;
            // 
            // materialLabel8
            // 
            this.materialLabel8.AutoSize = true;
            this.materialLabel8.Depth = 0;
            this.materialLabel8.Dock = System.Windows.Forms.DockStyle.Top;
            this.materialLabel8.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel8.Location = new System.Drawing.Point(14, 14);
            this.materialLabel8.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel8.Name = "materialLabel8";
            this.materialLabel8.Size = new System.Drawing.Size(112, 19);
            this.materialLabel8.TabIndex = 8;
            this.materialLabel8.Text = "Objects on map";
            // 
            // materialButton1
            // 
            this.materialButton1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.materialButton1.Depth = 0;
            this.materialButton1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.materialButton1.DrawShadows = true;
            this.materialButton1.HighEmphasis = true;
            this.materialButton1.Icon = null;
            this.materialButton1.Location = new System.Drawing.Point(14, 388);
            this.materialButton1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.materialButton1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialButton1.Name = "materialButton1";
            this.materialButton1.Size = new System.Drawing.Size(282, 36);
            this.materialButton1.TabIndex = 9;
            this.materialButton1.Text = "Delete selected";
            this.materialButton1.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.materialButton1.UseAccentColor = true;
            this.materialButton1.UseVisualStyleBackColor = true;
            this.materialButton1.Click += new System.EventHandler(this.materialButton1_Click);
            // 
            // materialCard4
            // 
            this.materialCard4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.materialCard4.Controls.Add(this.verticiesListControl);
            this.materialCard4.Controls.Add(this.materialLabel10);
            this.materialCard4.Controls.Add(this.deleteVerticie);
            this.materialCard4.Depth = 0;
            this.materialCard4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialCard4.Location = new System.Drawing.Point(13, 14);
            this.materialCard4.Margin = new System.Windows.Forms.Padding(14);
            this.materialCard4.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialCard4.Name = "materialCard4";
            this.materialCard4.Padding = new System.Windows.Forms.Padding(14);
            this.materialCard4.Size = new System.Drawing.Size(310, 438);
            this.materialCard4.TabIndex = 7;
            // 
            // verticiesListControl
            // 
            this.verticiesListControl.AutoSizeTable = false;
            this.verticiesListControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.verticiesListControl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.verticiesListControl.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader});
            this.verticiesListControl.Depth = 0;
            this.verticiesListControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.verticiesListControl.FullRowSelect = true;
            this.verticiesListControl.HideSelection = false;
            this.verticiesListControl.Location = new System.Drawing.Point(14, 33);
            this.verticiesListControl.MinimumSize = new System.Drawing.Size(200, 100);
            this.verticiesListControl.MouseLocation = new System.Drawing.Point(-1, -1);
            this.verticiesListControl.MouseState = MaterialSkin.MouseState.OUT;
            this.verticiesListControl.Name = "verticiesListControl";
            this.verticiesListControl.OwnerDraw = true;
            this.verticiesListControl.Size = new System.Drawing.Size(282, 355);
            this.verticiesListControl.TabIndex = 7;
            this.verticiesListControl.UseCompatibleStateImageBehavior = false;
            this.verticiesListControl.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader
            // 
            this.columnHeader.Text = "Vertex list";
            this.columnHeader.Width = 300;
            // 
            // materialLabel10
            // 
            this.materialLabel10.AutoSize = true;
            this.materialLabel10.Depth = 0;
            this.materialLabel10.Dock = System.Windows.Forms.DockStyle.Top;
            this.materialLabel10.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel10.Location = new System.Drawing.Point(14, 14);
            this.materialLabel10.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel10.Name = "materialLabel10";
            this.materialLabel10.Size = new System.Drawing.Size(114, 19);
            this.materialLabel10.TabIndex = 8;
            this.materialLabel10.Text = "Current verticies";
            // 
            // deleteVerticie
            // 
            this.deleteVerticie.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.deleteVerticie.Depth = 0;
            this.deleteVerticie.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.deleteVerticie.DrawShadows = true;
            this.deleteVerticie.HighEmphasis = true;
            this.deleteVerticie.Icon = null;
            this.deleteVerticie.Location = new System.Drawing.Point(14, 388);
            this.deleteVerticie.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.deleteVerticie.MouseState = MaterialSkin.MouseState.HOVER;
            this.deleteVerticie.Name = "deleteVerticie";
            this.deleteVerticie.Size = new System.Drawing.Size(282, 36);
            this.deleteVerticie.TabIndex = 9;
            this.deleteVerticie.Text = "Delete selected";
            this.deleteVerticie.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.deleteVerticie.UseAccentColor = true;
            this.deleteVerticie.UseVisualStyleBackColor = true;
            this.deleteVerticie.Click += new System.EventHandler(this.deleteVerticie_Click);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.materialCard6);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(1344, 814);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Misc";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.materialCard5);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1344, 814);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Debug Settings";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // materialCard5
            // 
            this.materialCard5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.materialCard5.Controls.Add(this.objectsOverlineControl);
            this.materialCard5.Controls.Add(this.drawCellGridControl);
            this.materialCard5.Controls.Add(this.applyDebugSettingsButton);
            this.materialCard5.Depth = 0;
            this.materialCard5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialCard5.Location = new System.Drawing.Point(13, 14);
            this.materialCard5.Margin = new System.Windows.Forms.Padding(14);
            this.materialCard5.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialCard5.Name = "materialCard5";
            this.materialCard5.Padding = new System.Windows.Forms.Padding(14);
            this.materialCard5.Size = new System.Drawing.Size(297, 438);
            this.materialCard5.TabIndex = 3;
            // 
            // objectsOverlineControl
            // 
            this.objectsOverlineControl.AutoSize = true;
            this.objectsOverlineControl.Checked = true;
            this.objectsOverlineControl.CheckState = System.Windows.Forms.CheckState.Checked;
            this.objectsOverlineControl.Depth = 0;
            this.objectsOverlineControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.objectsOverlineControl.Location = new System.Drawing.Point(14, 51);
            this.objectsOverlineControl.Margin = new System.Windows.Forms.Padding(0);
            this.objectsOverlineControl.MouseLocation = new System.Drawing.Point(-1, -1);
            this.objectsOverlineControl.MouseState = MaterialSkin.MouseState.HOVER;
            this.objectsOverlineControl.Name = "objectsOverlineControl";
            this.objectsOverlineControl.Ripple = true;
            this.objectsOverlineControl.Size = new System.Drawing.Size(269, 37);
            this.objectsOverlineControl.TabIndex = 9;
            this.objectsOverlineControl.Text = "Draw objects overline";
            this.objectsOverlineControl.UseVisualStyleBackColor = true;
            // 
            // drawCellGridControl
            // 
            this.drawCellGridControl.AutoSize = true;
            this.drawCellGridControl.Depth = 0;
            this.drawCellGridControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.drawCellGridControl.Location = new System.Drawing.Point(14, 14);
            this.drawCellGridControl.Margin = new System.Windows.Forms.Padding(0);
            this.drawCellGridControl.MouseLocation = new System.Drawing.Point(-1, -1);
            this.drawCellGridControl.MouseState = MaterialSkin.MouseState.HOVER;
            this.drawCellGridControl.Name = "drawCellGridControl";
            this.drawCellGridControl.Ripple = true;
            this.drawCellGridControl.Size = new System.Drawing.Size(269, 37);
            this.drawCellGridControl.TabIndex = 8;
            this.drawCellGridControl.Text = "Draw cell grid";
            this.drawCellGridControl.UseVisualStyleBackColor = true;
            // 
            // applyDebugSettingsButton
            // 
            this.applyDebugSettingsButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.applyDebugSettingsButton.Depth = 0;
            this.applyDebugSettingsButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.applyDebugSettingsButton.DrawShadows = true;
            this.applyDebugSettingsButton.HighEmphasis = true;
            this.applyDebugSettingsButton.Icon = null;
            this.applyDebugSettingsButton.Location = new System.Drawing.Point(14, 388);
            this.applyDebugSettingsButton.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.applyDebugSettingsButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.applyDebugSettingsButton.Name = "applyDebugSettingsButton";
            this.applyDebugSettingsButton.Size = new System.Drawing.Size(269, 36);
            this.applyDebugSettingsButton.TabIndex = 6;
            this.applyDebugSettingsButton.Text = "Apply";
            this.applyDebugSettingsButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.applyDebugSettingsButton.UseAccentColor = false;
            this.applyDebugSettingsButton.UseVisualStyleBackColor = true;
            this.applyDebugSettingsButton.Click += new System.EventHandler(this.applyDebugSettingsButton_Click);
            // 
            // materialDrawer1
            // 
            this.materialDrawer1.AutoHide = false;
            this.materialDrawer1.BackgroundWithAccent = false;
            this.materialDrawer1.BaseTabControl = this.materialTabControl1;
            this.materialDrawer1.Depth = 0;
            this.materialDrawer1.Dock = System.Windows.Forms.DockStyle.Left;
            this.materialDrawer1.HighlightWithAccent = true;
            this.materialDrawer1.IndicatorWidth = 0;
            this.materialDrawer1.IsOpen = true;
            this.materialDrawer1.Location = new System.Drawing.Point(0, 0);
            this.materialDrawer1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialDrawer1.Name = "materialDrawer1";
            this.materialDrawer1.ShowIconsWhenHidden = false;
            this.materialDrawer1.Size = new System.Drawing.Size(250, 840);
            this.materialDrawer1.TabIndex = 2;
            this.materialDrawer1.Text = "materialDrawer1";
            this.materialDrawer1.UseColors = true;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // saveSceneButton
            // 
            this.saveSceneButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.saveSceneButton.Depth = 0;
            this.saveSceneButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.saveSceneButton.DrawShadows = true;
            this.saveSceneButton.HighEmphasis = true;
            this.saveSceneButton.Icon = null;
            this.saveSceneButton.Location = new System.Drawing.Point(14, 86);
            this.saveSceneButton.Margin = new System.Windows.Forms.Padding(4, 60, 4, 6);
            this.saveSceneButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.saveSceneButton.Name = "saveSceneButton";
            this.saveSceneButton.Size = new System.Drawing.Size(252, 36);
            this.saveSceneButton.TabIndex = 6;
            this.saveSceneButton.Text = "Save scene";
            this.saveSceneButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.saveSceneButton.UseAccentColor = false;
            this.saveSceneButton.UseVisualStyleBackColor = true;
            this.saveSceneButton.Click += new System.EventHandler(this.saveSceneButton_Click);
            // 
            // loadSceneButton
            // 
            this.loadSceneButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.loadSceneButton.Depth = 0;
            this.loadSceneButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.loadSceneButton.DrawShadows = true;
            this.loadSceneButton.HighEmphasis = true;
            this.loadSceneButton.Icon = null;
            this.loadSceneButton.Location = new System.Drawing.Point(14, 122);
            this.loadSceneButton.Margin = new System.Windows.Forms.Padding(4, 60, 4, 6);
            this.loadSceneButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.loadSceneButton.Name = "loadSceneButton";
            this.loadSceneButton.Size = new System.Drawing.Size(252, 36);
            this.loadSceneButton.TabIndex = 7;
            this.loadSceneButton.Text = "Load scene";
            this.loadSceneButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.loadSceneButton.UseAccentColor = false;
            this.loadSceneButton.UseVisualStyleBackColor = true;
            this.loadSceneButton.Click += new System.EventHandler(this.loadSceneButton_Click);
            // 
            // materialCard6
            // 
            this.materialCard6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.materialCard6.Controls.Add(this.minecraftWorldGeneration);
            this.materialCard6.Depth = 0;
            this.materialCard6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialCard6.Location = new System.Drawing.Point(13, 14);
            this.materialCard6.Margin = new System.Windows.Forms.Padding(14);
            this.materialCard6.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialCard6.Name = "materialCard6";
            this.materialCard6.Padding = new System.Windows.Forms.Padding(14);
            this.materialCard6.Size = new System.Drawing.Size(297, 438);
            this.materialCard6.TabIndex = 4;
            // 
            // minecraftWorldGeneration
            // 
            this.minecraftWorldGeneration.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.minecraftWorldGeneration.Depth = 0;
            this.minecraftWorldGeneration.Dock = System.Windows.Forms.DockStyle.Top;
            this.minecraftWorldGeneration.DrawShadows = true;
            this.minecraftWorldGeneration.HighEmphasis = true;
            this.minecraftWorldGeneration.Icon = null;
            this.minecraftWorldGeneration.Location = new System.Drawing.Point(14, 14);
            this.minecraftWorldGeneration.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.minecraftWorldGeneration.MouseState = MaterialSkin.MouseState.HOVER;
            this.minecraftWorldGeneration.Name = "minecraftWorldGeneration";
            this.minecraftWorldGeneration.Size = new System.Drawing.Size(269, 36);
            this.minecraftWorldGeneration.TabIndex = 6;
            this.minecraftWorldGeneration.Text = "Generate terraria world";
            this.minecraftWorldGeneration.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.minecraftWorldGeneration.UseAccentColor = false;
            this.minecraftWorldGeneration.UseVisualStyleBackColor = true;
            this.minecraftWorldGeneration.Click += new System.EventHandler(this.minecraftWorldGeneration_Click);
            // 
            // PTGIForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1600, 900);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.KeyPreview = true;
            this.MaximumSize = new System.Drawing.Size(2560, 1400);
            this.MinimumSize = new System.Drawing.Size(261, 61);
            this.Name = "PTGIForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PTGI Remastered";
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PTGIForm_KeyUp);
            this.panel1.ResumeLayout(false);
            this.materialTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.renderingButtonsPanel.ResumeLayout(false);
            this.renderingButtonsPanel.PerformLayout();
            this.renderReportBox.ResumeLayout(false);
            this.renderReportBox.PerformLayout();
            this.objectSpecificationCard.ResumeLayout(false);
            this.objectSpecificationCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.materialCard2.ResumeLayout(false);
            this.materialCard2.PerformLayout();
            this.materialCard1.ResumeLayout(false);
            this.materialCard1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.materialCard3.ResumeLayout(false);
            this.materialCard3.PerformLayout();
            this.materialCard4.ResumeLayout(false);
            this.materialCard4.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.materialCard5.ResumeLayout(false);
            this.materialCard5.PerformLayout();
            this.materialCard6.ResumeLayout(false);
            this.materialCard6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private MaterialSkin.Controls.MaterialTabControl materialTabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TabPage tabPage2;
        private MaterialSkin.Controls.MaterialDrawer materialDrawer1;
        private MaterialSkin.Controls.MaterialSwitch useCudaCheckbox;
        private MaterialSkin.Controls.MaterialCard materialCard2;
        private MaterialSkin.Controls.MaterialLabel materialLabel2;
        private MaterialSkin.Controls.MaterialCard materialCard1;
        private MaterialSkin.Controls.MaterialTextBox renderResolutionHeight;
        private MaterialSkin.Controls.MaterialTextBox renderResolutionWidth;
        private MaterialSkin.Controls.MaterialLabel materialLabel1;
        private MaterialSkin.Controls.MaterialLabel materialLabel3;
        private MaterialSkin.Controls.MaterialButton rendererSettingsApply;
        private MaterialSkin.Controls.MaterialButton startRenderButton;
        private MaterialSkin.Controls.MaterialLabel materialLabel4;
        private MaterialSkin.Controls.MaterialTextBox bounceLimitControl;
        private MaterialSkin.Controls.MaterialButton renderSettingsApply;
        private MaterialSkin.Controls.MaterialLabel materialLabel5;
        private MaterialSkin.Controls.MaterialTextBox samplePerPixelControl;
        private MaterialSkin.Controls.MaterialLabel materialLabel6;
        private MaterialSkin.Controls.MaterialTextBox gridDividerControl;
        private MaterialSkin.Controls.MaterialComboBox gpuSelectorControl;
        private MaterialSkin.Controls.MaterialLabel materialLabel7;
        private System.Windows.Forms.TabPage tabPage3;
        private MaterialSkin.Controls.MaterialCard renderReportBox;
        private MaterialSkin.Controls.MaterialLabel renderReportTime;
        private System.Windows.Forms.Timer timer1;
        private MaterialSkin.Controls.MaterialCard materialCard4;
        private MaterialSkin.Controls.MaterialListView verticiesListControl;
        private MaterialSkin.Controls.MaterialLabel materialLabel10;
        private MaterialSkin.Controls.MaterialButton deleteVerticie;
        private System.Windows.Forms.ColumnHeader columnHeader;
        private System.Windows.Forms.TabPage tabPage4;
        private MaterialSkin.Controls.MaterialCard objectSpecificationCard;
        private MaterialSkin.Controls.MaterialComboBox objectMaterialControl;
        private MaterialSkin.Controls.MaterialLabel materialLabel9;
        private MaterialSkin.Controls.MaterialSwitch emitsLightControl;
        private MaterialSkin.Controls.MaterialCard materialCard5;
        private MaterialSkin.Controls.MaterialSwitch drawCellGridControl;
        private MaterialSkin.Controls.MaterialButton applyDebugSettingsButton;
        private Cyotek.Windows.Forms.ColorEditor colorEditor1;
        private MaterialSkin.Controls.MaterialTextBox objectEmissionStrengthControl;
        private MaterialSkin.Controls.MaterialLabel materialLabel12;
        private MaterialSkin.Controls.MaterialTextBox objectDensityControl;
        private MaterialSkin.Controls.MaterialLabel materialLabel11;
        private MaterialSkin.Controls.MaterialTextBox objectNameControl;
        private MaterialSkin.Controls.MaterialLabel materialLabel13;
        private MaterialSkin.Controls.MaterialCard materialCard3;
        private MaterialSkin.Controls.MaterialLabel materialLabel8;
        private MaterialSkin.Controls.MaterialButton materialButton1;
        private MaterialSkin.Controls.MaterialComboBox objectListControl;
        private MaterialSkin.Controls.MaterialButton saveRenderButton;
        private MaterialSkin.Controls.MaterialCard renderingButtonsPanel;
        private System.Windows.Forms.TabPage tabPage5;
        private MaterialSkin.Controls.MaterialSwitch objectsOverlineControl;
        private MaterialSkin.Controls.MaterialButton loadSceneButton;
        private MaterialSkin.Controls.MaterialButton saveSceneButton;
        private MaterialSkin.Controls.MaterialCard materialCard6;
        private MaterialSkin.Controls.MaterialButton minecraftWorldGeneration;
    }
}

