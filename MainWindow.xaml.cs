/// 
/// Creado por Pedro Padilla Reyes para el trabajo fin de grado en Ingeniería Informática por la UGR.
/// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Speech.Recognition;
using System.Globalization;
using System.Diagnostics;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

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
		static Random _R = new Random();
		double canvasMinX = 0;
		double canvasMinY = 0;
		double canvasMaxX = 0;
		double canvasMaxY = 0;

		// Default speech recognizer state
		Boolean isListening = false;
		Boolean nightMode = false;

		SpeechEraserProcessor speechEraser = new SpeechEraserProcessor();

		SolidColorBrush lightModeCanvasBackground = new SolidColorBrush(Color.FromArgb(100, 230, 230, 230));
		SolidColorBrush lightModeWindowBackground = new SolidColorBrush(Colors.White);
		SolidColorBrush darkModeWindowBackground = new SolidColorBrush(Color.FromArgb(100, 86, 86, 86));
		InstructionsGuide intructionsGuideWindow;

		static T RandomEnumValue<T>()
		{
			var v = Enum.GetValues(typeof(T));
			return (T)v.GetValue(_R.Next(v.Length - 1)+1);
		}
		public MainWindow()
        {
            InitializeComponent();
			Canvas_.Background = lightModeCanvasBackground;
			LastInstructionLabel.Visibility = Visibility.Hidden;

			// Set grammar and speech recognizer
			speechRecognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;

			Grammar referableObjectsGrammar = CreateGrammarFromFile();
			speechRecognizer.LoadGrammar(referableObjectsGrammar);
            Trace.WriteLine(referableObjectsGrammar.ToString());

            speechRecognizer.SetInputToDefaultAudioDevice();

			// Set canvas variables
			this.canvasMaxX = Canvas_.Width;
			this.canvasMaxY = Canvas_.Height;

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
			PointCollection myPointCollection = new PointCollection();
			myPointCollection.Add(Point1);
			myPointCollection.Add(Point2);
			myPointCollection.Add(Point3);
			Shape shape = new Shape(ShapeType.Triangle, System.Windows.Media.Brushes.Blue, Quadrants.Top_left, myPointCollection);
			AddShape(shape);

			//SQUARE
			System.Windows.Point Point4 = new System.Windows.Point(30, 350);
			System.Windows.Point Point5 = new System.Windows.Point(30, 250);
			System.Windows.Point Point6 = new System.Windows.Point(150, 250);
			System.Windows.Point Point7 = new System.Windows.Point(150, 350);
			PointCollection myPointCollection2 = new PointCollection();
			myPointCollection2.Add(Point4);
			myPointCollection2.Add(Point5);
			myPointCollection2.Add(Point6);
			myPointCollection2.Add(Point7);
			Shape shape2 = new Shape(ShapeType.Square, System.Windows.Media.Brushes.Red, Quadrants.Bottom_left, myPointCollection2);
			AddShape(shape2);

			//CIRCLE
			System.Windows.Point Point8 = new System.Windows.Point(500, 300);
			PointCollection myPointCollection3 = new PointCollection();
			myPointCollection3.Add(Point8);
			Shape shape3 = new Circle(GetColorFromString("Green"), Quadrants.Bottom_right, myPointCollection3, 25);
			AddShape(shape3);

			this.initialShapes = new List<Shape>(this.shapes);
			speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
		}

		private Boolean ShapesOverlap (Shape shape1, Shape shape2, Boolean paintsPath = false)
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
				Path myPath = new Path();
				myPath.Fill = Brushes.LemonChiffon;
				myPath.Stroke = Brushes.Black;
				myPath.StrokeThickness = 1;
				myPath.Data = boundingGeometry1;
				Canvas_.Children.Add(myPath);
			}
			return false;
            
		}

		private Boolean IsInCenterQuadrant(Point point)
        {
			double min_quadrant_x = this.canvasMaxX / 3;
			double max_quadrant_x = this.canvasMaxX * 2 / 3;
			double min_quadrant_y = this.canvasMaxY / 3;
			double max_quadrant_y = this.canvasMaxY * 2 / 3;

			return point.X > min_quadrant_x && point.X < max_quadrant_x && point.Y > min_quadrant_y && point.Y < max_quadrant_y;
		}
		private Boolean IsInTopLeftQuadrant(Point point)
		{
			return point.X > 0 && point.X <= canvasMaxX / 2 && point.Y > 0 && point.Y <= canvasMaxY / 2 && !IsInCenterQuadrant(point);
		}
		private Boolean IsInTopRightQuadrant(Point point)
		{
			return point.X > canvasMaxX / 2 && point.X < canvasMaxX && point.Y > 0 && point.Y <= canvasMaxY / 2 && !IsInCenterQuadrant(point);
		}
		private Boolean IsInBottomLeftQuadrant(Point point)
		{
			return point.X > 0 && point.X <= canvasMaxX / 2 && point.Y > canvasMaxY / 2 && point.Y < canvasMaxY && !IsInCenterQuadrant(point);
		}
		private Boolean IsInBottomRightQuadrant(Point point)
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
			var paths = Canvas_.Children.OfType<Path>().ToList();
			foreach (var path in paths)
			{
				Canvas_.Children.Remove(path);
			}
		}

		private void GenerateRandomShapes(int howMany)
		{
			clearPathPaints();

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
				SolidColorBrush colorGenerated = GetColorFromString(RandomEnumValue<ColorsEnum>().ToString());

				// If the random shape is a circle, reduce size and add it with length as radius
				if (randomShape == ShapeType.Circle)
                {
					if (shapeLength > 20) shapeLength = (int)(shapeLength - shapeLength * 0.3); // Circles are too big
					shapeToAdd = (new Circle(colorGenerated, generatedQuadrant, myPointCollection, shapeLength));
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
					shapeToAdd = (new Shape(randomShape, colorGenerated, generatedQuadrant, myPointCollection));
				}

				// Check if the shape overlaps any other shape, if it does don't add it to the list of shapes 

				Boolean overlaps = false;

				foreach (Shape shape in this.shapes)
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
			this.initialShapes = new List<Shape>(this.shapes);
		}

		private SolidColorBrush GetColorFromString (string color)
        {
			return (SolidColorBrush)new BrushConverter().ConvertFromString(color);
		}

		private void AddShape (Shape shape)
        {
			this.shapes.Add(shape);
			// The list of shapes is always sorted from biggest to smallest
			this.shapes.Sort(delegate (Shape x, Shape y)
			{
				return (x.Area < y.Area ? 1 : -1);
			});

		}

        private static Grammar CreateGrammarFromFile(String file = @"..\..\grammars\Grammar.xml")
		{

			Grammar shapesGrammar = new Grammar(file);
			shapesGrammar.Name = "SRGS File Grammar";
			return shapesGrammar;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
        {
			if (isListening)
            {
				isListening = false;
				voiceText.Text = "Activar reconocimiento de voz";
				voiceIcon.Source = new BitmapImage(new Uri("pack://application:,,,/microphone-solid.png"));
				btnVoiceRecognizing.Background = new SolidColorBrush(Color.FromArgb(255, 212, 255, 191));
			}
			else
            {
				isListening = true;
				voiceText.Text = "Desactivar reconocimiento de voz";
				voiceIcon.Source = new BitmapImage(new Uri("pack://application:,,,/microphone-slash-solid.png"));

				btnVoiceRecognizing.Background = new SolidColorBrush(Color.FromArgb(255, 255, 190, 190));
			}
		}

		private void Button_Instructions_Guide(object sender, RoutedEventArgs e)
        {
			intructionsGuideWindow = new InstructionsGuide(nightMode);
			intructionsGuideWindow.Show();
		}

		private void Button_Night_Mode(object sender, RoutedEventArgs e)
        {
			if (nightMode)
            {
				nightModeIcon.Source = new BitmapImage(new Uri("pack://application:,,,/moon-white-solid.png"));
				btnNightMode.Background = new SolidColorBrush(Color.FromArgb(255, 79, 79, 79));
				nightModeText.Text = "Modo noche";
				nightModeText.Foreground = new SolidColorBrush(Colors.White);
				this.Background = lightModeWindowBackground;
				Canvas_.Background = lightModeCanvasBackground;
				CanvasBorder.BorderBrush = new SolidColorBrush(Colors.Black);
				this.nightMode = false;
				LastInstructionLabel.Foreground = new SolidColorBrush(Colors.Black);
				LastInstruction.Foreground = new SolidColorBrush(Colors.Black);
			}
			else
            {
				btnNightMode.Background = new SolidColorBrush(Colors.LightGray);
				nightModeText.Foreground = new SolidColorBrush(Colors.Black);
				nightModeIcon.Source = new BitmapImage(new Uri("pack://application:,,,/sun-solid.png"));
				nightModeText.Text = "Modo día";
				this.Background = darkModeWindowBackground;
				Canvas_.Background = this.Background;
				CanvasBorder.BorderBrush = new SolidColorBrush(Colors.Gray);
				this.nightMode = true;
				LastInstructionLabel.Foreground = new SolidColorBrush(Colors.White);
				LastInstruction.Foreground = new SolidColorBrush(Colors.White);
			}
		}

		private void Button_Click_Reset(object sender, RoutedEventArgs e)
		{
			this.shapes = new List<Shape>(this.initialShapes);
			PaintShapes();
		}

		private void Button_Click_Generate_Random_Canvas(object sender, RoutedEventArgs e)
		{
			this.shapes = new List<Shape>();
			GenerateRandomShapes(5);
			PaintShapes();
			Console.WriteLine("List of ordered shapes");
			foreach (var shape in this.shapes)
            {
				Console.WriteLine(shape.GeometricShape.ToString() + ", " + shape.Size + ", " + shape.Area.ToString());
            }
		}

		private void PaintShapes()
		{
			if (this.shapes.Count > 0) {
				ClearCanvas();
				Console.WriteLine("Painting canvas...");
				foreach (var shape in this.shapes)
				{
					if (shape.GeometricShape == ShapeType.Circle)
					{
						var circle = (Circle)shape;
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
		}

		private void ClearCanvas()
		{
			Console.WriteLine("Clearing canvas...");
			var polygons = Canvas_.Children.OfType<Polygon>().ToList();
			foreach (var polygon in polygons)
			{
				Canvas_.Children.Remove(polygon);
			}
			var ellipses = Canvas_.Children.OfType<Ellipse>().ToList();
			foreach (var ellipse in ellipses)
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
			if (LastInstructionLabel.Visibility == Visibility.Hidden)
            {
				LastInstructionLabel.Visibility = Visibility.Visible;
			}

			LastInstruction.Text = wholeText;
			string firstWord = e.Result.Words[0].Text.ToLower();
			switch (firstWord)
            {
				case "escúchame":
					if (!isListening) Button_Click(null, null);
					break;
				case "empieza":
					if (!isListening) Button_Click(null, null);
					break;
				case "para":
					if (isListening)  Button_Click(null, null);
					break;
				case "deja":
					if (isListening) Button_Click(null, null);
					break;
				case "reinicia":
					if (isListening) Button_Click_Reset(null, null);
					break;
				case "genera":
					if (isListening) Button_Click_Generate_Random_Canvas(null, null);
					break;
				case "interpreta":
					if (isListening) this.speechEraser.ChangePositionInterpreter(e.Result.Words);
					break;
				case "borra":
					if (e.Result.Words.Count > 2 && isListening)
					{
						this.shapes = this.speechEraser.EraseShapes(this.shapes, e.Result.Words);
						PaintShapes();
					}
					break;
				case "modo":
					if (e.Result.Words.Count > 2 && isListening)
					{
						Boolean isVoice = e.Result.Words[2].Text.ToLower() == "voz" ? true : false;
						CustomMessageBox.SetActivated(!isVoice);
					}
					if (e.Result.Words.Count == 2 && isListening)
					{
						Button_Night_Mode(null, null);
					}
					break;
				case "abre":
					Button_Instructions_Guide(null, null);
					break;
				case "cierra":
					intructionsGuideWindow.Close();
					break;
				default:
					break;
			}
				
		}

		private void closeAutomatically()
		{
			Thread.Sleep(7000);
			SendKeys.SendWait("{Enter}");//or Esc
		}


		public void ShowMessageAutoClose(string message)
		{
			System.Windows.MessageBox.Show(message);
			(new System.Threading.Thread(closeAutomatically)).Start();
		}


		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            speechRecognizer.Dispose();
        }
    }
}
