using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Speech.Synthesis;

namespace Visual_filtering_referable_objects
{
    internal class CustomMessageBox
    {
		public static Boolean isActivated = true;
		public static RichTextBox textBlock;
		public static Boolean speakerActivated = true;
		public static SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();

		public static void Show(string text)
		{
			if (isActivated)
			{
				System.Windows.MessageBox.Show(text);
			}
		}

		public static void SetActivated(Boolean setter)
		{
			isActivated = setter;
		}

		public static void SetTextField(RichTextBox textBlockReference)
        {
			textBlock = textBlockReference;
        }

		public static void AddTextSystem(string text)
        {
			textBlock.AppendText("Sistema: " + text + "\n");
			if (speakerActivated) speechSynthesizer.SpeakAsync(text);
		}
		public static void AddTextUser(string text)
        {
			textBlock.AppendText("Usuario: " + text + "\n");
		}
		public static void ToggleSpeaker()
        {
			speakerActivated = !speakerActivated;
		}
	}
}
