﻿using MoeLoaderP.Core;
using MoeLoaderP.Core.Sites;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MoeLoaderP.Wpf.ControlParts
{
    /// <summary>
    /// 搜索控件
    /// </summary>
    public partial class SearchControl : INotifyPropertyChanged
    {
        private MoeSite _currentSelectedSite;

        public SiteManager SiteManager { get; set; }

        public MoeSite CurrentSelectedSite
        {
            get => _currentSelectedSite;
            set
            {
                _currentSelectedSite = value;
                OnPropertyChanged(nameof(CurrentSelectedSite));
            }
        }

        public Settings Settings { get; set; }
        public AutoHintItems CurrentHintItems { get; set; } = new();

        public SearchControl()
        {
            InitializeComponent();
        }

        public void Init(SiteManager manager, Settings settings)
        {
            SiteManager = manager;
            MoeSitesLv1ComboBox.ItemsSource = SiteManager.Sites;
            Settings = settings;
            DataContext = Settings;

            ShowExlicitOnlyCheckBox.Checked += (_, _) => FilterExlicitCheckBox.IsChecked = true;
            FilterExlicitCheckBox.Unchecked += (_, _) => ShowExlicitOnlyCheckBox.IsChecked = false;

            KeywordTextBox.TextChanged += KeywordTextBoxOnTextChanged;
            KeywordTextBox.GotFocus += (_, _) => KeywordPopup.IsOpen = true;
            KeywordTextBox.LostFocus += (_, _) => KeywordPopup.IsOpen = false;
            KeywordListBox.ItemsSource = CurrentHintItems;
            KeywordListBox.SelectionChanged += KeywordComboBoxOnSelectionChanged;

            SiteManager.Sites.CollectionChanged += SitesOnCollectionChanged;

            MoeSitesLv1ComboBox.SelectionChanged += MoeSitesLv1ComboBoxOnSelectionChanged;// 一级菜单选择改变
            MoeSitesLv2ComboBox.SelectionChanged += MoeSitesLv2ComboBoxOnSelectionChanged;// 二级菜单选择改变
            MoeSitesLv3ComboBox.SelectionChanged += MoeSitesLv3ComboBoxOnSelectionChanged;// 三级菜单选择改变
            MoeSitesLv4ComboBox.SelectionChanged += MoeSitesLv4ComboBoxOnSelectionChanged;// 四级菜单选择改变


            MoeSitesLv1ComboBox.SelectedIndex = 0;

            AccountButton.MouseRightButtonUp += AccountButtonOnMouseRightButtonUp;
        }
        private void AccountButtonOnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            CurrentSelectedSite.SiteSettings.LoginCookies = null;
            Ex.ShowMessage("已清除登录信息！");
        }

        private void SitesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (MoeSitesLv1ComboBox.SelectedIndex == -1) MoeSitesLv1ComboBox.SelectedIndex = 0;
        }

        private void VisualUpdate()
        {
            AdaptSupportState(CurrentSelectedSite.SupportState);
            var lv2 = CurrentSelectedSite.SubCategories;
            if (MoeSitesLv2ComboBox.SelectedIndex != -1 && lv2.Any())
            {
                var lv2ItemCat = CurrentSelectedSite
                    .SubCategories[MoeSitesLv2ComboBox.SelectedIndex];
                AdaptSupportState(lv2ItemCat.OverrideSupportState);
                var lv3 = lv2[MoeSitesLv2ComboBox.SelectedIndex].SubCategories;
                if (MoeSitesLv3ComboBox.SelectedIndex != -1 && lv3.Any())
                {
                    var lv3ItemCat = CurrentSelectedSite
                        .SubCategories[MoeSitesLv2ComboBox.SelectedIndex]
                        .SubCategories[MoeSitesLv3ComboBox.SelectedIndex];
                    AdaptSupportState(lv3ItemCat.OverrideSupportState);
                    var lv4 = lv3[MoeSitesLv3ComboBox.SelectedIndex].SubCategories;
                    if (MoeSitesLv4ComboBox.SelectedIndex != -1 && lv4.Any())
                    {
                        var lv4ItemCat = CurrentSelectedSite
                            .SubCategories[MoeSitesLv2ComboBox.SelectedIndex]
                            .SubCategories[MoeSitesLv3ComboBox.SelectedIndex]
                            .SubCategories[MoeSitesLv4ComboBox.SelectedIndex];
                        AdaptSupportState(lv4ItemCat.OverrideSupportState);
                    }
                }
            }
        }

        public MoeSiteSupportState CurrentSupportState { get; set; }

        public void AdaptSupportState(MoeSiteSupportState state)
        {
            if(state == null) return;
            CurrentSupportState = state;
            this.GoState(state.IsSupportAccount ? nameof(ShowAccountButtonState) : nameof(HideAccountButtonState));
            this.GoState(state.IsSupportDatePicker ? nameof(ShowDatePickerState) : nameof(HideDatePickerState));
            this.GoState(state.IsSupportKeyword ?  nameof(SurportKeywordState) : nameof(NotSurportKeywordState));
        }

        public void FilterBoxVisualUpdate()
        {
            FilterResolutionCheckBox.IsEnabled = CurrentSelectedSite.SupportState.IsSupportResolution;
            FilterExlicitGroup.IsEnabled = CurrentSelectedSite.SupportState.IsSupportRating;
            DownloadTypeComboBox.ItemsSource = CurrentSelectedSite.DownloadTypes;
            DownloadTypeComboBox.SelectedIndex = 0;
            FilterStartIdGrid.Visibility = CurrentSelectedSite.SupportState.IsSupportSearchByImageLastId ? Visibility.Visible : Visibility.Collapsed;
            FilterStartIdBox.MaxCount = 0;
            FilterStartPageBox.NumCount = 1;
        }

        public void Refresh()
        {
            MoeSitesLv1ComboBoxOnSelectionChanged(null, null);
        }
        
        private void MoeSitesLv1ComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)// site change
        {
            var lv1Si = MoeSitesLv1ComboBox.SelectedIndex;
            if (lv1Si == -1) return;
            CurrentSelectedSite = SiteManager.Sites[lv1Si];
            InitSearch();
            var lv2 = CurrentSelectedSite.SubCategories;
            if (lv2.Any())
            {
                MoeSitesLv2ComboBox.ItemsSource = lv2;
                if (MoeSitesLv2ComboBox.SelectedIndex == 0) MoeSitesLv2ComboBoxOnSelectionChanged(sender, e);
                else MoeSitesLv2ComboBox.SelectedIndex = 0;
                MoeSitesLv2ComboBox.SelectedIndex = 0;
                this.GoState(nameof(ShowSubMenuState));
            }
            else
            {
                this.GoState(nameof(HideSubMenuState));
                this.GoState(nameof(HideLv3MenuState));
                this.GoState(nameof(HideLv4MenuState));
            }
            VisualUpdate();
            FilterBoxVisualUpdate();
        }
        private void MoeSitesLv2ComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lv2Si = MoeSitesLv2ComboBox.SelectedIndex;
            if (lv2Si == -1) return;
            var lv3 = CurrentSelectedSite.SubCategories[lv2Si].SubCategories;
            if (lv3.Any())
            {
                MoeSitesLv3ComboBox.ItemsSource = lv3;
                if (MoeSitesLv3ComboBox.SelectedIndex == 0) MoeSitesLv3ComboBoxOnSelectionChanged(sender, e);
                else MoeSitesLv3ComboBox.SelectedIndex = 0;
                this.GoState(nameof(ShowLv3MenuState));
            }
            else
            {
                this.GoState(nameof(HideLv3MenuState));
                this.GoState(nameof(HideLv4MenuState));
            }

            VisualUpdate();
        }
        private void MoeSitesLv3ComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lv3Si = MoeSitesLv3ComboBox.SelectedIndex;
            if (lv3Si == -1) return;
            var lv4 = CurrentSelectedSite.SubCategories[MoeSitesLv2ComboBox.SelectedIndex].SubCategories[lv3Si].SubCategories;
            if (lv4.Any())
            {
                MoeSitesLv4ComboBox.ItemsSource = lv4;
                if (MoeSitesLv4ComboBox.SelectedIndex == 0) MoeSitesLv4ComboBoxOnSelectionChanged(sender, e);
                else MoeSitesLv4ComboBox.SelectedIndex = 0;
                this.GoState(nameof(ShowLv4MenuState));
            }
            else this.GoState(nameof(HideLv4MenuState));
            VisualUpdate();
        }
        private void MoeSitesLv4ComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e) { VisualUpdate(); }


        private async void KeywordTextBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (CurrentHintTaskCts == null)
            {
                this.Sb("SearchingSpinSb").Begin();
            }

            if (CurrentHintTaskCts != null)
            {
                CurrentHintTaskCts.Cancel();
                if (KeywordTextBox.Text.Length == 0)
                {
                    this.Sb("SearchingSpinSb").Stop();
                }
            }
            
            CurrentHintTaskCts = new CancellationTokenSource();

            var tempCts = CurrentHintTaskCts;
            try
            {
                await ShowKeywordComboBoxItemsAsync(KeywordTextBox.Text, tempCts.Token);
                this.Sb("SearchingSpinSb").Stop();
            }
            catch (TaskCanceledException)
            {
                if (tempCts.Equals(CurrentHintTaskCts))
                {
                    this.Sb("SearchingSpinSb").Stop();
                }
            }
            catch (Exception ex)
            {
                Ex.Log(ex.Message);
                if (tempCts.Equals(CurrentHintTaskCts))
                {
                    this.Sb("SearchingSpinSb").Stop();
                }
            }
            
            CurrentHintTaskCts = null;

        }

        private void KeywordComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (KeywordListBox.SelectedIndex < 0) return;
            var item = CurrentHintItems[KeywordListBox.SelectedIndex];
            if (!item.IsEnable) return;
            KeywordTextBox.Text = item.Word;
            KeywordTextBox.Focus();
        }

        public void InitSearch()
        {
            KeywordTextBox.Text = "";
            CurrentHintItems.Clear();
            AddHistoryItems();
            MoeDatePicker.SelectedDate = null;

        }

        private CancellationTokenSource CurrentHintTaskCts { get; set; }

        /// <summary>
        /// 获取关键字的联想
        /// </summary>
        public async Task ShowKeywordComboBoxItemsAsync(string keyword, CancellationToken token)
        {
            CurrentHintItems.Clear();
            AddHistoryItems();
            if (keyword.IsEmpty()) throw new TaskCanceledException();
            await Task.Delay(600, token);// 等待0.6再开始获取，避免每输入一个字都进行网络操作 
            var task = CurrentSelectedSite.GetAutoHintItemsAsync(GetSearchPara(), token);
            if (task == null) throw new TaskCanceledException();
            var list = await CurrentSelectedSite.GetAutoHintItemsAsync(GetSearchPara(), token);
            if (list != null && list.Any())
            {
                CurrentHintItems.Clear();
                foreach (var item in list) CurrentHintItems.Add(item);
                AddHistoryItems();
                Ex.Log($"AutoHint 搜索完成 结果个数{list.Count}");
            }
        }

        private void AddHistoryItems()
        {
            CurrentHintItems.Add(new AutoHintItem { IsEnable = false, Word = "---------历史---------" });
            if (Settings?.HistoryKeywords?.Count == 0 || Settings?.HistoryKeywords == null) return;
            foreach (var item in Settings.HistoryKeywords)
            {
                CurrentHintItems.Add(item);
            }
        }

        public SearchPara GetSearchPara()
        {
            var para = new SearchPara
            {
                Site = CurrentSelectedSite,
                Count = FilterCountBox.NumCount,
                PageIndex = FilterStartPageBox.NumCount,
                Keyword = KeywordTextBox.Text,
                IsShowExplicit = FilterExlicitCheckBox.IsChecked == true,
                IsShowExplicitOnly = ShowExlicitOnlyCheckBox.IsChecked == true,
                IsFilterResolution = FilterResolutionCheckBox.IsChecked == true,
                MinWidth = FilterMinWidthBox.NumCount,
                MinHeight = FilterMinHeightBox.NumCount,
                Orientation = (ImageOrientation)OrientationComboBox.SelectedIndex,
                IsFilterFileType = FilterFileTypeCheckBox.IsChecked == true,
                FilterFileTypeText = FilterFileTypeTextBox.Text,
                IsFileTypeShowSpecificOnly = FileTypeShowSpecificOnlyComboBox.SelectedIndex == 1,
                DownloadType = CurrentSelectedSite.DownloadTypes[DownloadTypeComboBox.SelectedIndex],
                Date = MoeDatePicker.SelectedDate,
                NextPageMark = $"{FilterStartIdBox.NumCount}" == "0"? null: $"{FilterStartIdBox.NumCount}",
                SubMenuIndex = MoeSitesLv2ComboBox.SelectedIndex,
                Lv3MenuIndex = MoeSitesLv3ComboBox.SelectedIndex,
                Lv4MenuIndex = MoeSitesLv4ComboBox.SelectedIndex,
                SupportState = CurrentSupportState
            };
            if (!Settings.IsXMode) para.IsShowExplicit = false;
            return para;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //[NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}