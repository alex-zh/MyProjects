   M S B A M L   `   `   `  ����R  NRobot.TesterApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullS.clr-namespace:Robot.TesterApplication.Controls Robot.TesterApplication.Controls  5   	   (    "Robot.TesterApplication.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Ccontrols.clr-namespace:Robot.TesterApplication.Controls     5   	     � Title$  
MainWindow��5   	   $	��600��6   $	��860��6)   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   6,   �� 5	      $��* �6   6)   .+�5   
   o�  o� 
IsMainMenu . .@�5      m� $��_File��6   .@�5      m� -   5        $��_Load Candles��5      5      m� 5      -   5        $��_Load Deals��5      5      5      5      ]� 5   
   $��1��6   .+�5      `� $	��Top=�6   .�5      c�  `� IsLocked . .@�5      �� -   5        $
��24,9q�5   9   $��-3q�6H   .��5      ��  �� Orientation$ 
Horizontal=�6%   .+�5      �� -   5        $��tbStartButton��5   (   $��Center=�6=   $��6,0,0,0q�6X   Run6n   5      5      5      5       �� 5!      -   5         ��Name$ 
tabControl��5!      .@�5"      �� $��
Statistics��6   .��5#      �  5$      �� 5%      $
��Auto �6,   6<   �� 5&      $
��Auto �6,   6<    � ColumnDefinitions 5(      �� 5)      $
��Auto �6/   6>   �� 5*      $��* �6/   6;   .+�5,      �� $��0��6%   $ 
Horizontal=�62   .+�5-      �� -   5         ��Name$ comboboxStrategyConfig��5-   )   $��3q�6I   $	��120��6T   $��Center=�6c   $��Center=�6�   �� 5.      -   5         ��Name$ 	ButtonMae��5.   3   $	��MAE��6%   $��80��6F   $��3q�6T   6{   �� 5/      -   5        $ 	ButtonMfe��5/   3   $	��MFE��6%   $��80��6F   $��3q�6T   6{   �� 50      -	   5        $ btnExportDialog��50   %      
LinkButton#��[  6?   $��Export result��6c   $��80��6�   $
��12,6q�6�   6�   �� 51      	 �� MinLines$	 2��6&   $	��380��63   6D   52      @   :Robot.TesterApplication.Controls.StrategyStatisticsControl  53      -
   5        
  Name$
 StrategyStatisticsControl��53   Z   $��1��6=   $��0��6J   6   55      57      �� 58      $��
View Deals��6   .��59      3   -Robot.TesterApplication.Controls.TradesViewer  -   5          Name$ tradesViewer��59   (   6@   5:      �� 5;      $��Deals Distribution��6   .��5<      >   8Robot.TesterApplication.Controls.DealsDistributionViewer  -   5          Name$ dealsDistributionViewer��5<   7   6Z   5=      �� 5>      $��Miscellaneous Info��6   .��5?      ?   9Robot.TesterApplication.Controls.MiscellaneousInfoControl  -   5          Name$ MiscellaneousInfo��5?   8   6U   5@      5A      5B      5C      5D      