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
using Google.Cloud.Speech.V1;

namespace Google_SpeechRecognition
{
    //to use Google Speech API, need several steps:
    //1. goto https://console.cloud.google.com/home/ to create a new cloud project.
    //2. goto https://console.cloud.google.com/apis/ to activate API you want to use.
    //   ** Each account can free recognize 60min(total) length speech per month. 
    //      You have to pay money if want to recognize longer time.
    //3. goto https://console.cloud.google.com/apis/credentials to create API key and authentication
    //4. I use "service account" to authenticate , so download key (in JSON file) 
    //5. set environment variable to this JSON file.
    //   in windows, it's 
    //      GOOGLE_APPLICATION_CREDENTIALS = "{%PATH}\{%JSON_KEY_FILE}"
    //6. in VS project, install Google.Cloud.Speech.V1 package from nuget
    //Then you can try it!

    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void SelectFile()
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                Filter = "WAV file (*.wav)|*.wav"
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = dlg.FileName;
            }
        }

        private void RecognizeFile(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                MessageBox.Show("Please select a wav file first!");
                return;
            }

            if (File.Exists(file) == false)
            {
                MessageBox.Show("Specified WAV file is NOT exist! Please try to select another file...");
                return;
            }

            SpeechClient client = SpeechClient.Create();
            RecognitionConfig config = new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRateHertz = 8000,
                LanguageCode = "zh-TW",
            };
            RecognitionAudio audio = RecognitionAudio.FromFile(file);
            var response = client.Recognize(config, audio);

            foreach (var item in response.Results)
            {
                textBox2.AppendLine(item.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelectFile();
            RecognizeFile(textBox1.Text);
        }
    }
}
