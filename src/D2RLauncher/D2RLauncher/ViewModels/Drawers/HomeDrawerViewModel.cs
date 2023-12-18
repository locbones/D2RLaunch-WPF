using Caliburn.Micro;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using D2RLauncher.Models.Enums;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;
using D2RLauncher.Properties;
using Syncfusion.Licensing;
using System.ComponentModel.DataAnnotations;
using System;
using System.Dynamic;
using D2RLauncher.Culture;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using JetBrains.Annotations;
using System.Threading;
using System.Windows;
using D2RLauncher.Extensions;
using D2RLauncher.Models;
using D2RLauncher.ViewModels.Dialogs;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Text;
using System.Windows.Threading;
using Microsoft.Win32;
using System.Diagnostics;
using System.Timers;

namespace D2RLauncher.ViewModels.Drawers;

public class HomeDrawerViewModel : INotifyPropertyChanged
{
    #region members
    private const string TAB_BYTE_CODE = "55AA55AA0000000061000000000000004400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000004A4D0000";
    private ILog _logger = LogManager.GetLogger(typeof(HomeDrawerViewModel));
    private IWindowManager _windowManager;
    private string _launcherDescription = "This application is used to download and configure mods for D2R.";
    private string _launcherTitle = "D2RLauncher";
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
    private ObservableCollection<KeyValuePair<string, eUiThemes>> _uiThemes = new ObservableCollection<KeyValuePair<string, eUiThemes>>();
    private DispatcherTimer _monsterStatsDispatcherTimer;
    private bool _uiThemeEnabled;
    

    #endregion

    public HomeDrawerViewModel()
    {
        if (Execute.InDesignMode)
        {
            DownloadProgressString = "70%";
            ProgressStatus = "Test Progress Status...";
        }
    }

    public HomeDrawerViewModel(ShellViewModel shellViewModel, IWindowManager windowManager)
    {
        ShellViewModel = shellViewModel;
        _windowManager = windowManager;


        _monsterStatsDispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        _monsterStatsDispatcherTimer.Tick += (sender, args) => MonsterStatsDispatcherTimerOnTick(ShellViewModel.UserSettings);
        _monsterStatsDispatcherTimer.Interval = TimeSpan.FromSeconds(15);
    }

    #region properties

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

    #endregion

    private void MonsterStatsDispatcherTimerOnTick(UserSettings userSettings)
    {
        RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Blizzard Entertainment\Battle.net\Launch Options\BNA");
        object data = key.GetValue("CONNECTION_STRING_CN");
        if (data != null && data.ToString() == "127.0.0.1")
        {
            if (userSettings.MonsterStatsDisplay != 0)
            {
                Process testrun;
                testrun = Process.GetProcessesByName("D2R")[0];
                if (testrun != null)
                {
                    try
                    {
                        Injector.MainInject();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
                    }
                    _monsterStatsDispatcherTimer.Stop();
                }
            }
        }
    }

    public async Task Initialize()
    {
        foreach (eMapLayouts mapLayout in Enum.GetValues<eMapLayouts>())
        {
            MapLayouts.Add(new KeyValuePair<string, eMapLayouts>(mapLayout.GetAttributeOfType<DisplayAttribute>().Name, mapLayout));
        }

        foreach (eUiThemes uiTheme in Enum.GetValues<eUiThemes>())
        {
            UiThemes.Add(new KeyValuePair<string, eUiThemes>(uiTheme.GetAttributeOfType<DisplayAttribute>().Name, uiTheme));
        }

        await InitializeLanguage();
        await InitializeMods();
    }

    public async Task InitializeLanguage()
    {
        eLanguage appLanguage = ((eLanguage)Settings.Default.AppLanguage);
        SelectedAppLanguage = new KeyValuePair<string, eLanguage>(appLanguage.GetAttributeOfType<DisplayAttribute>().Name, appLanguage);

        foreach (eLanguage language in Enum.GetValues<eLanguage>())
        {
            Languages.Add(new KeyValuePair<string, eLanguage>(language.GetAttributeOfType<DisplayAttribute>().Name, language));
        }

        await Translate();
    }

