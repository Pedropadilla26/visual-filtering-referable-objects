﻿using System;
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

		private int triangles = 4;
        private int squares = 3;
        private int circles = 4;

		List<Shape> shapes = new List<Shape>();

		public MainWindow()
        {
            InitializeComponent();
            speechRecognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;

            GrammarBuilder pluralGrammar = new GrammarBuilder("borra los");
            Choices shapesChoices = new Choices("triangulos", "cuadrados", "circulos");
            Choices colorsChoices = new Choices("rojos", "azules", "amarillos", "verdes", "negros");
            pluralGrammar.Append(shapesChoices);
            pluralGrammar.Append(colorsChoices, 0, 1);

            Grammar referableObjectsGrammar = CreateGrammarFromFile();

            //speechRecognizer.LoadGrammar(new Grammar(pluralGrammar));
            speechRecognizer.LoadGrammar(referableObjectsGrammar);
            Trace.WriteLine(referableObjectsGrammar.ToString());

            speechRecognizer.SetInputToDefaultAudioDevice();
            btnDisable.IsEnabled = false;
            showShapesText.Text = getShapesText();

			System.Windows.Point Point1 = new System.Windows.Point(150, 100);
			System.Windows.Point Point2 = new System.Windows.Point(200, 160);
			System.Windows.Point Point3 = new System.Windows.Point(100, 100);
			PointCollection myPointCollection = new PointCollection();
			myPointCollection.Add(Point1);
			myPointCollection.Add(Point2);
			myPointCollection.Add(Point3);

			Shape shape = new Shape(ShapeType.Triangle, System.Windows.Media.Brushes.Blue, Size.Medium, 1, myPointCollection);
			shapes.Add(shape);
			PaintShapes();
		}

        private static Grammar CreateGrammarFromFile()
		{
			Grammar shapesGrammar = new Grammar(@"..\..\grammars\Grammar.xml");
			shapesGrammar.Name = "SRGS File Grammar";
			return shapesGrammar;
		}

		private string getShapesText()
        {
			return "Formas geométricas: \n\n" + "Triangulos: " + triangles.ToString() + "\nCuadrados: " + squares.ToString() + "\nCirculos: " + circles.ToString();
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
		private void PaintShapes()
		{
			ClearCanvas();
			Console.WriteLine("Painting canvas...");
			foreach (Shape shape in this.shapes)
			{
				Console.WriteLine("Painting a shape");
				Console.WriteLine(shape.color);
				Console.WriteLine(shape.points);

				Polygon myPolygon = new Polygon();
				myPolygon.Stroke = System.Windows.Media.Brushes.Black;
				myPolygon.Fill = shape.color;
				myPolygon.StrokeThickness = 2;
				myPolygon.Points = shape.points;
				Canvas_.Children.Add(myPolygon);
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
			int quadrantToSearch1 = -1;
			int quadrantToSearch2 = -1;
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
							case "triangulos":
								shapeToSearch = ShapeType.Triangle;
								isValidStart = true;
								break;
							case "cuadrados":
								shapeToSearch = ShapeType.Square;
								isValidStart = true;
								break;
							case "circulos":
								shapeToSearch = ShapeType.Circle;
								isValidStart = true;
								break;
							default:
								break;
						}
                        if (isValidStart)
                        {
							for (int i = 3; i < e.Result.Words.Count - 1; i++)
							{
								string word = e.Result.Words[i].Text;
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
										string firstPositionWord = e.Result.Words[i+1].Text;
										string secondPositionWord = e.Result.Words[i+2].Text + " " + e.Result.Words[i + 3].Text + " " + e.Result.Words[i + 4].Text;
										switch (firstPositionWord)
                                        {
											case "arriba":
												switch (secondPositionWord)
                                                {
													case "a la izquierda":
														quadrantToSearch1 = 1;
														break;
													case "a la derecha":
														quadrantToSearch1 = 2;
														break;
													default:
														quadrantToSearch1 = 1;
														quadrantToSearch2 = 2;
														break;
												}
												break;
											case "abajo":
												switch (secondPositionWord)
												{
													case "a la izquierda":
														quadrantToSearch1 = 4;
														break;
													case "a la derecha":
														quadrantToSearch1 = 5;
														break;
													default:
														quadrantToSearch1 = 4;
														quadrantToSearch2 = 5;
														break;
												}
												break;
											case "en":
												string centerWord = e.Result.Words[i+2].Text + " " + e.Result.Words[i + 3].Text;
												if (centerWord == "el centro")
													quadrantToSearch1 = 3;
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
				showShapesText.Text = getShapesText();
				foreach (Shape shape in this.shapes)
				{
					if (matchesShape(shape, shapeToSearch, color, size, quadrantToSearch1, quadrantToSearch2))
					{
						this.shapes.Remove(shape);
					}
				}
			}
				
		}

		private Boolean matchesShape(Shape shape, ShapeType shapeToSearch, SolidColorBrush color, Size size, int quadrantToSearch1, int quadrantToSearch2)
        {
			Boolean matchesShape = false;
			Boolean matchesColor = false;
			Boolean matchesSize = false;
			Boolean matchesQuadrant = false;

			if (shapeToSearch == shape.shape)
			{
				matchesShape = true;
			}
            if (color == shape.color || color == System.Windows.Media.Brushes.White)
            {
                matchesColor = true;
            }
			if (size == shape.size || size == Size.None)
			{
				matchesSize = true;
			}
			if (quadrantToSearch1 == shape.quadrant || quadrantToSearch1 == -1 || quadrantToSearch2 == shape.quadrant)
			{
				matchesQuadrant = true;
			}

			return matchesShape && matchesColor && matchesSize && matchesQuadrant;
        }

		private Boolean isInQuadrant (int shapeQuadrant, int quadrantToSearch1, int quadrantToSearch2)
        {
			return quadrantToSearch2 == -1 ? (shapeQuadrant == quadrantToSearch1) : (shapeQuadrant == quadrantToSearch2 || shapeQuadrant == quadrantToSearch1);

		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            speechRecognizer.Dispose();
        }

        private void RichTextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }
    }
}
