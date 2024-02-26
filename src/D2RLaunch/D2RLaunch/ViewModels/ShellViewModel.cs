using System.ComponentModel.Composition;
using System.Windows.Controls;
using Caliburn.Micro;
using ILog = log4net.ILog;
using log4net;
using LogManager = log4net.LogManager;
using D2RLaunch.Views;
using Syncfusion.UI.Xaml.NavigationDrawer;
using System.Threading;
using System.Windows;
using System;
using System.Threading.Tasks;
using D2RLaunch.ViewModels.Drawers;
using D2RLaunch.Views.Drawers;
using JetBrains.Annotations;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using D2RLaunch.Properties;
using D2RLaunch.Models.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Syncfusion.Licensing;
using D2RLaunch.Culture;
using D2RLaunch.Models;
using Newtonsoft.Json;
using System.Windows.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Resources;
using WinForms = System.Windows.Forms;

namespace D2RLaunch.ViewModels;

public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
{
    #region members

    private ILog _logger = LogManager.GetLogger(typeof(ShellViewModel));
    private UserControl _userControl;
    private IWindowManager _windowManager;
    private string _title = "D2RLaunch";
    private string appVersion = "2.0.3";
    private string _gamePath;
    private bool _diabloInstallDetected;
    private bool _customizationsEnabled;
    private bool _wikiEnabled = true;
    private ModInfo _modInfo;
    private UserSettings _userSettings;
    private string _modLogo = "pack://application:,,,/Resources/Images/D2RL_Logo.png";
    private DispatcherTimer _autoBackupDispatcherTimer;
    private bool _skillIconPackEnabled = true;
    private bool _skillBuffIconsEnabled = true;
    private bool _showItemLevelsEnabled = true;
    private bool _superTelekinesisEnabled = true;
    private bool _itemIconDisplayEnabled;
    private bool _launcherHasUpdate;
    private string _launcherUpdateString = "D2RLaunch Update Ready!";
    private const string TAB_BYTE_CODE = "55AA55AA0000000061000000000000004400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000004A4D0000";

    #endregion

    public ShellViewModel()
    {
        if (Execute.InDesignMode)
        {
            ModLogo = "pack://application:,,,/Resources/Images/D2RL_Logo.png";
            Title = "D2RLaunch";
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

    public string LauncherUpdateString
    {
        get => _launcherUpdateString;
        set
        {
            if (value == _launcherUpdateString)
            {
                return;
            }
            _launcherUpdateString = value;
            NotifyOfPropertyChange();
        }
    }

    public bool LauncherHasUpdate
    {
        get => _launcherHasUpdate;
        set
        {
            if (value == _launcherHasUpdate)
            {
                return;
            }
            _launcherHasUpdate = value;
            NotifyOfPropertyChange();
        }
    }

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

    public bool ItemIconDisplayEnabled
    {
        get => _itemIconDisplayEnabled;
        set
        {
            if (value == _itemIconDisplayEnabled) return;
            _itemIconDisplayEnabled = value;
            NotifyOfPropertyChange();
        }
    }

    public bool SuperTelekinesisEnabled
    {
        get => _superTelekinesisEnabled;
        set
        {
            if (value == _superTelekinesisEnabled) return;
            _superTelekinesisEnabled = value;
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
        get => _title + " v" + appVersion;
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

    public async Task ApplyModSettings()
    {
        await StartAutoBackup();
        await ConfigureBuffIcons();
        await ConfigureSkillIcons();
        await ConfigureMercIcons();
        await ConfigureItemILvls();
        await ConfigureRuneDisplay();
        await ConfigureHideHelmets();
        await ConfigureMonsterStatsDisplay();
        await ConfigureItemIcons();
        await ConfigureSuperTelekinesis();
        await ConfigureRunewordSorting();
        await ConfigureHudDesign();
    }

    private async Task ConfigureHudDesign() //Merged HUD - QoL
    {
        eHudDesign hudDesign = (eHudDesign)UserSettings.HudDesign;


        string mergedHudDirectory = Path.Combine(SelectedModDataFolder, "D2RLaunch/Merged HUD");
        string layoutFolder = Path.Combine(SelectedModDataFolder, "global/ui/layouts");
        string hudPanelhdJsonFilePath = Path.Combine(layoutFolder, "hudpanelhd.json");
        string controllerhudPanelhdJsonFilePath = Path.Combine(layoutFolder, "controller/hudpanelhd.json");
        string skillSelecthdJsonFilePath = Path.Combine(layoutFolder, "skillselecthd.json");
        string controllerDirectory = Path.Combine(layoutFolder, "controller");

        if (Directory.Exists(mergedHudDirectory))
        {

            if (!File.Exists(layoutFolder))
                Directory.CreateDirectory(layoutFolder);

            switch (hudDesign)
            {
                case eHudDesign.Standard:
                    {
                        if (File.Exists(hudPanelhdJsonFilePath))
                        {
                            File.Delete(hudPanelhdJsonFilePath);

                            if ((eUiThemes)UserSettings.UiTheme == eUiThemes.Standard)
                                File.Copy(Path.Combine(SelectedModDataFolder, "D2RLaunch/UI Theme/expanded/layouts/hudpanelhd.json"), hudPanelhdJsonFilePath);
                            else
                                File.Copy(Path.Combine(SelectedModDataFolder, "D2RLaunch/UI Theme/remodded/layouts/hudpanelhd.json"), hudPanelhdJsonFilePath);

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
                            Directory.CreateDirectory(controllerDirectory);

                        File.Copy(Path.Combine(SelectedModDataFolder, "D2RLaunch/Merged HUD/hudpanelhd-merged.json"), hudPanelhdJsonFilePath, true);
                        File.Copy(Path.Combine(SelectedModDataFolder, "D2RLaunch/Merged HUD/Controller/hudpanelhd-merged_controller.json"), controllerhudPanelhdJsonFilePath, true);

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
    }

    private async Task ConfigureRunewordSorting()
    {
        eRunewordSorting runewordSorting = (eRunewordSorting)UserSettings.RunewordSorting;

        string abRunewordJsonFilePath = Path.Combine(SelectedModDataFolder, "D2RLaunch/Runeword Sort/runewords-ab.json");
        string itRunewordJsonFilePath = Path.Combine(SelectedModDataFolder, "D2RLaunch/Runeword Sort/runewords-it.json");
        string lvRunewordJsonFilePath = Path.Combine(SelectedModDataFolder, "D2RLaunch/Runeword Sort//runewords-lv.json");

        string abHelpPandelHdJsonFilePath = Path.Combine(SelectedModDataFolder, "D2RLaunch/Runeword Sort/helppanelhd-ab.json");
        string itHelpPandelHdJsonFilePath = Path.Combine(SelectedModDataFolder, "D2RLaunch/Runeword Sort/helppanelhd-it.json");
        string lvHelpPandelHdJsonFilePath = Path.Combine(SelectedModDataFolder, "D2RLaunch/Runeword Sort/helppanelhd-lv.json");

        string helpPandelHdJsonFilePath = Path.Combine(SelectedModDataFolder, "global/ui/layouts/helppanelhd.json");

        switch (runewordSorting)
        {
            case eRunewordSorting.ByName:
                {
                    if (ModInfo.Name == "ReMoDDeD")
                    {
                        File.Copy(abHelpPandelHdJsonFilePath, helpPandelHdJsonFilePath, true);
                        File.Copy(abRunewordJsonFilePath, Path.Combine(SelectedModDataFolder, $"global/ui/layouts/cuberecipes{6}panelhd.json"), true);
                    }
                    else
                    {
                        if (File.Exists(Path.Combine(SelectedModDataFolder, $"global/ui/layouts/cuberecipes{5}panelhd.json")))
                            File.Copy(abRunewordJsonFilePath, Path.Combine(SelectedModDataFolder, $"global/ui/layouts/cuberecipes{5}panelhd.json"), true);
                    }

                    break;
                }
            case eRunewordSorting.ByItemtype:
                {
                    if (ModInfo.Name == "ReMoDDeD")
                    {
                        File.Copy(itHelpPandelHdJsonFilePath, helpPandelHdJsonFilePath, true);
                        File.Copy(itRunewordJsonFilePath, Path.Combine(SelectedModDataFolder, $"global/ui/layouts/cuberecipes{6}panelhd.json"), true);
                    }
                    else
                        File.Copy(itRunewordJsonFilePath, Path.Combine(SelectedModDataFolder, $"global/ui/layouts/cuberecipes{5}panelhd.json"), true);

                    break;
                }
            case eRunewordSorting.ByReqLevel:
                {
                    if (ModInfo.Name == "ReMoDDeD")
                    {
                        File.Copy(lvHelpPandelHdJsonFilePath, helpPandelHdJsonFilePath, true);
                        File.Copy(lvRunewordJsonFilePath, Path.Combine(SelectedModDataFolder, $"global/ui/layouts/cuberecipes{6}panelhd.json"), true);
                    }
                    else
                        File.Copy(lvRunewordJsonFilePath, Path.Combine(SelectedModDataFolder, $"global/ui/layouts/cuberecipes{5}panelhd.json"), true);

                    break;
                }
        }
    }

    private async Task ConfigureSuperTelekinesis()
    {
        eEnabledDisabled superTelekinesis = (eEnabledDisabled)UserSettings.SuperTelekinesis;

        switch (superTelekinesis)
        {
            case eEnabledDisabled.Disabled:
            {
                RemoveSuperTkSkill();
                break;
            }
            case eEnabledDisabled.Enabled:
            {
                    CreateSuperTKSkill();
                string charStatsPath = Path.Combine(Path.Combine(SelectedModDataFolder, "global/excel/charstats.txt"));
                string itemTypesPath = Path.Combine(Path.Combine(SelectedModDataFolder, "global/excel/itemtypes.txt"));

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
                
                break;
            }
        }
    }

    private async Task ConfigureItemIcons()
    {
        eItemDisplay itemDisplay = (eItemDisplay)UserSettings.ItemIcons;

        string itemNameJsonFilePath = Path.Combine(SelectedModDataFolder, "local/lng/strings/item-names.json");
        string itemNameOriginalJsonFilePath = Path.Combine(SelectedModDataFolder, "local/lng/strings/item-names-original.json");
        string itemRuneJsonFilePath = Path.Combine(SelectedModDataFolder, "local/lng/strings/item-runes.json");

        switch (itemDisplay)
        {
            case eItemDisplay.NoIcons:
                {
                    ItemIconsHide(itemNameOriginalJsonFilePath, itemNameJsonFilePath);
                    RuneIconsHide(itemRuneJsonFilePath);
                    break;
                }
            case eItemDisplay.ItemRuneIcons:
                {
                    if (!Directory.Exists(SelectedModDataFolder + "hd/ui/fonts"))
                    {
                        string fontsFolder = Path.Combine(SelectedModDataFolder, "hd/ui/fonts");
                        byte[] font = await Helper.GetResourceByteArray($"Fonts.{UserSettings.Font}.otf");

                        if (!Directory.Exists(fontsFolder))
                        {
                            Directory.CreateDirectory(fontsFolder);
                            File.Create(Path.Combine(fontsFolder, "exocetblizzardot-medium.otf")).Close();
                        }

                        await File.WriteAllBytesAsync(Path.Combine(fontsFolder, "exocetblizzardot-medium.otf"), font);
                    }

                    if (!File.Exists(itemNameJsonFilePath))
                        Helper.ExtractFileFromCasc(GamePath, @"data:data\local\lng\strings\item-names.json", SelectedModDataFolder, "data:data");

                    if (!File.Exists(itemRuneJsonFilePath))
                        Helper.ExtractFileFromCasc(GamePath, @"data:data\local\lng\strings\item-runes.json", SelectedModDataFolder, "data:data");

                    string namesFile = await File.ReadAllTextAsync(itemNameJsonFilePath);

                    if (namesFile.Contains("Chipped Emerald"))
                        await File.WriteAllTextAsync(itemNameOriginalJsonFilePath, namesFile);

                    ItemIconsShow(itemNameJsonFilePath);
                    RuneIconsShow(itemRuneJsonFilePath);
                    break;
                }
            case eItemDisplay.ItemIconsOnly:
                {
                    if (!File.Exists(itemNameJsonFilePath))
                        Helper.ExtractFileFromCasc(GamePath, @"data:data\local\lng\strings\item-names.json", SelectedModDataFolder, "data:data");

                    string namesFile = await File.ReadAllTextAsync(itemNameJsonFilePath);

                    if (namesFile.Contains("Chipped Emerald"))
                        await File.WriteAllTextAsync(itemNameOriginalJsonFilePath, namesFile);

                    ItemIconsShow(itemNameJsonFilePath);
                    RuneIconsHide(itemRuneJsonFilePath);
                    break;
                }
            case eItemDisplay.RuneIconsOnly:
                {
                    if (!File.Exists(itemRuneJsonFilePath))
                        Helper.ExtractFileFromCasc(GamePath, @"data:data\local\lng\strings\item-runes.json", SelectedModDataFolder, "data:data");

                    ItemIconsHide(itemNameOriginalJsonFilePath, itemNameJsonFilePath);
                    RuneIconsShow(itemRuneJsonFilePath);
                    break;
                }
        }
    }

    private async Task ConfigureMonsterStatsDisplay()
    {
        eMonsterStats monsterStatsDisplay = (eMonsterStats)UserSettings.MonsterStatsDisplay;

        string uiLayoutsPath = Path.Combine(SelectedModDataFolder, "global/ui/layouts");
        string hudMonsterHealthHdJsonFilePath = Path.Combine(uiLayoutsPath, "hudmonsterhealthhd.json");
        string hudMonsterHealthHdDisabledJsonFilePath = Path.Combine(uiLayoutsPath, "hudmonsterhealthhd_disabled.json");
        string monsterStatsPath = Path.Combine(SelectedModDataFolder, "D2RLaunch/Monster Stats");

        if (!Directory.Exists(uiLayoutsPath))
            Directory.CreateDirectory(uiLayoutsPath);

        if (!Directory.Exists(monsterStatsPath))
            Directory.CreateDirectory(monsterStatsPath);

        if (!File.Exists(hudMonsterHealthHdJsonFilePath))
            await File.WriteAllBytesAsync(hudMonsterHealthHdJsonFilePath, await Helper.GetResourceByteArray("Options.MonsterStats.hudmonsterhealthhd.json"));

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
    }

    private async Task ConfigureHideHelmets()
    {
        eEnabledDisabled helmetDisplay = (eEnabledDisabled)UserSettings.HideHelmets;

        //Define filenames and paths
        string helmetBaseDir1 = Path.Combine(SelectedModDataFolder, "hd/items/armor/helmet");
        string helmetBaseDir2 = Path.Combine(SelectedModDataFolder, "hd/items/armor/circlet");
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
                            File.Delete(filename);
                    }

                    foreach (string filename in allHelmetFiles2)
                    {
                        if (File.Exists(filename))
                            File.Delete(filename);
                    }
                    break;
                }
            case eEnabledDisabled.Enabled:
                {
                    //Create directories if they don't exist
                    if (!Directory.Exists(helmetBaseDir1))
                        Directory.CreateDirectory(helmetBaseDir1);
                    if (!Directory.Exists(helmetBaseDir2))
                        Directory.CreateDirectory(helmetBaseDir2);

                    //Loop through both arrays to create files
                    foreach (string filename in allHelmetFiles1)
                    {
                        if (File.Exists(filename))
                            File.Delete(filename);
                        File.Create(filename).Close();
                        await File.WriteAllBytesAsync(filename, await Helper.GetResourceByteArray("Options.HideHelmets.hide_helmets.json"));
                    }

                    foreach (string filename in allHelmetFiles2)
                    {
                        if (File.Exists(filename))
                            File.Delete(filename);
                        File.Create(filename).Close();
                        await File.WriteAllBytesAsync(filename, await Helper.GetResourceByteArray("Options.HideHelmets.hide_helmets.json"));
                    }
                    break;
                }
        }
    }

    private async Task ConfigureRuneDisplay()
    {
        eEnabledDisabled runeDisplay = (eEnabledDisabled)UserSettings.RuneDisplay;

        //Define replacement strings
        string runePath = Path.Combine(SelectedModDataFolder, "hd/items/misc/rune");
        string runeStringPath = Path.Combine(SelectedModDataFolder, "local/lng/strings");
        string runeStringJsonFile = Path.Combine(SelectedModDataFolder, "local/lng/strings/item-runes.json");
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
                        Directory.CreateDirectory(runePath);

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
                        Directory.CreateDirectory(runeStringPath);
                    if (!File.Exists(runeStringJsonFile))
                        Helper.ExtractFileFromCasc(GamePath, cascItemRuneJsonFileName, SelectedModDataFolder, "data:data");

                    if (File.Exists(runeStringJsonFile))
                    {
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
                    }
                    
                    break;
                }
            case eEnabledDisabled.Enabled:
                {
                    string[] runeFiles1 = { "sol_rune.json", "shael_rune.json", "dol_rune.json", "hel_rune.json", "io_rune.json", "lum_rune.json", "ko_rune.json", "fal_rune.json", "lem_rune.json", "pul_rune.json", "um_rune.json" };
                    string[] runeFiles2 = { "mal_rune.json", "ist_rune.json", "gul_rune.json", "vex_rune.json", "ohm_rune.json", "lo_rune.json", "sur_rune.json", "ber_rune.json", "jah_rune.json", "cham_rune.json", "zod_rune.json" };

                    if (!Directory.Exists(runePath))
                        Directory.CreateDirectory(runePath);

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
                        Helper.ExtractFileFromCasc(GamePath, cascItemRuneJsonFileName, SelectedModDataFolder, "data:data");

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
    }

    private async Task ConfigureItemILvls()
    {
        if (ModInfo.Name == "ReMoDDeD")
            return;

        string excelPath = Path.Combine(SelectedModDataFolder, "global/excel");
        string armorTxtPath = Path.Combine(excelPath, "armor.txt");
        string weaponsTxtPath = Path.Combine(excelPath, "weapons.txt");
        string miscTxtPath = Path.Combine(excelPath, "misc.txt");

        string cascMiscTxtFileName = @"data:data\global\excel\misc.txt";
        string cascArmorTxtFileName = @"data:data\global\excel\armor.txt";
        string cascWeaponsTxtFileName = @"data:data\global\excel\weapons.txt";

        string[] files = new string[] { "armor.txt", "misc.txt", "weapons.txt" };


        eEnabledDisabled itemLvls = (eEnabledDisabled)UserSettings.ItemIlvls;

        switch (itemLvls)
        {
            case eEnabledDisabled.Disabled:
            {
                //TODO: This needs to undo the show itemlevels functionality.
                break;
            }
            case eEnabledDisabled.Enabled:
            {
                //search the defined files
                foreach (string file in files)
                {
                    if (!Directory.Exists(excelPath))
                        Directory.CreateDirectory(excelPath);

                    if (!File.Exists(armorTxtPath))
                    {
                        File.Create(armorTxtPath).Close();
                        Helper.ExtractFileFromCasc(GamePath, cascArmorTxtFileName, SelectedModDataFolder, "data:data");
                    }
                    if (!File.Exists(weaponsTxtPath))
                    {
                        File.Create(weaponsTxtPath).Close();
                        Helper.ExtractFileFromCasc(GamePath, cascWeaponsTxtFileName, SelectedModDataFolder, "data:data");
                    }
                    if (!File.Exists(miscTxtPath))
                    {
                        File.Create(miscTxtPath).Close();
                        Helper.ExtractFileFromCasc(GamePath, cascMiscTxtFileName, SelectedModDataFolder, "data:data");
                    }

                    string filePath = Path.Combine(excelPath, file);

                    if (!File.Exists(filePath))
                        continue;

                    string[] lines = await File.ReadAllLinesAsync(filePath);

                    if (lines.Length == 0)
                        continue;

                    string[] headers = lines[0].Split('\t'); //split by tab-delimited format
                    int showLevelIndex = Array.IndexOf(headers, "ShowLevel"); //make an array from the 'ShowLevel' entries

                    //search through 'ShowLevel' entries further
                    for (int i = 1; i < lines.Length; i++)
                    {
                        string[] columns = lines[i].Split('\t');
                        //check if entries match the dropdown index of 0 or 1
                        if (columns.Length > showLevelIndex && columns[showLevelIndex] != UserSettings.ItemIlvls.ToString())
                        {
                            columns[showLevelIndex] = UserSettings.ItemIlvls.ToString();
                            lines[i] = string.Join("\t", columns); //replace the 0 or 1 values as dropdown indicates
                        }
                    }
                    //We done boys
                    File.WriteAllLines(filePath, lines);
                }
                break;
            }
        }
    }

    private async Task ConfigureMercIcons()
    {
        eMercIdentifier mercIdentifier = (eMercIdentifier)UserSettings.MercIcons;

        string dataHdPath = Path.Combine(SelectedModDataFolder, "hd");
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
                        Directory.CreateDirectory(enemyPath);
                    if (!Directory.Exists(texturesPath))
                        Directory.CreateDirectory(texturesPath);
                    if (!Directory.Exists(particlesPath))
                        Directory.CreateDirectory(particlesPath);

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
                        Directory.CreateDirectory(enemyPath);
                    if (!Directory.Exists(texturesPath))
                        Directory.CreateDirectory(texturesPath);
                    if (!Directory.Exists(particlesPath))
                        Directory.CreateDirectory(particlesPath);

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
    }

    private async Task ConfigureSkillIcons()
    {
        eSkillIconPack skillIconPack = (eSkillIconPack)UserSettings.SkillIcons;

        string globalUiSpellPath = Path.Combine(SelectedModDataFolder, "hd/global/ui/spells");
        string amazonSkillIconsPath = Path.Combine(SelectedModDataFolder, "hd/global/ui/spells/amazon/amskillicon2.sprite");
        string profileHdJsonPath = Path.Combine(SelectedModDataFolder, "global/ui/layouts/_profilehd.json");
        string skillsTreePanelHdJsonPath = Path.Combine(SelectedModDataFolder, "global/ui/layouts/skillstreepanelhd.json");

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
                        Helper.ExtractFileFromCasc(GamePath, cascProfileHdJsonFileName, SelectedModDataFolder, "data:data");

                    if (!File.Exists(skillsTreePanelHdJsonPath))
                        Helper.ExtractFileFromCasc(GamePath, cascSkillsTreePanelHdJsonFileName, SelectedModDataFolder, "data:data");

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
                        Helper.ExtractFileFromCasc(GamePath, cascProfileHdJsonFileName, SelectedModDataFolder, "data:data");

                    if (!File.Exists(skillsTreePanelHdJsonPath))
                        Helper.ExtractFileFromCasc(GamePath, cascSkillsTreePanelHdJsonFileName, SelectedModDataFolder, "data:data");

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
    }

    private async Task ConfigureBuffIcons()
    {
        string buffIconsParticlesPath = Path.Combine(SelectedModDataFolder, "D2RLaunch/Buff Icons/Particles");
        string buffIconsParticlesDisabledPath = Path.Combine(SelectedModDataFolder, "D2RLaunch/Buff Icons/Particles (Disabled)");

        if ((eEnabledDisabled)UserSettings.BuffIcons == eEnabledDisabled.Disabled)
        {
            if (Directory.Exists(buffIconsParticlesPath))
                Directory.Move(buffIconsParticlesPath, buffIconsParticlesDisabledPath);
        }
        if ((eEnabledDisabled)UserSettings.BuffIcons == eEnabledDisabled.Enabled)
        {
            if (Directory.Exists(buffIconsParticlesDisabledPath))
                Directory.Move(buffIconsParticlesDisabledPath, buffIconsParticlesPath);
        }
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

        if (ModInfo.Name == "ReMoDDeD" || ModInfo.Name == "Vanilla++")
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
        string skillTextPath = Path.Combine(SelectedModDataFolder, "global/excel/skills.txt");

        if (File.Exists(skillTextPath))
        {
            string originalSkillTextPath = Path.Combine(SelectedModDataFolder, "D2RLaunch/Originals/skills-original.txt");

            //Remove SuperTK from charstats and itemtypes
            if (File.Exists(originalSkillTextPath))
                File.Copy(originalSkillTextPath, skillTextPath, true);

            string charStatsPath = Path.Combine(SelectedModDataFolder, "global/excel/charstats.txt");
            string itemTypesPath = Path.Combine(SelectedModDataFolder, "global/excel/itemtypes.txt");

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
        string skillTextPath = Path.Combine(SelectedModDataFolder, "global/excel/skills.txt");
        string itemTypesTextPath = Path.Combine(SelectedModDataFolder, "global/excel/itemtypes.txt");
        string charStatsPath = Path.Combine(SelectedModDataFolder, "global/excel/charstats.txt");
        string originalSkillTextPath = Path.Combine(SelectedModDataFolder, "D2RLaunch/Originals/skills-original.txt");
        string originalsDirectoryPath = Path.Combine(SelectedModDataFolder, "D2RLaunch/Originals");

        //Create needed folders and files
        if (!File.Exists(itemTypesTextPath))
            Helper.ExtractFileFromCasc(GamePath, @"data:data\global\excel\itemtypes.txt", SelectedModDataFolder, "data:data");
        if (!File.Exists(charStatsPath))
            Helper.ExtractFileFromCasc(GamePath, @"data:data\global\excel\charstats.txt", SelectedModDataFolder, "data:data");
        if (!Directory.Exists(originalsDirectoryPath))
            Directory.CreateDirectory(originalsDirectoryPath);
        if (!File.Exists(skillTextPath))
            Helper.ExtractFileFromCasc(GamePath, @"data:data\global\excel\skills.txt", SelectedModDataFolder, "data:data");

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
                Directory.CreateDirectory(BackupFolder);

            //Backup Character
            FileInfo mostRecentCharacterFile = new DirectoryInfo(SaveFilesFilePath).GetFiles("*.d2s").MaxBy(o => o.LastWriteTime);
            mostRecentCharacterName = Path.GetFileNameWithoutExtension(mostRecentCharacterFile.ToString());

            string mostRecentCharacterBackupFolder = Path.Combine(BackupFolder, mostRecentCharacterName);
            if (!Directory.Exists(mostRecentCharacterBackupFolder))
                Directory.CreateDirectory(mostRecentCharacterBackupFolder);

            File.Copy(mostRecentCharacterFile.FullName, Path.Combine(mostRecentCharacterBackupFolder, mostRecentCharacterName + DateTime.Now.ToString("_MM_dd--hh_mmtt") + ".d2s"), true);

            //Backup Stash
            string mostRecentStashFile = Path.Combine(SaveFilesFilePath, "SharedStashSoftCoreV2.d2i");
            string stashBackupFolder = Path.Combine(BackupFolder, "Stash");

            if (!Directory.Exists(stashBackupFolder))
                Directory.CreateDirectory(stashBackupFolder);

            File.Copy(mostRecentStashFile, Path.Combine(stashBackupFolder, "SharedStashSoftCoreV2" + DateTime.Now.ToString("_MM_dd--hh_mmtt") + ".d2i"), true);
        }
        catch (Exception ex)
        {
            _logger.Error(ex);
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return (null, false);
        }

        return (mostRecentCharacterName, true);
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
            return null;

        string[] subKeyNames = regKey.GetSubKeyNames();
        List<string> results = new();

        foreach (string subKeyName in subKeyNames)
        {
            using RegistryKey subKey = regKey.OpenSubKey(subKeyName);

            if (subKey == null)
                continue;

            string exeFullPath = subKey.GetValue("MatchedExeFullPath")?.ToString();

            if (string.IsNullOrEmpty(exeFullPath))
                continue;

            if (exeFullPath.Contains("D2R.exe"))
                results.Add(exeFullPath);
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

    private async Task SaveUserSettings()
    {
        //Protected
        if (Directory.Exists(SelectedModDataFolder))
            await File.WriteAllTextAsync(SelectedUserSettingsFilePath, JsonConvert.SerializeObject(UserSettings));
        //Unprotected
        else
            await File.WriteAllTextAsync(SelectedUserSettingsFilePath, JsonConvert.SerializeObject(UserSettings).Replace($"{Settings.Default.SelectedMod}.mpq/", ""));
    }

    private async Task RenameCharacter()
    {
        //TODO: This does not account for character that have been backed up. This should also have its own dedicated dialog
        OpenFileDialog ofd = new OpenFileDialog();
        {
            ofd.InitialDirectory = SaveFilesFilePath;
            ofd.Filter = "D2R Character Files (*.d2s)|*.d2s";
        };
        SaveFileDialog sfd = new SaveFileDialog();
        {
            sfd.InitialDirectory = SaveFilesFilePath;
            sfd.DefaultExt = ".d2s";
        };

        ofd.ShowDialog();
        string fileSource = Path.GetFileNameWithoutExtension(ofd.FileName);
        byte[] ba = Encoding.Default.GetBytes(fileSource);
        byte[] bytes = null;
        if (ofd.FileName != "")
        {
            bytes = await File.ReadAllBytesAsync(ofd.FileName);
            MessageBox.Show("Please choose your new save filename");

            sfd.ShowDialog();
            await File.WriteAllBytesAsync(sfd.FileName, bytes);

            string fileSource2 = Path.GetFileNameWithoutExtension(sfd.FileName);
            byte[] ba2 = Encoding.Default.GetBytes(fileSource2);
            string hexString = BitConverter.ToString(ba).Replace("-", string.Empty);
            string hexString2 = BitConverter.ToString(ba2).Replace("-", string.Empty);


            if (fileSource.Length != fileSource2.Length)
            {
                if (fileSource.Length > fileSource2.Length)
                    hexString2 = hexString2 + String.Concat(Enumerable.Repeat("00", fileSource.Length - fileSource2.Length));
                else
                    hexString = hexString + String.Concat(Enumerable.Repeat("00", fileSource2.Length - fileSource.Length));
            }

            MessageBox.Show(hexString);

            string bitString = BitConverter.ToString(bytes).Replace("-", string.Empty).Replace(hexString, hexString2);
            await File.WriteAllBytesAsync(sfd.FileName, Helper.StringToByteArray(bitString));

            byte[] bytes3 = await File.ReadAllBytesAsync(sfd.FileName);
            FixChecksum(bytes3);
            await File.WriteAllBytesAsync(sfd.FileName, bytes3);

            if (ModInfo.Name == "My Custom Mod")
            {
                File.Move(sfd.FileName, Path.Combine(SaveFilesFilePath, "placeholder"));

                foreach (var file in Directory.GetFiles(SaveFilesFilePath, fileSource + "*", SearchOption.TopDirectoryOnly))
                    File.Move(file, file.Replace(fileSource, fileSource2));
                File.Delete(sfd.FileName);
                File.Move(Path.Combine(SaveFilesFilePath, "placeholder"), sfd.FileName);
            }
            MessageBox.Show("Character renamed successfully!");
        }
    }

    private void FixChecksum(byte[] bytes)
    {
        new byte[4].CopyTo(bytes, 0xc);

        int checksum = 0;

        for (int i = 0; i < bytes.Length; i++)
        {
            checksum = bytes[i] + (checksum * 2) + (checksum < 0 ? 1 : 0);
        }

        BitConverter.GetBytes(checksum).CopyTo(bytes, 0xc);
    }

    private async Task FixStash()
    {
        string modPath;
        string hexString = String.Concat(Enumerable.Repeat(TAB_BYTE_CODE, 4));

        //Check for Default Save Path
        if (ModInfo.SavePath.Contains("\"../\""))
            modPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Saved Games\Diablo II Resurrected\";
        else
            modPath = BaseSelectedModFolder;

        if (!File.Exists(modPath + "/SharedStashSoftCoreV2.d2i"))
        {
            Helper.CreateFileIfNotExists(modPath + "/SharedStashSoftCoreV2.d2i", await Helper.GetResourceByteArray("SharedStashSoftCoreV2.d2i"));
            MessageBox.Show("Softcore Stash unlocked!\nYou now have 7 shared tabs in-game");
        }
        else
        {
            byte[] data = await File.ReadAllBytesAsync(modPath + "/SharedStashSoftCoreV2.d2i"); //read file
            string bitString = BitConverter.ToString(data).Replace("-", string.Empty);
            if (Regex.Matches(bitString, "4A4D0000").Count >= 7)
                MessageBox.Show("Softcore Stash already unlocked!");
            else
            {
                File.WriteAllBytes(modPath + "/SharedStashSoftCoreV2.d2i", Helper.StringToByteArray(bitString + hexString));
                MessageBox.Show("Softcore Stash unlocked!\nYou now have 7 shared tabs in-game");
            }
        }

        if (!File.Exists(modPath + "/SharedStashHardCoreV2.d2i"))
        {
            File.WriteAllBytes(modPath + "/SharedStashHardCoreV2.d2i", await Helper.GetResourceByteArray("SharedStashHardCoreV2.d2i"));
            MessageBox.Show("Hardcore Stash unlocked!\nYou now have 7 shared tabs in-game");
        }
        else
        {
            byte[] data = File.ReadAllBytes(modPath + "/SharedStashHardCoreV2.d2i"); //read file
            string bitString = BitConverter.ToString(data).Replace("-", string.Empty);
            if (Regex.Matches(bitString, "4A4D0000").Count >= 7)
                MessageBox.Show("Hardcore Stash already unlocked!");
            else
            {
                File.WriteAllBytes(modPath + "/SharedStashHardCoreV2.d2i", Helper.StringToByteArray(bitString + hexString));
                MessageBox.Show("Hardcore Stash unlocked!\nYou now have 7 shared tabs in-game");
            }
        }
    }

    private async Task CheckForLauncherUpdates()
    {
        WebClient webClient = new();

        if (!File.Exists(@"..\MyVersions_Temp.txt"))
        {
            string primaryLink = "https://drive.google.com/uc?export=download&id=1AW5tOJVpSkWdrXYdjllyPyU3Cpdd-SMV";
            string backupLink = "https://d2filesdrop.s3.us-east-2.amazonaws.com/MyVersions.txt";

            try
            {
                webClient.DownloadFile(primaryLink, @"..\MyVersions_Temp.txt");
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse response && ((int)response.StatusCode == 429 || (int)response.StatusCode == 500))
                {
                    try
                    {
                        webClient.DownloadFile(backupLink, @"..\MyVersions_Temp.txt");
                    }
                    catch (WebException)
                    {
                        _logger.Error("Backup download link for MyVersions_Temp.txt failed.");
                        return;
                    }
                }
                else
                {
                    _logger.Error(ex.Message);
                    _logger.Error("An error occurred during the download: ");
                    return;
                }
            }
        }

        string[] newVersions = await File.ReadAllLinesAsync(@"..\MyVersions_Temp.txt");
        //string LVersion = Directory.GetFiles(@"..\Stasher", "*.xyz").FirstOrDefault()?.Replace(@"..\Stasher\", "").Replace(".xyz", "");

        if (newVersions[0] != appVersion && (newVersions[0].Length <= 5))
        {
            LauncherUpdateString = $"D2RLaunch Update Ready! ({newVersions[0]})";
            LauncherHasUpdate = true;
        }

        File.Delete(@"..\MyVersions_Temp.txt");
    }

    private async Task CheckForVaultUpdates()
    {
        WebClient webClient = new WebClient();

        if (!File.Exists(@"..\MyVersions_Temp.txt"))
        {
            string primaryLink = "https://drive.google.com/uc?export=download&id=1AW5tOJVpSkWdrXYdjllyPyU3Cpdd-SMV";
            string backupLink = "https://d2filesdrop.s3.us-east-2.amazonaws.com/MyVersions.txt";

            try
            {
                webClient.DownloadFile(primaryLink, @"..\MyVersions_Temp.txt");
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse response && ((int)response.StatusCode == 429 || (int)response.StatusCode == 500))
                {
                    try
                    {
                        webClient.DownloadFile(backupLink, @"..\MyVersions_Temp.txt");
                    }
                    catch (WebException)
                    {
                        MessageBox.Show("Backup download link for MyVersions_Temp.txt failed.", "Download Error");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("An error occurred during the download: " + ex.Message, "Download Error");
                    return;
                }
            }
        }

        string[] newVersions = File.ReadAllLines(@"..\MyVersions_Temp.txt");
        string SVersion = Directory.GetFiles(@"..\Stasher", "*.xyz").FirstOrDefault()?.Replace(@"..\Stasher\", "").Replace(".xyz", "");

        if (newVersions.Length >= 2 && newVersions[1] != SVersion)
        {
            string primaryLink2 = "https://dl.dropboxusercontent.com/s/fh8j9okdhr64wpn/D2R_Updater.zip?dl=0";
            string backupLink2 = "https://d2filesdrop.s3.us-east-2.amazonaws.com/D2R_Updater.zip";

            try
            {
                webClient.DownloadFile(primaryLink2, @"..\UpdateU.zip");
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse response && ((int)response.StatusCode == 429 || (int)response.StatusCode == 500))
                {
                    try
                    {
                        webClient.DownloadFile(backupLink2, @"..\UpdateU.zip");
                    }
                    catch (WebException)
                    {
                        MessageBox.Show("Backup download link for UpdateU.zip failed.", "Download Error");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("An error occurred during the download: " + ex.Message, "Download Error");
                    return;
                }
            }

            Directory.Delete(@"..\Updater\", true);
            Directory.CreateDirectory(@"..\Updater\");
            ZipFile.ExtractToDirectory(@"..\UpdateU.zip", @"..\Updater\");
            File.Delete(@"..\UpdateU.zip");

            File.Create(@"..\Stasher\snu.txt").Close();
            MessageBox.Show("There is a new version of the vault available!\n\nCurrent Vault Version: " + SVersion + "\nNew Vault Version: " + newVersions[1] + "\n\nPlease click OK to begin the update process", "Vault Update Available!");
            Process.Start(@"..\Updater\RMDUpdater.exe");
        }
        else
        {
            DirectoryInfo di = new DirectoryInfo(Directory.GetCurrentDirectory());
            Process pr = new Process();
            pr.StartInfo.FileName = "RCM.exe";
            pr.StartInfo.WorkingDirectory = di.Parent.ToString() + @".\Stasher";
            pr.StartInfo.UseShellExecute = true;
            pr.Start();
        }

        File.Delete(@"..\MyVersions_Temp.txt");
    }

    [UsedImplicitly]
    public async void OnUpdateLauncher()
    {
        WebClient webClient = new();

        if (File.Exists("lnu.txt"))
        {
            File.Delete("lnu.txt");
        }
        string primaryLink2 = "https://www.dl.dropboxusercontent.com/scl/fi/m1e8kg5oh334qln8u7i39/D2R_Updater.zip?rlkey=e7o34dut4efpx648x8bq196s8&dl=0";
        string backupLink2 = "https://d2filesdrop.s3.us-east-2.amazonaws.com/D2R_Updater.zip";

        try
        {
            webClient.DownloadFile(primaryLink2, @"..\UpdateU.zip");
        }
        catch (WebException ex)
        {
            if (ex.Response is HttpWebResponse response && ((int)response.StatusCode == 429 || (int)response.StatusCode == 500))
            {
                try
                {
                    webClient.DownloadFile(backupLink2, @"..\UpdateU.zip");
                }
                catch (WebException)
                {
                    _logger.Error("Backup download link 2 failed.");
                    MessageBox.Show("Backup download link 2 failed.", "Download error.", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    return;
                }
            }
            else
            {
                _logger.Error(ex.Message);
                _logger.Error("An error occurred during the download: ");
                MessageBox.Show("An error occurred during the download:", "Download error.", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }
        }

        if (Directory.Exists(@"..\Updater\"))
            Directory.Delete(@"..\Updater\", true);

        Directory.CreateDirectory(@"..\Updater\");
        ZipFile.ExtractToDirectory(@"..\UpdateU.zip", @"..\Updater\");

        if (File.Exists(@"..\UpdateU.zip"))
            File.Delete(@"..\UpdateU.zip");

        File.Create(@"..\Launcher\lnu.txt").Close();
        Process.Start(@"..\Updater\RMDUpdater.exe");

        await TryCloseAsync();

        if (File.Exists(@"..\MyVersions_Temp.txt"))
            File.Delete(@"..\MyVersions_Temp.txt");
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
            case "CUSTOMIZATIONS":
                {
                    CustomizationsDrawerViewModel vm = new CustomizationsDrawerViewModel(this);
                    UserControl = new CustomizationsDrawerView() { DataContext = vm };
                    break;
                }
            case "RENAME CHARACTER":
                {
                    if (ModInfo == null || UserSettings == null)
                        break;

                    await RenameCharacter();
                    break;
                }
            case "FIX STASH":
                {
                    if (ModInfo == null || UserSettings == null)
                        break;

                    //TODO: Would prolly be a good idea to show some type of loading/working indication
                    await FixStash();
                    break;
                }
            case "COMMUNITY DISCORD":
                {
                    if (ModInfo == null || UserSettings == null)
                        break;

                    if (!string.IsNullOrEmpty(ModInfo.Discord))
                    {
                        ProcessStartInfo psi = new ProcessStartInfo(ModInfo.Discord);
                        psi.UseShellExecute = true;
                        Process.Start(psi);
                    }
                    else
                        MessageBox.Show(Helper.GetCultureString("NoDiscord"));
                    break;
                }
            case "WIKI":
                {
                    if (ModInfo == null || UserSettings == null)
                        break;
                    if (!string.IsNullOrEmpty(ModInfo.Wiki))
                    {
                        
                        ProcessStartInfo psi = new ProcessStartInfo(ModInfo.Wiki);
                        psi.UseShellExecute = true;
                        Process.Start(psi);
                    }
                    else
                        MessageBox.Show(Helper.GetCultureString("NoWiki"));
                    break;
                }
            case "COMMUNITY PATREON":
                {
                    if (ModInfo == null || UserSettings == null)
                        break;
                    if (!string.IsNullOrEmpty(ModInfo.Patreon))
                    {
                        ProcessStartInfo psi = new ProcessStartInfo(ModInfo.Patreon);
                        psi.UseShellExecute = true;
                        Process.Start(psi);
                    }
                    else
                        MessageBox.Show(Helper.GetCultureString("NoPatreon"));

                    break;
                }
            case "MOD FILES":
                {
                    if (ModInfo == null || UserSettings == null)
                        break;

                    if (Directory.Exists(SelectedModDataFolder))
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            Arguments = SelectedModDataFolder,
                            FileName = "explorer.exe"
                        };
                        Process.Start(startInfo);
                    }
                    else
                        MessageBox.Show($"{SelectedModDataFolder} Directory does not exist!");
                    break;
                }
            case "SAVE FILES":
                {
                    if (ModInfo == null || UserSettings == null)
                        break;

                    string modPath;

                    //Check for Default Save Path
                    if (ModInfo.SavePath.Contains("\"../\""))
                        modPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Saved Games\Diablo II Resurrected\";
                    else
                        modPath = SaveFilesFilePath;

                    if (Directory.Exists(modPath))
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            Arguments = modPath,
                            FileName = "explorer.exe"
                        };
                        Process.Start(startInfo);
                    }
                    else
                        MessageBox.Show($"{modPath} Directory does not exist!");

                    break;
                }
            case "LAUNCH FILES":
                {
                    if (ModInfo == null || UserSettings == null)
                        break;

                    string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "D2RLaunch");
                    if (Directory.Exists(folderPath))
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            Arguments = folderPath,
                            FileName = "explorer.exe"
                        };
                        Process.Start(startInfo);
                    }
                    else
                        MessageBox.Show($"{folderPath} Directory does not exist!");

                    break;
                }
            case "VAULT FILES":
                {
                    if (ModInfo == null || UserSettings == null)
                        break;

                    //Open Vault Config Folder
                    string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\rcm";
                    if (Directory.Exists(folderPath))
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            Arguments = folderPath,
                            FileName = "explorer.exe"
                        };
                        Process.Start(startInfo);
                    }
                    else
                        MessageBox.Show($"{folderPath} Directory does not exist!");

                    break;
                }
            case "D2RWEBSITE":
                {
                    ProcessStartInfo psi = new ProcessStartInfo("https://d2rmodding.com") { UseShellExecute = true };
                    Process.Start(psi);
                    break;
                }
            case "D2RDISCORD":
                {
                    ProcessStartInfo psi = new ProcessStartInfo("https://www.discord.gg/pqUWcDcjWF") { UseShellExecute = true };
                    Process.Start(psi);
                    break;
                }
            case "D2RYOUTUBE":
                {
                    ProcessStartInfo psi = new ProcessStartInfo("https://www.youtube.com/@locbones1") { UseShellExecute = true };
                    Process.Start(psi);
                    break;
                }
            case "THE VAULT":
                {
                    try
                    {
                        await CheckForVaultUpdates();
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e);
                        MessageBox.Show(e.Message);
                    }
                    break;
                }
            case "PATREON":
                {
                    ProcessStartInfo psi = new ProcessStartInfo("https://patreon.com/bonesyd2r") { UseShellExecute = true };
                    Process.Start(psi);
                    break;
                }

        }
    }

    [UsedImplicitly]
    public async void OnLoaded(object args)
    {
        eLanguage appLanguage = ((eLanguage)Settings.Default.AppLanguage);

        CultureInfo culture = new CultureInfo(appLanguage.GetAttributeOfType<DisplayAttribute>().Name.Split(' ')[1].Trim(new[] { '(', ')' })/*.Insert(2, "-")*/);
        CultureResources.ChangeCulture(culture);

        if (!string.IsNullOrEmpty(Settings.Default.InstallPath))
            GamePath = Settings.Default.InstallPath;
        else
        {
            GamePath = await GetDiabloInstallPath();
            Settings.Default.InstallPath = GamePath;
        }
        
        if (string.IsNullOrEmpty(GamePath))
        {
            DiabloInstallDetected = false;
            MessageBoxResult result = MessageBox.Show("Would you like to manually specify your install loation?\nThis tool is intended for legitimate copies of the game only!", "D2R Install could not be found!", MessageBoxButton.YesNo, MessageBoxImage.Error);
            if (result == MessageBoxResult.Yes)
            {
                WinForms.FolderBrowserDialog openFileDlg = new WinForms.FolderBrowserDialog();
                openFileDlg.InitialDirectory = GamePath;
                openFileDlg.ShowDialog();
                GamePath = openFileDlg.SelectedPath;
                Settings.Default.InstallPath = GamePath;
                DiabloInstallDetected = true;
            }
            else
            {
            }
            //MessageBox.Show("Diablo II Resurrected install could not be found!\nPlease be sure to have a legitimate copy of Diablo II Resurrected installed and restart the application!", "Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
            //return;
            Settings.Default.Save();
        }

        DiabloInstallDetected = true;

        if (!Directory.Exists(BaseModsFolder))
            Directory.CreateDirectory(BaseModsFolder);

        DisableBNetConnection();

        HomeDrawerViewModel vm = new HomeDrawerViewModel(this, _windowManager);
        await vm.Initialize();
        UserControl = new HomeDrawerView() { DataContext = vm };

        await Task.Run(CheckForLauncherUpdates);
    }
}