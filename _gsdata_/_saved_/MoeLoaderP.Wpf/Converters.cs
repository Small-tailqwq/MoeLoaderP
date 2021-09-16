﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MoeLoaderP.Core;

namespace MoeLoaderP.Wpf
{
    public class DoubleToRectConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double width)) return Rect.Empty;
            return Rect.Parse($"0,0,{width},{width}");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ImageSavePathNullConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string path)) return App.MoePicFolder;
            return path.IsEmpty() ? App.MoePicFolder : path;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
    public class ImagesCountVisibilityConvertor : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int count)) return Visibility.Collapsed;
            if (count > 1) return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
    [ValueConversion(typeof(string), typeof(BitmapImage))]
    public class StringToBitmapImageConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is string path))return null;
             return  new BitmapImage(new Uri($"/Assets/SiteIcon/{path}.ico", UriKind.Relative));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
    
    public class BoolToHighLightBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var b = (bool)value;
            var brush = new SolidColorBrush {Color = b ? Colors.HotPink : Colors.White};
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    [ValueConversion(typeof(double), typeof(Thickness))]
    public class OuterWidthToItemMarginConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var defMargin = new Thickness(6,8,6,8);
            var outerWidth = (double) values[0];
            var itemWidth = (double)values[1];
            var countd = outerWidth / (itemWidth + 12d);
            var count = (int)countd;
            if (count == 0) return defMargin;
            var duoyude = (itemWidth + 12d) * (countd % 1);
            var mar = duoyude / count / 2d + 6d;
            return new Thickness(mar, 8d, mar, 8d);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
    }

    [ValueConversion(typeof(bool?), typeof(bool?))]
    public sealed class BoolReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (bool?) value; 
            return !v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (bool?) value;
            return !v;
        }
    }

    [ValueConversion(typeof(ulong), typeof(string))]
    public class UlongToFileSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var size =  value as ulong? ?? 0;
            if (size == 0) return null;
            var temp = size / 1024d;
            if (temp < 1024) return $"{Math.Round(temp)}kB";
            temp /= 1024d;
            return $"{Math.Round(temp, 2)}MB";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    [ValueConversion(typeof(double), typeof(Visibility))]
    public class DoubleToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;
            var i = (int)(double) value;
            return i == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    [ValueConversion(typeof(int), typeof(Visibility))]
    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;
            var i = (int)value;
            return i == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    [ValueConversion(typeof(bool?), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (bool?)value;
            switch (v)
            {
                case true:
                    return Visibility.Visible;
                case false:
                    return Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    [ValueConversion(typeof(object), typeof(Visibility))]
    public class ObjectToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
    
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = (string)value;
            if ((parameter as string) == "reverse")
            {
                return text.IsEmpty() ? Visibility.Visible : Visibility.Collapsed;
            }

            return text.IsEmpty() ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    [ValueConversion(typeof(ulong), typeof(Visibility))]
    public class UlongToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = (ulong)value;
            if ((parameter as string) == "reverse")
            {
                return text==0 ? Visibility.Visible : Visibility.Collapsed;
            }

            return text==0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    [ValueConversion(typeof(Settings.ProxyModeEnum), typeof(int))]
    public class ProxyModeToSelectIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null) throw new Exception("badvalue");
            var index = (int) (Settings.ProxyModeEnum) value;
            return index;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) throw new Exception("badvalue");
            var e = (Settings.ProxyModeEnum) (int) value;
            return e;
        }
    }

}
