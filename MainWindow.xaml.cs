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
		double canvasMinX = 0;
		double canvasMinY = 0;
		double canvasMaxX = 0;
		double canvasMaxY = 0;
		Boolean isListening = false;

		static T RandomEnumValue<T>()
		{
			var v = Enum.GetValues(typeof(T));
			return (T)v.GetValue(_R.Next(v.Length - 1)+1);
		}
		public MainWindow()
        {
            InitializeComponent();

			// Set grammar and speech recognizer
            speechRecognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;

			Grammar referableObjectsGrammar = CreateGrammarFromFile();
			speechRecognizer.LoadGrammar(referableObjectsGrammar);
            Trace.WriteLine(referableObjectsGrammar.ToString());

            speechRecognizer.SetInputToDefaultAudioDevice();

			// Default speech recognizer state
            btnDisable.IsEnabled = false;

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
			isListening = true;
			btnDisable.IsEnabled = true;
			btnEnable.IsEnabled = false;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
			isListening = false;
			btnDisable.IsEnabled = false;
			btnEnable.IsEnabled = true;
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
			Quadrants quadrantToSearch1 = Quadrants.None;
			Quadrants quadrantToSearch2 = Quadrants.None;
			ShapeType shapeToSearch = ShapeType.None;
			SolidColorBrush color = System.Windows.Media.Brushes.White;
			Size size = Size.None;
			string firstWord = e.Result.Words[0].Text.ToLower();

			bool isValidStart = false;

			switch (firstWord)
            {
				case "escúchame":
					Button_Click(null, null);
					break;
				case "empieza":
					Button_Click(null, null);
					break;
				case "para":
					Button_Click_1(null, null);
					break;
				case "deja":
					Button_Click_1(null, null);
					break;
				case "reinicia":
					Button_Click_Reset(null, null);
					break;
				case "genera":
					Button_Click_Generate_Random_Canvas(null, null);
					break;
				case "interpreta":
					break;
				default:
					break;

			}

			if (e.Result.Words.Count > 2 && isListening)
			{
				string startCommand = e.Result.Words[0].Text.ToLower() + " " + e.Result.Words[1].Text.ToLower();
				string shapeString = e.Result.Words[2].Text.ToLower();
				SearchType searchType = startCommand == "borra los" ? SearchType.Multiple : startCommand == "borra el" ? SearchType.Single : SearchType.None;
				string wholeText = "";
				for (int i = 0; i < e.Result.Words.Count; i++)
				{
					wholeText = wholeText + ' ' + e.Result.Words[i].Text;
				}

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
							Button_Click_1(null, null);
							MessageBox.Show("Se va a ejecutar la siguiente acción de borrado: " + wholeText);

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
										string firstPositionWord = e.Result.Words[i + 1].Text.ToLower();
										string secondPositionWord = e.Result.Words.Count > i + 4 ? e.Result.Words[i + 2].Text.ToLower() + " " + e.Result.Words[i + 3].Text.ToLower() + " " + e.Result.Words[i + 4].Text.ToLower() : "";
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
												string centerWord = e.Result.Words.Count > i + 3 ? e.Result.Words[i + 2].Text.ToLower() + " " + e.Result.Words[i + 3].Text.ToLower() : "";
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
							string word = e.Result.Words[3].Text.ToLower();
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
							if (word == "más" || e.Result.Words[4].Text.ToLower() == "más")
							{
								string sizeWord = word == "más" ? e.Result.Words[4].Text.ToLower() : e.Result.Words[5].Text.ToLower();
								switch (sizeWord)
								{
									case "grande":
										size = Size.Big;
										break;
									case "mediano":
										size = Size.Medium;
										break;
									case "pequeño":
										size = Size.Small;
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
					Boolean anyMatch = false;

					if (searchType == SearchType.Multiple)
                    {
						List<Shape> shapesCopy = new List<Shape>(this.shapes);
						for (int i = 0; i < shapesCopy.Count(); i++)
						{
							Shape shape = shapesCopy[i];
							shape.Size = CalculateSizeFromIterator(shapesCopy, i);

							if (MatchesShape(shape, shapeToSearch, color, size, quadrantToSearch1, quadrantToSearch2))
							{
								this.shapes.Remove(shapesCopy[i]);
								anyMatch = true;
							}
						}
					}
					else if (searchType == SearchType.Single)
                    {
						List<Shape> geometricShapesList = GetSortedShapesOfType(shapeToSearch);

						for (int i = 0; i < geometricShapesList.Count(); i++)
						{
							Shape shape = geometricShapesList[i];
							if ((color == shape.Color || color == System.Windows.Media.Brushes.White) && IsBiggestOrSmallestShape(geometricShapesList, size, i))
							{
								this.shapes.Remove(geometricShapesList[i]);
								anyMatch = true;
								break;
							}
						}
					}

					if (!anyMatch)
					{
						MessageBox.Show("No encuentro ninguna forma que coincida con la descripción");
					}
					PaintShapes();
				}
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

		private Size CalculateSizeFromIterator (List<Shape> list, int i)
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

		private Boolean IsBiggestOrSmallestShape(List<Shape> list, Size size, int i)
		{
			if (list.Count() == 1) {
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

		private List<Shape> GetSortedShapesOfType (ShapeType shapeType)
        {
			List<Shape> geometricShapesList = new List<Shape>();
			foreach (var shape in this.shapes)
			{
				if (shapeType == shape.GeometricShape)
				{
					geometricShapesList.Add(shape);
				}
			}
			return geometricShapesList;
		}
	}
}
