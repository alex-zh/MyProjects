﻿#pragma checksum "..\..\..\Controls\QuikConnectionSummaryControl.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "E2A168511B9ED279ED05D31378CBE3F19F115F63"
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


namespace Robot.TradeApplication.Controls {
    
    
    /// <summary>
    /// QuikConnectionSummaryControl
    /// </summary>
    public partial class QuikConnectionSummaryControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 46 "..\..\..\Controls\QuikConnectionSummaryControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbQuikDdeStatus;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\..\Controls\QuikConnectionSummaryControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbQuikTime;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\Controls\QuikConnectionSummaryControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbRobotTime;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\..\Controls\QuikConnectionSummaryControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbDealsUpdateTime;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\Controls\QuikConnectionSummaryControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbTradesUpdateTime;
        
        #line default
        #line hidden
        
        
        #line 76 "..\..\..\Controls\QuikConnectionSummaryControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbRobotToQuikStatus;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\..\Controls\QuikConnectionSummaryControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbQuikToServerStatus;
        
        #line default
        #line hidden
        
        
        #line 85 "..\..\..\Controls\QuikConnectionSummaryControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnConnect;
        
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
            System.Uri resourceLocater = new System.Uri("/Robot.TradeApplication;component/controls/quikconnectionsummarycontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Controls\QuikConnectionSummaryControl.xaml"
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
            this.tbQuikDdeStatus = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            this.tbQuikTime = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.tbRobotTime = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.tbDealsUpdateTime = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.tbTradesUpdateTime = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.tbRobotToQuikStatus = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 7:
            this.tbQuikToServerStatus = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.btnConnect = ((System.Windows.Controls.Button)(target));
            
            #line 85 "..\..\..\Controls\QuikConnectionSummaryControl.xaml"
            this.btnConnect.Click += new System.Windows.RoutedEventHandler(this.BtnConnect_OnClick);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

