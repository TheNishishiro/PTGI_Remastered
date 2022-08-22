using System;
using PTGI_Remastered.Utilities;

namespace PTGI_Remastered.Classes
{
	public class Point
	{
		public float X { get; set; }
		public float Y { get; set; }
		public byte HasValue { get; set; }

		public Point()
		{
			
		}
		
		public Point(float x, float y)
		{
			X = x;
			Y = y;
			HasValue = 1;
		}
		
		public void Add(Point p)
		{
			X += p.X;
			Y += p.Y;
		}
		
		public bool IsEqualTo(Point sPoint)
		{
			return X == sPoint.X && Y == sPoint.Y;
		}

		public double GetDistance(Point destination)
		{
			return Math.Sqrt((destination.X - X) * (destination.X - X)  + (destination.Y - Y) * (destination.Y - Y));
		}
		
		public bool LiesInObject(Polygon obstacle)
		{
			switch (obstacle.structType)
			{
				case PTGI_StructTypes.Polygon:
					return LiesInPolygon(obstacle);
				case PTGI_StructTypes.Rectangle:
					return LiesInRectangle(obstacle);
				default:
					return false;
			}
		}

		private bool LiesInRectangle(Polygon obstacle)
		{
			return X >= obstacle.Walls[0].Source.X && Y >= obstacle.Walls[0].Source.Y &&
			       X <= obstacle.Walls[1].Destination.X && Y <= obstacle.Walls[1].Destination.Y;
		}
        
		private bool LiesInPolygon(Polygon obstacle)
		{
			var result = false;
			var verticiesCount = obstacle.Verticies.Length;
			var j = verticiesCount - 1;

			for (var i = 0; i < verticiesCount; i++)
			{
				if ((obstacle.Verticies[i].Y < Y && obstacle.Verticies[j].Y >= Y || obstacle.Verticies[j].Y < Y && obstacle.Verticies[i].Y >= Y) && obstacle.Verticies[i].X + (Y - obstacle.Verticies[i].Y) / (obstacle.Verticies[j].Y - obstacle.Verticies[i].Y) * (obstacle.Verticies[j].X - obstacle.Verticies[i].X) < X)
				{
					result = !result;
				}
				j = i;
			}

			return result;
		}
	}
}