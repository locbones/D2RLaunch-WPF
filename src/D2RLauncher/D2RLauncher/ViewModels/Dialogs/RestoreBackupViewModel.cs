using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace D2RLauncher.ViewModels.Dialogs
{
    public class RestoreBackupViewModel : Screen
    {
        #region members

        private ObservableCollection<string> _characters = new ObservableCollection<string>();
        private ILog _logger = LogManager.GetLogger(typeof(RestoreBackupViewModel));
        private string _selectedCharacter;

        #endregion

        public RestoreBackupViewModel(ShellViewModel shellViewModel)
        {
            DisplayName = "Restore Characters";
            ShellViewModel = shellViewModel;

            Execute.OnUIThread(async () => { await GetCharactersToRestore(); });
        }

        #region properties

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

        private async Task GetCharactersToRestore()
        {
            if (Directory.Exists(ShellViewModel.BackupFolder))
            {
                foreach (string character in Directory.GetDirectories(ShellViewModel.BackupFolder).Where(f => !f.Contains("Stash")))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(character);
                    Characters.Add(directoryInfo.Name);
                }
            }
        }

        [UsedImplicitly]
        public async void OnRestoreBackup()
        {
            string characterPath = Path.Combine(ShellViewModel.BackupFolder, SelectedCharacter);
            string stashPath = Path.Combine(ShellViewModel.BackupFolder, "Stash");

            if (Directory.Exists(characterPath))
            {
                File.Copy(Path.Combine(characterPath, $"{SelectedCharacter}.d2s"), Path.Combine(ShellViewModel.SaveFilesFilePath, $"{SelectedCharacter}.d2s"), true);
            }

            if (Directory.Exists(stashPath))
            {
                File.Copy(Path.Combine(stashPath, "SharedStashSoftCoreV2.d2i"), Path.Combine(ShellViewModel.SaveFilesFilePath, "SharedStashSoftCoreV2.d2i"), true);
            }

            MessageBox.Show($"{SelectedCharacter} Restored!", "Restore Complete", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}