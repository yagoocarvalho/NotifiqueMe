   M S B A M L   `   `   `  ����M  INotifiqueMe.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:NotifiqueMe.ServerNotifiqueMe.Server  5   	   #    NotifiqueMe.Server.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   2local clr-namespace:NotifiqueMe.Server     5   	   -   5          � Title$  NotifiqueMe Server��5   	   $	��350��6$   $	��667��61   .��5	      � .+�5
   
   �� -   5         ��Name$ startServerButton��5
      $��Start��6,   $��
0,0,115,10q�6<   $��Bottom=�6p   $��29��6�   $��Right=�6�   $	��100��6�   �� 5   
   -   5        $ closeServerButton��5      $	��End��6,   $��	0,0,10,10q�6:   {�.  $��Bottom=�6   $��29��6�   $��Right=�6�   $	��100��6�   �� 5   
   -   5         ��Name$ 	logWindow��5       �� TextWrapping$
 Wrap=�6%   $�� ��69   $��10,10,220,10q�6A    �� 
IsReadOnly . |�.  $��Disabled=�6{   $
��Auto=�6�   �� 5   
   -   5         ��Name$ groupBox��5      $��Server Information��6%   $��
0,10,10,78q�6A   $��Right=�6U   $	��205��6q   .��5      � $��0,0,0,0q�6   .+�5      �� -   5         ��Name$ currentConnectionsLabel��5      $��Open Connections:��69   $��	0,10,74,0q�6U   $��26��6h   $	��Top=�6t   �� 5      -   5        $ currentConnectionsDisplay��5      $
��0 /0��6;   $��
121,10,0,0q�6J   $��Right=�6^   $��26��6�   $	��Top=�6�   5      5      �� 5   
   -   5        $ SettingsButton��5      $��Settings��6)   $��	0,0,10,44q�6<   $��29��6O   $��Bottom=�6[   $��Right=�6v   $	��205��6�   6�   5      5      