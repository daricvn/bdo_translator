using BDOTranslator.Models;
using BDOTranslator.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace BDOTranslator_WPF.Views
{
    /// <summary>
    /// Interaction logic for Translator.xaml
    /// </summary>
    public partial class Translator : Window
    {
        private TextLine[] lines;
        public Translator()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Text files (*.txt)|*.txt";
            dialog.FileOk += Dialog_FileOk;
            dialog.ShowDialog();
        }

        private void Dialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var dialog = (OpenFileDialog)sender;
            filePath.Text = dialog.FileName;
            btnBrowse.IsEnabled = false;
            Task.Run(ProcessLines);
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = listBox.SelectedIndex;
            txtOld.Text = lines[index].Text;
            txtEdit.Text = "";
        }

        private async Task ProcessLines()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var reader = new LocalizationFile(filePath.Text);
                lines = reader.Process();
                BindingOperations.SetBinding(listBox, ItemsControl.ItemsSourceProperty, new Binding()
                {
                    Source = lines
                });
                listBox.DisplayMemberPath = "Text";
                btnBrowse.IsEnabled = true;
            });
        }
    }
}
