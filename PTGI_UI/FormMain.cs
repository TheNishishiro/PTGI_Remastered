﻿using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace PTGI_UI
{
    public class UIPolygon
    {
        public string Name { get; set; }
    }

    public class FormMain : MaterialForm
    {
        protected PTGI_Remastered.PTGI PathTracer { get; set; }
        protected bool UseCUDA { get; set; }
        protected int RenderWidth { get; set; }
        protected int RenderHeight { get; set; }
        protected int SamplesPerPixel { get; set; }
        protected int BounceLimit { get; set; }
        protected int GridDivider { get; set; }
        protected int GpuId { get; set; }
        protected int popupTime { get; set; } = 0;
        protected List<PTGI_Remastered.Structs.Polygon> Polygons { get; set; }

        protected List<PTGI_Remastered.Structs.Point> DebugRayPoints { get; set; }
        protected List<int> DebugRayVisitedCells { get; set; }
        protected bool DrawGrid { get; set; }
        protected bool DrawObjectsOverline { get; set; }

        protected MaterialCard PopupMessage { get; set; }
        protected MaterialLabel PopupMessageText { get; set; }

        protected MaterialComboBox WorldObjectList { get; set; }

        protected MaterialListView QueuedVerticiesList { get; set; }
        protected PictureBox RenderedPictureBox { get; set; }
        protected bool IsObjectEmittingLight { get; set; }

        protected string SelectedObjectMaterial { get; set; }
        protected Color ObjectColor { get; set; }
        protected double ObjectEmissionStrength { get; set; }
        protected double ObjectDensity { get; set; }
        protected string ObjectName { get; set; }

        protected int SelectedPolygon { get; set; }

        protected void ShowPopupMessage(string text, int popupTime)
        {
            PopupMessage.Visible = true;
            PopupMessageText.Visible = true;
            PopupMessageText.Text = text;
            this.popupTime = popupTime;
        }

        protected void HidePopupMessage()
        {
            PopupMessage.Visible = false;
            PopupMessageText.Visible = false;
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
                e.Graphics.DrawEllipse(
                    new Pen(Color.Red, 2f),
                    int.Parse(QueuedVerticiesList.Items[i].Text.Split(';')[0]),
                    int.Parse(QueuedVerticiesList.Items[i].Text.Split(';')[1]),
                    2,
                    2);
            }

            PTGI_Remastered.Structs.Bitmap bitmap = new PTGI_Remastered.Structs.Bitmap();
            bitmap.Create(RenderWidth, RenderHeight);

            Random rnd = new Random();
            PTGI_Remastered.Structs.Grid cellGrid = new PTGI_Remastered.Structs.Grid();
            cellGrid.Create(bitmap, GridDivider);

            if (DebugRayPoints != null)
            {
                for (int i = 0; i < DebugRayPoints.Count - 1; i++)
                {
                    e.Graphics.DrawLine(
                        new Pen(Color.Green, 1f),
                        (int)(DebugRayPoints[i].X),
                        (int)(DebugRayPoints[i].Y),
                        (int)(DebugRayPoints[i + 1].X),
                        (int)(DebugRayPoints[i + 1].Y));
                }
            }

            if (DrawGrid)
            {
                for(int i = 0; i < cellGrid.GridSize; i++)
                {
                    int row = (int)Math.Floor(i % (float)GridDivider);
                    int col = (int)Math.Floor(i / (float)GridDivider);

                    if (DebugRayVisitedCells != null && DebugRayVisitedCells.Contains(i))
                    {
                        e.Graphics.DrawRectangle(
                             new Pen(Color.Yellow, 1f),
                             (row * cellGrid.CellWidth) + 5,
                             (col * cellGrid.CellHeight) + 5,
                             cellGrid.CellWidth-5,
                             cellGrid.CellHeight-5);
                    }
                    else
                    {
                        e.Graphics.DrawRectangle(
                            new Pen(Color.Blue, 1f),
                            (row * cellGrid.CellWidth),
                            (col * cellGrid.CellHeight),
                            cellGrid.CellWidth,
                            cellGrid.CellHeight);
                    }
                }
            }    
            
            if(DrawObjectsOverline)
            {
                foreach(var polygon in Polygons)
                {
                    for(int i = 0; i < polygon.Walls.Length; i++)
                    {
                        e.Graphics.DrawLine(
                            new Pen(Color.Green, 1f),
                            (int)(polygon.Walls[i].Source.X),
                            (int)(polygon.Walls[i].Source.Y),
                            (int)(polygon.Walls[i].Destination.X),
                            (int)(polygon.Walls[i].Destination.Y));
                    }
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

                            UpdateObjectList();
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

        protected void SendDebugRay()
        {
            if (QueuedVerticiesList.Items.Count < 2)
                return;

            double x1 = double.Parse(QueuedVerticiesList.Items[0].Text.Split(';')[0]);
            double y1 = double.Parse(QueuedVerticiesList.Items[0].Text.Split(';')[1]);
            double x2 = double.Parse(QueuedVerticiesList.Items[1].Text.Split(';')[0]);
            double y2 = double.Parse(QueuedVerticiesList.Items[1].Text.Split(';')[1]);

            PTGI_Remastered.Structs.Point source = new PTGI_Remastered.Structs.Point();
            source.SetCoords(x1, y1);
            PTGI_Remastered.Structs.Point destination = new PTGI_Remastered.Structs.Point();
            destination.SetCoords(x2, y2);

            var traceResult = PathTracer.DebugRay(Polygons.ToArray(), source, destination, RenderWidth, RenderHeight, BounceLimit, GridDivider);
            DebugRayPoints = traceResult.rayTrace.Where(c => c.HasValue).ToList();
            DebugRayVisitedCells = traceResult.rayGridMovement.Where(c => c != int.MaxValue).ToList();


            QueuedVerticiesList.Items.Clear();
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
                double x = double.Parse(QueuedVerticiesList.Items[i].Text.Split(';')[0]);
                double y = double.Parse(QueuedVerticiesList.Items[i].Text.Split(';')[1]);

                PTGI_Remastered.Structs.Point point = new PTGI_Remastered.Structs.Point();
                point.SetCoords(x, y);

                Vertices.Add(point);
            }
            polygon.Name = ObjectName.ToArray();
            polygon.Setup(Vertices.ToArray(),
                IsObjectEmittingLight ? PTGI_Remastered.Utilities.PTGI_ObjectTypes.LightSource : PTGI_Remastered.Utilities.PTGI_ObjectTypes.Solid,
                (PTGI_Remastered.Utilities.PTGI_MaterialReflectivness)Enum.Parse(typeof(PTGI_Remastered.Utilities.PTGI_MaterialReflectivness), SelectedObjectMaterial), 
                color,
                ObjectEmissionStrength, 
                ObjectDensity);

            Polygons.Add(polygon);
            QueuedVerticiesList.Items.Clear();


            UpdateObjectList();
        }

        protected void UpdateObjectList()
        {
            WorldObjectList.Items.Clear();
            var polygons = Polygons.Select(c => new UIPolygon() { Name = new string(c.Name) }).ToArray();
            WorldObjectList.Items.AddRange(polygons);
        }

        protected void SelectObject(Point mouseLocation)
        {
            if (WorldObjectList.Items.Count <= 0)
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

            WorldObjectList.SelectedIndex = SelectedPolygon;
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
            if (WorldObjectList.Items.Count <= 0 && SelectedPolygon < 0)
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
            UpdateObjectList();
        }

        protected void TerrariaWorldGenerator()
        {
            Random rnd = new Random();
            int maxY = RenderHeight / 2;
            for (int x = 0; x <= RenderWidth; x += 32)
            {
                for(int y = maxY; y <= RenderHeight; y+=32)
                {
                    PTGI_Remastered.Structs.Point topRight = new PTGI_Remastered.Structs.Point();
                    topRight.SetCoords(x, y);
                    PTGI_Remastered.Structs.Point rightBottom = new PTGI_Remastered.Structs.Point();
                    rightBottom.SetCoords(x + 32, y + 32);

                    PTGI_Remastered.Structs.Polygon block = new PTGI_Remastered.Structs.Polygon();
                    PTGI_Remastered.Structs.Color color = new PTGI_Remastered.Structs.Color();
                    color.SetColor(255, 255, 255);
                    block.Setup(new PTGI_Remastered.Structs.Point[] { topRight, rightBottom }, PTGI_Remastered.Utilities.PTGI_ObjectTypes.Solid, PTGI_Remastered.Utilities.PTGI_MaterialReflectivness.Rough, color, 1, 1);

                    Polygons.Add(block);
                    if (Polygons.Count == 999)
                    {
                        UpdateObjectList();
                        return;
                    }
                }
                int yDirection = rnd.Next(2) == 0 ? -32 : 32;
                maxY += yDirection;
            }

            UpdateObjectList();
        }
    }
}
