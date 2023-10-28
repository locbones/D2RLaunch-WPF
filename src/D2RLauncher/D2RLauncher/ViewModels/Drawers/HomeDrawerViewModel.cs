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
using D2RLauncher.Properties;
using Syncfusion.Licensing;
using System.ComponentModel.DataAnnotations;
using System;
using System.Dynamic;
using D2RLauncher.Culture;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using JetBrains.Annotations;

namespace D2RLauncher.ViewModels.Drawers;

public class HomeDrawerViewModel : INotifyPropertyChanged
{
    #region members

   private ILog _logger = LogManager.GetLogger(typeof(HomeDrawerViewModel));
    private IWindowManager _windowManager;
    private string _launcherDescription = "This application is used to download and configure mods for D2R.";
    private string _launcherTitle = "D2RLauncher";
    private string _modDescription = "Please create a blank mod or download a new mod using the options below.";
    private string _modTitle = "No Mods Detected!";
    private ObservableCollection<KeyValuePair<string, eLanguage>> _languages = new ObservableCollection<KeyValuePair<string, eLanguage>>();
    private KeyValuePair<string, eLanguage> _selectedTextLanguage;
    private KeyValuePair<string, eLanguage> _selectedAppLanguage;
    private KeyValuePair<string, eLanguage> _selectedAudioLanguage;
    private ObservableCollection<string> _installedMods;
    private string _selectedMod;
    private bool _mapsComboBoxEnabled;
    private bool _uiComboBoxEnabled;

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

    public KeyValuePair<string, eLanguage> SelectedTextLanguage
    {
        get => _selectedTextLanguage;
        set
        {
            if (value.Equals(_selectedTextLanguage)) return;
            _selectedTextLanguage = value;

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

    public KeyValuePair<string, eLanguage> SelectedAudioLanguage
    {
        get => _selectedAudioLanguage;
        set
        {
            if (value.Equals(_selectedAudioLanguage)) return;
            _selectedAudioLanguage = value;

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

    public async Task Initialize()
    {
        await InitializeLanguage();
        await InitializeMods();
    }

    public async Task InitializeLanguage()
    {
        eLanguage appLanguage = ((eLanguage)Settings.Default.AppLanguage);
        SelectedAppLanguage = new KeyValuePair<string, eLanguage>(appLanguage.GetAttributeOfType<DisplayAttribute>().Name, appLanguage);

        eLanguage textLanguage = ((eLanguage)Settings.Default.TextLanguage);
        SelectedTextLanguage = new KeyValuePair<string, eLanguage>(textLanguage.GetAttributeOfType<DisplayAttribute>().Name, textLanguage);

        eLanguage audioLanguage = ((eLanguage)Settings.Default.AudioLanguage);
        SelectedAudioLanguage = new KeyValuePair<string, eLanguage>(audioLanguage.GetAttributeOfType<DisplayAttribute>().Name, audioLanguage);



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

        if (!string.IsNullOrEmpty(Settings.Default.SelectedMod))
        {
            SelectedMod = Settings.Default.SelectedMod;
        }
    }

    private async Task Translate()
    {
        ModTitle = await TranslateGoogleAsync(ModTitle);
        ModDescription = await TranslateGoogleAsync(ModDescription);
        LauncherTitle = await TranslateGoogleAsync(LauncherTitle);
        LauncherDescription = await TranslateGoogleAsync(LauncherDescription);
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

    [UsedImplicitly]
    public async void OnModSelectionChanged()
    {
        //TODO: Need to create a bool to enable/disable drawer "Customizations" based on mod type.
        //TODO: Need to create a bool to enable/disable drawer "WIKI" based on mod type.
        Settings.Default.SelectedMod = SelectedMod;
        Settings.Default.Save();
    }

    [UsedImplicitly]
    public async void OnTextLanguageSelectionChanged()
    {
        //TODO: Need to first finish mod selection and "MyUserSettings"
        Settings.Default.TextLanguage = (int)SelectedTextLanguage.Value;
        Settings.Default.Save();
    }

    [UsedImplicitly]
    public async void OnAudioLanguageSelectionChanged()
    {
        //TODO: Need to first finish mod selection and "MyUserSettings"
        Settings.Default.AudioLanguage = (int)SelectedAudioLanguage.Value;
        Settings.Default.Save();
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

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
}