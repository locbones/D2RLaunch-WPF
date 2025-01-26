﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Caliburn.Micro;
using JetBrains.Annotations;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace D2RLaunch.ViewModels.Dialogs
{
    public class RestoreBackupViewModel : Caliburn.Micro.Screen
    {
        #region ---Static Members---

        private ObservableCollection<string> _characters = new ObservableCollection<string>();
        private ILog _logger = LogManager.GetLogger(typeof(RestoreBackupViewModel));
        private string _selectedCharacter;

        OpenFileDialog openFileDialog = new OpenFileDialog();

        #endregion

        #region ---Window/Loaded Handlers---

        public RestoreBackupViewModel(ShellViewModel shellViewModel)
        {
            DisplayName = "Restore Characters";
            ShellViewModel = shellViewModel;

            Execute.OnUIThread(async () => { await GetCharactersToRestore(); });
        }

        #endregion

        #region ---Properties---

        public ShellViewModel ShellViewModel { get; }
        public string SelectedCharacter
        {
            get => _selectedCharacter;
            set
            {
                if (value == _selectedCharacter)
                {
                    return;
                }
                _selectedCharacter = value;
                NotifyOfPropertyChange();
            }
        }
        public ObservableCollection<string> Characters
        {
            get => _characters;
            set
            {
                if (Equals(value, _characters))
                {
                    return;
                }
                _characters = value;
                NotifyOfPropertyChange();
            }
        }

        #endregion

        #region ---Restore Character--

        private async Task GetCharactersToRestore() //Populate dropbox with retrieved backup file list
        {
            if (Directory.Exists(ShellViewModel.BackupFolder))
            {
                foreach (string character in Directory.GetDirectories(ShellViewModel.BackupFolder))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(character);
                    Characters.Add(directoryInfo.Name);
                }
            }
        }
        [UsedImplicitly]
        public async void OnRestoreBackupSelection() //Open File Dialog to Backup Folder
        {
            openFileDialog.InitialDirectory = Path.Combine(ShellViewModel.BackupFolder, SelectedCharacter);
            openFileDialog.ShowDialog();
            openFileDialog.Filter = "D2 Save Files (*.d2s)|*.d2s";
        }
        [UsedImplicitly]
        public async void OnRestoreBackup() //Restore chosen stash or character file
        {
            string characterPath = Path.Combine(ShellViewModel.BackupFolder, SelectedCharacter);
            string stashPath = Path.Combine(ShellViewModel.BackupFolder, "Stash");

            if (Directory.Exists(characterPath) && (Directory.Exists(stashPath)))
            {
                if (openFileDialog.FileName.Contains(".d2s"))
                {
                    File.Copy(openFileDialog.FileName, Path.Combine(ShellViewModel.SaveFilesFilePath, $"{SelectedCharacter}.d2s"), true);
                    System.Windows.MessageBox.Show($"{SelectedCharacter} Restored!", "Restore Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (openFileDialog.FileName.Contains("SoftCore"))
                        File.Copy(openFileDialog.FileName, Path.Combine(ShellViewModel.SaveFilesFilePath, "SharedStashSoftCoreV2.d2i"), true);
                    else
                        File.Copy(openFileDialog.FileName, Path.Combine(ShellViewModel.SaveFilesFilePath, "SharedStashHardCoreV2.d2i"), true);

                    System.Windows.MessageBox.Show($"Shared Stash Restored!", "Restore Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            await TryCloseAsync();
        }

        #endregion
    }
}