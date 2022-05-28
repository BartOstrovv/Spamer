using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MailSpamer
{
    public partial class MainWindow : Window
    {
        private Spamer customSpamer;
        private List<CustomContainer> comboData;
        private List<CustomContainer> listData;

        private bool _working;
        private int countOfCopy = 0;

        public MainWindow()
        {
            InitializeComponent();
            comboData = new List<CustomContainer>();
            listData = new List<CustomContainer>();

        }

        private void addMailToListBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(EmailToTxt.Text) && (recipientsList.Items != null))
            {
                recipientsList.ItemsSource = null;
                listData.Add(new CustomContainer(EmailToTxt.Text, listData.Count));
                recipientsList.ItemsSource = listData;
            }
        }

        private void delRecipientBtn_Click(object sender, RoutedEventArgs e)
        {
            var index = int.Parse(((Button)sender).Tag.ToString());
            if (index >= 0)
            {
                recipientsList.ItemsSource = null;
                listData.RemoveAt(index);
                foreach (var itemList in listData)
                {
                    if (index < itemList._index)
                        itemList._index--;
                }
                recipientsList.ItemsSource = listData;
            }
        }

        private void chooseFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog() { Multiselect = true };
            if (dlg.ShowDialog() == true)
            {
                fileNamesCombo.ItemsSource = null;
                comboData.Clear();

                foreach (var file in dlg.FileNames)
                    comboData.Add(new CustomContainer(file, comboData.Count));
                fileNamesCombo.ItemsSource = comboData;
            }
        }

        private void PutFilesToMessage()
        {
            if (customSpamer != null)
            {
                customSpamer.ClearFiles();

                foreach (var file in comboData)
                    customSpamer.PutFiles(file._text);
            }
        }

        private void delFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var index = int.Parse(((Button)sender).Tag.ToString());
            if (index >= 0)
            {
                fileNamesCombo.ItemsSource = null;
                comboData.RemoveAt(index);
                foreach (var itemCombo in comboData)
                {
                    if (index < itemCombo._index)
                        itemCombo._index--;
                }
                fileNamesCombo.ItemsSource = comboData;
            }
        }

        private void fileNamesCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            fileNamesCombo.SelectedItem = null;
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(loginTxt.Text) || string.IsNullOrEmpty(passTxt.Password) || string.IsNullOrEmpty(countCopyTxt.Text) || (listData.Count == 0))
            {
                MessageBox.Show("Please input all information and try again!");
                return;
            }

            

            int.TryParse(countCopyTxt.Text, out int countCopy);
            if (countCopy <= 0)
                return;

            countOfCopy = countCopy;

            if (!_working)
            {
                customSpamer = new Spamer(loginTxt.Text, passTxt.Password);
                Start();
            }
            else
                Stop();

        }

        private void Start()
        {
            Thread t = new Thread(Worker) { IsBackground = true };

            _working = true;

            startBtn.Content = "Stop";
            startBtn.Background = Brushes.Red;

            t.Start();
        }

        private void Stop()
        {
            _working = false;

            startBtn.Content = "Start";
            startBtn.Background = Brushes.Green;
        }

        private void Worker()
        {
            int sendCount = 0;
            while (_working && (sendCount < countOfCopy))
            {
                this.Dispatcher.Invoke(() =>
                {

                    foreach (var rec in listData)
                    {
                        customSpamer.InitMessage(rec._text, subjectTxt.Text, bodyTxt.Text);
                        PutFilesToMessage();
                        customSpamer.SendMessage();
                    }
                });
                sendCount++;
                Thread.Sleep(1000);
            }
        }
    }
}
