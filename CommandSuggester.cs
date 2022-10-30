using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Speech.Recognition;
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

        int colorsQuantity = 8;
        int shapesQuantity = 3;
        int sizeQuantities = 3;
        int quadrantQuantities = 5;
        int mostCommonShapeQuantityNow = 0;

        bool colorSuggestion = true;
        bool sizeSuggestion = false;
        bool locationSuggestion = true;

        bool stopWatchDebug = false;


        public void DisplayTimeElapsed(string node, TimeSpan ts)
        {
            if (stopWatchDebug) CustomMessageBox.AddTextSystem("Tiempo en hacer algoritmo de " + node + ": " + ts.Milliseconds + " ms.");
        }

        public void setColorSuggestion (bool state)
        {
            colorSuggestion = state;
        }

        public void setSizeSuggestion (bool state)
        {
            sizeSuggestion = state;
        }

        public void setLocationSuggestion (bool state)
        {
            locationSuggestion = state;
        }

        private Size CalculateSizeFromIterator(List<Shape> list, int i)
        {
            if (i <= list.Count() / 3)
            {
                return Size.Big;
            }
            else if (i <= list.Count() * 2 / 3)
            {
                return Size.Medium;
            }
            else
            {
                return Size.Small;
            }
        }

        // Create matrix for the properties of the shapes
        // Each i will be another shape
        // Each j will be a property
        public void CreateMatrix (List <Shape> shapes)
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
                switch (shapes[i].GeometricShape)
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
                Size size = CalculateSizeFromIterator(shapes, i);
                switch (size)
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
                    case Quadrants.Center:
                        shapesFramework[i, 10] = 1;
                        break;
                }
            }

            

            // Initite color properties
            for (int i = 0; i < shapes.Count - 1; i++)
            {
                switch (shapes[i].ColorEnumGenerated)
                {
                    case ColorsEnum.Black:
                        shapesFramework[i, 11] = 1;
                        break;
                    case ColorsEnum.Blue:
                        shapesFramework[i, 12] = 1;
                        break;
                    case ColorsEnum.Green:
                        shapesFramework[i, 13] = 1;
                        break;
                    case ColorsEnum.Red:
                        shapesFramework[i, 14] = 1;
                        break;
                    case ColorsEnum.Yellow:
                        shapesFramework[i, 15] = 1;
                        break;
                    case ColorsEnum.Pink:
                        shapesFramework[i, 16] = 1;
                        break;
                    case ColorsEnum.Orange:
                        shapesFramework[i, 17] = 1;
                        break;

                }
            }


        }

        public void CreateVector()
        {
            this.propertiesVector = new int[this.numberOfProperties];
            for (int i = 0; i < this.numberOfProperties - 1; i++)
            {
                this.propertiesVector[i] = 0;
                for (int j = 0; j < this.shapesNumber - 1; j++)
                {
                    this.propertiesVector[i] += shapesFramework[j, i];
                }
            }

        }

        public void Suggest()
        {
            CustomMessageBox.AddTextSystem("");
            string suggestion = "Te recomiendo que pruebes usando:";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<string> suggestions = GetMostCommon();

            if (suggestions.Count() < 1 || (suggestions[0].Length == 0 && suggestions.Count == 1))
            {
                CustomMessageBox.AddTextSystem("Lo siento, pero no he podido encontrar una expresión de referencia que coincida. Prueba a reducir el número de propiedades a sugerir.");
                return;
            }
            foreach (string text in suggestions)
            {
                CustomMessageBox.AppendTextSystemSilent(text);
            }
            CustomMessageBox.AppendTextSystemSilent("Lo cual borrará " + this.mostCommonShapeQuantityNow + " figuras\n\n");
            stopwatch.Stop();
            DisplayTimeElapsed("Sugerencias", stopwatch.Elapsed);
        }

        public bool ShouldIncrement (int shape, int size, int location, int color)
        {
            bool increment = shape == 1;
            if (colorSuggestion)
            {
                increment = increment && color == 1;
            }
            if (sizeSuggestion)
            {
                increment = increment && size == 1;
            }
            if (locationSuggestion)
            {
                increment = increment && location == 1;
            }
            return increment;
        }

        public List<string> GetMostCommon()
        {
            int numberOfMostCommon = 0;
            string color = "";
            string size = "";
            string location = "";
            string shape = "";
            List<string> suggestions = new List<string>();
            for (int w = 0; w < 3; w++)
            {
                for (int x = 3; x < 6; x++)
                {
                    for (int y = 6; y < 10; y++)
                    {
                        for (int z = 10; z < 18; z++)
                        {
                            int numberOfShapes = 0;
                            for (int i = 0; i < shapesNumber; i++)
                            {
                                if (ShouldIncrement(this.shapesFramework[i,w], this.shapesFramework[i,x], this.shapesFramework[i,y], this.shapesFramework[i,z]))
                                {
                                    numberOfShapes++;
                                }
                            }
                            if (numberOfShapes > numberOfMostCommon)
                            {
                                suggestions = new List<string>();
                                numberOfMostCommon = numberOfShapes;
                                color = GetColorOfIteration(z);
                                location = GetLocationOfIteration(y);
                                size = GetSizeOfIteration(x);
                                shape = GetShapeOfIteration(w);
                                string suggestion = GetSuggestionOfProperties(shape, color, size, location);
                                if (suggestions.IndexOf(suggestion) < 0) suggestions.Add(suggestion);
                            }
                            else if (numberOfShapes == numberOfMostCommon)
                            {
                                color = GetColorOfIteration(z);
                                location = GetLocationOfIteration(y);
                                shape = GetShapeOfIteration(w);
                                size = GetSizeOfIteration(x);
                                string suggestion = GetSuggestionOfProperties(shape, color, size, location);
                                if (suggestions.IndexOf(suggestion) < 0) suggestions.Add(suggestion);
                            }
                        }
                    }
                   }
            }
            this.mostCommonShapeQuantityNow = numberOfMostCommon;
            return suggestions;
        }



        public string GetShapeOfIteration(int w)
        {
            string mostCommonShape = "";
            switch (w)
            {
                case 0:
                    mostCommonShape = "triángulos";
                    break;
                case 1:
                    mostCommonShape = "círculos";
                    break;
                case 2:
                    mostCommonShape = "cuadrados";
                    break;
            }
            return mostCommonShape;
        }

        public string GetColorOfIteration(int z)
        {
            if (!colorSuggestion) return "";
            string mostCommonColor = "";
            switch (z)
            {
                case 11:
                    mostCommonColor = "negros";
                    break;
                case 12:
                    mostCommonColor = "azules";
                    break;
                case 13:
                    mostCommonColor = "verdes";
                    break;
                case 14:
                    mostCommonColor = "rojos";
                    break;
                case 15:
                    mostCommonColor = "amarillos";
                    break;
                case 16:
                    mostCommonColor = "rosas";
                    break;
                case 17:
                    mostCommonColor = "naranjas";
                    break;
            }
            return mostCommonColor;
        }

        public string GetSizeOfIteration(int x)
        {
            if (!sizeSuggestion) return "";
            string mostCommonSize = "";
            switch (x)
            {
                case 3:
                    mostCommonSize = "pequeños";
                    break;
                case 4:
                    mostCommonSize = "medianos";
                    break;
                case 5:
                    mostCommonSize = "grandes";
                    break;
            }
            return mostCommonSize;
        }

        public string GetLocationOfIteration(int y)
        {
            if (!locationSuggestion) return "";
            string mostCommonLocation = "";
            switch (y)
            {
                case 6:
                    mostCommonLocation = "arriba a la izquierda";
                    break;
                case 7:
                    mostCommonLocation = "arriba a la derecha";
                    break;
                case 8:
                    mostCommonLocation = "abajo a la derecha";
                    break;
                case 9:
                    mostCommonLocation = "abajo a la izquierda";
                    break;
            }
            return mostCommonLocation;
        }

        public string GetSuggestionOfProperties(string shape, string color = "", string size = "", string location = "")
        {
            if ((colorSuggestion && color.Length == 0) || (sizeSuggestion && size.Length == 0) || (locationSuggestion && location.Length == 0))
            {
                return "";
            }
            string suggestion = "- Borra los " + shape;
            if (colorSuggestion) suggestion += " " + color;
            if (sizeSuggestion) suggestion += " " + size;
            if (locationSuggestion) suggestion += " que estén " + location;
            return suggestion + "\n";
        }

        public int GetMostCommonIteration(int firstIndex, int lastIndex)
        {
            int result = -1;
            int biggestNumber = 0;
            for (int i = firstIndex; i <= lastIndex; i++)
            {
                if (this.propertiesVector[i] > biggestNumber)
                {
                    result = i;
                }
            }
            return result;
        }

        public void TryToSuggest(ReadOnlyCollection<RecognizedWordUnit> words)
        {
            if (words.Count == 5)
            {
                Suggest();
            }
            else if (words.Count > 7)
            {
                setColorSuggestion(false);
                setSizeSuggestion(false);
                setLocationSuggestion(false);
                for (int i = 7; i < words.Count; i++)
                {
                    if (words[i].Text == "color")
                    {
                        setColorSuggestion(true);
                    }
                    else if (words[i].Text == "tamaño")
                    {
                        setSizeSuggestion(true);
                    }
                    else if (words[i].Text == "posición")
                    {
                        setLocationSuggestion(true);
                    }
                }
                Suggest();

            }
            else if (words.Count == 6)
            {
                setColorSuggestion(false);
                setSizeSuggestion(false);
                setLocationSuggestion(false);
                Suggest();
            }
        }
    }

}
