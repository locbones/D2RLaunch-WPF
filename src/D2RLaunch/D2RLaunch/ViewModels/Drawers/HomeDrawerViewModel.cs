﻿using Caliburn.Micro;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using D2RLaunch.Models.Enums;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;
using D2RLaunch.Properties;
using Syncfusion.Licensing;
using System.ComponentModel.DataAnnotations;
using System;
using System.Dynamic;
using D2RLaunch.Culture;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using JetBrains.Annotations;
using System.Threading;
using System.Windows;
using D2RLaunch.Extensions;
using D2RLaunch.Models;
using D2RLaunch.ViewModels.Dialogs;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Net;
using System.Text;

namespace D2RLaunch.ViewModels.Drawers;

public class HomeDrawerViewModel : INotifyPropertyChanged
{
    #region ---Static Members---

    private const string TAB_BYTE_CODE = "55AA55AA0100000063000000000000004400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000004A4D0000";
    private ILog _logger = LogManager.GetLogger(typeof(HomeDrawerViewModel));
    private IWindowManager _windowManager;
    private string _launcherDescription = "This application is used to download and configure mods for D2R.";
    private string _launcherTitle = "D2RLaunch";
    private string _modDescription = "Please create a blank mod or download a new mod using the options below.";
    private string _modTitle = "No Mods Detected!";
    private ObservableCollection<KeyValuePair<string, eLanguage>> _languages = new ObservableCollection<KeyValuePair<string, eLanguage>>();
    private KeyValuePair<string, eLanguage> _selectedAppLanguage;
    private ObservableCollection<string> _installedMods;
    private string _selectedMod;
    private bool _mapsComboBoxEnabled;
    private bool _uiComboBoxEnabled;
    private bool _checkingForUpdates;
    private double _downloadProgress;
    private bool _progressBarIsIndeterminate;
    private string _progressStatus;
    private string _downloadProgressString;
    private bool _directTxtEnabled;
    private bool _hdrOpacityFixEnabled;
    private bool _mapRegenEnabled;
    private bool _respecEnabled;
    private ObservableCollection<KeyValuePair<string, eMapLayouts>> _mapLayouts = new ObservableCollection<KeyValuePair<string, eMapLayouts>>();
    private ObservableCollection<KeyValuePair<string, eWindowMode>> _windowMode = new ObservableCollection<KeyValuePair<string, eWindowMode>>();
    private ObservableCollection<KeyValuePair<string, eUiThemes>> _uiThemes = new ObservableCollection<KeyValuePair<string, eUiThemes>>();
    private DispatcherTimer _monsterStatsDispatcherTimer;
    private bool _uiThemeEnabled = true;
    private string _d2rArgs;

