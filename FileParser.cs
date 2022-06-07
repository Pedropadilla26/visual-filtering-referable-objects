using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace Visual_filtering_referable_objects
{

    internal class FileParser
    {
        public const string DEFAULT_FILE_PATH = "/objectFiles/shapes";

        public static List<Shape> getShapesFromFile() {
            List<Shape> shapes = new List<Shape>();
            Stream myStream;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = openFileDialog1.OpenFile()) != null)
                {
                    var brushConverter = new BrushConverter();
                    var sr = new StreamReader(myStream);
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

                        brush = brushConverter.ConvertFromString(line[1]) as SolidColorBrush;

                        switch (line[2])
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

                        string[] pointsStrings = line[3].Split('(');
                        foreach (var pointString in pointsStrings)
                        {
                            if (pointString.Length > 0)
                            {
                                string[] point = pointString.Split(',');
                                double.TryParse(point[0].ToString(), out double x);
                                double.TryParse(point[1].ToString(), out double y);
                                pointCollection.Add(new Point(x, y));
                            }
                            
                        }

                        if (type == ShapeType.Circle) shapes.Add(new Circle(brush, quadrant, pointCollection, radius));
                        else shapes.Add(new Shape(type, brush, quadrant, pointCollection));
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
                result += shape.Color + "-";
                result += shape.Quadrant + "-";
                foreach (var point in shape.Points)
                {
                    result += "(" + point.X + "," + point.Y + ")";
                }
                if (shape.GeometricShape == ShapeType.Circle)
                {
                    var circle = (Circle)shape;
                    result += "-" + circle.Radius.ToString();
                }
                result += "\n";
            }

            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.DefaultExt = ".txt";
            saveFileDialog1.FileName = "shapes"; // Default file name

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    var sw = new StreamWriter(myStream);
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
