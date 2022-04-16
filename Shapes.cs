using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visual_filtering_referable_objects
{

    public enum ShapeType
    {
        Triangle,
        Square,
        Circle
    }

    public enum Color
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
    public enum Size
    {
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
        public ShapeType shape { get; set; }
        public Color color { get; set; }
        public Size size { get; set; }
       // public LocationY location_y { get; set; }
       //   public LocationX location_x { get; set; }

        public int quadrant {get; set; }

        public int x { get; set; }
        public int y { get; set; }

        public Shape()
        {

        }

        public Shape(ShapeType shape, Color color, Size size, int quadrant)
        {
            this.shape = shape;
            this.color = color;
            this.size = size;
            //this.location_y = location_y;
            //this.location_x = location_x;
            this.quadrant = quadrant;
        }

        // Other properties, methods, events...
    }
}
