using System.ComponentModel.Composition;
using System.Windows.Controls;
using Caliburn.Micro;
using ILog = log4net.ILog;
using log4net;
using LogManager = log4net.LogManager;
using D2RLauncher.Views;
using Syncfusion.UI.Xaml.NavigationDrawer;
using System.Threading;
using System.Windows;
using System;
using System.Threading.Tasks;
using D2RLauncher.ViewModels.Drawers;
using D2RLauncher.Views.Drawers;
using JetBrains.Annotations;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using D2RLauncher.Properties;
using D2RLauncher.Models.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Syncfusion.Licensing;
using D2RLauncher.Culture;
using D2RLauncher.Models;
using Newtonsoft.Json;
using System.Windows.Threading;

namespace D2RLauncher.ViewModels;

public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
{
    #region members

    private ILog _logger = LogManager.GetLogger(typeof(ShellViewModel));
    private UserControl _userControl;
    private IWindowManager _windowManager;
    private string _title = "D2R Launcher";
    private string _gamePath;
    private bool _diabloInstallDetected;
    private bool _customizationsEnabled;
    private bool _wikiEnabled;
    private ModInfo _modInfo;
    private UserSettings _userSettings;
    private string _modLogo = "pack://application:,,,/Resources/Images/D2RL_Logo.png";
    private DispatcherTimer _autoBackupDispatcherTimer;
    private bool _skillIconPackEnabled;
    private bool _skillBuffIconsEnabled;
    private bool _showItemLevelsEnabled;

    #endregion

    public ShellViewModel()
    {
        if (Execute.InDesignMode)
        {
            ModLogo = "pack://application:,,,/Resources/Images/D2RL_Logo.png";
            Title = "D2R Launcher";
            DiabloInstallDetected = true;
            HomeDrawerViewModel vm = new HomeDrawerViewModel();
            UserControl = new HomeDrawerView() {DataContext = vm};
        }
    }

    public ShellViewModel(IWindowManager windowManager)
    {
        _windowManager = windowManager;
        _logger.Error("Shell view model being created..");
    }

    #region properties

    public string ModLogo
    {
        get => _modLogo;
        set
        {
            if (value == _modLogo) return;
            _modLogo = value;
            NotifyOfPropertyChange();
        }
    }

    public ModInfo ModInfo
    {
        get => _modInfo;
        set
        {
            if (Equals(value, _modInfo)) return;
            _modInfo = value;
            NotifyOfPropertyChange();
        }
    }

    public bool ShowItemLevelsEnabled
    {
        get => _showItemLevelsEnabled;
        set
        {
            if (value == _showItemLevelsEnabled) return;
            _showItemLevelsEnabled = value;
            NotifyOfPropertyChange();
        }
    }

    public bool SkillIconPackEnabled
    {
        get => _skillIconPackEnabled;
        set
        {
            if (value == _skillIconPackEnabled) return;
            _skillIconPackEnabled = value;
            NotifyOfPropertyChange();
        }
    }

    public bool SkillBuffIconsEnabled
    {
        get => _skillBuffIconsEnabled;
        set
        {
            if (value == _skillBuffIconsEnabled) return;
            _skillBuffIconsEnabled = value;
            NotifyOfPropertyChange();
        }
    }

    public bool CustomizationsEnabled
    {
        get => _customizationsEnabled;
        set
        {
            if (value == _customizationsEnabled) return;
            _customizationsEnabled = value;
            NotifyOfPropertyChange();
        }
    }

    public bool WikiEnabled
    {
        get => _wikiEnabled;
        set
        {
            if (value == _wikiEnabled) return;
            _wikiEnabled = value;
            NotifyOfPropertyChange();
        }
    }

    public bool DiabloInstallDetected
    {
        get => _diabloInstallDetected;
        set
        {
            if (value == _diabloInstallDetected) return;
            _diabloInstallDetected = value;
            NotifyOfPropertyChange();
        }
    }

    public string GamePath
    {
        get => _gamePath;
        set
        {
            if (value == _gamePath) return;
            _gamePath = value;
            NotifyOfPropertyChange();
        }
    }

    public string BaseModsFolder => Path.Combine(GamePath, "Mods");

    public string BaseSelectedModFolder => Path.Combine(BaseModsFolder, Settings.Default.SelectedMod);

    public string SelectedModVersionFilePath => Path.Combine(BaseSelectedModFolder, "version.txt");

    public string SelectedModDataFolder => Path.Combine(BaseSelectedModFolder, $"{Settings.Default.SelectedMod}.mpq", "data");

    public string SelectedModInfoFilePath => Path.Combine(BaseSelectedModFolder, $"{Settings.Default.SelectedMod}.mpq", "modinfo.json");

    public string OldSelectedUserSettingsFilePath => Path.Combine(BaseSelectedModFolder, $"{Settings.Default.SelectedMod}.mpq", "MyUserSettings.txt");

    public string SelectedUserSettingsFilePath => Path.Combine(BaseSelectedModFolder, $"{Settings.Default.SelectedMod}.mpq", "MyUserSettings.json");

    public string SaveFilesFilePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @$"Saved Games\Diablo II Resurrected\Mods\{Settings.Default.SelectedMod}");

    public string BackupFolder => Path.Combine(SaveFilesFilePath, "Backups");

    public string StasherPath => $@"..\Stasher";

    public UserSettings UserSettings
    {
        get => _userSettings;
        set
        {
            if (Equals(value, _userSettings)) return;
            _userSettings = value;
            _userSettings.PropertyChanged += (sender, args) =>
                                             {
                                                 Task.Run(SaveUserSettings);
                                             };

            NotifyOfPropertyChange();
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            if (value == _title) return;
            _title = value;
            NotifyOfPropertyChange();
        }
    }

    public UserControl UserControl
    {
        get => _userControl;
        set
        {
            _userControl = value;
            NotifyOfPropertyChange();
        }
    }

    #endregion

    public async Task StartAutoBackup()
    {
        if (UserSettings == null)
        {
            MessageBox.Show("Auto Backup was not started. Could not find User Settings!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if ((eBackup) UserSettings.AutoBackups == eBackup.Disabled)
        {
            _autoBackupDispatcherTimer = null;
            return;
        }

        _autoBackupDispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        _autoBackupDispatcherTimer.Tick += async (sender, args) =>
                                           {
                                               _logger.Info("Auto backup timer ticked.");
                                               await BackupRecentCharacter();
                                           };

        switch ((eBackup)UserSettings.AutoBackups)
        {
            case eBackup.FiveMinutes:
            {
                _autoBackupDispatcherTimer.Interval = TimeSpan.FromMinutes(5);
                break;
            }
            case eBackup.FifteenMinutes:
            {
                _autoBackupDispatcherTimer.Interval = TimeSpan.FromMinutes(15);
                break;
            }
            case eBackup.ThirtyMinutes:
            {
                _autoBackupDispatcherTimer.Interval = TimeSpan.FromMinutes(30);
                break;
            }
            case eBackup.OneHour:
            {
                _autoBackupDispatcherTimer.Interval = TimeSpan.FromMinutes(60);
                break;
            }
        }

        _autoBackupDispatcherTimer.Start();
    }

    public async Task<(string characterName, bool passed)> BackupRecentCharacter()
    {
        string mostRecentCharacterName;
        try
        {
            if (!Directory.Exists(BackupFolder))
            {
                Directory.CreateDirectory(BackupFolder);
            }

            //Backup Character
            FileInfo mostRecentCharacterFile = new DirectoryInfo(SaveFilesFilePath).GetFiles("*.d2s").MaxBy(o => o.LastWriteTime);
            mostRecentCharacterName = Path.GetFileNameWithoutExtension(mostRecentCharacterFile.ToString());

            string mostRecentCharacterBackupFolder = Path.Combine(BackupFolder, mostRecentCharacterName);
            if (!Directory.Exists(mostRecentCharacterBackupFolder))
            {
                Directory.CreateDirectory(mostRecentCharacterBackupFolder);
            }

            File.Copy(mostRecentCharacterFile.FullName, Path.Combine(mostRecentCharacterBackupFolder, mostRecentCharacterFile.Name), true);

            //Backup Stash
            string mostRecentStashFile = Path.Combine(SaveFilesFilePath, "SharedStashSoftCoreV2.d2i");
            string stashBackupFolder = Path.Combine(BackupFolder, "Stash");
            if (!Directory.Exists(stashBackupFolder))
            {
                Directory.CreateDirectory(stashBackupFolder);
            }
            File.Copy(mostRecentStashFile, Path.Combine(stashBackupFolder, "SharedStashSoftCoreV2.d2i"), true);
        }
        catch (Exception ex)
        {
            _logger.Error(ex);
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return (null, false);
        }

        return (mostRecentCharacterName, true);
    }

    [UsedImplicitly]
    public async void OnLoaded(object args)
    {
        eLanguage appLanguage = ((eLanguage)Settings.Default.AppLanguage);

        CultureInfo culture = new CultureInfo(appLanguage.GetAttributeOfType<DisplayAttribute>().Name.Split(' ')[1].Trim(new[] { '(', ')' })/*.Insert(2, "-")*/);
        CultureResources.ChangeCulture(culture);

        GamePath = await GetDiabloInstallPath();

        if (string.IsNullOrEmpty(GamePath))
        {
            DiabloInstallDetected = false;
            MessageBox.Show("Diablo II Resurrected install could not be found!\nPlease be sure to have a legitimate copy of Diablo II Resurrected installed and restart the application!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        DiabloInstallDetected = true;

        if (!Directory.Exists(BaseModsFolder))
        {
            Directory.CreateDirectory(BaseModsFolder);
        }

        DisableBNetConnection();

        HomeDrawerViewModel vm = new HomeDrawerViewModel(this, _windowManager);
        await vm.Initialize();
        UserControl = new HomeDrawerView() { DataContext = vm };
    }

    private async Task<string> GetDiabloInstallPath()
    {
        RegistryKey gameKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Diablo II Resurrected");

        string installLocation = gameKey?.GetValue("InstallLocation")?.ToString();

        if (installLocation != null)
        {
            return installLocation;
        }

        using RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);
        using RegistryKey regKey = baseKey.OpenSubKey(@"System\GameConfigStore\Children");

        if (regKey == null)
        {
            return null;
        }

        string[] subKeyNames = regKey.GetSubKeyNames();
        List<string> results = new();

        foreach (string subKeyName in subKeyNames)
        {
            using RegistryKey subKey = regKey.OpenSubKey(subKeyName);

            if (subKey == null)
            {
                continue;
            }

            string exeFullPath = subKey.GetValue("MatchedExeFullPath")?.ToString();

            if (string.IsNullOrEmpty(exeFullPath))
            {
                continue;
            }

            if (exeFullPath.Contains("D2R.exe"))
            {
                results.Add(exeFullPath);
            }
        }

        switch (results.Count)
        {
            case 1:
                return results[0].Replace(@"\D2R.exe", "");
            case >= 2: MessageBox.Show("If you experience mod loading issues, please contact Bonesy in Discord", "Multiple Install Locations found!");
                break;
        }

        return null;
    }

    public void DisableBNetConnection()
    {
        RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey(@"Software\Blizzard Entertainment\Battle.net\Launch Options\BNA", true) ?? throw new Exception("Failed to find registry key");
        key.SetValue("CONNECTION_STRING_CN", "127.0.0.1");
        key.SetValue("CONNECTION_STRING_CXX", "127.0.0.1");
        key.SetValue("CONNECTION_STRING_EU", "127.0.0.1");
        key.SetValue("CONNECTION_STRING_KR", "127.0.0.1");
        key.SetValue("CONNECTION_STRING_US", "127.0.0.1");
        key.SetValue("CONNECTION_STRING_XX", "127.0.0.1");
    }

    public async Task SaveUserSettings()
    {
        //Protected
        if (Directory.Exists(SelectedModDataFolder))
        {
            await File.WriteAllTextAsync(SelectedUserSettingsFilePath, JsonConvert.SerializeObject(UserSettings));
           
        }
        //Unprotected
        else
        {
            await File.WriteAllTextAsync(SelectedUserSettingsFilePath, JsonConvert.SerializeObject(UserSettings).Replace($"{Settings.Default.SelectedMod}.mpq/", ""));
        }
    }

    [UsedImplicitly]
    public async void OnItemClicked(NavigationItemClickedEventArgs args)
    {
        
        switch (((string)args.Item.Tag).ToUpperInvariant())
        {
            case "HOME":
            {
                HomeDrawerViewModel vm = new HomeDrawerViewModel(this, _windowManager);
                await vm.Initialize();
                UserControl = new HomeDrawerView() { DataContext = vm };
                break;
            }
            case "QOL OPTIONS":
            {
                QoLOptionsDrawerViewModel vm = new QoLOptionsDrawerViewModel(this, _windowManager);
                await vm.Initialize();
                UserControl = new QoLOptionsDrawerView() { DataContext = vm };
                    break;
            }
        }
    }
}