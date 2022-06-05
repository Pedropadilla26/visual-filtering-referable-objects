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
using System.Windows.Shapes;

namespace Visual_filtering_referable_objects
{
    /// <summary>
    /// Lógica de interacción para InstructionsGuide.xaml
    /// </summary>
    public partial class InstructionsGuide : Window
    {
        public InstructionsGuide(Boolean isNightMode)
        {
            SolidColorBrush lightModeWindowBackground = new SolidColorBrush(Colors.White);
            SolidColorBrush darkModeWindowBackground = new SolidColorBrush(Color.FromArgb(100, 86, 86, 86));
            InitializeComponent();
            this.Background = isNightMode ? darkModeWindowBackground : lightModeWindowBackground;
            this.FirstParagraph.BorderBrush = null;
            this.SecondParagraph.BorderBrush = null;
            this.ThirdParagraph.BorderBrush = null;

            if (isNightMode)
            {
                this.Title.Foreground = new SolidColorBrush(Colors.White);
                this.FirstParagraph.Foreground = new SolidColorBrush(Colors.White);
                this.FirstParagraph.Background = darkModeWindowBackground;
                this.SecondParagraph.Foreground = new SolidColorBrush(Colors.White);
                this.SecondParagraph.Background = darkModeWindowBackground;
                this.ThirdParagraph.Foreground = new SolidColorBrush(Colors.White);
                this.ThirdParagraph.Background = darkModeWindowBackground;
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
