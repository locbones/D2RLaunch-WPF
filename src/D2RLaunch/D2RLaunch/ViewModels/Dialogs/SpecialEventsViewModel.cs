﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace D2RLaunch.ViewModels.Dialogs;

public class SpecialEventsViewModel : Screen
{
    #region ---Static Members---
    private ILog _logger = LogManager.GetLogger(typeof(DownloadNewModViewModel));
    private ObservableCollection<KeyValuePair<string, string>> _mods = new ObservableCollection<KeyValuePair<string, string>>();
    private KeyValuePair<string, string> _selectedMod;
    public string eventNameStr;
    public bool eventJoined = false;
    private string _eventImage;

    #endregion

    #region ---Window/Loaded Handlers---

    public SpecialEventsViewModel()
    {
        GetCurrentEvents();
    }
    public SpecialEventsViewModel(ShellViewModel shellViewModel)
    {
        DisplayName = "Special Event Status";
        ShellViewModel = shellViewModel;
        Execute.OnUIThread(async () =>
        {
            await GetCurrentEvents();
        });
    }

    #endregion

    #region ---Properties---

    public ShellViewModel ShellViewModel { get; }
    public KeyValuePair<string, string> SelectedMod
    {
        get => _selectedMod;
        set
        {
            if (value.Equals(_selectedMod)) return;
            _selectedMod = value;
            NotifyOfPropertyChange();
        }
    }
    private string _eventNameText;
    private string _eventTypeText;
    private string _eventLocText;
    private string _eventDur1Text;
    private string _eventDur2Text;
    private string _eventDesc1Text;
    private string _eventDesc2Text;
    private string _eventDesc3Text;
    private string _eventDesc4Text;
    private string _eventLinkText;
    private string _eventImageText;
    private string _eventMLvlText;
    private string _eventPLvlText;
    private string _eventPDiffText;
    private string _eventPExpText;
    public string EventNameText
    {
        get => _eventNameText;
        set
        {
            if (_eventNameText != value)
            {
                _eventNameText = value;
                OnPropertyChanged(nameof(EventNameText));
            }
        }
    }
    public string EventTypeText
    {
        get => _eventTypeText;
        set
        {
            if (_eventTypeText != value)
            {
                _eventTypeText = value;
                OnPropertyChanged(nameof(EventTypeText));
            }
        }
    }
    public string EventLocText
    {
        get => _eventLocText;
        set
        {
            if (_eventLocText != value)
            {
                _eventLocText = value;
                OnPropertyChanged(nameof(EventLocText));
            }
        }
    }
    public string EventDur1Text
    {
        get => _eventDur1Text;
        set
        {
            if (_eventDur1Text != value)
            {
                _eventDur1Text = value;
                OnPropertyChanged(nameof(EventDur1Text));
            }
        }
    }
    public string EventDur2Text
    {
        get => _eventDur2Text;
        set
        {
            if (_eventDur2Text != value)
            {
                _eventDur2Text = value;
                OnPropertyChanged(nameof(EventDur2Text));
            }
        }
    }
    public string EventDesc1Text
    {
        get => _eventDesc1Text;
        set
        {
            if (_eventDesc1Text != value)
            {
                _eventDesc1Text = value;
                OnPropertyChanged(nameof(EventDesc1Text));
            }
        }
    }
    public string EventDesc2Text
    {
        get => _eventDesc2Text;
        set
        {
            if (_eventDesc2Text != value)
            {
                _eventDesc2Text = value;
                OnPropertyChanged(nameof(EventDesc2Text));
            }
        }
    }
    public string EventDesc3Text
    {
        get => _eventDesc3Text;
        set
        {
            if (_eventDesc3Text != value)
            {
                _eventDesc3Text = value;
                OnPropertyChanged(nameof(EventDesc3Text));
            }
        }
    }
    public string EventDesc4Text
    {
        get => _eventDesc4Text;
        set
        {
            if (_eventDesc4Text != value)
            {
                _eventDesc4Text = value;
                OnPropertyChanged(nameof(EventDesc4Text));
            }
        }
    }
    public string EventLinkText
    {
        get => _eventLinkText;
        set
        {
            if (_eventLinkText != value)
            {
                _eventLinkText = value;
                OnPropertyChanged(nameof(EventLinkText));
            }
        }
    }
    public string EventImageText
    {
        get => _eventImageText;
        set
        {
            if (_eventImageText != value)
            {
                _eventImageText = value;
                OnPropertyChanged(nameof(EventImageText));
            }
        }
    }
    public string EventMLvlText
    {
        get => _eventMLvlText;
        set
        {
            if (_eventMLvlText != value)
            {
                _eventMLvlText = value;
                OnPropertyChanged(nameof(EventMLvlText));
            }
        }
    }
    public string EventPLvlText
    {
        get => _eventPLvlText;
        set
        {
            if (_eventPLvlText != value)
            {
                _eventPLvlText = value;
                OnPropertyChanged(nameof(EventPLvlText));
            }
        }
    }
    public string EventPDiffText
    {
        get => _eventPDiffText;
        set
        {
            if (_eventPDiffText != value)
            {
                _eventPDiffText = value;
                OnPropertyChanged(nameof(EventPDiffText));
            }
        }
    }
    public string EventPExpText
    {
        get => _eventPExpText;
        set
        {
            if (_eventPExpText != value)
            {
                _eventPExpText = value;
                OnPropertyChanged(nameof(EventPExpText));
            }
        }
    }
    public string EventImage
    {
        get => _eventImage;
        set
        {
            if (_eventImage != value)
            {
                _eventImage = value;
                OnPropertyChanged(nameof(EventImage));
            }
        }
    }
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

