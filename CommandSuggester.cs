using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Visual_filtering_referable_objects
{
    internal class CommandSuggester
    {
        int numberOfProperties = 19;

        public int[,] shapesFramework;

        // Create matrix for the properties of the shapes
        // Each i will be another shape
        // Each j will be a property
        public void createMatrix (List <Shape> shapes)
        {
            // Initiate the matrix
            this.shapesFramework = new int[shapes.Count, numberOfProperties];
            for (int i = 0; i < shapes.Count; i++)
                for (int j = 0; j < numberOfProperties; j++)
                    shapesFramework[i, j] = 0;

            // Add shapeType properties
            for (int i = 0; i < shapes.Count; i++)
            {
                switch (shapes[i].ShapeType)
                {
                    case ShapeType.Triangle:
                        shapesFramework[i, 0] = 1;
                        break;
                    case ShapeType.Circle:
                        shapesFramework[i, 1] = 1;
                        break;
                    case ShapeType.Square:
                        shapesFramework[i, 2] = 1;
                        break;

                }
            }

            // Initiate size properties
            for (int i = 0; i < shapes.Count; i++)
            {
                switch (shapes[i].Size)
                {
                    case Size.Small:
                        shapesFramework[i, 3] = 1;
                        break;
                    case Size.Medium:
                        shapesFramework[i, 4] = 1;
                        break;
                    case Size.Big:
                        shapesFramework[i, 5] = 1;
                        break;
                }
            }

            // Initiate quadrant properties
            for (int i = 0; i < shapes.Count; i++)
            {
                switch (shapes[i].Quadrant)
                {
                    case Quadrants.Top_left:
                        shapesFramework[i, 6] = 1;
                        break;
                    case Quadrants.Top_right:
                        shapesFramework[i, 7] = 1;
                        break;
                    case Quadrants.Bottom_right:
                        shapesFramework[i, 8] = 1;
                        break;
                    case Quadrants.Bottom_left:
                        shapesFramework[i, 9] = 1;
                        break;
                }
            }

            // Initite color properties
            for (int i = 0; i < shapes.Count; i++)
            {
                switch (shapes[i].Color)
                {
                    case System.Windows.Media.Brushes.Black:
                        shapesFramework[i, 10] = 1;
                        break;
                    case ColorsEnum.Blue:
                        shapesFramework[i, 11] = 1;
                        break;

                }
            }


        }

         case "azules":
                                    color = System.Windows.Media.Brushes.Blue;
                                    break;
                                case "negros":
                                    color = System.Windows.Media.Brushes.Black;
                                    break;
                                case "rojos":
                                    color = System.Windows.Media.Brushes.Red;
                                    break;
                                case "morados":
                                    color = System.Windows.Media.Brushes.Purple;
                                    break;
                                case "amarillos":
                                    color = System.Windows.Media.Brushes.Yellow;
                                    break;
                                case "verdes":
                                    color = System.Windows.Media.Brushes.Green;
                                    break;
                                case "naranjas":
                                    color = System.Windows.Media.Brushes.Orange;
                                    break;
                                case "rosas":
                                    color = System.Windows.Media.Brushes.Pink;
                                    break;

 

        public static void Suggest(List<Shape> initialShapes)
        {
            string shapeString = "triángulos";

            List<Shape> triangles = initialShapes.FindAll(
            delegate (Shape shape)
            {
                return shape.GeometricShape == ShapeType.Triangle;
            }
            );

            int maxShapesCount = triangles.Count();

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
            if (squares.Count > triangles.Count && squares.Count > circles.Count)
            {
                shapeString = "cuadrados";
            }
            else if (circles.Count > triangles.Count)
            {
                shapeString = "círculos";
            }

            for (int fooInt = (int)ShapeType.Triangle; fooInt != (int)ShapeType.None; fooInt++)
            {
                ShapeType shapeType = (ShapeType)fooInt;
            }
            CustomMessageBox.AddTextSystem("Te recomiendo que pruebes usando 'borra los " + shapeString + "'");
        }
    }
}
