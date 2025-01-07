using System;
using System.IO;
using System.Windows.Media;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using System.Windows.Markup;

namespace Svg2Xaml
{
    public class SvgConverter
    {
        public static string ConvertToXaml(string svgPath)
        {
            var settings = new WpfDrawingSettings
            {
                IncludeRuntime = false,
                TextAsGeometry = true,
                OptimizePath = true,
                EnsureViewboxSize = true,
                EnsureViewboxPosition = true,
                IgnoreRootViewbox = false,
                CultureInfo = System.Globalization.CultureInfo.InvariantCulture,
            };

            try
            {
                var converter = new FileSvgConverter(settings);
                if (!converter.Convert(svgPath))
                {
                    throw new InvalidOperationException("SVG conversion failed");
                }

                var drawing = converter.Drawing;
                if (drawing == null)
                {
                    throw new InvalidOperationException("No drawing was generated");
                }

                var drawingImage = new DrawingImage();
                drawingImage.Drawing = drawing;

                // 使用 XamlWriter 序列化
                return XamlWriter.Save(drawingImage);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to convert SVG file: {svgPath}", ex);
            }
        }

        public static DrawingImage ConvertToDrawingImage(string svgPath)
        {
            var settings = new WpfDrawingSettings
            {
                IncludeRuntime = false,
                TextAsGeometry = true,
                OptimizePath = true,
                EnsureViewboxSize = true,
                EnsureViewboxPosition = true,
                IgnoreRootViewbox = false,
                CultureInfo = System.Globalization.CultureInfo.InvariantCulture,
            };

            try
            {
                var converter = new FileSvgConverter(false,false,settings);
                if (!converter.Convert(svgPath))
                {
                    throw new InvalidOperationException("SVG conversion failed");
                }

                var drawing = converter.Drawing;
                if (drawing == null)
                {
                    throw new InvalidOperationException("No drawing was generated");
                }

                var drawingImage = new DrawingImage();
                drawingImage.Drawing = drawing;
                
                if (drawingImage.CanFreeze)
                {
                    drawingImage.Freeze();
                }

                return drawingImage;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to convert SVG file: {svgPath}", ex);
            }
        }

        public static DrawingImage ConvertFromString(string svgContent)
        {
            var settings = new WpfDrawingSettings
            {
                IncludeRuntime = false,
                TextAsGeometry = true,
                OptimizePath = true,
                EnsureViewboxSize = true,
                EnsureViewboxPosition = true,
                IgnoreRootViewbox = false,
                CultureInfo = System.Globalization.CultureInfo.InvariantCulture
            };

            try
            {
                var tempFile = Path.GetTempFileName();
                File.WriteAllText(tempFile, svgContent);

                try
                {
                    var converter = new FileSvgConverter(settings);
                    if (!converter.Convert(tempFile))
                    {
                        throw new InvalidOperationException("SVG conversion failed");
                    }

                    var drawing = converter.Drawing;
                    if (drawing == null)
                    {
                        throw new InvalidOperationException("No drawing was generated");
                    }

                    var drawingImage = new DrawingImage();
                    drawingImage.Drawing = drawing;
                    
                    if (drawingImage.CanFreeze)
                    {
                        drawingImage.Freeze();
                    }

                    return drawingImage;
                }
                finally
                {
                    try
                    {
                        File.Delete(tempFile);
                    }
                    catch { /* 忽略清理错误 */ }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to convert SVG content", ex);
            }
        }
    }
} 