    public async Task InitializeMods()
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
                {
                    return;
                }

                MapsComboBoxEnabled = ShellViewModel.ModInfo.MapLayouts;
                UiComboBoxEnabled = ShellViewModel.ModInfo.UIThemes && (ShellViewModel.ModInfo.Name == "Vanilla++" || ShellViewModel.ModInfo.Name == "ReMoDDeD");
                ShellViewModel.CustomizationsEnabled = ShellViewModel.ModInfo.Customizations;

                string logoPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "D2Rlaunch/Logo.png");
                if (File.Exists(logoPath))
                {
                    string tempPath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
                    File.Copy(logoPath, tempPath, true);
                    ShellViewModel.ModLogo = tempPath;
                }
                else
                {
                    ShellViewModel.ModLogo = "pack://application:,,,/Resources/Images/D2RL_Logo.png";
                }

                await LoadUserSettings();

                await ShellViewModel.StartAutoBackup();

                if (ShellViewModel.ModInfo.Name == "ReMoDDeD")
                {
                    UiThemeEnabled = false;
                    ShellViewModel.WikiEnabled = false;
                    ShellViewModel.UserSettings.UiTheme = 1;

                    ShellViewModel.SuperTelekinesisEnabled = false;
                    ShellViewModel.SkillBuffIconsEnabled = false;
                    ShellViewModel.SkillIconPackEnabled = false;
                    ShellViewModel.ShowItemLevelsEnabled = true;
                    ShellViewModel.UserSettings.ItemIlvls = 1;
                }
                else
                {
                    UiThemeEnabled = true;
                    ShellViewModel.WikiEnabled = true;

                    ShellViewModel.SkillBuffIconsEnabled = true;
                    ShellViewModel.SkillIconPackEnabled = true;
                }

                await ApplyUiTheme();
            }
        }
    }

    private async Task LoadUserSettings()
    {
        //Protected
        if (Directory.Exists(ShellViewModel.SelectedModDataFolder))
        {
            if (!File.Exists(ShellViewModel.SelectedUserSettingsFilePath))
            {
                if (!File.Exists(ShellViewModel.OldSelectedUserSettingsFilePath))
                {
                    ShellViewModel.UserSettings = await Helper.GetDefaultUserSettings();
                }
                else
                {
                    string[] oldUserSettings = await File.ReadAllLinesAsync(ShellViewModel.OldSelectedUserSettingsFilePath);
                    ShellViewModel.UserSettings = await Helper.ConvertUserSettings(oldUserSettings);
                }
            }
            else
            {
                ShellViewModel.UserSettings = JsonConvert.DeserializeObject<UserSettings>(await File.ReadAllTextAsync(ShellViewModel.SelectedUserSettingsFilePath));
                
            }
        }
        else //Unprotected
        {
            if (!File.Exists(ShellViewModel.SelectedUserSettingsFilePath.Replace($"{Settings.Default.SelectedMod}.mpq/", "")))
            {
                if (!File.Exists(ShellViewModel.OldSelectedUserSettingsFilePath.Replace($"{Settings.Default.SelectedMod}.mpq/", "")))
                {
                    ShellViewModel.UserSettings = await Helper.GetDefaultUserSettings();
                }
                else
                {
                    string[] oldUserSettings = await File.ReadAllLinesAsync(ShellViewModel.OldSelectedUserSettingsFilePath.Replace($"{Settings.Default.SelectedMod}.mpq/", ""));
                    ShellViewModel.UserSettings = await Helper.ConvertUserSettings(oldUserSettings);
                }
            }
            else
            {
                ShellViewModel.UserSettings = JsonConvert.DeserializeObject<UserSettings>(await File.ReadAllTextAsync(ShellViewModel.SelectedUserSettingsFilePath.Replace($"{Settings.Default.SelectedMod}.mpq/", "")));
            }
        }

        //TODO: Should the autoback up timer be configured here?
        //TODO:_profilehd.json be setup here?
    }

    private async Task Translate()
    {
        if (ShellViewModel.ModInfo != null)
        {
            if (SelectedAppLanguage.Value == eLanguage.English)
            {
                ModTitle = ShellViewModel.ModInfo.ModTitle.Trim().Replace("\"", "");
                ModDescription = ShellViewModel.ModInfo.ModDescription.Trim().Replace("\"", "");

                LauncherTitle = ShellViewModel.ModInfo.NewsTitle.Trim().Replace("\"", "");
                LauncherDescription = ShellViewModel.ModInfo.NewsDescription.Trim().Replace("\"", "");

                return;
            }
            string pattern = @"(?<![0-9])\.(?![0-9])"; // Matches a period not surrounded by digits

            string modTitle = Regex.Replace(ShellViewModel.ModInfo.ModTitle.Trim().Replace("\"", ""), pattern, "||");
            string modDescription = Regex.Replace(ShellViewModel.ModInfo.ModDescription.Trim().Replace("\"", ""), pattern, "||");
            string launcherTitle = Regex.Replace(ShellViewModel.ModInfo.NewsTitle.Trim().Replace("\"", ""), pattern, "||");
            string launcherDescription = Regex.Replace(ShellViewModel.ModInfo.NewsDescription.Trim().Replace("\"", ""), pattern, "||");

            ModTitle = await TranslateGoogleAsync(modTitle);
            ModDescription = await TranslateGoogleAsync(modDescription.Replace("|| ", ".").Replace(@"\u0026", ". "));
            LauncherTitle = await TranslateGoogleAsync(launcherTitle);
            LauncherDescription = await TranslateGoogleAsync(launcherDescription.Replace("|| ", ".").Replace(@"\u0026", ". "));
        }
        else
        {
            ModTitle = await TranslateGoogleAsync(ModTitle);
            ModDescription = await TranslateGoogleAsync(ModDescription);
            LauncherTitle = await TranslateGoogleAsync(LauncherTitle);
            LauncherDescription = await TranslateGoogleAsync(LauncherDescription);
        }

    }

    private async Task<string> TranslateGoogleAsync(string text)
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

    private async Task LoadModOptions()
    {

    }

    private string GetD2RArgs()
    {
        string args = string.Empty;
        string regenArg = ShellViewModel.UserSettings.ResetMaps ? " -resetofflinemaps" : string.Empty;
        string respecArg = ShellViewModel.UserSettings.InfiniteRespec ? " -enablerespec" : string.Empty;
        string mapLayoutArg = GetMapLayoutArg();

        if (ShellViewModel.UserSettings.DirectTxt)
        {
            if (ShellViewModel.ModInfo.Name == "ReMoDDeD")
            {
                args = $"-direct{regenArg}{respecArg}{mapLayoutArg}";
            }
            else
            {
                args = $"-direct -txt{regenArg}{respecArg}{mapLayoutArg}";
            }
        }
        else
        {
            string excelDir = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/excel");

            if (Directory.Exists(excelDir))
            {
                int binFileCount = Directory.GetFiles(excelDir, "*.bin").Length;
                int txtFileCount = Directory.GetFiles(excelDir, "*.txt").Length;

                if (binFileCount >= 83 && txtFileCount >= 10)
                {
                    args = $"-mod {ShellViewModel.ModInfo.Name} -txt";
                }

                if (binFileCount >= 83 && txtFileCount < 10)
                {
                    args = $"-mod {ShellViewModel.ModInfo.Name}";
                }

                if (binFileCount < 83 && txtFileCount >= 1)
                {
                    args = $"-mod {ShellViewModel.ModInfo.Name} -txt";
                }
            }
            else
            {
                args = $"-mod {ShellViewModel.ModInfo.Name} -txt";
            }

            args = $"{args}{regenArg}{respecArg}{mapLayoutArg}";
        }

        return args;
    }

    private string GetMapLayoutArg()
    {
        string arg = string.Empty;

        switch ((eMapLayouts) ShellViewModel.UserSettings.MapLayout)
        {
            case eMapLayouts.Default:
            {
                return "";
            }
            case eMapLayouts.Tower:
            {
                return " -seed 1112";
            }
            case eMapLayouts.Catacombs:
            {
                return " -seed 348294647";
            }
            case eMapLayouts.AncientTunnels:
            {
                return " -seed 1111";
            }
            case eMapLayouts.LowerKurast:
            {
                return " -seed 1460994795";
            }
            case eMapLayouts.DuranceOfHate:
            {
                return " -seed 1113";
            }
            case eMapLayouts.Hellforge:
            {
                return " -seed 100";
            }
            case eMapLayouts.WorldstoneKeep:
            {
                return " -seed 1104";
            }
            case eMapLayouts.Cheater:
            {
                return " -seed 1056279548";
            }
            default:
            {
                return "";
            }
        }
    }

    private async Task ApplyHdrFix()
    {
        string profileHdJsonPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/ui/layouts/_profilehd.json");

        if (File.Exists(profileHdJsonPath))
        {
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
        else
        {
            MessageBox.Show("Unable to locate _profilehd.json file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            ShellViewModel.UserSettings.HdrFix = false;
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
        {
            await File.WriteAllBytesAsync(Path.Combine(layoutPath, "bankexpansionlayouthd.json"), await Helper.GetResourceByteArray("Options.PersonalizedTabs.stash_rmd"));
        }
        if (ShellViewModel.ModInfo.Name == "Vanilla++")
        {
            await File.WriteAllBytesAsync(Path.Combine(layoutPath, "bankexpansionlayouthd.json"), await Helper.GetResourceByteArray("Options.PersonalizedTabs.stash_vnp"));
        }

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
                    case eUiThemes.ReMoDDeD:
                        {
                            if (Directory.Exists(layoutPath))
                            {
                                Directory.Delete(layoutPath, true);
                                await Helper.CloneDirectory(layoutRemoddedPath, layoutPath);
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

    [UsedImplicitly]
    public async void OnMapsHelp()
    {
        MessageBox.Show("These options let you force specific map layouts so you can roll that 'perfect' map anytime you want. Details explained below:\n\nTower: The tower entrance is on the same screen as your waypoint.\n\nCatacombs: Levels 3 and 4 are less than 3 screens away\n\nAncient Tunnels: Entrance is 1 screen away from your waypoint\n\nLower Kurast: Very favorable super chest pattern near your waypoint\n\nDurance of Hate: Level 3 entrance is one teleport away from waypoint.\n\nHellforge: Forge is at closest spawn to your waypoint\n\nWorldstone Keep: Level 3 and 4 are right next to each other\n\nI'm a Cheater: Almost all entrances are absurdly close with a perfect LK pattern by the waypoint. You're basically just cheating now.\n\n\nNOTE: Lower Kurast and I'm a Cheater options are only available on Vanilla++.");
    }

    [UsedImplicitly]
    public async void OnPlayMod()
    {
        if (ShellViewModel.ModInfo == null)
        {
            return;
        }

        await ApplyHdrFix();

        //Unlock / create SharedStash
        if (ShellViewModel.ModInfo != null)
        {
            string hexString = String.Concat(Enumerable.Repeat(TAB_BYTE_CODE, 4));

            string d2rSavePath = string.Empty;

            if (ShellViewModel.ModInfo.SavePath == "\"../\"")
            {
                d2rSavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"Saved Games\Diablo II Resurrected");
            }
            else
            {
                d2rSavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @$"Saved Games\Diablo II Resurrected\Mods\{ShellViewModel.ModInfo.Name}");
            }

            if (!Directory.Exists(d2rSavePath))
            {
                Directory.CreateDirectory(d2rSavePath);
            }

            string sharedStashSoftCorePath = Path.Combine(d2rSavePath, "SharedStashSoftCoreV2.d2i");
            string sharedStashHardCorePath = Path.Combine(d2rSavePath, "SharedStashHardCoreV2.d2i");

            //If stash doesn't exist yet; create a new one with all 7 tabs unlocked
            if (!File.Exists(sharedStashSoftCorePath))
            {
                File.Create(sharedStashSoftCorePath).Close();
                await File.WriteAllBytesAsync(sharedStashSoftCorePath, await Helper.GetResourceByteArray("SharedStashSoftCoreV2.d2i"));
            }
            else
            {
                //Check if stash is unlocked already and unlock if not
                byte[] data = await File.ReadAllBytesAsync(sharedStashSoftCorePath);
                string bitString = BitConverter.ToString(data).Replace("-", string.Empty);
                if (Regex.Matches(bitString, "4A4D0000").Count == 3)
                {
                    await File.WriteAllBytesAsync(sharedStashSoftCorePath, Helper.StringToByteArray(bitString + hexString));
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
                if (Regex.Matches(bitString, "4A4D0000").Count == 3)
                {
                    await File.WriteAllBytesAsync(sharedStashHardCorePath, Helper.StringToByteArray(bitString + hexString));
                }
            }
        }

        //Load MonsterStats Setting
        switch (ShellViewModel.UserSettings.MonsterStatsDisplay)
        {
            case 1:
            case 2:
                {
                    string stasherEntityFrameworkPath = Path.Combine(ShellViewModel.StasherPath, "EntityFramework.pdb");
                    if (File.Exists(stasherEntityFrameworkPath))
                        File.Delete(stasherEntityFrameworkPath);
                    File.Create(stasherEntityFrameworkPath).Close();
                    await File.WriteAllBytesAsync(stasherEntityFrameworkPath, await Helper.GetResourceByteArray("Options.MonsterStats.MonsterStats.dll"));

                    _monsterStatsDispatcherTimer.Start();

                    break;
                }
        }

        ShellViewModel.DisableBNetConnection();

        //Start the mod
        string d2rArgs = GetD2RArgs();
        ProcessStartInfo startInfo = new ProcessStartInfo(Path.Combine(ShellViewModel.GamePath, "D2R.exe"), d2rArgs);
        startInfo.WorkingDirectory = @".\";
        try
        {
            //MessageBox.Show(launchArgsF);
            Process.Start(startInfo);
            ShellViewModel.UserSettings.MapLayout = 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error starting application: {ex}");
        }
    }

    [UsedImplicitly]
    public async void OnModSelectionChanged()
    {
        Settings.Default.SelectedMod = SelectedMod;
        Settings.Default.Save();

        await InitializeMods();
    }

    [UsedImplicitly]
    public async void OnUIThemeSelectionChanged()
    {
        if (ShellViewModel.ModInfo == null)
        {
            return;
        }

        await ApplyUiTheme();

    }

    [UsedImplicitly]
    public async void OnMapLayoutSelectionChanged()
    {
        if ((eMapLayouts) ShellViewModel.UserSettings.MapLayout != eMapLayouts.Default)
        {
            MessageBox.Show("WARNING: These options are meant for a fun experience or two, but will feel like cheating. Use at your own risk.\nIf you would like to proceed, please read these instructions:\n\nStep 1: Start the game with your selected layout\nStep 2: Once loaded into the game with your character fully, EXIT the game.\nStep 3: After exiting the game, you should see your layout dropdown on launcher changed back to Default. This is normal; Start the game again.\n\nIf you do not exit the game after changing your map layout...you will be stuck with a small drop pool of deterministic outcomes.\nThis does not need to be done every game; only if you change map layouts the normal ways; such as changing difficulty.");
        }
    }

    [UsedImplicitly]
    public async void OnTextLanguageSelectionChanged()
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
    public async void OnAudioLanguageSelectionChanged()
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
    public async void OnAppLanguageSelectionChanged()
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

            MessageBox.Show($"{Helper.GetCultureString("Version1")}: {version}\n {Helper.GetCultureString("Version2")}: {tempModInfo.ModVersion}",
                            Resources.ResourceManager.GetString("VersionRdy"), MessageBoxButton.OK);

            //Backup
            if (MessageBox.Show($"{Helper.GetCultureString("ModUpdateRdy").Replace("\\n", Environment.NewLine)}", "Backup Option", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ProgressStatus = Helper.GetCultureString("UpdateBackup");

                if (Directory.Exists(ShellViewModel.BaseSelectedModFolder))
                {
                    try
                    {
                        string backupPath = Path.Combine(ShellViewModel.BaseModsFolder, $"{Settings.Default.SelectedMod}(Backup-{ShellViewModel.ModInfo.ModVersion.Replace(".","-")})");
                        if (Directory.Exists(backupPath))
                        {
                            Directory.Delete(backupPath, true);
                        }

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
                {
                    File.Delete(tempUpdatePath);
                }

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
                {
                    Directory.Delete(tempExtractedModFolderPath, true);
                }

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
                {
                    userSettings = await File.ReadAllLinesAsync(ShellViewModel.SelectedUserSettingsFilePath);
                }
                else if (File.Exists(ShellViewModel.SelectedUserSettingsFilePath.Replace($"{Settings.Default.SelectedMod}.mpq/", "")))
                {
                    userSettings = await File.ReadAllLinesAsync(ShellViewModel.SelectedUserSettingsFilePath.Replace($"{Settings.Default.SelectedMod}.mpq/", ""));
                }

                //Delete current Mod folder if it exists
                if (Directory.Exists(modInstallPath))
                {
                    Directory.Delete(modInstallPath, true);
                }

                //Clone mod into base mods folder.
                await Task.Run(async () => { await Helper.CloneDirectory(tempParentDir, modInstallPath); });

                if (userSettings != null)
                {
                    File.Create(ShellViewModel.SelectedUserSettingsFilePath).Close();
                    await File.WriteAllTextAsync(ShellViewModel.SelectedUserSettingsFilePath, string.Join("\n", userSettings));
                }

                string versionPath = Path.Combine(modInstallPath, "version.txt");
                if (!File.Exists(versionPath))
                {
                    File.Create(versionPath).Close();
                }

                tempModInfoPath = Path.Combine(tempModDirPath, "modinfo.json");

                ModInfo modInfo = await Helper.ParseModInfo(tempModInfoPath);

                if (modInfo != null)
                {
                    await File.WriteAllTextAsync(versionPath, modInfo.ModVersion);
                }
                else
                {
                    MessageBox.Show("Could not parse ModInfo.json!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

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
        options.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        DownloadNewModViewModel vm = new DownloadNewModViewModel(ShellViewModel);

        if (await _windowManager.ShowDialogAsync(vm, null, options))
        {
            if (InstalledMods.Contains(vm.SelectedMod.Key))
            {
                Settings.Default.SelectedMod = vm.SelectedMod.Key;
            }
            await InitializeMods();
        }
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
                await InitializeMods();
            }
            else
            {
                MessageBox.Show("A custom mod has already been created!", "Error", MessageBoxButton.OK);
            }
        }
    }

    [UsedImplicitly]
    public async void OnDirectTxtChecked()
    {
        if (!Directory.Exists(Path.Combine(ShellViewModel.GamePath, "/data/global")) || !Directory.Exists(Path.Combine(ShellViewModel.GamePath, "/data/hd")) || !Directory.Exists(Path.Combine(ShellViewModel.GamePath, "/data/local")))
        {
            if (MessageBox.Show("You don't have the required files for -direct -txt mode\nWould you like to extract them now?", "Missing Files!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                dynamic options = new ExpandoObject();
                options.ResizeMode = ResizeMode.NoResize;
                options.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                CASCExtractorViewModel vm = new CASCExtractorViewModel(ShellViewModel);

                if (await _windowManager.ShowDialogAsync(vm, null, options))
                {
                   
                }

                if (!Directory.Exists(Path.Combine(ShellViewModel.GamePath, "/data/global")) || !Directory.Exists(Path.Combine(ShellViewModel.GamePath, "/data/hd")) ||
                    !Directory.Exists(Path.Combine(ShellViewModel.GamePath, "/data/local")))
                {
                    ShellViewModel.UserSettings.DirectTxt = false;
                }
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
}