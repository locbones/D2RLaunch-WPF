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
using Syncfusion.Licensing;
using D2RLauncher.Culture;

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
    private ObservableCollection<KeyValuePair<string, eLanguage>> _languages = new ObservableCollection<KeyValuePair<string, eLanguage>>();
    private KeyValuePair<string, eLanguage> _selectedTextLanguage;
    private KeyValuePair<string, eLanguage> _selectedAppLanguage;
    private KeyValuePair<string, eLanguage> _selectedAudioLanguage;
    //private Resources _resources;

    #endregion

    public ShellViewModel()
    {
        if (Execute.InDesignMode)
        {
            Title = "D2R Launcher";
            HomeDrawerViewModel vm = new HomeDrawerViewModel();
            UserControl = new HomeDrawerView() {DataContext = vm};
        }
    }

    public ShellViewModel(IWindowManager windowManager)
    {
        _windowManager = windowManager;
        _logger.Error("Shell view model being created..");

        HomeDrawerViewModel vm = new HomeDrawerViewModel(this, windowManager);
        UserControl = new HomeDrawerView() { DataContext = vm };
    }

    #region properties

    public KeyValuePair<string, eLanguage> SelectedTextLanguage
    {
        get => _selectedTextLanguage;
        set
        {
            if (value.Equals(_selectedTextLanguage)) return;
            _selectedTextLanguage = value;

            NotifyOfPropertyChange();
        }
    }

    public KeyValuePair<string, eLanguage> SelectedAppLanguage
    {
        get => _selectedAppLanguage;
        set
        {
            if (value.Equals(_selectedAppLanguage)) return;
            _selectedAppLanguage = value;

            if (!string.IsNullOrEmpty(_selectedAppLanguage.Key))
            {
                CultureInfo culture = new CultureInfo(_selectedAppLanguage.Key.Split(' ')[1].Trim(new[] {'(', ')'})/*.Insert(2, "-")*/);
                CultureResources.ChangeCulture(culture);
            }
            NotifyOfPropertyChange();
        }
    }

    public KeyValuePair<string, eLanguage> SelectedAudioLanguage
    {
        get => _selectedAudioLanguage;
        set
        {
            if (value.Equals(_selectedAudioLanguage)) return;
            _selectedAudioLanguage = value;

            NotifyOfPropertyChange();
        }
    }

    public ObservableCollection<KeyValuePair<string, eLanguage>> Languages
    {
        get => _languages;
        set
        {
            if (Equals(value, _languages)) return;
            _languages = value;
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


    [UsedImplicitly]
    public async void OnLoaded(object args)
    {
        eLanguage defaultLang = (eLanguage) Settings.Default.AppLanguage;
        string defaultLangDisplayName = defaultLang.GetAttributeOfType<DisplayAttribute>().Name;
        SelectedAppLanguage = SelectedAudioLanguage = SelectedTextLanguage = new KeyValuePair<string, eLanguage>(defaultLangDisplayName, defaultLang);

        foreach (eLanguage language in Enum.GetValues<eLanguage>())
        {
            Languages.Add(new KeyValuePair<string, eLanguage>(language.GetAttributeOfType<DisplayAttribute>().Name, language));
        }

        GamePath = await GetDiabloInstallPath();

        if (string.IsNullOrEmpty(GamePath))
        {
            DiabloInstallDetected = false;
            MessageBox.Show("Diablo II Resurrected install could not be found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        DiabloInstallDetected = true;
    }

    private async Task TranslateNews()
    {

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

    [UsedImplicitly]
    public async void OnItemClicked(NavigationItemClickedEventArgs args)
    {
        switch (((string)args.Item.Header).ToUpperInvariant())
        {
           
        }
    }
}