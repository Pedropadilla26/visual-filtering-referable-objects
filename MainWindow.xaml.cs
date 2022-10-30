/// 
/// Creado por Pedro Padilla Reyes para el trabajo fin de grado en Ingeniería Informática por la UGR.
/// 
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Speech.Recognition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Visual_filtering_referable_objects
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SpeechRecognitionEngine speechRecognizer = new SpeechRecognitionEngine(new CultureInfo("es-ES"));

        List<Shape> shapes = new List<Shape>();
        List<Shape> initialShapes = new List<Shape>();
        Stack<List<Shape>> lastShapes = new Stack<List<Shape>>();
        static Random _R = new Random();
        double canvasMinX = 0;
        double canvasMinY = 0;
        double canvasMaxX = 0;
        double canvasMaxY = 0;
        int numberToGenerate = 5;
        bool isSuggestionOptionsDesplegated = false;
        bool isCommandGuideOpen = false;

        // Default speech recognizer state
        bool isListening = false;
        bool nightMode = false;

        SpeechEraserProcessor speechEraser = new SpeechEraserProcessor();

        SolidColorBrush lightModeCanvasBackground = new SolidColorBrush(Color.FromArgb(100, 230, 230, 230));
        SolidColorBrush lightModeWindowBackground = new SolidColorBrush(Colors.White);
        SolidColorBrush darkModeWindowBackground = new SolidColorBrush(Color.FromArgb(100, 86, 86, 86));
        InstructionsGuide intructionsGuideWindow;
        CommandSuggester suggester = new CommandSuggester();

        static T RandomEnumValue<T>()
        {
            Array v = Enum.GetValues(typeof(T));
            // Decreases random number so we dont get None (last value) in random enum values
            int random = _R.Next(v.Length - 1);
            if (random < 0) random = 0;
            return (T)v.GetValue(random);
        }
        public MainWindow()
        {
            InitializeComponent();
            Canvas_.Background = lightModeCanvasBackground;

            // Set grammar and speech recognizer
            speechRecognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;

            Grammar referableObjectsGrammar = CreateGrammarFromFile();
            speechRecognizer.LoadGrammar(referableObjectsGrammar);
            Trace.WriteLine(referableObjectsGrammar.ToString());

            speechRecognizer.SetInputToDefaultAudioDevice();

            // Set canvas variables
            canvasMaxX = Canvas_.Width;
            canvasMaxY = Canvas_.Height;

            // DEFAULT SHAPES (FOR NOW)
            AddDefaultShapes();


            PaintShapes();
        }

        private void AddDefaultShapes()
        {
            // TRIANGLE
            System.Windows.Point Point1 = new System.Windows.Point(150, 130);
            System.Windows.Point Point2 = new System.Windows.Point(200, 130);
            System.Windows.Point Point3 = new System.Windows.Point(175, 75);
            PointCollection myPointCollection = new PointCollection
            {
                Point1,
                Point2,
                Point3
            };
            Shape shape = new Shape(ShapeType.Triangle, ColorsEnum.Blue, System.Windows.Media.Brushes.Blue, Quadrants.Top_left, myPointCollection);
            AddShape(shape);

            //SQUARE
            System.Windows.Point Point4 = new System.Windows.Point(30, 350);
            System.Windows.Point Point5 = new System.Windows.Point(30, 250);
            System.Windows.Point Point6 = new System.Windows.Point(150, 250);
            System.Windows.Point Point7 = new System.Windows.Point(150, 350);
            PointCollection myPointCollection2 = new PointCollection
            {
                Point4,
                Point5,
                Point6,
                Point7
            };
            Shape shape2 = new Shape(ShapeType.Square, ColorsEnum.Red, System.Windows.Media.Brushes.Red, Quadrants.Bottom_left, myPointCollection2);
            AddShape(shape2);

            //CIRCLE
            System.Windows.Point Point8 = new System.Windows.Point(500, 300);
            PointCollection myPointCollection3 = new PointCollection
            {
                Point8
            };
            Shape shape3 = new Circle(ColorsEnum.Green, GetColorFromString("Green"), Quadrants.Bottom_right, myPointCollection3, 25);
            AddShape(shape3);

            initialShapes = new List<Shape>(shapes);
            speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);

            CustomMessageBox.SetTextField(chat);
            CustomMessageBox.AddTextSystem("Hola, soy Syl, la asistente virtual de esta aplicación. Puedes usar los botones o los comandos de la guía de comandos para controlarla.");
        }

        private bool ShapesOverlap(Shape shape1, Shape shape2, bool paintsPath = false)
        {
            RectangleGeometry boundingGeometry1 = shape1.GetBoundingBox();
            RectangleGeometry boundingGeometry2 = shape2.GetBoundingBox();
            System.Windows.Media.IntersectionDetail intersection = boundingGeometry1.FillContainsWithDetail(boundingGeometry2);

            if (intersection != System.Windows.Media.IntersectionDetail.Empty)
            {
                return true;
            }

            if (paintsPath)
            {
                Path myPath = new Path
                {
                    Fill = Brushes.LemonChiffon,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Data = boundingGeometry1
                };
                Canvas_.Children.Add(myPath);
            }
            return false;

        }

        private bool IsInCenterQuadrant(Point point)
        {
            double min_quadrant_x = canvasMaxX / 3;
            double max_quadrant_x = canvasMaxX * 2 / 3;
            double min_quadrant_y = canvasMaxY / 3;
            double max_quadrant_y = canvasMaxY * 2 / 3;

            return point.X > min_quadrant_x && point.X < max_quadrant_x && point.Y > min_quadrant_y && point.Y < max_quadrant_y;
        }
        private bool IsInTopLeftQuadrant(Point point)
        {
            return point.X > 0 && point.X <= canvasMaxX / 2 && point.Y > 0 && point.Y <= canvasMaxY / 2 && !IsInCenterQuadrant(point);
        }
        private bool IsInTopRightQuadrant(Point point)
        {
            return point.X > canvasMaxX / 2 && point.X < canvasMaxX && point.Y > 0 && point.Y <= canvasMaxY / 2 && !IsInCenterQuadrant(point);
        }
        private bool IsInBottomLeftQuadrant(Point point)
        {
            return point.X > 0 && point.X <= canvasMaxX / 2 && point.Y > canvasMaxY / 2 && point.Y < canvasMaxY && !IsInCenterQuadrant(point);
        }
        private bool IsInBottomRightQuadrant(Point point)
        {
            return point.X > canvasMaxX / 2 && point.X < canvasMaxX && point.Y > canvasMaxY / 2 && point.Y < canvasMaxY && !IsInCenterQuadrant(point);

        }

        private Quadrants GetQuadrantFromPoint(Point point)
        {
            if (IsInCenterQuadrant(point)) return Quadrants.Center;
            else if (IsInTopLeftQuadrant(point)) return Quadrants.Top_left;
            else if (IsInTopRightQuadrant(point)) return Quadrants.Top_right;
            else if (IsInBottomLeftQuadrant(point)) return Quadrants.Bottom_left;
            else if (IsInBottomRightQuadrant(point)) return Quadrants.Bottom_right;
            else return Quadrants.None;
        }

        private void clearPathPaints()
        {
            List<Path> paths = Canvas_.Children.OfType<Path>().ToList();
            foreach (Path path in paths)
            {
                Canvas_.Children.Remove(path);
            }
        }

        private void GenerateRandomShapes(int howMany)
        {
            clearPathPaints();
            ClearCanvas();
            shapes = new List<Shape>();

            int i = 0;
            while (i < howMany)
            {
                // Generate data of a shape randomly, including the first point and 'length' of the shape
                Shape shapeToAdd;
                ShapeType randomShape = RandomEnumValue<ShapeType>();
                int shapeLength = _R.Next(65) + 10;
                PointCollection myPointCollection = new PointCollection();
                System.Windows.Point firstPoint = new System.Windows.Point(_R.Next(450) + 70, _R.Next(260) + 70);
                myPointCollection.Add(firstPoint);
                Quadrants generatedQuadrant = GetQuadrantFromPoint(firstPoint);
                ColorsEnum colorEnumGenerated = RandomEnumValue<ColorsEnum>();
                SolidColorBrush colorGenerated = GetColorFromString(colorEnumGenerated.ToString());

                // If the random shape is a circle, reduce size and add it with length as radius
                if (randomShape == ShapeType.Circle)
                {
                    if (shapeLength > 20) shapeLength = (int)(shapeLength - shapeLength * 0.3); // Circles are too big
                    shapeToAdd = (new Circle(colorEnumGenerated, colorGenerated, generatedQuadrant, myPointCollection, shapeLength));
                }
                // If the random shape is a triangle or circle, generate lefting points using the first one and length and then add it
                else
                {
                    if (randomShape == ShapeType.Triangle)
                    {
                        myPointCollection.Add(new Point(firstPoint.X + shapeLength, firstPoint.Y));
                        myPointCollection.Add(new Point(firstPoint.X + shapeLength / 2, firstPoint.Y - shapeLength));

                    }
                    else if (randomShape == ShapeType.Square)
                    {
                        myPointCollection.Add(new Point(firstPoint.X + shapeLength, firstPoint.Y));
                        myPointCollection.Add(new Point(firstPoint.X + shapeLength, firstPoint.Y - shapeLength));
                        myPointCollection.Add(new Point(firstPoint.X, firstPoint.Y - shapeLength));
                    }
                    shapeToAdd = (new Shape(randomShape, colorEnumGenerated, colorGenerated, generatedQuadrant, myPointCollection));
                }

                // Check if the shape overlaps any other shape, if it does don't add it to the list of shapes 

                bool overlaps = false;

                foreach (Shape shape in shapes)
                {
                    if (ShapesOverlap(shapeToAdd, shape))
                    {
                        overlaps = true;
                    }
                }

                if (!overlaps)
                {
                    i++;
                    AddShape(shapeToAdd);
                }
            }
            // Copy the generated list of shapes so it's used as a backup when 'reset' button is called
            initialShapes = new List<Shape>(shapes);
        }

        public SolidColorBrush GetColorFromString(string color)
        {
            return color == "None" ? new SolidColorBrush(Colors.White) : (SolidColorBrush)new BrushConverter().ConvertFromString(color);
        }

        private void AddShape(Shape shape)
        {
            shapes.Add(shape);
            // The list of shapes is always sorted from biggest to smallest
            shapes.Sort(delegate (Shape x, Shape y)
            {
                return (x.Area < y.Area ? 1 : -1);
            });

        }

        private static Grammar CreateGrammarFromFile(string file = @"..\..\grammars\Grammar.xml")
        {

            Grammar shapesGrammar = new Grammar(file)
            {
                Name = "SRGS File Grammar"
            };
            return shapesGrammar;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isListening)
            {
                isListening = false;
                voiceIcon.Source = new BitmapImage(new Uri("pack://application:,,,/microphone-solid.png"));
                btnVoiceRecognizing.Background = new SolidColorBrush(Color.FromArgb(255, 205, 240, 234));
            }
            else
            {
                isListening = true;
                voiceIcon.Source = new BitmapImage(new Uri("pack://application:,,,/microphone-slash-solid.png"));

                btnVoiceRecognizing.Background = new SolidColorBrush(Color.FromArgb(255, 246, 198, 234));
            }
        }

        private void Button_Click_Suggestion_Options(object sender, RoutedEventArgs e)
        {
            if (isSuggestionOptionsDesplegated)
            {
                isSuggestionOptionsDesplegated = false;
                optionsIcon.Source = new BitmapImage(new Uri("pack://application:,,,/arrow-down-solid.png"));
                OptionsGrid.Visibility = Visibility.Hidden;
            }
            else
            {
                isSuggestionOptionsDesplegated = true;
                optionsIcon.Source = new BitmapImage(new Uri("pack://application:,,,/arrow-up-solid.png"));
                OptionsGrid.Visibility = Visibility.Visible;
            }
        }

        private void Button_Instructions_Guide(object sender, RoutedEventArgs e)
        {
            intructionsGuideWindow = new InstructionsGuide(nightMode);
            intructionsGuideWindow.Show();
            isCommandGuideOpen = true;
        }

        private void Button_Night_Mode(object sender, RoutedEventArgs e)
        {
            if (nightMode)
            {
                nightModeIcon.Source = new BitmapImage(new Uri("pack://application:,,,/moon-white-solid.png"));
                btnNightMode.Background = new SolidColorBrush(Color.FromArgb(255, 79, 79, 79));
                //nightModeText.Text = "Modo noche";
                //nightModeText.Foreground = new SolidColorBrush(Colors.White);
                Background = lightModeWindowBackground;
                Canvas_.Background = lightModeCanvasBackground;
                CanvasBorder.BorderBrush = new SolidColorBrush(Colors.Black);
                nightMode = false;
                LastInstruction.Foreground = new SolidColorBrush(Colors.Black);
                ChatBorder.Background = lightModeCanvasBackground;
                chat.Foreground = new SolidColorBrush(Colors.Black);
                ChatBorder.BorderBrush = new SolidColorBrush(Colors.Black);
                Shapes_checkbox.Foreground = new SolidColorBrush(Colors.Black);
                Colors_checkbox.Foreground = new SolidColorBrush(Colors.Black);
                Size_checkbox.Foreground = new SolidColorBrush(Colors.Black);
                Location_checkbox.Foreground = new SolidColorBrush(Colors.Black);
            }
            else
            {
                btnNightMode.Background = new SolidColorBrush(Colors.LightGray);
                //nightModeText.Foreground = new SolidColorBrush(Colors.Black);
                nightModeIcon.Source = new BitmapImage(new Uri("pack://application:,,,/sun-solid.png"));
                //nightModeText.Text = "Modo día";
                Background = darkModeWindowBackground;
                Canvas_.Background = Background;
                CanvasBorder.BorderBrush = new SolidColorBrush(Colors.Gray);
                nightMode = true;
                LastInstruction.Foreground = new SolidColorBrush(Colors.White);
                ChatBorder.Background = darkModeWindowBackground;
                chat.Foreground = new SolidColorBrush(Colors.White);
                ChatBorder.BorderBrush = new SolidColorBrush(Colors.Gray);
                Shapes_checkbox.Foreground = new SolidColorBrush(Colors.White);
                Colors_checkbox.Foreground = new SolidColorBrush(Colors.White);
                Size_checkbox.Foreground = new SolidColorBrush(Colors.White);
                Location_checkbox.Foreground = new SolidColorBrush(Colors.White);
            }
        }

        private void SaveCurrentShapes()
        {
            lastShapes.Push(new List<Shape>(shapes));
        }

        private void Button_Click_Reset(object sender, RoutedEventArgs e)
        {
            shapes = new List<Shape>(initialShapes);
            SaveCurrentShapes();
            PaintShapes();
        }

        private void Button_Click_Backward(object sender, RoutedEventArgs e)
        {
            if (lastShapes.Count > 0)
            {
                shapes = new List<Shape>(lastShapes.Pop());
                PaintShapes();
            }
        }

        private void Button_Click_Speaker(object sender, RoutedEventArgs e)
        {
            CustomMessageBox.ToggleSpeaker();
            if (CustomMessageBox.speakerActivated)
                btnActivateSpeakerIcon.Source = new BitmapImage(new Uri("pack://application:,,,/volume-high-solid.png"));
            else
                btnActivateSpeakerIcon.Source = new BitmapImage(new Uri("pack://application:,,,/volume-xmark-solid.png"));
        }

        private void Button_Click_Clear_Chat(object sender, RoutedEventArgs e)
        {
            chat.SelectAll();
            chat.Selection.Text = "";
        }

        private void Button_Click_Suggest(object sender, RoutedEventArgs e)
        {
            suggester.Suggest();
        }

        private void Button_Save_Shapes_To_File(object sender, RoutedEventArgs e)
        {
            FileParser.saveShapesToFile(shapes);
        }
        private void Button_Load_Shapes_From_File(object sender, RoutedEventArgs e)
        {
            SaveCurrentShapes();
            List<Shape> shapesLoad = FileParser.getShapesFromFile();
            if (shapesLoad.Count < 1)
            {
                return;
            }
            this.shapes = shapesLoad;
            this.initialShapes = this.shapes;
            PaintShapes();
        }

        private void Button_Click_Generate_Random_Canvas(object sender, RoutedEventArgs e)
        {
            SaveCurrentShapes();
            GenerateRandomShapes(numberToGenerate);
            PaintShapes();
            Console.WriteLine("List of ordered shapes");
            foreach (Shape shape in shapes)
            {
                Console.WriteLine(shape.GeometricShape.ToString() + ", " + shape.Size + ", " + shape.Area.ToString());
            }
        }

        private void Button_Click_Generate_Random_Canvas_With_Text(object sender, RoutedEventArgs e, ReadOnlyCollection<RecognizedWordUnit> words = null)
        {
            SaveCurrentShapes();
            int howManyGenerate = words.Count > 5 ? ParseNumberString(words[5].Text) : numberToGenerate;
            GenerateRandomShapes(howManyGenerate);
            PaintShapes();
            Console.WriteLine("List of ordered shapes");
            foreach (Shape shape in shapes)
            {
                Console.WriteLine(shape.GeometricShape.ToString() + ", " + shape.Size + ", " + shape.Area.ToString());
            }
        }

        private void PaintShapes()
        {
            ClearCanvas();
            suggester.CreateMatrix(this.shapes);
            suggester.CreateVector();
            Console.WriteLine("Painting canvas...");
            foreach (Shape shape in shapes)
            {
                if (shape.GeometricShape == ShapeType.Circle)
                {
                    Circle circle = (Circle)shape;
                    Console.WriteLine("Painting a circle");
                    Console.WriteLine(circle.Color);
                    Console.WriteLine(circle.Points);

                    Ellipse myEllipse = new Ellipse
                    {
                        Width = circle.Radius * 2,
                        Height = circle.Radius * 2,
                        Stroke = System.Windows.Media.Brushes.Black,
                        StrokeThickness = 2,
                        Fill = circle.Color
                    };


                    Canvas_.Children.Add(myEllipse);

                    myEllipse.SetValue(Canvas.LeftProperty, (double)circle.Points[0].X - circle.Radius);
                    myEllipse.SetValue(Canvas.TopProperty, (double)circle.Points[0].Y - circle.Radius);

                }
                else
                {
                    Console.WriteLine("Painting a shape");
                    Console.WriteLine(shape.Color);
                    Console.WriteLine(shape.Points);

                    Polygon myPolygon = new Polygon
                    {
                        Stroke = System.Windows.Media.Brushes.Black,
                        Fill = shape.Color,
                        StrokeThickness = 2,
                        Points = shape.Points
                    };
                    Canvas_.Children.Add(myPolygon);
                }

            }
            
        }

        private void ClearCanvas()
        {
            Console.WriteLine("Clearing canvas...");
            List<Polygon> polygons = Canvas_.Children.OfType<Polygon>().ToList();
            foreach (Polygon polygon in polygons)
            {
                Canvas_.Children.Remove(polygon);
            }
            List<Ellipse> ellipses = Canvas_.Children.OfType<Ellipse>().ToList();
            foreach (Ellipse ellipse in ellipses)
            {
                Canvas_.Children.Remove(ellipse);
            }

        }

        private void speechRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string wholeText = "";
            for (int i = 0; i < e.Result.Words.Count; i++)
            {
                wholeText = wholeText + ' ' + e.Result.Words[i].Text.ToLower();
            }
            string firstWord = e.Result.Words[0].Text.ToLower();
            bool validStart = isListening || (!isListening && ((firstWord == "escúchame") || (firstWord == "empieza")));
            if (!validStart) return;

            CustomMessageBox.AddTextUser(wholeText);

            switch (firstWord)
            {
                case "escúchame":
                    if (isListening)
                    {
                        CustomMessageBox.AddTextSystem("La aplicacion ya está en ese modo.");
                        break;
                    }
                    Button_Click(null, null);
                    CustomMessageBox.AddTextSystem("Se ha activado el reconocimiento de voz.");
                    break;
                case "empieza":
                    if (isListening)
                    {
                        CustomMessageBox.AddTextSystem("La aplicacion ya está en ese modo.");
                        break;
                    }
                    Button_Click(null, null);
                    CustomMessageBox.AddTextSystem("Se ha activado el reconocimiento de voz.");
                    break;
                case "para":
                    if (isListening)
                    {
                        CustomMessageBox.AddTextSystem("La aplicacion ya está en ese modo.");
                        break;
                    }
                    Button_Click(null, null);
                    CustomMessageBox.AddTextSystem("Se ha desactivado el reconocimiento de voz.");
                    break;
                case "deja":
                    if (isListening)
                    {
                        CustomMessageBox.AddTextSystem("La aplicacion ya está en ese modo.");
                        break;
                    }
                    Button_Click(null, null);
                    CustomMessageBox.AddTextSystem("Se ha desactivado el reconocimiento de voz.");
                    break;
                case "reinicia":
                    Button_Click_Reset(null, null);
                    CustomMessageBox.AddTextSystem("Hecho.");
                    break;
                case "genera":
                    Button_Click_Generate_Random_Canvas_With_Text(null, null, e.Result.Words);
                    CustomMessageBox.AddTextSystem("Hecho.");
                    break;
                case "interpreta":
                    speechEraser.ChangePositionInterpreter(e.Result.Words);
                    break;
                case "borra":
                    SaveCurrentShapes();
                    shapes = speechEraser.EraseShapes(shapes, e.Result.Words);
                    PaintShapes();
                    Button_Click(null, null);
                    break;
                case "modo":
                    string secondWord = e.Result.Words[1].Text.ToLower();
                    if ((secondWord == "noche" && nightMode) || (secondWord == "día" && !nightMode))
                    {
                        CustomMessageBox.AddTextSystem("La aplicacion ya está en ese modo.");
                        break;
                    }
                    Button_Night_Mode(null, null);
                    CustomMessageBox.AddTextSystem("Hecho.");
                    break;
                case "abre":
                    Button_Instructions_Guide(null, null);
                    CustomMessageBox.AddTextSystem("Hecho.");
                    break;
                case "cierra":
                    if (isCommandGuideOpen)
                    {
                        intructionsGuideWindow.Close();
                        isCommandGuideOpen = false;
                        CustomMessageBox.AddTextSystem("Hecho.");
                    }
                    else
                    {
                        CustomMessageBox.AddTextSystem("No está abierta.");
                    }
                    break;
                case "activa":
                    if (!CustomMessageBox.speakerActivated) Button_Click_Speaker(null, null);
                    else
                    {
                        CustomMessageBox.AddTextSystem("La aplicacion ya está en ese modo.");
                        break;
                    }
                    CustomMessageBox.AddTextSystem("Hecho.");
                    break;
                case "desactiva":
                    if (CustomMessageBox.speakerActivated) Button_Click_Speaker(null, null);
                    else
                    {
                        CustomMessageBox.AddTextSystem("La aplicacion ya está en ese modo.");
                        break;
                    }
                    CustomMessageBox.AddTextSystem("Hecho.");
                    break;
                case "sí":
                    List<Shape> copy = speechEraser.AnswerYesToErase();
                    if (copy.Count > 0)
                    {
                        SaveCurrentShapes();
                        shapes = copy;
                        PaintShapes();
                        Button_Click(null, null);
                        CustomMessageBox.AddTextSystem("Vale.");
                    }
                    break;
                case "no":
                    if (speechEraser.AnswerNoToErase())
                    {
                        CustomMessageBox.AddTextSystem("Vale.");
                    }
                    break;
                case "deshacer":
                    Button_Click_Backward(null, null);
                    CustomMessageBox.AddTextSystem("Hecho.");
                    break;
                case "deshaz":
                    Button_Click_Backward(null, null);
                    CustomMessageBox.AddTextSystem("Hecho.");
                    break;
                case "vacía":
                    CustomMessageBox.AddTextSystem("Hecho.");
                    Button_Click_Clear_Chat(null, null);
                    break;
                case "sugiéreme":
                    suggester.TryToSuggest(e.Result.Words);
                    break;
                case "sugiere":
                    suggester.TryToSuggest(e.Result.Words);
                    break;
                case "haz":
                    suggester.TryToSuggest(e.Result.Words);
                    break;
                default:
                    break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            speechRecognizer.Dispose();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string text = (e.AddedItems[0] as ComboBoxItem).Content as string;
            if (text != null)
            {
                numberToGenerate = int.Parse(text);
            }
        }

        private void Shapes_checkbox_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Color_checkbox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as ToggleButton).IsChecked)
            {
                // Code for Checked state
                suggester.setColorSuggestion(true);
            }
            else
            {
                // Code for Un-Checked state
                suggester.setColorSuggestion(false);
            }
        }

        private void Size_checkbox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as ToggleButton).IsChecked)
            {
                // Code for Checked state
                suggester.setSizeSuggestion(true);
            }
            else
            {
                // Code for Un-Checked state
                suggester.setSizeSuggestion(false);
            }
        }

        private void Location_checkbox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as ToggleButton).IsChecked)
            {
                // Code for Checked state
                suggester.setLocationSuggestion(true);
            }
            else
            {
                // Code for Un-Checked state
                suggester.setLocationSuggestion(false);
            }
        }

        private int ParseNumberString(string numberWord)
        {
            switch (numberWord)
            {
                case "Una":
                    return 1;
                    break;
                case "Dos":
                    return 2;
                    break;
                case "Tres":
                    return 3;
                    break;
                case "Cuatro":
                    return 4;
                    break;
                case "Cinco":
                    return 5;
                    break;
                case "Seis":
                    return 6;
                    break;
                case "Siete":
                    return 7;
                    break;
                case "Ocho":
                    return 8;
                    break;
                case "Nueve":
                    return 9;
                    break;
                case "Diez":
                    return 10;
                    break;
                case "Once":
                    return 11;
                    break;
                case "Doce":
                    return 12;
                    break;
                case "Trece":
                    return 13;
                    break;
                case "Catorce":
                    return 14;
                    break;
                case "Quince":
                    return 15;
                    break;
                case "Dieciseis":
                    return 16;
                    break;
                case "Diecisiete":
                    return 17;
                    break;
                case "Dieciocho":
                    return 18;
                    break;
                case "Diecinueve":
                    return 19;
                    break;
                case "Veinte":
                    return 20;
                    break;
                default:
                    return 0;
                    break;
            }
        }

    }
}
