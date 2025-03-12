using MongoDB.Bson;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Generic;
using Gestio_Botiga_Calcat.model;
using SharpCompress.Readers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Gestio_Botiga_Calcat
{
    public partial class MainWindow : Window
    {
        private Service mdbService;
        private string inici = "Inici";
        private bool sel_cate_fill = false;
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

                button.Click += (s, e) => {
                    breadcr.Text = inici+"/"+cate.Name;
                    lvProds.ItemsSource = null;
                    lvProds.ItemsSource = mdbService.GetProdsDeCate(cate.Id);
                
                    lvFilles.ItemsSource = null;
                    List<CategoriaMDB> cates = mdbService.GetCatesFill(cate.Id);
                    lvFilles.ItemsSource = cates;
                    if (cates.Count > 0) 
                    { 
                        lvFilles.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        lvFilles.Visibility = Visibility.Collapsed;
                    }
                };
                spCates.Children.Add(button);
            }

        }

        private void lvFilles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lvFilles.SelectedValue is CategoriaMDB selected)
            {
                if (breadcr.Text.LastIndexOf('/') > 0 && sel_cate_fill)
                {
                    breadcr.Text = breadcr.Text.Remove(breadcr.Text.LastIndexOf('/'));
                }
                breadcr.Text = breadcr.Text + "/" + selected.Name;
                lvProds.ItemsSource = null;
                lvProds.ItemsSource = mdbService.GetProdsDeCate(selected.Id);
                sel_cate_fill= true;
            }
            else
            {
                sel_cate_fill=false;
                if (breadcr.Text.LastIndexOf('/') > 0)
                {
                    breadcr.Text = breadcr.Text.Remove(breadcr.Text.LastIndexOf('/'));
                }
                lvProds.ItemsSource = null;
            }

        }
    }
}
