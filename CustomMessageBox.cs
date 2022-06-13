using System.Speech.Synthesis;
using System.Windows.Controls;

namespace Visual_filtering_referable_objects
{
    internal class CustomMessageBox
    {
        public static bool isActivated = true;
        public static RichTextBox textBlock;
        public static bool speakerActivated = true;
        public static SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();

        public static void Show(string text)
        {
            if (isActivated)
            {
                System.Windows.MessageBox.Show(text);
            }
        }

        public static void SetActivated(bool setter)
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
            if (!speakerActivated)
            {
                speechSynthesizer.SpeakAsyncCancelAll();
            }
        }
    }
}
