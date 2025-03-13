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
using System.Windows.Controls.Primitives;
using Gestio_Botiga_Calcat.View;

namespace Gestio_Botiga_Calcat
{
    public partial class MainWindow : Window
    {
        private Service mdbService;
        private string inici = "Inici";
        private bool sel_cate_fill = false;

        private void carregar_cates_fill(CategoriaMDB cate) {
            List<CategoriaMDB> cates = mdbService.GetCatesFill(cate.Id);

            StackPanel spfilles = new StackPanel();
            spfilles.Orientation = Orientation.Horizontal;

            if (spCatesFill.Children.Count == 1) {
                sel_cate_fill = false;
            }
            else
            {
                sel_cate_fill = true;
            }

            foreach (CategoriaMDB cat in cates)
            {
                var button = new Button
                {
                    Content = cat.Name,
                    Margin = new Thickness(10),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush(Colors.Black),
                    Foreground = new SolidColorBrush(Colors.White),
                    Padding = new Thickness(5)
                };
                button.Click += (s, e) =>
                {
                    lvProds.ItemsSource = null;
                    lvProds.ItemsSource = mdbService.GetProdsDeCate(cat.Id);
                    carregar_cates_fill(cat);
                    if (breadcr.Text.LastIndexOf('/') > 0 && sel_cate_fill)
                    {
                        breadcr.Text = breadcr.Text.Remove(breadcr.Text.LastIndexOf('/'));
                    }
                    breadcr.Text = breadcr.Text + "/" + cat.Name;
                    lvProds.ItemsSource = null;
                    lvProds.ItemsSource = mdbService.GetProdsDeCate(cat.Id);
                };
                spfilles.Children.Add(button);
            }
            spCatesFill.Children.Add(spfilles);
            
        }
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
                    Margin = new Thickness(10),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush(Colors.Black),
                    Foreground = new SolidColorBrush(Colors.White),
                    Padding = new Thickness(5)
                };

                button.Click += (s, e) => {
                    breadcr.Text = inici+"/"+cate.Name;
                    lvProds.ItemsSource = null;
                    lvProds.ItemsSource = mdbService.GetProdsDeCate(cate.Id);
                    spCatesFill.Children.Clear();
                    carregar_cates_fill(cate);

                };
                spCates.Children.Add(button);
            }

        }

        private void lvFilles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            click_filles();

        }
        private void click_filles()
        {
            if (lvFilles.SelectedValue is CategoriaMDB selected)
            {
                if (breadcr.Text.LastIndexOf('/') > 0 && sel_cate_fill)
                {
                    breadcr.Text = breadcr.Text.Remove(breadcr.Text.LastIndexOf('/'));
                }
                breadcr.Text = breadcr.Text + "/" + selected.Name;
                lvProds.ItemsSource = null;
                lvProds.ItemsSource = mdbService.GetProdsDeCate(selected.Id);
                sel_cate_fill = true;
            }
            else
            {
                sel_cate_fill = false;
                if (breadcr.Text.LastIndexOf('/') > 0)
                {
                    breadcr.Text = breadcr.Text.Remove(breadcr.Text.LastIndexOf('/'));
                }
                lvProds.ItemsSource = null;
            }
        }

        private void lvProds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvProds.SelectedValue is ProducteMDB selected)
            {
                var newWindow = new UIProducte_info(selected);

                this.Close();

                newWindow.Show();
            }
        }
    }
}
