   M S B A M L   `   `   `  ����J  FVisualizer.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:Visualizer.CoreVisualizer.Core  5           Visualizer.Core.StockChart   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      /localclr-namespace:Visualizer.Core     5      .��5
      � -   5        $��	gridChart��5
        � RowDefinitions  5   
   �� 5      $��* �6   6)   �� 5      $
��Auto �6   6,   �� 5      $
��Auto �6   6,    � ColumnDefinitions 5   
   �� 5      $��* �6   6+   �� 5      $
��Auto �6   6.   .+�5   
   #   Visualizer.Core.CandleInfoBox  -   5          Name$ infoBox��5       K� ZIndex$	 999��61   $	��Top=�6D   $
��Left=�6\   
������ 6�   #   Visualizer.Core.CursorInfoBox  5   
   -   5          Name$ cursorInfoBox��5      $	 999��67   $	��Top=�6J   $��right=�6b   
������ 6�   �� 5   
   -   5         ��Name$ scrollViewerCanvasChart��5      $��Hidden=�68   $��0��6]   $��0��6m   $��Hidden=�6z   .��5      �� -   5        $��canvasChart��5      }�. 
������ 6_   5      �� 5    
   -   5        $ scrollViewerDateTimeAxis��5       $��Hidden=�69   $��Stretch=�6^   $��Stretch=�6z   $��0��6�   $��1��6�   $��Hidden=�6�   .��5!      "   Visualizer.Core.DateTimeAxis  -   5          Name$ axisHorizontal��5!   !   5"         Visualizer.Core.ValueAxis  5$   
   -   5          Name$ axisVertical��5$   *   $��1��6   $��0��6@   }�. 6M   �� 5%   
   -	   5         ��Name$ scrollBarHorizontal��5%   n   $��0��6   $��2��6$   $��16��61   
�������	 �� Orientation$	 
Horizontal=�6U   $��Stretch=�6�   6�   �� 5&   
   $��2��6   $��1��6!   
�������6K   �� 5'   
   $��1��6   $��1��6!   
�������6K   5(      5*      