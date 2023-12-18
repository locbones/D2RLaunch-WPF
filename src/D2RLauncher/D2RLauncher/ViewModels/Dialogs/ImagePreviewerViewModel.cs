using Caliburn.Micro;

namespace D2RLauncher.ViewModels.Dialogs;

public class ImagePreviewerViewModel : Screen
{
    #region members

    private string _imagePath;

    #endregion

    public ImagePreviewerViewModel(string imagePath, string title)
    {
        DisplayName = title;
        _imagePath = imagePath;
    }

    #region properties

    public string ImagePath
    {
        get => _imagePath;
        set
        {
            if (value == _imagePath)
            {
                return;
            }
            _imagePath = value;
            NotifyOfPropertyChange();
        }
    }

    #endregion
}