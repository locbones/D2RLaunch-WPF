using System.Windows.Controls;
using Caliburn.Micro;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;
using Syncfusion.UI.Xaml.NavigationDrawer;
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
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
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
using WinForms = System.Windows.Forms;
using D2RLaunch.ViewModels.Dialogs;
using System.Dynamic;
using System.Net.Sockets;

namespace D2RLaunch.ViewModels;

public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
{
    #region ---Static Members---

    private ILog _logger = LogManager.GetLogger(typeof(ShellViewModel));
    private UserControl _userControl;
    private IWindowManager _windowManager;
    private string _title = "D2RLaunch";
    private string appVersion = "2.5.0";
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
    private bool _ColorDyesEnabled = true;
    private bool _ExpandedInventoryEnabled = true;
    private bool _ExpandedStashEnabled = true;
    private bool _ExpandedCubeEnabled = true;
    private bool _ExpandedMercEnabled = true;

    #endregion

    #region ---Window/Loaded Handlers---

    public ShellViewModel() //Window and Title Display
    {
        if (Execute.InDesignMode)
        {
            ModLogo = "pack://application:,,,/Resources/Images/D2RL_Logo.png";
            Title = "D2RLaunch";
            DiabloInstallDetected = true;
            HomeDrawerViewModel vm = new HomeDrawerViewModel();
            UserControl = new HomeDrawerView() { DataContext = vm };
            Injector injector = new Injector(_gamePath);
        }
    }
    public ShellViewModel(IWindowManager windowManager) //Load Logger
    {
        _windowManager = windowManager;
        _logger.Error("Main Window Loaded");
    }
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
        await ConfigureColorDyes();
        await ConfigureCinematicSubs();
    } //Apply User-Defined QoL Options; executed from HomeDrawerView's OnPlayMod() Function
    [UsedImplicitly]
    public async void OnItemClicked(NavigationItemClickedEventArgs args) //Side Menu Controls
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
                        modPath = BaseSaveFilesFilePath;
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
            case "ERROR LOGS":
                {
                    if (ModInfo == null || UserSettings == null)
                        break;

                    //Open Vault Config Folder
                    string folderPath = "Error Logs";
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
            case "EVENTS":
                {
                    try
                    {
                        await CheckForEvents();
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
    public async void OnLoaded(object args) //Functions to perform after UI has been loaded
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

    #endregion

    #region ---Properties---

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
    public string BaseSaveFilesFilePath => Path.Combine(GetSavePath(), @$"Diablo II Resurrected");
    public string SaveFilesFilePath => Path.Combine(GetSavePath(), @$"Diablo II Resurrected\Mods\{Settings.Default.SelectedMod}");
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
    public bool ExpandedInventoryEnabled
    {
        get => _ExpandedInventoryEnabled;
        set
        {
            if (value == _ExpandedInventoryEnabled) return;
            _ExpandedInventoryEnabled = value;
            NotifyOfPropertyChange();
        }
    }
    public bool ExpandedStashEnabled
    {
        get => _ExpandedStashEnabled;
        set
        {
            if (value == _ExpandedStashEnabled) return;
            _ExpandedStashEnabled = value;
            NotifyOfPropertyChange();
        }
    }
    public bool ExpandedCubeEnabled
    {
        get => _ExpandedCubeEnabled;
        set
        {
            if (value == _ExpandedCubeEnabled) return;
            _ExpandedCubeEnabled = value;
            NotifyOfPropertyChange();
        }
    }
    public bool ExpandedMercEnabled
    {
        get => _ExpandedMercEnabled;
        set
        {
            if (value == _ExpandedMercEnabled) return;
            _ExpandedMercEnabled = value;
            NotifyOfPropertyChange();
        }
    }
    public bool ColorDyesEnabled
    {
        get => _ColorDyesEnabled;
        set
        {
            if (value == _ColorDyesEnabled) return;
            _ColorDyesEnabled = value;
            NotifyOfPropertyChange();
        }
    }
    #endregion

    #region ---Mod Settings---

    private async Task ConfigureHudDesign() //Merged HUD
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
                        if (File.Exists(Path.Combine(SelectedModDataFolder, "D2RLaunch/UI Theme/expanded/layouts/hudpanelhd.json")) || File.Exists(Path.Combine(SelectedModDataFolder, "D2RLaunch/UI Theme/remodded/layouts/hudpanelhd.json")))
                        {
                            if ((eUiThemes)UserSettings.UiTheme == eUiThemes.Standard)
                                File.Copy(Path.Combine(SelectedModDataFolder, "D2RLaunch/UI Theme/expanded/layouts/hudpanelhd.json"), hudPanelhdJsonFilePath, true);
                            else
                                File.Copy(Path.Combine(SelectedModDataFolder, "D2RLaunch/UI Theme/remodded/layouts/hudpanelhd.json"), hudPanelhdJsonFilePath, true);

                            if (File.Exists(controllerhudPanelhdJsonFilePath))
                                File.Delete(controllerhudPanelhdJsonFilePath);

                            // Update skillselecthd.json if it exists
                            if (File.Exists(skillSelecthdJsonFilePath))
                            {
                                string skillSelect = await File.ReadAllTextAsync(skillSelecthdJsonFilePath);
                                await File.WriteAllTextAsync(skillSelecthdJsonFilePath, skillSelect.Replace("\"centerMirrorGapWidth\": 846,", "\"centerMirrorGapWidth\": 146,"));
                            }
                        }
                        else
                            if (File.Exists(hudPanelhdJsonFilePath))
                            File.Delete(hudPanelhdJsonFilePath);
                        break;
                    }
                case eHudDesign.Merged:
                    {
                        if (!Directory.Exists(controllerDirectory))
                            Directory.CreateDirectory(controllerDirectory);

                        if (File.Exists(Path.Combine(SelectedModDataFolder, "D2RLaunch/Merged HUD/hudpanelhd-merged.json")))
                        {
                            File.Copy(Path.Combine(SelectedModDataFolder, "D2RLaunch/Merged HUD/hudpanelhd-merged.json"), hudPanelhdJsonFilePath, true);
                            File.Copy(Path.Combine(SelectedModDataFolder, "D2RLaunch/Merged HUD/Controller/hudpanelhd-merged_controller.json"), controllerhudPanelhdJsonFilePath, true);
                        }

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
    }
    private async Task ConfigureRunewordSorting() //Runeword Sorting
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
    private async Task ConfigureMonsterStatsDisplay() //Advanced Monster Stats
    {
        eMonsterHP monsterHP = (eMonsterHP)UserSettings.MonsterHP;

        string uiLayoutsPath = Path.Combine(SelectedModDataFolder, "global/ui/layouts");
        string hudMonsterHealthHdJsonFilePath = Path.Combine(uiLayoutsPath, "hudmonsterhealthhd.json");
        string hudMonsterHealthHdDisabledJsonFilePath = Path.Combine(uiLayoutsPath, "hudmonsterhealthhd_disabled.json");
        string monsterStatsPath = Path.Combine(SelectedModDataFolder, "D2RLaunch/Monster Stats");
        string outputPath = SelectedModDataFolder + "/D2RLaunch/Monster Stats/MS_Assets.zip";

        if (!Directory.Exists(uiLayoutsPath))
            Directory.CreateDirectory(uiLayoutsPath);

        if (!Directory.Exists(monsterStatsPath))
            Directory.CreateDirectory(monsterStatsPath);

        if (!File.Exists(hudMonsterHealthHdJsonFilePath) && !File.Exists(hudMonsterHealthHdDisabledJsonFilePath))
            await File.WriteAllBytesAsync(hudMonsterHealthHdJsonFilePath, await Helper.GetResourceByteArray("Options.MonsterStats.hudmonsterhealthhd.json"));
        if (!File.Exists(hudMonsterHealthHdJsonFilePath) && File.Exists(hudMonsterHealthHdDisabledJsonFilePath))
            File.Move(hudMonsterHealthHdDisabledJsonFilePath, hudMonsterHealthHdJsonFilePath, true);

        if (File.Exists(SelectedModDataFolder + "/D2RLaunch/Monster Stats/MS_Assets.zip"))
            File.Delete(SelectedModDataFolder + "/D2RLaunch/Monster Stats/MS_Assets.zip");
        else
            await File.WriteAllBytesAsync(outputPath, await Helper.GetResourceByteArray("Options.MonsterStats.MS_Assets.zip"));

        switch (monsterHP)
        {
            case eMonsterHP.Retail:
                {
                    if (File.Exists(SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json"))
                        File.Move(SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd_disabled.json", true);

                    break;
                }
            case eMonsterHP.BasicNoP:
                {
                    ZipFile.ExtractToDirectory(SelectedModDataFolder + "/D2RLaunch/Monster Stats/MS_Assets.zip", SelectedModDataFolder + "/D2RLaunch/Monster Stats/", true);
                    File.Delete(SelectedModDataFolder + "/D2RLaunch/Monster Stats/MS_Assets.zip");

                    if (!File.Exists(SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json"))
                        File.Move(SelectedModDataFolder + "/D2RLaunch/Monster Stats/hudmonsterhealthhd.json", SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", true);

                    string hudContents = File.ReadAllText(SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json");

                    if (hudContents.Contains("HB_A\""))
                        File.Move(SelectedModDataFolder + "/D2RLaunch/Monster Stats/hudmonsterhealthhd.json", SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", true);

                    if (hudContents.Contains("MonHPBar_UniFull\"") || hudContents.Contains("MonHPBar_UniFullPer\""))
                    {
                        hudContents = hudContents.Replace("MonHPBar_UniFull\"", "MonHPBar_UniSmall\"").Replace("MonHPBar_NormFull\"", "MonHPBar_NormSmall\"").Replace("MonHPBar_UniFullPer\"", "MonHPBar_UniSmall\"").Replace("MonHPBar_NormFullPer\"", "MonHPBar_NormSmall\"")
                            .Replace("MonHPBar_UniSmallPer\"", "MonHPBar_UniSmall\"").Replace("MonHPBar_NormSmallPer\"", "MonHPBar_NormSmall\"").Replace("\"y\": 115", "\"y\": 65").Replace("\"y\": 150", "\"y\": 100");
                        File.WriteAllText(SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", hudContents);
                    }

                    break;
                }
            case eMonsterHP.BasicP:
                {
                    ZipFile.ExtractToDirectory(SelectedModDataFolder + "/D2RLaunch/Monster Stats/MS_Assets.zip", SelectedModDataFolder + "/D2RLaunch/Monster Stats/", true);
                    File.Delete(SelectedModDataFolder + "/D2RLaunch/Monster Stats/MS_Assets.zip");

                    if (!File.Exists(SelectedModDataFolder + "/D2RLaunch/Monster Stats/hudmonsterhealthhd.json"))
                        File.Move(SelectedModDataFolder + "/D2RLaunch/Monster Stats/hudmonsterhealthhd.json", SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", true);

                    string hudContents = File.ReadAllText(SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json");

                    if (hudContents.Contains("HB_A\""))
                        File.Move(SelectedModDataFolder + "/D2RLaunch/Monster Stats/hudmonsterhealthhd.json", SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", true);

                    hudContents = hudContents.Replace("MonHPBar_UniFull\"", "MonHPBar_UniSmallPer\"").Replace("MonHPBar_NormFull\"", "MonHPBar_NormSmallPer\"").Replace("MonHPBar_UniFullPer\"", "MonHPBar_UniSmallPer\"").Replace("MonHPBar_NormFullPer\"", "MonHPBar_NormSmallPer\"")
                            .Replace("MonHPBar_UniSmall\"", "MonHPBar_UniSmallPer\"").Replace("MonHPBar_NormSmall\"", "MonHPBar_NormSmallPer\"").Replace("\"y\": 115", "\"y\": 65").Replace("\"y\": 150", "\"y\": 100");
                    File.WriteAllText(SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", hudContents);

                    break;
                }
            case eMonsterHP.AdvancedNoP:
                {
                    ZipFile.ExtractToDirectory(SelectedModDataFolder + "/D2RLaunch/Monster Stats/MS_Assets.zip", SelectedModDataFolder + "/D2RLaunch/Monster Stats/", true);
                    File.Delete(SelectedModDataFolder + "/D2RLaunch/Monster Stats/MS_Assets.zip");

                    if (!File.Exists(SelectedModDataFolder + "/D2RLaunch/Monster Stats/hudmonsterhealthhd.json"))
                        File.Move(SelectedModDataFolder + "/D2RLaunch/Monster Stats/hudmonsterhealthhd.json", SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", true);

                    string hudContents = File.ReadAllText(SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json");

                    if (hudContents.Contains("HB_A\""))
                        File.Move(SelectedModDataFolder + "/D2RLaunch/Monster Stats/hudmonsterhealthhd.json", SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", true);

                    hudContents = hudContents.Replace("MonHPBar_UniFullPer\"", "MonHPBar_UniFull\"").Replace("MonHPBar_NormFullPer\"", "MonHPBar_NormFull\"").Replace("MonHPBar_UniSmallPer\"", "MonHPBar_UniFull\"").Replace("MonHPBar_NormSmallPer\"", "MonHPBar_NormFull\"")
                            .Replace("MonHPBar_UniSmall\"", "MonHPBar_UniFull\"").Replace("MonHPBar_NormSmall\"", "MonHPBar_NormFull\"").Replace("\"y\": 65", "\"y\": 115").Replace("\"y\": 100", "\"y\": 150");
                    File.WriteAllText(SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", hudContents);

                    break;
                }
            case eMonsterHP.AdvancedP:
                {
                    ZipFile.ExtractToDirectory(SelectedModDataFolder + "/D2RLaunch/Monster Stats/MS_Assets.zip", SelectedModDataFolder + "/D2RLaunch/Monster Stats/", true);
                    File.Delete(SelectedModDataFolder + "/D2RLaunch/Monster Stats/MS_Assets.zip");

                    if (!File.Exists(SelectedModDataFolder + "/D2RLaunch/Monster Stats/hudmonsterhealthhd.json"))
                        File.Move(SelectedModDataFolder + "/D2RLaunch/Monster Stats/hudmonsterhealthhd.json", SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", true);

                    string hudContents = File.ReadAllText(SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json");

                    if (hudContents.Contains("HB_A\""))
                        File.Move(SelectedModDataFolder + "/D2RLaunch/Monster Stats/hudmonsterhealthhd.json", SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", true);

                    hudContents = hudContents.Replace("MonHPBar_UniFull\"", "MonHPBar_UniFullPer\"").Replace("MonHPBar_NormFull\"", "MonHPBar_NormFullPer\"").Replace("MonHPBar_UniSmallPer\"", "MonHPBar_UniFullPer\"").Replace("MonHPBar_NormSmallPer\"", "MonHPBar_NormFullPer\"")
                            .Replace("MonHPBar_UniSmall\"", "MonHPBar_UniFullPer\"").Replace("MonHPBar_NormSmall\"", "MonHPBar_NormFullPer\"").Replace("\"y\": 65", "\"y\": 115").Replace("\"y\": 100", "\"y\": 150");
                    File.WriteAllText(SelectedModDataFolder + "/global/ui/layouts/hudmonsterhealthhd.json", hudContents);

                    break;
                }
        }
    }
    private async Task ConfigureHideHelmets() //Hide Helmets
    {
        eEnabledDisabled helmetDisplay = (eEnabledDisabled)UserSettings.HideHelmets;

        //Define filenames and paths
        string helmetBaseDir1 = Path.Combine(SelectedModDataFolder, "hd/items/armor/helmet");
        string helmetBaseDir2 = Path.Combine(SelectedModDataFolder, "hd/items/armor/circlet");
        string helmetBaseDir3 = Path.Combine(SelectedModDataFolder, "hd/items/armor/pelt");
        string[] helmetFiles1 = new[] { "assault_helmet", "avenger_guard", "bone_helm", "cap_hat", "coif_of_glory", "crown", "crown_of_thieves", "duskdeep", "fanged_helm", "full_helm", "great_helm", "helm", "horned_helm", "jawbone_cap", "mask", "ondals_almighty", "rockstopper", "skull_cap", "war_bonnet", "wormskull" };
        string[] helmetFiles2 = new[] { "circlet", "coronet", "diadem", "tiara" };
        string[] helmetFiles3 = new[] { "antlers", "falcon_mask", "hawk_helm", "spirit_mask", "wolf_head" };

        //Add paths and extension to array
        string[] helmetFilesWithExtension1 = helmetFiles1.Select(x => x + ".json").ToArray();
        string[] helmetFilesWithExtension2 = helmetFiles2.Select(x => x + ".json").ToArray();
        string[] helmetFilesWithExtension3 = helmetFiles3.Select(x => x + ".json").ToArray();
        string[] allHelmetFiles1 = helmetFilesWithExtension1.Select(x => Path.Combine(helmetBaseDir1, x)).Concat(helmetFilesWithExtension1.Select(x => Path.Combine(helmetBaseDir1, x))).ToArray();
        string[] allHelmetFiles2 = helmetFilesWithExtension2.Select(x => Path.Combine(helmetBaseDir2, x)).Concat(helmetFilesWithExtension2.Select(x => Path.Combine(helmetBaseDir2, x))).ToArray();
        string[] allHelmetFiles3 = helmetFilesWithExtension3.Select(x => Path.Combine(helmetBaseDir3, x)).Concat(helmetFilesWithExtension3.Select(x => Path.Combine(helmetBaseDir3, x))).ToArray();

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

                    foreach (string filename in allHelmetFiles3)
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
                    if (!Directory.Exists(helmetBaseDir3))
                        Directory.CreateDirectory(helmetBaseDir3);

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

                    foreach (string filename in allHelmetFiles3)
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
    private async Task ConfigureRuneDisplay() //Rune Display (Special Rune Visuals)
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
    private async Task ConfigureItemILvls() //Show Item Levels
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


        eEnabledDisabledModify itemLvls = (eEnabledDisabledModify)UserSettings.ItemIlvls;

        switch (itemLvls)
        {
            case eEnabledDisabledModify.NoChange:
                return;

            case eEnabledDisabledModify.Disabled:
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
            case eEnabledDisabledModify.Enabled:
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
    private async Task ConfigureMercIcons() //Merc Icons
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
    private async Task ConfigureSkillIcons() //Skill Icon Packs
    {
        eSkillIconPack skillIconPack = (eSkillIconPack)UserSettings.SkillIcons;

        string globalUiSpellPath = Path.Combine(SelectedModDataFolder, "hd/global/ui/spells");
        string amazonSkillIconsPath = Path.Combine(SelectedModDataFolder, "hd/global/ui/spells/amazon/amskillicon2.sprite");
        string profileHdJsonPath = Path.Combine(SelectedModDataFolder, "global/ui/layouts/_profilehd.json");
        string skillsTreePanelHdJsonPath = Path.Combine(SelectedModDataFolder, "global/ui/layouts/skillstreepanelhd.json");
        string ControllerskillsTreePanelHdJsonPath = Path.Combine(SelectedModDataFolder, "global/ui/layouts/controller/skillstreepanelhd.json");

        string cascProfileHdJsonFileName = @"data:data\global\ui\layouts\_profilehd.json";
        string cascSkillsTreePanelHdJsonFileName = @"data:data\global\ui\layouts\skillstreepanelhd.json";
        string cascControllerSkillsTreePanelHdJsonFileName = @"data:data\global\ui\layouts\controller\skillstreepanelhd.json";

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

                    if (File.Exists(ControllerskillsTreePanelHdJsonPath))
                    {
                        string profileContents = await File.ReadAllTextAsync(ControllerskillsTreePanelHdJsonPath);
                        profileContents = profileContents.Replace("AmSkillicon2\"", "AmSkillicon\"").Replace("AmSkillicon3\"", "AmSkillicon\"");
                        profileContents = profileContents.Replace("AsSkillicon2\"", "AsSkillicon\"").Replace("AsSkillicon3\"", "AsSkillicon\"");
                        profileContents = profileContents.Replace("BaSkillicon2\"", "BaSkillicon\"").Replace("BaSkillicon3\"", "BaSkillicon\"");
                        profileContents = profileContents.Replace("DrSkillicon2\"", "DrSkillicon\"").Replace("DrSkillicon3\"", "DrSkillicon\"");
                        profileContents = profileContents.Replace("NeSkillicon2\"", "NeSkillicon\"").Replace("NeSkillicon3\"", "NeSkillicon\"");
                        profileContents = profileContents.Replace("PaSkillicon2\"", "PaSkillicon\"").Replace("PaSkillicon3\"", "PaSkillicon\"");
                        profileContents = profileContents.Replace("SoSkillicon2\"", "SoSkillicon\"").Replace("SoSkillicon3\"", "SoSkillicon\"");
                        await File.WriteAllTextAsync(ControllerskillsTreePanelHdJsonPath, profileContents);
                    }
                    break;
                }
            case eSkillIconPack.ReMoDDeD:
                {
                    if (!File.Exists(profileHdJsonPath))
                        Helper.ExtractFileFromCasc(GamePath, cascProfileHdJsonFileName, SelectedModDataFolder, "data:data");

                    if (!File.Exists(skillsTreePanelHdJsonPath))
                        Helper.ExtractFileFromCasc(GamePath, cascSkillsTreePanelHdJsonFileName, SelectedModDataFolder, "data:data");

                    if (!File.Exists(ControllerskillsTreePanelHdJsonPath))
                        Helper.ExtractFileFromCasc(GamePath, cascControllerSkillsTreePanelHdJsonFileName, SelectedModDataFolder, "data:data");

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

                    if (File.Exists(ControllerskillsTreePanelHdJsonPath))
                    {
                        string profileContents = await File.ReadAllTextAsync(ControllerskillsTreePanelHdJsonPath);
                        profileContents = profileContents.Replace("AmSkillicon\"", "AmSkillicon2\"").Replace("AmSkillicon3\"", "AmSkillicon2\"");
                        profileContents = profileContents.Replace("AsSkillicon\"", "AsSkillicon2\"").Replace("AsSkillicon3\"", "AsSkillicon2\"");
                        profileContents = profileContents.Replace("BaSkillicon\"", "BaSkillicon2\"").Replace("BaSkillicon3\"", "BaSkillicon2\"");
                        profileContents = profileContents.Replace("DrSkillicon\"", "DrSkillicon2\"").Replace("DrSkillicon3\"", "DrSkillicon2\"");
                        profileContents = profileContents.Replace("NeSkillicon\"", "NeSkillicon2\"").Replace("NeSkillicon3\"", "NeSkillicon2\"");
                        profileContents = profileContents.Replace("PaSkillicon\"", "PaSkillicon2\"").Replace("PaSkillicon3\"", "PaSkillicon2\"");
                        profileContents = profileContents.Replace("SoSkillicon\"", "SoSkillicon2\"").Replace("SoSkillicon3\"", "SoSkillicon2\"");
                        await File.WriteAllTextAsync(ControllerskillsTreePanelHdJsonPath, profileContents);
                    }
                    break;
                }
            case eSkillIconPack.Dize:
                {
                    if (!File.Exists(profileHdJsonPath))
                        Helper.ExtractFileFromCasc(GamePath, cascProfileHdJsonFileName, SelectedModDataFolder, "data:data");

                    if (!File.Exists(skillsTreePanelHdJsonPath))
                        Helper.ExtractFileFromCasc(GamePath, cascSkillsTreePanelHdJsonFileName, SelectedModDataFolder, "data:data");

                    if (!File.Exists(ControllerskillsTreePanelHdJsonPath))
                        Helper.ExtractFileFromCasc(GamePath, cascControllerSkillsTreePanelHdJsonFileName, SelectedModDataFolder, "data:data");

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

                    if (File.Exists(ControllerskillsTreePanelHdJsonPath))
                    {
                        string profileContents = await File.ReadAllTextAsync(ControllerskillsTreePanelHdJsonPath);
                        profileContents = profileContents.Replace("AmSkillicon\"", "AmSkillicon2\"").Replace("AmSkillicon3\"", "AmSkillicon2\"");
                        profileContents = profileContents.Replace("AsSkillicon\"", "AsSkillicon2\"").Replace("AsSkillicon3\"", "AsSkillicon2\"");
                        profileContents = profileContents.Replace("BaSkillicon\"", "BaSkillicon2\"").Replace("BaSkillicon3\"", "BaSkillicon2\"");
                        profileContents = profileContents.Replace("DrSkillicon\"", "DrSkillicon2\"").Replace("DrSkillicon3\"", "DrSkillicon2\"");
                        profileContents = profileContents.Replace("NeSkillicon\"", "NeSkillicon2\"").Replace("NeSkillicon3\"", "NeSkillicon2\"");
                        profileContents = profileContents.Replace("PaSkillicon\"", "PaSkillicon2\"").Replace("PaSkillicon3\"", "PaSkillicon2\"");
                        profileContents = profileContents.Replace("SoSkillicon\"", "SoSkillicon2\"").Replace("SoSkillicon3\"", "SoSkillicon2\"");
                        await File.WriteAllTextAsync(ControllerskillsTreePanelHdJsonPath, profileContents);
                    }
                    break;
                }
        }
    }
    private async Task ConfigureBuffIcons() //Buff Icons
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

    #region ---Super TK---
    private async Task ConfigureSuperTelekinesis() //Super Telekinesis
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
    } //Used in ConfigureSuperTelekinesis()
    private void RemoveSuperTkSkill() //Used in ConfigureSuperTelekinesis()
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

                    //Write blank entries to remove SuperTK Skill reference
                    if (i > 0 && i != 6)
                    {
                        splitContent[34] = "";
                        charStatsLines[i] = string.Join("\t", splitContent);
                    }
                }

                for (int i = 0; i < itemTypesLines.Length; i++)
                {
                    //Write blank entries to remove Equiv2 itemtype modifiers
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
    #endregion

    #region ---Item Icons---
    private async Task ConfigureItemIcons() //Item Display (Item/Rune Icons)
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
                    if (!Directory.Exists(SelectedModDataFolder + "/hd/ui/fonts"))
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

    private void ItemIconsHide(string itemNameOriginalJsonFilePath, string itemNameJsonFilePath) //Used in ConfigureItemIcons()
    {
        if (File.Exists(itemNameOriginalJsonFilePath))
        {
            string namesFile = File.ReadAllText(itemNameOriginalJsonFilePath);
            File.WriteAllText(itemNameJsonFilePath, namesFile);
        }
    }

    private void ItemIconsShow(string itemNameOriginalJsonFilePath) //Used in ConfigureItemIcons()
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

    private void RuneIconsHide(string itemRuneJsonFilePath) //Used in ConfigureItemIcons()
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

    private void RuneIconsShow(string itemRuneJsonFilePath) //Used in ConfigureItemIcons()
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

    #endregion

    #region ---Color Dyes---
    private async Task ConfigureColorDyes() //Enable or Disable Color Dye System
    {
        eEnabledDisabled ColorDyes = (eEnabledDisabled)UserSettings.ColorDye;

        switch (ColorDyes)
        {
            case eEnabledDisabled.Disabled:
                {
                    string filePath = "";
                    string searchString = "";
                    int rowsToDelete = 0;

                    if (ModInfo.Name == "ReMoDDeD")
                        return;

                    if (File.Exists(Path.Combine(Path.Combine(SelectedModDataFolder, "global/excel/itemstatcost.txt"))))
                    {
                        filePath = Path.Combine(Path.Combine(SelectedModDataFolder, "global/excel/itemstatcost.txt"));
                        searchString = "ColorDye_White";
                        rowsToDelete = 8;
                        RemoveColorDyes(filePath, searchString, rowsToDelete);
                    }

                    if (File.Exists(Path.Combine(Path.Combine(SelectedModDataFolder, "global/excel/properties.txt"))))
                    {
                        filePath = Path.Combine(Path.Combine(SelectedModDataFolder, "global/excel/properties.txt"));
                        searchString = "CD_White";
                        rowsToDelete = 8;
                        RemoveColorDyes(filePath, searchString, rowsToDelete);
                    }

                    if (File.Exists(Path.Combine(Path.Combine(SelectedModDataFolder, "global/excel/states.txt"))))
                    {
                        filePath = Path.Combine(Path.Combine(SelectedModDataFolder, "global/excel/states.txt"));
                        searchString = "Weapon_White";
                        rowsToDelete = 28;
                        RemoveColorDyes(filePath, searchString, rowsToDelete);
                    }

                    if (File.Exists(Path.Combine(Path.Combine(SelectedModDataFolder, "global/excel/cubemain.txt"))))
                    {
                        filePath = Path.Combine(Path.Combine(SelectedModDataFolder, "global/excel/cubemain.txt"));
                        searchString = "Weapon - Normal -> White";
                        rowsToDelete = 224;
                        RemoveColorDyes(filePath, searchString, rowsToDelete);
                    }


                    try
                    {
                        string stringPath = Path.Combine(Path.Combine(SelectedModDataFolder, "local/lng/strings/item-modifiers.json"));

                        if (!File.Exists(stringPath))
                        {
                            Console.WriteLine("File does not exist. No entries to remove.");
                            return;
                        }

                        List<Entry> entries;

                        using (StreamReader file = File.OpenText(stringPath))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            entries = (List<Entry>)serializer.Deserialize(file, typeof(List<Entry>));
                        }

                        // Remove entries with specified IDs
                        int[] idsToRemove = { 48000, 48001, 48002, 48003, 48004, 48005, 48006 };
                        entries.RemoveAll(entry => idsToRemove.Contains(entry.id));

                        using (StreamWriter file = File.CreateText(stringPath))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            serializer.Serialize(file, entries);
                        }

                        Console.WriteLine("Entries removed successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: " + ex.Message);
                    }

                    break;
                }
            case eEnabledDisabled.Enabled:
                {
                    await DyesISC();
                    await DyesProp();
                    await DyesState();
                    await DyesCube();
                    break;
                }
        }
    }

    private async Task DyesISC()
    {
        string iscPath = Path.Combine(Path.Combine(SelectedModDataFolder, "global/excel/itemstatcost.txt"));
        string iscPath2 = Path.Combine(Path.Combine(SelectedModDataFolder, "global/excel/itemstatcost2.txt"));

        if (!File.Exists(Path.Combine(Path.Combine(SelectedModDataFolder, "global/excel/itemstatcost.txt"))))
            Helper.ExtractFileFromCasc(GamePath, @"data:data\global\excel\itemstatcost.txt", SelectedModDataFolder, "data:data");

        try
        {
            int statIndex = -1;
            int idIndex = -1;
            int sendBitsIndex = -1;
            int LegacysaveBitsIndex = -1;
            int LegacysaveAddIndex = -1;
            int saveBitsIndex = -1;
            int saveAddIndex = -1;
            int descpriorityIndex = -1;
            int descfuncIndex = -1;
            int descvalIndex = -1;
            int descstr1Index = -1;
            int descstr2Index = -1;
            int eolIndex = -1;

            // Read existing content and determine column indices
            List<string> lines = new List<string>();
            List<string[]> dataRows = new List<string[]>();
            using (StreamReader reader = new StreamReader(iscPath))
            {
                string line;
                bool isFirstRow = true;
                while ((line = reader.ReadLine()) != null)
                {
                    if (isFirstRow)
                    {
                        // Parse the header row to get column indices
                        string[] columns = line.Split('\t');
                        statIndex = Array.IndexOf(columns, "Stat");
                        idIndex = Array.IndexOf(columns, "*ID");
                        sendBitsIndex = Array.IndexOf(columns, "Send Bits");
                        LegacysaveBitsIndex = Array.IndexOf(columns, "1.09-Save Bits");
                        LegacysaveAddIndex = Array.IndexOf(columns, "1.09-Save Add");
                        saveBitsIndex = Array.IndexOf(columns, "Save Bits");
                        saveAddIndex = Array.IndexOf(columns, "Save Add");
                        descpriorityIndex = Array.IndexOf(columns, "descpriority");
                        descfuncIndex = Array.IndexOf(columns, "descfunc");
                        descvalIndex = Array.IndexOf(columns, "descval");
                        descstr1Index = Array.IndexOf(columns, "descstrpos");
                        descstr2Index = Array.IndexOf(columns, "descstrneg");
                        eolIndex = Array.IndexOf(columns, "*eol");

                        // Verify if all indices are found
                        if (statIndex == -1 || idIndex == -1 || sendBitsIndex == -1 || LegacysaveBitsIndex == -1 || LegacysaveAddIndex == -1 || saveBitsIndex == -1 || saveAddIndex == -1 || descpriorityIndex == -1 || descfuncIndex == -1 || descvalIndex == -1 || descstr1Index == -1 || descstr2Index == -1 || eolIndex == -1)
                        {
                            throw new Exception("One or more columns not found in the header row.");
                        }

                        isFirstRow = false;
                        // Store the header row
                        lines.Add(line);
                    }
                    else
                    {
                        // Check if "ColorDye_White" already exists in the "Stat" column
                        string[] columns = line.Split('\t');
                        if (statIndex != -1 && columns.Length > statIndex && columns[statIndex] == "ColorDye_White")
                            return; // Exit the method as no modifications are needed

                        // Store existing rows for later
                        lines.Add(line);
                    }
                }
            }

            // Add 8 new empty rows
            for (int i = 0; i < 8; i++)
            {
                // Create an empty row
                string[] newRow = new string[Math.Max(statIndex, Math.Max(idIndex, Math.Max(sendBitsIndex, Math.Max(saveBitsIndex, Math.Max(saveAddIndex, Math.Max(descpriorityIndex, Math.Max(descfuncIndex, Math.Max(descvalIndex, Math.Max(descstr1Index, Math.Max(descstr2Index, eolIndex)))))))))) + 1];
                // Fill with empty strings
                Array.Fill(newRow, "");
                // Add this empty row to the dataRows list
                dataRows.Add(newRow);
            }

            // Get the total number of rows in the file
            int totalRowCount = lines.Count - 1; // Excluding the header row

            // Fill in specified columns for the new rows
            string[] colorDyes = { "ColorDye_White", "ColorDye_Black", "ColorDye_Red", "ColorDye_Green", "ColorDye_Blue", "ColorDye_Yellow", "ColorDye_Purple" };
            string[] iscStrings = { "ModCDWhite", "ModCDBlack", "ModCDRed", "ModCDGreen", "ModCDBlue", "ModCDYellow", "ModCDPurple" };
            for (int i = 0; i < 7; i++)
            {
                dataRows[i][statIndex] = colorDyes[i];
                dataRows[i][idIndex] = ((totalRowCount - 1) + i + 1).ToString(); // Assigning unique row numbers
                dataRows[i][sendBitsIndex] = "2";
                dataRows[i][LegacysaveBitsIndex] = "2";
                dataRows[i][LegacysaveAddIndex] = "1";
                dataRows[i][saveBitsIndex] = "2";
                dataRows[i][saveAddIndex] = "1";
                dataRows[i][descpriorityIndex] = "999";
                dataRows[i][descfuncIndex] = "3";
                dataRows[i][descvalIndex] = "0";
                dataRows[i][descstr1Index] = iscStrings[i];
                dataRows[i][descstr2Index] = iscStrings[i];
                dataRows[i][eolIndex] = "0";
            }

            // Fill in specified columns for the 8th row
            dataRows[7][statIndex] = "ColorDye_Tracker";
            dataRows[7][idIndex] = (totalRowCount + 7).ToString(); // Assigning unique row number
            dataRows[7][sendBitsIndex] = "4";
            dataRows[7][LegacysaveBitsIndex] = "4";
            dataRows[7][LegacysaveAddIndex] = "7";
            dataRows[7][saveBitsIndex] = "4";
            dataRows[7][saveAddIndex] = "7";
            dataRows[7][eolIndex] = "0";

            // Write back to the file
            using (StreamWriter writer = new StreamWriter(iscPath, append: false))
            {
                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }
                // Append the new rows to the file
                foreach (var row in dataRows)
                {
                    writer.WriteLine(string.Join("\t", row));
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }

        try
        {
            // Read existing JSON file
            string filePath = Path.Combine(Path.Combine(SelectedModDataFolder, "local/lng/strings/item-modifiers.json"));

            if (!File.Exists(filePath))
                Helper.ExtractFileFromCasc(GamePath, @"data:data\local\lng\strings\item-modifiers.json", SelectedModDataFolder, "data:data");

            List<Entry> entries;

            using (StreamReader file = File.OpenText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                entries = (List<Entry>)serializer.Deserialize(file, typeof(List<Entry>));
            }

            // Add new entries for color dye strings
            entries.Add(new Entry
            {
                id = 48000,
                Key = "ModCDWhite",
                deDE = "ÿc4Farbe gefärbt: ÿc0Weiß",
                enUS = "ÿc4Color Dyed: ÿc0White",
                esES = "ÿc4Color teñido: ÿc0Blanco",
                esMX = "ÿc4Color teñido: ÿc0Blanco",
                frFR = "ÿc4Couleur teint : ÿc0Blanc",
                itIT = "ÿc4Colore tinto: ÿc0Bianco",
                jaJP = "ÿc4カラー染色: ÿc0ホワイト",
                koKR = "ÿc4색상 염색: ÿc0White",
                plPL = "ÿc4Color Barwiony: ÿc0Biały",
                ptBR = "ÿc4Cor tingida: ÿc0Branco",
                ruRU = "ÿc4Окрашенный цвет: ÿc0Белый",
                zhCN = "ÿc4Color 染色：ÿc0White",
                zhTW = "ÿc4Color 染色：ÿc0White"
            });

            entries.Add(new Entry
            {
                id = 48001,
                Key = "ModCDBlack",
                deDE = "ÿc4Farbe gefärbt: ÿc5Schwarz",
                enUS = "ÿc4Color Dyed: ÿc5Black",
                esES = "ÿc4Color teñido: ÿc5Negro",
                esMX = "ÿc4Color teñido: ÿc5Negro",
                frFR = "ÿc4Couleur teint : ÿc5Noir",
                itIT = "ÿc4Colore tinto: ÿc5Nero",
                jaJP = "ÿc4カラー染色: ÿc5ブラック",
                koKR = "ÿc4색상 염색: ÿc5Black",
                plPL = "ÿc4Color Barwiony: ÿc5Black",
                ptBR = "ÿc4Cor tingida: ÿc5Preto",
                ruRU = "ÿc4Окрашенный цвет: ÿc5Черный",
                zhCN = "ÿc4Color 染色：ÿc5Black",
                zhTW = "ÿc4Color 染色：ÿc5Black"
            });

            entries.Add(new Entry
            {
                id = 48002,
                Key = "ModCDRed",
                deDE = "ÿc4Farbe gefärbt: ÿc1Rot",
                enUS = "ÿc4Color Dyed: ÿc1Red",
                esES = "ÿc4Color teñido: ÿc1Rojo",
                esMX = "ÿc4Color teñido: ÿc1Rojo",
                frFR = "ÿc4Color Teint : ÿc1Red",
                itIT = "ÿc4Colore tinto: ÿc1Rosso",
                jaJP = "ÿc4色染め: ÿc1レッド",
                koKR = "ÿc4색상 염색: ÿc1Red",
                plPL = "ÿc4Color Barwiony: ÿc1Red",
                ptBR = "ÿc4Cor tingida: ÿc1Vermelho",
                ruRU = "Окрашенный цвет ÿc4: ÿc1Red",
                zhCN = "ÿc4Color 染色：ÿc1Red",
                zhTW = "ÿc4Color 染色：ÿc1Red"
            });

            entries.Add(new Entry
            {
                id = 48003,
                Key = "ModCDGreen",
                deDE = "ÿc4Farbe gefärbt: ÿc2Grün",
                enUS = "ÿc4Color Dyed: ÿc2Green",
                esES = "ÿc4Color teñido: ÿc2Verde",
                esMX = "ÿc4Color teñido: ÿc2Verde",
                frFR = "ÿc4Color Teint : ÿc2Green",
                itIT = "ÿc4Colore tinto: ÿc2Verde",
                jaJP = "ÿc4色染め: ÿc2グリーン",
                koKR = "ÿc4Color 염색: ÿc2Green",
                plPL = "ÿc4Color Barwiony: ÿc2Green",
                ptBR = "ÿc4Cor tingida: ÿc2Verde",
                ruRU = "ÿc4Окрашенный цвет: ÿc2Зеленый",
                zhCN = "ÿc4Color 染色：ÿc2Green",
                zhTW = "ÿc4Color 染色：ÿc2Green"
            });

            entries.Add(new Entry
            {
                id = 48004,
                Key = "ModCDBlue",
                deDE = "ÿc4Farbe gefärbt: ÿc3Blau",
                enUS = "ÿc4Color Dyed: ÿc3Blue",
                esES = "ÿc4Color teñido: ÿc3Azul",
                esMX = "ÿc4Color teñido: ÿc3Azul",
                frFR = "ÿc4Color Teint : ÿc3Blue",
                itIT = "ÿc4Colore tinto: ÿc3Blu",
                jaJP = "ÿc4カラー染色: ÿc3ブルー",
                koKR = "ÿc4Color 염색: ÿc3Blue",
                plPL = "ÿc4Color Barwiony: ÿc3Blue",
                ptBR = "ÿc4Cor tingida: ÿc3Azul",
                ruRU = "ÿc4Окрашенный цвет: ÿc3Blue",
                zhCN = "ÿc4Color 染色：ÿc3Blue",
                zhTW = "ÿc4Color 染色：ÿc3Blue"
            });

            entries.Add(new Entry
            {
                id = 48005,
                Key = "ModCDYellow",
                deDE = "ÿc4Farbe gefärbt: ÿc9Gelb",
                enUS = "ÿc4Color Dyed: ÿc9Yellow",
                esES = "ÿc4Color teñido: ÿc9Amarillo",
                esMX = "ÿc4Color teñido: ÿc9Amarillo",
                frFR = "ÿc4Couleur teinte : ÿc9Jaune",
                itIT = "ÿc4Colore tinto: ÿc9Giallo",
                jaJP = "ÿc4色染め：ÿc9イエロー",
                koKR = "ÿc4색상 염색: ÿc9Yellow",
                plPL = "ÿc4Color Barwiony: ÿc9Yellow",
                ptBR = "ÿc4Cor tingida: ÿc9Amarelo",
                ruRU = "ÿc4Окрашенный цвет: ÿc9Желтый",
                zhCN = "ÿc4颜色染色：ÿc9黄色",
                zhTW = "ÿc4顏色染色：ÿc9黃色"
            });

            entries.Add(new Entry
            {
                id = 48006,
                Key = "ModCDPurple",
                deDE = "ÿc4Farbe gefärbt: ÿc;Lila",
                enUS = "ÿc4Color Dyed: ÿc;Purple",
                esES = "ÿc4Color teñido: ÿc;Púrpura",
                esMX = "ÿc4Color teñido: ÿc;Púrpura",
                frFR = "ÿc4Color Teint : ÿc ; Violet",
                itIT = "ÿc4Colore tinto: ÿc;Viola",
                jaJP = "ÿc4カラー染色: ÿc;パープル",
                koKR = "ÿc4색상 염색: ÿc; 보라색",
                plPL = "ÿc4Color Barwiony: ÿc;Fioletowy",
                ptBR = "ÿc4Cor tingida: ÿc;Roxo",
                ruRU = "ÿc4Окрашенный цвет: ÿc;Фиолетовый",
                zhCN = "ÿc4颜色染色：ÿc；紫色",
                zhTW = "ÿc4顏色染色：ÿc；紫色"
            });

            // Write the new color dye entries back to the JSON file
            using (StreamWriter file = File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, entries);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }

    private async Task DyesProp()
    {
        string propPath = Path.Combine(SelectedModDataFolder, "global/excel/properties.txt");
        string propPath2 = Path.Combine(SelectedModDataFolder, "global/excel/properties2.txt");

        if (!File.Exists(Path.Combine(Path.Combine(SelectedModDataFolder, "global/excel/properties.txt"))))
            Helper.ExtractFileFromCasc(GamePath, @"data:data\global\excel\properties.txt", SelectedModDataFolder, "data:data");

        try
        {
            // Initialize column indices
            int codeIndex = -1, enabledIndex = -1, funcIndex = -1, statIndex = -1;

            // Read existing content and determine column indices
            List<string> lines = new List<string>();
            List<string[]> dataRows = new List<string[]>();
            using (StreamReader reader = new StreamReader(propPath))
            {
                string line;
                bool isFirstRow = true;
                while ((line = reader.ReadLine()) != null)
                {
                    if (isFirstRow)
                    {
                        // Parse the header row to get column indices
                        string[] columns = line.Split('\t');
                        codeIndex = Array.IndexOf(columns, "code");
                        enabledIndex = Array.IndexOf(columns, "*Enabled");
                        funcIndex = Array.IndexOf(columns, "func1");
                        statIndex = Array.IndexOf(columns, "stat1");

                        // Verify if all indices are found
                        if (codeIndex == -1 || enabledIndex == -1 || funcIndex == -1 || statIndex == -1)
                        {
                            throw new Exception("One or more columns not found in the header row.");
                        }

                        isFirstRow = false;
                        lines.Add(line); // Store the header row
                    }
                    else
                    {
                        // Check if "CD_White" already exists in the "stat1" column
                        string[] columns = line.Split('\t');
                        if (codeIndex != -1 && columns.Length > codeIndex && columns[codeIndex] == "CD_White")
                            return;

                        lines.Add(line); // Store existing rows for later
                    }
                }
            }

            // Add 8 new empty rows
            for (int i = 0; i < 8; i++)
            {
                string[] newRow = new string[Math.Max(codeIndex, Math.Max(enabledIndex, Math.Max(funcIndex, statIndex))) + 1];
                Array.Fill(newRow, "");
                dataRows.Add(newRow);
            }

            // Fill in specified columns for the new rows
            string[] colorDyes = { "CD_White", "CD_Black", "CD_Red", "CD_Green", "CD_Blue", "CD_Yellow", "CD_Purple", "CD_Tracker" };
            string[] colorDyesStats = { "ColorDye_White", "ColorDye_Black", "ColorDye_Red", "ColorDye_Green", "ColorDye_Blue", "ColorDye_Yellow", "ColorDye_Purple", "ColorDye_Tracker" };
            for (int i = 0; i < 8; i++)
            {
                dataRows[i][codeIndex] = colorDyes[i];
                dataRows[i][enabledIndex] = "1";
                dataRows[i][funcIndex] = "1";
                dataRows[i][statIndex] = colorDyesStats[i];
            }

            // Write back to the file
            using (StreamWriter writer = new StreamWriter(propPath, append: false))
            {
                foreach (var line in lines)
                    writer.WriteLine(line);

                foreach (var row in dataRows)
                    writer.WriteLine(string.Join("\t", row));
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }
    }

    private async Task DyesState()
    {
        string statePath = Path.Combine(Path.Combine(SelectedModDataFolder, "global/excel/states.txt"));
        string statePath2 = Path.Combine(Path.Combine(SelectedModDataFolder, "global/excel/states2.txt"));

        if (!File.Exists(Path.Combine(Path.Combine(SelectedModDataFolder, "global/excel/states.txt"))))
            Helper.ExtractFileFromCasc(GamePath, @"data:data\global\excel\states.txt", SelectedModDataFolder, "data:data");

        try
        {
            int stateIndex = -1;
            int idIndex = -1;
            int itemtypeIndex = -1;
            int itemtransIndex = -1;
            int eolIndex = -1;

            // Read existing content and determine column indices
            List<string> lines = new List<string>();
            List<string[]> dataRows = new List<string[]>();
            using (StreamReader reader = new StreamReader(statePath))
            {
                string line;
                bool isFirstRow = true;
                while ((line = reader.ReadLine()) != null)
                {
                    if (isFirstRow)
                    {
                        // Parse the header row to get column indices
                        string[] columns = line.Split('\t');
                        stateIndex = Array.IndexOf(columns, "state");
                        idIndex = Array.IndexOf(columns, "*ID");
                        itemtypeIndex = Array.IndexOf(columns, "itemtype");
                        itemtransIndex = Array.IndexOf(columns, "itemtrans");
                        eolIndex = Array.IndexOf(columns, "*eol");

                        // Verify if all indices are found
                        if (stateIndex == -1 || idIndex == -1 || itemtypeIndex == -1 || itemtransIndex == -1 || eolIndex == -1)
                        {
                            throw new Exception("One or more columns not found in the header row.");
                        }

                        isFirstRow = false;
                        // Store the header row
                        lines.Add(line);
                    }
                    else
                    {
                        // Check if "ColorDye_White" already exists in the "Stat" column
                        string[] columns = line.Split('\t');
                        if (stateIndex != -1 && columns.Length > stateIndex && columns[stateIndex] == "Weapon_White")
                            return; // Exit the method as no modifications are needed

                        // Store existing rows for later
                        lines.Add(line);
                    }
                }
            }

            // Add 8 new empty rows
            for (int i = 0; i < 28; i++)
            {
                // Create an empty row
                string[] newRow = new string[Math.Max(stateIndex, Math.Max(idIndex, Math.Max(itemtypeIndex, Math.Max(itemtransIndex, eolIndex)))) + 1];
                // Fill with empty strings
                Array.Fill(newRow, "");
                // Add this empty row to the dataRows list
                dataRows.Add(newRow);
            }

            // Get the total number of rows in the file
            int totalRowCount = lines.Count - 1; // Excluding the header row

            // Fill in specified columns for the new rows
            string[] colorDyesW = { "Weapon_White", "Weapon_Black", "Weapon_Red", "Weapon_Green", "Weapon_Blue", "Weapon_Yellow", "Weapon_Purple" };
            string[] colorDyesA = { "Torso_White", "Torso_Black", "Torso_Red", "Torso_Green", "Torso_Blue", "Torso_Yellow", "Torso_Purple" };
            string[] colorDyesH = { "Helm_White", "Helm_Black", "Helm_Red", "Helm_Green", "Helm_Blue", "Helm_Yellow", "Helm_Purple" };
            string[] colorDyesS = { "Shield_White", "Shield_Black", "Shield_Red", "Shield_Green", "Shield_Blue", "Shield_Yellow", "Shield_Purple" };
            string[] colorDyesCode = { "bwht", "blac", "cred", "cgrn", "cblu", "lyel", "lpur" };

            // Filling for weapon rows (7 rows)
            for (int i = 0; i < 7; i++)
            {
                dataRows[i][stateIndex] = colorDyesW[i];
                dataRows[i][idIndex] = ((totalRowCount - 1) + i + 1).ToString(); // Assigning unique row numbers
                dataRows[i][itemtypeIndex] = "weap";
                dataRows[i][itemtransIndex] = colorDyesCode[i];
                dataRows[i][eolIndex] = "0";
            }

            // Filling for tors rows (7 rows starting from index 7)
            for (int i = 7; i < 14; i++)
            {
                dataRows[i][stateIndex] = colorDyesA[i - 7];
                dataRows[i][idIndex] = ((totalRowCount - 1) + i + 1).ToString(); // Assigning unique row numbers
                dataRows[i][itemtypeIndex] = "tors";
                dataRows[i][itemtransIndex] = colorDyesCode[i - 7];
                dataRows[i][eolIndex] = "0";
            }

            // Filling for helm rows (7 rows starting from index 14)
            for (int i = 14; i < 21; i++)
            {
                dataRows[i][stateIndex] = colorDyesH[i - 14];
                dataRows[i][idIndex] = ((totalRowCount - 1) + i + 1).ToString(); // Assigning unique row numbers
                dataRows[i][itemtypeIndex] = "helm";
                dataRows[i][itemtransIndex] = colorDyesCode[i - 14];
                dataRows[i][eolIndex] = "0";
            }

            // Filling for shld rows (7 rows starting from index 21)
            for (int i = 21; i < 28; i++)
            {
                dataRows[i][stateIndex] = colorDyesS[i - 21];
                dataRows[i][idIndex] = ((totalRowCount - 1) + i + 1).ToString(); // Assigning unique row numbers
                dataRows[i][itemtypeIndex] = "shld";
                dataRows[i][itemtransIndex] = colorDyesCode[i - 21];
                dataRows[i][eolIndex] = "0";
            }



            // Write back to the file
            using (StreamWriter writer = new StreamWriter(statePath, append: false))
            {
                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }
                // Append the new rows to the file
                foreach (var row in dataRows)
                {
                    writer.WriteLine(string.Join("\t", row));
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }
    }

    private async Task DyesCube()
    {

        string cubePath = Path.Combine(SelectedModDataFolder, "global/excel/cubemain.txt");
        string cubePath2 = Path.Combine(SelectedModDataFolder, "global/excel/cubemain2.txt");

        if (!File.Exists(Path.Combine(Path.Combine(SelectedModDataFolder, "global/excel/cubemain.txt"))))
            Helper.ExtractFileFromCasc(GamePath, @"data:data\global\excel\cubemain.txt", SelectedModDataFolder, "data:data");

        try
        {
            // Define column indices
            int descriptionIndex = -1, enabledIndex = -1, opIndex = -1, paramIndex = -1, valueIndex = -1,
                inputsIndex = -1, input1Index = -1, input2Index = -1, outputIndex = -1, mod1Index = -1,
                mod1minIndex = -1, mod1maxIndex = -1, mod2Index = -1, mod2paramIndex = -1, mod2minIndex = -1, mod2maxIndex = -1,
                mod3Index = -1, mod3minIndex = -1, mod3maxIndex = -1, mod4Index = -1, mod4minIndex = -1,
                mod4maxIndex = -1, mod5Index = -1, mod5paramIndex = -1, mod5minIndex = -1, mod5maxIndex = -1, eolIndex = -1;

            // Read existing content and determine column indices
            List<string> lines = new List<string>();
            List<string[]> dataRows = new List<string[]>();

            using (StreamReader reader = new StreamReader(cubePath))
            {
                string line;
                bool isFirstRow = true;

                while ((line = reader.ReadLine()) != null)
                {
                    if (isFirstRow)
                    {
                        string[] columns = line.Split('\t');
                        descriptionIndex = Array.IndexOf(columns, "description");
                        enabledIndex = Array.IndexOf(columns, "enabled");
                        opIndex = Array.IndexOf(columns, "op");
                        paramIndex = Array.IndexOf(columns, "param");
                        valueIndex = Array.IndexOf(columns, "value");
                        inputsIndex = Array.IndexOf(columns, "numinputs");
                        input1Index = Array.IndexOf(columns, "input 1");
                        input2Index = Array.IndexOf(columns, "input 2");
                        outputIndex = Array.IndexOf(columns, "output");
                        mod1Index = Array.IndexOf(columns, "mod 1");
                        mod1minIndex = Array.IndexOf(columns, "mod 1 min");
                        mod1maxIndex = Array.IndexOf(columns, "mod 1 max");
                        mod2Index = Array.IndexOf(columns, "mod 2");
                        mod2paramIndex = Array.IndexOf(columns, "mod 2 param");
                        mod2minIndex = Array.IndexOf(columns, "mod 2 min");
                        mod2maxIndex = Array.IndexOf(columns, "mod 2 max");
                        mod3Index = Array.IndexOf(columns, "mod 3");
                        mod3minIndex = Array.IndexOf(columns, "mod 3 min");
                        mod3maxIndex = Array.IndexOf(columns, "mod 3 max");
                        mod4Index = Array.IndexOf(columns, "mod 4");
                        mod4minIndex = Array.IndexOf(columns, "mod 4 min");
                        mod4maxIndex = Array.IndexOf(columns, "mod 4 max");
                        mod5Index = Array.IndexOf(columns, "mod 5");
                        mod5paramIndex = Array.IndexOf(columns, "mod 5 param");
                        mod5minIndex = Array.IndexOf(columns, "mod 5 min");
                        mod5maxIndex = Array.IndexOf(columns, "mod 5 max");
                        eolIndex = Array.IndexOf(columns, "*eol");

                        if (descriptionIndex == -1 || enabledIndex == -1 || opIndex == -1 || paramIndex == -1 ||
                            valueIndex == -1 || inputsIndex == -1 || input1Index == -1 || input2Index == -1 ||
                            outputIndex == -1 || mod1Index == -1 || mod1minIndex == -1 || mod1maxIndex == -1 ||
                            mod2Index == -1 || mod2paramIndex == -1 || mod2minIndex == -1 || mod2maxIndex == -1 || mod3Index == -1 ||
                            mod3minIndex == -1 || mod3maxIndex == -1 || mod4Index == -1 || mod4minIndex == -1 ||
                            mod4maxIndex == -1 || mod5Index == -1 || mod2paramIndex == -1 || mod5minIndex == -1 || mod5maxIndex == -1 ||
                            eolIndex == -1)
                        {
                            throw new Exception("One or more columns not found in the header row.");
                        }

                        isFirstRow = false;
                        lines.Add(line); // Store the header row
                    }
                    else
                    {
                        string[] columns = line.Split('\t');
                        if (descriptionIndex != -1 && columns.Length > descriptionIndex && columns[descriptionIndex] == "Weapon - Normal -> White")
                            return; // Exit the method as no modifications are needed

                        lines.Add(line); // Store existing rows for later
                    }
                }
            }

            string filePath = Path.Combine(SelectedModDataFolder, "global/excel/itemstatcost.txt");
            string searchTerm = "ColorDye_Tracker";
            string filePath2 = Path.Combine(SelectedModDataFolder, "global/excel/states.txt");
            string searchTerm2 = "Weapon_White";

            int result = SearchItemID(filePath, searchTerm);
            int result2 = SearchStateID(filePath2, searchTerm2);

            // Define the colors and their corresponding codes
            string[] colors0 = { "White", "Black", "Red", "Green", "Blue", "Yellow", "Purple" };
            string[] colors1 = { "Black", "Red", "Green", "Blue", "Yellow", "Purple", "Normal" };
            string[] colors2 = { "Red", "Green", "Blue", "Yellow", "Purple", "White", "Normal" };
            string[] colors3 = { "Green", "Blue", "Yellow", "Purple", "White", "Black", "Normal" };
            string[] colors4 = { "Blue", "Yellow", "Purple", "White", "Black", "Red", "Normal" };
            string[] colors5 = { "Yellow", "Purple", "White", "Black", "Red", "Green", "Normal", };
            string[] colors6 = { "Purple", "White", "Black", "Red", "Green", "Blue", "Normal" };
            string[] colors7 = { "White", "Black", "Red", "Green", "Blue", "Yellow", "Normal" };
            string[] gems0 = { "gpw,qty=3", "skz,qty=3", "gpr,qty=3", "gpg,qty=3", "gpb,qty=3", "gpy,qty=3", "gpv,qty=3" };
            string[] gems1 = { "skz,qty=3", "gpr,qty=3", "gpg,qty=3", "gpb,qty=3", "gpy,qty=3", "gpv,qty=3", "yps,qty=3" };
            string[] gems2 = { "gpr,qty=3", "gpg,qty=3", "gpb,qty=3", "gpy,qty=3", "gpv,qty=3", "gpw,qty=3", "yps,qty=3" };
            string[] gems3 = { "gpg,qty=3", "gpb,qty=3", "gpy,qty=3", "gpv,qty=3", "gpw,qty=3", "skz,qty=3", "yps,qty=3" };
            string[] gems4 = { "gpy,qty=3", "gpv,qty=3", "gpw,qty=3", "skz,qty=3", "gpr,qty=3", "gpg,qty=3", "yps,qty=3" };
            string[] gems5 = { "gpv,qty=3", "gpw,qty=3", "skz,qty=3", "gpr,qty=3", "gpg,qty=3", "gpb,qty=3", "yps,qty=3" };
            string[] gems6 = { "gpv,qty=3", "gpw,qty=3", "skz,qty=3", "gpr,qty=3", "gpg,qty=3", "gpb,qty=3", "yps,qty=3" };
            string[] gems7 = { "gpw,qty=3", "skz,qty=3", "gpr,qty=3", "gpg,qty=3", "gpb,qty=3", "gpy,qty=3", "yps,qty=3" };
            string[] value = { "1", "2", "3", "4", "5", "6", "7" };
            string[] trackerValue0 = { "1", "2", "3", "4", "5", "6", "7" };
            string[] trackerValue1 = { "1", "2", "3", "4", "5", "6", "-1" };
            string[] trackerValue2 = { "1", "2", "3", "4", "5", "-1", "-2" };
            string[] trackerValue3 = { "1", "2", "3", "4", "-2", "-1", "-3" };
            string[] trackerValue4 = { "1", "2", "3", "-3", "-2", "-1", "-4" };
            string[] trackerValue5 = { "1", "2", "-4", "-3", "-2", "-1", "-5" };
            string[] trackerValue6 = { "1", "-5", "-4", "-3", "-2", "-1", "-6" };
            string[] trackerValue7 = { "-6", "-5", "-4", "-3", "-2", "-1", "-7" };
            int[] stateValue0 = { result2, result2 + 1, result2 + 2, result2 + 3, result2 + 4, result2 + 5, result2 + 6 };
            int[] stateValue1 = { result2 + 1, result2 + 2, result2 + 3, result2 + 4, result2 + 5, result2 + 6, result2 };
            int[] stateValue2 = { result2 + 2, result2 + 3, result2 + 4, result2 + 5, result2 + 6, result2, result2 + 1 };
            int[] stateValue3 = { result2 + 3, result2 + 4, result2 + 5, result2 + 6, result2, result2 + 1, result2 + 2 };
            int[] stateValue4 = { result2 + 4, result2 + 5, result2 + 6, result2, result2 + 1, result2 + 2, result2 + 3 };
            int[] stateValue5 = { result2 + 5, result2 + 6, result2, result2 + 1, result2 + 2, result2 + 3, result2 + 4 };
            int[] stateValue6 = { result2 + 6, result2, result2 + 1, result2 + 2, result2 + 3, result2 + 4, result2 + 5 };
            string[] colorDyeProps0 = { "CD_White", "CD_Black", "CD_Red", "CD_Green", "CD_Blue", "CD_Yellow", "CD_Purple" };
            string[] colorDyeProps1 = { "CD_Black", "CD_Red", "CD_Green", "CD_Blue", "CD_Yellow", "CD_Purple", "CD_White" };
            string[] colorDyeProps2 = { "CD_Red", "CD_Green", "CD_Blue", "CD_Yellow", "CD_Purple", "CD_White", "CD_Black" };
            string[] colorDyeProps3 = { "CD_Green", "CD_Blue", "CD_Yellow", "CD_Purple", "CD_White", "CD_Black", "CD_Red" };
            string[] colorDyeProps4 = { "CD_Blue", "CD_Yellow", "CD_Purple", "CD_White", "CD_Black", "CD_Red", "CD_Green" };
            string[] colorDyeProps5 = { "CD_Yellow", "CD_Purple", "CD_White", "CD_Black", "CD_Red", "CD_Green", "CD_Blue" };
            string[] colorDyeProps6 = { "CD_Purple", "CD_White", "CD_Black", "CD_Red", "CD_Green", "CD_Blue", "CD_Yellow" };

            // Define the item types
            string[] itemTypesCode = { "weap", "tors", "helm", "shld" };
            string[] colors = null;
            string[] gems = null;
            string[] colorDyeProps = null;
            string colorDyePropsR = "";
            int iscValue = 0;
            int[] stateValue = null;
            int stateRValue = 0;
            string[] trackerValue = null;


            // Add new rows for each item type
            for (int i = 0; i < 32; i++)
            {

                if (i == 0 || i == 8 || i == 16 || i == 24)
                {
                    colors = colors0;
                    gems = gems0;
                    colorDyeProps = colorDyeProps0;
                    colorDyePropsR = "";
                    iscValue = result;
                    stateValue = stateValue0;
                    trackerValue = trackerValue0;
                }
                else if (i == 1 || i == 9 || i == 17 || i == 25)
                {
                    colors = colors1;
                    gems = gems1;
                    colorDyeProps = colorDyeProps1;
                    colorDyePropsR = "CD_White";
                    iscValue = result + 1;
                    stateValue = stateValue1;
                    stateRValue = result2;
                    trackerValue = trackerValue1;
                }
                else if (i == 2 || i == 10 || i == 18 || i == 26)
                {
                    colors = colors2;
                    gems = gems2;
                    colorDyeProps = colorDyeProps2;
                    colorDyePropsR = "CD_Black";
                    iscValue = result + 2;
                    stateValue = stateValue2;
                    stateRValue = result2 + 1;
                    trackerValue = trackerValue2;
                }
                else if (i == 3 || i == 11 || i == 19 || i == 27)
                {
                    colors = colors3;
                    gems = gems3;
                    colorDyeProps = colorDyeProps3;
                    colorDyePropsR = "CD_Red";
                    iscValue = result + 3;
                    stateValue = stateValue3;
                    stateRValue = result2 + 2;
                    trackerValue = trackerValue3;
                }
                else if (i == 4 || i == 12 || i == 20 || i == 28)
                {
                    colors = colors4;
                    gems = gems4;
                    colorDyeProps = colorDyeProps4;
                    colorDyePropsR = "CD_Green";
                    iscValue = result + 4;
                    stateValue = stateValue4;
                    stateRValue = result2 + 3;
                    trackerValue = trackerValue4;
                }
                else if (i == 5 || i == 13 || i == 21 || i == 29)
                {
                    colors = colors5;
                    gems = gems5;
                    colorDyeProps = colorDyeProps5;
                    colorDyePropsR = "CD_Blue";
                    iscValue = result + 5;
                    stateValue = stateValue5;
                    stateRValue = result2 + 4;
                    trackerValue = trackerValue5;
                }
                else if (i == 6 || i == 14 || i == 22 || i == 30)
                {
                    colors = colors6;
                    gems = gems6;
                    colorDyeProps = colorDyeProps6;
                    colorDyePropsR = "CD_Yellow";
                    iscValue = result + 6;
                    stateValue = stateValue6;
                    stateRValue = result2 + 5;
                    trackerValue = trackerValue6;
                }
                else if (i == 7 || i == 15 || i == 23 || i == 31)
                {
                    colors = colors7;
                    gems = gems7;
                    colorDyeProps = colorDyeProps0;
                    colorDyePropsR = "CD_Purple";
                    iscValue = result + 7;
                    stateValue = stateValue0;
                    stateRValue = result2 + 6;
                    trackerValue = trackerValue7;
                }
                else
                {
                    // Handle the case where i is out of range
                }

                for (int j = 0; j < colors.Length; j++)
                {
                    int maxIndex = Math.Max(descriptionIndex, Math.Max(enabledIndex, Math.Max(opIndex, Math.Max(paramIndex, Math.Max(valueIndex,
                        Math.Max(inputsIndex, Math.Max(input1Index, Math.Max(input2Index, Math.Max(outputIndex,
                        Math.Max(mod1Index, Math.Max(mod1minIndex, Math.Max(mod1maxIndex, Math.Max(mod2Index,
                        Math.Max(mod2paramIndex, Math.Max(mod2minIndex, Math.Max(mod2maxIndex, Math.Max(mod3Index, Math.Max(mod3minIndex,
                        Math.Max(mod3maxIndex, Math.Max(mod4Index, Math.Max(mod4minIndex, Math.Max(mod4maxIndex,
                        Math.Max(mod5Index, Math.Max(mod5paramIndex, Math.Max(mod5minIndex, Math.Max(mod5maxIndex, eolIndex))))))))))))))))))))))))));

                    string[] newRow = new string[maxIndex + 1];

                    if (i == 0)
                        newRow[descriptionIndex] = "Weapon - Normal -> " + colors[j];
                    else if (i == 1)
                        newRow[descriptionIndex] = "Weapon - White -> " + colors[j];
                    else if (i == 2)
                        newRow[descriptionIndex] = "Weapon - Black -> " + colors[j];
                    else if (i == 3)
                        newRow[descriptionIndex] = "Weapon - Red -> " + colors[j];
                    else if (i == 4)
                        newRow[descriptionIndex] = "Weapon - Green -> " + colors[j];
                    else if (i == 5)
                        newRow[descriptionIndex] = "Weapon - Blue -> " + colors[j];
                    else if (i == 6)
                        newRow[descriptionIndex] = "Weapon - Yellow -> " + colors[j];
                    else if (i == 7)
                        newRow[descriptionIndex] = "Weapon - Purple -> " + colors[j];
                    else if (i == 8)
                        newRow[descriptionIndex] = "Torso - Normal -> " + colors[j];
                    else if (i == 9)
                        newRow[descriptionIndex] = "Torso - White -> " + colors[j];
                    else if (i == 10)
                        newRow[descriptionIndex] = "Torso - Black -> " + colors[j];
                    else if (i == 11)
                        newRow[descriptionIndex] = "Torso - Red -> " + colors[j];
                    else if (i == 12)
                        newRow[descriptionIndex] = "Torso - Green -> " + colors[j];
                    else if (i == 13)
                        newRow[descriptionIndex] = "Torso - Blue -> " + colors[j];
                    else if (i == 14)
                        newRow[descriptionIndex] = "Torso - Yellow -> " + colors[j];
                    else if (i == 15)
                        newRow[descriptionIndex] = "Torso - Purple -> " + colors[j];
                    else if (i == 16)
                        newRow[descriptionIndex] = "Helm - Normal -> " + colors[j];
                    else if (i == 17)
                        newRow[descriptionIndex] = "Helm - White -> " + colors[j];
                    else if (i == 18)
                        newRow[descriptionIndex] = "Helm - Black -> " + colors[j];
                    else if (i == 19)
                        newRow[descriptionIndex] = "Helm - Red -> " + colors[j];
                    else if (i == 20)
                        newRow[descriptionIndex] = "Helm - Green -> " + colors[j];
                    else if (i == 21)
                        newRow[descriptionIndex] = "Helm - Blue -> " + colors[j];
                    else if (i == 22)
                        newRow[descriptionIndex] = "Helm - Yellow -> " + colors[j];
                    else if (i == 23)
                        newRow[descriptionIndex] = "Helm - Purple -> " + colors[j];
                    else if (i == 24)
                        newRow[descriptionIndex] = "Shield - Normal -> " + colors[j];
                    else if (i == 25)
                        newRow[descriptionIndex] = "Shield - White -> " + colors[j];
                    else if (i == 26)
                        newRow[descriptionIndex] = "Shield - Black -> " + colors[j];
                    else if (i == 27)
                        newRow[descriptionIndex] = "Shield - Red -> " + colors[j];
                    else if (i == 28)
                        newRow[descriptionIndex] = "Shield - Green -> " + colors[j];
                    else if (i == 29)
                        newRow[descriptionIndex] = "Shield - Blue -> " + colors[j];
                    else if (i == 30)
                        newRow[descriptionIndex] = "Shield - Yellow -> " + colors[j];
                    else if (i == 31)
                        newRow[descriptionIndex] = "Shield - Purple -> " + colors[j];

                    newRow[enabledIndex] = "1";
                    newRow[opIndex] = "18";
                    newRow[paramIndex] = result.ToString();
                    newRow[valueIndex] = (i % 8).ToString();
                    newRow[inputsIndex] = "4";
                    newRow[input1Index] = (i < 8) ? "weap,any" : (i < 16) ? "tors,any" : (i < 24) ? "helm,any" : "shld,any";
                    newRow[input2Index] = gems[j];
                    newRow[outputIndex] = "useitem";
                    newRow[mod1Index] = (i > 0 && (j % 7) == 6) ? "" : colorDyeProps[j];
                    newRow[mod1minIndex] = (i > 0 && (j % 7) == 6) ? "" : "1";
                    newRow[mod1maxIndex] = (i > 0 && (j % 7) == 6) ? "" : "1";
                    newRow[mod2Index] = (i > 0 && (j % 7) == 6) ? "" : "state";
                    newRow[mod2paramIndex] = (i > 0 && (j % 7) == 6) ? "" : (stateValue[j] + (7 * (i / 8))).ToString();
                    newRow[mod2minIndex] = (i > 0 && (j % 7) == 6) ? "" : "1";
                    newRow[mod2maxIndex] = (i > 0 && (j % 7) == 6) ? "" : "1";
                    newRow[mod3Index] = "CD_Tracker";
                    newRow[mod3minIndex] = trackerValue[j];
                    newRow[mod3maxIndex] = trackerValue[j];
                    newRow[mod4Index] = colorDyePropsR;
                    newRow[mod4minIndex] = (i == 0) ? "" : "-1";
                    newRow[mod4maxIndex] = (i == 0) ? "" : "-1";
                    newRow[mod5Index] = (i == 0) ? "" : "state";
                    newRow[mod5paramIndex] = (i == 0) ? "" : (i > 0 && (i % 8) == 0) ? "" : ((7 * (i / 8)) + (result2 + (i % 8)) - 1).ToString();
                    newRow[mod5minIndex] = (i == 0) ? "" : "-1";
                    newRow[mod5maxIndex] = (i == 0) ? "" : "-1";
                    newRow[eolIndex] = "0";

                    dataRows.Add(newRow);
                }
            }


            // Write back to the file
            using (StreamWriter writer = new StreamWriter(cubePath, append: false))
            {
                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }

                // Append the new rows to the file
                foreach (var row in dataRows)
                {
                    writer.WriteLine(string.Join("\t", row));
                }
            }
            //await DyesCube_Torso();
        }

        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }
    }

    private int SearchItemID(string filePath, string searchTerm)
    {
        int result = -1; // Default result if entry not found
        int statColumnIndex = -1; // Index of the "Stat" column
        int idColumnIndex = -1; // Index of the "*ID" column

        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                // Read the header row to determine column indexes
                string[] headers = reader.ReadLine().Split('\t');
                for (int i = 0; i < headers.Length; i++)
                {
                    if (headers[i] == "Stat")
                        statColumnIndex = i;
                    else if (headers[i] == "*ID")
                        idColumnIndex = i;
                }

                if (statColumnIndex == -1 || idColumnIndex == -1)
                {
                    MessageBox.Show("Column headers not found in the file.");
                    return result;
                }

                // Search for the desired entry
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] columns = line.Split('\t');
                    if (columns.Length > statColumnIndex && columns[statColumnIndex] == searchTerm)
                    {
                        if (int.TryParse(columns[idColumnIndex], out result))
                            return result; // Return the *ID as an integer
                        else
                        {
                            MessageBox.Show("Unable to parse *ID as an integer.");
                            return -1; // Return -1 indicating failure
                        }
                    }
                }

                MessageBox.Show($"No entry with '{searchTerm}' found in the Stat column.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }

        return result;
    }

    private int SearchStateID(string filePath, string searchTerm)
    {
        int result2 = -1; // Default result if entry not found
        int stateColumnIndex = -1; // Index of the "Stat" column
        int idColumnIndex = -1; // Index of the "*ID" column

        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                // Read the header row to determine column indexes
                string[] headers = reader.ReadLine().Split('\t');
                for (int i = 0; i < headers.Length; i++)
                {
                    if (headers[i] == "state")
                        stateColumnIndex = i;
                    else if (headers[i] == "*ID")
                        idColumnIndex = i;
                }

                if (stateColumnIndex == -1 || idColumnIndex == -1)
                {
                    MessageBox.Show("Column headers not found in the file.");
                    return result2;
                }

                // Search for the desired entry
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] columns = line.Split('\t');
                    if (columns.Length > stateColumnIndex && columns[stateColumnIndex] == searchTerm)
                    {
                        if (int.TryParse(columns[idColumnIndex], out result2))
                            return result2; // Return the *ID as an integer
                        else
                        {
                            MessageBox.Show("Unable to parse *ID as an integer.");
                            return -1; // Return -1 indicating failure
                        }
                    }
                }

                MessageBox.Show($"No entry with '{searchTerm}' found in the Stat column.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }

        return result2;
    }

    private void RemoveColorDyes(string filePath, string searchString, int rowsToDelete)
    {
        try
        {
            string[] lines = File.ReadAllLines(filePath);

            // Initialize variables to keep track of deletion process
            bool foundSearchString = false;
            int deleteCounter = 0;
            int totalDeleted = 0;

            // Create a new list to store lines to keep
            var linesToKeep = lines.Where(line =>
            {
                if (foundSearchString && deleteCounter < rowsToDelete)
                {
                    deleteCounter++;
                    totalDeleted++;
                    return false; // Exclude this line
                }
                else if (line.StartsWith(searchString))
                {
                    foundSearchString = true;
                    deleteCounter = 0; // Reset delete counter for each match
                    totalDeleted++;
                    return false; // Exclude this line
                }
                return true; // Include this line
            }).ToList();

            // Rewrite the file if any lines were deleted
            if (totalDeleted > 0)
                File.WriteAllLines(filePath, linesToKeep);
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occurred: " + ex.Message);
        }
    }
    #endregion

    #region ---Cinematic Subs---
    private async Task ConfigureCinematicSubs()
    {
        eCinematicSubs cinematicSubs = (eCinematicSubs)UserSettings.CinematicSubs;
        string srtPath = Path.Combine(SelectedModDataFolder, "local/lng/subtitles");
        string profilePath = Path.Combine(SelectedModDataFolder, "global/ui/layouts");

        var languagePaths = new Dictionary<int, string>
    {
        { 0, "enus" }, { 1, "dede" }, { 2, "eses" }, { 3, "esmx" }, { 4, "frfr" }, { 5, "itit" }, { 6, "jajp" },
        { 7, "kokr" }, { 8, "plpl" }, { 9, "ptbr" }, { 10, "ruru" }, { 11, "zhcn" }, { 12, "zhtw" }
    };

        var filesToExtract = new List<string>
    {
        "act02start.srt", "act03start.srt", "act04end.srt", "act04start.srt", "d2intro.srt", "d2x_intro.srt"
    };

        switch (cinematicSubs)
        {
            case eCinematicSubs.Disabled:
                {
                    if (Directory.Exists(srtPath))
                        Directory.Delete(srtPath, true);

                    if (File.Exists(Path.Combine(profilePath, "_profilehd.json")))
                    {
                        string profileContents = File.ReadAllText(Path.Combine(profilePath, "_profilehd.json"));
                        string defaultValue = @"""anchor"": { ""x"": 0.5, ""y"": 0.95 }";
                        string bottomValue = @"""anchor"": { ""x"": 0.5, ""y"": 0.8 }";
                        string updatedProfileContents = profileContents.Replace(defaultValue, bottomValue);

                        File.WriteAllText(Path.Combine(profilePath, "_profilehd.json"), updatedProfileContents);
                    }

                    break;
                }
            case eCinematicSubs.Enabled:
                {
                    if (!Directory.Exists(srtPath))
                        Directory.CreateDirectory(srtPath);
                    if (!Directory.Exists(profilePath))
                        Directory.CreateDirectory(profilePath);

                    if (!File.Exists(Path.Combine(profilePath, "_profilehd.json")))
                        Helper.ExtractFileFromCasc(GamePath, @"data:data\global\ui\layouts\_profilehd.json", SelectedModDataFolder, "data:data");


                    int selectedLanguage = UserSettings.TextLanguage;

                    if (languagePaths.TryGetValue(selectedLanguage, out var languagePath))
                    {
                        if (!File.Exists(Path.Combine(srtPath, $"{languagePath}/act02start.srt")))
                        {
                            foreach (var file in filesToExtract)
                            {
                                var sourcePath = @$"data:data\local\lng\subtitles\{languagePath}\{file}";
                                Helper.ExtractFileFromCasc(GamePath, sourcePath, SelectedModDataFolder, "data:data");
                            }
                        }

                        if (File.Exists(Path.Combine(profilePath, "_profilehd.json")))
                        {
                            string profileContents = File.ReadAllText(Path.Combine(profilePath, "_profilehd.json"));
                            string defaultValue = @"""anchor"": { ""x"": 0.5, ""y"": 0.8 }";
                            string bottomValue = @"""anchor"": { ""x"": 0.5, ""y"": 0.95 }";
                            string updatedProfileContents = profileContents.Replace(defaultValue, bottomValue);

                            File.WriteAllText(Path.Combine(profilePath, "_profilehd.json"), updatedProfileContents);
                        }

                        // Convert SDH to standard subtitles
                        ConvertSDHToStandard(Path.Combine(srtPath, languagePath));
                    }

                    break;
                }
        }
    }

    private void ConvertSDHToStandard(string folderPath)
    {
        var sdhRegex = new Regex(@"\[\s*(?!Marius\s*]|Tyrael\s*]|Mephisto\s*])[^]]*\]");
        var patternRegex = new Regex(@"^\s*\d+\s*$\r?\n^\d{2}:\d{2}:\d{2},\d{3} --> \d{2}:\d{2}:\d{2},\d{3}\r?\n(?:^\s*$\r?)+", RegexOptions.Multiline);

        foreach (var file in Directory.GetFiles(folderPath, "*.srt"))
        {
            string fileContent = File.ReadAllText(file);

            // Replace SDH entries
            string updatedContent = sdhRegex.Replace(fileContent, "");

            // Remove now blank entries
            updatedContent = patternRegex.Replace(updatedContent, "");

            // Renumber IDs sequentially starting from 1
            updatedContent = RenumberIds(updatedContent);

            // Ensure there is never more than one blank line between entries
            updatedContent = NormalizeBlankLines(updatedContent);

            // Write back the updated content to the file
            File.WriteAllText(file, updatedContent);
        }
    }

    private string RenumberIds(string content)
    {
        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var lineNumber = 1;
        var updatedLines = new List<string>();

        foreach (var line in lines)
        {
            if (Regex.IsMatch(line, @"^\s*\d+\s*$"))
            {
                updatedLines.Add(lineNumber.ToString());
                lineNumber++;
            }
            else
            {
                updatedLines.Add(line);
            }
        }

        return string.Join(Environment.NewLine, updatedLines);
    }

    private string NormalizeBlankLines(string content)
    {
        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var updatedLines = new List<string>();

        bool previousLineWasEmpty = false;

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                if (!previousLineWasEmpty)
                {
                    updatedLines.Add(line);
                    previousLineWasEmpty = true;
                }
            }
            else
            {
                updatedLines.Add(line);
                previousLineWasEmpty = false;
            }
        }

        return string.Join(Environment.NewLine, updatedLines);
    }

    public class SubtitleExtractor
    {
        private static readonly List<string> Languages = new List<string>
    {
        "dede", "enus", "eses", "esmx", "frfr", "itit", "jajp", "kokr", "plpl", "ptbr", "ruru", "zhcn", "zhtw"
    };

        private static readonly List<string> Files = new List<string>
    {
        "act02start.srt", "act03start.srt", "act04end.srt", "act04start.srt", "d2intro.srt", "d2x_intro.srt"
    };

        public static void ExtractAllSubtitles(string gamePath, string selectedModDataFolder)
        {
            foreach (var language in Languages)
            {
                foreach (var file in Files)
                {
                    string filePath = $@"data:data\local\lng\subtitles\{language}\{file}";
                    Helper.ExtractFileFromCasc(gamePath, filePath, selectedModDataFolder, "data:data");
                }
            }
        }
    }

    #endregion

    #region ---Auto Backups---
    public async Task StartAutoBackup() //Determine Auto-Backups status and enable timer
    {
        if (UserSettings == null)
        {
            MessageBox.Show("Auto Backup was not started. Could not find User Settings!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        _autoBackupDispatcherTimer?.Stop();

        if ((eBackup)UserSettings.AutoBackups == eBackup.Disabled)
            return;

        if (_autoBackupDispatcherTimer == null)
        {
            _autoBackupDispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            _autoBackupDispatcherTimer.Tick += async (sender, args) =>
            {
                _logger.Info("Auto backup timer ticked.");
                await BackupRecentCharacter();
            };
        }

        //Auto-Backup Timer Intervals
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

    public async Task<List<string>> GetCharacterNames()
    {
        string actualSaveFilePath;
        // Determine if the mod is using a mod folder or retail folder for backups by verifying the directories first
        if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @$"Saved Games\Diablo II Resurrected\Mods\{Settings.Default.SelectedMod}")))
        {
            // The save directory doesn't exist; this mod is using retail location - set default pathing info
            actualSaveFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @$"Saved Games\Diablo II Resurrected\");
        }
        else
        {
            // The save directory exists; this mod is using mod folder locations - proceed normally
            actualSaveFilePath = SaveFilesFilePath;
        }

        List<string> saveFiles = Directory.GetFiles(actualSaveFilePath).ToList();
        List<string> characterNames = new List<string>();
        foreach (string save in saveFiles.Where(s => s.EndsWith(".d2s")))
        {
            characterNames.Add(Path.GetFileNameWithoutExtension(save.Split('\\').Last()));
        }

        return characterNames;
    }

    public async Task<(string characterName, bool passed)> BackupRecentCharacter()
    {
        string mostRecentCharacterName = null;
        string baseSavePath = GetSavePath();
        string actualSaveFilePath;
        string actualBackupFolder;

        try
        {
            // Determine if the mod is using a mod folder or retail folder for backups by verifying the directories first
            if (!Directory.Exists(Path.Combine(baseSavePath, @$"Diablo II Resurrected\Mods\{Settings.Default.SelectedMod}")))
            {
                // The save directory doesn't exist; this mod is using retail location - set default pathing info
                actualSaveFilePath = BaseSaveFilesFilePath;
                actualBackupFolder = Path.Combine(BaseSaveFilesFilePath, "Backups");
            }
            else
            {
                // The save directory exists; this mod is using mod folder locations - proceed normally
                actualSaveFilePath = SaveFilesFilePath;
                actualBackupFolder = BackupFolder;
            }

            // Create backup folder if it doesn't exist yet
            if (!Directory.Exists(actualBackupFolder))
                Directory.CreateDirectory(actualBackupFolder);

            if (new DirectoryInfo(actualSaveFilePath).GetFiles("*.d2s").Length >= 1)
            {
                // Backup Character
                FileInfo mostRecentCharacterFile = new DirectoryInfo(actualSaveFilePath).GetFiles("*.d2s").OrderByDescending(o => o.LastWriteTime).First();
                mostRecentCharacterName = Path.GetFileNameWithoutExtension(mostRecentCharacterFile.Name);

                string mostRecentCharacterBackupFolder = Path.Combine(actualBackupFolder, mostRecentCharacterName);
                if (!Directory.Exists(mostRecentCharacterBackupFolder))
                    Directory.CreateDirectory(mostRecentCharacterBackupFolder);

                File.Copy(mostRecentCharacterFile.FullName, Path.Combine(mostRecentCharacterBackupFolder, mostRecentCharacterFile.Name + DateTime.Now.ToString("_MM_dd--hh_mmtt") + ".d2s"), true);
                _logger.Error($"Auto Backups: Backed up {mostRecentCharacterFile.Name} at {DateTime.Now.ToString("_MM_dd--hh_mmtt")} in {mostRecentCharacterBackupFolder}");

                // Backup Stash
                string mostRecentStashFileSC = Path.Combine(actualSaveFilePath, "SharedStashSoftCoreV2.d2i");
                string mostRecentStashFileHC = Path.Combine(actualSaveFilePath, "SharedStashHardCoreV2.d2i");
                string stashBackupFolder = Path.Combine(actualBackupFolder, "Stash");

                if (!Directory.Exists(stashBackupFolder))
                    Directory.CreateDirectory(stashBackupFolder);

                File.Copy(mostRecentStashFileSC, Path.Combine(stashBackupFolder, "SharedStashSoftCoreV2" + DateTime.Now.ToString("_MM_dd--hh_mmtt") + ".d2i"), true);
                File.Copy(mostRecentStashFileHC, Path.Combine(stashBackupFolder, "SharedStashHardCoreV2" + DateTime.Now.ToString("_MM_dd--hh_mmtt") + ".d2i"), true);
                _logger.Error($"Auto Backups: Backed up Shared Stash files at {DateTime.Now.ToString("_MM_dd--hh_mmtt")} in {mostRecentCharacterBackupFolder}");
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex);
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return (null, false);
        }

        return (mostRecentCharacterName, true);
    }
    public string GetSavePath() //Retrieve save path from Regsitry
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
    #endregion

    #endregion

    #region ---Game Path, BNET and User Settings---
    private async Task<string> GetDiabloInstallPath() //Attempt to find D2R Install Path as defined by Blizzard
    {
        //Check Primary Blizzard path location entry
        RegistryKey gameKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Diablo II Resurrected");
        string installLocation = gameKey?.GetValue("InstallLocation")?.ToString();

        if (installLocation != null)
            return installLocation;

        //Perform an exhaustive search of D2R.exe in Secondary Blizzard path location entry
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

        //Either use parsed result as GamePath or inform user of multiple installs found; possible game migration issue
        switch (results.Count)
        {
            case 1:
                return results[0].Replace(@"\D2R.exe", "");
            case >= 2:
                MessageBox.Show("If you experience mod loading issues, please contact Bonesy in Discord", "Multiple Install Locations found!");
                break;
        }

        return null;
    }
    public void DisableBNetConnection() //Safety measure to avoid accidental online connections
    {
        //Set Battle.Net registry entries to localhost while launcher is open
        RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey(@"Software\Blizzard Entertainment\Battle.net\Launch Options\BNA", true) ?? throw new Exception("Failed to find registry key");
        key.SetValue("CONNECTION_STRING_CN", "127.0.0.1");
        key.SetValue("CONNECTION_STRING_CXX", "127.0.0.1");
        key.SetValue("CONNECTION_STRING_EU", "127.0.0.1");
        key.SetValue("CONNECTION_STRING_KR", "127.0.0.1");
        key.SetValue("CONNECTION_STRING_US", "127.0.0.1");
        key.SetValue("CONNECTION_STRING_XX", "127.0.0.1");
    }
    public async Task SaveUserSettings() //Update user settings based on mod type
    {
        //Protected
        if (Directory.Exists(SelectedModDataFolder))
            await File.WriteAllTextAsync(SelectedUserSettingsFilePath, JsonConvert.SerializeObject(UserSettings));
        //Unprotected
        else
            await File.WriteAllTextAsync(SelectedUserSettingsFilePath, JsonConvert.SerializeObject(UserSettings).Replace($"{Settings.Default.SelectedMod}.mpq/", ""));
    }
    #endregion

    #region ---Save File Functions---
    private async Task RenameCharacter() //Function used to change in-game character name
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

        //Extract raw byte data from specified save file
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

            //Define and string replace byte code for save file (ugly solution)
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

            //Write and convert byte array to byte data back to save file
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
    public static int ParseD2SFile(string filePath) //Read and store .d2s data
    {
        // Ensure the file exists before attempting to read it
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"The file at {filePath} was not found.");

        byte[] fileData = File.ReadAllBytes(filePath);

        if (fileData.Length < 175)
            throw new InvalidDataException("The file is too short to contain the required data.");

        int startIndex = 171;
        byte[] data = new byte[4];
        Array.Copy(fileData, startIndex, data, 0, 4);
        int result = BitConverter.ToInt32(data, 0);

        return result;
    }
    private void FixChecksum(byte[] bytes) //Used in RenameCharacter()
    {
        //Update save file checksum data to match edited content
        new byte[4].CopyTo(bytes, 0xc);
        int checksum = 0;

        for (int i = 0; i < bytes.Length; i++)
        {
            checksum = bytes[i] + (checksum * 2) + (checksum < 0 ? 1 : 0);
        }
        MessageBox.Show(BitConverter.ToString(BitConverter.GetBytes(checksum)));
        BitConverter.GetBytes(checksum).CopyTo(bytes, 0xc);
    }
    #endregion

    #region ---Update Functions---
    private async Task CheckForLauncherUpdates() //Performed after loading to check for D2RLaunch upgrades
    {
        WebClient webClient = new();

        //Download the most recent version info file to compare values
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

        //Read downloaded file and parse entries for comparison
        string[] newVersions = await File.ReadAllLinesAsync(@"..\MyVersions_Temp.txt");

        //If parsed entry does not match appVersion member value, display Update Ready Notification
        if (newVersions[0] != appVersion && (newVersions[0].Length <= 5))
        {
            LauncherUpdateString = $"D2RLaunch Update Ready! ({newVersions[0]})";
            LauncherHasUpdate = true;
        }

        File.Delete(@"..\MyVersions_Temp.txt");
    }
    private async Task CheckForVaultUpdates() //Performed upon launch of Vault
    {
        HttpClient httpClient = new HttpClient();

        if (!File.Exists(@"..\MyVersions_Temp.txt"))
        {
            string primaryLink = "https://drive.google.com/uc?export=download&id=1AW5tOJVpSkWdrXYdjllyPyU3Cpdd-SMV";
            string backupLink = "https://d2filesdrop.s3.us-east-2.amazonaws.com/MyVersions.txt";

            try
            {
                using (var response = await httpClient.GetAsync(primaryLink))
                {
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsByteArrayAsync();
                    await File.WriteAllBytesAsync(@"..\MyVersions_Temp.txt", content);
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.TooManyRequests || ex.StatusCode == HttpStatusCode.InternalServerError)
                {
                    try
                    {
                        using (var response = await httpClient.GetAsync(backupLink))
                        {
                            response.EnsureSuccessStatusCode();
                            var content = await response.Content.ReadAsByteArrayAsync();
                            await File.WriteAllBytesAsync(@"..\MyVersions_Temp.txt", content);
                        }
                    }
                    catch (HttpRequestException)
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

        string[] newVersions = await File.ReadAllLinesAsync(@"..\MyVersions_Temp.txt");
        string SVersion = Directory.GetFiles(@"..\Stasher", "*.xyz").FirstOrDefault()?.Replace(@"..\Stasher\", "").Replace(".xyz", "");

        if (newVersions.Length >= 2 && newVersions[1] != SVersion)
        {
            string primaryLink2 = "https://www.dl.dropboxusercontent.com/scl/fi/m1e8kg5oh334qln8u7i39/D2R_Updater.zip?rlkey=e7o34dut4efpx648x8bq196s8&dl=0";
            string backupLink2 = "https://d2filesdrop.s3.us-east-2.amazonaws.com/D2R_Updater.zip";

            try
            {
                using (var response = await httpClient.GetAsync(primaryLink2))
                {
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsByteArrayAsync();
                    await File.WriteAllBytesAsync(@"..\UpdateU.zip", content);
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.TooManyRequests || ex.StatusCode == HttpStatusCode.InternalServerError)
                {
                    try
                    {
                        using (var response = await httpClient.GetAsync(backupLink2))
                        {
                            response.EnsureSuccessStatusCode();
                            var content = await response.Content.ReadAsByteArrayAsync();
                            await File.WriteAllBytesAsync(@"..\UpdateU.zip", content);
                        }
                    }
                    catch (HttpRequestException)
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

            if (Directory.Exists(@"..\Updater\"))
            {
                Directory.Delete(@"..\Updater\", true);
            }
            Directory.CreateDirectory(@"..\Updater\");
            ZipFile.ExtractToDirectory(@"..\UpdateU.zip", @"..\Updater\");
            File.Delete(@"..\UpdateU.zip");

            File.Create(@"..\Stasher\snu.txt").Close();
            MessageBox.Show($"There is a new version of the vault available!\n\nCurrent Vault Version: {SVersion}\nNew Vault Version: {newVersions[1]}\n\nPlease click OK to begin the update process", "Vault Update Available!");
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
    public async void OnUpdateLauncher() //User has decided to update D2RLaunch; prep external updater program for update
    {
        WebClient webClient = new();

        if (File.Exists("lnu.txt"))
            File.Delete("lnu.txt");

        //Force download of the latest updater program
        string primaryLink2 = "https://www.dl.dropboxusercontent.com/scl/fi/m1e8kg5oh334qln8u7i39/D2R_Updater.zip?rlkey=e7o34dut4efpx648x8bq196s8&dl=0";
        string backupLink2 = "https://d2filesdrop.s3.us-east-2.amazonaws.com/D2R_Updater.zip";
        string baseDir = Path.GetFullPath(@"..\");

        string updaterDir = Path.Combine(baseDir, "Updater");

        // Check and remove the \\?\ prefix if it exists and is not necessary
        if (updaterDir.StartsWith(@"\\?\"))
        {
            updaterDir = updaterDir.Substring(4);
        }

        try
        {
            webClient.DownloadFile(primaryLink2, Path.Combine(baseDir, "UpdateU.zip"));
        }
        catch (WebException ex)
        {
            if (ex.Response is HttpWebResponse response && ((int)response.StatusCode == 429 || (int)response.StatusCode == 500))
            {
                try
                {
                    webClient.DownloadFile(backupLink2, Path.Combine(baseDir, "UpdateU.zip"));
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

        MessageBox.Show(updaterDir);

        //Download complete, remove old files and extract new ones
        if (Directory.Exists(updaterDir))
            Directory.Delete(updaterDir, true);

        Directory.CreateDirectory(updaterDir);
        ZipFile.ExtractToDirectory(Path.Combine(baseDir, "UpdateU.zip"), updaterDir);

        if (File.Exists(Path.Combine(baseDir, "UpdateU.zip")))
            File.Delete(Path.Combine(baseDir, "UpdateU.zip"));

        //Updater has finished extraction, create dummy .txt file to inform updater program of launcher update
        File.Create(Path.Combine(baseDir, @"Launcher\lnu.txt")).Close(); //lnu = Launcher Needs Update
        Process.Start(Path.Combine(baseDir, @"Updater\RMDUpdater.exe"));
        await TryCloseAsync();



        if (File.Exists(Path.Combine(baseDir, "MyVersions_Temp.txt")))
            File.Delete(Path.Combine(baseDir, "MyVersions_Temp.txt"));
    }
    #endregion

    #region ---Event System---
    private async Task CheckForEvents() //Load event window if time has not been tampered
    {

        DateTime systemTime = DateTime.UtcNow;
        DateTime ntpTime = GetNetworkTime();
        TimeSpan difference = systemTime - ntpTime;

        //Check if the difference is greater than or equal to 1 hour
        if (Math.Abs(difference.TotalHours) >= 1)
            MessageBox.Show("Manual Time modification detected!\nAborting Event System...");
        else
        {
            dynamic options = new ExpandoObject();
            options.ResizeMode = ResizeMode.NoResize;
            options.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            SpecialEventsViewModel vm = new SpecialEventsViewModel(this);

            if (await _windowManager.ShowDialogAsync(vm, null, options))
            {
            }
        }
    }
    static DateTime GetNetworkTime() //Compare current time to real time
    {
        const string ntpServer = "time.windows.com";
        var ntpData = new byte[48];
        ntpData[0] = 0x1B; // Leap indicator, version number, mode

        var addresses = Dns.GetHostEntry(ntpServer).AddressList;
        var ipEndPoint = new IPEndPoint(addresses[0], 123);
        using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
        {
            socket.Connect(ipEndPoint);
            socket.ReceiveTimeout = 3000;
            socket.Send(ntpData);
            socket.Receive(ntpData);
        }

        const byte serverReplyTime = 40;
        ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);
        ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

        intPart = SwapEndianness(intPart);
        fractPart = SwapEndianness(fractPart);

        var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
        var networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);

        return networkDateTime;
    }
    static uint SwapEndianness(ulong x) //Reverse byte order
    {
        return (uint)(((x & 0x000000ff) << 24) +
                      ((x & 0x0000ff00) << 8) +
                      ((x & 0x00ff0000) >> 8) +
                      ((x & 0xff000000) >> 24));
    }
    public class Entry //JSON language entry
    {
        public int id { get; set; }
        public string Key { get; set; }
        public string deDE { get; set; }
        public string enUS { get; set; }
        public string esES { get; set; }
        public string esMX { get; set; }
        public string frFR { get; set; }
        public string itIT { get; set; }
        public string jaJP { get; set; }
        public string koKR { get; set; }
        public string plPL { get; set; }
        public string ptBR { get; set; }
        public string ruRU { get; set; }
        public string zhCN { get; set; }
        public string zhTW { get; set; }
    }
    #endregion
}