   M S B A M L   `   `   `  ����R  NRobot.TesterApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullS.clr-namespace:Robot.TesterApplication.Controls Robot.TesterApplication.Controls  5      T PVisualizer.VisualControls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullzSclr-namespace:Visualizer.VisualControls.Controls;assembly=Visualizer.VisualControls"Visualizer.VisualControls.Controls 5      ?    9Robot.TesterApplication.Controls.MiscellaneousInfoControl   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Ccontrols.clr-namespace:Robot.TesterApplication.Controls     5      bvcSclr-namespace:Visualizer.VisualControls.Controls;assembly=Visualizer.VisualControls     5        � 	Resources  5
         TableColumnHeaders �� 
TargetType �� Property �� Value  TableRowHeaders  
TableValue��%�  &	        &	 z     &	 �     �� 5   
   " ��6,   .�5      ��  � ��$
 30,9q�6'   66   �� 5       � ��$
 Bold"�6+   6:   5      �� 5   
   " ��6)   .�5      ��  � ��$	 6,9q�6'   65   �� 5       � ��$
 Bold"�6+   6:   5      �� 5   
   " ��6$   .�5      ��  �@��TextAlignment$ Center=�6.   6?   �� 5       � ��$ Center=�62   6C   5      5      .��5      � -   5         �Name$ statisticsGrid��5       � RowDefinitions 5   
   �� 5      $
��Auto �6   6,   �� 5      $
��Auto �6   6,   �� 5      $
��Auto �6   6,   �� 5      $
��Auto �6   6,   �� 5      $��* �6   6)    � ColumnDefinitions 5!   
   �� 5"      $
��Auto �6   6.   �� 5#      $��* �6   6+   .+�5&   
   �� $��0��6   $��0��6!   $��Candles Count:��61   #��[  6G   6v   �� 5'   
   -   5         ��Name$ tbCandlesCount��5'   1   $��0��6   $��1��6!   $��0��6I   #��[  6T   6�   �� 5)   
   $��1��6   $��0��6!   $"��Movings Intersections Count:��61   #��[  6U   6�   �� 5*   
   -   5        $! tbMovingsIntersectionsCount��5*   1   $��1��6   $��1��6!   $��0��6V   #��[  6a   6�   �� 5,   
   $��2��6   $��0��6!   $	��200��61   $��WrapWithOverflow=�6=   $E��?Buy Average Maximum Deviation (among all movings intersections)��6]   #��[  6�   6�   �� 5-   
   -   5        $! tbUpAverageMaximumDeviation��5-   1   $��2��6   $��1��6!   $��0��6V   #��[  6a   6�   �� 5/   
   $��3��6   $��0��6!   $	��200��61   $��WrapWithOverflow=�6=   $F��@Sell Average Maximum Deviation (among all movings intersections)��6]   #��[  6�   6�   �� 50   
   -   5        $# tbDownAverageMaximumDeviation��50   1   $��3��6   $��1��6!   $��0��6X   #��[  6c   6�   �� 52   
   $��4��6   $��0��6$   $��2��64   $
��Auto=�6I   $
��Auto=�6l   .��53      �  54      �� 55      $��* �6$   61   �� 56      $��* �6$   61    58      �� 59      $��* �6'   63   .+�5;      �� $	��0.5q�6   
��� � �._�5<      :  4Visualizer.VisualControls.Controls.DistributionChart  -   5          Name$ DistributionChartUp��5<   B   $��6q�6+   $��3q�67   $	��280��6_   6q   5=      �� 5>      $��1��6   $	��0.5q�6&   
��� � �._�5?        -   5        $ DistributionChartDown��5?   C   $��6q�6,   $��3q�68   $	��280��6b   6t   5@      5A      5B      5D      5G      