
using Gma.System.MouseKeyHook;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;

namespace DoAn3_WinDow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool mediaPlayerIsPlaying = false;
        private bool userIsDraggingSlider = false;
        private bool mediaPlayRandom = false;
        private int mediaPlayLoop = 1;
        private List<Song> ListSong = null;
        private MediaPlayer mePlayer = new MediaPlayer();
        private IKeyboardMouseEvents _hook;

        public MainWindow()
        {
            InitializeComponent();
            ListSong = new List<Song>();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;

            timer.Start();

            _hook = Hook.GlobalEvents();
            _hook.KeyUp += _hook_KeyUp;
        }

        private void _hook_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if(e.KeyCode == Keys.F7)
            {
                ImagesStart_MouseDown(null, null);
            }
            else if (e.KeyCode == Keys.F6)
            {
                imagesPriveous_MouseDown(null,null);
            }
            else if (e.KeyCode == Keys.F8)
            {
                imagesNext_MouseDown(null,null);
            }
            e.Handled = true;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            spListMusic.Height = ActualHeight / 15 * 13 - 30;
            if ((mePlayer.Source != null) && (mePlayer.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
            {
                sliProgress.Minimum = 0;
                sliProgress.Maximum = mePlayer.NaturalDuration.TimeSpan.TotalSeconds;
                sliProgress.Value = mePlayer.Position.TotalSeconds;
                maxTimer.Text = String.Format("{0}", mePlayer.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss"));
            }
            if (mePlayer.Position == mePlayer.NaturalDuration)
                RandomAndLoop();
        }

        private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }

        private void sliProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            mePlayer.Position = TimeSpan.FromSeconds(sliProgress.Value);
        }

        private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblProgressStatus.Text = TimeSpan.FromSeconds(sliProgress.Value).ToString(@"hh\:mm\:ss");
        }

        private bool IsExist(string str1)
        {

            for (var i = 0; i < actionsListBox.Items.Count; i++)
            {
                if (String.Compare(str1, ListSong[i].Path, false) == 0)
                {

                    return true;
                }
            }
            return false;
        }

        private void ChooseFileButton_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            string name;
            if (openFileDialog.ShowDialog() == true)
            {
                name = openFileDialog.FileName;
                if (true)//chu y kiem tr trung bai hat
                {
                    string[] arrname = name.Split(new char[] { '\\', '.' });
                    Song s = new Song();
                    s.Name = arrname[arrname.Length - 2];
                    s.Path = name;
                    if (!IsExist(name))
                    {
                        ListSong.Add(s);
                        actionsListBox.Items.Add(s);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("The song already exists! ");
                    }
                  
                    var bc = new BrushConverter();
                    actionsListBox.Background = (Brush)bc.ConvertFrom("#1A1C29");
                }
            }
            //Title = ListSong.Count.ToString();
        }

        private void ImagesStart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(ListSong != null && actionsListBox.SelectedItem != null)
            {
                if (!mediaPlayerIsPlaying)
                {
                    mePlayer.Play();
                    mediaPlayerIsPlaying = true;
                    imagesStart.Source = new BitmapImage(new Uri(@"Images\Pause.ico", UriKind.Relative));
                    SelectesSongPlay();
                }
                else
                {
                    mePlayer.Pause();
                    mediaPlayerIsPlaying = false;
                    imagesStart.Source = new BitmapImage(new Uri(@"Images\Start.ico", UriKind.Relative));
                }
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) { }

        private void actionsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //mePlayer.Stop();
            //SelectesSongPlay();
        }

        private void SelectesSongPlay()
        {
            if (actionsListBox.SelectedIndex < actionsListBox.Items.Count)
            {
                var song = actionsListBox.SelectedItem as Song;
                lbNameMusic.Text = song.Name;
                mePlayer.Open(new Uri(song.Path));
                imagesStart.Source = new BitmapImage(new Uri(@"Images\Pause.ico", UriKind.Relative));
                mePlayer.Play();
            }
        }

        int count = 0;
        int[] temp = null;
        private List<int> Listint = new List<int>();

        private void RandomAndLoopSong()
        {
            //tao mot mang gom n phan tu
            if (count != actionsListBox.Items.Count)
            {
                temp = new int[actionsListBox.Items.Count];
                for (int i = 0; i < temp.Length; i++)
                    temp[i] = i;
            }
            if (mediaPlayRandom && mediaPlayLoop == 2) //nga^u nhie^n va` lap. vo`ng 
            {
                Random rd = new Random();
                actionsListBox.SelectedIndex = rd.Next(0, temp.Length - 1);
                SelectesSongPlay();
                return;
            }
        }

        private void LoopSong()
        {
            if (mediaPlayLoop == 0)//trang thai lap 1 bai hat
            {
                SelectesSongPlay();
            }
            else
            {
                if (actionsListBox.SelectedIndex + 1 < actionsListBox.Items.Count && !mediaPlayRandom)
                    actionsListBox.SelectedIndex++;
                else
                {
                    if (mediaPlayLoop == 2 && !mediaPlayRandom) // trang thai lap vong
                    {
                        actionsListBox.SelectedIndex = 0;
                        SelectesSongPlay();
                    }
                    if (mediaPlayLoop == 1 && !mediaPlayRandom)  // trang thay phat 1 danh sach
                    {
                        mePlayer.Stop();
                        mediaPlayerIsPlaying = false;
                        imagesStart.Source = new BitmapImage(new Uri(@"Images\Start.ico", UriKind.Relative));
                    }
                }
            }
        }

        private void RandomAndListSong()
        {
            if (count != actionsListBox.Items.Count)
            {
                count = actionsListBox.Items.Count;
                for (int i = 0; i < count; i++)
                    Listint.Add(i);
            }
            if (mediaPlayRandom && mediaPlayLoop == 1) //nga^u nhie^n va` pha't he^'t 1 danh sa'ch
            {
                if (Listint.Count == 0)
                {
                    mePlayer.Stop();
                    mediaPlayerIsPlaying = false;
                    imagesStart.Source = new BitmapImage(new Uri(@"Images\Start.ico", UriKind.Relative));
                    count = 0;
                    return;
                }
                Random rd = new Random();
                int temp = rd.Next(0, Listint.Count - 1);
                actionsListBox.SelectedIndex = Listint[temp];
                Listint[temp] = Listint[Listint.Count - 1];
                Listint.RemoveAt(Listint.Count - 1);
                SelectesSongPlay();
                return;
            }
        }

        private void RandomAndLoop()
        {
            RandomAndLoopSong();
            RandomAndListSong();
            LoopSong();
        }

        private void volumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mePlayer.Volume = (double)volumeSlider.Value * 0.2;
        }

        private void imagesPriveous_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (actionsListBox.SelectedIndex + 1 < actionsListBox.Items.Count)
                actionsListBox.SelectedIndex++;
            else
                actionsListBox.SelectedIndex = 0;
            SelectesSongPlay();
        }

        private void imagesNext_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RandomAndLoop();
            //if (actionsListBox.SelectedIndex > 0)
            //    actionsListBox.SelectedIndex--;
            //else
            //    actionsListBox.SelectedIndex = actionsListBox.Items.Count - 1;
            //SelectesSongPlay();
        }

        private void imagesLoop_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (mediaPlayLoop == 0)
            {
                mediaPlayLoop = 1;
                imagesLoop.Source = new BitmapImage(new Uri(@"Images\Loop_Out.ico", UriKind.Relative));
            }
            else if (mediaPlayLoop == 1)
            {
                mediaPlayLoop = 2;
                imagesLoop.Source = new BitmapImage(new Uri(@"Images\Loop_On.ico", UriKind.Relative));
            }
            else
            {
                mediaPlayLoop = 0;
                imagesLoop.Source = new BitmapImage(new Uri(@"Images\Loop_One.ico", UriKind.Relative));
            }
        }

        private void imagesRadom_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!mediaPlayRandom)
            {
                mediaPlayRandom = true;
                imagesRadom.Source = new BitmapImage(new Uri(@"Images\Random_On.ico", UriKind.Relative));
            }
            else
            {
                mediaPlayRandom = false;
                imagesRadom.Source = new BitmapImage(new Uri(@"Images\Random.ico", UriKind.Relative));
            }
        }

        private void imagesBackward_MouseDown(object sender, MouseButtonEventArgs e)
        {
            userIsDraggingSlider = false;
            if (sliProgress.Value > 10)
            {
                mePlayer.Position = TimeSpan.FromSeconds(sliProgress.Value - 10);
                lblProgressStatus.Text = TimeSpan.FromSeconds(sliProgress.Value - 10).ToString(@"hh\:mm\:ss");
            }

        }

        private void ImagesForward_MouseDown(object sender, MouseButtonEventArgs e)
        {
            userIsDraggingSlider = false;
            if (sliProgress.Value < sliProgress.Maximum - 10)
            {
                mePlayer.Position = TimeSpan.FromSeconds(sliProgress.Value + 10);
                lblProgressStatus.Text = TimeSpan.FromSeconds(sliProgress.Value + 10).ToString(@"hh\:mm\:ss");
            }
        }

        string Dir = Directory.GetCurrentDirectory() + @"\playlists.txt";
        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Dir))
                File.Delete(Dir);
            StreamWriter sw = new StreamWriter(Dir, true);
            using (sw)
            {
                for (int i = 0; i < ListSong.Count; i++)
                {
                    sw.WriteLine(ListSong[i].Path);
                }
            }
            System.Windows.MessageBox.Show("Playlist is saved!" + "\n" + Dir);
            sw.Close();
        }

        private void ChoosePlayListButton_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(Dir))
            {
                System.Windows.MessageBox.Show("There is no list of saved songs!!!");
                return;
            }
            var reader = new StreamReader("playlists.txt");

            while (!reader.EndOfStream)
            {
                string item = reader.ReadLine();
                string[] arrname = item.Split(new char[] { '\\', '.' });
                Song s = new Song();
                s.Name = arrname[arrname.Length - 2];
                s.Path = item;
                if(!IsExist(item))
                {
                    ListSong.Add(s);
                    actionsListBox.Items.Add(s);
                }
                var bc = new BrushConverter();
                actionsListBox.Background = (Brush)bc.ConvertFrom("#1A1C29");
            }

            System.Windows.MessageBox.Show((actionsListBox.Items.Count).ToString() + " songs");
            reader.Close();
        }


        private void RemoveFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (actionsListBox.SelectedItem != null)
            {
                int index = 0;
                List<Song> NewListSong = null;
                //Title = ListSong.Count.ToString();
                System.Windows.MessageBox.Show(ListSong[actionsListBox.SelectedIndex].Name + " is deleted!");

                if (actionsListBox.SelectedItem != null)
                {
                    index = actionsListBox.SelectedIndex;
                }
                //Remove the song is chose in ListSong
                NewListSong = ListSong;
                NewListSong.RemoveAt(index);
                ListSong = NewListSong;

                //Remove the song is chose in actionsListBox
                actionsListBox.Items.Clear();
                for (var i = 0; i < ListSong.Count; i++)
                {
                    actionsListBox.Items.Add(ListSong[i]);
                }
                //Title = ListSong.Count.ToString();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _hook.KeyUp -= _hook_KeyUp;
            _hook.Dispose();
        }
    }
}
