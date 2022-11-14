using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Speech.Recognition;
using System.Windows.Media;
using System;

namespace Visual_filtering_referable_objects
{
    internal class SpeechEraserProcessor
    {
        string positionInterpreter = "";
        List<Shape> shapesWaitingForAnswer = new List<Shape>();
        List<Shape> shapesWaitingForAnswer2 = new List<Shape>();
        bool stopWatchDebug = true;


        public void DisplayTimeElapsed(string node, TimeSpan ts)
        {
            if (stopWatchDebug) CustomMessageBox.AddTextSystem("Tiempo en hacer algoritmo de " + node + ": " + ts.Milliseconds + " ms.");
        }

        public SpeechEraserProcessor()
        {
        }
        public SpeechEraserProcessor(string interpreterOption)
        {
            positionInterpreter = interpreterOption;
        }
        public void ChangePositionInterpreter(ReadOnlyCollection<RecognizedWordUnit> words)
        {
            string newInterpretation = words[words.Count - 1].Text.ToLower();
            if (newInterpretation != positionInterpreter)
            {

                CustomMessageBox.AddTextSystem("Se va a cambiar la interpretación de posiciones a " + newInterpretation);
                positionInterpreter = words[words.Count - 1].Text.ToLower();
            }
        }
        public List<Shape> AnswerYesToErase()
        {
            List<Shape> copy = shapesWaitingForAnswer;
            shapesWaitingForAnswer = new List<Shape> ();
            return copy;
        }
        public bool AnswerNoToErase()
        {

            return shapesWaitingForAnswer.Count > 0 ? true : false;
        }
        public List<Shape> AnswerRelative()
        {

            List<Shape> copy = shapesWaitingForAnswer;
            shapesWaitingForAnswer = new List<Shape>();
            shapesWaitingForAnswer2 = new List<Shape>();
            return copy;
        }
        public List<Shape> AnswerAbsolute()
        {

            List<Shape> copy = shapesWaitingForAnswer2;
            shapesWaitingForAnswer = new List<Shape>();
            shapesWaitingForAnswer2 = new List<Shape>();
            return copy;
        }
        public List<Shape> EraseShapes(List<Shape> initialShapes, ReadOnlyCollection<RecognizedWordUnit> words)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Quadrants quadrantToSearch1 = Quadrants.None;
            Quadrants quadrantToSearch2 = Quadrants.None;
            ShapeType shapeToSearch = ShapeType.None;
            SolidColorBrush color = System.Windows.Media.Brushes.White;
            Size size = Size.None;
            Positions positionToSearch = Positions.None;
            bool searchForExtreme = false;

            bool isValidStart = false;
            string startCommand = words[0].Text.ToLower() + " " + words[1].Text.ToLower();
            string shapeString = words[2].Text.ToLower();
            SearchType searchType = startCommand == "borra los" ? SearchType.Multiple : startCommand == "borra el" ? SearchType.Single : SearchType.None;
            string wholeText = "";
            for (int i = 0; i < words.Count; i++)
            {
                wholeText = wholeText + ' ' + words[i].Text;
            }

            CustomMessageBox.AddTextSystem("realizando borrado...");

