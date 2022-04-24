/// 
/// Creado por Pedro Padilla Reyes para el trabajo fin de grado en Ingeniería Informática por la UGR.
/// 
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

    public class Shape
    {
        public ShapeType GeometricShape { get; set; }
        public SolidColorBrush Color { get; set; }
        public Size Size { get; set; }
 
        public Quadrants Quadrant {get; set; }

        public PointCollection Points { get; set; }

        public Shape(ShapeType shape, SolidColorBrush color, Size size, Quadrants quadrant, PointCollection points)
        {
            this.GeometricShape = shape;
            this.Color = color;
            this.Size = size;
            this.Quadrant = quadrant;
            this.Points = points;
        }
    }
}
