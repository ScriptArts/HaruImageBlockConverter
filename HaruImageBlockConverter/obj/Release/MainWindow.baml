   M S B A M L   `   `   `  ���� R  NHaruImageBlockConverter, Version=2.1.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:HaruImageBlockConverterHaruImageBlockConverter  (    "HaruImageBlockConverter.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     7local%clr-namespace:HaruImageBlockConverter     V RMaterialDesignThemes.Wpf, Version=2.5.0.1205, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes    J� 	AllowDrop  .    MaterialDesignBody#���     MaterialDesignPaper#���   � Title$0 *ハルの画像ブロック変換ソフト��$	��450��$	��800��.��� $��20,10,20,10q� � RowDefinitions �� $��40 ��� $��40 ��� $��40 ���  � ColumnDefinitions �� $	��100 ��� .+��� $��0��$��0��$��16(�$
��Left=�$��Center=�$��画像ファイル��]� ��. $��0��$��1��$��Center=�.+��� -   $��Right=� " MaterialDesignRaisedDarkButton#��[ $	��100��$��画像を選択���� -    ��Name$ FileNameTextBox��$��Stretch=�$��0,0,10,0q�)  #MaterialDesignThemes.Wpf.HintAssist   Hint$ 画像ファイルパス������ *
FileName+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=��� $��1��$��0��$��16(�$
��Left=�$��Center=�$��出力方式����  �� Orientation$ 
Horizontal=�$��1��$��1��$��Stretch=�.+�	� -   	 	�Name$	 NBTRadioButton��
 d� 	IsChecked$

 TrueY�$	��NBT��$��16(�$��80��$��Stretch=�$��Center=�  MaterialDesignRadioButton#��[ 	� -   $	 SchematicRadioButton��$��	Schematic��$��20,0,0,0q�$��16(�$��Stretch=�$��Center=�#��[ �� $��2��$��0��$��16(�$
��Left=�$��Center=�$��変換方式���� $ 
Horizontal=�$��2��$��1��$��Stretch=�.+�	� -   $	 DitherRadioButton��$
 FalseY�$��	近似色��$��16(�$��80��$��Stretch=�$��Center=�#��[ 	� -   $	 FloydSteinbergRadioButton��$��誤差拡散��$

 TrueY�$��20,0,0,0q�$��16(�$��Stretch=�$��Center=�#��[ �� $��3��$��0��$��2��$
��Left=�$
��Auto=�$	��Top=�$
��Auto=�.���� -    ��Name$ 	ImageView��$��Stretch=�$��Stretch=���. �� -    ��Name$ ProgressBorder��$��0��$��4��$��0��$��2�����#303030$y�	Collapsed=��� -	    ��Name$ ConvertButton��$��0��$��4��$��0��$��2��$	��200��  MaterialDesignRaisedButton#��[ $��Stretch=�$y�	Collapsed=�3  -MaterialDesignThemes.Wpf.ButtonProgressAssist   IsIndicatorVisible . ���� *DismissComand+   Maximum �� *Total+$ OneWay=�$ PropertyChanged=�   Value �� *
Complete+$ OneWay=�$ PropertyChanged=�.���� $ 
Horizontal=�.+��� $��	変換中���� -
   $ BuildButton��$��0��$��4��$��0��$��2��$	��200��#��[ $��Stretch=�$y�	Collapsed=�   IsIndeterminate .  . ���� *DismissComand+ �� *Total+$ OneWay=�$ PropertyChanged=� �� *
Complete+$ OneWay=�$ PropertyChanged=�.���� $ 
Horizontal=�.+��� $��	出力中��