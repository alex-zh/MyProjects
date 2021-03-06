﻿#pragma checksum "..\..\..\Controls\LogsControl.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "979086F1ED9168A3E33A9502A05275FECA15F790"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using Visualizer.VisualControls.Classes;


namespace Robot.TradeApplication.Controls {
    
    
    /// <summary>
    /// LogsControl
    /// </summary>
    public partial class LogsControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 65 "..\..\..\Controls\LogsControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton tbErrors;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\..\Controls\LogsControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton tbWarnings;
        
        #line default
        #line hidden
        
        
        #line 76 "..\..\..\Controls\LogsControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton tbInfo;
        
        #line default
        #line hidden
        
        
        #line 87 "..\..\..\Controls\LogsControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid LogsGrid;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Robot.TradeApplication;component/controls/logscontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Controls\LogsControl.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.tbErrors = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            
            #line 65 "..\..\..\Controls\LogsControl.xaml"
            this.tbErrors.Click += new System.Windows.RoutedEventHandler(this.ErrorsFilterButton_OnClick);
            
            #line default
            #line hidden
            return;
            case 2:
            this.tbWarnings = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            
            #line 71 "..\..\..\Controls\LogsControl.xaml"
            this.tbWarnings.Click += new System.Windows.RoutedEventHandler(this.WarningsFilterButton_OnClick);
            
            #line default
            #line hidden
            return;
            case 3:
            this.tbInfo = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            
            #line 76 "..\..\..\Controls\LogsControl.xaml"
            this.tbInfo.Click += new System.Windows.RoutedEventHandler(this.InfoFilterButton_OnClick);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 82 "..\..\..\Controls\LogsControl.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.CleanLog_OnClick);
            
            #line default
            #line hidden
            return;
            case 5:
            this.LogsGrid = ((System.Windows.Controls.DataGrid)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

