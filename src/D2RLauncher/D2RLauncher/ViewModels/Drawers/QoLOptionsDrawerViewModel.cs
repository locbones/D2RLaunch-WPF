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
using D2RLauncher.Models;
using D2RLauncher.Models.Enums;
using D2RLauncher.Properties;
using D2RLauncher.ViewModels.Dialogs;
using D2RLauncher.Views.Dialogs;
using JetBrains.Annotations;
using Syncfusion.Licensing;

namespace D2RLauncher.ViewModels.Drawers;

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

        ShellViewModel.UserSettings.PropertyChanged += async (sender, args) => await UserSettingsOnPropertyChanged(sender, args);
    }

    private async Task UserSettingsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != null)
        {
            switch (e.PropertyName)
            {
                case nameof(ShellViewModel.UserSettings.AutoBackups):
                {
                    await ShellViewModel.StartAutoBackup();
                    break;
                }
                case nameof(ShellViewModel.UserSettings.BuffIcons):
                {
                    string buffIconsParticlesPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLaunch/Buff Icons/Particles");
                    string buffIconsParticlesDisabledPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLaunch/Buff Icons/Particles (Disabled)");

                    if ((eEnabledDisabled)ShellViewModel.UserSettings.BuffIcons == eEnabledDisabled.Disabled)
                    {
                        if (Directory.Exists(buffIconsParticlesPath))
                        {
                            Directory.Move(buffIconsParticlesPath, buffIconsParticlesDisabledPath);
                        }
                    }
                    if ((eEnabledDisabled)ShellViewModel.UserSettings.BuffIcons == eEnabledDisabled.Enabled)
                    {
                        if (Directory.Exists(buffIconsParticlesDisabledPath))
                        {
                            Directory.Move(buffIconsParticlesDisabledPath, buffIconsParticlesPath);
                        }
                            
                    }
                    break;
                }
                case nameof(ShellViewModel.UserSettings.SkillIcons):
                {
                    eSkillIconPack skillIconPack = (eSkillIconPack) ShellViewModel.UserSettings.SkillIcons;

                    string globalUiSpellPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "hd/global/ui/spells");
                    string amazonSkillIconsPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "hd/global/ui/spells/amazon/amskillicon2.sprite");
                    string profileHdJsonPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/ui/layouts/_profilehd.json");
                    string skillsTreePanelHdJsonPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/ui/layouts/skillstreepanelhd.json");

                    string cascProfileHdJsonFileName = @"data:data\global\ui\layouts\_profilehd.json";
                    string cascSkillsTreePanelHdJsonFileName = @"data:data\global\ui\layouts\skillstreepanelhd.json";

                    //Create Skill Icons if they don't exist
                    if (!File.Exists(amazonSkillIconsPath))
                    {
                        string skillIconsPath = Path.Combine(Path.GetTempPath(), "SkillIcons.zip");

                        await File.WriteAllBytesAsync(skillIconsPath, await Helper.GetResourceByteArray("Options.D2RL_SkillIcons.zip"));
                        ZipFile.ExtractToDirectory(skillIconsPath, globalUiSpellPath);
                        File.Delete(skillIconsPath);
                    }

                    switch (skillIconPack)
                    {
                        case eSkillIconPack.Disabled:
                        {
                                    if (File.Exists(profileHdJsonPath))
                                    {
                                        string profileContents = await File.ReadAllTextAsync(profileHdJsonPath);
                                        profileContents = profileContents.Replace("AmSkillicon2\"", "AmSkillicon\"").Replace("AmSkillicon3\"", "AmSkillicon\"");
                                        profileContents = profileContents.Replace("AsSkillicon2\"", "AsSkillicon\"").Replace("AsSkillicon3\"", "AsSkillicon\"");
                                        profileContents = profileContents.Replace("BaSkillicon2\"", "BaSkillicon\"").Replace("BaSkillicon3\"", "BaSkillicon\"");
                                        profileContents = profileContents.Replace("DrSkillicon2\"", "DrSkillicon\"").Replace("DrSkillicon3\"", "DrSkillicon\"");
                                        profileContents = profileContents.Replace("NeSkillicon2\"", "NeSkillicon\"").Replace("NeSkillicon3\"", "NeSkillicon\"");
                                        profileContents = profileContents.Replace("PaSkillicon2\"", "PaSkillicon\"").Replace("PaSkillicon3\"", "PaSkillicon\"");
                                        profileContents = profileContents.Replace("SoSkillicon2\"", "SoSkillicon\"").Replace("SoSkillicon3\"", "SoSkillicon\"");
                                        await File.WriteAllTextAsync(profileHdJsonPath, profileContents);
                                    }

                                    if (File.Exists(skillsTreePanelHdJsonPath))
                                    {
                                        string profileContents = await File.ReadAllTextAsync(skillsTreePanelHdJsonPath);
                                        profileContents = profileContents.Replace("AmSkillicon2\"", "AmSkillicon\"").Replace("AmSkillicon3\"", "AmSkillicon\"");
                                        profileContents = profileContents.Replace("AsSkillicon2\"", "AsSkillicon\"").Replace("AsSkillicon3\"", "AsSkillicon\"");
                                        profileContents = profileContents.Replace("BaSkillicon2\"", "BaSkillicon\"").Replace("BaSkillicon3\"", "BaSkillicon\"");
                                        profileContents = profileContents.Replace("DrSkillicon2\"", "DrSkillicon\"").Replace("DrSkillicon3\"", "DrSkillicon\"");
                                        profileContents = profileContents.Replace("NeSkillicon2\"", "NeSkillicon\"").Replace("NeSkillicon3\"", "NeSkillicon\"");
                                        profileContents = profileContents.Replace("PaSkillicon2\"", "PaSkillicon\"").Replace("PaSkillicon3\"", "PaSkillicon\"");
                                        profileContents = profileContents.Replace("SoSkillicon2\"", "SoSkillicon\"").Replace("SoSkillicon3\"", "SoSkillicon\"");
                                        await File.WriteAllTextAsync(skillsTreePanelHdJsonPath, profileContents);
                                    }
                                    break;
                        }
                        case eSkillIconPack.ReMoDDeD:
                        {
                                    if (!File.Exists(profileHdJsonPath))
                                    {
                                        Helper.ExtractFileFromCasc(ShellViewModel.GamePath, cascProfileHdJsonFileName, ShellViewModel.BaseModsFolder, "data:data", "data");
                                    }

                                    if (!File.Exists(skillsTreePanelHdJsonPath))
                                    {
                                        Helper.ExtractFileFromCasc(ShellViewModel.GamePath, cascSkillsTreePanelHdJsonFileName, ShellViewModel.BaseModsFolder, "data:data", "data");
                                    }

                                    if (File.Exists(profileHdJsonPath))
                                    {
                                        string profileContents = await File.ReadAllTextAsync(profileHdJsonPath);
                                        profileContents = profileContents.Replace("AmSkillicon\"", "AmSkillicon2\"").Replace("AmSkillicon3\"", "AmSkillicon2\"");
                                        profileContents = profileContents.Replace("AsSkillicon\"", "AsSkillicon2\"").Replace("AsSkillicon3\"", "AsSkillicon2\"");
                                        profileContents = profileContents.Replace("BaSkillicon\"", "BaSkillicon2\"").Replace("BaSkillicon3\"", "BaSkillicon2\"");
                                        profileContents = profileContents.Replace("DrSkillicon\"", "DrSkillicon2\"").Replace("DrSkillicon3\"", "DrSkillicon2\"");
                                        profileContents = profileContents.Replace("NeSkillicon\"", "NeSkillicon2\"").Replace("NeSkillicon3\"", "NeSkillicon2\"");
                                        profileContents = profileContents.Replace("PaSkillicon\"", "PaSkillicon2\"").Replace("PaSkillicon3\"", "PaSkillicon2\"");
                                        profileContents = profileContents.Replace("SoSkillicon\"", "SoSkillicon2\"").Replace("SoSkillicon3\"", "SoSkillicon2\"");
                                        await File.WriteAllTextAsync(profileHdJsonPath, profileContents);
                                    }

                                    if (File.Exists(skillsTreePanelHdJsonPath))
                                    {
                                        string profileContents = await File.ReadAllTextAsync(skillsTreePanelHdJsonPath);
                                        profileContents = profileContents.Replace("AmSkillicon\"", "AmSkillicon2\"").Replace("AmSkillicon3\"", "AmSkillicon2\"");
                                        profileContents = profileContents.Replace("AsSkillicon\"", "AsSkillicon2\"").Replace("AsSkillicon3\"", "AsSkillicon2\"");
                                        profileContents = profileContents.Replace("BaSkillicon\"", "BaSkillicon2\"").Replace("BaSkillicon3\"", "BaSkillicon2\"");
                                        profileContents = profileContents.Replace("DrSkillicon\"", "DrSkillicon2\"").Replace("DrSkillicon3\"", "DrSkillicon2\"");
                                        profileContents = profileContents.Replace("NeSkillicon\"", "NeSkillicon2\"").Replace("NeSkillicon3\"", "NeSkillicon2\"");
                                        profileContents = profileContents.Replace("PaSkillicon\"", "PaSkillicon2\"").Replace("PaSkillicon3\"", "PaSkillicon2\"");
                                        profileContents = profileContents.Replace("SoSkillicon\"", "SoSkillicon2\"").Replace("SoSkillicon3\"", "SoSkillicon2\"");
                                        await File.WriteAllTextAsync(skillsTreePanelHdJsonPath, profileContents);
                                    }
                                    break;
                        }
                        case eSkillIconPack.Dize:
                        {
                                    if (!File.Exists(profileHdJsonPath))
                                    {
                                        Helper.ExtractFileFromCasc(ShellViewModel.GamePath, cascProfileHdJsonFileName, ShellViewModel.BaseModsFolder, "data:data", "data");
                                    }

                                    if (!File.Exists(skillsTreePanelHdJsonPath))
                                    {
                                        Helper.ExtractFileFromCasc(ShellViewModel.GamePath, cascSkillsTreePanelHdJsonFileName, ShellViewModel.BaseModsFolder, "data:data", "data");
                                    }

                                    if (File.Exists(profileHdJsonPath))
                                    {
                                        string profileContents = await File.ReadAllTextAsync(profileHdJsonPath);
                                        profileContents = profileContents.Replace("AmSkillicon2\"", "AmSkillicon3\"").Replace("AmSkillicon\"", "AmSkillicon3\"");
                                        profileContents = profileContents.Replace("AsSkillicon2\"", "AsSkillicon3\"").Replace("AsSkillicon\"", "AsSkillicon3\"");
                                        profileContents = profileContents.Replace("BaSkillicon2\"", "BaSkillicon3\"").Replace("BaSkillicon\"", "BaSkillicon3\"");
                                        profileContents = profileContents.Replace("DrSkillicon2\"", "DrSkillicon3\"").Replace("DrSkillicon\"", "DrSkillicon3\"");
                                        profileContents = profileContents.Replace("NeSkillicon2\"", "NeSkillicon3\"").Replace("NeSkillicon\"", "NeSkillicon3\"");
                                        profileContents = profileContents.Replace("PaSkillicon2\"", "PaSkillicon3\"").Replace("PaSkillicon\"", "PaSkillicon3\"");
                                        profileContents = profileContents.Replace("SoSkillicon2\"", "SoSkillicon3\"").Replace("SoSkillicon\"", "SoSkillicon3\"");
                                        await File.WriteAllTextAsync(profileHdJsonPath, profileContents);
                                    }

                                    if (File.Exists(skillsTreePanelHdJsonPath))
                                    {
                                        string profileContents = await File.ReadAllTextAsync(skillsTreePanelHdJsonPath);
                                        profileContents = profileContents.Replace("AmSkillicon2\"", "AmSkillicon3\"").Replace("AmSkillicon\"", "AmSkillicon3\"");
                                        profileContents = profileContents.Replace("AsSkillicon2\"", "AsSkillicon3\"").Replace("AsSkillicon\"", "AsSkillicon3\"");
                                        profileContents = profileContents.Replace("BaSkillicon2\"", "BaSkillicon3\"").Replace("BaSkillicon\"", "BaSkillicon3\"");
                                        profileContents = profileContents.Replace("DrSkillicon2\"", "DrSkillicon3\"").Replace("DrSkillicon\"", "DrSkillicon3\"");
                                        profileContents = profileContents.Replace("NeSkillicon2\"", "NeSkillicon3\"").Replace("NeSkillicon\"", "NeSkillicon3\"");
                                        profileContents = profileContents.Replace("PaSkillicon2\"", "PaSkillicon3\"").Replace("PaSkillicon\"", "PaSkillicon3\"");
                                        profileContents = profileContents.Replace("SoSkillicon2\"", "SoSkillicon3\"").Replace("SoSkillicon\"", "SoSkillicon3\"");
                                        await File.WriteAllTextAsync(skillsTreePanelHdJsonPath, profileContents);
                                    }
                                    break;
                        }
                    }
                    break;
                }
                case nameof(ShellViewModel.UserSettings.MercIcons):
                {
                    eMercIdentifier mercIdentifier = (eMercIdentifier)ShellViewModel.UserSettings.MercIcons;

                    string dataHdPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "hd");
                    string mercNameTexturePath = Path.Combine(dataHdPath, "vfx/textures/MercName.texture");
                    string mercNameParticlesPath = Path.Combine(dataHdPath, "vfx/particles/MercName.particles");
                    string mercNameDimTexturePath = Path.Combine(dataHdPath, "vfx/textures/MercNameDim.texture");
                    string rogueHireJsonPath = Path.Combine(dataHdPath, "character/enemy/roguehire.json");
                    string act2HireJsonPath = Path.Combine(dataHdPath, "character/enemy/act2hire.json");
                    string act3HireJsonPath = Path.Combine(dataHdPath, "character/enemy/act3hire.json");
                    string act5HireJsonPath = Path.Combine(dataHdPath, "character/enemy/act5hire1.json");
                    string enemyPath = Path.Combine(dataHdPath, "character/enemy");
                    string texturesPath = Path.Combine(dataHdPath, "vfx/textures");
                    string particlesPath = Path.Combine(dataHdPath, "vfx/particles");

                        switch (mercIdentifier)
                    {
                        case eMercIdentifier.Disabled:
                        {
                            if (File.Exists(mercNameTexturePath))
                            {
                                File.Delete(mercNameTexturePath);
                                File.Delete(mercNameDimTexturePath);
                            }
                            if (File.Exists(rogueHireJsonPath))
                            {
                                File.Delete(rogueHireJsonPath);
                                File.Delete(act2HireJsonPath);
                                File.Delete(act3HireJsonPath);
                                File.Delete(act5HireJsonPath);
                            }
                            break;
                        }
                        case eMercIdentifier.Enabled:
                        {
                            if (!Directory.Exists(enemyPath))
                            {
                                Directory.CreateDirectory(enemyPath);
                            }
                            if (!Directory.Exists(texturesPath))
                            {
                                Directory.CreateDirectory(texturesPath);
                            }
                            if (!Directory.Exists(particlesPath))
                            {
                                Directory.CreateDirectory(particlesPath);
                            }
                            await File.WriteAllBytesAsync(rogueHireJsonPath, await Helper.GetResourceByteArray("Options.MercIcons.roguehire.json"));
                            await File.WriteAllBytesAsync(act2HireJsonPath, await Helper.GetResourceByteArray("Options.MercIcons.act2hire.json"));
                            await File.WriteAllBytesAsync(act3HireJsonPath, await Helper.GetResourceByteArray("Options.MercIcons.act3hire.json"));
                            await File.WriteAllBytesAsync(act5HireJsonPath, await Helper.GetResourceByteArray("Options.MercIcons.act5hire1.json"));
                            await File.WriteAllBytesAsync(mercNameTexturePath, await Helper.GetResourceByteArray("Options.MercIcons.MercName1a.texture"));
                            await File.WriteAllBytesAsync(mercNameDimTexturePath, await Helper.GetResourceByteArray("Options.MercIcons.MercName1b.texture"));
                            await File.WriteAllBytesAsync(mercNameParticlesPath, await Helper.GetResourceByteArray("Options.MercIcons.MercName.particles"));
                            break;
                        }
                        case eMercIdentifier.EnabledMini:
                        {
                            if (!Directory.Exists(enemyPath))
                            {
                                Directory.CreateDirectory(enemyPath);
                            }
                            if (!Directory.Exists(texturesPath))
                            {
                                Directory.CreateDirectory(texturesPath);
                            }
                            if (!Directory.Exists(particlesPath))
                            {
                                Directory.CreateDirectory(particlesPath);
                            }
                            await File.WriteAllBytesAsync(rogueHireJsonPath, await Helper.GetResourceByteArray("Options.MercIcons.roguehire.json"));
                            await File.WriteAllBytesAsync(act2HireJsonPath, await Helper.GetResourceByteArray("Options.MercIcons.act2hire.json"));
                            await File.WriteAllBytesAsync(act3HireJsonPath, await Helper.GetResourceByteArray("Options.MercIcons.act3hire.json"));
                            await File.WriteAllBytesAsync(act5HireJsonPath, await Helper.GetResourceByteArray("Options.MercIcons.act5hire1.json"));
                            await File.WriteAllBytesAsync(mercNameTexturePath, await Helper.GetResourceByteArray("Options.MercIcons.MercName2a.texture"));
                            await File.WriteAllBytesAsync(mercNameDimTexturePath, await Helper.GetResourceByteArray("Options.MercIcons.MercName2b.texture"));
                            await File.WriteAllBytesAsync(mercNameParticlesPath, await Helper.GetResourceByteArray("Options.MercIcons.MercName.particles"));
                                    break;
                        }
                    }
                    break;
                }
                case nameof(ShellViewModel.UserSettings.ItemIlvls):
                {
                    string excelPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/excel");
                    string armorTxtPath = Path.Combine(excelPath, "armor.txt");
                    string weaponsTxtPath = Path.Combine(excelPath, "weapons.txt");
                    string miscTxtPath = Path.Combine(excelPath, "misc.txt");

                    string cascMiscTxtFileName = @"data:data\global\excel\misc.txt";
                    string cascArmorTxtFileName = @"data:data\global\excel\armor.txt";
                    string cascWeaponsTxtFileName = @"data:data\global\excel\weapons.txt";

                    string[] files = new string[] {"armor.txt", "misc.txt", "weapons.txt"};

                    if (ShellViewModel.ModInfo.Name != "ReMoDDeD")
                    {
                        if (!Directory.Exists(excelPath))
                        {
                            Directory.CreateDirectory(excelPath);
                        }

                        if (!File.Exists(armorTxtPath))
                        {
                            File.Create(armorTxtPath).Close();
                            Helper.ExtractFileFromCasc(ShellViewModel.GamePath, cascArmorTxtFileName, ShellViewModel.BaseModsFolder, "data:data", "data");
                        }
                        if (!File.Exists(weaponsTxtPath))
                        {
                            File.Create(weaponsTxtPath).Close();
                            Helper.ExtractFileFromCasc(ShellViewModel.GamePath, cascWeaponsTxtFileName, ShellViewModel.BaseModsFolder, "data:data", "data");
                        }
                        if (!File.Exists(miscTxtPath))
                        {
                            File.Create(miscTxtPath).Close();
                            Helper.ExtractFileFromCasc(ShellViewModel.GamePath, cascMiscTxtFileName, ShellViewModel.BaseModsFolder, "data:data", "data");
                        }
                    }

                    //search the defined files
                    foreach (string file in files)
                    {
                        string filePath = Path.Combine(excelPath, file);
                        string[] lines = await File.ReadAllLinesAsync(filePath);
                        string[] headers = lines[0].Split('\t'); //split by tab-delimited format
                        int showLevelIndex = Array.IndexOf(headers, "ShowLevel"); //make an array from the 'ShowLevel' entries

                        //search through 'ShowLevel' entries further
                        for (int i = 1; i < lines.Length; i++)
                        {
                            string[] columns = lines[i].Split('\t');
                            //check if entries match the dropdown index of 0 or 1
                            if (columns.Length > showLevelIndex && columns[showLevelIndex] != ShellViewModel.UserSettings.ItemIlvls.ToString())
                            {
                                columns[showLevelIndex] = ShellViewModel.UserSettings.ItemIlvls.ToString();
                                lines[i] = string.Join("\t", columns); //replace the 0 or 1 values as dropdown indicates
                            }
                        }
                        //We done boys
                        File.WriteAllLines(filePath, lines);
                    }

                    break;
                }
                case nameof(ShellViewModel.UserSettings.RuneDisplay):
                {
                    eEnabledDisabled runeDisplay = (eEnabledDisabled)ShellViewModel.UserSettings.RuneDisplay;

                    //Define replacement strings
                    string runePath = Path.Combine(ShellViewModel.SelectedModDataFolder, "hd/items/misc/rune");
                    string runeStringPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "local/lng/strings");
                    string runeStringJsonFile = Path.Combine(ShellViewModel.SelectedModDataFolder, "local/lng/strings/item-runes.json");
                    string noOverlay = "\"terrainBlendMode\": 1\r\n                }\r\n            ]";
                    string overlay1 = "\"terrainBlendMode\": 1\r\n                },\r\n                {\r\n                    \"type\": \"VfxDefinitionComponent\",\r\n                    \"name\": \"item_icon_2\",\r\n                    \"filename\": \"data/hd/vfx/particles/overlays/objects/multigleam/fx_multigleam.particles\",\r\n                    \"hardKillOnDestroy\": false\r\n                },\r\n                {\r\n                    \"type\": \"TransformDefinitionComponent\",\r\n                    \"name\": \"entity_root_TransformDefinition\",\r\n                    \"inheritOnlyPosition\": true\r\n                }\r\n            ]";
                    string overlay2 = "\"terrainBlendMode\": 1\r\n                },\r\n                {\r\n                    \"type\": \"VfxDefinitionComponent\",\r\n                    \"name\": \"item_icon_2\",\r\n                    \"filename\": \"data/hd/vfx/particles/overlays/common/impregnated/vfx_impregnated.particles\",\r\n                    \"hardKillOnDestroy\": false\r\n                },\r\n                {\r\n                    \"type\": \"TransformDefinitionComponent\",\r\n                    \"name\": \"entity_root_TransformDefinition\",\r\n                    \"inheritOnlyPosition\": true\r\n                }\r\n            ]";
                    string[] fileNames = null;

                    string cascItemRuneJsonFileName = @"data:data\local\lng\strings\item-runes.json";

                    switch (runeDisplay)
                    {
                        case eEnabledDisabled.Disabled:
                        {
                            if (!Directory.Exists(runePath))
                            {
                                Directory.CreateDirectory(runePath);
                            }

                            fileNames = Directory.GetFiles(runePath, "*.json");

                            foreach (string fileName in fileNames)
                            {
                                string fileContent = await File.ReadAllTextAsync(fileName);
                                fileContent = fileContent.Replace(overlay1, noOverlay);
                                fileContent = fileContent.Replace(overlay2, noOverlay);
                                await File.WriteAllTextAsync(fileName, fileContent);
                            }

                            //Replace rune string contents to display names
                            if (!Directory.Exists(runeStringPath))
                            {
                                Directory.CreateDirectory(runeStringPath);
                            }
                            if (!File.Exists(runeStringJsonFile))
                            {
                                Helper.ExtractFileFromCasc(ShellViewModel.GamePath, cascItemRuneJsonFileName, ShellViewModel.BaseModsFolder, "data:data", "data");
                            }

                            string runeStrings = await File.ReadAllTextAsync(runeStringJsonFile);
                            runeStrings = runeStrings.Replace("\"⅐ Elÿc0\"", "\"El Rune\"");
                            runeStrings = runeStrings.Replace("\"⅑ Eldÿc0\"", "\"Eld Rune\"");
                            runeStrings = runeStrings.Replace("\"⅒ Tirÿc0\"", "\"Tir Rune\"");
                            runeStrings = runeStrings.Replace("\"⅓ Nefÿc0\"", "\"Nef Rune\"");
                            runeStrings = runeStrings.Replace("\"⅔ Ethÿc0\"", "\"Eth Rune\"");
                            runeStrings = runeStrings.Replace("\"⅕ Ithÿc0\"", "\"Ith Rune\"");
                            runeStrings = runeStrings.Replace("\"⅖ Talÿc0\"", "\"Tal Rune\"");
                            runeStrings = runeStrings.Replace("\"⅗ Ralÿc0\"", "\"Ral Rune\"");
                            runeStrings = runeStrings.Replace("\"⅘ Ortÿc0\"", "\"Ort Rune\"");
                            runeStrings = runeStrings.Replace("\"⅙ Thulÿc0\"", "\"Thul Rune\"");
                            runeStrings = runeStrings.Replace("\"⅚ Amnÿc0\"", "\"Amn Rune\"");
                            runeStrings = runeStrings.Replace("\"⅛ Solÿc0\"", "\"Sol Rune\"");
                            runeStrings = runeStrings.Replace("\"⅜ Shaelÿc0\"", "\"Shael Rune\"");
                            runeStrings = runeStrings.Replace("\"⅝ Dolÿc0\"", "\"Dol Rune\"");
                            runeStrings = runeStrings.Replace("\"⅞ Helÿc0\"", "\"Hel Rune\"");
                            runeStrings = runeStrings.Replace("\"⅟ Ioÿc0\"", "\"Io Rune\"");
                            runeStrings = runeStrings.Replace("\"Ⅰ Lumÿc0\"", "\"Lum Rune\"");
                            runeStrings = runeStrings.Replace("\"Ⅱ Koÿc0\"", "\"Ko Rune\"");
                            runeStrings = runeStrings.Replace("\"Ⅲ Falÿc0\"", "\"Fal Rune\"");
                            runeStrings = runeStrings.Replace("\"Ⅳ Lemÿc0\"", "\"Lem Rune\"");
                            runeStrings = runeStrings.Replace("\"Ⅴ Pulÿc0\"", "\"Pul Rune\"");
                            runeStrings = runeStrings.Replace("\"Ⅵ Umÿc0\"", "\"Um Rune\"");
                            runeStrings = runeStrings.Replace("\"Ⅶ Malÿc0\"", "\"Mal Rune\"");
                            runeStrings = runeStrings.Replace("\"Ⅷ Istÿc0\"", "\"Ist Rune\"");
                            runeStrings = runeStrings.Replace("\"Ⅸ Gulÿc0\"", "\"Gul Rune\"");
                            runeStrings = runeStrings.Replace("\"Ⅹ Vexÿc0\"", "\"Vex Rune\"");
                            runeStrings = runeStrings.Replace("\"Ⅺ Ohmÿc0\"", "\"Ohm Rune\"");
                            runeStrings = runeStrings.Replace("\"Ⅻ Loÿc0\"", "\"Lo Rune\"");
                            runeStrings = runeStrings.Replace("\"Ⅼ Surÿc0\"", "\"Sur Rune\"");
                            runeStrings = runeStrings.Replace("\"Ⅽ Berÿc0\"", "\"Ber Rune\"");
                            runeStrings = runeStrings.Replace("\"Ⅾ Jahÿc0\"", "\"Jah Rune\"");
                            runeStrings = runeStrings.Replace("\"Ⅿ Chamÿc0\"", "\"Cham Rune\"");
                            runeStrings = runeStrings.Replace("\"ⅰ Zodÿc0\"", "\"Zod Rune\"");
                            await File.WriteAllTextAsync(runeStringJsonFile, runeStrings);
                            break;
                        }
                        case eEnabledDisabled.Enabled:
                        {
                                    string[] runeFiles1 = { "sol_rune.json", "shael_rune.json", "dol_rune.json", "hel_rune.json", "io_rune.json", "lum_rune.json", "ko_rune.json", "fal_rune.json", "lem_rune.json", "pul_rune.json", "um_rune.json" };
                                    string[] runeFiles2 = { "mal_rune.json", "ist_rune.json", "gul_rune.json", "vex_rune.json", "ohm_rune.json", "lo_rune.json", "sur_rune.json", "ber_rune.json", "jah_rune.json", "cham_rune.json", "zod_rune.json" };

                                    if (!Directory.Exists(runePath))
                                    {
                                        Directory.CreateDirectory(runePath);
                                    }

                                    //Assign overlay1 to mid runes
                                    foreach (string fileName in runeFiles1)
                                    {

                                        string filePath = Path.Combine(runePath, fileName);
                                        if (!File.Exists(filePath))
                                        {
                                            Helper.CreateFileIfNotExists(Path.Combine(runePath, "sol_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.sol_rune.json"));
                                            Helper.CreateFileIfNotExists(Path.Combine(runePath, "shael_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.shael_rune.json"));
                                            Helper.CreateFileIfNotExists(Path.Combine(runePath, "dol_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.dol_rune.json"));
                                            Helper.CreateFileIfNotExists(Path.Combine(runePath, "hel_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.hel_rune.json"));
                                            Helper.CreateFileIfNotExists(Path.Combine(runePath, "io_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.io_rune.json"));
                                            Helper.CreateFileIfNotExists(Path.Combine(runePath, "lum_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.lum_rune.json"));
                                            Helper.CreateFileIfNotExists(Path.Combine(runePath, "ko_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.ko_rune.json"));
                                            Helper.CreateFileIfNotExists(Path.Combine(runePath, "fal_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.fal_rune.json"));
                                            Helper.CreateFileIfNotExists(Path.Combine(runePath, "lem_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.lem_rune.json"));
                                            Helper.CreateFileIfNotExists(Path.Combine(runePath, "pul_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.pul_rune.json"));
                                            Helper.CreateFileIfNotExists(Path.Combine(runePath, "um_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.um_rune.json"));
                                        }

                                        if (File.Exists(filePath))
                                        {
                                            string fileContent = await File.ReadAllTextAsync(filePath);
                                            fileContent = fileContent.Replace(noOverlay, overlay1);
                                            await File.WriteAllTextAsync(filePath, fileContent);
                                        }
                                    }

                                    //Assign overlay2 to high runes
                                    foreach (string fileName in runeFiles2)
                                    {
                                        string filePath = Path.Combine(runePath, fileName);
                                        if (!File.Exists(filePath))
                                        {
                                            Helper.CreateFileIfNotExists(Path.Combine(runePath, "mal_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.mal_rune.json"));
                                            Helper.CreateFileIfNotExists(Path.Combine(runePath, "ist_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.ist_rune.json"));
                                            Helper.CreateFileIfNotExists(Path.Combine(runePath, "gul_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.gul_rune.json"));
                                            Helper.CreateFileIfNotExists(Path.Combine(runePath, "vex_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.vex_rune.json"));
                                            Helper.CreateFileIfNotExists(Path.Combine(runePath, "ohm_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.ohm_rune.json"));
                                            Helper.CreateFileIfNotExists(Path.Combine(runePath, "lo_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.lo_rune.json"));
                                            Helper.CreateFileIfNotExists(Path.Combine(runePath, "sur_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.sur_rune.json"));
                                            Helper.CreateFileIfNotExists(Path.Combine(runePath, "ber_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.ber_rune.json"));
                                            Helper.CreateFileIfNotExists(Path.Combine(runePath, "jah_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.jah_rune.json"));
                                            Helper.CreateFileIfNotExists(Path.Combine(runePath, "cham_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.cham_rune.json"));
                                            Helper.CreateFileIfNotExists(Path.Combine(runePath, "zod_rune.json"), await Helper.GetResourceByteArray("Options.ItemIcons.zod_rune.json"));
                                        }

                                        if (File.Exists(filePath))
                                        {
                                            string fileContent = await File.ReadAllTextAsync(filePath);
                                            fileContent = fileContent.Replace(noOverlay, overlay2);
                                            await File.WriteAllTextAsync(filePath, fileContent);
                                        }
                                    }

                                    //Replace rune string contents to display icons
                                    if (!File.Exists(runeStringJsonFile))
                                    {
                                        Helper.ExtractFileFromCasc(ShellViewModel.GamePath, cascItemRuneJsonFileName, ShellViewModel.BaseModsFolder, "data:data", "data");
                                    }

                                    string runeStrings = await File.ReadAllTextAsync(runeStringJsonFile);
                                    runeStrings = runeStrings.Replace("\"El Rune\"", "\"⅐ Elÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Eld Rune\"", "\"⅑ Eldÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Tir Rune\"", "\"⅒ Tirÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Nef Rune\"", "\"⅓ Nefÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Eth Rune\"", "\"⅔ Ethÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Ith Rune\"", "\"⅕ Ithÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Tal Rune\"", "\"⅖ Talÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Ral Rune\"", "\"⅗ Ralÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Ort Rune\"", "\"⅘ Ortÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Thul Rune\"", "\"⅙ Thulÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Amn Rune\"", "\"⅚ Amnÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Sol Rune\"", "\"⅛ Solÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Shael Rune\"", "\"⅜ Shaelÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Dol Rune\"", "\"⅝ Dolÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Hel Rune\"", "\"⅞ Helÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Io Rune\"", "\"⅟ Ioÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Lum Rune\"", "\"Ⅰ Lumÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Ko Rune\"", "\"Ⅱ Koÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Fal Rune\"", "\"Ⅲ Falÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Lem Rune\"", "\"Ⅳ Lemÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Pul Rune\"", "\"Ⅴ Pulÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Um Rune\"", "\"Ⅵ Umÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Mal Rune\"", "\"Ⅶ Malÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Ist Rune\"", "\"Ⅷ Istÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Gul Rune\"", "\"Ⅸ Gulÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Vex Rune\"", "\"Ⅹ Vexÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Ohm Rune\"", "\"Ⅺ Ohmÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Lo Rune\"", "\"Ⅻ Loÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Sur Rune\"", "\"Ⅼ Surÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Ber Rune\"", "\"Ⅽ Berÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Jah Rune\"", "\"Ⅾ Jahÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Cham Rune\"", "\"Ⅿ Chamÿc0\"");
                                    runeStrings = runeStrings.Replace("\"Zod Rune\"", "\"ⅰ Zodÿc0\"");
                                    await File.WriteAllTextAsync(runeStringJsonFile, runeStrings);
                                    break;
                        }
                    }
                    break;
                }
                case nameof(ShellViewModel.UserSettings.HideHelmets):
                {
                    eEnabledDisabled helmetDisplay = (eEnabledDisabled)ShellViewModel.UserSettings.HideHelmets;

                    //Define filenames and paths
                    string helmetBaseDir1 = Path.Combine(ShellViewModel.SelectedModDataFolder, "hd/items/armor/helmet");
                    string helmetBaseDir2 = Path.Combine(ShellViewModel.SelectedModDataFolder, "hd/items/armor/circlet");
                    string[] helmetFiles1 = new[] { "assault_helmet", "avenger_guard", "bone_helm", "cap_hat", "coif_of_glory", "crown", "crown_of_thieves", "duskdeep", "fanged_helm", "full_helm", "great_helm", "helm", "horned_helm", "jawbone_cap", "mask", "indals_almighty", "rockstopper", "skull_cap", "war_bonnet", "wormskull" };
                    string[] helmetFiles2 = new[] { "circlet", "coronet", "diadem", "tiara" };

                    //Add paths and extension to array
                    string[] helmetFilesWithExtension1 = helmetFiles1.Select(x => x + ".json").ToArray();
                    string[] helmetFilesWithExtension2 = helmetFiles2.Select(x => x + ".json").ToArray();
                    string[] allHelmetFiles1 = helmetFilesWithExtension1.Select(x => Path.Combine(helmetBaseDir1, x)).Concat(helmetFilesWithExtension1.Select(x => Path.Combine(helmetBaseDir1, x))).ToArray();
                    string[] allHelmetFiles2 = helmetFilesWithExtension2.Select(x => Path.Combine(helmetBaseDir2, x)).Concat(helmetFilesWithExtension2.Select(x => Path.Combine(helmetBaseDir2, x))).ToArray();

                    switch (helmetDisplay)
                    {
                        case eEnabledDisabled.Disabled:
                        {
                            foreach (string filename in allHelmetFiles1)
                            {
                                if (File.Exists(filename))
                                {
                                    File.Delete(filename);
                                }
                            }

                            foreach (string filename in allHelmetFiles2)
                            {
                                if (File.Exists(filename))
                                {
                                    File.Delete(filename);
                                }
                            }
                            break;
                        }
                        case eEnabledDisabled.Enabled:
                        {
                            //Create directories if they don't exist
                            if (!Directory.Exists(helmetBaseDir1))
                            {
                                Directory.CreateDirectory(helmetBaseDir1);
                            }
                            if (!Directory.Exists(helmetBaseDir2))
                            {
                                Directory.CreateDirectory(helmetBaseDir2);
                            }

                            //Loop through both arrays to create files
                            foreach (string filename in allHelmetFiles1)
                            {
                                if (File.Exists(filename))
                                {
                                    File.Delete(filename);
                                }
                                File.Create(filename).Close();
                                await File.WriteAllBytesAsync(filename, await Helper.GetResourceByteArray("Options.HideHelmets.hide_helmets.json"));
                            }

                            foreach (string filename in allHelmetFiles2)
                            {
                                if (File.Exists(filename))
                                {
                                    File.Delete(filename);
                                }
                                File.Create(filename).Close();
                                await File.WriteAllBytesAsync(filename, await Helper.GetResourceByteArray("Options.HideHelmets.hide_helmets.json"));
                            }
                            break;
                        }
                    }

                    break;
                }
                case nameof(ShellViewModel.UserSettings.MonsterStatsDisplay):
                {
                    eMonsterStats monsterStatsDisplay = (eMonsterStats)ShellViewModel.UserSettings.MonsterStatsDisplay;

                    string uiLayoutsPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/ui/layouts");
                    string hudMonsterHealthHdJsonFilePath = Path.Combine(uiLayoutsPath, "hudmonsterhealthhd.json");
                    string hudMonsterHealthHdDisabledJsonFilePath = Path.Combine(uiLayoutsPath, "hudmonsterhealthhd_disabled.json");
                    string monsterStatsPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLaunch/Monster Stats");

                    if (!Directory.Exists(uiLayoutsPath))
                    {
                        Directory.CreateDirectory(uiLayoutsPath);
                    }

                    if (!Directory.Exists(monsterStatsPath))
                    {
                        Directory.CreateDirectory(monsterStatsPath);
                    }

                    if (!File.Exists(hudMonsterHealthHdJsonFilePath))
                    {
                        await File.WriteAllBytesAsync(hudMonsterHealthHdJsonFilePath, await Helper.GetResourceByteArray("Options.MonsterStats.hudmonsterhealthhd.json"));
                    }

                    switch (monsterStatsDisplay)
                    {
                        case eMonsterStats.Background:
                        case eMonsterStats.NoBackground:
                        {
                            if (!File.Exists(Path.Combine(monsterStatsPath, "HB_L.sprite")))
                            {
                                await File.WriteAllBytesAsync(Path.Combine(monsterStatsPath, "HB_L.sprite"), await Helper.GetResourceByteArray("Options.MonsterStats.HB_L.sprite"));
                                await File.WriteAllBytesAsync(Path.Combine(monsterStatsPath, "HB_L.lowend.sprite"), await Helper.GetResourceByteArray("Options.MonsterStats.HB_L.lowend.sprite"));
                                await File.WriteAllBytesAsync(Path.Combine(monsterStatsPath, "HB_M.sprite"), await Helper.GetResourceByteArray("Options.MonsterStats.HB_M.sprite"));
                                await File.WriteAllBytesAsync(Path.Combine(monsterStatsPath, "HB_M.lowend.sprite"), await Helper.GetResourceByteArray("Options.MonsterStats.HB_M.lowend.sprite"));
                                await File.WriteAllBytesAsync(Path.Combine(monsterStatsPath, "HB_R.sprite"), await Helper.GetResourceByteArray("Options.MonsterStats.HB_R.sprite"));
                                await File.WriteAllBytesAsync(Path.Combine(monsterStatsPath, "HB_R.lowend.sprite"), await Helper.GetResourceByteArray("Options.MonsterStats.HB_R.lowend.sprite"));
                                await File.WriteAllBytesAsync(Path.Combine(monsterStatsPath, "HB_A.sprite"), await Helper.GetResourceByteArray("Options.MonsterStats.HB_A.sprite"));
                                await File.WriteAllBytesAsync(Path.Combine(monsterStatsPath, "HB_A.lowend.sprite"), await Helper.GetResourceByteArray("Options.MonsterStats.HB_A.lowend.sprite"));
                            }
                            break;
                        }
                    }

                    switch (monsterStatsDisplay)
                    {
                        case eMonsterStats.Disabled:
                        {
                            if (File.Exists(hudMonsterHealthHdJsonFilePath))
                            {
                                File.Delete(hudMonsterHealthHdDisabledJsonFilePath);
                                File.Move(hudMonsterHealthHdJsonFilePath, hudMonsterHealthHdDisabledJsonFilePath, true);
                            }
                            break;
                        }
                        case eMonsterStats.Background:
                        {
                            if (File.Exists(hudMonsterHealthHdDisabledJsonFilePath))
                            {
                                File.Delete(hudMonsterHealthHdJsonFilePath);
                                File.Move(hudMonsterHealthHdDisabledJsonFilePath, hudMonsterHealthHdJsonFilePath, true);
                            }

                            string content = await File.ReadAllTextAsync(hudMonsterHealthHdJsonFilePath);
                            await File.WriteAllTextAsync(hudMonsterHealthHdJsonFilePath, content.Replace("HB_L_Blank\"", "HB_L\""));
                            string content2 = await File.ReadAllTextAsync(hudMonsterHealthHdJsonFilePath);
                            await File.WriteAllTextAsync(hudMonsterHealthHdJsonFilePath, content2.Replace("HB_M_Blank\"", "HB_M\""));
                            string content3 = await File.ReadAllTextAsync(hudMonsterHealthHdJsonFilePath);
                            await File.WriteAllTextAsync(hudMonsterHealthHdJsonFilePath, content3.Replace("HB_R_Blank\"", "HB_R\""));
                            string content4 = await File.ReadAllTextAsync(hudMonsterHealthHdJsonFilePath);
                            await File.WriteAllTextAsync(hudMonsterHealthHdJsonFilePath, content4.Replace("HB_A_Blank\"", "HB_A\""));
                            break;
                        }
                        case eMonsterStats.NoBackground:
                        {
                            if (File.Exists(hudMonsterHealthHdDisabledJsonFilePath))
                            {
                                File.Move(hudMonsterHealthHdDisabledJsonFilePath, hudMonsterHealthHdJsonFilePath, true);
                            }

                            string content = await File.ReadAllTextAsync(hudMonsterHealthHdJsonFilePath);
                            await File.WriteAllTextAsync(hudMonsterHealthHdJsonFilePath, content.Replace("HB_L\"", "HB_L_Blank\""));
                            string content2 = await File.ReadAllTextAsync(hudMonsterHealthHdJsonFilePath);
                            await File.WriteAllTextAsync(hudMonsterHealthHdJsonFilePath, content2.Replace("HB_M\"", "HB_M_Blank\""));
                            string content3 = await File.ReadAllTextAsync(hudMonsterHealthHdJsonFilePath);
                            await File.WriteAllTextAsync(hudMonsterHealthHdJsonFilePath, content3.Replace("HB_R\"", "HB_R_Blank\""));
                            string content4 = await File.ReadAllTextAsync(hudMonsterHealthHdJsonFilePath);
                            await File.WriteAllTextAsync(hudMonsterHealthHdJsonFilePath, content4.Replace("HB_A\"", "HB_A_Blank\""));
                            break;
                        }
                    }

                    break;
                }
                case nameof(ShellViewModel.UserSettings.ItemIcons):
                {
                    eItemDisplay itemDisplay = (eItemDisplay)ShellViewModel.UserSettings.ItemIcons;

                    string itemNameJsonFilePath = Path.Combine(ShellViewModel.SelectedModDataFolder, "local/lng/strings/item-names.json");
                    string itemNameOriginalJsonFilePath = Path.Combine(ShellViewModel.SelectedModDataFolder, "local/lng/strings/item-names-original.json");
                    string itemRuneJsonFilePath = Path.Combine(ShellViewModel.SelectedModDataFolder, "local/lngstrings/item-runes.json");

                        switch (itemDisplay)
                    {
                        case eItemDisplay.NoIcons:
                        {
                            ItemIconsHide(itemNameJsonFilePath, itemNameOriginalJsonFilePath);
                            RuneIconsHide(itemRuneJsonFilePath);
                            break;
                        }
                        case eItemDisplay.ItemRuneIcons:
                        {
                            if (!File.Exists(itemNameJsonFilePath))
                            {
                                Helper.ExtractFileFromCasc(ShellViewModel.GamePath, @"data:data\local\lng\strings\item-names.json", ShellViewModel.BaseModsFolder, "data:data", "data");
                            }

                            if (!File.Exists(itemRuneJsonFilePath))
                            {
                                Helper.ExtractFileFromCasc(ShellViewModel.GamePath, @"data:data\local\lng\strings\item-runes.json", ShellViewModel.BaseModsFolder, "data:data", "data");
                            }

                            string namesFile = await File.ReadAllTextAsync(itemNameJsonFilePath);

                            if (namesFile.Contains("Chipped Emerald"))
                            {
                                await File.WriteAllTextAsync(itemNameOriginalJsonFilePath, namesFile);
                            }

                            ItemIconsShow(itemNameJsonFilePath);
                            RuneIconsShow(itemRuneJsonFilePath);
                            break;
                        }
                        case eItemDisplay.ItemIconsOnly:
                        {
                            if (!File.Exists(itemNameJsonFilePath))
                            {
                                Helper.ExtractFileFromCasc(ShellViewModel.GamePath, @"data:data\local\lng\strings\item-names.json", ShellViewModel.BaseModsFolder, "data:data", "data");
                            }

                            string namesFile = await File.ReadAllTextAsync(itemNameJsonFilePath);

                            if (namesFile.Contains("Chipped Emerald"))
                            {
                                await File.WriteAllTextAsync(itemNameOriginalJsonFilePath, namesFile);
                            }

                            ItemIconsShow(itemNameJsonFilePath);
                            RuneIconsHide(itemRuneJsonFilePath);
                            break;
                        }
                        case eItemDisplay.RuneIconsOnly:
                        {
                            if (!File.Exists(itemRuneJsonFilePath))
                            {
                                Helper.ExtractFileFromCasc(ShellViewModel.GamePath, @"data:data\local\lng\strings\item-runes.json", ShellViewModel.BaseModsFolder, "data:data", "data");
                            }

                            ItemIconsHide(itemNameJsonFilePath, itemNameOriginalJsonFilePath);
                            RuneIconsShow(itemRuneJsonFilePath);
                            break;
                        }
                    }

                    break;
                }
                case nameof(ShellViewModel.UserSettings.SuperTelekinesis):
                {
                    eEnabledDisabled superTelekinesis = (eEnabledDisabled)ShellViewModel.UserSettings.SuperTelekinesis;

                    switch (superTelekinesis)
                    {
                        case eEnabledDisabled.Disabled:
                        {
                            RemoveSuperTkSkill();
                            break;
                        }
                        case eEnabledDisabled.Enabled:
                        {
                            string charStatsPath = Path.Combine(Path.Combine(ShellViewModel.SelectedModDataFolder,"global/excel/charstats.txt"));
                            string itemTypesPath = Path.Combine(Path.Combine(ShellViewModel.SelectedModDataFolder, "global/excel/itemtypes.txt"));

                            if (File.Exists(charStatsPath) && File.Exists(itemTypesPath))
                            {
                                string[] charStatsLines = await File.ReadAllLinesAsync(charStatsPath);
                                string[] itemTypesLines = await File.ReadAllLinesAsync(itemTypesPath);

                                for (int i = 0; i < charStatsLines.Length; i++)
                                {
                                    string line = charStatsLines[i];
                                    string[] splitContent = line.Split('\t');

                                    if (i > 0 && i != 6)
                                    {
                                        splitContent[34] = "SuperTK";
                                        charStatsLines[i] = string.Join("\t", splitContent);
                                    }
                                }

                                for (int i = 0; i < itemTypesLines.Length; i++)
                                {
                                    string line = itemTypesLines[i];
                                    string[] splitContent = line.Split('\t');

                                    if (i == 14 || i == 21 || i == 60 || i == 76) splitContent[3] = "poti";
                                    if (i == 46 || i == 51) splitContent[2] = "gold";

                                    itemTypesLines[i] = string.Join("\t", splitContent);
                                }

                                // Write the modified content back to the files
                                File.WriteAllLines(charStatsPath, charStatsLines);
                                File.WriteAllLines(itemTypesPath, itemTypesLines);
                            }
                            CreateSuperTKSkill();
                            break;
                        }
                    }
                    break;
                }
                case nameof(ShellViewModel.UserSettings.RunewordSorting):
                {
                    eRunewordSorting runewordSorting = (eRunewordSorting)ShellViewModel.UserSettings.RunewordSorting;

                    string abRunewordJsonFilePath = Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLaunch/Runeword Sort/runewords-ab.json");
                    string itRunewordJsonFilePath = Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLaunch/Runeword Sort/runewords-it.json");
                    string lvRunewordJsonFilePath = Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLaunch/Runeword Sort//runewords-lv.json");

                    string abHelpPandelHdJsonFilePath = Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLaunch/Runeword Sort/helppanelhd-ab.json");
                    string itHelpPandelHdJsonFilePath = Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLaunch/Runeword Sort/helppanelhd-it.json");
                    string lvHelpPandelHdJsonFilePath = Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLaunch/Runeword Sort/helppanelhd-lv.json");

                    string helpPandelHdJsonFilePath = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/ui/layouts/helppanelhd.json");

                    switch (runewordSorting)
                    {
                        case eRunewordSorting.ByName:
                        {
                            if (ShellViewModel.ModInfo.Name == "ReMoDDeD")
                            {
                                File.Copy(abHelpPandelHdJsonFilePath, helpPandelHdJsonFilePath, true);
                                File.Copy(abRunewordJsonFilePath, Path.Combine(ShellViewModel.SelectedModDataFolder, $"global/ui/layouts/cuberecipes{6}panelhd.json"), true);
                            }
                            else
                            {
                                File.Copy(abRunewordJsonFilePath, Path.Combine(ShellViewModel.SelectedModDataFolder, $"global/ui/layouts/cuberecipes{5}panelhd.json"), true);
                            }

                            break;
                        }
                        case eRunewordSorting.ByItemtype:
                        {
                            if (ShellViewModel.ModInfo.Name == "ReMoDDeD")
                            {
                                File.Copy(itHelpPandelHdJsonFilePath, helpPandelHdJsonFilePath, true);
                                File.Copy(itRunewordJsonFilePath, Path.Combine(ShellViewModel.SelectedModDataFolder, $"global/ui/layouts/cuberecipes{6}panelhd.json"), true);
                            }
                            else
                            {
                                File.Copy(itRunewordJsonFilePath, Path.Combine(ShellViewModel.SelectedModDataFolder, $"global/ui/layouts/cuberecipes{5}panelhd.json"), true);
                            }

                            break;
                        }
                        case eRunewordSorting.ByReqLevel:
                        {
                            if (ShellViewModel.ModInfo.Name == "ReMoDDeD")
                            {
                                File.Copy(lvHelpPandelHdJsonFilePath, helpPandelHdJsonFilePath, true);
                                File.Copy(lvRunewordJsonFilePath, Path.Combine(ShellViewModel.SelectedModDataFolder, $"global/ui/layouts/cuberecipes{6}panelhd.json"), true);
                            }
                            else
                            {
                                File.Copy(lvRunewordJsonFilePath, Path.Combine(ShellViewModel.SelectedModDataFolder, $"global/ui/layouts/cuberecipes{5}panelhd.json"), true);
                            }

                            break;
                        }
                    }
                    break;
                }
                case nameof(ShellViewModel.UserSettings.HudDesign):
                {
                    eHudDesign hudDesign = (eHudDesign) ShellViewModel.UserSettings.HudDesign;


                    string mergedHudDirectory = Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLaunch/Merged HUD");
                    string layoutFolder = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/ui/layouts");
                    string hudPanelhdJsonFilePath = Path.Combine(layoutFolder, "hudpanelhd.json");
                    string skillSelecthdJsonFilePath = Path.Combine(layoutFolder, "skillselecthd.json");
                    string controllerDirectory = Path.Combine(layoutFolder, "controller");

                    if (Directory.Exists(mergedHudDirectory))
                    {

                        if (!File.Exists(layoutFolder))
                        {
                            Directory.CreateDirectory(layoutFolder);
                        }

                        switch (hudDesign)
                        {
                            case eHudDesign.Standard:
                            {
                                if (File.Exists(hudPanelhdJsonFilePath))
                                {
                                    File.Delete(hudPanelhdJsonFilePath);

                                    if ((eUiThemes) ShellViewModel.UserSettings.UiTheme == eUiThemes.Standard)
                                    {

                                        File.Copy(Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLaunch/UI Theme/expanded/layouts/hudpanelhd.json"), hudPanelhdJsonFilePath);
                                    }
                                    else
                                    {
                                        File.Copy(Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLaunch/UI Theme/remodded/layouts/hudpanelhd.json"), hudPanelhdJsonFilePath);
                                    }

                                    // Update skillselecthd.json if it exists
                                    if (File.Exists(skillSelecthdJsonFilePath))
                                    {
                                        string skillSelect = await File.ReadAllTextAsync(skillSelecthdJsonFilePath);
                                        await File.WriteAllTextAsync(skillSelecthdJsonFilePath, skillSelect.Replace("\"centerMirrorGapWidth\": 846,", "\"centerMirrorGapWidth\": 146,"));
                                    }
                                }
                                break;
                            }
                            case eHudDesign.Merged:
                            {
                                if (!Directory.Exists(controllerDirectory))
                                {
                                    Directory.CreateDirectory(controllerDirectory);
                                }
                                File.Copy(Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLaunch/Merged HUD/hudpanelhd-merged.json"), hudPanelhdJsonFilePath, true);
                                File.Copy(Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLaunch/Merged HUD/Controller/hudpanelhd-merged_controller.json"), hudPanelhdJsonFilePath, true);

                                // Update skillselecthd.json if it exists
                                if (!File.Exists(skillSelecthdJsonFilePath))
                                {
                                    File.Create(skillSelecthdJsonFilePath).Close();
                                    await File.WriteAllBytesAsync(skillSelecthdJsonFilePath, await Helper.GetResourceByteArray("Options.MergedHUD.skillselecthd.json"));
                                }
                                string skillSelect = File.ReadAllText(skillSelecthdJsonFilePath);
                                await File.WriteAllTextAsync(skillSelecthdJsonFilePath, skillSelect.Replace("\"centerMirrorGapWidth\": 146,", "\"centerMirrorGapWidth\": 846,"));
                                break;
                            }
                        }
                    }
                    //TODO: Add warning.
                    break;
                }
            }
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

    private void ItemIconsHide(string itemNameOriginalJsonFilePath, string itemNameJsonFilePath)
    {

        if (File.Exists(itemNameOriginalJsonFilePath))
        {
            string namesFile = File.ReadAllText(itemNameOriginalJsonFilePath);
            File.WriteAllText(itemNameJsonFilePath, namesFile);
        }
    }

    private void RuneIconsHide(string itemRuneJsonFilePath)
    {

        if (File.Exists(itemRuneJsonFilePath))
        {
            string itemRunes = File.ReadAllText(itemRuneJsonFilePath);

            // Reverse the replacements for Runes
            itemRunes = itemRunes.Replace("\"⅐ Elÿc0\"", "\"El Rune\"").Replace("\"⅑ Eldÿc0\"", "\"Eld Rune\"").Replace("\"⅒ Tirÿc0\"", "\"Tir Rune\"");
            itemRunes = itemRunes.Replace("\"⅓ Nefÿc0\"", "\"Nef Rune\"").Replace("\"⅔ Ethÿc0\"", "\"Eth Rune\"").Replace("\"⅕ Ithÿc0\"", "\"Ith Rune\"");
            itemRunes = itemRunes.Replace("\"⅖ Talÿc0\"", "\"Tal Rune\"").Replace("\"⅗ Ralÿc0\"", "\"Ral Rune\"").Replace("\"⅘ Ortÿc0\"", "\"Ort Rune\"");
            itemRunes = itemRunes.Replace("\"⅙ Thulÿc0\"", "\"Thul Rune\"").Replace("\"⅚ Amnÿc0\"", "\"Amn Rune\"").Replace("\"⅛ Solÿc0\"", "\"Sol Rune\"");
            itemRunes = itemRunes.Replace("\"⅜ Shaelÿc0\"", "\"Shael Rune\"").Replace("\"⅝ Dolÿc0\"", "\"Dol Rune\"").Replace("\"⅞ Helÿc0\"", "\"Hel Rune\"");
            itemRunes = itemRunes.Replace("\"⅟ Ioÿc0\"", "\"Io Rune\"").Replace("\"Ⅰ Lumÿc0\"", "\"Lum Rune\"").Replace("\"Ⅱ Koÿc0\"", "\"Ko Rune\"");
            itemRunes = itemRunes.Replace("\"Ⅲ Falÿc0\"", "\"Fal Rune\"").Replace("\"Ⅳ Lemÿc0\"", "\"Lem Rune\"").Replace("\"Ⅴ Pulÿc0\"", "\"Pul Rune\"");
            itemRunes = itemRunes.Replace("\"Ⅵ Umÿc0\"", "\"Um Rune\"").Replace("\"Ⅶ Malÿc0\"", "\"Mal Rune\"").Replace("\"Ⅷ Istÿc0\"", "\"Ist Rune\"");
            itemRunes = itemRunes.Replace("\"Ⅸ Gulÿc0\"", "\"Gul Rune\"").Replace("\"Ⅹ Vexÿc0\"", "\"Vex Rune\"").Replace("\"Ⅺ Ohmÿc0\"", "\"Ohm Rune\"");
            itemRunes = itemRunes.Replace("\"Ⅻ Loÿc0\"", "\"Lo Rune\"").Replace("\"Ⅼ Surÿc0\"", "\"Sur Rune\"").Replace("\"Ⅽ Berÿc0\"", "\"Ber Rune\"");
            itemRunes = itemRunes.Replace("\"Ⅾ Jahÿc0\"", "\"Jah Rune\"").Replace("\"Ⅿ Chamÿc0\"", "\"Cham Rune\"").Replace("\"ⅰ Zodÿc0\"", "\"Zod Rune\"");
            File.WriteAllText(itemRuneJsonFilePath, itemRunes);
        }
    }

    private void ItemIconsShow(string itemNameOriginalJsonFilePath)
    {
        string itemNames = File.ReadAllText(itemNameOriginalJsonFilePath);

        if (ShellViewModel.ModInfo.Name == "ReMoDDeD" || ShellViewModel.ModInfo.Name == "Vanilla++")
        {
            //Replace Potions, Scrolls and Keys
            itemNames = itemNames.Replace("\"Minor Healing Potion\"", "\"ÿc1 ³ ÿc0\"").Replace("\"Light Healing Potion\"", "\"ÿc1 ³ ÿc0\"").Replace("\"Healing Potion\"", "\"ÿc1 ³ ÿc0\"").Replace("\"Greater Healing Potion\"", "\"ÿc1¸ ÿc0\"").Replace("\"Super Healing Potion\"", "\"ÿc1¸ ÿc0\"");
            itemNames = itemNames.Replace("\"Minor Mana Potion\"", "\"ÿc3 ³ ÿc0\"").Replace("\"Light Mana Potion\"", "\"ÿc3 ³ ÿc0\"").Replace("\"Mana Potion\"", "\"ÿc3 ³ ÿc0\"").Replace("\"Greater Mana Potion\"", "\"ÿc3¸ ÿc0\"").Replace("\"Super Mana Potion\"", "\"ÿc3¸ ÿc0\"");
            itemNames = itemNames.Replace("\"Rejuvenation Potion\"", "\"ÿc; ³ ÿc0\"").Replace("\"Full Rejuvenation Potion\"", "\"ÿc; ¸ ÿc0\"").Replace("\"Antidote Potion\"", "\"ÿc5 ³ ÿc0\"").Replace("\"Thawing Potion\"", "\"ÿc9 ³ ÿc0\"").Replace("\"Stamina Potion\"", "\"ÿc0 ³ ÿc0\"");
            itemNames = itemNames.Replace("\"Scroll of Town Portal\"", "\"ÿc3 ¯ ÿc0\"").Replace("\"Scroll of Identify\"", "\"ÿc1 ¯ ÿc0\"").Replace("\"enUS\": \"Key\"", "\"enUS\": \"ÿc4 ±ÿc0\"");
        }
        else
        {
            //Replace Potions, Scrolls and Keys
            itemNames = itemNames.Replace("\"Minor Healing Potion\"", "\"ÿc1 © ÿc0\"").Replace("\"Light Healing Potion\"", "\"ÿc1 ª ÿc0\"").Replace("\"Healing Potion\"", "\"ÿc1 « ÿc0\"").Replace("\"Greater Healing Potion\"", "\"ÿc1 ¬ ÿc0\"").Replace("\"Super Healing Potion\"", "\"ÿc1 ® ÿc0\"");
            itemNames = itemNames.Replace("\"Minor Mana Potion\"", "\"ÿc3 © ÿc0\"").Replace("\"Light Mana Potion\"", "\"ÿc3 ª ÿc0\"").Replace("\"Mana Potion\"", "\"ÿc3 « ÿc0\"").Replace("\"Greater Mana Potion\"", "\"ÿc3 ¬ ÿc0\"").Replace("\"Super Mana Potion\"", "\"ÿc3 ® ÿc0\"");
            itemNames = itemNames.Replace("\"Rejuvenation Potion\"", "\"ÿc; ³ ÿc0\"").Replace("\"Full Rejuvenation Potion\"", "\"ÿc; ¸ ÿc0\"").Replace("\"Antidote Potion\"", "\"ÿc5 ³ ÿc0\"").Replace("\"Thawing Potion\"", "\"ÿc9 ³ ÿc0\"").Replace("\"Stamina Potion\"", "\"ÿc0 ³ ÿc0\"");
            itemNames = itemNames.Replace("\"Scroll of Town Portal\"", "\"ÿc3 ¯ ÿc0\"").Replace("\"Scroll of Identify\"", "\"ÿc1 ¯ ÿc0\"").Replace("\"enUS\": \"Key\"", "\"enUS\": \"ÿc4 ±ÿc0\"");
        }

        //Replace Gems
        itemNames = itemNames.Replace("\"Chipped Amethyst\"", "\"ÿc;¶ ÿc0\"").Replace("\"Flawed Amethyst\"", "\"ÿc;¶ ÿc0\"").Replace("\"Amethyst\"", "\"ÿc;¶ ÿc0\"").Replace("\"Flawless Amethyst\"", "\"ÿc;¶ ÿc0\"").Replace("\"Perfect Amethyst\"", "\"ÿc;¶ ÿc0\"");
        itemNames = itemNames.Replace("\"Chipped Topaz\"", "\"ÿc9¶ ÿc0\"").Replace("\"Flawed Topaz\"", "\"ÿc9¶ ÿc0\"").Replace("\"Topaz\"", "\"ÿc9¶ ÿc0\"").Replace("\"Flawless Topaz\"", "\"ÿc9¶ ÿc0\"").Replace("\"Perfect Topaz\"", "\"ÿc9¶ ÿc0\"");
        itemNames = itemNames.Replace("\"Chipped Sapphire\"", "\"ÿc3¶ ÿc0\"").Replace("\"Flawed Sapphire\"", "\"ÿc3¶ ÿc0\"").Replace("\"Sapphire\"", "\"ÿc3¶ ÿc0\"").Replace("\"Flawless Sapphire\"", "\"ÿc3¶ ÿc0\"").Replace("\"Perfect Sapphire\"", "\"ÿc3¶ ÿc0\"");
        itemNames = itemNames.Replace("\"Chipped Emerald\"", "\"ÿc2¶ ÿc0\"").Replace("\"Flawed Emerald\"", "\"ÿc2¶ ÿc0\"").Replace("\"Emerald\"", "\"ÿc2¶ ÿc0\"").Replace("\"Flawless Emerald\"", "\"ÿc2¶ ÿc0\"").Replace("\"Perfect Emerald\"", "\"ÿc2¶ ÿc0\"");
        itemNames = itemNames.Replace("\"Chipped Ruby\"", "\"ÿc1¶ ÿc0\"").Replace("\"Flawed Ruby\"", "\"ÿc1¶ ÿc0\"").Replace("\"Ruby\"", "\"ÿc1¶ ÿc0\"").Replace("\"Flawless Ruby\"", "\"ÿc1¶ ÿc0\"").Replace("\"Perfect Ruby\"", "\"ÿc1¶ ÿc0\"");
        itemNames = itemNames.Replace("\"Chipped Diamond\"", "\"¶ \"").Replace("\"Flawed Diamond\"", "\"¶ \"").Replace("\"Diamond\"", "\"¶ \"").Replace("\"Flawless Diamond\"", "\"¶ \"").Replace("\"Perfect Diamond\"", "\"¶ \"");
        itemNames = itemNames.Replace("\"Chipped Skull\"", "\"ÿc0 ¹ ÿc0\"").Replace("\"Flawed Skull\"", "\"ÿc0 ¹ ÿc0\"").Replace("\"Skull\"", "\"ÿc0 ¹ ÿc0\"").Replace("\"Flawless Skull\"", "\"ÿc0 ¹ ÿc0\"").Replace("\"Perfect Skull\"", "\"ÿc0 ¹ ÿc0\"");

        File.WriteAllText(itemNameOriginalJsonFilePath, itemNames);
    }

    private void RuneIconsShow(string itemRuneJsonFilePath)
    {
        string itemRunes = File.ReadAllText(itemRuneJsonFilePath);

        //Replace Runes
        itemRunes = itemRunes.Replace("\"El Rune\"", "\"⅐ Elÿc0\"").Replace("\"Eld Rune\"", "\"⅑ Eldÿc0\"").Replace("\"Tir Rune\"", "\"⅒ Tirÿc0\"");
        itemRunes = itemRunes.Replace("\"Nef Rune\"", "\"⅓ Nefÿc0\"").Replace("\"Eth Rune\"", "\"⅔ Ethÿc0\"").Replace("\"Ith Rune\"", "\"⅕ Ithÿc0\"");
        itemRunes = itemRunes.Replace("\"Tal Rune\"", "\"⅖ Talÿc0\"").Replace("\"Ral Rune\"", "\"⅗ Ralÿc0\"").Replace("\"Ort Rune\"", "\"⅘ Ortÿc0\"");
        itemRunes = itemRunes.Replace("\"Thul Rune\"", "\"⅙ Thulÿc0\"").Replace("\"Amn Rune\"", "\"⅚ Amnÿc0\"").Replace("\"Sol Rune\"", "\"⅛ Solÿc0\"");
        itemRunes = itemRunes.Replace("\"Shael Rune\"", "\"⅜ Shaelÿc0\"").Replace("\"Dol Rune\"", "\"⅝ Dolÿc0\"").Replace("\"Hel Rune\"", "\"⅞ Helÿc0\"");
        itemRunes = itemRunes.Replace("\"Io Rune\"", "\"⅟ Ioÿc0\"").Replace("\"Lum Rune\"", "\"Ⅰ Lumÿc0\"").Replace("\"Ko Rune\"", "\"Ⅱ Koÿc0\"");
        itemRunes = itemRunes.Replace("\"Fal Rune\"", "\"Ⅲ Falÿc0\"").Replace("\"Lem Rune\"", "\"Ⅳ Lemÿc0\"").Replace("\"Pul Rune\"", "\"Ⅴ Pulÿc0\"");
        itemRunes = itemRunes.Replace("\"Um Rune\"", "\"Ⅵ Umÿc0\"").Replace("\"Mal Rune\"", "\"Ⅶ Malÿc0\"").Replace("\"Ist Rune\"", "\"Ⅷ Istÿc0\"");
        itemRunes = itemRunes.Replace("\"Gul Rune\"", "\"Ⅸ Gulÿc0\"").Replace("\"Vex Rune\"", "\"Ⅹ Vexÿc0\"").Replace("\"Ohm Rune\"", "\"Ⅺ Ohmÿc0\"");
        itemRunes = itemRunes.Replace("\"Lo Rune\"", "\"Ⅻ Loÿc0\"").Replace("\"Sur Rune\"", "\"Ⅼ Surÿc0\"").Replace("\"Ber Rune\"", "\"Ⅽ Berÿc0\"");
        itemRunes = itemRunes.Replace("\"Jah Rune\"", "\"Ⅾ Jahÿc0\"").Replace("\"Cham Rune\"", "\"Ⅿ Chamÿc0\"").Replace("\"Zod Rune\"", "\"ⅰ Zodÿc0\"");

        File.WriteAllText(itemRuneJsonFilePath, itemRunes);
    }

    private void RemoveSuperTkSkill()
    {
        string skillTextPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/excel/skills.txt");

        if (File.Exists(skillTextPath))
        {
            string originalSkillTextPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLaunch/Originals/skills-original.txt");

            //Remove SuperTK from charstats and itemtypes
            if (File.Exists(originalSkillTextPath))
            {
                File.Copy(originalSkillTextPath, skillTextPath, true);
            }

            string charStatsPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/excel/charstats.txt");
            string itemTypesPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/excel/itemtypes.txt");

            if (File.Exists(charStatsPath) && File.Exists(itemTypesPath))
            {
                string[] charStatsLines = File.ReadAllLines(charStatsPath);
                string[] itemTypesLines = File.ReadAllLines(itemTypesPath);

                for (int i = 0; i < charStatsLines.Length; i++)
                {
                    string line = charStatsLines[i];
                    string[] splitContent = line.Split('\t');

                    if (i > 0 && i != 6)
                    {
                        splitContent[34] = "";
                        charStatsLines[i] = string.Join("\t", splitContent);
                    }
                }

                for (int i = 0; i < itemTypesLines.Length; i++)
                {
                    string line = itemTypesLines[i];
                    string[] splitContent = line.Split('\t');

                    if (i == 14 || i == 21 || i == 60 || i == 76)
                        splitContent[3] = "";
                    if (i == 46 || i == 51)
                        splitContent[2] = "";

                    itemTypesLines[i] = string.Join("\t", splitContent);
                }

                // Write the modified content back to the files
                File.WriteAllLines(charStatsPath, charStatsLines);
                File.WriteAllLines(itemTypesPath, itemTypesLines);
            }

            //Remove SuperTK from skills
            bool superTKExists = false;
            List<string> lines = new List<string>();

            //Check the last entry in file for the SuperTK skill; if it exists, flag it
            using (StreamReader reader = new StreamReader(skillTextPath))
            {
                while (reader.ReadLine() is { } line)
                {
                    string[] columns = line.Split('\t');
                    if (columns.Length > 0 && columns[0] == "SuperTK")
                        superTKExists = true;
                    else
                        lines.Add(line);
                }
            }

            //Check for flag and write the modified content back to the file
            if (superTKExists)
            {
                using StreamWriter writer = new StreamWriter(skillTextPath, false);

                foreach (string line in lines)
                {
                    writer.WriteLine(line);
                }
            }
        }

    }

    private void CreateSuperTKSkill()
    {
        string skillTextPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/excel/skills.txt");
        string itemTypesTextPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/excel/itemtypes.txt");
        string charStatsPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/excel/charstats.txt");
        string originalSkillTextPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLaunch/Originals/skills-original.txt");
        string originalsDirectoryPath = Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLaunch/Originals");

        //Create needed folders and files
        if (!File.Exists(itemTypesTextPath))
        {
            Helper.ExtractFileFromCasc(ShellViewModel.GamePath, @"data:data\global\excel\itemtypes.txt", ShellViewModel.BaseModsFolder, "data:data", "data");
        }
        if (!File.Exists(charStatsPath))
        {
            Helper.ExtractFileFromCasc(ShellViewModel.GamePath, @"data:data\global\excel\charstats.txt", ShellViewModel.BaseModsFolder, "data:data", "data");
        }
        if (!Directory.Exists(originalsDirectoryPath))
        {
            Directory.CreateDirectory(originalsDirectoryPath);
        }
        if (!File.Exists(skillTextPath))
        {
            Helper.ExtractFileFromCasc(ShellViewModel.GamePath, @"data:data\global\excel\skills.txt", ShellViewModel.BaseModsFolder, "data:data", "data");
        }
        File.Copy(skillTextPath, originalSkillTextPath, true);

        //Check to see if we already added the skill previously
        bool superTKExists = false;
        using (StreamReader reader = new StreamReader(skillTextPath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] columns = line.Split('\t');
                if (columns.Length > 0 && columns[0] == "SuperTK")
                {
                    superTKExists = true;
                    break;
                }
            }
        }

        if (!superTKExists)
        {
            //Skill doesn't exist yet; let's create it
            int tkKSkill = 44; //ID of TK Skill
            string[] lines = File.ReadAllLines(skillTextPath);
            int lineCount = 0; //track ID we added skill to

            // Check if the specified line number is valid
            if (tkKSkill < lines.Length)
            {
                string lineToCopy = lines[tkKSkill];
                lineCount = lines.Length;
                Array.Resize(ref lines, lineCount + 1);
                lines[lineCount] = lineToCopy;
                File.WriteAllLines(skillTextPath, lines);
            }

            //TK has been cloned now; let's edit it
            string[] linesS = File.ReadAllLines(skillTextPath);
            string outstr = "";
            string sep = "\t";
            int index = 0;

            foreach (string line in linesS)
            {
                string[] splitContent = line.Split(sep.ToCharArray());

                if (index == lineCount)
                {
                    splitContent[0] = "SuperTK"; //Change Skill Name
                    splitContent[1] = (lineCount - 2).ToString(); //Update Comment ID
                    splitContent[2] = ""; //Remove sorceress-type skill
                    splitContent[189] = ""; //Remove mana requirement
                    splitContent[214] = "50"; //Increase Range
                    splitContent[216] = "0"; //Remove Knockback Chance
                    splitContent[244] = "13"; //Remove Knockback Layer
                    for (int i = 261; i <= 273; i++)
                    {
                        splitContent[i] = "";
                    }

                    outstr += String.Join("\t", splitContent) + "\n";
                }
                else
                {
                    outstr += line + "\n";
                }
                index += 1;
            }
            File.WriteAllText(skillTextPath, outstr);
        }
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