            switch (searchType)
            {
                case SearchType.Multiple:
                    switch (shapeString)
                    {
                        case "triángulos":
                            shapeToSearch = ShapeType.Triangle;
                            isValidStart = true;
                            break;
                        case "cuadrados":
                            shapeToSearch = ShapeType.Square;
                            isValidStart = true;
                            break;
                        case "círculos":
                            shapeToSearch = ShapeType.Circle;
                            isValidStart = true;
                            break;
                        default:
                            break;
                    }
                    if (isValidStart)
                    {
                        //CustomMessageBox.AddTextSystem("Se va a ejecutar la siguiente acción de borrado: " + wholeText);

                        for (int i = 3; i < words.Count; i++)
                        {
                            string word = words[i].Text.ToLower();
                            switch (word)
                            {
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

                                case "estén":
                                    string firstPositionWord = words[i + 1].Text.ToLower();
                                    string secondPositionWord = "";
                                    switch (firstPositionWord)
                                    {
                                        case "arriba":
                                            secondPositionWord = words.Count > i + 4 ? words[i + 2].Text.ToLower() + " " + words[i + 3].Text.ToLower() + " " + words[i + 4].Text.ToLower() : "";
                                            switch (secondPositionWord)
                                            {
                                                case "a la izquierda":
                                                    quadrantToSearch1 = Quadrants.Top_left;
                                                    break;
                                                case "a la derecha":
                                                    quadrantToSearch1 = Quadrants.Top_right;
                                                    break;
                                                default:
                                                    quadrantToSearch1 = Quadrants.Top_left;
                                                    quadrantToSearch2 = Quadrants.Top_right;
                                                    positionToSearch = Positions.Top;
                                                    break;
                                            }
                                            break;
                                        case "abajo":
                                            secondPositionWord = words.Count > i + 4 ? words[i + 2].Text.ToLower() + " " + words[i + 3].Text.ToLower() + " " + words[i + 4].Text.ToLower() : "";
                                            switch (secondPositionWord)
                                            {
                                                case "a la izquierda":
                                                    quadrantToSearch1 = Quadrants.Bottom_left;
                                                    break;
                                                case "a la derecha":
                                                    quadrantToSearch1 = Quadrants.Bottom_right;
                                                    break;
                                                default:
                                                    quadrantToSearch1 = Quadrants.Bottom_left;
                                                    quadrantToSearch2 = Quadrants.Bottom_right;
                                                    positionToSearch = Positions.Bottom;
                                                    break;
                                            }
                                            break;
                                        case "a":
                                            secondPositionWord = words.Count > i + 3 ? words[i + 2].Text.ToLower() + " " + words[i + 3].Text.ToLower() : "";
                                                switch (secondPositionWord)
                                                {
                                                    case "la izquierda":
                                                        quadrantToSearch1 = Quadrants.Bottom_left;
                                                        quadrantToSearch2 = Quadrants.Top_left;
                                                        positionToSearch = Positions.Left;

                                                    break;
                                                    case "la derecha":
                                                        quadrantToSearch1 = Quadrants.Bottom_right;
                                                        quadrantToSearch2 = Quadrants.Top_right;
                                                        positionToSearch = Positions.Right;
                                                    break;
                                                    default:
                                                        break;
                                                }
                                            break;
                                        case "en":
                                            string centerWord = words.Count > i + 3 ? words[i + 2].Text.ToLower() + " " + words[i + 3].Text.ToLower() : "";
                                            if (centerWord == "el centro")
                                                quadrantToSearch1 = Quadrants.Center;
                                            break;
                                        case "más":
                                            secondPositionWord = words.Count > i + 4 ? words[i + 2].Text.ToLower() + " " + words[i + 3].Text.ToLower() + " " + words[i + 4].Text.ToLower() : words[i + 2].Text.ToLower();
                                            switch (secondPositionWord)
                                            {
                                                case "a la izquierda":
                                                    positionToSearch = Positions.Left;
                                                    break;
                                                case "a la derecha":
                                                    positionToSearch = Positions.Right;
                                                    break;
                                                case "arriba":
                                                    positionToSearch = Positions.Top;
                                                    break;
                                                case "abajo":
                                                    positionToSearch = Positions.Bottom;
                                                    break;
                                            }
                                            break;
                                    }

                                    break;

                                case "grandes":
                                    size = Size.Big;
                                    break;
                                case "medianos":
                                    size = Size.Medium;
                                    break;
                                case "pequeños":
                                    size = Size.Small;
                                    break;
                                default:
                                    break;
                            }
                        }

                    }
                    break;
                case SearchType.Single:
                    switch (shapeString)
                    {
                        case "triángulo":
                            shapeToSearch = ShapeType.Triangle;
                            isValidStart = true;
                            break;
                        case "cuadrado":
                            shapeToSearch = ShapeType.Square;
                            isValidStart = true;
                            break;
                        case "círculo":
                            shapeToSearch = ShapeType.Circle;
                            isValidStart = true;
                            break;
                        default:
                            break;
                    }
                    if (isValidStart)
                    {
                        string word = words.Count() > 3 ? words[3].Text.ToLower() : "";
                        string wordFor = words.Count() > 4 ? words[4].Text.ToLower() : "";
                        switch (word)
                        {
                            case "azul":
                                color = System.Windows.Media.Brushes.Blue;
                                break;
                            case "negro":
                                color = System.Windows.Media.Brushes.Black;
                                break;
                            case "rojo":
                                color = System.Windows.Media.Brushes.Red;
                                break;
                            case "morado":
                                color = System.Windows.Media.Brushes.Purple;
                                break;
                            case "amarillo":
                                color = System.Windows.Media.Brushes.Yellow;
                                break;
                            case "verde":
                                color = System.Windows.Media.Brushes.Green;
                                break;
                            case "naranja":
                                color = System.Windows.Media.Brushes.Orange;
                                break;
                            case "rosa":
                                color = System.Windows.Media.Brushes.Pink;
                                break;
                        }
                        if (word == "más" || wordFor == "más")
                        {
                            string sizeWord = word == "más" ? words[4].Text.ToLower() : words[5].Text.ToLower();
                            switch (sizeWord)
                            {
                                case "grande":
                                    size = Size.Big;
                                    searchForExtreme = true;
                                    break;
                                case "mediano":
                                    size = Size.Medium;
                                    searchForExtreme = true;
                                    break;
                                case "pequeño":
                                    size = Size.Small;
                                    searchForExtreme = true;
                                    break;
                                case "a":
                                    string positionWord = word == "más" ? words[6].Text.ToLower() : words[7].Text.ToLower();
                                    switch (positionWord)
                                    {
                                        case "izquierda":
                                            positionToSearch = Positions.Left;
                                            break;
                                        case "derecha":
                                            positionToSearch = Positions.Right;
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                case "arriba":
                                    positionToSearch = Positions.Top;
                                    break;
                                case "abajo":
                                    positionToSearch = Positions.Bottom;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    break;
            }
            if (isValidStart)
            {
                bool anyMatch = false;

                if (searchForExtreme && AreAllSameSize(GetSortedShapesOfType(shapeToSearch, initialShapes)))
                {
                    CustomMessageBox.AddTextSystem("Todos los objetos son del mismo tamaño. ¿Quiere que los borre todos?");
                    shapesWaitingForAnswer = new List<Shape>(initialShapes);
                    shapesWaitingForAnswer.RemoveAll(shape => MatchesShapeType(shape, shapeToSearch));
                    return initialShapes;
                }

                if (searchType == SearchType.Multiple)
                {
                    List<Shape> shapesCopy = new List<Shape>(initialShapes);
                    for (int i = 0; i < shapesCopy.Count(); i++)
                    {
                        Shape shape = shapesCopy[i];
                        shape.Size = CalculateSizeFromIterator(shapesCopy, i, size);

                        if (MatchesShape(shapesCopy, shape, shapeToSearch, color, size, quadrantToSearch1, quadrantToSearch2, positionToSearch))
                        {
                            initialShapes.Remove(shapesCopy[i]);
                            anyMatch = true;
                        }
                    }
                    if ((shapesCopy.Count - initialShapes.Count) == 1)
                    {
                        CustomMessageBox.AddTextSystem("Solo hay un "+shapeString +" con las características descritas. ¿Quiere que lo borre?");
                        shapesWaitingForAnswer = initialShapes;
                        return shapesCopy;
                    }
                }
                else if (searchType == SearchType.Single)
                {
                    List<Shape> geometricShapesList = GetSortedShapesOfType(shapeToSearch, initialShapes);
                    bool warnedAboutPlural = false;

                    for (int i = 0; i < geometricShapesList.Count(); i++)
                    {
                        Shape shape = geometricShapesList[i];
                        bool isColor = color == System.Windows.Media.Brushes.White ? true : color.Color == shape.Color.Color;
                        bool isBiggestOrSmallestSisze = IsBiggestOrSmallestShape(geometricShapesList, size, i);
                        bool isExtremePosition = IsExtremePosition(geometricShapesList, positionToSearch, shape);
                        if (isColor && isBiggestOrSmallestSisze && isExtremePosition)
                        {
                            if (anyMatch && !warnedAboutPlural)
                            {
                                string shapePropertiesMessage = "Hay más de un " + shapeString;
                                if (words.Count() > 3)
                                {
                                    shapePropertiesMessage += " de color " + words[3].Text.ToLower();
                                }
                                CustomMessageBox.AddTextSystem(shapePropertiesMessage + ", los voy a borrar todos, pero mejor hable en plural si hay varios.");
                                warnedAboutPlural = true;
                            }
                            initialShapes.Remove(geometricShapesList[i]);
                            anyMatch = true;
                        }
                    }
                }

                if (!anyMatch)
                {
                    CustomMessageBox.AddTextSystem("No encuentro ninguna forma que coincida con la descripción");
                }
                else
                {
                    CustomMessageBox.AddTextSystem("He borrado las figuras que coincidían.");
                }
            }
            stopwatch.Stop();
            DisplayTimeElapsed("Borrado", stopwatch.Elapsed);

            return initialShapes;
        }
        private bool MatchesShape(
            List<Shape> shapes,
            Shape shape,
            ShapeType shapeToSearch,
            SolidColorBrush color,
            Size size,
            Quadrants quadrantToSearch1,
            Quadrants quadrantToSearch2,
            Positions positionToSearch)
        {

            if (shapeToSearch != shape.GeometricShape)
            {
                return false;
            }
            if (color != shape.Color && color != System.Windows.Media.Brushes.White)
            {
                return false;
            }
            if (size != shape.Size && size != Size.None)
            {
                return false;
            }
            if (positionToSearch != Positions.None)
            {
                if (!IsExtremePositionSoft(shapes, positionToSearch, shape, shapeToSearch)) return false;
            }
            else if (quadrantToSearch1 != shape.Quadrant && quadrantToSearch1 != Quadrants.None && quadrantToSearch2 != shape.Quadrant)
            {
                return false;
            }

            return true;
        }

        private Size CalculateSizeFromIterator(List<Shape> list, int i, Size sizeToSearch)
        {
            int iteratorForBig = list.Count() / 3;
            int iteratorForMedium = list.Count() * 2 / 3;

            // If it qualifies for something it means the user sees some Shape as this size, and they may have the same size
            // of one of those that are designated that size, even if it is not in the correct iterator
            bool qualifiesForBig = (list[i].Area == list[iteratorForBig].Area) && sizeToSearch == Size.Big;
            bool qualifiesForMedium = (list[i].Area == list[iteratorForMedium].Area) && sizeToSearch == Size.Medium;
            bool qualifiesForSmall = (list[i].Area == list[list.Count() - 1].Area) && sizeToSearch == Size.Small;

            if (qualifiesForBig)
                return Size.Big;
            else if (qualifiesForMedium)
                return Size.Medium;
            else if (qualifiesForSmall)
                return Size.Small;


            if (i <= iteratorForBig)
            {
                return Size.Big;
            }
            else if (i <= iteratorForMedium)
            {
                return Size.Medium;
            }
            else
            {
                return Size.Small;
            }
        }

        private bool IsBiggestOrSmallestShape(List<Shape> list, Size size, int i)
        {
            if (list.Count() == 1 || size == Size.None)
            {
                return true;
            }
            if (size == Size.Big)
            {
                if (i == 0)
                {
                    return true;
                }
            }
            else if (size == Size.Small)
            {
                if (i == list.Count() - 1)
                {
                    return true;
                }
            }
            return false;
        }

        private List<Shape> GetSortedShapesOfType(ShapeType shapeType, List<Shape> shapes)
        {
            List<Shape> geometricShapesList = new List<Shape>();
            foreach (Shape shape in shapes)
            {
                if (shapeType == shape.GeometricShape)
                {
                    geometricShapesList.Add(shape);
                }
            }
            return geometricShapesList;
        }

        public bool IsExtremePositionSoft(List<Shape> shapes, Positions positionToSearch, Shape shapeToSearch, ShapeType shapeType)
        {
            bool result = false;

            List<Shape> geometricShapesList = GetSortedShapesOfType(shapeType, shapes);


            if (geometricShapesList.Count < 2)
            {
                return true;
            }

            switch (positionToSearch)
            {
                case Positions.Left:
                    geometricShapesList.Sort(delegate (Shape x, Shape y)
                    {
                        return (x.Points[0].X < y.Points[0].X ? -1 : 1);
                    });
                    break;
                case Positions.Right:
                    geometricShapesList.Sort(delegate (Shape x, Shape y)
                    {
                        return (x.Points[0].X < y.Points[0].X ? 1 : -1);
                    });
                    break;
                case Positions.Top:
                    geometricShapesList.Sort(delegate (Shape x, Shape y)
                    {
                        return (x.Points[0].Y > y.Points[0].Y ? 1 : -1);
                    });
                    break;
                case Positions.Bottom:
                    geometricShapesList.Sort(delegate (Shape x, Shape y)
                    {
                        return (x.Points[0].Y > y.Points[0].Y ? -1 : 1);
                    });
                    break;

            }
            if (geometricShapesList.Count() < 3)
            {
                if (geometricShapesList.IndexOf(shapeToSearch) == 0) result = true;
            }
            else if (geometricShapesList.IndexOf(shapeToSearch) < geometricShapesList.Count / 3) result = true;
            return result;
        }


        public bool IsExtremePosition(List<Shape> shapes, Positions positionToSearch, Shape shapeToSearch)
        {
            if (positionToSearch == Positions.None || shapes.Count < 2)
            {
                return true;
            }
            bool result = true;
            foreach (Shape shape in shapes)
            {
                if (shapeToSearch != shape)
                {
                    switch (positionToSearch)
                    {
                        case Positions.Left:
                            if (shape.Points[0].X < shapeToSearch.Points[0].X)
                            {
                                result = false;
                            }
                            break;
                        case Positions.Right:
                            if (shape.Points[0].X > shapeToSearch.Points[0].X)
                            {
                                result = false;
                            }
                            break;
                        case Positions.Top:
                            if (shape.Points[0].Y < shapeToSearch.Points[0].Y)
                            {
                                result = false;
                            }
                            break;
                        case Positions.Bottom:
                            if (shape.Points[0].Y > shapeToSearch.Points[0].Y)
                            {
                                result = false;
                            }
                            break;
                    }
                }
            }
            return result;
        }

        private bool AreAllSameSize(List<Shape> shapes)
        {
            if (shapes[0] == shapes[shapes.Count - 1])
            {
                return true;
            }
            else
                return false;
        }

        private bool MatchesShapeType(Shape s, ShapeType shapeType)
        {
            if (s.GeometricShape.Equals(shapeType))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}