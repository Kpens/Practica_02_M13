using MongoDB.Bson;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Generic;
using Gestio_Botiga_Calcat.model;

namespace Gestio_Botiga_Calcat
{
    public partial class MainWindow : Window
    {
        private Service mdbService;
        public MainWindow()
        {
            InitializeComponent();
            mdbService = new Service("Botiga");


            List<CategoriaMDB> cats = mdbService.GetCatesPare();
            spCates.Children.Clear();



            foreach (CategoriaMDB cate in cats)
            {

                var button = new Button
                {
                    Content = cate.Name,
                    Background = new SolidColorBrush(Colors.White),
                    Margin = new Thickness(10),
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                    
                button.MouseEnter += (s, e) =>
                {
                    lvFilles.ItemsSource = null;
                    List<string> list = new List<string>();

                    foreach (CategoriaMDB cat in mdbService.GetCatesFill(cate.Id))
                    {
                        list.Add(cat.Name);
                    }

                    lvFilles.ItemsSource = list;
                    if (list.Count > 0) 
                    { 
                        lvFilles.Visibility = Visibility.Visible;
                    }
                                       

                    lvProds.ItemsSource = null;
                    lvProds.ItemsSource = mdbService.GetProdsDeCate(cate.Id);

                };
                button.MouseLeave += (s, e) =>
                {

                    lvFilles.Visibility = Visibility.Collapsed;

                };

                spCates.Children.Add(button);
            }

        }

        private void lvFilles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*if(lvFilles.SelectedValue is String selected)
            {
                lvProds.ItemsSource = null;
                lvProds.ItemsSource = selected.ToList();
            }*/

        }
    }
}
