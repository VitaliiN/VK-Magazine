﻿#pragma checksum "D:\svn\ВК журнал\VKMagazine\Helpers\ZoomingImage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "3292EA1234770DE395461835D7B04CC3"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.18408
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

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


namespace VKMagazine.Helpers {
    
    
    public partial class ZoomingImage : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.UserControl myZoomingImage;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Primitives.ViewportControl viewport;
        
        internal System.Windows.Controls.Canvas canvas;
        
        internal System.Windows.Controls.Image imageDocument;
        
        internal System.Windows.Media.ScaleTransform xform;
        
        internal System.Windows.Controls.Image temporaryImage;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/VKMagazine;component/Helpers/ZoomingImage.xaml", System.UriKind.Relative));
            this.myZoomingImage = ((System.Windows.Controls.UserControl)(this.FindName("myZoomingImage")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.viewport = ((System.Windows.Controls.Primitives.ViewportControl)(this.FindName("viewport")));
            this.canvas = ((System.Windows.Controls.Canvas)(this.FindName("canvas")));
            this.imageDocument = ((System.Windows.Controls.Image)(this.FindName("imageDocument")));
            this.xform = ((System.Windows.Media.ScaleTransform)(this.FindName("xform")));
            this.temporaryImage = ((System.Windows.Controls.Image)(this.FindName("temporaryImage")));
        }
    }
}

