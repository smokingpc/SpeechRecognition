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
using System.Net.Http;

using System.Net.Http.Headers;

namespace Microsoft_CognitiveServices
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
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
        }

        private Task<string> FetchAccessToken(string region, string key)
        {
            using (var client = new HttpClient())
            {
                string url = $"https://{region}.api.cognitive.microsoft.com/sts/v1.0";
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
                UriBuilder uriBuilder = new UriBuilder(url);
                uriBuilder.Path += "/issueToken";
                //string msg = string.Format("Token Uri: {0} , key: [{1}]", uriBuilder.Uri.AbsoluteUri, key);
                //textBox2.AppendLine(msg);

                var content = new FormUrlEncodedContent(new Dictionary<string, string>());
                var post = client.PostAsync(uriBuilder.Uri.AbsoluteUri, content);

                var token = post.Result.Content.ReadAsStringAsync();
                return token;
            }
        }


        //language should use BCP-47 format, e.g.  en-US, zh-TW...etc.
        private Task<string> RecognizeSpeech(string access_tokrn, string region, string language, string filename)
        {
            // curl -v -X POST  "https://westus.api.cognitive.microsoft.com/sts/v1.0/issueToken" -H "Content-Length: 0" -H "Ocp-Apim-Subscription-Key: 245f167a4a3b4ee6a74716905a34fde5"
            // curl -v -X POST "https://westus.stt.speech.microsoft.com/speech/recognition/interactive/cognitiveservices/v1?language=en-US" -H "Authorization: Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJyZWdpb24iOiJ3ZXN0dXMiLCJzdWJzY3JpcHRpb24taWQiOiJkMTM2OTY1OTE3MWI0ZDI1OGYyYmIzN2E0NDRkZWEyZSIsInByb2R1Y3QtaWQiOiJTcGVlY2hTZXJ2aWNlcy5GcmVlIiwiY29nbml0aXZlLXNlcnZpY2VzLWVuZHBvaW50IjoiaHR0cHM6Ly9hcGkuY29nbml0aXZlLm1pY3Jvc29mdC5jb20vaW50ZXJuYWwvdjEuMC8iLCJhenVyZS1yZXNvdXJjZS1pZCI6IiIsInNjb3BlIjoic3BlZWNoc2VydmljZXMiLCJhdWQiOiJ1cm46bXMuc3BlZWNoc2VydmljZXMud2VzdHVzIiwiZXhwIjoxNTk0MDI0NTIwLCJpc3MiOiJ1cm46bXMuY29nbml0aXZlc2VydmljZXMifQ.xNXP4pykZsjJvRAmlwVO0OnWXLALic_Tw2UQ25ZupEc" -H "Transfer-Encoding: chunked" -H "Content-type: audio/wav; codec=audio/pcm; samplerate=16000"  -H "Content-Length: 65004"  --data-binary @"./test.wav"
            //trial REST can only recognize 15second WAV.
            //if want to recognize longer speech, should pay money....
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", access_tokrn);
                var filedata = File.ReadAllBytes(filename);
                var content = new ByteArrayContent(filedata);
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/wav");

                string url = $"https://{region}.stt.speech.microsoft.com/speech/recognition/interactive/cognitiveservices/v1?language={language}";
                UriBuilder uriBuilder = new UriBuilder(url);
                var post = client.PostAsync(uriBuilder.Uri.AbsoluteUri, content);

                var result = post.Result.Content.ReadAsStringAsync();
                return result;
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            //string region = "westus";
            //string language = "zh-TW";        //speech languate
            string region = comboBox1.Text;
            string language = comboBox2.Text;
            string key = textBox3.Text;
            string filepath = textBox1.Text;

            var token = await FetchAccessToken(region, key);
            //string msg = string.Format("got token [{0}]", token);
            //textBox2.AppendLine(msg);
            //textBox2.AppendLine("<--------------------------------------------------------->");

            //SDK怪怪的，直接呼叫 REST API 反而會通.........!@$@#%$#@%#
            string speech = await RecognizeSpeech(token, region, language, filepath);

            textBox2.AppendLine("Recognize Result = " + speech);
            textBox2.AppendLine("<========================================================>");
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 1;
        }
    }
}
