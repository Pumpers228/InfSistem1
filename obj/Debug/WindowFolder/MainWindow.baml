   M S B A M L   `   `   `  ����  UPZhukov&     UPZhukov.WindowFolder.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	     � Title$M  GСистема регистрации обращений граждан��5   	   $	��700��5   	   $
��1200��5	   	    � WindowStartupLocation$ CenterScreen=�5
   	      WindowStyle#��[  5   	   .��5      ��   
MainBorder#��[ 6   ._�5   
   �  � RowDefinitions 5      �� 5      $��32 �6    �� 5      $��50 �6    �� 5      $��* �6    .+�5      � -   5        $��0��5      ���	#FF0078D7 � ColumnDefinitions 5      �� 5      $
��Auto �6'   �� 5      $��* �6'   �� 5      $
��Auto �6'   .+�5      �� $M��GСистема регистрации обращений граждан��6   $��1��5        	TitleText#��[ 5      �� 5!      -   5         ��Name$ CloseButton��5!      $��2��5"      $	����5#        WindowButtonStyle#��[ 5%      5&      �� 5)      $��1��6   ���#F5F5F5���#E0E0E0$��0,0,0,1q�5,      ._�5-      � $
��16,0q�6    5.      �� 5/      $��* �6+   �� 50      $
��Auto �6+   .+�53      �� $��0��6!    �� Orientation$ 
Horizontal=�54      $��Center=�55      .+�56      �� $ ��Пользователь: ��6$   ���#66666657   "   �� 58      -   5         ��Name$ 
UserNameTb��58   $   ���#333333$��SemiBold"�5:   "   5;      �� 5=      -   5        $ ExitBtn��5=      $��1��5>      $��
Выход��5?        MainButtonStyle#��[ 5A      $	��100��5B      ��5C      �� 5D      .R�5E   "   �� $ 
Horizontal=�6-   .+�5F   &   �� $	����60   $��Segoe MDL2 Assets)�5G   .   $��0,0,8,0q�5H   .   $��Center=�5I   .   �� 5J   &   $��
Выход��60   $��Center=�5K   .   5L   #   5M      5O      5P      5Q      �� 5T      $��2��6   
�������$��16q�5V      ._�5W       � -   5          �Name$ 	MainFrame��5W        � NavigationUIVisibility$ Hidden=�5X      5Y      5Z      5[      5\      