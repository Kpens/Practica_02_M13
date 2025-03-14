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
        private CategoriaMDB Cate_select;
        private StockMDB Stock_select;

        List<VariantMDB> variants = new List<VariantMDB>();

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
                    Margin = new Thickness(5),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush(Colors.Black),
                    Foreground = new SolidColorBrush(Colors.White),
                    Padding = new Thickness(5)
                };
                button.Click += (s, e) =>
                {
                    Cate_select = cat;
                    carregar_cates_fill(cat);

                    string cate_an = breadcr.Text.Substring(breadcr.Text.LastIndexOf('/')+1);

                    if (breadcr.Text.LastIndexOf('/') > 0 && sel_cate_fill && cate_an.Trim()!= cate.Name.Trim())
                    {
                        breadcr.Text = breadcr.Text.Remove(breadcr.Text.LastIndexOf('/'));
                    }
                    if (!breadcr.Text.Contains(cat.Name))
                    {
                        breadcr.Text = breadcr.Text + "/" + cat.Name;
                    }

                    carregar_prods();
                };

                bool trobat = false;
                foreach (StackPanel sp in spCatesFill.Children)
                {
                    if (sp.Children.OfType<Button>().Any(b => b.Content.ToString() == button.Content.ToString()))
                    {
                        trobat = true;
                        break;
                    }
                }

                if (!trobat)
                {
                    spfilles.Children.Add(button);
                }
                
            }
            spCatesFill.Children.Add(spfilles);
            
        }
        public MainWindow()
        {
            InitializeComponent();
            mdbService = new Service("Botiga");
            variants = mdbService.GetAllVariants();

            slMinPreu.ValueChanged += PriceSlider_ValueChanged;
            slMaxPreu.ValueChanged += PriceSlider_ValueChanged;
            List<CategoriaMDB> cats = mdbService.GetCatesPare();
            spCates.Children.Clear();
            foreach (CategoriaMDB cate in cats)
            {

                var button = new Button
                {
                    Content = cate.Name,
                    Margin = new Thickness(5),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush(Colors.Black),
                    Foreground = new SolidColorBrush(Colors.White),
                    Padding = new Thickness(5)
                };

                button.Click += (s, e) => {
                    Cate_select = cate;
                    breadcr.Text = inici+"/"+cate.Name;
                    carregar_prods();
                    spCatesFill.Children.Clear();
                    carregar_cates_fill(cate);

                };
                spCates.Children.Add(button);
            }
            carregar_prods();

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
                carregar_prods();
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

        private void spNom_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (tbNom.Visibility == Visibility.Collapsed)
            {
                tbMostrar.Text = "-";
                tbNom.Visibility = Visibility.Visible;
            }
            else
            {
                tbMostrar.Text = "+";
                tbNom.Visibility = Visibility.Collapsed;
            }
        }

        private void spPreu_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (tbpreu.Visibility == Visibility.Collapsed)
            {
                tbMostrar2.Text = "-";
                tbpreu.Visibility = Visibility.Visible;
            }
            else
            {
                tbMostrar2.Text = "+";
                tbpreu.Visibility = Visibility.Collapsed;
            }

        }
        private void PriceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbMax.Text = slMaxPreu.Value+"€";
            tbMin.Text = slMinPreu.Value + "€";
            carregar_prods();

        }

        private void spTalles_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lvTalles.Visibility == Visibility.Collapsed)
            {
                lvTalles.Children.Clear();
                foreach(VariantMDB variant in variants)
                {
                    foreach (StockMDB stock in variant.Stock)
                    {
                        var button = new Button
                        {
                            Margin = new Thickness(5),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Background = new SolidColorBrush(Colors.White),
                            Foreground = new SolidColorBrush(Colors.Black),
                            Padding = new Thickness(5)
                        };
                        TextBlock textBlock = new TextBlock
                        {
                            Text = stock.Talla + "",
                            Foreground = new SolidColorBrush(Colors.Black),
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        
                        button.Content = textBlock;

                        button.Click += (s, e) =>
                        {
                            foreach(Button but in lvTalles.Children)
                            {
                                but.Background = new SolidColorBrush(Colors.White);
                            }

                            if (Stock_select != stock)
                            {
                                button.Background = new SolidColorBrush(Colors.LightGray);
                                Stock_select = stock;
                            }
                            else
                            {
                                button.Background = new SolidColorBrush(Colors.White);
                                Stock_select = null;
                            }
                            carregar_prods();

                        };

                        if (!lvTalles.Children.OfType<Button>().Any(b => (b.Content as TextBlock)?.Text == textBlock.Text))
                        {
                            lvTalles.Children.Add(button);
                        }

                    }
                }
                tbMostrar3.Text = "-";
                lvTalles.Visibility = Visibility.Visible;
            }
            else
            {
                tbMostrar3.Text = "+";
                lvTalles.Visibility = Visibility.Collapsed;
            }

        }

        private void tbNom_TextChanged(object sender, TextChangedEventArgs e)
        {
            carregar_prods();
        }
        void carregar_prods()
        {
            lvProds.ItemsSource = null;
            lvProds.ItemsSource = mdbService.ProdsFiltrats(Stock_select, Cate_select, ((int)slMinPreu.Value), ((int)slMaxPreu.Value), tbNom.Text);
        }
    }
}
