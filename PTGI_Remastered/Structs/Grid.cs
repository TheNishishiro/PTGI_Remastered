using ILGPU;
using ILGPU.Algorithms;
using PTGI_Remastered.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILGPU.Runtime;

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

    public struct GridTraversalResult
    {
        public Point IntersectionPoint;
        public Line IntesectedWall;
        public bool IsClosestIntersectionLight;

        public bool IsIntersectionPointFound() => IntersectionPoint.HasValue == 1;
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

        private List<Point> GetVisitedCells(Point StartLocation, Point EndLocation)
        {
            var visited = new List<Point>();

            var startTileIDX = (int)XMath.Floor(StartLocation.X / CellWidth);
            var startTileIDY = (int)XMath.Floor(StartLocation.Y / CellHeight);

            var endTileIDX = (int)XMath.Floor(EndLocation.X / CellWidth);
            var endTileIDY = (int)XMath.Floor(EndLocation.Y / CellHeight);

            var currentTileIDX = startTileIDX;
            var currentTileIDY = startTileIDY;

            Point firstPoint = new Point();
            firstPoint.SetCoords(currentTileIDX, currentTileIDY);
            visited.Add(firstPoint);

            if (startTileIDX >= CellsInRow || startTileIDY >= CellsInRow || endTileIDX >= CellsInRow || endTileIDY >= CellsInRow)
                return null;

            if (startTileIDX == endTileIDX && startTileIDY == endTileIDY)
                return visited;

            var StepX = EndLocation.X > StartLocation.X ? 1 : -1;
            if (EndLocation.X == StartLocation.X)
                StepX = 0;
            var StepY = EndLocation.Y > StartLocation.Y ? 1 : -1;
            if (EndLocation.Y == StartLocation.Y)
                StepY = 0;

            var moveAlongX = true;
            var moveAlongY = true;

            var DeltaX = CellWidth / Math.Abs(EndLocation.X - StartLocation.X);
            var DeltaY = CellHeight / Math.Abs(EndLocation.Y - StartLocation.Y);
            if (float.IsInfinity(DeltaX))
                moveAlongX = false;
            if (float.IsInfinity(DeltaY))
                moveAlongY = false;

            var PositionInsideTile_X = (StartLocation.X % CellWidth) / CellWidth;
            var DistanceToBoundary_X = Math.Abs(StepX - PositionInsideTile_X) % 1;
            var PositionInsideTile_Y = (StartLocation.Y % CellHeight) / CellHeight;
            var DistanceToBoundary_Y = Math.Abs(StepY - PositionInsideTile_Y) % 1;

            var CurrentDeltaX = moveAlongX ? DeltaX * DistanceToBoundary_X : 1;
            var CurrentDeltaY = moveAlongY ? DeltaY * DistanceToBoundary_Y : 1;


            while ((CurrentDeltaX < 1) || (CurrentDeltaY < 1))
            {
                switch (moveAlongX)
                {
                    case true when moveAlongY:
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

                        break;
                    }
                    case true:
                        currentTileIDX += StepX;
                        CurrentDeltaX += DeltaX;
                        break;
                    default:
                        currentTileIDY += StepY;
                        CurrentDeltaY += DeltaY;
                        break;
                }
                if (currentTileIDY < 0 || currentTileIDY >= CellsInRow || currentTileIDX < 0 || currentTileIDX >= CellsInRow)
                    break;

                var nextPoint = new Point();
                nextPoint.SetCoords(currentTileIDX, currentTileIDY);
                visited.Add(nextPoint);
            }

            return visited;
        }

        public bool IsOutsideGrid(GridVariables gridVariables)
        {
            return (gridVariables.CurrentTileIDY < 0 || gridVariables.CurrentTileIDY >= CellsInRow || gridVariables.CurrentTileIDX < 0 || gridVariables.CurrentTileIDX >= CellsInRow);
        }
        public int GetCellIndex(GridVariables gridVariables)
        {
            return gridVariables.CurrentTileIDY * CellsInRow + gridVariables.CurrentTileIDX;
        }
        public GridVariables GetGridTraversalVariables(Line line)
        {
            return GetGridTraversalVariables(line.Source, line.Destination);
        }
        public GridVariables GetGridTraversalVariables(Point StartLocation, Point EndLocation)
        {
            var gridVariables = new GridVariables();

            // Calculate starting cell index based on begining of a ray
            var startTileIDX = (int)XMath.Floor(StartLocation.X / CellWidth);
            var startTileIDY = (int)XMath.Floor(StartLocation.Y / CellHeight);

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
            var dividerX = XMath.Abs(EndLocation.X - StartLocation.X);
            var dividerY = XMath.Abs(EndLocation.Y - StartLocation.Y);
            if (dividerX == 0)
                gridVariables.MoveAlongX = 0;
            if (dividerY == 0)
                gridVariables.MoveAlongY = 0;

            // Calculate how much to add each step to reach end of a ray
            gridVariables.DeltaX = CellWidth / dividerX;
            gridVariables.DeltaY = CellHeight / dividerY;

            // Calculate how far the ray is into cell
            var PositionInsideTile_X = PTGI_Math.Modulo(StartLocation.X, CellWidth) / CellWidth;
            var DistanceToBoundary_X = PTGI_Math.Modulo(XMath.Abs(gridVariables.StepX - PositionInsideTile_X), 1);
            var PositionInsideTile_Y = PTGI_Math.Modulo(StartLocation.Y, CellHeight) / CellHeight;
            var DistanceToBoundary_Y = PTGI_Math.Modulo(XMath.Abs(gridVariables.StepY - PositionInsideTile_Y), 1);

            // How far are we into the grid
            gridVariables.CurrentDeltaX = gridVariables.MoveAlongX == 1 ? gridVariables.DeltaX * DistanceToBoundary_X : 1;
            gridVariables.CurrentDeltaY = gridVariables.MoveAlongY == 1 ? gridVariables.DeltaY * DistanceToBoundary_Y : 1;

            // Correct delta for cases when ray starts on the edge of a cell and is assigned to a different one than algorithm assumes
            if (gridVariables.CurrentDeltaX == 0 && gridVariables.StepX == 1)
                gridVariables.CurrentDeltaX += gridVariables.DeltaX;
            if (gridVariables.CurrentDeltaY == 0 && gridVariables.StepY == 1)
                gridVariables.CurrentDeltaY += gridVariables.DeltaY;

            return gridVariables;
        }
        public GridTraversalResult TraverseGrid(Line lightRay, int wallsCount, ArrayView3D<int, Stride3D.DenseXY> gridData, ArrayView1D<Line, Stride1D.Dense> walls, Line ignoredWall)
        {
            var closestDistance = float.MaxValue;
            var gridVariables = GetGridTraversalVariables(lightRay);
            var gridTraversalResult = new GridTraversalResult();
            while (true)
            {
                if (IsOutsideGrid(gridVariables))
                    break;

                for (var cellObjectId = 0; cellObjectId < wallsCount; cellObjectId++)
                {
                    int collisionObjectIds = gridData[gridVariables.CurrentTileIDX, gridVariables.CurrentTileIDY, cellObjectId];
                    if (collisionObjectIds == 0)
                        break;

                    collisionObjectIds--;
                    var rayWallIntersection = walls[collisionObjectIds].GetIntersection(lightRay, ignoredWall);
                    if (rayWallIntersection.HasValue != 1) continue;
                    
                    var raySourceToWallIntersectionDistance = lightRay.Source.GetDistance(rayWallIntersection);
                    if (!(raySourceToWallIntersectionDistance < closestDistance)) continue;
                    
                    gridTraversalResult.IntersectionPoint = rayWallIntersection;
                    closestDistance = raySourceToWallIntersectionDistance;
                    gridTraversalResult.IsClosestIntersectionLight = walls[collisionObjectIds].ObjectType == 2;
                    gridTraversalResult.IntesectedWall = walls[collisionObjectIds];
                    gridTraversalResult.IntesectedWall.HasValue = 1;
                }

                if (gridTraversalResult.IsIntersectionPointFound())
                {
                    var intersectionTileIdX = (int)XMath.Floor(gridTraversalResult.IntersectionPoint.X / CellWidth);
                    var intersectionTileIdY = (int)XMath.Floor(gridTraversalResult.IntersectionPoint.Y / CellHeight);
                    if (intersectionTileIdX == gridVariables.CurrentTileIDX && intersectionTileIdY == gridVariables.CurrentTileIDY)
                        break;
                }

                gridVariables.TraverseToNextCell();
            }

            return gridTraversalResult;
        }
    }
}
