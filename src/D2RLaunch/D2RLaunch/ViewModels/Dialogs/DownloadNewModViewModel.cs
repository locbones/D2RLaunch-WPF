using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
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
    //TODO: Need to add a "Custom Mod" entry to the mod install drop down and that should dictate if a user and enter/modify the download link.
    #region members
    private ILog _logger = LogManager.GetLogger(typeof(DownloadNewModViewModel));
    private ObservableCollection<KeyValuePair<string, string>> _mods = new ObservableCollection<KeyValuePair<string, string>>();

    //TODO: Both of these should really be acquired in a more safe way such as querying an API endpoint to get mod info.
    private string _serviceAccountEmail = "d2rlaunch@d2rlaunchcore.iam.gserviceaccount.com";
    private string _privateKey = "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCqwvhB5hfC8fK8\n1SVdybxQAgSXKegys3QHsu+xx/Gv+/f70PkuZFt35UODV0385vlk0nu0aohIWmkF\nt/tmyhITSQqLNfWrVoKHfENRVtcaHn788BOCfDXFNsx0anz9FMACM4ZXBe3zPWGr\nfs/qtG+eSQ4eKP1vR8qn/a5Dj2WR26lNUzEPxMzGjPzwLcLqUUPbX1RGiRD1cKrC\noa7QNbzcPedFaOFRhfj1HmQK3H0H63qFCZOXW+yZoVRRMItCoZug4oJFltViU61m\n0rIbGbmdeU3DIAT93mi3o7WYOcrYC/XhVhBI2Fz1qsMKp4XXPzUZhIFEuidKJvv6\nhn+PrzLpAgMBAAECggEAAnwCs6a+2sG9Z9zsBcDNIhbdbTuZWr98pS4HybzgedB/\nK6U/MtsX75cg09Td2BueLkbXsOjJ4c+a7o/eMwEmoSwzYJIg6GTCUmlO62yJhaJC\n87gkeIYJHDzvXZQ9DEuUfZO1VSfLbfoLJT0blk0YwKNMdsje4xMW0jnhIq9/6U7U\nOkKFQRw7z+Hm/DpwZNJPR6rm3fZlqgL/NspKg0fz4Rark713WeGkgK+/YcyHbaYk\neq6lAJz/eSPwOQ/DXWOpV3F1Ide46V8LzHbg70qj153FWyZUPXeT8t7tlFFdMZ6b\ncuk6qDjWfdmYraVmnnpzah/P8MkJMzyy6Fig4Eai+QKBgQDoGa7h3Xmmze7oLxyd\nqEfY/85RCONVys4m2IlBDet1mHfrFEPnf76FNEMTEBgwL5doGMO4FC9bwAJW3BrD\n8nYwTiZ20vOmxYkIwKt7UXYk+fxEzwBmwa1Wil9JjeSUkhG4I7V+r0i2b71zZlMr\n/oEi120Rq/2WZUyQDfB+UoBMVQKBgQC8WFuL1r5ZWtNGyp5V6v4/gOd0k+Ts3FQO\npNM2bohBWNYMulQSBiKiPz7n2vmq5qxec30dn+be8SMfBJgDaasn58wiMDoAitCB\nEnBnNBKygX6nPx1/83syAYmR9wRlINiHKpWK5p4XXtbANMC/XcH+PGR5PGJb7d/i\n/CX8s1AgRQKBgH0zHXsJFU49V9o3T6Bb3iXYF1rvCHKG651YwPEuqQzOKiHM1LRT\n3FnOT0BBNksH4QxuD2WEvecoNBrWsDly2P5Fqcn/ER+s/raR9+6Vir13e/VCFF1Z\nrD86dRwgRmU+RgCmgojL1NVUgUV2tPbOWqqIunUF6czu59XtLwV1S2/hAoGAIjDE\nBaGpEl17hxlXHu+20d5bpf0HDLx+gd4H/ZSZJYuz58GXa2IzvVJP4BUPR6fyWH8M\nkmkppwUNRB84XT48dNUOaJJqpRiN+zBWuVVpo4AAduntOAICNjSzPY0i/hy1Uew4\nE2wD/OgZgfDRoKurgLSD5MJCdL+86d6uIq6GeCUCgYEAlXLZttb0ocGChfzSLQhd\nnXM3uVuxxqCgXaK9ocUDfC7oF0O0Cq9pL8jyUglHkXjDjTTb/Isfb8MQZi1502ew\nfOvhjRvHivwEED3IzDDg3UL6j0h1kkP1cm2rvfAi7ohzR3TnVOdOkUidn/o111If\nskO0qnMmEU8OVCwo3id1W5E=\n-----END PRIVATE KEY-----\n";
    private KeyValuePair<string, string> _selectedMod;
    private string _modDownloadLink;
    private double _downloadProgress;
    private bool _progressBarIsIndeterminate;
    private string _progressStatus;
    private string _downloadProgressString;

    #endregion

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
        Execute.OnUIThread(async () =>
                           {
                               await GetAvailableMods();
                           });
    }

    #region properties

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

    public ShellViewModel ShellViewModel { get; }

    #endregion

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

            Mods.Add(new KeyValuePair<string, string>("Custom", ""));

            for (int i = 0; i < dValues.Count; i++)
            {
                Mods.Add(new KeyValuePair<string, string>(dValues[i][0].ToString(), gValues[i][0].ToString()));
            }

            SelectedMod = Mods[0];
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
                               ZipFile.ExtractToDirectory(tempFile, tempExtractedModFolderPath);
                               return Task.CompletedTask;
                           });

            string tempModDirPath = await Helper.FindFolderWithMpq(tempExtractedModFolderPath);
            string tempModDir = Path.GetFileName(tempModDirPath);
            string tempParentDir = Path.GetDirectoryName(tempModDirPath);
            string modName = string.Empty;

            if (tempModDir != null)
            {
                modName = tempModDir.Replace(".mpq", "");
            }
            else
            {
                MessageBox.Show("Mod download was unsuccessful", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string modInstallPath = Path.Combine(ShellViewModel.BaseModsFolder, modName);

            //Delete current Mod folder if it exists
            if (Directory.Exists(modInstallPath))
            {
                Directory.Delete(modInstallPath, true);
            }

            ProgressStatus = "Installing mod...";

            await Task.Run(async () =>
                           {
                               await Helper.CloneDirectory(tempParentDir, modInstallPath);
                           });

            string versionPath = Path.Combine(modInstallPath, "version.txt");
            if (!File.Exists(versionPath))
            {
                File.Create(versionPath).Close();
            }

            string tempModInfoPath = Path.Combine(tempModDirPath, "modinfo.json");

            ModInfo modInfo = await Helper.ParseModInfo(tempModInfoPath);

            if (modInfo != null)
            {
                await File.WriteAllTextAsync(versionPath, modInfo.ModVersion);
            }
            else
            {
                MessageBox.Show("Could not parse ModInfo.json!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //Always clean up temp files.
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
            if (Directory.Exists(tempExtractedModFolderPath))
            {
                Directory.Delete(tempExtractedModFolderPath, true);
            }
            ProgressStatus = "Installing Complete!";

            MessageBox.Show($"{modName} has been installed!", "Mod Installed!", MessageBoxButton.OK, MessageBoxImage.None);

            //We installed a custom mod from a direct link. 
            if (string.IsNullOrEmpty(SelectedMod.Key) || SelectedMod.Key == "Custom")
            {
                SelectedMod = new KeyValuePair<string, string>(modName, "Custom");
            }

            await TryCloseAsync(true);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            _logger.Error(ex);

            //Always clean up temp files.
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
            if (Directory.Exists(tempExtractedModFolderPath))
            {
                Directory.Delete(tempExtractedModFolderPath, true);
            }

            await TryCloseAsync(false);
        }
    }

    [UsedImplicitly]
    public async void OnModInstallSelectionChanged()
    {
        if (!string.IsNullOrEmpty(SelectedMod.Value))
        {
            ModDownloadLink = SelectedMod.Value;
        }
    }
}