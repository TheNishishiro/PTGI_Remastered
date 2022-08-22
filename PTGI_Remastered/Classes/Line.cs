using PTGI_Remastered.Structs;

namespace PTGI_Remastered.Classes
{
	public class Line
	{
		public Point Source;
		public Point Destination;
		public LineCoefficient Coefficient;
		public byte HasValue;
		public byte WasChecked;

		public int StructType;
		public int ObjectType;
		public int ReflectivnessType;
		public Color Color;
		public float EmissionStrength;
		public float Density;
		
		public Line(){}
		
		public Line(Point source, Point destination)
		{
			Source = source;
			Destination = destination;
			Coefficient = GetCoefficients();
			HasValue = 1;
		}
		
		public bool IsEqualTo(Line line)
		{
			return Source.IsEqualTo(line.Source) && Destination.IsEqualTo(line.Destination);
		}
		
		private LineCoefficient GetCoefficients()
		{
			var lineComponents = new LineCoefficient();

			lineComponents.A = (Destination.Y - Source.Y) / (Destination.X - Source.X);
			lineComponents.B = Source.Y - lineComponents.A * Source.X;

			return lineComponents;
		}
		
		public Point GetIntersection(Line intersectingLine, Line ignoredLine)
		{
			var intersectionPoint = new Point();

			if ((ignoredLine.HasValue == 1 && ignoredLine.IsEqualTo(this)) || WasChecked == 1)
				return intersectionPoint;
			var delta = (Source.X - Destination.X) * (intersectingLine.Source.Y - intersectingLine.Destination.Y) - (Source.Y - Destination.Y) * (intersectingLine.Source.X - intersectingLine.Destination.X);
			if(delta == 0)
				return intersectionPoint;

			var t = ((Source.X - intersectingLine.Source.X) * (intersectingLine.Source.Y - intersectingLine.Destination.Y) - (Source.Y - intersectingLine.Source.Y) * (intersectingLine.Source.X - intersectingLine.Destination.X)) / delta;
			var u = -((Source.X - Destination.X) * (Source.Y - intersectingLine.Source.Y) - (Source.Y - Destination.Y) * (Source.X - intersectingLine.Source.X)) / delta;

			if(t >= 0 && t <= 1 && u >= 0 && u <= 1)
			{
				intersectionPoint = new Point(Source.X + t * (Destination.X - Source.X), Source.Y + t * (Destination.Y - Source.Y));
			}
            
			return intersectionPoint;
		}
	}
}