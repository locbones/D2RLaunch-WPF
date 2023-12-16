using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace D2RLauncher.Models;

public class UserSettings : INotifyPropertyChanged
{
    #region members

    private int _audioLanguage;
    private int _autoBackups;
    private int _buffIcons;
    private bool _hdrFix;
    private int _hideHelmets;
    private int _hudDesign;
    private bool _infiniteRespec;
    private int _itemIcons;
    private int _itemIlvls;
    private int _mercIcons;
    private int _monsterStatsDisplay;
    private int _mapLayout;
    private int _personalizedTabs;
    private bool _resetMaps;
    private int _runewordSorting;
    private int _skillIcons;
    private int _textLanguage;
    private int _uiTheme;
    private bool _directTxt;
    private int _personalizedStashTabs;
    private int _font;
    private int _superTelekinesis;
    private int _runeDisplay;

    #endregion

    #region properties

    public bool InfiniteRespec
    {
        get => _infiniteRespec;
        set
        {
            if (value == _infiniteRespec)
            {
                return;
            }
            _infiniteRespec = value;
            OnPropertyChanged();
        }
    }

    public bool ResetMaps
    {
        get => _resetMaps;
        set
        {
            if (value == _resetMaps)
            {
                return;
            }
            _resetMaps = value;
            OnPropertyChanged();
        }
    }

    public int AudioLanguage
    {
        get => _audioLanguage;
        set
        {
            if (value == _audioLanguage)
            {
                return;
            }
            _audioLanguage = value;
            OnPropertyChanged();
        }
    }

    public int TextLanguage
    {
        get => _textLanguage;
        set
        {
            if (value == _textLanguage)
            {
                return;
            }
            _textLanguage = value;
            OnPropertyChanged();
        }
    }

    public int UiTheme
    {
        get => _uiTheme;
        set
        {
            if (value == _uiTheme)
            {
                return;
            }
            _uiTheme = value;
            OnPropertyChanged();
        }
    }

    public int ItemIcons
    {
        get => _itemIcons;
        set
        {
            if (value == _itemIcons)
            {
                return;
            }
            _itemIcons = value;
            OnPropertyChanged();
        }
    }

    public int MercIcons
    {
        get => _mercIcons;
        set
        {
            if (value == _mercIcons)
            {
                return;
            }
            _mercIcons = value;
            OnPropertyChanged();
        }
    }

    public int RunewordSorting
    {
        get => _runewordSorting;
        set
        {
            if (value == _runewordSorting)
            {
                return;
            }
            _runewordSorting = value;
            OnPropertyChanged();
        }
    }

    public int AutoBackups
    {
        get => _autoBackups;
        set
        {
            if (value == _autoBackups)
            {
                return;
            }
            _autoBackups = value;
            OnPropertyChanged();
        }
    }

    public int HudDesign
    {
        get => _hudDesign;
        set
        {
            if (value == _hudDesign)
            {
                return;
            }
            _hudDesign = value;
            OnPropertyChanged();
        }
    }

    public int ItemIlvls
    {
        get => _itemIlvls;
        set
        {
            if (value == _itemIlvls)
            {
                return;
            }
            _itemIlvls = value;
            OnPropertyChanged();
        }
    }

    public int BuffIcons
    {
        get => _buffIcons;
        set
        {
            if (value == _buffIcons)
            {
                return;
            }
            _buffIcons = value;
            OnPropertyChanged();
        }
    }

    public int MonsterStatsDisplay
    {
        get => _monsterStatsDisplay;
        set
        {
            if (value == _monsterStatsDisplay)
            {
                return;
            }
            _monsterStatsDisplay = value;
            OnPropertyChanged();
        }
    }

    public int HideHelmets
    {
        get => _hideHelmets;
        set
        {
            if (value == _hideHelmets)
            {
                return;
            }
            _hideHelmets = value;
            OnPropertyChanged();
        }
    }

    public int PersonalizedTabs
    {
        get => _personalizedTabs;
        set
        {
            if (value == _personalizedTabs)
            {
                return;
            }
            _personalizedTabs = value;
            OnPropertyChanged();
        }
    }

    public bool HdrFix
    {
        get => _hdrFix;
        set
        {
            if (value == _hdrFix)
            {
                return;
            }
            _hdrFix = value;
            OnPropertyChanged();
        }
    }

    public int SkillIcons
    {
        get => _skillIcons;
        set
        {
            if (value == _skillIcons)
            {
                return;
            }
            _skillIcons = value;
            OnPropertyChanged();
        }
    }

    public int MapLayout
    {
        get => _mapLayout;
        set
        {
            if (value == _mapLayout)
            {
                return;
            }
            _mapLayout = value;
            OnPropertyChanged();
        }
    }

    public bool DirectTxt
    {
        get => _directTxt;
        set
        {
            if (value == _directTxt) return;
            _directTxt = value;
            OnPropertyChanged();
        }
    }

    public int PersonalizedStashTabs
    {
        get => _personalizedStashTabs;
        set
        {
            if (value == _personalizedStashTabs) return;
            _personalizedStashTabs = value;
            OnPropertyChanged();
        }
    }

    public int Font
    {
        get => _font;
        set
        {
            if (value == _font) return;
            _font = value;
            OnPropertyChanged();
        }
    }

    public int SuperTelekinesis
    {
        get => _superTelekinesis;
        set
        {
            if (value == _superTelekinesis) return;
            _superTelekinesis = value;
            OnPropertyChanged();
        }
    }

    public int RuneDisplay
    {
        get => _runeDisplay;
        set
        {
            if (value == _runeDisplay) return;
            _runeDisplay = value;
            OnPropertyChanged();
        }
    }

    #endregion

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
}