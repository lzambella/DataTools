using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Music_Plot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SeriesCollection WordList { get; set; }
        public List<String> UniqueWords { get; set; }
        public List<String> UniqueWordsSorted { get; set; }
        public ChartValues<int> WordFrequency { get; set; }
        private String lyrics { get; set; }
        private char[] splitregex = {' ', ',' , '\r', '\n' , '\0', '\"', };
        private List<(int, string)> WordDict { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            WordList = new SeriesCollection();
            UniqueWords = new List<string>();
            UniqueWordsSorted = new List<string>();
            WordFrequency = new ChartValues<int>();
            WordDict = new List<(int, string)>();
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                lyrics = await ReadFileAsync(openFileDialog.FileName);
                Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                lyrics = rgx.Replace(lyrics, "\n");
                // Load all of the unique words into a list
                foreach (var word in lyrics.Split(splitregex))
                {
                    if (UniqueWords.Count == 0)
                        UniqueWords.Add(word);
                    else if (!WordExists(word, UniqueWords) && word.Length >= 3) UniqueWords.Add(word);
                }



                // Then get the counts of all of them
                foreach (var word in UniqueWords)
                {
                    var count = lyrics.Split(splitregex).Count(i => i == word);
                    if (count < 5) continue;
                    WordDict.Add((count, word));
                }
                foreach (var freq in WordDict.OrderByDescending(x => x.Item1))
                {
                    WordFrequency.Add(freq.Item1);
                    UniqueWordsSorted.Add(freq.Item2);
                }

                WordList.Add(new ColumnSeries
                {
                    Title = "Words in a song.",
                    Values = WordFrequency
                });
                DataContext = this;
            }

        }
        /// <summary>
        /// Check if a word exists in a list
        /// </summary>
        /// <param name="word"></param>
        /// <param name="wordList"></param>
        /// <returns></returns>
        private bool WordExists(string word, List<String> wordList)
        {
            return wordList.Count(w => w == word) > 0;
        }
        private async Task<string> ReadFileAsync(string dir)
        {
            var r = new StreamReader(dir);
            var content = await r.ReadToEndAsync();
            return content;
        }
    }
}
