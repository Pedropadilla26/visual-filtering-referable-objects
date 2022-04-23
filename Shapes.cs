using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
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

   /* public enum Color
    {
        Black,
        Blue,
        Red,
        Purple,
        Yellow,
        Green,
        Orange,
        Pink
    }
   */
    public enum Size
    {
        None,
        Big,
        Medium,
        Small,
    }
    public enum LocationY
    {
        Top,
        Bottom,
    }
    public enum LocationX
    {
        Right,
        Left
    }


    public class Shape
    {
        public ShapeType GeometricShape { get; set; }
        public SolidColorBrush Color { get; set; }
        public Size Size { get; set; }
       // public LocationY location_y { get; set; }
       //   public LocationX location_x { get; set; }

        public int Quadrant {get; set; }

        //public int x { get; set; }
        //public int y { get; set; }

        public PointCollection Points { get; set; }

        public Shape(ShapeType shape, SolidColorBrush color, Size size, int quadrant, PointCollection points)
        {
            this.GeometricShape = shape;
            this.Color = color;
            this.Size = size;
            //this.location_y = location_y;
            //this.location_x = location_x;
            this.Quadrant = quadrant;

            this.Points = points;
        }

        // Other properties, methods, events...
    }
}
