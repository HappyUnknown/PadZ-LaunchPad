using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PadZ
{
    /// <summary>
    /// Логика взаимодействия для PadWindow.xaml
    /// </summary>
    public partial class PadWindow : Window
    {
        MediaPlayer player = new MediaPlayer();
        int queuePos = 0;
        const string SPEC = "~";
        string setPath = "settings.txt";
        List<string> setts = new List<string>();
        List<Button> butts = new List<Button>();
        int ButtonIndex(object sender)
        {
            Button b = (Button)sender;
            for (int i = 0; i < butts.Count; i++)
            {
                if (butts[i] == b) return i;
            }
            return -1;
        }
        private void button1_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (QueueAvailable()) liQueue.Items.Add(GetPath(setts, sender));
                MediaPlayer music = new MediaPlayer();
                music.Open(new Uri(GetPath(setts, sender)));
                music.Play();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public PadWindow()
        {
            InitializeComponent();
            try
            {
                setts = File.ReadAllLines(setPath).ToList();
                for (int i = 0; i < setts.Count; i++)
                {
                    Button b = new Button();
                    b.Width = 20;
                    b.Height = 20;
                    b.Content = i + 1;
                    b.Click += new RoutedEventHandler(button1_Click);
                    butts.Add(b);
                    pnlContent.Children.Add(b);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnEnableQueue_Click(object sender, RoutedEventArgs e)
        {
            btnOfQueueSound.IsEnabled = !btnOfQueueSound.IsEnabled;
        }
        bool QueueAvailable()
        {
            return btnOfQueueSound.IsEnabled;
        }
        private void btnOfQueueSound_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (liQueue.Items.Count > 0)
                {
                    liQueue.Items.RemoveAt(liQueue.Items.Count - 1);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        List<string> GetListedQueue()
        {
            List<string> queue = new List<string>();
            for (int i = 0; i < liQueue.Items.Count; i++)
                queue.Add(liQueue.Items[i].ToString());
            return queue;
        }
        private void btnPlayQueue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var queue = GetListedQueue();
                player.Open(new Uri(queue[queuePos].ToString()));
                player.Play();
                queuePos++;
                player.MediaEnded += new EventHandler(MediaEnded);
            }
            catch (Exception ex) { MessageBox.Show("btnPlayQueue_Click(): " + ex.Message); }
        }
        void MediaEnded(object sender, EventArgs e)
        {
            try
            {
                if (queuePos < GetListedQueue().Count)
                {
                    player.Open(new Uri(liQueue.Items[queuePos].ToString()));
                    player.Play();
                    queuePos++;
                }
                else { queuePos = 0; MessageBox.Show("DONE"); }

            }
            catch (Exception ex) { MessageBox.Show("MediaEnded:Index - " + queuePos + ">=" + GetListedQueue().Count + ". " + ex.Message); }
        }
        string GetPath(List<string> setts, object sender)
        {
            return setts[ButtonIndex(sender)].Split(SPEC[0])[2];
        }
    }
}
