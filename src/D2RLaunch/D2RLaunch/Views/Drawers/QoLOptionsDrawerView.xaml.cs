using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
using System.IO;
using System.Text.RegularExpressions;
using D2RLaunch.ViewModels;
using D2RLaunch.Models;
using D2RLaunch.ViewModels.Drawers;

namespace D2RLaunch.Views.Drawers
{
    /// <summary>
    /// Interaction logic for QoLOptionsDrawerView.xaml
    /// </summary>
    public partial class QoLOptionsDrawerView : UserControl
    {
        public QoLOptionsDrawerView()
        {
            InitializeComponent();
        }

        private void ColorPicker_ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e, int matchCount)
        {
            if (e.NewValue is Color chosenColor)
            {
                byte redValue = chosenColor.R;
                byte greenValue = chosenColor.G;
                byte blueValue = chosenColor.B;

                double Normalize(byte value) => Math.Round(value / 2.55 * 0.01, 6);
                string Format(double val) => val.ToString("0.0######");

                var mainWindow = Window.GetWindow(this);

                if (mainWindow != null)
                {
                    var shellViewModel = mainWindow.DataContext as ShellViewModel;

                    if (shellViewModel != null)
                    {
                        string folderPath = System.IO.Path.Combine(shellViewModel.SelectedModDataFolder, "D2RLaunch", "Monster Stats");
                        string filePath = System.IO.Path.Combine(folderPath, "MyColorSettings.txt");

                        Directory.CreateDirectory(folderPath);
                        List<string> lines = new List<string>();

                        if (!File.Exists(filePath))
                            File.Create(filePath).Close();

                        lines.AddRange(File.ReadAllLines(filePath));

                        if (matchCount >= 0 && matchCount < lines.Count)
                            lines[matchCount] = $"Red: {chosenColor.R} Green: {chosenColor.G} Blue: {chosenColor.B}";
                        else
                        {
                            Console.WriteLine($"Invalid matchCount: {matchCount}");
                            return;
                        }

                        // Write modified content back to the file
                        File.WriteAllLines(filePath, lines);

                        // Your existing code for modifying another file
                        string jsonFilePath = System.IO.Path.Combine(shellViewModel.SelectedModDataFolder, "global", "ui", "layouts", "hudmonsterhealthhd.json");
                        string[] patterns = { @"""r""\s*:\s*-?\d*\.?\d*(?:[Ee][-+]?\d+)?", @"""g""\s*:\s*-?\d*\.?\d*(?:[Ee][-+]?\d+)?", @"""b""\s*:\s*-?\d*\.?\d*(?:[Ee][-+]?\d+)?" };
                        string[] replacements = { $"\"r\": {Format(Normalize(redValue))}", $"\"g\": {Format(Normalize(greenValue))}", $"\"b\": {Format(Normalize(blueValue))}" };

                        if (File.Exists(jsonFilePath))
                        {
                            string fileContents = File.ReadAllText(jsonFilePath);
                            string updatedContents = fileContents;

                            for (int i = 0; i < patterns.Length; i++)
                            {
                                Match match = Regex.Match(updatedContents, patterns[i]);
                                for (int j = 0; j < matchCount; j++)
                                {
                                    match = match.NextMatch();
                                    if (!match.Success)
                                    {
                                        Console.WriteLine($"Not enough matches found for the pattern {patterns[i]}.");
                                        break;
                                    }
                                }
                                if (match.Success)
                                    updatedContents = updatedContents.Remove(match.Index, match.Length).Insert(match.Index, replacements[i]);
                                else
                                    Console.WriteLine($"No match found for the pattern {patterns[i]}.");
                            }
                            File.WriteAllText(jsonFilePath, updatedContents);
                        }
                    }
                }  
            }
        }

        private void colorPicker_ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker_ColorChanged(d, e, 0);
        }

        private void colorPicker2_ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker_ColorChanged(d, e, 1);
        }

        private void colorPicker3_ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker_ColorChanged(d, e, 2);
        }

    }
}
