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

		private int triangles = 4;
        private int squares = 3;
        private int circles = 4;

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


			// Add a Rectangle Element
			
			Rectangle myRect = new System.Windows.Shapes.Rectangle();
			myRect.Stroke = System.Windows.Media.Brushes.Black;
			myRect.Fill = System.Windows.Media.Brushes.SkyBlue;
			myRect.HorizontalAlignment = HorizontalAlignment.Left;
			myRect.VerticalAlignment = VerticalAlignment.Center;
			myRect.Height = 50;
			myRect.Width = 50;
			Canvas_.Children.Add(myRect);
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

		private void speechRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
		{
			Button_Click_1(null, null);
			Shape shapeToSearch = new Shape();
			string location = "";
			string shape = "";
			string color = "";
			string size = "";
			string startCommand = "";
			bool isValidStart = false;

			if (e.Result.Words.Count>2)
			{
				startCommand = e.Result.Words[0].Text.ToLower() + " " + e.Result.Words[1].Text.ToLower();
				shape = e.Result.Words[2].Text.ToLower();
				string adjetive = e.Result.Words.Count > 3 ? e.Result.Words[3].Text.ToLower() : null;
                string secondCommand = e.Result.Words.Count > 4 ? e.Result.Words[4].Text.ToLower() + " " + (e.Result.Words.Count > 4 ? e.Result.Words[5].Text.ToLower() : "") : null;
				string wholeText = "";
				for (int i = 0; i < e.Result.Words.Count; i++)
                {
					wholeText = wholeText + ' ' + e.Result.Words[i].Text;
                }
				MessageBox.Show("Has hablado! Has dicho esto: " + wholeText);

				switch (startCommand)
				{
					case "borra los":
						switch (shape)
						{
							case "triangulos":
								shapeToSearch.shape = ShapeType.Triangle;
								isValidStart = true;
								break;
							case "cuadrados":
								shapeToSearch.shape = ShapeType.Square;
								isValidStart = true;
								break;
							case "circulos":
								shapeToSearch.shape = ShapeType.Circle;
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
							}

						}
						shapeToSearch = setAdjetives(shapeToSearch, adjetive);
						switch (secondCommand)
                        {
							case "que estén":
								string[] locationStrings = new string[2];
								locationStrings[0] = e.Result.Words[6].Text.ToLower();
								locationStrings[1] = e.Result.Words.Count > 7 ? e.Result.Words[9].Text.ToLower() : "";
								shapeToSearch = setLocation(shapeToSearch, locationStrings[0]);
								shapeToSearch = setLocation(shapeToSearch, locationStrings[1]);
								break;
							case "que sean":
								string secondAdjetive = e.Result.Words[6].Text.ToLower();
								shapeToSearch = setAdjetives(shapeToSearch, secondAdjetive);
								break;
							default:
								break;
						}
						break;
				}
			}
			showShapesText.Text = getShapesText();
		}

		private Shape setLocation (Shape originalShape, string locationString)
        {
			switch (locationString)
			{
				case "arriba":
					originalShape.location_y = LocationY.Top;
					break;
				case "abajo":
					originalShape.location_y = LocationY.Bottom;
					break;
				case "derecha":
					originalShape.location_x = LocationX.Right;
					break;
				case "izquierda":
					originalShape.location_x = LocationX.Left;
					break;
				default:
					break;
			}
			return originalShape;
		}

		private Shape setAdjetives(Shape originalShape, string adjetive)
		{
			switch (adjetive)
			{
				case "azules":
					originalShape.color = Color.Blue;
					break;
				case "negros":
					originalShape.color = Color.Black;
					break;
				case "rojos":
					originalShape.color = Color.Red;
					break;
				case "morados":
					originalShape.color = Color.Purple;
					break;
				case "amarillos":
					originalShape.color = Color.Yellow;
					break;
				case "verdes":
					originalShape.color = Color.Green;
					break;
				case "naranjas":
					originalShape.color = Color.Orange;
					break;
				case "rosas":
					originalShape.color = Color.Pink;
					break;
				case "grandes":
					originalShape.size = Size.Big;
					break;
				case "medianos":
					originalShape.size = Size.Medium;
					break;
				case "pequeños":
					originalShape.size = Size.Small;
					break;
				default:
					break;
			}
			return originalShape;
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
