﻿#pragma checksum "C:\Posh_TFS\POSH.Socrata.WP8\POSH.Socrata.Dev\POSH.Socrata\POSH.Socrata.WP8\Views\PrivacyPolicy.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B92214137E1A892B357E4126CC63F6AF"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.33440
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using POSH.Socrata.WP8.Class;
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


namespace POSH.Socrata.WP8.Views {
    
    
    public partial class PrivacyPolicy : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal POSH.Socrata.WP8.Class.ScrollableTextBlock tbkPrivacyPolicy;
        
        internal System.Windows.Controls.TextBlock tbkLoadingData;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/POSH.Socrata.WP8;component/Views/PrivacyPolicy.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.tbkPrivacyPolicy = ((POSH.Socrata.WP8.Class.ScrollableTextBlock)(this.FindName("tbkPrivacyPolicy")));
            this.tbkLoadingData = ((System.Windows.Controls.TextBlock)(this.FindName("tbkLoadingData")));
        }
    }
}

