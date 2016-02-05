using System;
using Windows.UI.Xaml.Controls;
using Microsoft.Azure.Devices.Client;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using System.IO;
using System.Threading.Tasks;
using App1.Models;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.Storage.Streams;
using Microsoft.WindowsAzure.Storage.Blob;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "liusjIoTHub.azure-devices.net";
        static string deviceKey = "hhiv1QVbLF7NejItxRJxQuZDD20fEff+HCv3u35mARs=";
        public String imageOnlinePath { get; set; }
        public int imageIndex = 0;
        public MainPage()
        {
            this.InitializeComponent();


            Debug.WriteLine("Simulated device\n");

            try
            {
                deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("myFirstDevice", deviceKey), TransportType.Http1);
            }
            catch (Exception e)
            {
                string meg = e.Message;
            }
            ReceiveC2dAsync();

        }

        private async void ReceiveC2dAsync()
        {
            Debug.WriteLine("\nReceiving cloud to device messages from service");
            while (true)
            {
                Message receivedMessage = null;
                try
                {
                    receivedMessage = await deviceClient.ReceiveAsync();
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                }
                if (receivedMessage == null) continue;
                string recmsg = Encoding.ASCII.GetString(receivedMessage.GetBytes());

                Debug.WriteLine(string.Format("Received message: {0}", recmsg));
                await deviceClient.CompleteAsync(receivedMessage);
                addImage(recmsg);

                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("picture.jpg", CreationCollisionOption.ReplaceExisting);


                CloudBlockBlob blockBlob = new CloudBlockBlob(new Uri(recmsg));
                // Save blob contents to a file.
                await blockBlob.DownloadToFileAsync(file);
                //shows here
                //var rass = RandomAccessStreamReference.CreateFromUri(new Uri(recmsg));
                //IRandomAccessStream inputStream = await rass.OpenReadAsync();
                //Stream input = WindowsRuntimeStreamExtensions.AsStreamForRead(inputStream.GetInputStreamAt(0));
                String source = "https://faceofficestorage.blob.core.windows.net/iothubwechatstore/PsLNCDnH5KJ4JMERRbVBTvlj3lxcC2Q-eyavDxYV8pjK3bEoFFECOTvWFO_ODEfx.jpg?sv=2015-02-21&sr=b&sig=JvaTbJ3sEzTPcCNDhnOnUtwvAqEft7l1CsX6Eg9r308%3D&st=2016-01-26T11%3A14%3A38Z&se=2016-01-31T11%3A19%3A38Z&sp=rd";
                //SaveStreamToFile("ms-appx:///Assets/Images/image.jpg", input);
                Windows.Storage.Streams.IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read) ;
                    changeEmotion(fileStream.AsStream());
            }
        }
        public void SaveStreamToFile(string fileFullPath, Stream stream)
        {
            //if (stream.Length == 0) return;

            // Create a FileStream object to write a stream to a file
            using (FileStream fileStream = System.IO.File.Create(fileFullPath, (int)stream.Length))
            {
                // Fill the bytes[] array with the stream data
                byte[] bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, (int)bytesInStream.Length);

                // Use FileStream object to write to the specified file
                fileStream.Write(bytesInStream, 0, bytesInStream.Length);
            }
        }



        private void addImage(string recmsg)
        {
            switch (imageIndex)
            {
                case 0:
                    this.UIimage1.Source = new BitmapImage(new Uri(recmsg));
                    imageIndex = (imageIndex + 1) % 4;
                    break;
                case 1:
                    this.UIimage2.Source = new BitmapImage(new Uri(recmsg));
                    imageIndex = (imageIndex + 1) % 4;
                    break;
                case 2:
                    this.UIimage3.Source = new BitmapImage(new Uri(recmsg));
                    imageIndex = (imageIndex + 1) % 4;
                    break;
                case 3:
                    this.UIimage4.Source = new BitmapImage(new Uri(recmsg));
                    imageIndex = (imageIndex + 1) % 4;
                    break;
                default:
                    this.UIimage1.Source = new BitmapImage(new Uri(recmsg));
                    imageIndex = 1;
                    break;
            }
        }

        private async Task<Emotion[]> UploadAndDetectEmotions(Stream imageFilePath)
        {

            EmotionServiceClient emotionServiceClient = new EmotionServiceClient("0de934b0f96b430dbbead0e37881a56e");
            try
            {
                Emotion[] emotionResult;
                emotionResult = await emotionServiceClient.RecognizeAsync(imageFilePath);

                //http://www.merakoh.com/wp-content/uploads/2013/04/baby-faces-4.jpg

                return emotionResult;

            }
            catch (Exception exception)
            {
                statusbar.Text = "No emotion is detected. This might be due to:\n" +
                    "    image is too small to detect faces\n" +
                    "    no faces are in the images\n" +
                    "    faces poses make it difficult to detect emotions\n" +
                    "    or other factors";
                return null;
            }

        }

        private async void changeEmotion(Stream imageFilePath)
        {
            Emotion[] emotionResult = await UploadAndDetectEmotions(imageFilePath);

            if (emotionResult != null)
            {
                EmotionResultDisplay[] resultDisplay = new EmotionResultDisplay[8];
                for (int i = 0; i < emotionResult.Length; i++)
                {
                    Emotion emotion = emotionResult[i];
                    resultDisplay[0] = new EmotionResultDisplay { EmotionString = "Anger", Score = emotion.Scores.Anger };
                    resultDisplay[1] = new EmotionResultDisplay { EmotionString = "Contempt", Score = emotion.Scores.Contempt };
                    resultDisplay[2] = new EmotionResultDisplay { EmotionString = "Disgust", Score = emotion.Scores.Disgust };
                    resultDisplay[3] = new EmotionResultDisplay { EmotionString = "Fear", Score = emotion.Scores.Fear };
                    resultDisplay[4] = new EmotionResultDisplay { EmotionString = "Happiness", Score = emotion.Scores.Happiness };
                    resultDisplay[5] = new EmotionResultDisplay { EmotionString = "Neutral", Score = emotion.Scores.Neutral };
                    resultDisplay[6] = new EmotionResultDisplay { EmotionString = "Sadness", Score = emotion.Scores.Sadness };
                    resultDisplay[7] = new EmotionResultDisplay { EmotionString = "Surprise", Score = emotion.Scores.Surprise };

                    Array.Sort(resultDisplay, delegate (EmotionResultDisplay result1, EmotionResultDisplay result2)
                    {
                        return ((result1.Score == result2.Score) ? 0 : ((result1.Score < result2.Score) ? 1 : -1));
                    });
                }

                statusbar.Text = resultDisplay[0].ToString();
                BGchange(resultDisplay[0].EmotionString);
                MusicFactory music = new MusicFactory(this.player);
                music.playnext(resultDisplay[0].EmotionString);
            }

        }

        private void BGchange(String emotion)
        {
            switch (emotion) {
                case "Anger":
                    background.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/EmotionImages/Anger.jpg", UriKind.Relative));
                    break;
                case "Contempt":
                    background.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/EmotionImages/Contempt.jpg", UriKind.Relative));
                    break;
                case "Disgust":
                    background.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/EmotionImages/Disgust.jpg", UriKind.Relative));
                    break;
                case "Fear":
                    background.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/EmotionImages/Fear.jpg", UriKind.Relative));
                    break;
                case "Happiness":
                    background.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/EmotionImages/Happiness.jpg"));
                    break;
                case "Neutral":
                    background.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/EmotionImages/Neutral.jpg"));
                    break;
                case "Sadness":
                    background.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/EmotionImages/Sadness.jpg", UriKind.Relative));
                    break;
                case "Surprise":
                    background.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/EmotionImages/Surprise.jpg", UriKind.Relative));
                    break;

            }


        }

        internal class EmotionResultDisplay
        {
            public string EmotionString
            {
                get;
                set;
            }
            public float Score
            {
                get;
                set;
            }
            public override string ToString()
            {
                return this.EmotionString + ":" + this.Score.ToString("0.000000");
            }
        }

    }
}
