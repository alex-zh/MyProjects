   M S B A M L   `   `   `  ����T  PVisualizer.VisualControls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullW0clr-namespace:Visualizer.VisualControls.Controls"Visualizer.VisualControls.Controls  5      3    -Visualizer.VisualControls.Controls.StockChart   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Econtrols0clr-namespace:Visualizer.VisualControls.Controls     5      .��5
      � -   5        $��	gridChart��5
        � RowDefinitions  5   
   �� 5      $��* �6   6)   �� 5      $
��Auto �6   6,   �� 5      $
��Auto �6   6,    � ColumnDefinitions 5   
   �� 5      $��* �6   6+   �� 5      $
��Auto �6   6.   .+�5   
   6   0Visualizer.VisualControls.Controls.CandleInfoBox  -   5          Name$ infoBox��5   !    K� ZIndex$	 999��64   $	��Top=�6G   $
��Left=�6_   
������ 6�   6   0Visualizer.VisualControls.Controls.CursorInfoBox  5   
   -   5          Name$ cursorInfoBox��5   !   $	 999��6:   $	��Top=�6M   $��right=�6e   
������ 6�   �� 5   
   -   5         ��Name$ scrollViewerCanvasChart��5      $��Hidden=�68   $��0��6]   $��0��6m   $��Hidden=�6z   .��5      �� -   5        $��canvasChart��5      }�. 
������ 6`   5      �� 5   
   -   5        $ scrollViewerDateTimeAxis��5      $��Hidden=�69   $��Stretch=�6^   $��Stretch=�6z   $��0��6�   $��1��6�   $��Hidden=�6�   .��5      5   /Visualizer.VisualControls.Controls.DateTimeAxis  -   5          Name$ axisHorizontal��5   $   5       2   ,Visualizer.VisualControls.Controls.ValueAxis  5"   
   -   5          Name$ axisVertical��5"   -   $��1��6   $��0��6C   }�. 6P   �� 5#   
   -	   5         ��Name$ scrollBarHorizontal��5#   V   $��0��6   $��2��6$   $��16��61   	 �� Orientation$	 
Horizontal=�6=   $��Stretch=�6s   6�   �� 5$   
   $��2��6   $��1��6!   
������ 6L   �� 5%   
   $��1��6   $��1��6!   
������ 6L   5&      5(      