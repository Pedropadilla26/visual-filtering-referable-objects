using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace Visual_filtering_referable_objects
{

    internal class FileParser
    {
        public const string DEFAULT_FILE_PATH = "/objectFiles/shapes";

        public static ColorsEnum GetColorEnum(string color)
        {
            ColorsEnum colorsEnum = ColorsEnum.None;

            switch (color)
            {
                case "Blue":
                    colorsEnum = ColorsEnum.Blue;
                    break;
                case "Black":
                    colorsEnum = ColorsEnum.Black;
                    break;
                case "Red":
                    colorsEnum = ColorsEnum.Red;
                    break;
                case "Purple":
                    colorsEnum = ColorsEnum.Purple;
                    break;
                case "Yellow":
                    colorsEnum = ColorsEnum.Yellow;
                    break;
                case "Green":
                    colorsEnum = ColorsEnum.Green;
                    break;
                case "Pink":
                    colorsEnum = ColorsEnum.Pink;
                    break;
                case "Orange":
                    colorsEnum = ColorsEnum.Orange;
                    break;
            }
            return colorsEnum;
        }

        public static List<Shape> getShapesFromFile()
        {
            List<Shape> shapes = new List<Shape>();
            Stream myStream;
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = openFileDialog1.OpenFile()) != null)
                {
                    BrushConverter brushConverter = new BrushConverter();
                    StreamReader sr = new StreamReader(myStream);
                    string lineRead = "";
                    // Code to write the stream goes here.
                    while ((lineRead = sr.ReadLine()) != null)
                    {
                        ShapeType type = ShapeType.None;
                        Quadrants quadrant = Quadrants.None;
                        PointCollection pointCollection = new PointCollection();
                        SolidColorBrush brush = new SolidColorBrush();
                        int radius = 0;
                        string[] line = lineRead.Split('-');
                        switch (line[0])
                        {
                            case "C":
                                type = ShapeType.Circle;
                                int.TryParse(line[4], out radius);
                                break;
                            case "T":
                                type = ShapeType.Triangle;
                                break;
                            case "S":
                                type = ShapeType.Square;
                                break;
                        }

                        ColorsEnum colorEnum = GetColorEnum(line[1]);

                        brush = brushConverter.ConvertFromString(line[2]) as SolidColorBrush;

                        switch (line[3])
                        {
                            case "Bottom_right":
                                quadrant = Quadrants.Bottom_right;
                                break;
                            case "Bottom_left":
                                quadrant = Quadrants.Bottom_left;
                                break;
                            case "Top_right":
                                quadrant = Quadrants.Top_right;
                                break;
                            case "Top_left":
                                quadrant = Quadrants.Top_left;
                                break;
                            case "Center":
                                quadrant = Quadrants.Center;
                                break;
                        }

                        string[] pointsStrings = line[4].Split('(');
                        foreach (string pointString in pointsStrings)
                        {
                            if (pointString.Length > 0)
                            {
                                string[] point = pointString.Split(',');
                                point[1] = point[1].Trim(')');
                                double.TryParse(point[0].ToString(), out double x);
                                double.TryParse(point[1].ToString(), out double y);
                                pointCollection.Add(new Point(x, y));
                            }

                        }

                        if (type == ShapeType.Circle) shapes.Add(new Circle(colorEnum, brush, quadrant, pointCollection, radius));
                        else shapes.Add(new Shape(type, colorEnum, brush, quadrant, pointCollection));
                    }
                    sr.Dispose();
                    myStream.Close();
                }
            }

            return shapes;
        }
        public static void saveShapesToFile(List<Shape> shapes)
        {
            string result = "";
            foreach (Shape shape in shapes)
            {
                switch (shape.GeometricShape)
                {
                    case ShapeType.Circle:
                        result += "C-";
                        break;
                    case ShapeType.Triangle:
                        result += "T-";
                        break;
                    case ShapeType.Square:
                        result += "S-";
                        break;
                }
                result += shape.ColorEnumGenerated.ToString() + "-";
                result += shape.Color + "-";
                result += shape.Quadrant + "-";
                foreach (Point point in shape.Points)
                {
                    result += "(" + point.X + "," + point.Y + ")";
                }
                if (shape.GeometricShape == ShapeType.Circle)
                {
                    Circle circle = (Circle)shape;
                    result += "-" + circle.Radius.ToString();
                }
                result += "\n";
            }

            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog
            {
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true,
                DefaultExt = ".txt",
                FileName = "shapes" // Default file name
            };

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    StreamWriter sw = new StreamWriter(myStream);
                    // Code to write the stream goes here.
                    sw.Write(result);
                    sw.Flush();//otherwise you are risking empty stream
                    sw.Dispose();
                    myStream.Close();
                }
            }
        }
    }
}
