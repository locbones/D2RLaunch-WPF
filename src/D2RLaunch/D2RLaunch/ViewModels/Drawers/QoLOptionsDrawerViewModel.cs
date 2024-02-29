using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using System.Dynamic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
    public async void OnExpanded_Inventory()
    {
        CreateExpandedDirs();
        DownloadExpandedZip();

        string inventoryFilePath = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/excel/inventory.txt");

        // Create needed folders and files
        if (!File.Exists(inventoryFilePath))
            Helper.ExtractFileFromCasc(ShellViewModel.GamePath, @"data:data\global\excel\inventory.txt", ShellViewModel.SelectedModDataFolder, "data:data");

        // Read lines from the inventory file
        string[] lines = File.ReadAllLines(inventoryFilePath);

        // Iterate through lines to edit values
        for (int index = 0; index < lines.Length; index++)
        {
            string line = lines[index];
            string[] splitContent = line.Split('\t');

            // Locate Class Rows
            if (index == 1 || index == 2 || index == 3 || index == 4 || index == 5 || index == 14 || index == 15 || index == 16 || index == 17 || index == 18 || index == 19 || index == 20 || index == 28 || index == 29)
            {
                if (!ShellViewModel.UserSettings.ExpandedInventory)
                {
                    splitContent[5] = "10"; // Edit XGrid
                    splitContent[6] = "8";  // Edit YGrid
                    lines[index] = String.Join("\t", splitContent);
                }
                else
                {
                    splitContent[5] = "10"; // Edit XGrid
                    splitContent[6] = "4";  // Edit YGrid
                    lines[index] = String.Join("\t", splitContent);
                }
            }
        }

        // Write modified lines back to the file
        File.WriteAllLines(inventoryFilePath, lines);

        //Write Json and Sprite Assets
        string KBMDir = ShellViewModel.SelectedModDataFolder + "/D2RLaunch/Expanded/Inventory/inventory"; // Path of the source directory
        string KBMDestDir = ShellViewModel.SelectedModDataFolder + "/hd/global/ui/panel/inventory"; // Path of the destination directory
        string ContDir = ShellViewModel.SelectedModDataFolder + "/D2RLaunch/Expanded/Inventory/inventorypanel"; // Path of the source directory
        string ContDestDir = ShellViewModel.SelectedModDataFolder + "/hd/global/ui/controller/panel/inventorypanel"; // Path of the destination directory

        if (!ShellViewModel.UserSettings.ExpandedInventory)
        {
            try
            {
                CopyDirectory(KBMDir, KBMDestDir);
                CopyDirectory(ContDir, ContDestDir);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            File.Copy(ShellViewModel.SelectedModDataFolder + "/D2RLaunch/Expanded/Inventory/playerinventoryoriginallayouthd.json", ShellViewModel.SelectedModDataFolder + "/global/ui/layouts/playerinventoryoriginallayouthd.json", true);
            File.Copy(ShellViewModel.SelectedModDataFolder + "/D2RLaunch/Expanded/Inventory/playerinventoryexpansionlayouthd.json", ShellViewModel.SelectedModDataFolder + "/global/ui/layouts/playerinventoryexpansionlayouthd.json", true);
        }
        else
        {
            if (Directory.Exists(KBMDestDir))
            {
                File.Delete(ShellViewModel.SelectedModDataFolder + "/global/ui/layouts/playerinventoryoriginallayouthd.json");
                File.Delete(ShellViewModel.SelectedModDataFolder + "/global/ui/layouts/playerinventoryexpansionlayouthd.json");
                Directory.Delete(ContDestDir, true);
                Directory.Delete(KBMDestDir, true);
            } 
        }
    }

    [UsedImplicitly]
    public async void OnExpanded_Stash()
    {
        CreateExpandedDirs();
        DownloadExpandedZip();

        string StashFile = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/excel/inventory.txt");

        //Create needed folders and files
        if (!File.Exists(StashFile))
            Helper.ExtractFileFromCasc(ShellViewModel.GamePath, @"data:data\global\excel\inventory.txt", ShellViewModel.SelectedModDataFolder, "data:data");

        //Defin tab seperators, input file, index counter, etc
        string[] lines = File.ReadAllLines(StashFile);
        string outstr = "";
        string sep = "\t";
        int index = 0;

        //Iterate through file to edit values
        foreach (string line in lines)
        {
            string[] splitContent = line.Split(sep.ToCharArray());

            //Locate Class Rows
            if (index == 9 || index == 12 || index == 24 || index == 26)
            {
                if (!ShellViewModel.UserSettings.ExpandedStash)
                {
                    splitContent[5] = "16"; //Edit XGrid
                    splitContent[6] = "13"; //Edit YGrid
                    outstr += String.Join("\t", splitContent) + "\n";
                }
                else
                {
                    splitContent[5] = "10"; //Edit XGrid
                    splitContent[6] = "10"; //Edit YGrid
                    outstr += String.Join("\t", splitContent) + "\n";
                }
            }
            else
                outstr += line + "\n";
            index += 1;
        }
        File.WriteAllText(StashFile, outstr);

        //Write Json and Sprite Assets
        string KBMDir = ShellViewModel.SelectedModDataFolder + "/D2RLaunch/Expanded/Stash/stash"; // Path of the source directory
        string KBMDestDir = ShellViewModel.SelectedModDataFolder + "/hd/global/ui/panel/stash"; // Path of the destination directory
        string ContDir = ShellViewModel.SelectedModDataFolder + "/D2RLaunch/Expanded/Stash/stashc"; // Path of the source directory
        string ContDestDir = ShellViewModel.SelectedModDataFolder + "/hd/global/ui/controller/panel/stash"; // Path of the destination directory

        if (!ShellViewModel.UserSettings.ExpandedStash)
        {
            try
            {
                CopyDirectory(KBMDir, KBMDestDir);
                CopyDirectory(ContDir, ContDestDir);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            File.Copy(ShellViewModel.SelectedModDataFolder + "/D2RLaunch/Expanded/Stash/bankoriginallayouthd.json", ShellViewModel.SelectedModDataFolder + "/global/ui/layouts/bankoriginallayouthd.json", true);
            File.Copy(ShellViewModel.SelectedModDataFolder + "/D2RLaunch/Expanded/Stash/bankexpansionlayouthd.json", ShellViewModel.SelectedModDataFolder + "/global/ui/layouts/bankexpansionlayouthd.json", true);
        }
        else
        {
            if (Directory.Exists(KBMDestDir))
            {
                File.Delete(ShellViewModel.SelectedModDataFolder + "/global/ui/layouts/bankoriginallayouthd.json");
                File.Delete(ShellViewModel.SelectedModDataFolder + "/global/ui/layouts/bankexpansionlayouthd.json");
                Directory.Delete(ContDestDir, true);
                Directory.Delete(KBMDestDir, true);
            }
        }
    }

    [UsedImplicitly]
    public async void OnExpanded_Cube()
    {
        CreateExpandedDirs();
        DownloadExpandedZip();

        string CubeFile = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/excel/inventory.txt");

        //Create needed folders and files
        if (!File.Exists(CubeFile))
            Helper.ExtractFileFromCasc(ShellViewModel.GamePath, @"data:data\global\excel\inventory.txt", ShellViewModel.SelectedModDataFolder, "data:data");

        //Defin tab seperators, input file, index counter, etc
        string[] lines = File.ReadAllLines(CubeFile);
        string outstr = "";
        string sep = "\t";
        int index = 0;

        //Iterate through file to edit values
        foreach (string line in lines)
        {
            string[] splitContent = line.Split(sep.ToCharArray());

            //Locate Class Rows
            if (index == 10 || index == 25)
            {
                if (!ShellViewModel.UserSettings.ExpandedCube)
                {
                    splitContent[5] = "16"; //Edit XGrid
                    splitContent[6] = "13"; //Edit YGrid
                    outstr += String.Join("\t", splitContent) + "\n";
                }
                else
                {
                    splitContent[5] = "3"; //Edit XGrid
                    splitContent[6] = "4"; //Edit YGrid
                    outstr += String.Join("\t", splitContent) + "\n";
                }
            }
            else
                outstr += line + "\n";
            index += 1;
        }
        File.WriteAllText(CubeFile, outstr);

        //Write Json and Sprite Assets
        string KBMDir = ShellViewModel.SelectedModDataFolder + "/D2RLaunch/Expanded/Cube/horadric_cube"; // Path of the source directory
        string KBMDestDir = ShellViewModel.SelectedModDataFolder + "/hd/global/ui/panel/horadric_cube"; // Path of the destination directory
        string ContDir = ShellViewModel.SelectedModDataFolder + "/D2RLaunch/Expanded/Cube/horadriccube"; // Path of the source directory
        string ContDestDir = ShellViewModel.SelectedModDataFolder + "/hd/global/ui/controller/panel/horadriccube"; // Path of the destination directory

        if (!ShellViewModel.UserSettings.ExpandedCube)
        {
            try
            {
                CopyDirectory(KBMDir, KBMDestDir);
                CopyDirectory(ContDir, ContDestDir);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            File.Copy(ShellViewModel.SelectedModDataFolder + "/D2RLaunch/Expanded/Cube/horadriccubelayouthd.json", ShellViewModel.SelectedModDataFolder + "/global/ui/layouts/horadriccubelayouthd.json", true);
        }
        else
        {
            if (Directory.Exists(KBMDestDir))
            {
                File.Delete(ShellViewModel.SelectedModDataFolder + "/global/ui/layouts/horadriccubelayouthd.json");
                Directory.Delete(ContDestDir, true);
                Directory.Delete(KBMDestDir, true);
            }
        }
    }

    [UsedImplicitly]
    public async Task OnExpanded_Merc()
    {
        CreateExpandedDirs();
        DownloadExpandedZip();

        string mercFilePath = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/excel/inventory.txt");

        //Create needed folders and files
        if (!File.Exists(mercFilePath))
            Helper.ExtractFileFromCasc(ShellViewModel.GamePath, @"data:data\global\excel\inventory.txt", ShellViewModel.SelectedModDataFolder, "data:data");

        //Define tab separators
        string sep = "\t";
        StringBuilder sb = new StringBuilder();

        //Clone Amazon and Amazon 2 Entry
        string[] lines = File.ReadAllLines(mercFilePath);
        string lineToCopy = lines[1];
        string lineToCopy2 = lines[16];
        if (!ShellViewModel.UserSettings.ExpandedMerc)
        {
            lines[13] = lineToCopy;
            lines[27] = lineToCopy2;
        }

        //Update Name Entries and ItemTypes.txt Edit
        int index = 0;
        foreach (string line in lines)
        {
            string[] splitContent = line.Split(sep.ToCharArray());

            if (!ShellViewModel.UserSettings.ExpandedMerc)
            {
                if (index == 13)
                    splitContent[0] = "Hireling";
                else if (index == 27)
                    splitContent[0] = "Hireling2";
            }
            else
            {
                if (index == 13 || index == 27)
                {
                    for (int i = 1; i <= 10; i++)
                        splitContent[i] = "-1";
                }
            }
            sb.AppendLine(string.Join("\t", splitContent));
            index++;
        }
        File.WriteAllText(mercFilePath, sb.ToString());

        //Begin ItemTypes.txt Edit
        await ExpandedItemTypesEdit();

        //Write Json and Sprite Assets
        string KBMDir = ShellViewModel.SelectedModDataFolder + "/D2RLaunch/Expanded/Merc/hireling"; // Path of the source directory
        string KBMDestDir = ShellViewModel.SelectedModDataFolder + "/hd/global/ui/panel/hireling"; // Path of the destination directory
        string ContDir = ShellViewModel.SelectedModDataFolder + "/D2RLaunch/Expanded/Merc/hirelinginventory"; // Path of the source directory
        string ContDestDir = ShellViewModel.SelectedModDataFolder + "/hd/global/ui/controller/panel/hirelinginventory"; // Path of the destination directory

        if (!ShellViewModel.UserSettings.ExpandedMerc)
        {
            try
            {
                CopyDirectory(KBMDir, KBMDestDir);
                CopyDirectory(ContDir, ContDestDir);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            File.Copy(ShellViewModel.SelectedModDataFolder + "/D2RLaunch/Expanded/Merc/hirelinginventorypanelhd.json", ShellViewModel.SelectedModDataFolder + "/global/ui/layouts/hirelinginventorypanelhd.json", true);
        }
        else
        {
            if (Directory.Exists(KBMDestDir))
            {
                File.Delete(ShellViewModel.SelectedModDataFolder + "/global/ui/layouts/hirelinginventorypanelhd.json");
                Directory.Delete(ContDestDir, true);
                Directory.Delete(KBMDestDir, true);
            }
        }
    }


    [UsedImplicitly]
    public async void OnBackup()
    {
        (string characterName, bool passed) result = await ShellViewModel.BackupRecentCharacter();

        if (result.passed)
            MessageBox.Show($"{result.characterName} and your Shared Stash has been backed up successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        else
            MessageBox.Show("Failed to backup character!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            await UpdateFontPreview();
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
        options.WindowStartupLocation = WindowStartupLocation.CenterOwner;

        ImagePreviewerViewModel vm = new ImagePreviewerViewModel($"pack://application:,,,/Resources/Preview/{imageName}", title);

        if (await _windowManager.ShowDialogAsync(vm, null, options))
        {  
        }
    }

    public async Task ExpandedItemTypesEdit()
    {
        string itemTypesFile = Path.Combine(ShellViewModel.SelectedModDataFolder, "global/excel/itemtypes.txt");

        if (!File.Exists(itemTypesFile))
            Helper.ExtractFileFromCasc(ShellViewModel.GamePath, @"data:data\global\excel\itemtypes.txt", ShellViewModel.SelectedModDataFolder, "data:data");

        //Check to see if we already added the skill previously
        bool mercEquipExists = false;
        using (StreamReader reader = new StreamReader(itemTypesFile))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] columns = line.Split('\t');
                if (columns.Length > 0 && columns[0] == "Merc Equipment")
                {
                    mercEquipExists = true;
                    break;
                }
            }
        }

        if (!mercEquipExists && !ShellViewModel.UserSettings.ExpandedMerc)
        {
            //Let's clone the helm itemtype
            int mercID = 38; //ID of Helm
            string[] lines = File.ReadAllLines(itemTypesFile);
            int lineCount = lines.Length;

            //Expand the table by 1 row and clone the helm entry to it
            if (mercID < lineCount)
            {
                string lineToCopy = lines[mercID];
                Array.Resize(ref lines, lineCount + 1);
                lines[lineCount] = lineToCopy;
                lineCount++;
                File.WriteAllLines(itemTypesFile, lines);
            }

            //Helm has been cloned now; let's edit it
            string sep = "\t";
            StringBuilder sb = new StringBuilder();
            int index = 0;

            foreach (string line in lines)
            {
                string[] splitContent = line.Split(sep.ToCharArray());

                if (index == lineCount - 1)
                {
                    splitContent[0] = "Merc Equipment"; //Update Clone Name
                    splitContent[3] = "merc"; //Assign Clone Itemtype 
                }
                else if (index == 11 || index == 13 || index == 16 || index == 17 || index == 20)
                    splitContent[3] = "merc"; //Add new itemtype to Rings, Amulets, Gloves, Belts and Boots
                else if (index == 38)
                {
                    splitContent[1] = "merc"; //Re-Classify helms as new itemtype
                    splitContent[2] = ""; //Remove Equiv2 Entry
                }

                //Fill in non-matching rows
                sb.AppendLine(string.Join("\t", splitContent));
                index++;
            }
            File.WriteAllText(itemTypesFile, sb.ToString());
        }
        else if (mercEquipExists && ShellViewModel.UserSettings.ExpandedMerc)
        {
            string[] lines = File.ReadAllLines(itemTypesFile);
            int lineCount = lines.Length;

            //Reduce the table by 1 row and re-write it
            if (lineCount > 0)
            {
                Array.Resize(ref lines, lineCount - 1);
                File.WriteAllLines(itemTypesFile, lines);
                lineCount--;
            }

            //Let's undo our previous expanded merc edits
            string sep = "\t";
            StringBuilder sb = new StringBuilder();
            int index = 0;

            foreach (string line in lines)
            {
                string[] splitContent = line.Split(sep.ToCharArray());

                if (index == 11 || index == 13 || index == 16 || index == 17 || index == 20)
                    splitContent[3] = ""; //Remove new itemtype to Rings, Amulets, Gloves, Belts and Boots
                else if (index == 38)
                {
                    splitContent[1] = "helm"; //Return to helm type
                    splitContent[2] = "armo"; //Return to armor type
                }

                //Fill in non-matching rows
                sb.AppendLine(string.Join("\t", splitContent));
                index++;
            }
            File.WriteAllText(itemTypesFile, sb.ToString());
        }
    }

    static void CopyDirectory(string sourceDir, string destinationDir)
    {
        if (!Directory.Exists(destinationDir))
            Directory.CreateDirectory(destinationDir);

        // Copy each file into the new directory
        foreach (string file in Directory.GetFiles(sourceDir))
        {
            string fileName = Path.GetFileName(file);
            string destinationFile = Path.Combine(destinationDir, fileName);
            File.Copy(file, destinationFile);
        }

        // Recursively copy each subdirectory
        foreach (string subDir in Directory.GetDirectories(sourceDir))
        {
            string subDirName = Path.GetFileName(subDir);
            string destinationSubDir = Path.Combine(destinationDir, subDirName);
            CopyDirectory(subDir, destinationSubDir);
        }
    }

    public async void DownloadExpandedZip()
    {
        string url = "https://drive.google.com/uc?export=download&id=11W5rbVwZCB6TkU8O1YNkj9iQ0xCi0Jxg"; // URL of the file to download
        string savePath = "Expanded.zip"; // Path where the file will be saved
        string extractPath = ShellViewModel.SelectedModDataFolder + "/D2RLaunch/Expanded/"; // Path to extract the contents

        if (!Directory.Exists(extractPath))
        {
            if (MessageBox.Show("You don't have the required files for this feature!\nWould you like to download and extract them now?\n\n(It will download in the background and display when it's complete)", "Missing Files!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFileCompleted += (sender, e) =>
                    {
                        try
                        {
                            if (e.Error == null)
                            {
                                ZipFile.ExtractToDirectory(savePath, extractPath);
                                MessageBox.Show("Expanded Storage setup successfully!\nYou may now toggle your desired options.");
                            }
                            else
                                MessageBox.Show($"An error occurred during download: {e.Error.Message}");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"An error occurred: {ex.Message}");
                        }
                    };

                    try
                    {
                        client.DownloadFileAsync(new Uri(url), savePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                    }
                }
            }
        }
    }

    public async void CreateExpandedDirs()
    {
        //Create needed directories if they don't exist
        if (!Directory.Exists(ShellViewModel.SelectedModDataFolder + "/global/ui/layouts"))
            Directory.CreateDirectory(ShellViewModel.SelectedModDataFolder + "/global/ui/layouts");

        if (!Directory.Exists(ShellViewModel.SelectedModDataFolder + "/hd/global/ui/panel"))
            Directory.CreateDirectory(ShellViewModel.SelectedModDataFolder + "/hd/global/ui/panel");

        if (!Directory.Exists(ShellViewModel.SelectedModDataFolder + "/hd/global/ui/panel/controller"))
            Directory.CreateDirectory(ShellViewModel.SelectedModDataFolder + "/hd/global/ui/panel/controller");
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
}