    #region ---Event Functions---

    private async Task GetCurrentEvents()
    {
        string eventScheduleFile = Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLaunch/Events/EventSchedule.txt");
        string eventFolder = Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLaunch/Events/");

        if (File.Exists(eventScheduleFile))
        {
            string[] scheduleContents = File.ReadAllLines(eventScheduleFile);
            DateTime currentTime = DateTime.Now;
            bool eventFound = false;

            // Iterate through each line and extract data
            foreach (string line in scheduleContents)
            {
                string[] parts = line.Split(',');

                if (parts.Length == 3)
                {
                    string startTimeStr = parts[0].Trim();
                    string endTimeStr = parts[1].Trim();
                    eventNameStr = parts[2].Trim();
                    DateTime startTime;
                    DateTime endTime;

                    if (DateTime.TryParse(startTimeStr, out startTime) && DateTime.TryParse(endTimeStr, out endTime))
                    {
                        startTime = ConvertUtcToLocalTime(startTimeStr);
                        endTime = ConvertUtcToLocalTime(endTimeStr);

                        // Check if the current time falls within the event's time range
                        if (currentTime >= startTime && currentTime <= endTime)
                        {
                            if (Directory.Exists(Path.Combine(eventFolder, eventNameStr)))
                            {
                                string TZFile = Path.Combine(Path.Combine(eventFolder, eventNameStr), "hd/global/excel/desecratedzones.json");

                                try
                                {
                                    if (File.Exists(TZFile))
                                    {
                                        string fileContent = File.ReadAllText(TZFile);
                                        string[] eventInfo = new string[]
                                        { "Event Name: ", "Event Type: ", "Event Location(s): ", "\"start_time_utc\": ", "\"end_time_utc\": ", "Event Description 1: ", "Event Description 2: ", "Event Description 3: ", "Event Description 4: ", "Event Link: ", "Event Image: ", "\"bound_incl_max\": ", "\"boost_level\": ", "\"difficulty_scale\": ", "\"boost_experience_percent\": " };


                                        Dictionary<string, string> extractedValues = new Dictionary<string, string>();

                                        foreach (string pattern in eventInfo)
                                        {
                                            string value = ExtractString(fileContent, pattern);
                                            extractedValues[pattern] = value;
                                        }

                                        List<string> eventInfo2 = new List<string>
                                        { "\"bound_incl_max\": ", "\"boost_level\": ", "\"difficulty_scale\": ", "\"boost_experience_percent\": " };

                                        Dictionary<string, List<string>> extractedValues2 = new Dictionary<string, List<string>>();

                                        foreach (string pattern in eventInfo2)
                                        {
                                            List<string> values = ExtractStrings(fileContent, pattern);
                                            values = values.GetRange(0, Math.Min(3, values.Count));
                                            extractedValues2[pattern] = values;
                                        }

                                        string eventMLvl = FormatValues(extractedValues2["\"bound_incl_max\": "], ", /* Maximum level of a terrorized monster. MAX(bound_incl_max, original_monster_level) */").Replace(",", "");
                                        string eventPLvl = FormatValues(extractedValues2["\"boost_level\": "], ", /* player_level + boost_level = terrorized_monster_level */", "+").Replace(",", "");
                                        string eventPDiff = FormatValues(extractedValues2["\"difficulty_scale\": "], ", /* Fake the amount of players in the game. AKA /players X */").Replace(",", "");
                                        string eventPExp = FormatValuesWithPercentage(extractedValues2["\"boost_experience_percent\": "], " /* Bonus experience percentage applied at to the monster's base experience in monstats.txt */");

                                        string eventName = extractedValues["Event Name: "];
                                        string eventType = extractedValues["Event Type: "];
                                        string eventLoc = extractedValues["Event Location(s): "];
                                        string eventDur1 = extractedValues["\"start_time_utc\": "].Replace("\"", "").Replace(",", "");
                                        string eventDur2 = extractedValues["\"end_time_utc\": "].Replace("\"", "").Replace(",", "").Replace(" /* Use end time to automate multiple configs. Useful for events. */", "");
                                        string eventDesc1 = extractedValues["Event Description 1: "];
                                        string eventDesc2 = extractedValues["Event Description 2: "];
                                        string eventDesc3 = extractedValues["Event Description 3: "];
                                        string eventDesc4 = extractedValues["Event Description 4: "];
                                        string eventLink = extractedValues["Event Link: "];
                                        string eventImage = extractedValues["Event Image: "];

                                        DateTime localTime1 = ConvertUtcToLocalTime(eventDur1);
                                        DateTime localTime2 = ConvertUtcToLocalTime(eventDur2);
                                        string formattedTime1 = localTime1.ToString("MM-dd HH:mm");
                                        string formattedTime2 = localTime2.ToString("MM-dd HH:mm");

                                        string logoPath = Path.Combine(Path.Combine(eventFolder, eventNameStr), "Event.png");
                                        if (File.Exists(logoPath))
                                        {
                                            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
                                            File.Copy(logoPath, tempPath, true);
                                            EventImage = tempPath;
                                        }

                                        ChangeText(eventName, eventType, eventLoc, formattedTime1.ToString(), formattedTime2.ToString(), eventDesc1, eventDesc2, eventDesc3, eventDesc4, eventLink, eventImage, eventMLvl, eventPLvl, eventPDiff, eventPExp);

                                        if (eventJoined == true)
                                        {
                                            if (!Directory.Exists(Path.Combine(Path.GetDirectoryName(ShellViewModel.SelectedModDataFolder), "data_noevent")))
                                                CloneDirectory(ShellViewModel.SelectedModDataFolder, Path.Combine(Path.GetDirectoryName(ShellViewModel.SelectedModDataFolder), "data_noevent"));

                                            Directory.Delete(ShellViewModel.SelectedModDataFolder, true);
                                            CloneDirectory(Path.Combine(Path.GetDirectoryName(ShellViewModel.SelectedModDataFolder), "data_noevent"), ShellViewModel.SelectedModDataFolder);

                                            await CopyDirectoryAndOverwrite(Path.Combine(Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLaunch/Events/"), eventNameStr), ShellViewModel.SelectedModDataFolder);
                                            MessageBox.Show("Event Joined Successfully!");
                                        }
                                        //MessageBox.Show($"Current Event\n{eventNameStr}\n{startTime}\n{endTime}");
                                        eventFound = true;
                                        return;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    _logger.Error(ex);
                                    return;
                                }
                            }
                        }
                    }
                    else
                        Console.WriteLine("Invalid date format in line: " + line);
                }
                else
                    Console.WriteLine("Invalid line format: " + line);
            }

            // No Event Found
            if (!eventFound)
            {
                ChangeText("No Active Events", "N/A", "N/A", "N/A", "N/A", "No Data Available", "", "", "", "", "", "", "", "", "");

                if (Directory.Exists(Path.Combine(Path.GetDirectoryName(ShellViewModel.SelectedModDataFolder), "data_noevent")))
                {
                    Directory.Delete(ShellViewModel.SelectedModDataFolder, true);
                    Directory.Move(Path.Combine(Path.GetDirectoryName(ShellViewModel.SelectedModDataFolder), "data_noevent"), ShellViewModel.SelectedModDataFolder);
                }
            }
        }
    }
    public async void OnJoinEvent()
    {
        eventJoined = true;
        MessageBox.Show("Please allow the launcher a few moments to make file changes:\n- Backup Non-Event Files\n- Install Event Files\n\nThe launcher will notify you when it is finished");
        CloneDirectory(ShellViewModel.SelectedModDataFolder, Path.Combine(Path.GetDirectoryName(ShellViewModel.SelectedModDataFolder), "data_noevent"));
        await CopyDirectoryAndOverwrite(Path.Combine(Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLaunch/Events/"), eventNameStr), ShellViewModel.SelectedModDataFolder);
        GetCurrentEvents();
    }
    public async void OnLeaveEvent()
    {
        Directory.Delete(ShellViewModel.SelectedModDataFolder, true);
        Directory.Move(Path.Combine(Path.GetDirectoryName(ShellViewModel.SelectedModDataFolder), "data_noevent"), ShellViewModel.SelectedModDataFolder);
        MessageBox.Show("Event Left!");
    }

    #endregion

    #region ---Helper Functions---

    static string FormatValues(List<string> values, string patternToRemove, string additionalPrefix = "")
    {
        List<string> formattedValues = new List<string>();
        foreach (string value in values)
        {
            string formattedValue = additionalPrefix + value.Replace(patternToRemove, "").Trim();
            formattedValues.Add(formattedValue);
        }
        return string.Join(" / ", formattedValues);
    }
    static string FormatValuesWithPercentage(List<string> values, string patternToRemove)
    {
        List<string> formattedValues = new List<string>();
        foreach (string value in values)
        {
            string formattedValue = value.Replace(patternToRemove, "").Trim() + "%";
            formattedValues.Add(formattedValue);
        }
        return string.Join(" / ", formattedValues);
    }
    static string ExtractString(string content, string pattern)
    {
        string regexPattern = Regex.Escape(pattern) + "(.*)";
        Match match = Regex.Match(content, regexPattern);
        return match.Success ? match.Groups[1].Value.Trim() : "Not found";
    }
    static List<string> ExtractStrings(string content, string pattern)
    {
        List<string> values = new List<string>();
        string regexPattern = Regex.Escape(pattern) + "(.*)";
        MatchCollection matches = Regex.Matches(content, regexPattern);

        foreach (Match match in matches)
        {
            if (match.Success)
            {
                values.Add(match.Groups[1].Value.Trim());
            }
            else
            {
                values.Add("Not found");
            }
        }

        return values;
    }
    public void ChangeText(string eventName, string eventType, string eventLoc, string eventDur1, string eventDur2, string eventDesc1, string eventDesc2, string eventDesc3, string eventDesc4, string eventLink, string eventImage, string eventMLvl, string eventPLvl, string eventPDiff, string eventPExp)
    {
        EventNameText = eventName.Replace("\\n", "\n");
        EventTypeText = eventType.Replace("\\n", "\n");
        EventLocText = eventLoc.Replace("\\n", "\n");
        EventDur1Text = eventDur1.Replace("\\n", "\n");
        EventDur2Text = eventDur2.Replace("\\n", "\n");
        EventDesc1Text = eventDesc1.Replace("\\n", "\n") + "\n";
        EventDesc2Text = eventDesc2.Replace("\\n", "\n") + "\n\n";
        EventDesc3Text = eventDesc3.Replace("\\n", "\n") + "\n";
        EventDesc4Text = eventDesc4.Replace("\\n", "\n") + "\n\n";
        EventLinkText = "\n" + eventLink.Replace("\\n", "\n");
        EventImageText = eventImage.Replace("\\n", "\n");
        EventMLvlText = eventMLvl.Replace("\\n", "\n");
        EventPLvlText = eventPLvl.Replace("\\n", "\n");
        EventPDiffText = eventPDiff.Replace("\\n", "\n");
        EventPExpText = eventPExp.Replace("\\n", "\n");
    }
    private DateTime ConvertUtcToLocalTime(string utcTimeString)
    {
        DateTime utcDateTime = DateTime.ParseExact(utcTimeString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
        DateTime localDateTime = utcDateTime.ToLocalTime();

        return localDateTime;
    }
    public DateTime ConvertUtcToLocalTime2(DateTime utcTime)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(utcTime, TimeZoneInfo.Local);
    }
    static async Task CopyDirectoryAndOverwrite(string sourceDir, string destDir)
    {
        if (!Directory.Exists(destDir))
            Directory.CreateDirectory(destDir);

        foreach (string filePath in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
        {
            string destFilePath = filePath.Replace(sourceDir, destDir);
            string destFileDir = Path.GetDirectoryName(destFilePath);

            if (!Directory.Exists(destFileDir))
                Directory.CreateDirectory(destFileDir);

            File.Copy(filePath, destFilePath, true);
        }

        foreach (string dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
        {
            string destDirPath = dirPath.Replace(sourceDir, destDir);

            if (!Directory.Exists(destDirPath))
                Directory.CreateDirectory(destDirPath);
        }
    }
    public static void CloneDirectory(string sourceDirName, string destDirName)
    {
        if (Directory.Exists(destDirName))
            Directory.Delete(destDirName, true);

        DirectoryInfo dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
            throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);

        if (!Directory.Exists(destDirName))
            Directory.CreateDirectory(destDirName);

        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string tempPath = Path.Combine(destDirName, file.Name);
            file.CopyTo(tempPath, false);
        }

        DirectoryInfo[] dirs = dir.GetDirectories();
        foreach (DirectoryInfo subdir in dirs)
        {
            string tempPath = Path.Combine(destDirName, subdir.Name);
            CloneDirectory(subdir.FullName, tempPath);
        }
    }

    #endregion
}