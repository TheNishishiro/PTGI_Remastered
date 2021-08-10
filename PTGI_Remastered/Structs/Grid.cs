using ILGPU.Algorithms;
using PTGI_Remastered.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTGI_Remastered.Structs
{
    public struct GridVariables
    {
        public byte MoveAlongX;
        public byte MoveAlongY;
        public int StepX;
        public int StepY;
        public int CurrentTileIDX;
        public int CurrentTileIDY;
        public float CurrentDeltaX;
        public float CurrentDeltaY;
        public float DeltaX;
        public float DeltaY;

        public void MoveXDirection()
        {
            CurrentTileIDX += StepX;
            CurrentDeltaX += DeltaX;
        }

        public void MoveYDirection()
        {
            CurrentTileIDY += StepY;
            CurrentDeltaY += DeltaY;
        }

        public void MoveInProperDirection()
        {
            if (CurrentDeltaX < CurrentDeltaY)
                MoveXDirection();
            else
                MoveYDirection();
        }

        public void TraverseToNextCell()
        {
            if (MoveAlongX == 1 && MoveAlongY == 1)
                MoveInProperDirection();
            else if (MoveAlongX == 1)
                MoveXDirection();
            else
                MoveYDirection();
        }
    }

    public struct Grid
    {
        public int CellsInRow;

        public int CellWidth;
        public int CellHeight;
        public int GridSize;

        public Grid Create(Bitmap bitmap, int cellsInRow)
        {
            CellWidth = (int)Math.Ceiling((float)bitmap.Width / (float)cellsInRow);
            CellHeight = (int)Math.Ceiling((float)bitmap.Height / (float)cellsInRow);
            GridSize = cellsInRow * cellsInRow;
            CellsInRow = cellsInRow;
            return this;
        }

        public int[,,] CPU_FillGrid(Line[] walls)
        {
            var cells = new int[CellsInRow, CellsInRow, walls.Length];


            for(int wallIndex = 0; wallIndex < walls.Length; wallIndex++)
            {
                var cellsContainingWall = GetVisitedCells(walls[wallIndex].Source, walls[wallIndex].Destination);
                if(cellsContainingWall != null)
                {
                    foreach(Point markedCell in cellsContainingWall)
                    {
                        for(int i = 0; i < walls.Length; i++)
                        {
                            if (cells[(int)markedCell.X, (int)markedCell.Y, i] == wallIndex + 1)
                                break;

                            if (cells[(int)markedCell.X, (int)markedCell.Y, i] == 0)
                            {
                                cells[(int)markedCell.X, (int)markedCell.Y, i] = wallIndex + 1;
                                break;
                            }
                        }
                    }
                }
            }

            return cells;
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
            int startTileIDX = (int)XMath.Floor(StartLocation.X / CellWidth);
            int startTileIDY = (int)XMath.Floor(StartLocation.Y / CellHeight);

            gridVariables.CurrentTileIDX = startTileIDX;
            gridVariables.CurrentTileIDY = startTileIDY;

            // Determine step (direction) in which cells will be traversed
            gridVariables.StepX = EndLocation.X > StartLocation.X ? 1 : -1;
            if (EndLocation.X == StartLocation.X)
                gridVariables.StepX = 0;
            gridVariables.StepY = EndLocation.Y > StartLocation.Y ? 1 : -1;
            if (EndLocation.Y == StartLocation.Y)
                gridVariables.StepY = 0;

            gridVariables.MoveAlongX = 1;
            gridVariables.MoveAlongY = 1;

            // if dividing by 0 disable moving along that axis
            float dividerX = XMath.Abs(EndLocation.X - StartLocation.X);
            float dividerY = XMath.Abs(EndLocation.Y - StartLocation.Y);
            if (dividerX == 0)
                gridVariables.MoveAlongX = 0;
            if (dividerY == 0)
                gridVariables.MoveAlongY = 0;

            // Calculate how much to add each step to reach end of a ray
            gridVariables.DeltaX = CellWidth / dividerX;
            gridVariables.DeltaY = CellHeight / dividerY;

            // Calculate how far the ray is into cell
            float PositionInsideTile_X = PTGI_Math.Modulo(StartLocation.X, CellWidth) / CellWidth;
            float DistanceToBoundary_X = PTGI_Math.Modulo(XMath.Abs(gridVariables.StepX - PositionInsideTile_X), 1);
            float PositionInsideTile_Y = PTGI_Math.Modulo(StartLocation.Y, CellHeight) / CellHeight;
            float DistanceToBoundary_Y = PTGI_Math.Modulo(XMath.Abs(gridVariables.StepY - PositionInsideTile_Y), 1);

            // How far are we into the grid
            gridVariables.CurrentDeltaX = gridVariables.MoveAlongX == 1 ? gridVariables.DeltaX * DistanceToBoundary_X : 1;
            gridVariables.CurrentDeltaY = gridVariables.MoveAlongY == 1 ? gridVariables.DeltaY * DistanceToBoundary_Y : 1;

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

            float DeltaX = CellWidth / Math.Abs(EndLocation.X - StartLocation.X);
            float DeltaY = CellHeight / Math.Abs(EndLocation.Y - StartLocation.Y);
            if (float.IsInfinity(DeltaX))
                moveAlongX = false;
            if (float.IsInfinity(DeltaY))
                moveAlongY = false;

            float PositionInsideTile_X = (StartLocation.X % CellWidth) / CellWidth;
            float DistanceToBoundary_X = Math.Abs(StepX - PositionInsideTile_X) % 1;
            float PositionInsideTile_Y = (StartLocation.Y % CellHeight) / CellHeight;
            float DistanceToBoundary_Y = Math.Abs(StepY - PositionInsideTile_Y) % 1;

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
