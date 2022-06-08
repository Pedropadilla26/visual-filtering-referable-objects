using System.Windows;
/// 
/// Creado por Pedro Padilla Reyes para el trabajo fin de grado en Ingeniería Informática por la UGR.
/// 
using System.Windows.Media;

namespace Visual_filtering_referable_objects
{

    public enum ShapeType
    {
        None,
        Triangle,
        Square,
        Circle
    }

    public enum Size
    {
        None,
        Big,
        Medium,
        Small,
    }

    public enum Quadrants
    {
        None,
        Top_left,
        Top_right,
        Center,
        Bottom_left,
        Bottom_right,
    }

    public enum ColorsEnum
    {
        None,
        Blue,
        Black,
        Red,
        Purple,
        Yellow,
        Green,
        Orange,
        Pink
    }

    public enum SearchType
    {
        None,
        Single,
        Multiple
    }

    public enum Positions
    {
        None,
        Right,
        Left,
        Top,
        Bottom,
    }

    public abstract class AbstractShape
    {
        public ShapeType ShapeType { get; set; }
        public abstract RectangleGeometry GetBoundingBox();

    }

    public class Shape : AbstractShape
    {
        public ShapeType GeometricShape { get; set; }
        public SolidColorBrush Color { get; set; }
        public Size Size { get; set; }
        public Quadrants Quadrant { get; set; }
        public PointCollection Points { get; set; }

        public double Area { get; set; }

        public Shape(ShapeType shape, SolidColorBrush color, Quadrants quadrant, PointCollection points)
        {
            GeometricShape = shape;
            Color = color;
            Quadrant = quadrant;
            Points = points;

            switch (GeometricShape)
            {
                case ShapeType.Triangle:
                    Area = (points[1].X - points[0].X) * (points[1].Y - points[2].Y) / 2;
                    break;
                case ShapeType.Square:
                    Area = (points[1].X - points[0].X) * (points[1].Y - points[3].Y);
                    break;
                default:
                    Area = -1;
                    break;
            }
        }

        public override RectangleGeometry GetBoundingBox()
        {
            PointCollection points = new PointCollection(Points);
            Point bottom_right = new Point(points[1].X + 2, points[1].Y + 2);
            Point top_left = new Point(points[0].X - 2, points[2].Y - 2);
            return new RectangleGeometry(new Rect(top_left, bottom_right));
        }

    }
}
