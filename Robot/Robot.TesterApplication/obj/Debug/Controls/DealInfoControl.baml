   M S B A M L   `   `   `  ����T  PVisualizer.VisualControls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullxRclr-namespace:Visualizer.VisualControls.Classes;assembly=Visualizer.VisualControls!Visualizer.VisualControls.Classes  5       Robot.TesterApplication6   0Robot.TesterApplication.Controls.DealInfoControl   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      mvisualControlsRclr-namespace:Visualizer.VisualControls.Classes;assembly=Visualizer.VisualControls     5        � 	Resources  5	       �� 
TargetType �� Property �� Value=  7System.Windows.Controls.Primitives.DataGridColumnHeader)  #System.Windows.Controls.DataGridRow*  $System.Windows.Controls.DataGridCell5   /Visualizer.VisualControls.Classes.SizeConverter   MyConverter:   4Visualizer.VisualControls.Classes.DirectionConverter  DirectionConverter�� 5      %J  '��       '  ~     '  K    '  �    &	  �    &	 �    �� 5      " ��6   .�5      ��  �@��Padding$	 6,3q�6,   6:   �� 5       � ��$ 3q�6+   67   5      �� 5      "  6   .�5      ��  � ��$ Center=�6?   6P   �� 5       � ��$ 80��6-   6:   �� 5       � ��$
 Bold"�6/   6>   �� 5       � ��$ 1q�64   6@   5      �� 5      "  6   .�5      ��  � ��
 ���� 60   �� 5       � ��$ 0q�64   5      �� 5      "  6   .�5      ��  �@��TextAlignment$ Center=�6<   6M   �� 5       � ��$ 80��6-   6:   �� 5       � ��$ Center=�6?   6P   �� 5        � ��$ Center=�6=   6N   �� 5!       � ��
 �����6?   �� 5"       � ��$ 0q�64   6@   5#        5$      6+     5%      60   5&      .��5(      �  � RowDefinitions 5)      �� 5*      $
��Auto �6   6,   �� 5+      $
��Auto �6   6,   �� 5,      $
��Auto �6   6,   �� 5-      $
��Auto �6   6,   �� 5.      $
��Auto �6   6,    � ColumnDefinitions 50   
   �� 51      $
��Auto �6   6.   �� 52      $
��Auto �6   6.   �� 53      $
��Auto �6   6.   .+�56   
   �� $��Index:��6   $
��Bold"�6"   $��0��64   $��0��6A   6S   �� 57   
   -   5         ��Name$ tbIndex��57      $��0��6%   $��1��62   6D   �� 59   
   $��1��6   $��Start Date:��6!   $
��Bold"�64   $��0��6G   6Y   �� 5:   
   -   5        $ tbStartDate��5:   !   $��1��6   $��1��67   6I   �� 5<   
   $��2��6   $��	End Date:��6!   $
��Bold"�62   $��0��6D   6V   �� 5=   
   -   5        $ 	tbEndDate��5=   !   $��2��6   $��1��64   6F   �� 5?   
   $��3��6   $��Profit in Money:��6!   $
��Bold"�69   $��0��6K   6]   �� 5@   
   -   5        $ tbProfitInMoney��5@   !   $��3��6   $��1��6:   6L   �� 5B   
   $��4��6   $��Profit in Percent:��6!   $
��Bold"�6;   $��0��6N   6`   �� 5C   
   -   5        $ tbProfitInPercent��5C   !   $��4��6   $��1��6=   6O   &   System.Windows.Controls.DataGrid  5E   
   -   5        $��TradeItemsGrid��5E   4   $��5��6   $��2��6$      HeadersVisibility$ Column=�6J      
IsReadOnly . $��0q�6w   $��3q�6�   	   CanUserAddRows	 .  
   CanUserReorderColumns
 .     CanUserSortColumns .  
������    HorizontalGridLinesBrush
 �����$��0q�6     VerticalGridLinesBrush
 �����   AutoGenerateColumns .     Columns 5F      0  *System.Windows.Controls.DataGridTextColumn  5G      ,  &System.Windows.Controls.DataGridColumn   Header$
 Date��6W   1	  +System.Windows.Controls.DataGridBoundColumn 	  Binding 6%   ��  �� Path$
 Date� �� StringFormat$ dd/MM��6W     5H      $
 Time��6Z    6%   �� $
 Date�$ HH:mm:ss��6Z     5I      $ Price��6U    6%   �� $ Price�$ F2��6U     5J      $
 Size��6k    6%   �� $
 Size� �� 	Converter# [  6k     5K      $ 	Direction��6r    6%   �� $
 Size�# [ 6r   5N      5P      5Q      