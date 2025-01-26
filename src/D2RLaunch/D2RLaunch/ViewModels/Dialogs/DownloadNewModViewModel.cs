using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using D2RLaunch.Extensions;
using D2RLaunch.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using JetBrains.Annotations;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace D2RLaunch.ViewModels.Dialogs;

public class DownloadNewModViewModel : Screen
{
    #region ---Static Members---
    private ILog _logger = LogManager.GetLogger(typeof(DownloadNewModViewModel));
    private ObservableCollection<KeyValuePair<string, string>> _mods = new ObservableCollection<KeyValuePair<string, string>>();

    //TODO: Both of these should really be acquired in a more safe way such as querying an API endpoint to get mod info.
    private string _serviceAccountEmail;
    private string _privateKey;
    private KeyValuePair<string, string> _selectedMod;
    private string _modDownloadLink;
    private double _downloadProgress;
    private bool _progressBarIsIndeterminate;
    private string _progressStatus;
    private string _downloadProgressString;

    #endregion

    #region ---Window/Loaded Handlers---

    public DownloadNewModViewModel()
    {
        if (Execute.InDesignMode)
        {
            DownloadProgressString = "70%";
            ProgressStatus = "Test Progress Status...";
            SelectedMod = new KeyValuePair<string, string>("Text Mod", "This is a test string");
            ModDownloadLink = "This is a test string";
        }
    }
    public DownloadNewModViewModel(ShellViewModel shellViewModel)
    {
        DisplayName = "Download A New Mod";
        ShellViewModel = shellViewModel;

        _serviceAccountEmail = ShellViewModel.Configuration["ServiceAccountEmail"] ?? string.Empty;
        _privateKey = ShellViewModel.Configuration["PrivateKey"] ?? string.Empty;

        if (string.IsNullOrEmpty(_serviceAccountEmail) || string.IsNullOrEmpty(_privateKey))
        {
            MessageBox.Show("Please make sure appSettings.json has been properly setup!");
            return;
        }

        Execute.OnUIThread(async () =>
        {
            await GetAvailableMods();
        });
    }

    #endregion

    #region ---Properties---

    public ShellViewModel ShellViewModel { get; }
    public string ProgressStatus
    {
        get => _progressStatus;
        set
        {
            if (value == _progressStatus) return;
            _progressStatus = value;
            NotifyOfPropertyChange();
        }
    }
    public bool ProgressBarIsIndeterminate
    {
        get => _progressBarIsIndeterminate;
        set
        {
            if (value == _progressBarIsIndeterminate) return;
            _progressBarIsIndeterminate = value;
            NotifyOfPropertyChange();
        }
    }
    public string DownloadProgressString
    {
        get => _downloadProgressString;
        set
        {
            if (value == _downloadProgressString) return;
            _downloadProgressString = value;
            NotifyOfPropertyChange();
        }
    }
    public double DownloadProgress
    {
        get => _downloadProgress;
        set
        {
            if (value.Equals(_downloadProgress)) return;
            _downloadProgress = value;
            NotifyOfPropertyChange();
        }
    }
    public string ModDownloadLink
    {
        get => _modDownloadLink;
        set
        {
            if (value == _modDownloadLink) return;
            _modDownloadLink = value;
            NotifyOfPropertyChange();
        }
    }
    public KeyValuePair<string, string> SelectedMod
    {
        get => _selectedMod;
        set
        {
            if (value.Equals(_selectedMod)) return;
            _selectedMod = value;
            NotifyOfPropertyChange();
        }
    }
    public ObservableCollection<KeyValuePair<string, string>> Mods
    {
        get => _mods;
        set
        {
            if (Equals(value, _mods))
            {
                return;
            }
            _mods = value;
            NotifyOfPropertyChange();
        }
    }

    #endregion

    #region ---Mod Download Functions--

