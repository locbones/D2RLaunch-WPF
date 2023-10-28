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
    }

    #region properties

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

    public string SelectedModDataFolder => Path.Combine($"{BaseSelectedModFolder}.mpq", "data");

    public string SelectedModInfoFilePath => Path.Combine($"{BaseSelectedModFolder}.mpq", "modinfo.json");

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
        eLanguage appLanguage = ((eLanguage)Settings.Default.AppLanguage);

        CultureInfo culture = new CultureInfo(appLanguage.GetAttributeOfType<DisplayAttribute>().Name.Split(' ')[1].Trim(new[] { '(', ')' })/*.Insert(2, "-")*/);
        CultureResources.ChangeCulture(culture);

        GamePath = await GetDiabloInstallPath();

        if (string.IsNullOrEmpty(GamePath))
        {
            DiabloInstallDetected = false;
            MessageBox.Show("Diablo II Resurrected install could not be found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        DiabloInstallDetected = true;

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

    [UsedImplicitly]
    public async void OnItemClicked(NavigationItemClickedEventArgs args)
    {
        switch (((string)args.Item.Header).ToUpperInvariant())
        {
           
        }
    }
}