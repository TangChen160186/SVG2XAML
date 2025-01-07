using System.Configuration;
using System.Data;
using System.Windows;
using Svg2Xaml.Properties;

namespace Svg2Xaml
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 加载保存的主题设置
            var isDarkTheme = Settings.Default.IsDarkTheme;
            var themePath = isDarkTheme 
                ? "pack://application:,,,/Themes/DarkTheme.xaml" 
                : "pack://application:,,,/Themes/LightTheme.xaml";

            // 应用主题
            Resources.MergedDictionaries.Clear();
            Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(themePath) });
        }
    }

}
