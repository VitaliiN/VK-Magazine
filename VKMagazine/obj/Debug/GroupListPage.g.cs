﻿#pragma checksum "D:\svn\ВК журнал\VKMagazine\GroupListPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "33F509AEE6A0DF14AF72E08A505B1CA0"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.18408
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace VKMagazine {
    
    
    public partial class GroupListPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBlock TitleTextBlock;
        
        internal Microsoft.Phone.Controls.PhoneTextBox SearchTextBox;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal Microsoft.Phone.Controls.LongListSelector LongListSelector;
        
        internal Microsoft.Phone.Controls.LongListSelector SearchLongListSelector;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/VKMagazine;component/GroupListPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.TitleTextBlock = ((System.Windows.Controls.TextBlock)(this.FindName("TitleTextBlock")));
            this.SearchTextBox = ((Microsoft.Phone.Controls.PhoneTextBox)(this.FindName("SearchTextBox")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.LongListSelector = ((Microsoft.Phone.Controls.LongListSelector)(this.FindName("LongListSelector")));
            this.SearchLongListSelector = ((Microsoft.Phone.Controls.LongListSelector)(this.FindName("SearchLongListSelector")));
        }
    }
}

