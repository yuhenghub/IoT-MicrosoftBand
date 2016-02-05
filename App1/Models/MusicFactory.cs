using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace App1.Models
{
    class MusicFactory
    {
        //private static string angerMusciUri = "";
        //private static string ContemptMusciUri = "";
        //private static string DisgustMusciUri = "";
        //private static string FearMusciUri = "";
        //private static string HappinessMusciUri = "";
        //private static string NeutralMusciUri = "";
        //private static string SadnessMusciUri = "";
        //private static string SurpriseMusciUri = "";


        private  MediaElement player;
        private  string state;

        public MusicFactory(MediaElement me) {
            player = me;
            state = "";
        }

        public void playnext(String emotion)
        {

            if (state.Equals(emotion))
                return;
            else
                state = emotion;
            switch (emotion)
            {
                case "Anger":
                    player.Source = new Uri("ms-appx://ms-appx:///Assets/EmotionMusic/Anger.mp3");
                    break;
                case "Contempt":
                    player.Source = new Uri("ms-appx:///Assets/EmotionMusic/Contempt.mp3");
                    break;
                case "Disgust":
                    player.Source = new Uri("ms-appx:///Assets/EmotionMusic/Disgust.mp3");
                    break;
                case "Fear":
                    player.Source = new Uri("ms-appx:///Assets/EmotionMusic/Fear.mp3");
                    break;
                case "Happiness":
                    player.Source = new Uri("ms-appx:///Assets/EmotionMusic/Happiness.mp3");
                    break;
                case "Neutral":
                    player.Source = new Uri("ms-appx:///Assets/EmotionMusic/Neutral.mp3");
                    break;
                case "Sadness":
                    player.Source = new Uri("ms-appx:///Assets/EmotionMusic/Sadness.mp3");
                    break;
                case "Surprise":
                    player.Source = new Uri("ms-appx:///Assets/EmotionMusic/Surprise.mp3");
                    break;
            }
            player.Play();
            return;

        }
    }
}
