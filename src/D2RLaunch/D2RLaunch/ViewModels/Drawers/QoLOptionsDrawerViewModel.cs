using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using D2RLaunch.Models;
using D2RLaunch.Models.Enums;
using D2RLaunch.Properties;
using D2RLaunch.ViewModels.Dialogs;
using JetBrains.Annotations;
using Syncfusion.Licensing;

namespace D2RLaunch.ViewModels.Drawers;

public class QoLOptionsDrawerViewModel : INotifyPropertyChanged
{
    #region members

    private IWindowManager _windowManager;
    private bool _showFontPreview;
    private ImageSource _fontImage;
    private ObservableCollection<KeyValuePair<string, eFont>> _fonts = new();
    private ObservableCollection<KeyValuePair<string, eBackup>> _backupsSettings = new ();
    private ObservableCollection<KeyValuePair<string, eEnabledDisabled>> _enabledDisabledOptions = new();
    private ObservableCollection<KeyValuePair<string, eSkillIconPack>> _skillIconPacks = new();
    private ObservableCollection<KeyValuePair<string, eMercIdentifier>> _mercIdentifiers = new();
    private ObservableCollection<KeyValuePair<string, eMonsterStats>> _monsterStats = new();
    private ObservableCollection<KeyValuePair<string, eItemDisplay>> _itemDisplays = new();
    private ObservableCollection<KeyValuePair<string, eRunewordSorting>> _runewordSorting = new();
    private ObservableCollection<KeyValuePair<string, eHudDesign>> _hudDesigns = new();

    #endregion

    public QoLOptionsDrawerViewModel()
    {
        if (Execute.InDesignMode)
        { }
    }

    public QoLOptionsDrawerViewModel(ShellViewModel shellViewModel, IWindowManager windowManager)
    {
        ShellViewModel = shellViewModel;
        _windowManager = windowManager;
    }

    #region properties

