   M S B A M L   `   `   `  ����  Visualizer.VisualControls6    0Visualizer.VisualControls.Controls.CandleInfoBox   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      .��5      �   � RowDefinitions  5	   
   �� 5
      $
��auto �6   6,   �� 5      $
��auto �6   6,   .+�5   
   �� -   5         ��Name$ infoBox��5      $y�Visible=�6&    �� Orientation$ 
Horizontal=�6;   $��20��6T   
������ .+�5      �� $ 
Horizontal=�6   .+�5      �� $��SemiBold"�6    �� Padding$ 3,1,6,0q�62   Index:6L   �� 5      -   5         ��Name$ tbCandleIndex��5      $ 3,1,3,1q�63   $
��Bold"�6E   6Y   5      �� 5      $ 
Horizontal=�6   .+�5      �� $��SemiBold"�6   $ 3,1,3,1q�62   	DateTime:6P   �� 5      -   5        $ tbCandleDate��5      $
��Bold"�62   $ 3,1,3,1q�6D   6X   5      �� 5      $ 
Horizontal=�6   .+�5      �� $��SemiBold"�6   $ 3,1,3,1q�62   Open:6K   �� 5      -   5        $ tbCandleOpen��5      $ 3,1,3,1q�62   $
��Bold"�6D   6X   5      �� 5      $ 
Horizontal=�6   .+�5      �� $��SemiBold"�6   $ 3,1,6,0q�62   Close:6L   �� 5       -   5        $ tbCandleClose��5       $ 3,1,3,1q�63   $
��Bold"�6E   6Y   5!      �� 5#      $ 
Horizontal=�6   .+�5$      �� $��SemiBold"�6   $ 3,1,6,0q�62   Min:6J   �� 5%      -   5        $ tbCandleMin��5%      $ 3,1,3,1q�61   $
��Bold"�6C   6W   5&      �� 5(      $ 
Horizontal=�6   .+�5)      �� $��SemiBold"�6   $ 3,1,6,0q�62   Max:6J   �� 5*      -   5        $ tbCandleMax��5*      $ 3,1,3,1q�61   $
��Bold"�6C   6W   5+      �� 5-      $ 
Horizontal=�6   .+�5.      �� $��SemiBold"�6   $ 3,1,6,0q�62   	Volume:6M   �� 5/      -   5        $ tbCandleVolume��5/      $ 3,1,3,1q�64   $
��Bold"�6F   6Z   50      �� 52      $ 
Horizontal=�6   .+�53      �� $��SemiBold"�6   $ 3,1,6,0q�62   Volume Median:6T   �� 54      -	   5        $ tbVolumeMedian��54      $ 3,1,3,1q�64   $
��Bold"�6F   6Z   55      �� 57      $ 
Horizontal=�6   .+�58      �� $��SemiBold"�6   $ 3,1,6,0q�62   Atr:6J   �� 59      -
   5        $ 
tbAtrValue��59      $ 3,1,3,1q�60   $
��Bold"�6B   6V   5:      5<      �� 5=   
   -   5        $ deviationInfo��5=      $��1��6,   $y�Visible=�69   $ 
Horizontal=�6N   $��20��6g   
������ .+�5>      �� -   5        $ tbDeviationInfo��5>      $ 3,1,3,1q�61   $
��Bold"�6C   6W   5?      5A      5B      