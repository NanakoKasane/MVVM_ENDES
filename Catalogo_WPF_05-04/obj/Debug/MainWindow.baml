   M S B A M L   `   `   `  ����M  ICatalogo_WPF_05-04, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:catalogo2018catalogo2018  5   	       catalogo2018.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   ,localclr-namespace:catalogo2018     5   	     � Title$  Catálogo de Dvd��5   	   $��357.342��6!   $��719.731��62    � 
ResizeMode$ NoResize=�6B   .��5      �  � DataContext 5   
     catalogo2018.CatalogoVM  5	      .+�5   
   &   System.Windows.Controls.DataGrid     SelectionMode$ Single=�6   $
��Left=�6*   $��	10,10,0,0q�6E   $	��Top=�6X   $	��230��6p   $	��571��6}      CanUserAddRows .  ��5      �� *	Listado+ �� SelectedItem 63   �� *DVDSeleccionado+ �� Mode$ TwoWay=�6i   �� 5   
   -   5         ��Name$ btn_Conectar��5      $��Conectar��6~   $
��Left=�6�   $��
10,256,0,0q�6�   $	��Top=�6�   $��85��6�   $��32��6�   ��6'   �� *ColorConectar+��6L   �� *ConectarBD_Click+$ OneWay=�6�   �� 5   
   -   5        $ btn_Desconectar��5      $��Desconectar��6*   $
��Left=�6�   $��121,256,0,0q�6�   $	��Top=�6�   $��83��6�   $��32��6�   {�6@   �� *	Conectado+$ OneWay=���6m   �� *DesconectarBD_Click+$ OneWay=�6�   �� 5   
   -   5        $ 
btn_Listar��5      $��
Listar DVD��6%   $
��Left=�6g   $��235,256,0,0q�6�   $	��Top=�6�   $��85��6�   $��32��6�   {�6:   �� *	Conectado+$ OneWay=���6�   �� *ListarTodosDVD_Click+$ OneWay=��� 5   
   -   5         ��Name$ 
lbl_Estado��5      $
��Left=�6@   $��
10,293,0,0q�6[   $	��Top=�6o   $	��571��6�   $��27��6�   ��6$   �� *	Mensaje+6�   	� 5   
   $��MySQL��69   $
��Left=�6I   $��620,189,0,0q�6d   $	��Top=�6y   	 	� 	GroupName$	 bd��6�   
 d� 	IsChecked
 6   �� *TipoConexion+6�   	� 5   
   $��SQLite��6   $
��Left=�6'   $��620,224,0,0q�6B   $	��Top=�6W   $	 bd��6o   �� 5   
   $��Borrar��65   $
��Left=�6o   $��352,256,0,0q�6�   $	��Top=�6�   $��90��6�   $��32��6�   ��6   �� *BorrarDVD_Click+{�6F   �� *HayDVDSeleccionado+6�   �� 5    
   $��
Actualizar��69   $
��Left=�6w   $��484,256,0,0q�6�   $	��Top=�6�   $��97��6�   $��32��6�   ��6   �� *ActualizarDVD_Click+{�6N   �� *HayDVDSeleccionado+6�   5"      5#      