    public ObservableCollection<KeyValuePair<string, eHudDesign>> HudDesigns
    {
        get => _hudDesigns;
        set
        {
            if (Equals(value, _hudDesigns)) return;
            _hudDesigns = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<KeyValuePair<string, eRunewordSorting>> RunewordSorting
    {
        get => _runewordSorting;
        set
        {
            if (Equals(value, _runewordSorting)) return;
            _runewordSorting = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<KeyValuePair<string, eItemDisplay>> ItemDisplays
    {
        get => _itemDisplays;
        set
        {
            if (Equals(value, _itemDisplays)) return;
            _itemDisplays = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<KeyValuePair<string, eMonsterStats>> MonsterStats
    {
        get => _monsterStats;
        set
        {
            if (Equals(value, _monsterStats)) return;
            _monsterStats = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<KeyValuePair<string, eMercIdentifier>> MercIdentifiers
    {
        get => _mercIdentifiers;
        set
        {
            if (Equals(value, _mercIdentifiers)) return;
            _mercIdentifiers = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<KeyValuePair<string, eSkillIconPack>> SkillIconPacks
    {
        get => _skillIconPacks;
        set
        {
            if (Equals(value, _skillIconPacks)) return;
            _skillIconPacks = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<KeyValuePair<string, eEnabledDisabled>> EnabledDisabledOptions
    {
        get => _enabledDisabledOptions;
        set
        {
            if (Equals(value, _enabledDisabledOptions)) return;
            _enabledDisabledOptions = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<KeyValuePair<string, eBackup>> BackupsSettings
    {
        get => _backupsSettings;
        set
        {
            if (Equals(value, _backupsSettings)) return;
            _backupsSettings = value;
            OnPropertyChanged();
        }
    }

    public ShellViewModel ShellViewModel { get; }

    public ImageSource FontImage
    {
        get => _fontImage;
        set
        {
            if (value == _fontImage) return;
            _fontImage = value;
            OnPropertyChanged();
        }
    }

    public bool ShowFontPreview
    {
        get => _showFontPreview;
        set
        {
            if (value == _showFontPreview) return;
            _showFontPreview = value;
            Task.Run(UpdateFontPreview);
            OnPropertyChanged();
        }
    }

    public ObservableCollection<KeyValuePair<string, eFont>> Fonts
    {
        get => _fonts;
        set
        {
            if (Equals(value, _fonts))
            {
                return;
            }
            _fonts = value;
            OnPropertyChanged();
        }
    }

    #endregion

    public event PropertyChangedEventHandler PropertyChanged;

    public async Task Initialize()
    {
        await UpdateFontPreview();

        foreach (eFont font in Enum.GetValues<eFont>())
        {
            Fonts.Add(new KeyValuePair<string, eFont>(font.GetAttributeOfType<DisplayAttribute>().Name, font));
        }

        foreach (eBackup backupSetting in Enum.GetValues<eBackup>())
        {
            BackupsSettings.Add(new KeyValuePair<string, eBackup>(backupSetting.GetAttributeOfType<DisplayAttribute>().Name, backupSetting));
        }

        foreach (eEnabledDisabled enabledDisabledSetting in Enum.GetValues<eEnabledDisabled>())
        {
            EnabledDisabledOptions.Add(new KeyValuePair<string, eEnabledDisabled>(enabledDisabledSetting.GetAttributeOfType<DisplayAttribute>().Name, enabledDisabledSetting));
        }

        foreach (eSkillIconPack skillIconPackSetting in Enum.GetValues<eSkillIconPack>())
        {
            SkillIconPacks.Add(new KeyValuePair<string, eSkillIconPack>(skillIconPackSetting.GetAttributeOfType<DisplayAttribute>().Name, skillIconPackSetting));
        }

        foreach (eMercIdentifier mercIdentifierSetting in Enum.GetValues<eMercIdentifier>())
        {
            MercIdentifiers.Add(new KeyValuePair<string, eMercIdentifier>(mercIdentifierSetting.GetAttributeOfType<DisplayAttribute>().Name, mercIdentifierSetting));
        }

        foreach (eMonsterStats monsterStatsSetting in Enum.GetValues<eMonsterStats>())
        {
            MonsterStats.Add(new KeyValuePair<string, eMonsterStats>(monsterStatsSetting.GetAttributeOfType<DisplayAttribute>().Name, monsterStatsSetting));
        }

        foreach (eItemDisplay itemDisplaySetting in Enum.GetValues<eItemDisplay>())
        {
            ItemDisplays.Add(new KeyValuePair<string, eItemDisplay>(itemDisplaySetting.GetAttributeOfType<DisplayAttribute>().Name, itemDisplaySetting));
        }

        foreach (eRunewordSorting runewordSortingSetting in Enum.GetValues<eRunewordSorting>())
        {
            RunewordSorting.Add(new KeyValuePair<string, eRunewordSorting>(runewordSortingSetting.GetAttributeOfType<DisplayAttribute>().Name, runewordSortingSetting));
        }

        foreach (eHudDesign hudDesignSetting in Enum.GetValues<eHudDesign>())
        {
            HudDesigns.Add(new KeyValuePair<string, eHudDesign>(hudDesignSetting.GetAttributeOfType<DisplayAttribute>().Name, hudDesignSetting));
        }
    }

    [UsedImplicitly]
    public async void OnHudDesignDisplayPreview()
    {
        await ShowPreviewImage("Preview_HUD.png", "HUD Preview");
    }

    [UsedImplicitly]
    public async void OnSuperTelekinesisPreview()
    {
        await ShowPreviewImage("Preview_SuperTK.gif", "Super Telekinesis Preview");
    }

    [UsedImplicitly]
    public async void OnItemDisplayPreview()
    {
        await ShowPreviewImage("Preview_ItemIcons.png", "Item Icons Preview");
    }

    [UsedImplicitly]
    public async void OnMonsterStatsDisplayPreview()
    {
        await ShowPreviewImage("Preview_MonsterStats.gif", "Monster Stats Preview");
    }

    [UsedImplicitly]
    public async void OnRuneDisplayPreview()
    {
        await ShowPreviewImage("Preview_RuneDisplay.gif", "Rune Display Preview");
    }

    [UsedImplicitly]
    public async void OnMercIdentifierPreview()
    {
        await ShowPreviewImage("Preview_MercIcon.png", "Merc Icons Preview");
    }

    [UsedImplicitly]
    public async void OnSkillIconPreview()
    {
        await ShowPreviewImage("Preview_SkillIcons.gif", "Skill Icons Preview");
    }

    [UsedImplicitly]
    public async void OnSkillBuffIconsSettings()
    {
        dynamic options = new ExpandoObject();
        options.ResizeMode = ResizeMode.NoResize;
        options.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        BuffIconSettingsViewModel vm = new BuffIconSettingsViewModel(ShellViewModel);

        if (await _windowManager.ShowDialogAsync(vm, null, options))
        {

        }
    }

    [UsedImplicitly]
    public async void OnStashTabsSettings()
    {
        dynamic options = new ExpandoObject();
        options.ResizeMode = ResizeMode.NoResize;
        options.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        StashTabSettingsViewModel vm = new StashTabSettingsViewModel(ShellViewModel);

        if (await _windowManager.ShowDialogAsync(vm, null, options))
        {

        }
    }

    [UsedImplicitly]
    public async void OnBackup()
    {
        (string characterName, bool passed) result = await ShellViewModel.BackupRecentCharacter();

        if (result.passed)
        {
            MessageBox.Show($"{result.characterName} along with stash has been backed up successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show("Failed to backup character!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [UsedImplicitly]
    public async void OnRestoreBackup()
    {
        dynamic options = new ExpandoObject();
        options.ResizeMode = ResizeMode.NoResize;
        options.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        RestoreBackupViewModel vm = new RestoreBackupViewModel(ShellViewModel);

        if (await _windowManager.ShowDialogAsync(vm, null, options))
        {

        }
    }

    [UsedImplicitly]
    public async void OnUseFont()
    {
        string fontsFolder = Path.Combine(ShellViewModel.SelectedModDataFolder, "hd/ui/fonts");


        byte[] font = await Helper.GetResourceByteArray($"Fonts.{ShellViewModel.UserSettings.Font}.otf");

        if (!Directory.Exists(fontsFolder))
        {
            Directory.CreateDirectory(fontsFolder);
            File.Create(Path.Combine(fontsFolder, "exocetblizzardot-medium.otf")).Close();
        }

        await File.WriteAllBytesAsync(Path.Combine(fontsFolder, "exocetblizzardot-medium.otf"), font);


        MessageBox.Show($"Font \"{((eFont)ShellViewModel.UserSettings.Font).GetAttributeOfType<DisplayAttribute>().Name}\" Loaded!");
    }

    [UsedImplicitly]
    public async void OnUsePreview()
    {
        if (ShowFontPreview)
        {
            await UpdateFontPreview();
        }
    }

    private async Task UpdateFontPreview()
    {
       await Execute.OnUIThreadAsync(async () =>
                                     {
                                         BitmapImage biImg = new BitmapImage();
                                         byte[] image = await Helper.GetResourceByteArray($"Fonts.{ShellViewModel.UserSettings.Font}.png");
                                         MemoryStream ms = new MemoryStream(image);
                                         biImg.BeginInit();
                                         biImg.StreamSource = ms;
                                         biImg.EndInit();
                                         FontImage = biImg;
                                     });

    }

    private async Task ShowPreviewImage(string imageName, string title)
    {
        dynamic options = new ExpandoObject();
        options.ResizeMode = ResizeMode.NoResize;
        options.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        ImagePreviewerViewModel vm = new ImagePreviewerViewModel($"pack://application:,,,/Resources/Preview/{imageName}", title);

        if (await _windowManager.ShowDialogAsync(vm, null, options))
        {
            
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
}