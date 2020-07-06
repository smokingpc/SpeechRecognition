using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using NAudio;
using NAudio.Wave;
using System.Speech.Recognition;  
using System.Speech.AudioFormat; 

namespace MicrosoftSpeech
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var installed = SpeechRecognitionEngine.InstalledRecognizers();
            foreach (var item in installed)
            {
                textBox2.AppendLine("installed engine: " + item.Name);
            }

            OpenFileDialog dlg = new OpenFileDialog() { Filter = "WAV File (*.wav)|*.wav" };

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            string filepath = textBox1.Text = dlg.FileName;

            if (textBox1.Text == "")
            {
                MessageBox.Show("Please select WAV speech file first!");
                return;
            }

            if (false == File.Exists(filepath))
            {
                MessageBox.Show("Specified WAV file is not EXIST!!");
                return;
            }

            //reference URL : https://docs.microsoft.com/zh-tw/dotnet/api/system.speech.recognition.speechrecognitionengine?view=netframework-4.7.2
            using (var recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("zh-TW")))
            {
                // Create and load a dictation grammar.  
                recognizer.LoadGrammar(new DictationGrammar());
                // Add a handler for the speech recognized event.  
                recognizer.SpeechRecognized +=
                  new EventHandler<SpeechRecognizedEventArgs>(Event_SpeechRecognized);

                // Configure input to the speech recognizer.  
                //recognizer.SetInputToDefaultAudioDevice();
                recognizer.SetInputToWaveFile(filepath);
                
                // Start asynchronous, continuous speech recognition.  
                //recognizer.RecognizeAsync(RecognizeMode.Single);
                recognizer.Recognize();
                //// Keep the console window open.  
                //while (true)
                //{
                //    Console.ReadLine();
                //}
            }

        }

        private void Event_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            textBox2.AppendLine("[recognize result]" + e.Result.Text);
        }
    }
}
