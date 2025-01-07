using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Linq;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.Xml;
using ControlzEx.Theming;
using Svg2Xaml.Properties;
using MahApps.Metro.Controls;

namespace Svg2Xaml
{
    public partial class MainWindow : MetroWindow
    {
        private string currentSvgPath;
        private IHighlightingDefinition lightThemeHighlighting;
        private IHighlightingDefinition darkThemeHighlighting;

        public MainWindow()
        {
            InitializeComponent();
            
            // 初始化语法高亮
            InitializeHighlighting();
            
            // 从设置加载主题
            var isDarkTheme = Settings.Default.IsDarkTheme;
            btnTheme.IsChecked = isDarkTheme;
            ApplyTheme(isDarkTheme);
        }

        private void InitializeHighlighting()
        {
            // 加载亮色主题语法高亮
            using (var stream = typeof(MainWindow).Assembly.GetManifestResourceStream("Svg2Xaml.Resources.LightXamlSyntax.xshd"))
            using (var reader = new XmlTextReader(stream))
            {
                lightThemeHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }

            // 加载暗色主题语法高亮
            using (var stream = typeof(MainWindow).Assembly.GetManifestResourceStream("Svg2Xaml.Resources.DarkXamlSyntax.xshd"))
            using (var reader = new XmlTextReader(stream))
            {
                darkThemeHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }

            // 设置编辑器选项
            xamlOutput.Options.EnableHyperlinks = true;
            xamlOutput.Options.EnableEmailHyperlinks = true;
            xamlOutput.Options.ShowSpaces = false;
            xamlOutput.Options.ShowTabs = false;
            xamlOutput.Options.ShowEndOfLine = false;
            xamlOutput.Options.IndentationSize = 4;
            xamlOutput.Options.ConvertTabsToSpaces = true;
        }

        private void btnOpenSvg_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "SVG files (*.svg)|*.svg|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    currentSvgPath = openFileDialog.FileName;
                    
                    // 直接使用 DrawingImage 来预览
                    var drawingImage = SvgConverter.ConvertToDrawingImage(currentSvgPath);
                    svgPreview.Source = drawingImage;
                    
                    // 生成 XAML
                    xamlOutput.Text = FormatXaml(SvgConverter.ConvertToXaml(currentSvgPath));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"加载SVG文件时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnTheme_Click(object sender, RoutedEventArgs e)
        {
            var isDarkTheme = btnTheme.IsChecked ?? false;
            ApplyTheme(isDarkTheme);
        }

        private void ApplyTheme(bool isDarkTheme)
        {
            try
            {
                var theme = isDarkTheme ? "Dark" : "Light";
                var accent = "Cobalt";

                // 应用 MahApps 主题
                ThemeManager.Current.ChangeTheme(this, $"{theme}.{accent}");

                // 更新语法高亮
                xamlOutput.SyntaxHighlighting = isDarkTheme ? darkThemeHighlighting : lightThemeHighlighting;

                // 保存主题设置
                Settings.Default.IsDarkTheme = isDarkTheme;
                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"主题切换失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string FormatXaml(string xaml)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xaml);

                // 创建格式化设置
                var settings = new XmlWriterSettings
                {
                    Indent = true,                    // 启用缩进
                    IndentChars = "    ",            // 使用4个空格作为缩进
                    NewLineChars = Environment.NewLine,  // 使用系统换行符
                    NewLineHandling = NewLineHandling.Replace,
                    OmitXmlDeclaration = true        // 不输出 XML 声明
                };

                // 格式化 XML
                using var stringWriter = new StringWriter();
                using var xmlWriter = XmlWriter.Create(stringWriter, settings);
                doc.WriteTo(xmlWriter);
                xmlWriter.Flush();

                return stringWriter.ToString();
            }
            catch
            {
                return xaml; // 如果格式化失败，返回原始文本
            }
        }

        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaxButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? 
                WindowState.Normal : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}