using ILGPU.Runtime;
using MaterialSkin.Controls;
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
        protected SettingsView Settings { get; set; }
        protected AcceleratorId GpuId { get; set; }
        protected int popupTime { get; set; } = 0;
        protected List<PTGI_Remastered.Structs.Polygon> Polygons { get; set; }

        protected MaterialCard PopupMessage { get; set; }
        protected MaterialLabel PopupMessageText { get; set; }

        protected MaterialListView QueuedVerticiesList { get; set; }
        protected PictureBox RenderedPictureBox { get; set; }
        protected bool IsObjectEmittingLight { get; set; }

        protected string SelectedObjectMaterial { get; set; }
        protected Color ObjectColor { get; set; }
        protected float ObjectEmissionStrength { get; set; }
        protected float ObjectDensity { get; set; }
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

            if(Settings.DrawObjectsOverline)
            {
                int polygonId = 0;
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
                    polygonId++;
                }
            }

            if (SelectedPolygon != -1 && Polygons.Count > 0)
            {
                var polygon = Polygons[SelectedPolygon];
                foreach (var wall in polygon.Walls)
                {
                    e.Graphics.DrawLine(
                            new Pen(Color.Red, 1f),
                            (int)(wall.Source.X),
                            (int)(wall.Source.Y),
                            (int)(wall.Destination.X),
                            (int)(wall.Destination.Y));
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
                        (row * cellGrid.CellWidth),
                        (col * cellGrid.CellHeight),
                        cellGrid.CellWidth,
                        cellGrid.CellHeight);
                    
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
                float x = float.Parse(QueuedVerticiesList.Items[i].Text.Split(';')[0]);
                float y = float.Parse(QueuedVerticiesList.Items[i].Text.Split(';')[1]);

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
    }
}
