using ILGPU.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTGI_Remastered.Structs
{
    public struct GridVariables
    {
        public bool MoveAlongX;
        public bool MoveAlongY;
        public int StepX;
        public int StepY;
        public int CurrentTileIDX;
        public int CurrentTileIDY;
        public float CurrentDeltaX;
        public float CurrentDeltaY;
        public float DeltaX;
        public float DeltaY;

        private void MoveXDirection()
        {
            CurrentTileIDX += StepX;
            CurrentDeltaX += DeltaX;
        }

        private void MoveYDirection()
        {
            CurrentTileIDY += StepY;
            CurrentDeltaY += DeltaY;
        }

        private void MoveInProperDirection()
        {
            if (CurrentDeltaX < CurrentDeltaY)
                MoveXDirection();
            else
                MoveYDirection();
        }

        public void TraverseToNextCell()
        {
            if (MoveAlongX && MoveAlongY)
                MoveInProperDirection();
            else if (MoveAlongX)
                MoveXDirection();
            else
                MoveYDirection();
        }
    }

    public struct GridCellContainer
    {
        public int PolygonId;
        public int WallId;

        public void Setup(int PolygonId, int WallId)
        {
            this.PolygonId = PolygonId;
            this.WallId = WallId;
        }
    }

    public struct Grid
    {
        public int CellsInRow;

        public int CellWidth;
        public int CellHeight;
        public int GridSize;

        public GridCellContainer[][] Cells;

        public void Create(Bitmap bitmap, int cellsInRow)
        {
            CellWidth = (int)Math.Ceiling((float)bitmap.Width / (float)cellsInRow);
            CellHeight = (int)Math.Ceiling((float)bitmap.Height / (float)cellsInRow);
            GridSize = cellsInRow * cellsInRow;
            CellsInRow = cellsInRow;
        }

        public bool ContainsKeyPair(List<GridCellContainer> gridList, int PolygonId, int WallId)
        {
            foreach(var cell in gridList)
            {
                if (cell.PolygonId == PolygonId && cell.WallId == WallId)
                    return true;
            }
            return false;
        }

        private int Index2DToIndex1D(Point index2D)
        {
            return (int)(index2D.Y * CellsInRow + index2D.X);
        }

        public void CPU_FillGrid(Polygon[] polygons)
        {
            var gridList = new List<List<GridCellContainer>>();
            for (int i = 0; i < GridSize; i++)
                gridList.Add(new List<GridCellContainer>());

            for (int polygonIndex = 0; polygonIndex < polygons.Length; polygonIndex++)
            {
                for(int wallIndex = 0; wallIndex < polygons[polygonIndex].Walls.Length; wallIndex++)
                {
                    var cellsContainingWall = GetVisitedCells(polygons[polygonIndex].Walls[wallIndex].Source, polygons[polygonIndex].Walls[wallIndex].Destination);
                    if(cellsContainingWall != null)
                    {
                        foreach(Point markedCell in cellsContainingWall)
                        {
                            int cellId = Index2DToIndex1D(markedCell);
                            if (!ContainsKeyPair(gridList[cellId], polygonIndex, wallIndex))
                            {
                                GridCellContainer gridCellContainer = new GridCellContainer();
                                gridCellContainer.Setup(polygonIndex, wallIndex);
                                gridList[cellId].Add(gridCellContainer);
                            }
                        }
                    }
                }
            }

            Cells = gridList.Select(a => a.ToArray()).ToArray();
        }

        public GridCellContainer[] GetObjectsInCell(int cellId)
        {
            return Cells[cellId];
        }

        public bool CUDAIsOutsideGrid(GridVariables gridVariables)
        {
            return (gridVariables.CurrentTileIDY < 0 || gridVariables.CurrentTileIDY >= CellsInRow || gridVariables.CurrentTileIDX < 0 || gridVariables.CurrentTileIDX >= CellsInRow);
        }

        public int CUDAGetCellIndex(GridVariables gridVariables)
        {
            return gridVariables.CurrentTileIDY * CellsInRow + gridVariables.CurrentTileIDX;
        }

        public GridVariables CUDAGetGridTraversalVariables(Line line)
        {
            return CUDAGetGridTraversalVariables(line.Source, line.Destination);
        }

        public GridVariables CUDAGetGridTraversalVariables(Point StartLocation, Point EndLocation)
        {
            GridVariables gridVariables = new GridVariables();

            // Calculate starting cell index based on begining of a ray
            int startTileIDX = (int)Math.Floor(StartLocation.X / CellWidth);
            int startTileIDY = (int)Math.Floor(StartLocation.Y / CellHeight);

            gridVariables.CurrentTileIDX = startTileIDX;
            gridVariables.CurrentTileIDY = startTileIDY;

            // Determine step (direction) in which cells will be traversed
            gridVariables.StepX = EndLocation.X > StartLocation.X ? 1 : -1;
            if (EndLocation.X == StartLocation.X)
                gridVariables.StepX = 0;
            gridVariables.StepY = EndLocation.Y > StartLocation.Y ? 1 : -1;
            if (EndLocation.Y == StartLocation.Y)
                gridVariables.StepY = 0;

            gridVariables.MoveAlongX = true;
            gridVariables.MoveAlongY = true;

            // if dividing by 0 disable moving along that axis
            float dividerX = 1;//XMath.Abs(EndLocation.X - StartLocation.X);
            float dividerY = 1;//XMath.Abs(EndLocation.Y - StartLocation.Y);
            if (dividerX == 0)
                gridVariables.MoveAlongX = false;
            if (dividerY == 0)
                gridVariables.MoveAlongY = false;

            // Calculate how much to add each step to reach end of a ray
            gridVariables.DeltaX = CellWidth / dividerX;
            gridVariables.DeltaY = CellHeight / dividerY;

            // Calculate how far the ray is into cell
            float PositionInsideTile_X = (StartLocation.X % CellWidth) / CellWidth;
            float DistanceToBoundary_X = 1;//XMath.Abs(gridVariables.StepX - PositionInsideTile_X) % 1;
            float PositionInsideTile_Y = (StartLocation.Y % CellHeight) / CellHeight;
            float DistanceToBoundary_Y = 1;//XMath.Abs(gridVariables.StepY - PositionInsideTile_Y) % 1;

            // How far are we into the grid
            gridVariables.CurrentDeltaX = gridVariables.MoveAlongX == true ? gridVariables.DeltaX * DistanceToBoundary_X : 1;
            gridVariables.CurrentDeltaY = gridVariables.MoveAlongY == true ? gridVariables.DeltaY * DistanceToBoundary_Y : 1;

            // Correct delta for cases when ray starts on the edge of a cell and is assigned to a different one than algorith assumes
            if (gridVariables.CurrentDeltaX == 0 && gridVariables.StepX == 1)
                gridVariables.CurrentDeltaX += gridVariables.DeltaX;
            if (gridVariables.CurrentDeltaY == 0 && gridVariables.StepY == 1)
                gridVariables.CurrentDeltaY += gridVariables.DeltaY;

            return gridVariables;
        }

        private List<Point> GetVisitedCells(Point StartLocation, Point EndLocation)
        {
            List<Point> visited = new List<Point>();

            int startTileIDX = (int)XMath.Floor(StartLocation.X / CellWidth);
            int startTileIDY = (int)XMath.Floor(StartLocation.Y / CellHeight);

            int endTileIDX = (int)XMath.Floor(EndLocation.X / CellWidth);
            int endTileIDY = (int)XMath.Floor(EndLocation.Y / CellHeight);

            int currentTileIDX = startTileIDX;
            int currentTileIDY = startTileIDY;

            Point firstPoint = new Point();
            firstPoint.SetCoords(currentTileIDX, currentTileIDY);
            visited.Add(firstPoint);

            if (startTileIDX >= CellsInRow || startTileIDY >= CellsInRow || endTileIDX >= CellsInRow || endTileIDY >= CellsInRow)
                return null;

            if (startTileIDX == endTileIDX && startTileIDY == endTileIDY)
                return visited;

            int StepX = EndLocation.X > StartLocation.X ? 1 : -1;
            if (EndLocation.X == StartLocation.X)
                StepX = 0;
            int StepY = EndLocation.Y > StartLocation.Y ? 1 : -1;
            if (EndLocation.Y == StartLocation.Y)
                StepY = 0;

            bool moveAlongX = true;
            bool moveAlongY = true;

            float DeltaX = CellWidth / XMath.Abs(EndLocation.X - StartLocation.X);
            float DeltaY = CellHeight / XMath.Abs(EndLocation.Y - StartLocation.Y);
            if (float.IsInfinity(DeltaX))
                moveAlongX = false;
            if (float.IsInfinity(DeltaY))
                moveAlongY = false;

            float PositionInsideTile_X = (StartLocation.X % CellWidth) / CellWidth;
            float DistanceToBoundary_X = XMath.Abs(StepX - PositionInsideTile_X) % 1;
            float PositionInsideTile_Y = (StartLocation.Y % CellHeight) / CellHeight;
            float DistanceToBoundary_Y = XMath.Abs(StepY - PositionInsideTile_Y) % 1;

            float CurrentDeltaX = moveAlongX ? DeltaX * DistanceToBoundary_X : 1;
            float CurrentDeltaY = moveAlongY ? DeltaY * DistanceToBoundary_Y : 1;


            while ((CurrentDeltaX < 1) || (CurrentDeltaY < 1))
            {
                if (moveAlongX && moveAlongY)
                {
                    if (CurrentDeltaX < CurrentDeltaY)
                    {
                        currentTileIDX += StepX;
                        CurrentDeltaX += DeltaX;
                    }
                    else
                    {
                        currentTileIDY += StepY;
                        CurrentDeltaY += DeltaY;
                    }
                }
                else if (moveAlongX)
                {
                    currentTileIDX += StepX;
                    CurrentDeltaX += DeltaX;
                }
                else
                {
                    currentTileIDY += StepY;
                    CurrentDeltaY += DeltaY;
                }
                if (currentTileIDY < 0 || currentTileIDY >= CellsInRow || currentTileIDX < 0 || currentTileIDX >= CellsInRow)
                    break;

                Point nextPoint = new Point();
                nextPoint.SetCoords(currentTileIDX, currentTileIDY);
                visited.Add(nextPoint);
            }

            return visited;
        }
    }
}
