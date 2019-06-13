using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace WordHw
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            comboBoxFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            comboBoxFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
            richTextBoxEditor.Focus();

            ThreadPool.QueueUserWorkItem(Copying);
        }

        private void RichTextBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            object temp = richTextBoxEditor.Selection.GetPropertyValue(TextElement.FontWeightProperty);
            buttonBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));
            temp = richTextBoxEditor.Selection.GetPropertyValue(TextElement.FontStyleProperty);
            buttonItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));
            temp = richTextBoxEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            buttonUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

            temp = richTextBoxEditor.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
            comboBoxFontFamily.SelectedItem = temp;
            temp = richTextBoxEditor.Selection.GetPropertyValue(TextElement.FontSizeProperty);
            comboBoxFontSize.Text = temp.ToString();
        }

        private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
            if (fileDialog.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(fileDialog.FileName, FileMode.Open);
                TextRange range = new TextRange(richTextBoxEditor.Document.ContentStart, richTextBoxEditor.Document.ContentEnd);
                range.Load(fileStream, DataFormats.Rtf);
            }
        }

        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
            if (fileDialog.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(fileDialog.FileName, FileMode.Create);
                TextRange range = new TextRange(richTextBoxEditor.Document.ContentStart, richTextBoxEditor.Document.ContentEnd);
                range.Save(fileStream, DataFormats.Rtf);
            }
        }

        private void ComboBoxFontFamilySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxFontFamily.SelectedItem != null)
                richTextBoxEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, comboBoxFontFamily.SelectedItem);
            richTextBoxEditor.FontFamily = comboBoxFontFamily.SelectedItem as FontFamily;
            richTextBoxEditor.Focus();
        }

        private void ComboBoxFontSizeTextChanged(object sender, TextChangedEventArgs e)
        {
            if (comboBoxFontFamily.SelectedItem != null)
                richTextBoxEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, comboBoxFontSize.Text);
            richTextBoxEditor.FontSize = Convert.ToDouble(comboBoxFontSize.Text);
            richTextBoxEditor.Focus();
        }

        private void Copying(object obj)
        {
            while (true)
            {
                Thread.Sleep(10000);
                Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    TextRange doc = new TextRange(richTextBoxEditor.Document.ContentStart, richTextBoxEditor.Document.ContentEnd);
                    using (FileStream fileStream = File.Create("SuperFile.rtf"))
                    {
                        doc.Save(fileStream, DataFormats.Rtf);
                    }
                }));
            }
        }
    }
}
