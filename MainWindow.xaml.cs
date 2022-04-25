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
		static T RandomEnumValue<T>()
		{
			var v = Enum.GetValues(typeof(T));
			return (T)v.GetValue(_R.Next(v.Length - 1)+1);
		}
		public MainWindow()
        {
            InitializeComponent();
            speechRecognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;

			Grammar referableObjectsGrammar = CreateGrammarFromFile();
			speechRecognizer.LoadGrammar(referableObjectsGrammar);
            Trace.WriteLine(referableObjectsGrammar.ToString());

            speechRecognizer.SetInputToDefaultAudioDevice();
            btnDisable.IsEnabled = false;

			// DEFAULT SHAPES (FOR NOW)

			// TRIANGLE
			System.Windows.Point Point1 = new System.Windows.Point(150, 100);
			System.Windows.Point Point2 = new System.Windows.Point(200, 160);
			System.Windows.Point Point3 = new System.Windows.Point(100, 100);
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
			Shape shape3 = new Circle(GetColorFromString("Green"), Quadrants.Bottom_right, myPointCollection3, 50);
			AddShape(shape3);

			this.initialShapes = new List<Shape>(this.shapes);
			PaintShapes();
		}
		private void CreateAndAddShapeFromString (string shapeInfo)
        {

        }

		private void GenerateRandomShapes(int howMany)
		{
			for (int i = 0; i < howMany; i++)
            {
				ShapeType randomShape = RandomEnumValue<ShapeType>();
				int howManyPoints = randomShape == ShapeType.Circle ? 1 : randomShape == ShapeType.Triangle ? 3 : 4;
				System.Windows.Point firstPoint = new System.Windows.Point(_R.Next(360) + 20, _R.Next(560) + 20);
				Quadrants generatedQuadrant = GetQuadrantFromPoint(firstPoint);
				int shapeLength = _R.Next(45) + 5;

				PointCollection myPointCollection = new PointCollection();
				myPointCollection.Add(firstPoint);
				for (int j = 1; j < howManyPoints; j++)
                {

                }

				if (randomShape == ShapeType.Circle)
                {
					AddShape(new Circle(GetColorFromString(RandomEnumValue<ColorsEnum>().ToString()), Quadrants.Bottom_left, myPointCollection, shapeLength));


				}
				else
                {
					AddShape(new Shape(randomShape, GetColorFromString(RandomEnumValue<ColorsEnum>().ToString()), Quadrants.Bottom_left, myPointCollection));
				}
			}
		}
		private SolidColorBrush GetColorFromString (string color)
        {
			return (SolidColorBrush)new BrushConverter().ConvertFromString(color);
		}
		private void AddShape (Shape shape)
        {
			this.shapes.Add(shape);
			this.shapes.Sort(delegate (Shape x, Shape y)
			{
				return (x.Area > y.Area ? 1 : -1);
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
            speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
			btnDisable.IsEnabled = true;
			btnEnable.IsEnabled = false;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            speechRecognizer.RecognizeAsyncStop();
			btnDisable.IsEnabled = false;
			btnEnable.IsEnabled = true;
		}

		private void Button_Click_Reset(object sender, RoutedEventArgs e)
		{
			speechRecognizer.RecognizeAsyncStop();
			btnDisable.IsEnabled = false;
			btnEnable.IsEnabled = true;
			this.shapes = new List<Shape>(this.initialShapes);
			PaintShapes();
		}
		private void PaintShapes()
		{
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
						Width = circle.Radius,
						Height = circle.Radius,
						Stroke = System.Windows.Media.Brushes.Black,
						StrokeThickness = 2,
						Fill = circle.Color
					};


					Canvas_.Children.Add(myEllipse);

					myEllipse.SetValue(Canvas.LeftProperty, (double)circle.Points[0].X);
					myEllipse.SetValue(Canvas.TopProperty, (double)circle.Points[0].Y);


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
			Button_Click_1(null, null);
			Quadrants quadrantToSearch1 = Quadrants.None;
			Quadrants quadrantToSearch2 = Quadrants.None;
			ShapeType shapeToSearch = ShapeType.None;
			SolidColorBrush color = System.Windows.Media.Brushes.White;
			Size size = Size.None;

			bool isValidStart = false;

			if (e.Result.Words.Count>2)
			{
				string startCommand = e.Result.Words[0].Text.ToLower() + " " + e.Result.Words[1].Text.ToLower();
				string shapeString = e.Result.Words[2].Text.ToLower();
				string wholeText = "";
				for (int i = 0; i < e.Result.Words.Count; i++)
                {
					wholeText = wholeText + ' ' + e.Result.Words[i].Text;
                }
				MessageBox.Show("Has hablado! Has dicho esto: " + wholeText);

				switch (startCommand)
				{
					case "borra los":
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
							for (int i = 3; i < e.Result.Words.Count; i++)
							{
								string word = e.Result.Words[i].Text.ToLower();
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
										string firstPositionWord = e.Result.Words[i+1].Text.ToLower();
										string secondPositionWord = e.Result.Words.Count > i+4 ? e.Result.Words[i+2].Text.ToLower() + " " + e.Result.Words[i + 3].Text.ToLower() + " " + e.Result.Words[i + 4].Text.ToLower() : "";
										switch (firstPositionWord)
                                        {
											case "arriba":
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
														break;
												}
												break;
											case "abajo":
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
														break;
												}
												break;
											case "en":
												string centerWord = e.Result.Words.Count > i + 3 ? e.Result.Words[i+2].Text.ToLower() + " " + e.Result.Words[i + 3].Text.ToLower() : "";
												if (centerWord == "el centro")
													quadrantToSearch1 = Quadrants.Center;
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
				}
			}
			if (isValidStart)
            {
				List<Shape> shapesCopy = new List<Shape>(this.shapes);
				Boolean anyMatch = false;

				for (int i = 0; i < shapesCopy.Count(); i++)
				{
					Shape shape = shapesCopy[i];
					shape.Size = CalculateSizeFromIterator(i);

					if (MatchesShape(shape, shapeToSearch, color, size, quadrantToSearch1, quadrantToSearch2))
					{
						this.shapes.Remove(shapesCopy[i]);
						anyMatch = true;
					}
				}
				if (!anyMatch)
                {
					MessageBox.Show("No encuentro ninguna forma que coincida con la descripción");
				}
				PaintShapes();
			}
				
		}

		private Boolean MatchesShape(
			Shape shape, 
			ShapeType shapeToSearch, 
			SolidColorBrush color, 
			Size size, 
			Quadrants quadrantToSearch1,
			Quadrants quadrantToSearch2)
        {
			Boolean matchesShape = false;
			Boolean matchesColor = false;
			Boolean matchesSize = false;
			Boolean matchesQuadrant = false;

			if (shapeToSearch == shape.GeometricShape)
			{
				matchesShape = true;
			}
            if (color == shape.Color || color == System.Windows.Media.Brushes.White)
            {
                matchesColor = true;
            }
			if (size == shape.Size || size == Size.None)
			{
				matchesSize = true;
			}
			if (quadrantToSearch1 == shape.Quadrant || quadrantToSearch1 == Quadrants.None || quadrantToSearch2 == shape.Quadrant)
			{
				matchesQuadrant = true;
			}

			return matchesShape && matchesColor && matchesSize && matchesQuadrant;
        }

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            speechRecognizer.Dispose();
        }

        private void RichTextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

		private Size CalculateSizeFromIterator (int i)
        {
			if (i <= this.shapes.Count() / 3)
			{
				return Size.Big;
			}
			else if (i <= this.shapes.Count() * 2 / 3)
			{
				return Size.Medium;
			}
			else
			{
				return Size.Small;
			}
		}
    }
}
