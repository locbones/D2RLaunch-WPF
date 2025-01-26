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
using JetBrains.Annotations;
using Syncfusion.Licensing;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace D2RLaunch.ViewModels.Dialogs
{
    public class BuffIconSettingsViewModel : Screen
    {
        #region ---Static Members---

        private ILog _logger = LogManager.GetLogger(typeof(RestoreBackupViewModel));
        private ObservableCollection<CharacterBuffTemplate> _templates = new ObservableCollection<CharacterBuffTemplate>();
        private CharacterBuffTemplate _selectedTemplate;
        private int _buffIconOne = -1;
        private int _buffIconTwo = -1;
        private int _buffIconThree = -1;
        private int _buffIconFour = -1;
        private int _buffIconFive = -1;
        private int _buffIconSix = -1;
        private int _buffIconSeven = -1;
        private int _buffIconEight = -1;
        private int _buffIconNine = -1;
        private int _buffIconTen = -1;
        private int _buffIconEleven = -1;
        private int _buffIconTwelve = -1;
        private ObservableCollection<KeyValuePair<string, int>> _buffIcons = new ObservableCollection<KeyValuePair<string, int>>();
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

        private List<string> _buffIconNames = new List<string>();

        private readonly List<string> _classes = new List<string>()
                                                 {
                                                     "Amazon",
                                                     "Assassin",
                                                     "Barbarian",
                                                     "Druid",
                                                     "Necromancer",
                                                     "Paladin",
                                                     "Sorceress"
                                                 };

        private string _buffIconLocation;

        #endregion

        #region ---Window/Loaded Handlers---

        public async Task Initialize()
        {
            if (!Helper.ResourceNamespaceExists($"Resources.BuffIcons.{ShellViewModel.ModInfo.Name}"))
            {
                _buffIconLocation = "Resources.BuffIcons.Default";
                _buffIconNames = (await Helper.GetResourceFileNames($"Resources.BuffIcons.Default")).Select(s => s.Replace("Preview_", "").Replace(".png", "")).ToList();
            }
            else
            {
                _buffIconLocation = $"Resources.BuffIcons.{ShellViewModel.ModInfo.Name}";
                _buffIconNames = (await Helper.GetResourceFileNames($"Resources.BuffIcons.{ShellViewModel.ModInfo.Name}")).Select(s => s.Replace("Preview_", "").Replace(".png", "")).ToList();
            }

            foreach (eBuffIcons buffIcon in Enum.GetValues<eBuffIcons>())
            {
                string name = buffIcon.GetAttributeOfType<DisplayAttribute>().Name;

                if (_buffIconNames.Contains(name.Replace(" ", "")))
                {
                    BuffIcons.Add(new KeyValuePair<string, int>(name, (int) buffIcon));
                }
            }

            await GetTemplates();
        }
        public BuffIconSettingsViewModel() { }
        public BuffIconSettingsViewModel(ShellViewModel shellViewModel)
        {
            ShellViewModel = shellViewModel;
        }

        #endregion

        #region ---Properties---

        public ShellViewModel ShellViewModel { get; }
        public ObservableCollection<KeyValuePair<string, int>> BuffIcons
        {
            get => _buffIcons;
            set
            {
                if (Equals(value, _buffIcons)) return;
                _buffIcons = value;
                NotifyOfPropertyChange();
            }
        }

        //TODO: These BuffIcon index (int) and images should really be put in some type of dictionary and remove all of these properties. ELB
        public int BuffIconOne
        {
            get => _buffIconOne;
            set
            {
                if (value == _buffIconOne) return;
                _buffIconOne = value;

                Execute.OnUIThread(async () =>
                                   {
                                       BuffIconImageOne = await GetImage((eBuffIcons)BuffIconOne);
                                   });

                NotifyOfPropertyChange();
            }
        }

        public int BuffIconTwo
        {
            get => _buffIconTwo;
            set
            {
                if (value == _buffIconTwo) return;
                _buffIconTwo = value;

                Execute.OnUIThread(async () =>
                                   {
                                       BuffIconImageTwo = await GetImage((eBuffIcons)BuffIconTwo);
                                   });

                NotifyOfPropertyChange();
            }
        }

        public int BuffIconThree
        {
            get => _buffIconThree;
            set
            {
                if (value == _buffIconThree) return;
                _buffIconThree = value;

                Execute.OnUIThread(async () =>
                                   {
                                       BuffIconImageThree = await GetImage((eBuffIcons)BuffIconThree);
                                   });
                NotifyOfPropertyChange();
            }
        }

        public int BuffIconFour
        {
            get => _buffIconFour;
            set
            {
                if (value == _buffIconFour) return;
                _buffIconFour = value;

                Execute.OnUIThread(async () =>
                                   {
                                       BuffIconImageFour = await GetImage((eBuffIcons)BuffIconFour);
                                   });
                NotifyOfPropertyChange();
            }
        }

        public int BuffIconFive
        {
            get => _buffIconFive;
            set
            {
                if (value == _buffIconFive) return;
                _buffIconFive = value;

                Execute.OnUIThread(async () =>
                                   {
                                       BuffIconImageFive = await GetImage((eBuffIcons)BuffIconFive);
                                   });
                NotifyOfPropertyChange();
            }
        }

        public int BuffIconSix
        {
            get => _buffIconSix;
            set
            {
                if (value == _buffIconSix) return;
                _buffIconSix = value;

                Execute.OnUIThread(async () =>
                                   {
                                       BuffIconImageSix = await GetImage((eBuffIcons)BuffIconSix);
                                   });
                NotifyOfPropertyChange();
            }
        }

        public int BuffIconSeven
        {
            get => _buffIconSeven;
            set
            {
                if (value == _buffIconSeven) return;
                _buffIconSeven = value;

                Execute.OnUIThread(async () =>
                                   {
                                       BuffIconImageSeven = await GetImage((eBuffIcons)BuffIconSeven);
                                   });
                NotifyOfPropertyChange();
            }
        }

        public int BuffIconEight
        {
            get => _buffIconEight;
            set
            {
                if (value == _buffIconEight) return;
                _buffIconEight = value;

                Execute.OnUIThread(async () =>
                                   {
                                       BuffIconImageEight = await GetImage((eBuffIcons)BuffIconEight);
                                   });
                NotifyOfPropertyChange();
            }
        }

        public int BuffIconNine
        {
            get => _buffIconNine;
            set
            {
                if (value == _buffIconNine) return;
                _buffIconNine = value;

                Execute.OnUIThread(async () =>
                                   {
                                       BuffIconImageNine = await GetImage((eBuffIcons)BuffIconNine);
                                   });
                NotifyOfPropertyChange();
            }
        }

        public int BuffIconTen
        {
            get => _buffIconTen;
            set
            {
                if (value == _buffIconTen) return;
                _buffIconTen = value;

                Execute.OnUIThread(async () =>
                                   {
                                       BuffIconImageTen = await GetImage((eBuffIcons)BuffIconTen);
                                   });
                NotifyOfPropertyChange();
            }
        }

        public int BuffIconEleven
        {
            get => _buffIconEleven;
            set
            {
                if (value == _buffIconEleven) return;
                _buffIconEleven = value;

                Execute.OnUIThread(async () =>
                                   {
                                       BuffIconImageEleven = await GetImage((eBuffIcons)BuffIconEleven);
                                   });
                NotifyOfPropertyChange();
            }
        }

        public int BuffIconTwelve
        {
            get => _buffIconTwelve;
            set
            {
                if (value == _buffIconTwelve) return;
                _buffIconTwelve = value;

                Execute.OnUIThread(async () =>
                                   {
                                       BuffIconImageTwelve = await GetImage((eBuffIcons)BuffIconTwelve);
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

                Execute.OnUIThread(async () =>
                                   {
                                       await LoadTemplate();
                                   });
            }
        }

        #endregion

        private async Task GetTemplates()
        {
            string templateFileName = "DefaultBuffTemplates.txt";

            Templates = new ObservableCollection<CharacterBuffTemplate>();

            try
            {
                if (!File.Exists(templateFileName))
                {
                    var fileByteArray = await Helper.GetResourceByteArray(templateFileName);

                    await File.WriteAllBytesAsync(templateFileName, fileByteArray);
                }


                if (File.Exists(templateFileName))
                {
                    string[] lines = await File.ReadAllLinesAsync(templateFileName);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(':');

                        if (parts.Length == 2)
                        {
                            string templateName = parts[0].Trim();
                            string[] values = parts[1].Split(',');

                            if (values.Length == 12)
                            {
                                List<int> buffValues = new List<int>();
                                foreach (string value in values)
                                {
                                    if (int.TryParse(value.Trim(), out int intValue))
                                    {
                                        buffValues.Add(intValue);
                                    }
                                    // Handle invalid value if necessary
                                }

                                Templates.Add(new CharacterBuffTemplate { TemplateName = templateName, BuffValues = buffValues });
                            }
                            // Handle invalid line with the wrong number of values
                        }
                    }
                }

                SelectedTemplate = Templates.FirstOrDefault();

                if (SelectedTemplate != null)
                {
                    ShellViewModel.UserSettings.BuffIconTemplate = SelectedTemplate.TemplateName;
                    await LoadTemplate();
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
                    case 0:
                        BuffIconOne = SelectedTemplate.BuffValues[i];
                        break;
                    case 1:
                        BuffIconTwo = SelectedTemplate.BuffValues[i];
                        break;
                    case 2:
                        BuffIconThree = SelectedTemplate.BuffValues[i];
                        break;
                    case 3:
                        BuffIconFour = SelectedTemplate.BuffValues[i];
                        break;
                    case 4:
                        BuffIconFive = SelectedTemplate.BuffValues[i];
                        break;
                    case 5:
                        BuffIconSix = SelectedTemplate.BuffValues[i];
                        break;
                    case 6:
                        BuffIconSeven = SelectedTemplate.BuffValues[i];
                        break;
                    case 7:
                        BuffIconEight = SelectedTemplate.BuffValues[i];
                        break;
                    case 8:
                        BuffIconNine = SelectedTemplate.BuffValues[i];
                        break;
                    case 9:
                        BuffIconTen = SelectedTemplate.BuffValues[i];
                        break;
                    case 10:
                        BuffIconEleven = SelectedTemplate.BuffValues[i];
                        break;
                    case 11:
                        BuffIconTwelve = SelectedTemplate.BuffValues[i];
                        break;
                }
            }
        }

        private async Task<ImageSource> GetImage(eBuffIcons buffIcon)
        {
            string buffName = buffIcon.GetAttributeOfType<DisplayAttribute>().Name.Replace(" ", "");

            ImageSource imageSource = null;

            if (!string.IsNullOrEmpty(buffName))
            {
                await Execute.OnUIThreadAsync(async () =>
                                              {
                                                  BitmapImage biImg = new BitmapImage();
                                                  byte[] image = await Helper.GetResourceByteArray($"{_buffIconLocation}.Preview_{buffName}.png");
                                                  MemoryStream ms = new MemoryStream(image);
                                                  biImg.BeginInit();
                                                  biImg.StreamSource = ms;
                                                  biImg.EndInit();
                                                  imageSource = biImg;
                                              });
            }

            return imageSource;
        }

        [UsedImplicitly]
        public async void OnSave()
        {
            if (string.IsNullOrEmpty(SelectedTemplate.TemplateName))
            {
                MessageBox.Show("Template can not be empty!");
                return;
            }

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

                if (!File.Exists(templateFileName))
                {
                    File.Create(templateFileName).Close();
                }


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
                            if (existingTemplateName == SelectedTemplate.TemplateName)
                            {
                                // Update the existing template
                                lines[i] = $"{SelectedTemplate.TemplateName}: {string.Join(",", selectedIndexes)}";
                                templateExists = true;
                                break;
                            }
                        }
                    }

                    if (!templateExists)
                    {
                        // Add a new template
                        lines.Add($"{SelectedTemplate.TemplateName}: {string.Join(",", selectedIndexes)}");
                    }

                    string templateContents = await File.ReadAllTextAsync(templateFileName);
                    if (templateContents.Contains(SelectedTemplate.TemplateName))
                        MessageBox.Show("Template \"" + SelectedTemplate.TemplateName + "\" updated successfully!");
                    else
                        MessageBox.Show("Template Saved as: \"" + SelectedTemplate.TemplateName + "\" successfully!");


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
                if (!line.Contains(SelectedTemplate.TemplateName))
                    modifiedContent.AppendLine(line);
            }
            await File.WriteAllTextAsync("MyBuffTemplates.txt", modifiedContent.ToString());
            MessageBox.Show("Template \"" + SelectedTemplate.TemplateName + "\" deleted successfully!");
            await GetTemplates();
        }

    }
    public class CharacterBuffTemplate
    {
        #region ---Properties---

        public string TemplateName { get; set; }
        public List<int> BuffValues { get; set; }

        #endregion
    }
}