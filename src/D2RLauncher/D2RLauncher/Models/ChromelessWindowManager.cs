using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using Syncfusion.SfSkinManager;
using Syncfusion.Themes.MaterialDark.WPF;
using Syncfusion.Windows.Shared;

namespace D2RLauncher.Models;

public class ChromelessWindowManager : WindowManager
{
    protected override Window EnsureWindow(object model, object view, bool isDialog)
    {
        if (view is Window window)
        {
            Window owner = InferOwnerOf(window);
            if (owner != null && isDialog)
            {
                window.Owner = owner;
                window.ResizeMode = ResizeMode.CanResizeWithGrip;
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            }
            else
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            SfSkinManager.SetVisualStyle(window, VisualStyles.MaterialDark);
            SfSkinManager.ApplyStylesOnApplication = true;
        }
        else
        {
            window = new ChromelessWindow {Content = view, SizeToContent = SizeToContent.WidthAndHeight, UseNativeChrome = true};

            SfSkinManager.SetTheme(window, new Theme("MaterialDark"));

            window.SetValue(View.IsGeneratedProperty, true);

            Window owner = InferOwnerOf(window);
            if (owner != null)
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                window.Owner = owner;
            }
            else
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            SfSkinManager.SetVisualStyle(window, VisualStyles.MaterialDark);
            SfSkinManager.ApplyStylesOnApplication = true;
        }

        return window;
    }
}