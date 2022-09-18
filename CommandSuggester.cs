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
        int numberOfProperties = 18;
        int shapesNumber = 0;

        public int[,] shapesFramework;
        public int[] propertiesVector;

        SolidColorBrush black = new SolidColorBrush(Colors.Black);
        SolidColorBrush white = new SolidColorBrush(Colors.White);
        SolidColorBrush blue = new SolidColorBrush(Colors.Blue);
        SolidColorBrush green = new SolidColorBrush(Colors.Green);
        SolidColorBrush red = new SolidColorBrush(Colors.Red);
        SolidColorBrush yellow = new SolidColorBrush(Colors.Yellow);
        SolidColorBrush pink = new SolidColorBrush(Colors.Pink);
        SolidColorBrush orange = new SolidColorBrush(Colors.Orange);

        // Create matrix for the properties of the shapes
        // Each i will be another shape
        // Each j will be a property
        public void createMatrix (List <Shape> shapes)
        {
            // Initiate the matrix
            this.shapesFramework = new int[shapes.Count, numberOfProperties];
            for (int i = 0; i < shapes.Count - 1; i++)
                for (int j = 0; j < numberOfProperties - 1; j++)
                    shapesFramework[i, j] = 0;

            this.shapesNumber = shapes.Count;

            // Add shapeType properties
            for (int i = 0; i < shapes.Count - 1; i++)
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
            for (int i = 0; i < shapes.Count - 1; i++)
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
            for (int i = 0; i < shapes.Count - 1; i++)
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
            for (int i = 0; i < shapes.Count - 1; i++)
            {
                switch (shapes[i].Color)
                {
                    case black:
                        shapesFramework[i, 10] = 1;
                        break;
                    case white:
                        shapesFramework[i, 11] = 1;
                        break;
                    case blue:
                        shapesFramework[i, 12] = 1;
                        break;
                    case green:
                        shapesFramework[i, 13] = 1;
                        break;
                    case red:
                        shapesFramework[i, 14] = 1;
                        break;
                    case yellow:
                        shapesFramework[i, 15] = 1;
                        break;
                    case pink:
                        shapesFramework[i, 16] = 1;
                        break;
                    case orange:
                        shapesFramework[i, 17] = 1;
                        break;

                }
            }


        }

        public void createVector()
        {
            this.propertiesVector = new int[this.numberOfProperties];
            for (int i = 0; i < this.numberOfProperties - 1; i++)
            {
                this.propertiesVector[i] = 0;
                for (int j = 0; j < this.shapesNumber - 1; j++)
                {
                    this.propertiesVector[i] += shapesFramework[i, j];
                }
            }

        }

 

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
