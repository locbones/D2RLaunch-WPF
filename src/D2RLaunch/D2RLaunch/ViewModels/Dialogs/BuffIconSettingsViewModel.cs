using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using D2RLaunch.Models;
using D2RLaunch.Models.Enums;
using D2RLaunch.ViewModels;
using JetBrains.Annotations;
using Syncfusion.Licensing;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace D2RLaunch.ViewModels.Dialogs
{
    public class BuffIconSettingsViewModel : Screen
    {
        #region members

        private ILog _logger = LogManager.GetLogger(typeof(RestoreBackupViewModel));
        private ObservableCollection<CharacterBuffTemplate> _templates = new ObservableCollection<CharacterBuffTemplate>();
        private CharacterBuffTemplate _selectedTemplate;
        private eBuffIcons _buffIconOne;
        private eBuffIcons _buffIconTwo;
        private eBuffIcons _buffIconThree;
        private eBuffIcons _buffIconFour;
        private eBuffIcons _buffIconFive;
        private eBuffIcons _buffIconSix;
        private eBuffIcons _buffIconSeven;
        private eBuffIcons _buffIconEight;
        private eBuffIcons _buffIconNine;
        private eBuffIcons _buffIconTen;
        private eBuffIcons _buffIconEleven;
        private eBuffIcons _buffIconTwelve;
        private ObservableCollection<KeyValuePair<string, eBuffIcons>> _buffIcons = new ObservableCollection<KeyValuePair<string, eBuffIcons>>();
        private ImageSource _buffIconImageOne;
        private ImageSource _buffIconImageTwo;
        private ImageSource _buffIconImageThree;
        private ImageSource _buffIconImageFour;
        private ImageSource _buffIconImageFive;
        private ImageSource _buffIconImageSix;
        private ImageSource _buffIconImageSeven;
        private ImageSource _buffIconImageEight;
        private ImageSource _buffIconImageNine;
        private ImageSource _buffIconImageTen;
        private ImageSource _buffIconImageEleven;
        private ImageSource _buffIconImageTwelve;

        #endregion

        public BuffIconSettingsViewModel() { }

        public BuffIconSettingsViewModel(ShellViewModel shellViewModel)
        {
            ShellViewModel = shellViewModel;
        }

        #region properties

        public ObservableCollection<KeyValuePair<string, eBuffIcons>> BuffIcons
        {
            get => _buffIcons;
            set
            {
                if (Equals(value, _buffIcons)) return;
                _buffIcons = value;
                NotifyOfPropertyChange();
            }
        }

        public eBuffIcons BuffIconOne
        {
            get => _buffIconOne;
            set
            {
                if (value == _buffIconOne) return;
                _buffIconOne = value;

                Execute.OnUIThread(async () =>
                                   {
                                       BuffIconImageOne = await GetImage(BuffIconOne);
                                   });

                NotifyOfPropertyChange();
            }
        }

        public eBuffIcons BuffIconTwo
        {
            get => _buffIconTwo;
            set
            {
                if (value == _buffIconTwo) return;
                _buffIconTwo = value;

                Execute.OnUIThread(async () =>
                                   {
                                       BuffIconImageTwo = await GetImage(BuffIconTwo);
                                   });

                NotifyOfPropertyChange();
            }
        }

        public eBuffIcons BuffIconThree
        {
            get => _buffIconThree;
            set
            {
                if (value == _buffIconThree) return;
                _buffIconThree = value;

                Execute.OnUIThread(async () =>
                                   {
                                       BuffIconImageThree = await GetImage(BuffIconThree);
                                   });
                NotifyOfPropertyChange();
            }
        }

        public eBuffIcons BuffIconFour
        {
            get => _buffIconFour;
            set
            {
                if (value == _buffIconFour) return;
                _buffIconFour = value;

                Execute.OnUIThread(async () =>
                                   {
                                       BuffIconImageFour = await GetImage(BuffIconFour);
                                   });
                NotifyOfPropertyChange();
            }
        }

        public eBuffIcons BuffIconFive
        {
            get => _buffIconFive;
            set
            {
                if (value == _buffIconFive) return;
                _buffIconFive = value;

                Execute.OnUIThread(async () =>
                                   {
                                       BuffIconImageFive = await GetImage(BuffIconFive);
                                   });
                NotifyOfPropertyChange();
            }
        }

        public eBuffIcons BuffIconSix
        {
            get => _buffIconSix;
            set
            {
                if (value == _buffIconSix) return;
                _buffIconSix = value;

                Execute.OnUIThread(async () =>
                                   {
                                       BuffIconImageSix = await GetImage(BuffIconSix);
                                   });
                NotifyOfPropertyChange();
            }
        }

        public eBuffIcons BuffIconSeven
        {
            get => _buffIconSeven;
            set
            {
                if (value == _buffIconSeven) return;
                _buffIconSeven = value;

                Execute.OnUIThread(async () =>
                                   {
                                       BuffIconImageSeven = await GetImage(BuffIconSeven);
                                   });
                NotifyOfPropertyChange();
            }
        }

        public eBuffIcons BuffIconEight
        {
            get => _buffIconEight;
            set
            {
                if (value == _buffIconEight) return;
                _buffIconEight = value;

                Execute.OnUIThread(async () =>
                                   {
                                       BuffIconImageEight = await GetImage(BuffIconEight);
                                   });
                NotifyOfPropertyChange();
            }
        }

        public eBuffIcons BuffIconNine
        {
            get => _buffIconNine;
            set
            {
                if (value == _buffIconNine) return;
                _buffIconNine = value;

                Execute.OnUIThread(async () =>
                                   {
                                       BuffIconImageNine = await GetImage(BuffIconNine);
                                   });
                NotifyOfPropertyChange();
            }
        }

        public eBuffIcons BuffIconTen
        {
            get => _buffIconTen;
            set
            {
                if (value == _buffIconTen) return;
                _buffIconTen = value;

                Execute.OnUIThread(async () =>
                                   {
                                       BuffIconImageTen = await GetImage(BuffIconTen);
                                   });
                NotifyOfPropertyChange();
            }
        }

        public eBuffIcons BuffIconEleven
        {
            get => _buffIconEleven;
            set
            {
                if (value == _buffIconEleven) return;
                _buffIconEleven = value;

                Execute.OnUIThread(async () =>
                                   {
                                       BuffIconImageEleven = await GetImage(BuffIconEleven);
                                   });
                NotifyOfPropertyChange();
            }
        }

        public eBuffIcons BuffIconTwelve
        {
            get => _buffIconTwelve;
            set
            {
                if (value == _buffIconTwelve) return;
                _buffIconTwelve = value;

                Execute.OnUIThread(async () =>
                                   {
                                       BuffIconImageTwelve = await GetImage(BuffIconTwelve);
                                   });
                NotifyOfPropertyChange();
            }
        }

        public ImageSource BuffIconImageOne
        {
            get => _buffIconImageOne;
            set
            {
                if (Equals(value, _buffIconImageOne)) return;
                _buffIconImageOne = value;
                NotifyOfPropertyChange();
            }
        }

        public ImageSource BuffIconImageTwo
        {
            get => _buffIconImageTwo;
            set
            {
                if (Equals(value, _buffIconImageTwo)) return;
                _buffIconImageTwo = value;
                NotifyOfPropertyChange();
            }
        }

        public ImageSource BuffIconImageThree
        {
            get => _buffIconImageThree;
            set
            {
                if (Equals(value, _buffIconImageThree)) return;
                _buffIconImageThree = value;
                NotifyOfPropertyChange();
            }
        }

        public ImageSource BuffIconImageFour
        {
            get => _buffIconImageFour;
            set
            {
                if (Equals(value, _buffIconImageFour)) return;
                _buffIconImageFour = value;
                NotifyOfPropertyChange();
            }
        }

        public ImageSource BuffIconImageFive
        {
            get => _buffIconImageFive;
            set
            {
                if (Equals(value, _buffIconImageFive)) return;
                _buffIconImageFive = value;
                NotifyOfPropertyChange();
            }
        }

        public ImageSource BuffIconImageSix
        {
            get => _buffIconImageSix;
            set
            {
                if (Equals(value, _buffIconImageSix)) return;
                _buffIconImageSix = value;
                NotifyOfPropertyChange();
            }
        }

        public ImageSource BuffIconImageSeven
        {
            get => _buffIconImageSeven;
            set
            {
                if (Equals(value, _buffIconImageSeven)) return;
                _buffIconImageSeven = value;
                NotifyOfPropertyChange();
            }
        }

        public ImageSource BuffIconImageEight
        {
            get => _buffIconImageEight;
            set
            {
                if (Equals(value, _buffIconImageEight)) return;
                _buffIconImageEight = value;
                NotifyOfPropertyChange();
            }
        }

        public ImageSource BuffIconImageNine
        {
            get => _buffIconImageNine;
            set
            {
                if (Equals(value, _buffIconImageNine)) return;
                _buffIconImageNine = value;
                NotifyOfPropertyChange();
            }
        }

        public ImageSource BuffIconImageTen
        {
            get => _buffIconImageTen;
            set
            {
                if (Equals(value, _buffIconImageTen)) return;
                _buffIconImageTen = value;
                NotifyOfPropertyChange();
            }
        }

        public ImageSource BuffIconImageEleven
        {
            get => _buffIconImageEleven;
            set
            {
                if (Equals(value, _buffIconImageEleven)) return;
                _buffIconImageEleven = value;
                NotifyOfPropertyChange();
            }
        }

        public ImageSource BuffIconImageTwelve
        {
            get => _buffIconImageTwelve;
            set
            {
                if (Equals(value, _buffIconImageTwelve)) return;
                _buffIconImageTwelve = value;
                NotifyOfPropertyChange();
            }
        }

        public ShellViewModel ShellViewModel { get; }

        public ObservableCollection<CharacterBuffTemplate> Templates
        {
            get => _templates;
            set
            {
                if (Equals(value, _templates))
                {
                    return;
                }
                _templates = value;
                NotifyOfPropertyChange();
            }
        }

        public CharacterBuffTemplate SelectedTemplate
        {
            get => _selectedTemplate;
            set
            {
                if (value == _selectedTemplate) return;
                _selectedTemplate = value;
                NotifyOfPropertyChange();
            }
        }

        #endregion

        public async Task Initialize()
        {
            foreach (eBuffIcons buffIcon in Enum.GetValues<eBuffIcons>())
            {
                BuffIcons.Add(new KeyValuePair<string, eBuffIcons>(buffIcon.GetAttributeOfType<DisplayAttribute>().Name, buffIcon));
            }

            await GetTemplates();
        }

        private async Task GetTemplates()
        {
            string templateFileName = "MyBuffTemplates.txt";

            Templates = new ObservableCollection<CharacterBuffTemplate>();

            try
            {
                if (File.Exists(templateFileName))
                {
                    string[] lines = await File.ReadAllLinesAsync(templateFileName);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(':');

                        if (parts.Length == 2)
                        {
                            string characterName = parts[0].Trim();
                            string[] values = parts[1].Split(',');

                            if (values.Length == 12)
                            {
                                List<int> buffValues = new List<int>();
                                foreach (string value in values)
                                {
                                    if (int.TryParse(value, out int intValue))
                                    {
                                        buffValues.Add(intValue);
                                    }
                                    // Handle invalid value if necessary
                                }

                                Templates.Add(new CharacterBuffTemplate { CharacterName = characterName, BuffValues = buffValues });
                            }
                            // Handle invalid line with the wrong number of values
                        }
                    }

                    SelectedTemplate = Templates.FirstOrDefault();

                    if (SelectedTemplate != null)
                    {
                        ShellViewModel.UserSettings.BuffIconTemplate = SelectedTemplate.CharacterName;
                        await LoadTemplate();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async Task LoadTemplate()
        {

            for (int i = 0; i < SelectedTemplate.BuffValues.Count && i < 12; i++)
            {
                switch (i)
                {
                    case 1:
                        BuffIconOne = (eBuffIcons)SelectedTemplate.BuffValues[i];
                        break;
                    case 2:
                        BuffIconTwo = (eBuffIcons)SelectedTemplate.BuffValues[i];
                        break;
                    case 3:
                        BuffIconThree = (eBuffIcons)SelectedTemplate.BuffValues[i];
                        break;
                    case 4:
                        BuffIconFour = (eBuffIcons)SelectedTemplate.BuffValues[i];
                        break;
                    case 5:
                        BuffIconFive = (eBuffIcons)SelectedTemplate.BuffValues[i];
                        break;
                    case 6:
                        BuffIconSix = (eBuffIcons)SelectedTemplate.BuffValues[i];
                        break;
                    case 7:
                        BuffIconSeven = (eBuffIcons)SelectedTemplate.BuffValues[i];
                        break;
                    case 8:
                        BuffIconEight = (eBuffIcons)SelectedTemplate.BuffValues[i];
                        break;
                    case 9:
                        BuffIconNine = (eBuffIcons)SelectedTemplate.BuffValues[i];
                        break;
                    case 10:
                        BuffIconTen = (eBuffIcons)SelectedTemplate.BuffValues[i];
                        break;
                    case 11:
                        BuffIconEleven = (eBuffIcons)SelectedTemplate.BuffValues[i];
                        break;
                }
            }
        }

        private async Task<ImageSource> GetImage(eBuffIcons buffIcon)
        {
            string buffName = string.Empty;
            switch (buffIcon)
            {
                case eBuffIcons.None:
                    buffName = string.Empty;
                    break;
                case eBuffIcons.BattleCommand:
                    buffName = "BattleCommand";
                    break;
                case eBuffIcons.BattleOrders:
                    buffName = "BattleOrders";
                    break;
                case eBuffIcons.BladesOfIce:
                    buffName = "BladesofIce";
                    break;
                case eBuffIcons.Blaze:
                    buffName = "Blaze";
                    break;
                case eBuffIcons.BoneArmor:
                    buffName = "BoneArmor";
                    break;
                case eBuffIcons.BurstOfSpeed:
                    buffName = "BurstofSpeed";
                    break;
                case eBuffIcons.ChillingArmor:
                    buffName = "ChillingArmor";
                    break;
                case eBuffIcons.ClawsOfThunder:
                    buffName = "ClawsofThunder";
                    break;
                case eBuffIcons.CobraStrike:
                    buffName = "CobraStrike";
                    break;
                case eBuffIcons.CycloneArmor:
                    buffName = "CycloneArmor";
                    break;
                case eBuffIcons.Enchant:
                    buffName = "Enchant";
                    break;
                case eBuffIcons.EnergyShield:
                    buffName = "EnergyShield";
                    break;
                case eBuffIcons.Fade:
                    buffName = "Fade";
                    break;
                case eBuffIcons.FistsOfFire:
                    buffName = "FistsofFire";
                    break;
                case eBuffIcons.FrozenArmor:
                    buffName = "FrozenArmor";
                    break;
                case eBuffIcons.HolyShield:
                    buffName = "HolyShield";
                    break;
                case eBuffIcons.PhoenixStrike:
                    buffName = "PhoenixStrike";
                    break;
                case eBuffIcons.ShiverArmor:
                    buffName = "ShiverArmor";
                    break;
                case eBuffIcons.Shout:
                    buffName = "Shout";
                    break;
                case eBuffIcons.ThunderStorm:
                    buffName = "ThunderStorm";
                    break;
                case eBuffIcons.TigerStrike:
                    buffName = "TigerStrike";
                    break;
                case eBuffIcons.Venom:
                    buffName = "Venom";
                    break;
            }

            ImageSource imageSource = null;

            if (!string.IsNullOrEmpty(buffName))
            {
                await Execute.OnUIThreadAsync(async () =>
                                              {
                                                  BitmapImage biImg = new BitmapImage();
                                                  byte[] image = await Helper.GetResourceByteArray($"BuffIcons.Images.Preview_{buffName}.png");
                                                  MemoryStream ms = new MemoryStream(image);
                                                  biImg.BeginInit();
                                                  biImg.StreamSource = ms;
                                                  biImg.EndInit();
                                                  imageSource = biImg;
                                              });
            }

            return imageSource;
        }

        public class CharacterBuffTemplate
        {
            #region properties

            public string CharacterName { get; set; }

            public List<int> BuffValues { get; set; }

            #endregion
        }

        [UsedImplicitly]
        public async void OnSave()
        {
            List<int> selectedIndexes = new List<int>()
                                        {
                                            (int) BuffIconOne,
                                            (int) BuffIconTwo,
                                            (int) BuffIconThree,
                                            (int) BuffIconFour,
                                            (int) BuffIconFive,
                                            (int) BuffIconSix,
                                            (int) BuffIconSeven,
                                            (int) BuffIconEight,
                                            (int) BuffIconNine,
                                            (int) BuffIconTen,
                                            (int) BuffIconEleven,
                                            (int) BuffIconTwelve
                                        };
            try
            {
                string templateFileName = "MyBuffTemplates.txt";
                if (File.Exists(templateFileName))
                {
                    List<string> lines = (await File.ReadAllLinesAsync(templateFileName)).ToList();

                    bool templateExists = false;

                    for (int i = 0; i < lines.Count; i++)
                    {
                        string[] parts = lines[i].Split(':');
                        if (parts.Length == 2)
                        {
                            string existingTemplateName = parts[0].Trim();
                            if (existingTemplateName == SelectedTemplate.CharacterName)
                            {
                                // Update the existing template
                                lines[i] = $"{SelectedTemplate.CharacterName}: {string.Join(",", selectedIndexes)}";
                                templateExists = true;
                                break;
                            }
                        }
                    }

                    if (!templateExists)
                    {
                        // Add a new template
                        lines.Add($"{SelectedTemplate.CharacterName}: {string.Join(",", selectedIndexes)}");
                    }

                    string templateContents = await File.ReadAllTextAsync(templateFileName);
                    if (templateContents.Contains(SelectedTemplate.CharacterName))
                        MessageBox.Show("Template \"" + SelectedTemplate.CharacterName + "\" updated successfully!");
                    else
                        MessageBox.Show("Template Saved as: \"" + SelectedTemplate.CharacterName + "\" successfully!");


                    // Write the updated or new templates back to the file
                    await File.WriteAllLinesAsync(templateFileName, lines);

                    await GetTemplates();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        [UsedImplicitly]
        public async void OnDelete()
        {
            string[] lines = await File.ReadAllLinesAsync("MyBuffTemplates.txt");
            StringBuilder modifiedContent = new StringBuilder();

            foreach (string line in lines)
            {
                if (!line.Contains(SelectedTemplate.CharacterName))
                    modifiedContent.AppendLine(line);
            }
            await File.WriteAllTextAsync("MyBuffTemplates.txt", modifiedContent.ToString());
            MessageBox.Show("Template \"" + SelectedTemplate.CharacterName + "\" deleted successfully!");
            await GetTemplates();
        }
    }
}