﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using MoeLoaderP.Core;

namespace MoeLoaderP.Wpf
{
    public static class UiFunc
    {
        public static string LangText(this FrameworkElement el, string key)
        {
            var text = el.TryFindResource(key) as string;
            return string.IsNullOrWhiteSpace(text) ? text : "{N/A}";
        }

        public static void AddEasyDoubleAnime(this Storyboard sb, DependencyObject target, double fromValue, double toValue, double timeSec, string property)
        {
            var path = "";
            switch (property)
            {
                case "opacity":
                    path = "(UIElement.Opacity)";
                    break;
                case "scale-x":
                    path = "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)";
                    break;
                case "scale-y":
                    path = "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)";
                    break;
                case "x":
                    path = "(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)";
                    break;
                case "y":
                    path = "(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)";
                    break;
            }

            var el = (UIElement)target;
            if (el.RenderTransform == null)
            {
                var group = new TransformGroup {Children = {[3] = new TranslateTransform()}};
                el.RenderTransform = group;
            }


            var animation = new DoubleAnimationUsingKeyFrames
            {
                KeyFrames =
                {
                    new EasingDoubleKeyFrame(fromValue, KeyTime.FromTimeSpan(TimeSpan.Zero)),
                    new EasingDoubleKeyFrame(toValue, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(timeSec)))
                    {
                        EasingFunction = new ExponentialEase {EasingMode = EasingMode.EaseOut}
                    }
                },
            };
            Storyboard.SetTargetProperty(animation, new PropertyPath(path));
            Storyboard.SetTarget(animation, target);
            sb.Children.Add(animation);
        }

        /// <summary>
        /// 获取当前FrameworkElement的Storyboard资源
        /// </summary>
        public static Storyboard Sb(this FrameworkElement element, string key)
        {
            return (Storyboard)element.Resources[key];
        }

        /// <summary>
        /// 设置popup显示动画（右键菜单出现动画等）
        /// </summary>
        public static Storyboard EnlargeShowSb(this FrameworkElement target)
        {
            var sb = new Storyboard();
            sb.AddEasyDoubleAnime(target, 0, 1, 0.3, "opacity");
            return sb;
        }

        public static Storyboard FadeHideSb(this FrameworkElement target)
        {
            var sb = new Storyboard();
            sb.AddEasyDoubleAnime(target, 1, 0, 0.3, "opacity");
            return sb;
        }

        public static void SetBgPos(this Viewbox vb,string filename)
        {
            var pairs = filename.Split(' ');
            foreach (var pair in pairs)
            {
                var pair2 = pair.Split('=');
                if(pair2.Length!=2)continue;
                switch (pair2[0])
                {
                    case "width":
                        var w = pair2[1].ToInt();
                        if (w > 0) vb.Width = w;
                        break;
                    case "height":
                        var h = pair2[1].ToInt();
                        if (h > 0) vb.Height =h;
                        break;
                    case "ha":
                        if (pair2[1] == "left") vb.HorizontalAlignment = HorizontalAlignment.Left;
                        if (pair2[1] == "right") vb.HorizontalAlignment = HorizontalAlignment.Right;
                        if (pair2[1] == "center") vb.HorizontalAlignment = HorizontalAlignment.Center;
                        break;
                }
            }
        }

        public static void GoState(this FrameworkElement fe, params string[] stateNames)
        {
            foreach (var stateName in stateNames)
            {
                VisualStateManager.GoToState(fe, stateName, true);
            }
        }

        public static void GoElementState(this FrameworkElement fe, params string[] stateNames)
        {
            foreach (var stateName in stateNames)
            {
                VisualStateManager.GoToElementState(fe, stateName, true);
            }
        }

        public static void CopyToClipboard(this string text)
        {
            try
            {
                //Clipboard.SetText(text);
                Clipboard.SetDataObject(text);
                Ex.ShowMessage("已复制到剪贴板");
            }
            catch (Exception ex)
            {
                Ex.Log(ex.Message);
                Ex.ShowMessage("复制失败");
            }
        }
    }
}
