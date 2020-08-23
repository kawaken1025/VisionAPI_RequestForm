using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Google.Cloud.Vision.V1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VisionAPI_RequestForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 画像選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofDialog = new OpenFileDialog();

            // デフォルトのフォルダを指定する
            ofDialog.InitialDirectory = @"C:";
            
            //ダイアログのタイトルを指定する
            ofDialog.Title = "画像選択";

            //ダイアログを表示する
            if (ofDialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = ofDialog.FileName;
                pictureBox1.ImageLocation = textBox1.Text;
            }


            // オブジェクトを破棄する
            ofDialog.Dispose();
        }

        /// <summary>
        /// Auth認証
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReqButton_Click(object sender, EventArgs e)
        {
            var projectId = "massive-runway-229116";
            // If you don't specify credentials when constructing the client, the
            // client library will look for credentials in the environment.
            var credential = GoogleCredential.GetApplicationDefault();
            var storage = StorageClient.Create(credential);
            // Make an authenticated API request.
            var buckets = storage.ListBuckets(projectId);
            foreach (var bucket in buckets)
            {
                Console.WriteLine(bucket.Name);
            }
            RequestVisionAPI();
        }

        /// <summary>
        /// VISION API へリクエストを送信
        /// </summary>
        private void RequestVisionAPI()
        {
            // 画像からリクエスト情報を生成
            if (textBox1.Text.Length == 0) return;
            var client = ImageAnnotatorClient.Create();
            var image = Google.Cloud.Vision.V1.Image.FromFile(textBox1.Text);
            var response = client.DetectText(image);

            var resultText = "";
            foreach (EntityAnnotation text in response)
            {
                //認識結果、言語、認識結果の各頂点を標準出力に表示
                var coordinates = string.Join(",", text.BoundingPoly.Vertices.Select(v => $"({v.X},{v.Y})"));
                Console.WriteLine($"Description: {text.Description} Locale:{text.Locale} coordinates:{coordinates}");
                resultText += text.Description;
            }
            Console.WriteLine();
            // 画面へ結果を表示
            resultBox.Text = resultText;

        }
    }
}
