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
using System.Windows.Shapes;
using System.Speech.Recognition;
using System.Globalization;

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
			//Choices colorChoices = new Choices("rojos", "azules", "amarillos", "verdes", "negros");
			pluralGrammar.Append(shapesChoices);
			//grammarBuilder.Append(colorChoices);

			GrammarBuilder singularGrammar = new GrammarBuilder("borra un");
			Choices shapeChoices = new Choices("triangulo", "cuadrado", "circulo");
			//Choices colorChoices = new Choices("rojo", "azul", "amarillo", "verde", "negro");
			singularGrammar.Append(shapeChoices);
			//grammarBuilder.Append(colorChoices);

			speechRecognizer.LoadGrammar(new Grammar(pluralGrammar));
			speechRecognizer.LoadGrammar(new Grammar(singularGrammar));
			speechRecognizer.SetInputToDefaultAudioDevice();
			btnDisable.IsEnabled = false;
			showShapesText.Text = getShapesText();
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
			if (e.Result.Words.Count == 3)
			{
				MessageBox.Show("Has hablado! Has dicho esto: " + e.Result.Words[0].Text +" "+ e.Result.Words[1].Text + " "+ e.Result.Words[2].Text);
				string command = e.Result.Words[0].Text.ToLower() + " " + e.Result.Words[1].Text.ToLower();
				string shape = e.Result.Words[2].Text.ToLower();
				switch (command)
				{
					case "borra los":
						switch (shape)
						{
							case "triangulos":
								MessageBox.Show("Borrando los triangulos...");
								triangles = 0;
								break;
							case "cuadrados":
								MessageBox.Show("Borrando los cuadrados...");
								squares = 0;
								break;
							case "circulos":
								MessageBox.Show("Borrando los circulos...");
								circles = 0;
								break;
						}
						break;
					case "borra un":
						switch (shape)
						{
							case "triangulo":
								MessageBox.Show("Borrando un triangulo...");
								triangles--;
								break;
							case "cuadrado":
								MessageBox.Show("Borrando un cuadrado...");
								squares--;
								break;
							case "circulo":
								MessageBox.Show("Borrando un circulo...");
								circles--;
								break;
						}
						break;
				}
			}
			showShapesText.Text = getShapesText();
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