    private async Task GetAvailableMods()
    {
        try
        {
            // Create credentials
            ServiceAccountCredential serviceAccountCredential = new(new ServiceAccountCredential.Initializer(_serviceAccountEmail)
                                                                    {
                                                                        Scopes = new[] { SheetsService.Scope.Spreadsheets }
                                                                    }.FromPrivateKey(_privateKey));

            // Create Google Sheets service
            SheetsService sheetsService = new SheetsService(new BaseClientService.Initializer
                                                            {
                                                                HttpClientInitializer = serviceAccountCredential,
                                                                ApplicationName = "D2RLaunch"
                                                            });

            // Define spreadsheetId and ranges
            string spreadsheetId = "1RMqexbqTzxOyjk7tWbLhRYJk9RkzPGJ9cKHSLtsuGII";
            string columnDRange = "Sheet1!D10:D";
            string columnGRange = "Sheet1!G10:G";

            // Fetch values from Google Sheets for column D
            SpreadsheetsResource.ValuesResource.GetRequest request =
                sheetsService.Spreadsheets.Values.Get(spreadsheetId, columnDRange);

            ValueRange response = await request.ExecuteAsync();
            IList<IList<object>> dValues = response.Values;

            // Fetch values from Google Sheets for column G
            SpreadsheetsResource.ValuesResource.GetRequest request2 =
                sheetsService.Spreadsheets.Values.Get(spreadsheetId, columnGRange);

            response = await request2.ExecuteAsync();
            IList<IList<object>> gValues = response.Values;

            if (dValues.Count != gValues.Count)
            {
                MessageBox.Show("The number of items in column D does not match the number of items in column G.\nPlease notify an admin.", "Column Mismatch!", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.Error("The number of items in column D does not match the number of items in column G.");
                return;
            }

            for (int i = 0; i < dValues.Count; i++)
            {
                Mods.Add(new KeyValuePair<string, string>(dValues[i][0].ToString(), gValues[i][0].ToString()));
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            _logger.Error(ex);
        }
    }
    [UsedImplicitly]
    public async void OnInstallMod()
    {
        /*
        if (SelectedMod.Key == null || SelectedMod.Value == null)
        {
            MessageBox.Show("Please select a mod to install.", "No Mod Selected!", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        */

        ModDownloadLink = ModDownloadLink.TrimEnd();

        string tempPath = Path.GetTempPath();
        string tempFile = Path.Combine(tempPath, "NewModDownload.zip");
        string tempExtractedModFolderPath = Path.Combine(tempPath, "NewModDownload");

        try
        {
            Progress<double> progress = new Progress<double>();

            progress.ProgressChanged += (sender, args) =>
                                        {
                                            Execute.OnUIThread(() =>
                                                                          {
                                                                              if (args == -1)
                                                                              {
                                                                                  DownloadProgress = 0;
                                                                                  DownloadProgressString = string.Empty;
                                                                                  ProgressBarIsIndeterminate = true;
                                                                              }
                                                                              else
                                                                              {
                                                                                  DownloadProgress = Math.Round(args, MidpointRounding.AwayFromZero);
                                                                                  DownloadProgressString = $"{DownloadProgress}%";
                                                                              }
                                                                          });
                                        };

            // Seting up the http client used to download the data
            using HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(5);

            // Create a file stream to store the downloaded data.
            // This really can be any type of writeable stream.
            await using FileStream file = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None);
        
            // Use the custom extension method below to download the data.
            // The passed progress-instance will receive the download status updates.
            ProgressStatus = "Downloading mod...";
            //TODO: Add cancellation token

            await Execute.OnUIThreadAsync(async () =>
                                          {
                                              await client.DownloadAsync(ModDownloadLink, file, progress, CancellationToken.None);
                                          });

            file.Close();
            client.Dispose();

            // Extract the downloaded ZIP file
            ProgressStatus = "Extracting mod...";
            DownloadProgressString = string.Empty;
            ProgressBarIsIndeterminate = true;

            if (Directory.Exists(tempExtractedModFolderPath))
            {
                Directory.Delete(tempExtractedModFolderPath, true);
            }

            await Task.Run(() =>
                           {
                               ZipFile.ExtractToDirectory(tempFile, tempExtractedModFolderPath, true);
                               return Task.CompletedTask;
                           });

            string tempModDirPath = await Helper.FindFolderWithMpq(tempExtractedModFolderPath);
            string tempModDir = Path.GetFileName(tempModDirPath);
            string tempParentDir = Path.GetDirectoryName(tempModDirPath);
            string modName = string.Empty;

            if (tempModDir != null)
                modName = tempModDir.Replace(".mpq", "");
            else
            {
                MessageBox.Show("Mod download was unsuccessful", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string modInstallPath = Path.Combine(ShellViewModel.BaseModsFolder, modName);

            //Delete current Mod folder if it exists
            if (Directory.Exists(modInstallPath))
                Directory.Delete(modInstallPath, true);

            ProgressStatus = "Installing mod...";

            await Task.Run(async () =>
                           {
                               await Helper.CloneDirectory(tempParentDir, modInstallPath);
                           });

            string versionPath = Path.Combine(modInstallPath, "version.txt");
            if (!File.Exists(versionPath))
                File.Create(versionPath).Close();

            string tempModInfoPath = Path.Combine(tempModDirPath, "modinfo.json");

            ModInfo modInfo = await Helper.ParseModInfo(tempModInfoPath);

            if (modInfo != null)
                await File.WriteAllTextAsync(versionPath, modInfo.ModVersion);
            else
                MessageBox.Show("Could not parse ModInfo.json!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            //Always clean up temp files.
            if (File.Exists(tempFile))
                File.Delete(tempFile);
            if (Directory.Exists(tempExtractedModFolderPath))
                Directory.Delete(tempExtractedModFolderPath, true);
            ProgressStatus = "Installing Complete!";

            MessageBox.Show($"{modName} has been installed!", "Mod Installed!", MessageBoxButton.OK, MessageBoxImage.None);

            
            //We installed a custom mod from a direct link. 
            if (string.IsNullOrEmpty(SelectedMod.Key))
                SelectedMod = new KeyValuePair<string, string>(modName, "DirectDownload");

            await TryCloseAsync(true);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            _logger.Error(ex);

            //Always clean up temp files.
            if (File.Exists(tempFile))
                File.Delete(tempFile);
            if (Directory.Exists(tempExtractedModFolderPath))
                Directory.Delete(tempExtractedModFolderPath, true);

            await TryCloseAsync(false);
        }
    }
    [UsedImplicitly]
    public async void OnModInstallSelectionChanged()
    {
        if (!string.IsNullOrEmpty(SelectedMod.Value))
            ModDownloadLink = SelectedMod.Value;
    }

    #endregion
}