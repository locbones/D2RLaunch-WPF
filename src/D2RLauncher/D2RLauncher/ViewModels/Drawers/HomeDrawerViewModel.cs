using Caliburn.Micro;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using D2RLauncher.Models.Enums;
using log4net;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace D2RLauncher.ViewModels.Drawers;

public class HomeDrawerViewModel : INotifyPropertyChanged
{
    #region members

    private string _launcherDescription = "This application is used to download and configure mods for D2R.";
    private string _launcherTitle = "D2RLauncher";
    private string _modDescription = "Please create a blank mod or download a new mod using the options below.";
    private string _modTitle = "No Mods Detected!";

    private ILog _logger = LogManager.GetLogger(typeof(HomeDrawerViewModel));
    private IWindowManager _windowManager;

    #endregion

    public HomeDrawerViewModel()
    {
        if (Execute.InDesignMode)
        {

        }
    }

    public HomeDrawerViewModel(ShellViewModel shellViewModel, IWindowManager windowManager)
    {
        ShellViewModel = shellViewModel;
        _windowManager = windowManager;
    }

    #region properties

    public ShellViewModel ShellViewModel { get; }

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

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}