using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PadZ
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string setPath = "settings.txt";
        const string SPEC = "~";
        public MainWindow()
        {
            InitializeComponent();
            if (!System.IO.File.Exists(setPath)) System.IO.File.Create(setPath);
            RefreshList();
        }
        char GetSpec() { return SPEC[0]; }
        void RefreshList()
        {
            liPads.Items.Clear();
            foreach (var set in System.IO.File.ReadAllLines(setPath))
            {
                liPads.Items.Add(set.Replace(SPEC[0], ' '));
            }
        }
        string FilesPath()
        {
            return Environment.CurrentDirectory + "\\AppFiles\\";
        }
        string[] RemoveBrokenSounds(string[] sets)
        {
            List<string> sounds = new List<string>();
            try
            {
                MessageBox.Show(sets[0].Split(GetSpec())[2]);
                foreach (string set in sets)
                {
                    string thispath = set.Split(GetSpec())[2];
                    if (File.Exists(thispath))
                    {
                        sounds.Add(set);
                    }
                    else MessageBox.Show("File !exists " + set);
                }
            }
            catch (Exception ex) { MessageBox.Show("RemoveBrokenSounds():" + ex.Message); }
            return sounds.ToArray();
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (tbCaption.Text.Length != 0 && tbColor.Text.Length != 0 && tbAudioPath.Text.Length != 0)
            {
                try
                {
                    System.IO.File.AppendAllText(setPath, tbCaption.Text + SPEC + tbColor.Text + SPEC + Environment.CurrentDirectory + "\\AppFiles\\" + GetFileName(tbAudioPath.Text) + Environment.NewLine);
                    RefreshList();
                    if (!Directory.Exists(Environment.CurrentDirectory + "\\AppFiles")) Directory.CreateDirectory(Environment.CurrentDirectory + "\\AppFiles");
                    File.Copy(tbAudioPath.Text, Environment.CurrentDirectory + "\\AppFiles\\" + GetFileName(tbAudioPath.Text));
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void liPads_Selected(object sender, RoutedEventArgs e)
        {
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (liPads.SelectedIndex != -1)
            {
                try
                {
                    List<string> setli = System.IO.File.ReadAllLines(setPath).ToList();
                    setli.RemoveAt(liPads.SelectedIndex);
                    System.IO.File.WriteAllLines(setPath, setli.ToArray());
                    RefreshList();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }
        private void btnChooseAudio_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            if (fd.ShowDialog() == true)
            {
                tbAudioPath.Text = fd.FileName;
            }
        }
        private void btnMake_Click(object sender, RoutedEventArgs e)
        {
            if (File.ReadAllLines(setPath).Length > 0)
            {
                try
                {
                    int lastlen = File.ReadAllLines(setPath).Length;
                    File.WriteAllLines(setPath, RemoveBrokenSounds(File.ReadAllLines(setPath)));
                    int finlen = File.ReadAllLines(setPath).Length;
                    if (lastlen != finlen) MessageBox.Show("Pre-launch sound availbility check changed sounds count: " + lastlen + "=>" + finlen);
                }
                catch (Exception ex) { MessageBox.Show("btnMake_Click: " + ex.Message); }
                new PadWindow().ShowDialog();
            }
            else
            {
                MessageBox.Show("There is no sense in creating empty launchpad.");
            }
        }
        string GetFileName(string path)
        {
            for (int i = path.Length - 1; i >= 0; i--)
            {
                if (path[i] == '\\')
                {
                    return path.Substring(i + 1, path.Length - i - 1);
                }
            }
            return "NO_NAME";
        }
        string FileToBase64(string path)
        {
            return Convert.ToBase64String(File.ReadAllBytes(path));
        }
        List<byte[]> Base64ToBytes(string[] base64s)
        {
            List<byte[]> bytes = new List<byte[]>();
            for (int i = 0; i < base64s.Length; i++)
            {
                bytes.Add(Convert.FromBase64String(base64s[i]));
            }
            return bytes;
        }
        void CreateFile(string name = "")
        {
            try
            {
                byte[] ret = Convert.FromBase64String(name);
                FileInfo fil = new FileInfo(name);
                using (Stream sw = fil.OpenWrite())
                {
                    sw.Write(ret, 0, ret.Length);
                    sw.Close();
                }
            }
            catch
            {
            }
        }

    }
}
