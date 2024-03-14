using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using Avalonia.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.Primitives;
using System.Timers;
using Avalonia.Media.Imaging;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using TagLib;
using NetCoreAudio;
using System.Drawing;

namespace player
{

    //hgufiugyv
    public partial class MainWindow : Window
    {
        Playlist win = new Playlist();
        string plik = "girls.mp3";
        bool isPlaying = false;
        Player music = new Player();
        byte volume = Convert.ToByte(3);
        private int sekundy = 0;
        System.Timers.Timer timer = new Timer();

        private async void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (music.Playing)
                {
                    sekundy++;
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        slider1.Value++;
                    });
                }
            }
            catch (Exception ex)
            {
                // Obsługa błędu
            }
        }
        

        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = 1000;
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            win.Show();
        }

        public async void btn_open_click(Object sender, RoutedEventArgs e)
{
    var dialog = new OpenFileDialog();
    
    dialog.Title = "Otwórz plik mp3";
    string[] result = await dialog.ShowAsync(this);
    if (result != null)
    {
        if (result != null && result.Length > 0 && result[0] != null)
        {
            await LoadMp3Metadata(result[0]);
            win.Add(System.IO.Path.GetFileName(result[0]));
            //win.muzyka.Items.Add(result[0]);
        }
        mp3.Text = result[0];
        TagLib.File file = TagLib.File.Create(mp3.Text);
        string title2 = await GetMp3Title(result[0]);
        string author2 = await GetMp3Author(result[0]);
        string length = await GetMp3Length(result[0]);
        //win.Add(title2);
       // win.Add(mp3.Text);
        //string filePath = result[0];
        
        switch(title2)
        {
            case "Girls":
            {
                obrazek.Source=new Avalonia.Media.Imaging.Bitmap("Assets/1.jpg");
                break;
            }
            case "Drive BY":
            {
                obrazek.Source=new Avalonia.Media.Imaging.Bitmap("Assets/2.jpg");
                break;
            }
            case "Move On":
            {
                obrazek.Source=new Avalonia.Media.Imaging.Bitmap("Assets/3.jpg");
                break;
            }
        }
        
     

        czas.Text = length;
        Tytul.Text = title2;
        wykonawca.Text = author2;
    }
}


        public void btn1_click(Object sender, RoutedEventArgs e)
        {
            music.Stop().Wait();
        }

        public async void btn2_click(Object sender, RoutedEventArgs e)
        {
            if (!music.Playing)
            {
                if (mp3.Text != null)
                {
                    music.Play(win.playlista.SelectedItem.ToString()).Wait();
                    slider1.Value = 0;
                    btn2.Content = "";
                    timer.Start();
                    string title2 = await GetMp3Title(win.playlista.SelectedItem.ToString());
        string author2 = await GetMp3Author(win.playlista.SelectedItem.ToString());
        string length = await GetMp3Length(win.playlista.SelectedItem.ToString());
        
        
        
        switch(title2)
        {
            case "Girls":
            {
                obrazek.Source=new Avalonia.Media.Imaging.Bitmap("Assets/1.jpg");
                break;
            }
            case "Drive BY":
            {
                obrazek.Source=new Avalonia.Media.Imaging.Bitmap("Assets/2.jpg");
                break;
            }
            case "Move On":
            {
                obrazek.Source=new Avalonia.Media.Imaging.Bitmap("Assets/3.jpg");
                break;
            }
        }
        czas.Text = length;
        Tytul.Text = title2;
        wykonawca.Text = author2;
                }
                else
                {
                    slider1.Value = 0;
                    music.Play(plik).Wait();
                    btn2.Content = "⏸";
                    this.Title = await GetMp3Title(mp3.Text);
                }
            }
            else
            {
                music.Stop().Wait();
                btn2.Content = "▶";
                timer.Stop();
            }
        }

        private async Task<string> GetMp3Title(string filePath)
        {
            try
            {
                var file = await Task.Run(() => TagLib.File.Create(filePath));
                if (file.Tag != null && !string.IsNullOrEmpty(file.Tag.Title))
                {
                    return file.Tag.Title;
                }
                else
                {
                    return "Nieznany tytuł";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd odczytu pliku MP3: {ex.Message}");
                return "Nieznany tytuł";
            }
        }

        private async Task<byte[]> GetMp3Cover(string filePath)
        {
            try
            {
                var file = await Task.Run(() => TagLib.File.Create(filePath));
                if (file.Tag != null && file.Tag.Pictures.Length > 0)
                {
                    var picture = file.Tag.Pictures[0];
                    return picture.Data.Data;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd odczytu pliku MP3: {ex.Message}");
                return null;
            }
        }

        private async Task<string> GetMp3Author(string filePath)
        {
            try
            {
                var file = await Task.Run(() => TagLib.File.Create(filePath));
                if (file.Tag != null && !string.IsNullOrEmpty(file.Tag.FirstPerformer))
                {
                    return file.Tag.FirstPerformer;
                }
                else
                {
                    return "Nieznany autor";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd odczytu pliku MP3: {ex.Message}");
                return "Nieznany autor";
            }
        }

        private async Task<string> GetMp3Length(string filePath)
        {
            try
            {
                var file = await Task.Run(() => TagLib.File.Create(filePath));
                if (file.Properties != null)
                {
                    return $"{file.Properties.Duration:mm\\:ss}";
                }
                else
                {
                    return "Nieznany czas trwania";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd odczytu pliku MP3: {ex.Message}");
                return "Nieznany czas trwania";
            }
        }
        private async Task LoadMp3Metadata(string filePath)
{
    try
    {
        TagLib.File file = await Task.Run(() => TagLib.File.Create(filePath));
        if (file != null)
        {
            string title = string.IsNullOrEmpty(file.Tag.Title) ? "Unknown Title" : file.Tag.Title;
            string artist = string.IsNullOrEmpty(file.Tag.FirstPerformer) ? "Unknown Artist" : file.Tag.FirstPerformer;
            string duration = file.Properties.Duration.ToString(@"mm\:ss");
            // Update UI with metadata
            Tytul.Text = title;
            wykonawca.Text = artist;
            czas.Text = duration;
            // Load cover image if available
            byte[] coverData = file.Tag.Pictures.Length > 0 ? file.Tag.Pictures[0].Data.Data : null;
            // Display cover image in UI
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Error reading MP3 file: {ex.Message}");
    }
}
        
    }
}