    // P/Invoke declarations
    private const int GWL_STYLE = -16;
    private const int GWL_EXSTYLE = -20;
    private const uint WS_CAPTION = 0x00C00000;
    private const uint WS_THICKFRAME = 0x00040000;
    private const uint WS_EX_CLIENTEDGE = 0x00000200;
    private const uint SWP_FRAMECHANGED = 0x0020;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOSIZE = 0x0001;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);
    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetDesktopWindow();
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }

    #endregion

    #region ---Window/Loaded Handlers---

    public HomeDrawerViewModel() //Main Window
    {
        if (Execute.InDesignMode)
        {
            DownloadProgressString = "70%";
            ProgressStatus = "Test Progress Status...";
        }
    }
    public HomeDrawerViewModel(ShellViewModel shellViewModel, IWindowManager windowManager) //Main Window Settings
    {
        ShellViewModel = shellViewModel;
        _windowManager = windowManager;

        _monsterStatsDispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        _monsterStatsDispatcherTimer.Tick += (sender, args) => MonsterStatsDispatcherTimerOnTick(ShellViewModel.UserSettings);
        _monsterStatsDispatcherTimer.Interval = TimeSpan.FromSeconds(15);
    }
    private void MonsterStatsDispatcherTimerOnTick(UserSettings userSettings) //Inject DLL after timed delay
    {
        RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Blizzard Entertainment\Battle.net\Launch Options\BNA");
        object data = key.GetValue("CONNECTION_STRING_CN");

        if (data != null && data.ToString() == "127.0.0.1") //Only inject is we've verified BNET access has been disabled
        {
            Process testrun;
            testrun = Process.GetProcessesByName("D2R")[0];
            if (testrun != null)
            {
                try
                {
                    List<string> MSIPath = null;

                    if (ShellViewModel.UserSettings.MSIFix == true)
                    {
                        //Close MSI Afterburner and Riva Tuner to allow Monster Stats Display to load correctly; restarted after game launch
                        MSIPath = CloseMSIAfterburner("MSIAfterburner"); //Special Function to retrieve MSI path info; don't need others
                        CloseRivaTuner("RTSS");
                        CloseRivaTuner("RTSSHooksLoader64");
                        CloseRivaTuner("EncoderServer");
                        Thread.Sleep(1000);
                    }

                    Injector.Inject(new string[] { "D2R.exe" }, ShellViewModel.GamePath, "D2RHUD.DLL");
                    _logger.Error("Monster Stats: D2RHUD.dll has been loaded");

                    if (ShellViewModel.UserSettings.MSIFix == true)
                    {
                        foreach (string path in MSIPath)
                        {
                            Thread.Sleep(1000);
                            Process.Start(path); //Restart MSI Afterburner (which restarts Riva Tuner)
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
                }
                _monsterStatsDispatcherTimer.Stop();
            }
        }
        else
            _logger.Error("Monster Stats: No D2R Path Data Found!");
    }
    public async Task Initialize() //Load User's Map, Window Mode and UI Theme Settings
    {
        foreach (eMapLayouts mapLayout in Enum.GetValues<eMapLayouts>())
        {
            MapLayouts.Add(new KeyValuePair<string, eMapLayouts>(mapLayout.GetAttributeOfType<DisplayAttribute>().Name, mapLayout));
        }

        foreach (eWindowMode windowMode in Enum.GetValues<eWindowMode>())
        {
            WindowMode.Add(new KeyValuePair<string, eWindowMode>(windowMode.GetAttributeOfType<DisplayAttribute>().Name, windowMode));
        }

        foreach (eUiThemes uiTheme in Enum.GetValues<eUiThemes>())
        {
            UiThemes.Add(new KeyValuePair<string, eUiThemes>(uiTheme.GetAttributeOfType<DisplayAttribute>().Name, uiTheme));
        }

        await InitializeLanguage();
        await InitializeMods();
        GetD2RArgs();
        if (ShellViewModel.UserSettings != null)
        {
            ShellViewModel.UserSettings.MapSeed = "";
            ShellViewModel.UserSettings.MapSeedName = "An Evil Force's Seed: ";
        }

    }
    public async Task InitializeLanguage() //Load User's Audio and Text Language
    {
        eLanguage appLanguage = ((eLanguage)Settings.Default.AppLanguage);
        SelectedAppLanguage = new KeyValuePair<string, eLanguage>(appLanguage.GetAttributeOfType<DisplayAttribute>().Name, appLanguage);

        foreach (eLanguage language in Enum.GetValues<eLanguage>())
        {
            Languages.Add(new KeyValuePair<string, eLanguage>(language.GetAttributeOfType<DisplayAttribute>().Name, language));
        }

        await Translate();
    }
    public async Task InitializeMods() //Load Author-Enabled Mod Settings
    {
        string[] modFolders = Directory.GetDirectories(ShellViewModel.BaseModsFolder);

        InstalledMods = new ObservableCollection<string>(modFolders.Where(m => !m.ToUpperInvariant().Contains("Backup".ToUpperInvariant())).Select(Path.GetFileName));

        if (Directory.Exists(ShellViewModel.BaseSelectedModFolder))
        {
            if (!string.IsNullOrEmpty(Settings.Default.SelectedMod))
            {
                SelectedMod = Settings.Default.SelectedMod;
                ShellViewModel.ModInfo = await Helper.ParseModInfo(ShellViewModel.SelectedModInfoFilePath);

                await Translate();

                if (ShellViewModel.ModInfo == null)
                    return;

                MapsComboBoxEnabled = ShellViewModel.ModInfo.MapLayouts;
                UiComboBoxEnabled = ShellViewModel.ModInfo.UIThemes && (ShellViewModel.ModInfo.Name == "Vanilla++" || ShellViewModel.ModInfo.Name == "ReMoDDeD");
                ShellViewModel.CustomizationsEnabled = ShellViewModel.ModInfo.Customizations;

                //Disable RW Sort+Merged HUD if author enabled without providing template files
                if (!Directory.Exists(Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLaunch/Merged HUD")))
                    ShellViewModel.ModInfo.HudDisplay = false;

                if (!Directory.Exists(Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLaunch/Runeword Sort")))
                    ShellViewModel.ModInfo.RunewordSorting = false;

                string logoPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "D2Rlaunch/Logo.png");
                if (File.Exists(logoPath))
                {
                    string tempPath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
                    File.Copy(logoPath, tempPath, true);
                    ShellViewModel.ModLogo = tempPath;
                }
                else
                    ShellViewModel.ModLogo = "pack://application:,,,/Resources/Images/D2RL_Logo.png";

                await LoadUserSettings();

                await ShellViewModel.StartAutoBackup();

                if (ShellViewModel.ModInfo.Name == "ReMoDDeD")
                {
                    UiThemeEnabled = true;
                    ShellViewModel.WikiEnabled = true;
                    ShellViewModel.ShowItemLevelsEnabled = false;
                    ShellViewModel.SuperTelekinesisEnabled = false;
                    ShellViewModel.SkillBuffIconsEnabled = false;
                    ShellViewModel.SkillIconPackEnabled = false;
                    ShellViewModel.ItemIconDisplayEnabled = false;
                    ShellViewModel.UserSettings.ItemIlvls = 1;
                    ShellViewModel.ExpandedInventoryEnabled = false;
                    ShellViewModel.ExpandedStashEnabled = false;
                    ShellViewModel.ExpandedCubeEnabled = false;
                    ShellViewModel.ExpandedMercEnabled = false;
                    ShellViewModel.ColorDyesEnabled = false;
                }
                else
                {
                    UiThemeEnabled = false;
                    ShellViewModel.WikiEnabled = true;
                    ShellViewModel.UserSettings.UiTheme = 2;

                    ShellViewModel.ShowItemLevelsEnabled = true;
                    ShellViewModel.SuperTelekinesisEnabled = true;
                    ShellViewModel.SkillBuffIconsEnabled = false;
                    ShellViewModel.SkillIconPackEnabled = true;
                    ShellViewModel.ItemIconDisplayEnabled = true;
                    ShellViewModel.ExpandedInventoryEnabled = true;
                    ShellViewModel.ExpandedStashEnabled = true;
                    ShellViewModel.ExpandedCubeEnabled = true;
                    ShellViewModel.ExpandedMercEnabled = true;
                    ShellViewModel.ColorDyesEnabled = true;
                }

                GetD2RArgs();
                DownloadD2RHUDZip();
                //await ApplyUiTheme();
            }
        }
    }
    private async Task LoadUserSettings() //Create User Settings file
    {
        //Protected
        if (Directory.Exists(ShellViewModel.SelectedModDataFolder))
        {
            if (!File.Exists(ShellViewModel.SelectedUserSettingsFilePath))
            {
                if (!File.Exists(ShellViewModel.OldSelectedUserSettingsFilePath))
                    ShellViewModel.UserSettings = await Helper.GetDefaultUserSettings();
                else
                {
                    string[] oldUserSettings = await File.ReadAllLinesAsync(ShellViewModel.OldSelectedUserSettingsFilePath);
                    ShellViewModel.UserSettings = await Helper.ConvertUserSettings(oldUserSettings);
                }
            }
            else
                ShellViewModel.UserSettings = JsonConvert.DeserializeObject<UserSettings>(await File.ReadAllTextAsync(ShellViewModel.SelectedUserSettingsFilePath));
        }
        else //Unprotected
        {
            if (!File.Exists(ShellViewModel.SelectedUserSettingsFilePath.Replace($"{Settings.Default.SelectedMod}.mpq/", "")))
            {
                if (!File.Exists(ShellViewModel.OldSelectedUserSettingsFilePath.Replace($"{Settings.Default.SelectedMod}.mpq/", "")))
                    ShellViewModel.UserSettings = await Helper.GetDefaultUserSettings();
                else
                {
                    string[] oldUserSettings = await File.ReadAllLinesAsync(ShellViewModel.OldSelectedUserSettingsFilePath.Replace($"{Settings.Default.SelectedMod}.mpq/", ""));
                    ShellViewModel.UserSettings = await Helper.ConvertUserSettings(oldUserSettings);
                }
            }
            else
                ShellViewModel.UserSettings = JsonConvert.DeserializeObject<UserSettings>(await File.ReadAllTextAsync(ShellViewModel.SelectedUserSettingsFilePath.Replace($"{Settings.Default.SelectedMod}.mpq/", "")));
        }

        //TODO: Should the autoback up timer be configured here?
        //TODO:_profilehd.json be setup here?
    }

    #endregion

    #region ---Properties---

    public ShellViewModel ShellViewModel { get; }
    public bool UiThemeEnabled
    {
        get => _uiThemeEnabled;
        set
        {
            if (value == _uiThemeEnabled) return;
            _uiThemeEnabled = value;
            OnPropertyChanged();
        }
    }
    public bool DirectTxtEnabled
    {
        get => _directTxtEnabled;
        set
        {
            if (value == _directTxtEnabled) return;
            _directTxtEnabled = value;
            OnPropertyChanged();
        }
    }
    public bool HdrOpacityFixEnabled
    {
        get => _hdrOpacityFixEnabled;
        set
        {
            if (value == _hdrOpacityFixEnabled) return;
            _hdrOpacityFixEnabled = value;
            OnPropertyChanged();
        }
    }
    public bool MapRegenEnabled
    {
        get => _mapRegenEnabled;
        set
        {
            if (value == _mapRegenEnabled) return;
            _mapRegenEnabled = value;
            OnPropertyChanged();
        }
    }
    public bool RespecEnabled
    {
        get => _respecEnabled;
        set
        {
            if (value == _respecEnabled) return;
            _respecEnabled = value;
            OnPropertyChanged();
        }
    }
    public string ProgressStatus
    {
        get => _progressStatus;
        set
        {
            if (value == _progressStatus) return;
            _progressStatus = value;
            OnPropertyChanged();
        }
    }
    public bool ProgressBarIsIndeterminate
    {
        get => _progressBarIsIndeterminate;
        set
        {
            if (value == _progressBarIsIndeterminate) return;
            _progressBarIsIndeterminate = value;
            OnPropertyChanged();
        }
    }
    public string DownloadProgressString
    {
        get => _downloadProgressString;
        set
        {
            if (value == _downloadProgressString) return;
            _downloadProgressString = value;
            OnPropertyChanged();
        }
    }
    public double DownloadProgress
    {
        get => _downloadProgress;
        set
        {
            if (value.Equals(_downloadProgress)) return;
            _downloadProgress = value;
            OnPropertyChanged();
        }
    }
    public bool CheckingForUpdates
    {
        get => _checkingForUpdates;
        set
        {
            if (value == _checkingForUpdates) return;
            _checkingForUpdates = value;
            OnPropertyChanged();
        }
    }
    public bool UiComboBoxEnabled
    {
        get => _uiComboBoxEnabled;
        set
        {
            if (value == _uiComboBoxEnabled) return;
            _uiComboBoxEnabled = value;
            OnPropertyChanged();
        }
    }
    public bool MapsComboBoxEnabled
    {
        get => _mapsComboBoxEnabled;
        set
        {
            if (value == _mapsComboBoxEnabled) return;
            _mapsComboBoxEnabled = value;
            OnPropertyChanged();
        }
    }
    public string SelectedMod
    {
        get => _selectedMod;
        set
        {
            if (value == _selectedMod) return;
            _selectedMod = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<string> InstalledMods
    {
        get => _installedMods;
        set
        {
            if (Equals(value, _installedMods)) return;
            _installedMods = value;
            OnPropertyChanged();
        }
    }
    public KeyValuePair<string, eLanguage> SelectedAppLanguage
    {
        get => _selectedAppLanguage;
        set
        {
            if (value.Equals(_selectedAppLanguage)) return;
            _selectedAppLanguage = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<KeyValuePair<string, eUiThemes>> UiThemes
    {
        get => _uiThemes;
        set
        {
            if (Equals(value, _uiThemes)) return;
            _uiThemes = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<KeyValuePair<string, eMapLayouts>> MapLayouts
    {
        get => _mapLayouts;
        set
        {
            if (Equals(value, _mapLayouts)) return;
            _mapLayouts = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<KeyValuePair<string, eWindowMode>> WindowMode
    {
        get => _windowMode;
        set
        {
            if (Equals(value, _windowMode)) return;
            _windowMode = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<KeyValuePair<string, eLanguage>> Languages
    {
        get => _languages;
        set
        {
            if (Equals(value, _languages)) return;
            _languages = value;
            OnPropertyChanged();
        }
    }
    public string ModTitle
    {
        get => _modTitle;
        set
        {
            if (value == _modTitle)
            {
                return;
            }
            _modTitle = value;
            OnPropertyChanged();
        }
    }
    public string ModDescription
    {
        get => _modDescription;
        set
        {
            if (value == _modDescription)
            {
                return;
            }
            _modDescription = value;
            OnPropertyChanged();
        }
    }
    public string LauncherTitle
    {
        get => _launcherTitle;
        set
        {
            if (value == _launcherTitle)
            {
                return;
            }
            _launcherTitle = value;
            OnPropertyChanged();
        }
    }
    public string LauncherDescription
    {
        get => _launcherDescription;
        set
        {
            if (value == _launcherDescription)
            {
                return;
            }
            _launcherDescription = value;
            OnPropertyChanged();
        }
    }
    public string D2RArgsText
    {
        get => _d2rArgs;
        set
        {
            if (value == _d2rArgs) return;
            _d2rArgs = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region ---Launch Arguments/Game Start---

    [UsedImplicitly]
    public async void OnPlayMod() //Main execution function
    {
        string dllPath = Path.Combine(ShellViewModel.GamePath, "D2RHUD.dll");

        if (ShellViewModel.ModInfo == null)
        {
            return;
        }

        await ApplyHdrFix();
        await ApplyCinematicSkip();
        await ShellViewModel.ApplyModSettings();

        // Unlock or Create SharedStash
        if (ShellViewModel.ModInfo != null)
        {
            string hexString = String.Concat(Enumerable.Repeat(TAB_BYTE_CODE, 4));
            string d2rSavePath = string.Empty;

            if (ShellViewModel.ModInfo.SavePath == "\"../\"")
                d2rSavePath = GetSavePath();
            else
                d2rSavePath = Path.Combine(GetSavePath(), @$"Diablo II Resurrected\Mods\{ShellViewModel.ModInfo.Name}");

            if (!Directory.Exists(d2rSavePath))
                Directory.CreateDirectory(d2rSavePath);

            string sharedStashSoftCorePath = Path.Combine(d2rSavePath, "SharedStashSoftCoreV2.d2i");
            string sharedStashHardCorePath = Path.Combine(d2rSavePath, "SharedStashHardCoreV2.d2i");

            // If stash doesn't exist yet; create a new one with all 7 tabs unlocked
            if (!File.Exists(sharedStashSoftCorePath))
            {
                File.Create(sharedStashSoftCorePath).Close();
                await File.WriteAllBytesAsync(sharedStashSoftCorePath, await Helper.GetResourceByteArray("SharedStashSoftCoreV2.d2i"));
            }
            else
            {
                // Check if stash is unlocked already and unlock if not
                byte[] data = await File.ReadAllBytesAsync(sharedStashSoftCorePath);
                string bitString = BitConverter.ToString(data).Replace("-", string.Empty);

                if (Regex.Matches(bitString, "4A4D").Count == 3)
                {
                    await File.WriteAllBytesAsync(sharedStashSoftCorePath, Helper.StringToByteArray(bitString + hexString));
                    _logger.Error("Startup: Stash Tabs Unlocked - Softcore");
                }
                    
            }

            //Repeat for the hardcore stash
            if (!File.Exists(sharedStashHardCorePath))
            {
                File.Create(sharedStashHardCorePath).Close();
                await File.WriteAllBytesAsync(sharedStashHardCorePath, await Helper.GetResourceByteArray("SharedStashHardCoreV2.d2i"));
            }
            else
            {
                byte[] data = await File.ReadAllBytesAsync(sharedStashHardCorePath); //read file
                string bitString = BitConverter.ToString(data).Replace("-", string.Empty);
                if (Regex.Matches(bitString, "4A4D").Count == 3)
                {
                    await File.WriteAllBytesAsync(sharedStashHardCorePath, Helper.StringToByteArray(bitString + hexString));
                    _logger.Error("Startup: Stash Tabs Unlocked - Hardcore");
                }
                    
            }
        }

        string filePath = System.IO.Path.Combine(ShellViewModel.GamePath, "D2RHUD_Config.txt");

        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
            await File.WriteAllBytesAsync(filePath, await Helper.GetResourceByteArray("Options.MonsterStats.D2RHUD_Config.txt"));
        }

        

        //Load MonsterStats Setting
        switch (ShellViewModel.UserSettings.MonsterHP)
        {
            case 0:
            case 1:
                {
                    string[] displayValue = File.ReadAllLines(filePath);
                    if (displayValue[0] == "Monster Stats: 1")
                        displayValue[0] = displayValue[0].Replace("1", "0");
                    File.WriteAllLines(filePath, displayValue);

                    _monsterStatsDispatcherTimer.Start();
                    _logger.Error("Monster Stats: Basic Timer Started");

                    break;
                }
            case 2:
                {
                    string[] displayValue = File.ReadAllLines(filePath);
                    if (displayValue[0] == "Monster Stats: 1")
                        displayValue[0] = displayValue[0].Replace("1", "0");
                    File.WriteAllLines(filePath, displayValue);

                    _monsterStatsDispatcherTimer.Start();
                    _logger.Error("Monster Stats: Basic Timer Started");

                    break;
                }
            case 3:
                {
                    string[] displayValue = File.ReadAllLines(filePath);
                    if (displayValue[0] == "Monster Stats: 0")
                        displayValue[0] = displayValue[0].Replace("0", "1");
                    File.WriteAllLines(filePath, displayValue);

                    _monsterStatsDispatcherTimer.Start();
                    _logger.Error("Monster Stats: Advanced Timer Started");

                    break;
                }
            case 4:
                {
                    string[] displayValue = File.ReadAllLines(filePath);
                    if (displayValue[0] == "Monster Stats: 0")
                        displayValue[0] = displayValue[0].Replace("0", "1");
                    File.WriteAllLines(filePath, displayValue);

                    _monsterStatsDispatcherTimer.Start();
                    _logger.Error("Monster Stats: Advanced Timer Started");

                    break;
                }
        }


        ShellViewModel.DisableBNetConnection();

        //Add Exocet Font to D2R base Folder for Monster Stat Display (mod agnostic)
        if (!File.Exists("../Exocet.otf"))
        {
            byte[] font = await Helper.GetResourceByteArray($"Fonts.0.otf");
            await File.WriteAllBytesAsync(ShellViewModel.GamePath + "/Exocet.otf", font);
        }

        //Start the mod
        string d2rArgs = ShellViewModel.UserSettings.CurrentD2RArgs;
        ProcessStartInfo startInfo = new ProcessStartInfo(Path.Combine(ShellViewModel.GamePath, "D2R.exe"), d2rArgs);
        startInfo.WorkingDirectory = @".\";
        try
        {
            Process.Start(startInfo); //Start the game
            ShellViewModel.UserSettings.MapLayout = 0;

            if (ShellViewModel.UserSettings.WindowMode == 2)
            {
                Thread.Sleep(10000);
                ExtendDiabloWindowToFullScreen();
            }
            if (ShellViewModel.UserSettings.WindowMode == 3)
            {
                Thread.Sleep(10000);
                RemoveDiabloWindowTitleBarAndBorder();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error starting application: {ex}");
            _logger.Error($"Monster Stats: FAIL - GamePath:{ShellViewModel.GamePath}");
        }

        _logger.Error($"\n\n--------------------\nMod Name: {ShellViewModel.ModInfo.Name}\nGame Path: {ShellViewModel.GamePath}\nSave Path: {ShellViewModel.SaveFilesFilePath}\nLaunch Arguments: {ShellViewModel.UserSettings.CurrentD2RArgs}\n\nAudio Language: {ShellViewModel.UserSettings.AudioLanguage}\nText Language: {ShellViewModel.UserSettings.TextLanguage}\nUI Theme: {ShellViewModel.UserSettings.UiTheme}\nWindow Mode: {ShellViewModel.UserSettings.WindowMode}\nHDR Fix: {ShellViewModel.UserSettings.HdrFix}\n\nFont: {ShellViewModel.UserSettings.Font}\nBackups: {ShellViewModel.UserSettings.AutoBackups}\nPersonalized Tabs: {ShellViewModel.UserSettings.PersonalizedStashTabs}\nExpanded Cube: {ShellViewModel.UserSettings.ExpandedCube}\nExpanded Inventory: {ShellViewModel.UserSettings.ExpandedInventory}\nExpanded Merc: {ShellViewModel.UserSettings.ExpandedMerc}\nExpanded Stash: {ShellViewModel.UserSettings.ExpandedStash}\nBuff Icons: {ShellViewModel.UserSettings.BuffIcons}\nMonster Display: {ShellViewModel.UserSettings.MonsterStatsDisplay}\nSkill Icons: {ShellViewModel.UserSettings.SkillIcons}\nMerc Identifier: {ShellViewModel.UserSettings.MercIcons}\nItem Levels: {ShellViewModel.UserSettings.ItemIlvls}\nRune Display: {ShellViewModel.UserSettings.RuneDisplay}\nHide Helmets: {ShellViewModel.UserSettings.HideHelmets}\nItem Display: {ShellViewModel.UserSettings.ItemIcons}\nSuper Telekinesis: {ShellViewModel.UserSettings.SuperTelekinesis}\nColor Dyes: {ShellViewModel.UserSettings.ColorDye}\nCinematic Subtitles: {ShellViewModel.UserSettings.CinematicSubs}\nRuneword Sorting: {ShellViewModel.UserSettings.RunewordSorting}\nMerged HUD: {ShellViewModel.UserSettings.HudDesign}\n--------------------");

    }
    public string GetD2RArgs() //Determine whether to run the mod with -txt or not
    {
        string args = string.Empty;
        string regenArg = ShellViewModel?.UserSettings?.ResetMaps ?? false ? " -resetofflinemaps" : string.Empty;
        string respecArg = ShellViewModel?.UserSettings?.InfiniteRespec ?? false ? " -enablerespec" : string.Empty;
        string windowedArg = ShellViewModel?.UserSettings?.WindowMode >= 1 ? " -windowed" : string.Empty;
        string mapLayoutArg = GetMapLayoutArg();

        string excelDir = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/excel");

        if (Directory.Exists(excelDir))
        {
            int binFileCount = Directory.GetFiles(excelDir, "*.bin").Length;
            int txtFileCount = Directory.GetFiles(excelDir, "*.txt").Length;

            if (binFileCount >= 83 && txtFileCount >= 10)
                args = $"-mod {ShellViewModel.ModInfo.Name} -txt";
            else if (binFileCount >= 83 && txtFileCount < 10)
                args = $"-mod {ShellViewModel.ModInfo.Name}";
            else if (binFileCount < 83 && txtFileCount >= 1)
                args = $"-mod {ShellViewModel.ModInfo.Name} -txt";
        }
        else 
        {
            if (ShellViewModel.ModInfo != null)
                args = $"-mod {ShellViewModel.ModInfo.Name} -txt";
            else
                args = "";
        }
        
        string mArgs = args;

        if (SelectedMod == "ReMoDDeD")
            mArgs = args.Replace(" -txt", "");

        args = $"{mArgs}{regenArg}{respecArg}{mapLayoutArg}{windowedArg}";

        if (ShellViewModel != null && ShellViewModel.SelectedModDataFolder != null)
        {
            string filePath = Path.Combine(ShellViewModel.SelectedModDataFolder, @"local\macui\d2logo.pcx");

            if (ShellViewModel.UserSettings != null)
            {
                if (File.Exists(filePath))
                    ShellViewModel.UserSettings.FastLoad = "On";
                else
                    ShellViewModel.UserSettings.FastLoad = "Off";

                ShellViewModel.UserSettings.CurrentD2RArgs = args;
            }

        }

        return args;
    }
    private string GetMapLayoutArg() //Convert User's map choice to seed values
    {
        if (ShellViewModel?.UserSettings == null)
            return string.Empty;

        string arg = string.Empty;

        switch ((eMapLayouts)ShellViewModel.UserSettings.MapLayout)
        {
            case eMapLayouts.Default:
                    return "";
            case eMapLayouts.Tower:
                    return " -seed 1112";
            case eMapLayouts.Catacombs:
                    return " -seed 348294647";
            case eMapLayouts.AncientTunnels:
                    return " -seed 1111";
            case eMapLayouts.LowerKurast:
                    return " -seed 1460994795";
            case eMapLayouts.DuranceOfHate:
                    return " -seed 1113";
            case eMapLayouts.Hellforge:
                    return " -seed 100";
            case eMapLayouts.WorldstoneKeep:
                    return " -seed 1104";
            case eMapLayouts.Cheater:
                    return " -seed 1056279548";
            default:
                    return "";
        }
    }
    private void ExtendDiabloWindowToFullScreen() //Fullscreen Borderless
    {
        Process[] processes = Process.GetProcesses();
        bool foundDiabloWindow = false;

        foreach (Process process in processes)
        {
            IntPtr mainWindowHandle = process.MainWindowHandle;
            if (mainWindowHandle != IntPtr.Zero)
            {
                string windowTitle = process.MainWindowTitle.ToLower();
                if (windowTitle.Contains("diablo"))
                {
                    foundDiabloWindow = true;

                    // Remove title bar, border, and client edge
                    uint style = GetWindowLong(mainWindowHandle, GWL_STYLE);
                    uint exStyle = GetWindowLong(mainWindowHandle, GWL_EXSTYLE);

                    style &= ~(WS_CAPTION | WS_THICKFRAME); // Remove title bar and resizable border
                    exStyle &= ~WS_EX_CLIENTEDGE; // Remove client edge

                    SetWindowLong(mainWindowHandle, GWL_STYLE, style);
                    SetWindowLong(mainWindowHandle, GWL_EXSTYLE, exStyle);

                    // Update window position and size to cover the entire screen
                    IntPtr desktopHandle = GetDesktopWindow();
                    RECT desktopRect;
                    GetWindowRect(desktopHandle, out desktopRect);

                    SetWindowPos(mainWindowHandle, IntPtr.Zero, desktopRect.Left, desktopRect.Top, desktopRect.Right - desktopRect.Left, desktopRect.Bottom - desktopRect.Top, SWP_FRAMECHANGED);

                    //MessageBox.Show("Diablo window extended to full screen resolution.");
                    break; // Once we find a Diablo window, no need to continue looping
                }
            }
        }
    }
    private void RemoveDiabloWindowTitleBarAndBorder() //Windowed Borderless
    {
        Process[] processes = Process.GetProcesses();
        bool foundDiabloWindow = false;

        foreach (Process process in processes)
        {
            IntPtr mainWindowHandle = process.MainWindowHandle;
            if (mainWindowHandle != IntPtr.Zero)
            {
                string windowTitle = process.MainWindowTitle.ToLower();
                if (windowTitle.Contains("diablo"))
                {
                    foundDiabloWindow = true;

                    // Modify the window style to remove the title bar and border
                    uint style = GetWindowLong(mainWindowHandle, GWL_STYLE);
                    uint exStyle = GetWindowLong(mainWindowHandle, GWL_EXSTYLE);

                    style &= ~(WS_CAPTION | WS_THICKFRAME); // Remove title bar and resizable border
                    exStyle &= ~WS_EX_CLIENTEDGE; // Remove client edge

                    SetWindowLong(mainWindowHandle, GWL_STYLE, style);
                    SetWindowLong(mainWindowHandle, GWL_EXSTYLE, exStyle);

                    // Update window position to refresh the appearance
                    SetWindowPos(mainWindowHandle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_FRAMECHANGED);

                    //MessageBox.Show("Title bar, border, and client edge removed from Diablo window.");
                    break; // Once we find a Diablo window, no need to continue looping
                }
            }
        }
    }
    public List<string> CloseMSIAfterburner(string processName) //Used to find path info and close MSI Afterburner
    {
        List<string> exePaths = new List<string>();

        try
        {
            // Get all processes by name
            Process[] processes = Process.GetProcessesByName(processName);

            foreach (Process process in processes)
            {
                try
                {
                    exePaths.Add(process.MainModule.FileName);
                    process.Kill();
                }
                catch (AccessViolationException)
                {
                    Console.WriteLine($"Access denied to process {processName}. Unable to retrieve the file path.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error accessing process {processName}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        return exePaths;
    }
    public void CloseRivaTuner(string processName) //Used to find and close Riva Tuner
    {
        try
        {
            Process[] processes = Process.GetProcessesByName(processName);

            foreach (Process process in processes)
            {
                try
                {
                    _logger.Error($"Closing process: {processName}");
                    _logger.Error($"Executable Path: {process.MainModule.FileName}");
                    process.Kill();
                }
                catch (AccessViolationException)
                {
                    _logger.Error($"Access denied to process {processName}. Unable to retrieve the file path.");
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error accessing process {processName}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
    public async void DownloadD2RHUDZip() //Download Expanded File Package
    {
        string url = "https://github.com/locbones/D2RHud/archive/refs/heads/main.zip";
        string savePath = "D2RHUD.zip";
        string extractPathTemp = "./";

        if (File.Exists(savePath))
            File.Delete(savePath);

        using (WebClient client = new WebClient())
        {
            client.DownloadFileCompleted += (sender, e) =>
            {
                try
                {
                    if (e.Error == null)
                    {
                        ZipFile.ExtractToDirectory(savePath, extractPathTemp);
                        _logger.Error("Monster Stats: D2RHUD Downloaded and Extracted");

                        File.Copy(extractPathTemp + "D2RHud-main/x64/Release/D2RHUD.dll", ShellViewModel.GamePath + "/D2RHUD.dll", true);
                        _logger.Error($"Monster Stats: D2RHUD.dll copied to {ShellViewModel.GamePath}");

                        File.Delete(savePath);
                        Directory.Delete(extractPathTemp + "/D2Rhud-main", true);
                        _logger.Error($"Monster Stats: D2RHUD Cleanup Completed");
                    }
                    else
                        MessageBox.Show($"An error occurred during download: {e.Error.Message}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            };

            try
            {
                client.DownloadFileAsync(new Uri(url), savePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
    }

    #endregion

    #region ---Translations---

    private async Task Translate() //Parse and Prepare News Messages for Translation
    {
        if (ShellViewModel.ModInfo != null)
        {
            if (string.IsNullOrEmpty(ShellViewModel.ModInfo.ModTitle))
            {
                ModTitle = "No News Found!";
                ModDescription = "The mod author has not specified any news messages for this mod";
                LauncherTitle = "Add D2RLaunch Support";
                LauncherDescription = "Unlock one-click mod updates, additional QoL controls, live news display and more. It's as easy as editing a single already included file to add D2RLaunch support to your mod. Visit the D2RModding Discord for more info.";

                return;
            }

            if (SelectedAppLanguage.Value == eLanguage.English)
            {
                ModTitle = ShellViewModel.ModInfo.ModTitle.Trim().Replace("\"", "");
                ModDescription = ShellViewModel.ModInfo.ModDescription.Trim().Replace("|| ", ".").Replace(@"\u0026", ". ").Replace("\\n", Environment.NewLine);

                LauncherTitle = ShellViewModel.ModInfo.NewsTitle.Trim().Replace("\"", "");
                //LauncherDescription = ShellViewModel.ModInfo.NewsDescription.Trim().Replace("\"", "");
                LauncherDescription = ShellViewModel.ModInfo.NewsDescription.Trim().Replace("|| ", ".").Replace(@"\u0026", ". ").Replace("\\n", Environment.NewLine);

                return;
            }
            string pattern = @"(?<![0-9])\.(?![0-9])"; // Matches a period not surrounded by digits

            string modTitle = Regex.Replace(ShellViewModel.ModInfo.ModTitle.Trim().Replace("\"", ""), pattern, "||");
            string modDescription = Regex.Replace(ShellViewModel.ModInfo.ModDescription.Trim().Replace("\"", ""), pattern, "||");
            string launcherTitle = Regex.Replace(ShellViewModel.ModInfo.NewsTitle.Trim().Replace("\"", ""), pattern, "||");
            string launcherDescription = Regex.Replace(ShellViewModel.ModInfo.NewsDescription.Trim().Replace("\"", ""), pattern, "||");

            try
            {
                ModTitle = await TranslateGoogleAsync(modTitle);
                ModDescription = await TranslateGoogleAsync(modDescription.Replace("|| ", ".").Replace(@"\u0026", ". "));
                LauncherTitle = await TranslateGoogleAsync(launcherTitle.Replace("|| ", ".").Replace(@"\u0026", ". ").Replace("\\n", Environment.NewLine));
                //LauncherDescription = await TranslateGoogleAsync(launcherDescription.Replace("|| ", ".").Replace(@"\u0026", ". "));
                LauncherDescription = await TranslateGoogleAsync(launcherDescription.Replace("|| ", ".").Replace(@"\u0026", ". ").Replace("\\n", Environment.NewLine));
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to translate test with google translate.");
                _logger.Error(ex);
            }
        }
        else
        {
            try
            {
                ModTitle = await TranslateGoogleAsync(ModTitle);
                ModDescription = await TranslateGoogleAsync(ModDescription);
                LauncherTitle = await TranslateGoogleAsync(LauncherTitle);
                LauncherDescription = await TranslateGoogleAsync(LauncherDescription);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to translate test with google translate.");
                _logger.Error(ex);
            }
        }
    }
    private async Task<string> TranslateGoogleAsync(string text) //Function to convert text without API or quota restrictions
    {
        try
        {
            text = Uri.EscapeDataString(text);
            string uri = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl={Resources.Culture.Name}&dt=t&q={text}";

            using HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            string translation = responseBody.Split('"')[1].Trim();
            return translation;
        }
        catch (Exception ex)
        {
            _logger.Error($"Translation failed for: {text}");
            _logger.Error(ex);
            return null;
        }
    }
    [UsedImplicitly]
    public async void OnTextLanguageSelectionChanged() //Update registry with Language Selection
    {
        if (ShellViewModel.UserSettings != null)
        {
            RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey(@"Software\Blizzard Entertainment\Battle.net\Launch Options\OSI", true) ?? throw new Exception("Failed to find registry key");

            switch (ShellViewModel.UserSettings.TextLanguage)
            {
                case 0:
                    key.SetValue("LOCALE", "enUS");
                    break;
                case 1:
                    key.SetValue("LOCALE", "deDE");
                    break;
                case 2:
                    key.SetValue("LOCALE", "esES");
                    break;
                case 3:
                    key.SetValue("LOCALE", "esMX");
                    break;
                case 4:
                    key.SetValue("LOCALE", "frFR");
                    break;
                case 5:
                    key.SetValue("LOCALE", "itIT");
                    break;
                case 6:
                    key.SetValue("LOCALE", "jaJP");
                    break;
                case 7:
                    key.SetValue("LOCALE", "koKR");
                    break;
                case 8:
                    key.SetValue("LOCALE", "plPL");
                    break;
                case 9:
                    key.SetValue("LOCALE", "ptBR");
                    break;
                case 10:
                    key.SetValue("LOCALE", "ruRU");
                    break;
                case 11:
                    key.SetValue("LOCALE", "zhCN");
                    break;
                case 12:
                    key.SetValue("LOCALE", "zhTW");
                    break;
                default:
                    key.SetValue("LOCALE", "enUS");
                    break;
            }
            key.Close();
        }
    }
    [UsedImplicitly]
    public async void OnAudioLanguageSelectionChanged() //Update registry with Language Selection
    {
        if (ShellViewModel.UserSettings != null)
        {
            RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey(@"Software\Blizzard Entertainment\Battle.net\Launch Options\OSI", true) ?? throw new Exception("Failed to find registry key");

            switch (ShellViewModel.UserSettings.AudioLanguage)
            {
                case 0:
                    key.SetValue("LOCALE_AUDIO", "enUS");
                    break;
                case 1:
                    key.SetValue("LOCALE_AUDIO", "deDE");
                    break;
                case 2:
                    key.SetValue("LOCALE_AUDIO", "esES");
                    break;
                case 3:
                    key.SetValue("LOCALE_AUDIO", "esMX");
                    break;
                case 4:
                    key.SetValue("LOCALE_AUDIO", "frFR");
                    break;
                case 5:
                    key.SetValue("LOCALE_AUDIO", "itIT");
                    break;
                case 6:
                    key.SetValue("LOCALE_AUDIO", "jaJP");
                    break;
                case 7:
                    key.SetValue("LOCALE_AUDIO", "koKR");
                    break;
                case 8:
                    key.SetValue("LOCALE_AUDIO", "plPL");
                    break;
                case 9:
                    key.SetValue("LOCALE_AUDIO", "ptBR");
                    break;
                case 10:
                    key.SetValue("LOCALE_AUDIO", "ruRU");
                    break;
                case 11:
                    key.SetValue("LOCALE_AUDIO", "zhCN");
                    break;
                case 12:
                    key.SetValue("LOCALE_AUDIO", "zhTW");
                    break;
                default:
                    key.SetValue("LOCALE_AUDIO", "enUS");
                    break;
            }
            key.Close();
        }
    }
    [UsedImplicitly]
    public async void OnAppLanguageSelectionChanged() //Update app text with Language Selection
    {
        Settings.Default.AppLanguage = (int)SelectedAppLanguage.Value;
        Settings.Default.Save();

        if (!string.IsNullOrEmpty(SelectedAppLanguage.Key))
        {
            CultureInfo culture = new CultureInfo(SelectedAppLanguage.Key.Split(' ')[1].Trim(new[] { '(', ')' }));
            CultureResources.ChangeCulture(culture);
        }

        await Translate();
    }

    #endregion

    #region ---Button/Checkbox Controls---

    private async Task ApplyHdrFix()
    {
        string profileHdJsonPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/ui/layouts/_profilehd.json");

        if (!File.Exists(profileHdJsonPath))
            Helper.ExtractFileFromCasc(ShellViewModel.GamePath, @"data:data\global\ui\layouts\_profilehd.json", ShellViewModel.SelectedModDataFolder, "data:data");

        try
        {
            string backgroundColorNormal = "\"backgroundColor\": [ 0, 0, 0, 0.75 ],";
            string backgroundColorFix = "\"backgroundColor\": [ 0, 0, 0, 0.95 ],";
            string inGameBackgroundColorNormal = "\"inGameBackgroundColor\": [ 0, 0, 0, 0.6 ],";
            string inGameBackgroundColorFix = "\"inGameBackgroundColor\": [ 0, 0, 0, 0.95 ],";
            string fileContent = File.ReadAllText(profileHdJsonPath);

            if (ShellViewModel.UserSettings.HdrFix)
            {
                fileContent = fileContent.Replace(backgroundColorNormal, backgroundColorFix);
                fileContent = fileContent.Replace(inGameBackgroundColorNormal, inGameBackgroundColorFix);
                await File.WriteAllTextAsync(profileHdJsonPath, fileContent);
            }
            else
            {
                fileContent = fileContent.Replace(backgroundColorFix, backgroundColorNormal);
                fileContent = fileContent.Replace(inGameBackgroundColorFix, inGameBackgroundColorNormal);
                await File.WriteAllTextAsync(profileHdJsonPath, fileContent);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex);
            MessageBox.Show(ex.Message);
        }
    }
    private async Task ApplyCinematicSkip()
    {
        string videoPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "hd/global/video");

        if (!File.Exists(videoPath + "/act2/act02start.webm"))
        {
            if (ShellViewModel.UserSettings.skipCinematics)
            {
                if (!Directory.Exists(videoPath))
                    Directory.CreateDirectory(videoPath);
                if (!Directory.Exists(videoPath + "/act2"))
                    Directory.CreateDirectory(videoPath + "/act2");
                if (!Directory.Exists(videoPath + "/act3"))
                    Directory.CreateDirectory(videoPath + "/act3");
                if (!Directory.Exists(videoPath + "/act4"))
                    Directory.CreateDirectory(videoPath + "/act4");
                if (!Directory.Exists(videoPath + "/act5"))
                    Directory.CreateDirectory(videoPath + "/act5");

                File.Create(videoPath + "/act2/act02start.webm").Close();
                File.Create(videoPath + "/act3/act03start.webm").Close();
                File.Create(videoPath + "/act4/act04start.webm").Close();
                File.Create(videoPath + "/act4/act04end.webm").Close();
                File.Create(videoPath + "/act5/d2x_out.webm").Close();
            }        
        }
    }
    private async Task ApplyUiTheme()
    {
        string layoutPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/ui/layouts");
        string layoutExpandedPath = Path.Combine(ShellViewModel.SelectedModDataFolder, @"D2RLaunch/UI Theme/expanded/layouts");
        string layoutRemoddedPath = Path.Combine(ShellViewModel.SelectedModDataFolder, @"D2RLaunch/UI Theme/remodded/layouts");
        string uiPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/ui");
        string uiExpandedPath = Path.Combine(ShellViewModel.SelectedModDataFolder, @"D2RLaunch/UI Theme/Expanded/ui");

        if (ShellViewModel.ModInfo.Name == "ReMoDDeD")
            await File.WriteAllBytesAsync(Path.Combine(layoutPath, "bankexpansionlayouthd.json"), await Helper.GetResourceByteArray("Options.PersonalizedTabs.stash_rmd"));
        if (ShellViewModel.ModInfo.Name == "Vanilla++")
            await File.WriteAllBytesAsync(Path.Combine(layoutPath, "bankexpansionlayouthd.json"), await Helper.GetResourceByteArray("Options.PersonalizedTabs.stash_vnp"));

        if (Directory.Exists(layoutRemoddedPath) || Directory.Exists(layoutExpandedPath))
        {
            if (Settings.Default.SelectedMod != "MyCustomMod")
            {
                switch ((eUiThemes)ShellViewModel.UserSettings.UiTheme)
                {
                    case eUiThemes.Standard:
                        {
                            if (Directory.Exists(layoutPath))
                            {
                                Directory.Delete(layoutPath, true);
                                await Helper.CloneDirectory(layoutExpandedPath, layoutPath);
                            }
                            if (Directory.Exists(uiPath + "controller"))
                            {
                                Directory.Delete(uiPath + "controller", true);
                                await Helper.CloneDirectory(uiExpandedPath + "controller", uiPath + "controller");
                            }
                            if (Directory.Exists(uiPath + "panel"))
                            {
                                Directory.Delete(uiPath + "panel", true);
                                await Helper.CloneDirectory(uiExpandedPath + "panel", uiExpandedPath + "panel");
                            }
                            break;
                        }
                    case eUiThemes.ReMoDDeD1:
                        {
                            if (Directory.Exists(layoutPath))
                            {
                                Directory.Delete(layoutPath, true);
                                await Helper.CloneDirectory(layoutRemoddedPath, layoutPath);

                                string[] searchStrings = { "_B\"", "_P\"", "_Y\"", "_G\"", "_D\"" };

                                if (Directory.Exists(layoutPath))
                                {
                                    foreach (string file in Directory.GetFiles(layoutPath, "*.json*", SearchOption.AllDirectories))
                                    {
                                        ReplaceStringsInFile(file, searchStrings, "_R\"");
                                    }
                                }
                            }
                            break;
                        }
                    case eUiThemes.ReMoDDeD2:
                        {
                            if (Directory.Exists(layoutPath))
                            {
                                Directory.Delete(layoutPath, true);
                                await Helper.CloneDirectory(layoutRemoddedPath, layoutPath);

                                string[] searchStrings = { "_R\"", "_P\"", "_Y\"", "_G\"", "_D\"" };

                                if (Directory.Exists(layoutPath))
                                {
                                    foreach (string file in Directory.GetFiles(layoutPath, "*.json*", SearchOption.AllDirectories))
                                    {
                                        ReplaceStringsInFile(file, searchStrings, "_B\"");
                                    }
                                }
                            }
                            break;
                        }
                    case eUiThemes.ReMoDDeD3:
                        {
                            if (Directory.Exists(layoutPath))
                            {
                                Directory.Delete(layoutPath, true);
                                await Helper.CloneDirectory(layoutRemoddedPath, layoutPath);

                                string[] searchStrings = { "_R\"", "_B\"", "_Y\"", "_G\"", "_D\"" };

                                if (Directory.Exists(layoutPath))
                                {
                                    foreach (string file in Directory.GetFiles(layoutPath, "*.json*", SearchOption.AllDirectories))
                                    {
                                        ReplaceStringsInFile(file, searchStrings, "_P\"");
                                    }
                                }
                            }
                            break;
                        }
                    case eUiThemes.ReMoDDeD4:
                        {
                            if (Directory.Exists(layoutPath))
                            {
                                Directory.Delete(layoutPath, true);
                                await Helper.CloneDirectory(layoutRemoddedPath, layoutPath);

                                string[] searchStrings = { "_R\"", "_B\"", "_P\"", "_G\"", "_D\"" };

                                if (Directory.Exists(layoutPath))
                                {
                                    foreach (string file in Directory.GetFiles(layoutPath, "*.json*", SearchOption.AllDirectories))
                                    {
                                        ReplaceStringsInFile(file, searchStrings, "_Y\"");
                                    }
                                }
                            }
                            break;
                        }
                    case eUiThemes.ReMoDDeD5:
                        {
                            if (Directory.Exists(layoutPath))
                            {
                                Directory.Delete(layoutPath, true);
                                await Helper.CloneDirectory(layoutRemoddedPath, layoutPath);

                                string[] searchStrings = { "_R\"", "_B\"", "_P\"", "_Y\"", "_D\"" };

                                if (Directory.Exists(layoutPath))
                                {
                                    foreach (string file in Directory.GetFiles(layoutPath, "*.json*", SearchOption.AllDirectories))
                                    {
                                        ReplaceStringsInFile(file, searchStrings, "_G\"");
                                    }
                                }
                            }
                            break;
                        }
                    case eUiThemes.ReMoDDeD6:
                        {
                            if (Directory.Exists(layoutPath))
                            {
                                Directory.Delete(layoutPath, true);
                                await Helper.CloneDirectory(layoutRemoddedPath, layoutPath);

                                string[] searchStrings = { "_R\"", "_B\"", "_P\"", "_Y\"", "_G\"" };

                                if (Directory.Exists(layoutPath))
                                {
                                    foreach (string file in Directory.GetFiles(layoutPath, "*.json*", SearchOption.AllDirectories))
                                    {
                                        ReplaceStringsInFile(file, searchStrings, "_D\"");
                                    }
                                }
                            }
                            break;
                        }
                }
            }
        }

        if (ShellViewModel.ModInfo.Name == "Vanilla++" || ShellViewModel.ModInfo.Name == "ReMoDDeD")
        {
            if (File.Exists(Path.Combine(ShellViewModel.SelectedModDataFolder, @"D2RLaunch/UI Theme/bankexpansionlayouthd.json")))
            {
                File.Delete(Path.Combine(ShellViewModel.SelectedModDataFolder, "global/ui/layouts/bankexpansionlayouthd.json"));
                File.Copy(Path.Combine(ShellViewModel.SelectedModDataFolder, @"D2RLaunch/UI Theme/bankexpansionlayouthd.json"), Path.Combine(ShellViewModel.SelectedModDataFolder, "global/ui/layouts/bankexpansionlayouthd.json"));
            }
            else
            {
                if ((ePersonalizedStashTabs)ShellViewModel.UserSettings.PersonalizedStashTabs == ePersonalizedStashTabs.Enabled)
                    File.Copy(Path.Combine(ShellViewModel.SelectedModDataFolder, "global/ui/layouts/bankexpansionlayouthd.json"), Path.Combine(ShellViewModel.SelectedModDataFolder, @"D2RLaunch/UI Theme/bankexpansionlayouthd.json"));
            }
        }
    }

    static void ReplaceStringsInFile(string filePath, string[] searchStrings, string replacementString)
    {
        try
        {
            string content = File.ReadAllText(filePath, Encoding.UTF8);
            bool modified = false;

            foreach (string searchString in searchStrings)
            {
                if (content.Contains(searchString))
                {
                    content = content.Replace(searchString, replacementString);
                    modified = true;
                }
            }

            if (modified)
            {
                File.WriteAllText(filePath, content, Encoding.UTF8);
                Console.WriteLine($"Updated: {filePath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing file {filePath}: {ex.Message}");
        }
    }

    [UsedImplicitly]
    public async void OnMapsHelp()
    {
        MessageBox.Show("These options let you force specific map layouts so you can roll that 'perfect' map anytime you want. Details explained below:\n\nTower: The tower entrance is on the same screen as your waypoint.\n\nCatacombs: Levels 3 and 4 are less than 3 screens away\n\nAncient Tunnels: Entrance is 1 screen away from your waypoint\n\nLower Kurast: Very favorable super chest pattern near your waypoint\n\nDurance of Hate: Level 3 entrance is one teleport away from waypoint.\n\nHellforge: Forge is at closest spawn to your waypoint\n\nWorldstone Keep: Level 3 and 4 are right next to each other\n\nI'm a Cheater: Almost all entrances are absurdly close with a perfect LK pattern by the waypoint. You're basically just cheating now.\n\n\nNOTE: Lower Kurast and I'm a Cheater options are only available on Vanilla++.");
    }
    [UsedImplicitly]
    public async void OnMSIFixHelp()
    {
        MessageBox.Show("Use this feature if you meet these conditions:\n- Using either of the 'Advanced' Monster Stats Display Options\n- Using MSI Afterburner (Riva Tuner) for in-game overlays\n\nThis will restart the apps to avoid loading conflicts; requires:\n- D2RLaunch must be ran as Administrator\n- Change MSI Settings to 'Start App Minimized' (for QoL purposes)");
    }
    [UsedImplicitly]
    public async Task OnCheckForUpdates()
    {
        if (string.IsNullOrEmpty(Settings.Default.SelectedMod))
        {
            MessageBox.Show("Please select a mod first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        CheckingForUpdates = true;
        ProgressBarIsIndeterminate = true;
        ProgressStatus = "Checking for updates...";


        string tempPath = Path.GetTempPath();
        string tempModInfoPath = Path.Combine(tempPath, "modinfo.json");
        string version = string.Empty;

        if (!File.Exists(ShellViewModel.SelectedModVersionFilePath))
        {
            if (ShellViewModel.ModInfo == null)
            {
                MessageBox.Show("Could not parse ModInfo.json!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CheckingForUpdates = false;
                return;
            }

            version = ShellViewModel.ModInfo.ModVersion;
        }
        else
        {
            version = await File.ReadAllTextAsync(ShellViewModel.SelectedModVersionFilePath);
        }

        // Seting up the http client used to download the data
        using HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromMinutes(5);

        //Download remote modinfo.json
        try
        {
            // Create a file stream to store the downloaded data.
            // This really can be any type of writeable stream.
            await using FileStream file = new FileStream(tempModInfoPath, FileMode.Create, FileAccess.Write, FileShare.None);

            await Execute.OnUIThreadAsync(async () => { await client.DownloadAsync(ShellViewModel.ModInfo.ModConfigDownloadLink, file, null, CancellationToken.None); });

            file.Close();
            await file.DisposeAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex);
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        ModInfo tempModInfo = await Helper.ParseModInfo(tempModInfoPath);

        File.Delete(tempModInfoPath);

        if (tempModInfo != null && SelectedMod != "MyCustomMod")
        {
            if (version == tempModInfo.ModVersion)
            {
                MessageBox.Show("No updates available.", "Update", MessageBoxButton.OK);
                CheckingForUpdates = false;
                return;
            }

            MessageBox.Show($"{Helper.GetCultureString("Version1")} {version}\n {Helper.GetCultureString("Version2")} {tempModInfo.ModVersion}",
                            Resources.ResourceManager.GetString("VersionRdy"), MessageBoxButton.OK);

            //Backup
            if (MessageBox.Show($"{Helper.GetCultureString("ModUpdateRdy").Replace("\\n", Environment.NewLine)}", "Backup Option", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ProgressStatus = Helper.GetCultureString("UpdateBackup");

                if (Directory.Exists(ShellViewModel.BaseSelectedModFolder))
                {
                    try
                    {
                        string backupPath = Path.Combine(ShellViewModel.BaseModsFolder, $"{Settings.Default.SelectedMod}(Backup-{ShellViewModel.ModInfo.ModVersion.Replace(".", "-")})");
                        if (Directory.Exists(backupPath))
                            Directory.Delete(backupPath, true);

                        await Task.Run(async () => { await Helper.CloneDirectory(ShellViewModel.BaseSelectedModFolder, backupPath); });
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                        MessageBox.Show(Helper.GetCultureString("UpdateBackupError"));
                    }

                }
                else
                {
                    MessageBox.Show(Helper.GetCultureString("UpdateBackupError"));
                }
            }

            //Download Update
            string tempUpdatePath = Path.Combine(tempPath, "Update.zip");
            ProgressBarIsIndeterminate = false;
            ProgressStatus = Helper.GetCultureString("UpdateBegin");

            try
            {
                Progress<double> progress = new Progress<double>();

                progress.ProgressChanged += (sender, args) =>
                {
                    Execute.OnUIThread(() =>
                    {
                        if (args == -1)
                        {
                            DownloadProgress = 0;
                            DownloadProgressString = string.Empty;
                            ProgressBarIsIndeterminate = true;
                            ProgressStatus = Helper.GetCultureString("UpdateProgressGHSize");
                        }
                        else
                        {
                            DownloadProgress = Math.Round(args, MidpointRounding.AwayFromZero);
                            DownloadProgressString = $"{DownloadProgress}%";
                        }
                    });
                };

                if (File.Exists(tempUpdatePath))
                    File.Delete(tempUpdatePath);

                // Create a file stream to store the downloaded data.
                // This really can be any type of writeable stream.
                await using FileStream file = new FileStream(tempUpdatePath, FileMode.Create, FileAccess.Write, FileShare.None);

                //TODO: Add cancellation token
                await Execute.OnUIThreadAsync(async () => { await client.DownloadAsync(tempModInfo.ModDownloadLink, file, progress, CancellationToken.None); });

                file.Close();
                await file.DisposeAsync();

                ProgressBarIsIndeterminate = true;
                ProgressStatus = Helper.GetCultureString("UpdateProgress1");
                DownloadProgressString = string.Empty;

                string tempExtractedModFolderPath = Path.Combine(tempPath, "UpdateDownload");

                if (Directory.Exists(tempExtractedModFolderPath))
                    Directory.Delete(tempExtractedModFolderPath, true);

                await Task.Run(() =>
                {
                    ZipFile.ExtractToDirectory(tempUpdatePath, tempExtractedModFolderPath);
                    return Task.CompletedTask;
                });

                string tempModDirPath = await Helper.FindFolderWithMpq(tempExtractedModFolderPath);
                string tempParentDir = Path.GetDirectoryName(tempModDirPath);
                string modInstallPath = Path.Combine(ShellViewModel.BaseModsFolder, tempModInfo.Name);

                string[] userSettings = null;

                if (File.Exists(ShellViewModel.SelectedUserSettingsFilePath))
                    userSettings = await File.ReadAllLinesAsync(ShellViewModel.SelectedUserSettingsFilePath);
                else if (File.Exists(ShellViewModel.SelectedUserSettingsFilePath.Replace($"{Settings.Default.SelectedMod}.mpq/", "")))
                    userSettings = await File.ReadAllLinesAsync(ShellViewModel.SelectedUserSettingsFilePath.Replace($"{Settings.Default.SelectedMod}.mpq/", ""));

                //Delete current Mod folder if it exists
                if (Directory.Exists(modInstallPath))
                    Directory.Delete(modInstallPath, true);

                //Clone mod into base mods folder.
                await Task.Run(async () => { await Helper.CloneDirectory(tempParentDir, modInstallPath); });

                if (userSettings != null)
                {
                    File.Create(ShellViewModel.SelectedUserSettingsFilePath).Close();
                    await File.WriteAllTextAsync(ShellViewModel.SelectedUserSettingsFilePath, string.Join("\n", userSettings));
                }

                string versionPath = Path.Combine(modInstallPath, "version.txt");
                if (!File.Exists(versionPath))
                    File.Create(versionPath).Close();

                tempModInfoPath = Path.Combine(tempModDirPath, "modinfo.json");

                ModInfo modInfo = await Helper.ParseModInfo(tempModInfoPath);

                if (modInfo != null)
                    await File.WriteAllTextAsync(versionPath, modInfo.ModVersion);
                else
                    MessageBox.Show("Could not parse ModInfo.json!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                File.Delete(tempUpdatePath);
                Directory.Delete(tempExtractedModFolderPath, true);

                ProgressBarIsIndeterminate = false;
                DownloadProgress = 100;
                ProgressStatus = Helper.GetCultureString("UpdateProgressDone");
                CheckingForUpdates = false;

                await InitializeMods();

                MessageBox.Show(Helper.GetCultureString("UpdateProgressDone"), "Update", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CheckingForUpdates = false;
                return;
            }
        }
        else
        {
            MessageBox.Show("Mod Author hasn't added support for auto-updates yet!");
        }
        CheckingForUpdates = false;

    }
    [UsedImplicitly]
    public async void OnDownloadMod()
    {
        dynamic options = new ExpandoObject();
        options.ResizeMode = ResizeMode.NoResize;
        options.WindowStartupLocation = WindowStartupLocation.CenterOwner;

        DownloadNewModViewModel vm = new DownloadNewModViewModel(ShellViewModel);

        if (await _windowManager.ShowDialogAsync(vm, null, options))
        {
            Settings.Default.SelectedMod = vm.SelectedMod.Key;
            Settings.Default.Save();

            await InitializeMods();
        }
    }
    [UsedImplicitly]
    public async void OnCASCSettings()
    {
        dynamic options = new ExpandoObject();
        options.ResizeMode = ResizeMode.NoResize;
        options.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        CASCExtractorViewModel vm = new CASCExtractorViewModel(ShellViewModel);
        await _windowManager.ShowDialogAsync(vm, null, options);
    }

    [UsedImplicitly]
    public async void OnCreateMod()
    {
        string createModDesc = Helper.GetCultureString("CreateModDesc").Replace("\\n", Environment.NewLine);

        if (MessageBox.Show(createModDesc, Helper.GetCultureString("Create"), MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            if (!Directory.Exists(Path.Combine(ShellViewModel.BaseModsFolder, "MyCustomMod/MyCustomMod.mpq/data/global")))
            {
                Directory.CreateDirectory(Path.Combine(ShellViewModel.BaseModsFolder, "MyCustomMod/MyCustomMod.mpq/data/global"));
                await File.WriteAllBytesAsync(Path.Combine(ShellViewModel.BaseModsFolder, "MyCustomMod/MyCustomMod.mpq/modinfo.json"), await Helper.GetResourceByteArray("modinfo_blank.json"));
                Settings.Default.SelectedMod = "MyCustomMod";
                Settings.Default.Save();
                CloneDirectory(Path.Combine(GetSavePath(), @"Diablo II Resurrected"), Path.Combine(GetSavePath(), @"Diablo II Resurrected\Mods\MyCustomMod"));
                await InitializeMods();
            }
            else
                MessageBox.Show("A custom mod has already been created!", "Error", MessageBoxButton.OK);
        }
    }

    [UsedImplicitly]
    public async void OnDirectTxtChecked()
    {
        string sourceDirectory = ShellViewModel.SelectedModDataFolder;

        if (!File.Exists(Path.Combine(sourceDirectory, @"local\macui\d2logo.pcx")))
        {
            ShellViewModel.UserSettings.DirectTxt = false;

            if (MessageBox.Show("You must first extract all ~40GB of game data to use this mode\nWould you like to extract them now?", "Missing Files!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                dynamic options = new ExpandoObject();
                options.ResizeMode = ResizeMode.NoResize;
                options.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                CASCExtractorViewModel vm = new CASCExtractorViewModel(ShellViewModel);

                if (await _windowManager.ShowDialogAsync(vm, null, options))
                {
                }
            }
            else
                ShellViewModel.UserSettings.DirectTxt = false;
        }

        GetD2RArgs();
    }

    [UsedImplicitly]
    public async void OnMapRegenChecked()
    {
        GetD2RArgs();
    }

    #endregion

    #region ---Change Handlers---

    [UsedImplicitly]
    public async void OnModSelectionChanged()
    {
        Settings.Default.SelectedMod = SelectedMod;
        Settings.Default.Save();

        await InitializeMods();
        GetD2RArgs();
    }
    [UsedImplicitly]
    public async void OnUIThemeSelectionChanged()
    {
        if (ShellViewModel.ModInfo == null)
            return;

        await ApplyUiTheme();
    }
    [UsedImplicitly]
    public async void OnMapLayoutSelectionChanged()
    {
        if ((eMapLayouts) ShellViewModel.UserSettings.MapLayout != eMapLayouts.Default)
            MessageBox.Show("WARNING: These options are meant for a fun experience or two, but will feel like cheating. Use at your own risk.\nIf you would like to proceed, please read these instructions:\n\nStep 1: Start the game with your selected layout\nStep 2: Once loaded into the game with your character fully, EXIT the game.\nStep 3: After exiting the game, you should see your layout dropdown on launcher changed back to Default. This is normal; Start the game again.\n\nIf you do not exit the game after changing your map layout...you will be stuck with a small drop pool of deterministic outcomes.\nThis does not need to be done every game; only if you change map layouts the normal ways; such as changing difficulty.");

        GetD2RArgs();
    }
    static void CloneDirectory(string sourceDirectory, string targetDirectory)
    {
        if (!Directory.Exists(sourceDirectory))
            return;

        if (!Directory.Exists(targetDirectory))
            Directory.CreateDirectory(targetDirectory);

        string[] files = Directory.GetFiles(sourceDirectory);
        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);
            string targetPath = Path.Combine(targetDirectory, fileName);
            File.Copy(file, targetPath, true);
        }
    }

    #endregion

    #region ---Map Seed Functions---
    public string GetSavePath()
    {
        string savePath = null;

        // Get all SIDs
        string[] userSIDs = Registry.Users.GetSubKeyNames()
            .Where(name => Regex.IsMatch(name, @"S-1-5-21-\d+-\d+-\d+-\d+$"))
            .ToArray();

        // GUID for Saved Games folder
        string valueName = "{4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4}";

        foreach (string SID in userSIDs)
        {
            // Find the location of the registry key under the current user's hive
            string keyPath = $"Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders";
            using (RegistryKey key = Registry.Users.OpenSubKey($"{SID}\\{keyPath}"))
            {
                if (key != null)
                {
                    object value = key.GetValue(valueName);
                    if (value != null)
                    {
                        savePath = value.ToString();
                        break;
                    }
                }
            }
        }

        // If not found under specific user SID, check under HKEY_CURRENT_USER
        if (savePath == null)
        {
            string currentUserKeyPath = $"Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders";
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(currentUserKeyPath))
            {
                if (key != null)
                {
                    object value = key.GetValue(valueName);
                    if (value != null)
                    {
                        savePath = value.ToString();
                    }
                }
            }
        }

        return savePath;
    }
    [UsedImplicitly]
    public async void OnCharSelect()
    {
        OpenFileDialog ofd = new OpenFileDialog();
        {
            ofd.InitialDirectory = ShellViewModel.SaveFilesFilePath;
            ofd.Filter = "D2R Character Files (*.d2s)|*.d2s";
        };

        ofd.ShowDialog();

        if (ofd.FileName != "")
        {
            string seedID = ParseD2SSeed(ofd.FileName).ToString();
            ShellViewModel.UserSettings.MapSeed = seedID;
            ShellViewModel.UserSettings.MapSeedName = Path.GetFileNameWithoutExtension(ofd.FileName) + "'s Seed: ";
            ShellViewModel.UserSettings.MapSeedLoc = ofd.FileName;
        }
        else
        {
            ShellViewModel.UserSettings.MapSeed = "";
            ShellViewModel.UserSettings.MapSeedName = "An Evil Force's Seed: ";
            ShellViewModel.UserSettings.MapSeedLoc = "";
        }
    }
    public async void OnCharMapSeed()
    {
        try
        {
            // Read the character file and parse the saved map seed ID
            byte[] bytes = await File.ReadAllBytesAsync(ShellViewModel.UserSettings.MapSeedLoc);
            byte[] newSeedBytes = BitConverter.GetBytes(uint.Parse(ShellViewModel.UserSettings.MapSeed));

            //Apply the saved map seed ID and update the D2S checksum
            Array.Copy(newSeedBytes, 0, bytes, 171, newSeedBytes.Length);
            int checksum = FixChecksum(bytes);
            byte[] checksumBytes = BitConverter.GetBytes(checksum);
            Array.Copy(checksumBytes, 0, bytes, 12, checksumBytes.Length);

            // Write the modified content back to the file
            await File.WriteAllBytesAsync(ShellViewModel.UserSettings.MapSeedLoc, bytes);
            MessageBox.Show($"{ShellViewModel.UserSettings.MapSeedName.Replace(":","")}Updated!");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }
    }
    public static int ParseD2SSeed(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"The file at {filePath} was not found.");

        byte[] fileData = File.ReadAllBytes(filePath);

        // Read 4 bytes starting at byte 171 for seed ID
        int startIndex = 171;
        byte[] data = new byte[4];
        Array.Copy(fileData, startIndex, data, 0, 4);

        // Convert the 4 bytes to an integer and return the result
        int result = BitConverter.ToInt32(data, 0);
        return result;
    }
    private int FixChecksum(byte[] bytes)
    {
        //Update save file checksum data to match edited content
        new byte[4].CopyTo(bytes, 0xc);
        int checksum = 0;

        for (int i = 0; i < bytes.Length; i++)
        {
            checksum = bytes[i] + (checksum * 2) + (checksum < 0 ? 1 : 0);
        }

        return checksum;
    }

    #endregion
}