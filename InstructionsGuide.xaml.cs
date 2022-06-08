using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Visual_filtering_referable_objects
{
    /// <summary>
    /// Lógica de interacción para InstructionsGuide.xaml
    /// </summary>
    public partial class InstructionsGuide : Window
    {
        public InstructionsGuide(bool isNightMode)
        {
            SolidColorBrush lightModeWindowBackground = new SolidColorBrush(Colors.White);
            SolidColorBrush darkModeWindowBackground = new SolidColorBrush(Color.FromArgb(100, 86, 86, 86));
            InitializeComponent();
            Background = isNightMode ? darkModeWindowBackground : lightModeWindowBackground;
            FirstParagraph.BorderBrush = null;
            SecondParagraph.BorderBrush = null;
            ThirdParagraph.BorderBrush = null;

            if (isNightMode)
            {
                Title.Foreground = new SolidColorBrush(Colors.White);
                FirstParagraph.Foreground = new SolidColorBrush(Colors.White);
                FirstParagraph.Background = darkModeWindowBackground;
                SecondParagraph.Foreground = new SolidColorBrush(Colors.White);
                SecondParagraph.Background = darkModeWindowBackground;
                ThirdParagraph.Foreground = new SolidColorBrush(Colors.White);
                ThirdParagraph.Background = darkModeWindowBackground;
            }
        }

        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void RichTextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }
    }
}
