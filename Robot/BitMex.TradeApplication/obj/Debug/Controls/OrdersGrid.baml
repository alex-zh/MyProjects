   M S B A M L   `   `   `  ����  BitMex.TradeApplication1    +BitMex.TradeApplication.Controls.OrdersGrid   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5        � 	Resources  5	       �� 
TargetType �� Property �� Value=  7System.Windows.Controls.Primitives.DataGridColumnHeader*  $System.Windows.Controls.DataGridCell   DataGridCellStyle  LastDataGridCellStyle��%�  '��       '  L     &	  D    &	 z    �� 5
   
   " ��6   .�5      ��  � ��$ 3q�6'   63   5      �� 5   
   "  6   .�5      ��  � ��$ Center=�6;   6L   �� 5       � ��$ 60��6)   66   �� 5       � ��$
 Bold"�6+   6:   �� 5       � ��$ 1q�60   6<   �� 5       � ��$ 	-1,-1,0,0q�6'   5      �� 5   
   "  6   .�5      ��  �@��TextAlignment$ Center=�68   6I   �� 5       � ��$ 80��6)   66   �� 5       � ��$ Center=�6;   6L   �� 5       � ��$ Center=�69   6J   �� 5       � ��
 �����6=   �� 5       � ��$ 1,0,0,1q�60   6B   5      �� 5   
   "  6   .�5      ��  � ��
 �����6=   �� 5       �@��TextAlignment$ Center=�68   6I   �� 5       � ��$ 80��6)   66   �� 5        � ��$ Center=�69   6J   �� 5!       � ��$ 1,0,1,1q�60   6B   �� 5"       � ��$ 0,30,0,0q�6(   6;   5#      5$      .��5%      � .+�5&   
   &   System.Windows.Controls.DataGrid  -   5        $��DataGrid��5&         	CellStyle# [  6#      HeadersVisibility$ Column=�6R      
IsReadOnly .    GridLinesVisibility$
 None=�6   $��0q�6�      CanUserAddRows .  	   CanUserReorderColumns	 .  
   CanUserSortColumns
 .  
������    HorizontalGridLinesBrush
 �����$��0q�62     VerticalGridLinesBrush
 �����   AutoGenerateColumns .     Columns 5'      0  *System.Windows.Controls.DataGridTextColumn  5(      ,  &System.Windows.Controls.DataGridColumn   Header$ Order ID��6F   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6%   ��  �� Path$ OrderId�6F     5)      $ DateTime��6[    6%   �� $ DateTime� �� StringFormat$ dd/MM��6[     5*      $
 Time��6^    6%   �� $ DateTime�$ HH:mm:ss��6^     5+      $ Price��6U    6%   �� $ Price�$ F2��6U     5,      $
 Size��6C    6%   �� $
 Size�6C     5-      $
 Side��6H      Width5  /System.Windows.Controls.DataGridLengthConverter$	 100 6V    6%   �� $ 	OrderSide�6V   50      51      52      