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

        public int[] CPU_FillGrid(Line[] walls)
        {
            var cells = new int[CellsInRow * CellsInRow * walls.Length];


            for(var wallIndex = 0; wallIndex < walls.Length; wallIndex++)
            {
                var cellsContainingWall = GetVisitedCells(walls[wallIndex].Source, walls[wallIndex].Destination);
                if (!cellsContainingWall.Any()) continue;
                
                foreach(var markedCell in cellsContainingWall)
                {
                    for(var i = 0; i < walls.Length; i++)
                    {
                        var index = PTGI_Math.Convert3dIndexTo1d((int) markedCell.X, (int) markedCell.Y, i, CellsInRow, CellsInRow);
                        if (cells[index] == wallIndex + 1)
                            break;

                        if (cells[index] != 0) continue;
                            
                        cells[index] = wallIndex + 1;
                        break;
                    }
                }
            }

            return cells;
        }

        private List<Point> GetVisitedCells(Point StartLocation, Point EndLocation)
        {
            var visited = new List<Point>();

            var startTileIdx = (int)XMath.Floor(StartLocation.X / CellWidth);
            var startTileIdy = (int)XMath.Floor(StartLocation.Y / CellHeight);

            var endTileIdx = (int)XMath.Floor(EndLocation.X / CellWidth);
            var endTileIdy = (int)XMath.Floor(EndLocation.Y / CellHeight);

            var currentTileIdx = startTileIdx;
            var currentTileIdy = startTileIdy;

            var firstPoint = new Point();
            firstPoint.SetCoords(currentTileIdx, currentTileIdy);
            visited.Add(firstPoint);

            if (startTileIdx >= CellsInRow || startTileIdy >= CellsInRow || endTileIdx >= CellsInRow || endTileIdy >= CellsInRow)
                return new List<Point>();

            if (startTileIdx == endTileIdx && startTileIdy == endTileIdy)
                return visited;

            var stepX = EndLocation.X > StartLocation.X ? 1 : -1;
            if (EndLocation.X == StartLocation.X)
                stepX = 0;
            var stepY = EndLocation.Y > StartLocation.Y ? 1 : -1;
            if (EndLocation.Y == StartLocation.Y)
                stepY = 0;

            var moveAlongX = true;
            var moveAlongY = true;

            var deltaX = CellWidth / Math.Abs(EndLocation.X - StartLocation.X);
            var deltaY = CellHeight / Math.Abs(EndLocation.Y - StartLocation.Y);
            if (float.IsInfinity(deltaX))
                moveAlongX = false;
            if (float.IsInfinity(deltaY))
                moveAlongY = false;

            var positionInsideTileX = (StartLocation.X % CellWidth) / CellWidth;
            var distanceToBoundaryX = Math.Abs(stepX - positionInsideTileX) % 1;
            var positionInsideTileY = (StartLocation.Y % CellHeight) / CellHeight;
            var distanceToBoundaryY = Math.Abs(stepY - positionInsideTileY) % 1;

            var currentDeltaX = moveAlongX ? deltaX * distanceToBoundaryX : 1;
            var currentDeltaY = moveAlongY ? deltaY * distanceToBoundaryY : 1;


            while ((currentDeltaX < 1) || (currentDeltaY < 1))
            {
                switch (moveAlongX)
                {
                    case true when moveAlongY:
                    {
                        if (currentDeltaX < currentDeltaY)
                        {
                            currentTileIdx += stepX;
                            currentDeltaX += deltaX;
                        }
                        else
                        {
                            currentTileIdy += stepY;
                            currentDeltaY += deltaY;
                        }

                        break;
                    }
                    case true:
                        currentTileIdx += stepX;
                        currentDeltaX += deltaX;
                        break;
                    default:
                        currentTileIdy += stepY;
                        currentDeltaY += deltaY;
                        break;
                }
                if (currentTileIdy < 0 || currentTileIdy >= CellsInRow || currentTileIdx < 0 || currentTileIdx >= CellsInRow)
                    break;

                var nextPoint = new Point();
                nextPoint.SetCoords(currentTileIdx, currentTileIdy);
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

            // Calculate starting cell index based on beginning of a ray
            var startTileIdx = (int)(StartLocation.X / CellWidth);
            var startTileIdy = (int)(StartLocation.Y / CellHeight);

            gridVariables.CurrentTileIDX = startTileIdx;
            gridVariables.CurrentTileIDY = startTileIdy;

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
            var positionInsideTileX = (StartLocation.X % CellWidth) / CellWidth;
            var distanceToBoundaryX = (XMath.Abs(gridVariables.StepX - positionInsideTileX) % 1);
            var positionInsideTileY = (StartLocation.Y % CellHeight) / CellHeight;
            var distanceToBoundaryY = (XMath.Abs(gridVariables.StepY - positionInsideTileY) % 1);

            // How far are we into the grid
            gridVariables.CurrentDeltaX = gridVariables.MoveAlongX == 1 ? gridVariables.DeltaX * distanceToBoundaryX : 1;
            gridVariables.CurrentDeltaY = gridVariables.MoveAlongY == 1 ? gridVariables.DeltaY * distanceToBoundaryY : 1;

            // Correct delta for cases when ray starts on the edge of a cell and is assigned to a different one than algorithm assumes
            if (gridVariables.CurrentDeltaX == 0 && gridVariables.StepX == 1)
                gridVariables.CurrentDeltaX += gridVariables.DeltaX;
            if (gridVariables.CurrentDeltaY == 0 && gridVariables.StepY == 1)
                gridVariables.CurrentDeltaY += gridVariables.DeltaY;

            return gridVariables;
        }
        public GridTraversalResult TraverseGrid(Line lightRay, int wallsCount, ArrayView1D<int, Stride1D.Dense> gridData, ArrayView1D<Line, Stride1D.Dense> walls, Line ignoredWall)
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
                    var index3d = PTGI_Math.Convert3dIndexTo1d(gridVariables.CurrentTileIDX,
                        gridVariables.CurrentTileIDY, cellObjectId, CellsInRow, CellsInRow);
                    var collisionObjectIds = gridData[index3d];
                    if (collisionObjectIds == 0)
                        break;

                    collisionObjectIds--;
                    var currentWall = walls[collisionObjectIds];
                    var rayWallIntersection = currentWall.GetIntersection(lightRay, ignoredWall);
                    if (rayWallIntersection.HasValue != 1) continue;
                    
                    var raySourceToWallIntersectionDistance = lightRay.Source.GetDistance(rayWallIntersection);
                    if (raySourceToWallIntersectionDistance >= closestDistance) continue;
                    
                    gridTraversalResult.IntersectionPoint = rayWallIntersection;
                    closestDistance = raySourceToWallIntersectionDistance;
                    gridTraversalResult.IsClosestIntersectionLight = currentWall.ObjectType == 2;
                    gridTraversalResult.IntesectedWall = currentWall;
                    gridTraversalResult.IntesectedWall.HasValue = 1;
                }

                if (gridTraversalResult.IsIntersectionPointFound())
                {
                    var intersectionTileIdX = (int)(gridTraversalResult.IntersectionPoint.X / CellWidth);
                    var intersectionTileIdY = (int)(gridTraversalResult.IntersectionPoint.Y / CellHeight);
                    if (intersectionTileIdX == gridVariables.CurrentTileIDX && intersectionTileIdY == gridVariables.CurrentTileIDY)
                        break;
                }

                gridVariables.TraverseToNextCell();
            }

            return gridTraversalResult;
        }
    }
}
