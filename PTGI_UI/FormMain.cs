using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            if (QueuedVerticiesList.Items.Count <= 0)
                return;


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

    }
}
