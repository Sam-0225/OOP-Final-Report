using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Media.Capture;
using Windows.Storage;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;

//空白頁項目範本收錄在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App2{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class MainPage : Page {
        CameraCaptureUI captureUI = new CameraCaptureUI();
        StorageFile photo;
        IRandomAccessStream imageStream;

        const string APIKEY = "b557e7c8539342fca8d71af7bdf20381";
        EmotionServiceClient emotionServiceClient = new EmotionServiceClient(APIKEY);
        Emotion[] emotionResult;

        public MainPage(){
            this.InitializeComponent();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Size(300, 300);
        }
        
        private async void takePhoto_Click(object sender, RoutedEventArgs e) {
            try {
                photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

                if(photo == null) {
                    return;
                }else {
                    imageStream = await photo.OpenAsync(FileAccessMode.Read);
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(imageStream);
                    SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();

                    SoftwareBitmap softwareBitmapBgra8 = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                    SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
                    await bitmapSource.SetBitmapAsync(softwareBitmapBgra8);

                    image.Source = bitmapSource; 
                }
            } catch {
                output.Text = "Error taking photo";
            }
        }

        private async void getEmotion_Click(object sender, RoutedEventArgs e) {
            try {
                emotionResult = await emotionServiceClient.RecognizeAsync(imageStream.AsStream());

                if (emotionResult != null) {
                    Scores score = emotionResult[0].Scores;
                    double Happiness = score.Happiness*10000;
                    double Sadness = score.Sadness * 10000;
                    double Surprise = score.Surprise * 10000;
                    double Fear = score.Fear * 10000;
                    double Anger = score.Anger * 10000;
                    double Contempt = score.Contempt * 10000;
                    double Disgust = score.Disgust * 10000;
                    double Neutral = score.Neutral * 10000;
                    output.Text = "Your emotion are: \nThe value of Emotion range from 0 to 10000\n\n" +
                                  "\tHappiness\t: " + (int) Happiness + "\n" +
                                  "\tSadness\t: " + (int) Sadness + "\n" +
                                  "\tSurprise\t: " + (int) Surprise + "\n" +
                                  "\tFear\t\t: " + (int) Fear + "\n" +
                                  "\tAnger\t\t: " + (int) Anger + "\n" +
                                  "\tContempt\t: " + (int) Contempt + "\n" +
                                  "\tDisgust\t: " + (int) Disgust + "\n\n" +
                                  "\tNeutral\t: " + (int) Neutral + "\n"; 
                }

            }catch {
                output.Text = "Error returning the emotion";
            }
        }
        
    } 
}
