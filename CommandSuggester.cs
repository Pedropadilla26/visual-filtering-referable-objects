using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visual_filtering_referable_objects
{
    internal class CommandSuggester
    {
        public static void Suggest(List<Shape> initialShapes)
        {
            List<Shape> triangles = initialShapes.FindAll(
            delegate (Shape shape)
            {
                return shape.GeometricShape == ShapeType.Triangle;
            }
            );
            List<Shape> squares = initialShapes.FindAll(
            delegate (Shape shape)
            {
                return shape.GeometricShape == ShapeType.Square;
            });
            List <Shape> circles = initialShapes.FindAll(
            delegate (Shape shape)
            {
                return shape.GeometricShape == ShapeType.Circle;
            });
            string shapeString = "triángulos";
            if (squares.Count > triangles.Count && squares.Count > circles.Count)
            {
                shapeString = "cuadrados";
            }
            else if (circles.Count > triangles.Count)
            {
                shapeString = "círculos";
            }
                

            CustomMessageBox.AddTextSystem("Te recomiendo que pruebes usando 'borra los " + shapeString + "'");
        }
    }
}
