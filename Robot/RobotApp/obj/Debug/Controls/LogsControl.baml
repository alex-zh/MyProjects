   M S B A M L   `   `   `  ����T  PVisualizer.VisualControls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullxRclr-namespace:Visualizer.VisualControls.Classes;assembly=Visualizer.VisualControls!Visualizer.VisualControls.Classes  5       Robot.TradeApplication1   +Robot.TradeApplication.Controls.LogsControl   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      mvisualControlsRclr-namespace:Visualizer.VisualControls.Classes;assembly=Visualizer.VisualControls     5        � 	Resources  5	         LogTextColumnTemplate �� MaxLines �� TextAlignment �� Path �� BasedOn �� 
TargetType �� Property �� Value=  7System.Windows.Controls.Primitives.DataGridColumnHeader)  #System.Windows.Controls.DataGridRow  RowDataGridStyle �� Triggers	 �� Binding
 �� Value*  $System.Windows.Controls.DataGridCell  DataGridCellStyle P� Property P� Value:   4Visualizer.VisualControls.Classes.RobotNameConverter  RobotNameConverter�� 5
   
   %�  &	        'd� p     7d�'  �     &	 �    &	 �    &	 �    �� 5      .R�5      �� $��0q�6   $ 1��6.   $
 Left=�6;   ��6P   �� $
 Text�6m   5      �� 5      8   " d�6F   .�5      ��  � ��$ 3q�6+   68   �� 5       � ��$ 9q�6,   69   5      �� 5      "  6   .�5      ��  � ��$ Center=�6?   6P   �� 5       � ��$ 60��6-   6:   �� 5       � ��$
 Bold"�6/   6>   �� 5       � ��$ 1q�64   6@   �� 5       � ��$ 	-1,-1,0,0q�6+   5      �� 5      "  6    5      �� 5      	 6"   �� *IsLast+
 true6=   .a�5      ��  � ��$ 0q�6<   5       .�5"      ��  � ��
 �����60   �� 5#       � ��$ 0,0,0,1q�64   5$      �� 5%      "  6    5'      P� 5(       �@ 
IsSelected . . �5)      ��  � ��
 �   �67   �� 5*       � ��
 ���� 67   5+      .�5.      ��  �@��TextAlignment$ Center=�6<   6M   �� 5/       � ��$ 80��6-   6:   �� 50       � ��$ Center=�6?   6P   �� 51       � ��$ Center=�6=   6N   �� 52       � ��
 �����6A   �� 53       � ��$ 0,0,1,0q�64   6F   54        55      60   56      .��59      �  � RowDefinitions 5:   
   �� 5;      $
��Auto �6   6,   �� 5<      $��* �6   6)   .+�5?   
   ]� $	��0,3q�6   .+�5A      d� -   5         d�Name$ tbErrors��5A       d� 	IsChecked$
 TrueY�6P   .��5B      ��  �� Orientation$ 
Horizontal=�6   .+�5D      �� $��Center=�6    $��3,0,6,0q�6;   Errors6T   5E      5F      d� 5G      -   5        $ 
tbWarnings��5G      .��5H      �� $ 
Horizontal=�6   .+�5I      �� $��Center=�6    $��3,0,6,0q�6;   
Warnings6V   5J      5K      d� 5L      -   5        $ tbInfo��5L      .��5M      �� $ 
Horizontal=�6   .+�5N      �� $��Center=�6    $��3,0,6,0q�6;   
Messages6V   5O      5P      �� 5R      -   5        $��Right=�5R      $��24��62   $	��6,0q�6>   $	��110��6K   .��5S      �� $��Center=�6   $��3,0,6,0q�67   	Clean log6S   5T      5U      &   System.Windows.Controls.DataGrid  5W   
   -   5        $��LogsGrid��5W   +   $��1��6   $��0q�6       	CellStyle# [ 6;      RowStyle# [ 6j      HeadersVisibility$ Column=�6�      
IsReadOnly .    GridLinesVisibility$
 None=�6�   $��0q�6�      CanUserAddRows .     CanUserReorderColumns .     CanUserSortColumns .  
������    HorizontalGridLinesBrush
 �����$��0q�6w     VerticalGridLinesBrush
 �����   AutoGenerateColumns .     Columns 5X      0  *System.Windows.Controls.DataGridTextColumn  5Y      ,  &System.Windows.Controls.DataGridColumn   Header$
 Time��6G      Width5  /System.Windows.Controls.DataGridLengthConverter$	 140 6U   1	  +System.Windows.Controls.DataGridBoundColumn 	  Binding 6%   �� $ DateTime�6U     5Z      $ 
Robot Name��6u   $	 120 6�    6%   �� $ RobotId�  �� 	Converter#  [ 6�     5[      $
 Type��6J   $ 80 6X    6%   �� $ MessageType�6X   4
  .System.Windows.Controls.DataGridTemplateColumn
  5\      ! 
  CellTemplate#! [  6*   $
 Text��6`   $ * 6n   5^      5_      5`      