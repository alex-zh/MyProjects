﻿#pragma checksum "..\..\..\Controls\MiscellaneousInfoControl.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "7C19E0C1A4200FCEDB861AC1A66C552C0CBDBB30"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Robot.TesterApplication.Controls;
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
using Visualizer.VisualControls.Controls;


namespace Robot.TesterApplication.Controls {
    
    
    /// <summary>
    /// MiscellaneousInfoControl
    /// </summary>
    public partial class MiscellaneousInfoControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 25 "..\..\..\Controls\MiscellaneousInfoControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid statisticsGrid;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\Controls\MiscellaneousInfoControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbCandlesCount;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\Controls\MiscellaneousInfoControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbMovingsIntersectionsCount;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\Controls\MiscellaneousInfoControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbUpAverageMaximumDeviation;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\Controls\MiscellaneousInfoControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbDownAverageMaximumDeviation;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\..\Controls\MiscellaneousInfoControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Visualizer.VisualControls.Controls.DistributionChart DistributionChartUp;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\Controls\MiscellaneousInfoControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Visualizer.VisualControls.Controls.DistributionChart DistributionChartDown;
        
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
            System.Uri resourceLocater = new System.Uri("/Robot.TesterApplication;component/controls/miscellaneousinfocontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Controls\MiscellaneousInfoControl.xaml"
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
            this.statisticsGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.tbCandlesCount = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.tbMovingsIntersectionsCount = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.tbUpAverageMaximumDeviation = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.tbDownAverageMaximumDeviation = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.DistributionChartUp = ((Visualizer.VisualControls.Controls.DistributionChart)(target));
            return;
            case 7:
            this.DistributionChartDown = ((Visualizer.VisualControls.Controls.DistributionChart)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

