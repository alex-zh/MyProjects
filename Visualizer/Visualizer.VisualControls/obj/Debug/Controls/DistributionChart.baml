   M S B A M L   `   `   `  ����  Visualizer.VisualControls:    4Visualizer.VisualControls.Controls.DistributionChart   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5        � 	Resources  5	         	CopyLabel �� 
TargetType �� Property �� Value��%�   &	        �� 5
   
   " ��6"   .�5      ��  � ��
 ���� 6+   �� 5       � �� 6,   Z� �� 5       � ��$ 4q�60   �� 5       � �� .  6*   �� 5       �@��
IsReadOnly . 6+   5      5      .��5      � -   5         �Name$ 
gridCanvas��5       � RowDefinitions 5   
   �� 5      $
��Auto �6   6,   �� 5      $��* �6   6)   .+�5      �  5      �� 5      $
��Auto �6    60   �� 5      $
��Auto �6    60   �� 5      $
��Auto �6    60    � ColumnDefinitions 5      �� 5      $
��Auto �6#   62   �� 5      $
��Auto �6#   62   .+�5!      �� -   5         ��Name$ lblTitle��5!   s   #��[  6   $��16(�6:   $
��Bold"�6H   $	��6,3q�6Z   $��3q�6g   6�   �� 5"      -   5         ��Name$ 
cbShowMean��5"   8   $��1��6   $��	Show Mean��6$   $��Center=�6L   $	��9,3q�6g   6�   �� 5#      $��1��6   $��1��6&   	 �� Orientation$	 
Horizontal=�66   .+�5$      �� -   5        $ cbShowSigma��5$   +   $
��Show��6   $��Center=�6@   $��3q�6[   6�   �� 5%      -   5        $ tbSigmas��5%   S   $��20��6   $��1��6(   
 �� TextAlignment$
 Center=�61   $��3q�6H   6g   �� 5&      $��sigmas��6   $��Center=�6*   6G   5'      �� 5(      -   5        $ cbShowMedian��5(   J   $��2��6   $��0��6$   $��Show Median��64   $��Center=�6`   $	��9,3q�6{   6�   �� 5)      $��2��6   $��1��6&   $	 
Horizontal=�66   .+�5*      �� -   5        $ cbShowMedianSigma��5*   +   $
��Show��6   $��Center=�6F   $��3q�6a   6�   �� 5+      -   5        $ tbMedianSigmas��5+   H   $��20��6   $��1��6(   $
 Center=�61   $��3q�6`   6m   �� 5,      $��median sigmas��6   $��Center=�61   6N   5-      5.      �� 5/      -	   5         ��Name$ ChartCanvas��5/   "   $��1��6   
������ $��3q�6P   6^   51      52      