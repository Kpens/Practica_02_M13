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
using System.Collections.Specialized;

namespace Gestio_Botiga_Calcat
{
    public partial class MainWindow : Window
    {
        private string inici = "Inici";
        private bool sel_cate_fill = false;
        private CategoriaMDB Cate_select;
        private StockMDB Stock_select;
        private List<ProducteMDB> prods_act = new List<ProducteMDB>();
        private CistellMDB cistell;
        //private UsuariMDB usuari;

        List<VariantMDB> variants = new List<VariantMDB>();

        private void carregar_cates_fill(CategoriaMDB cate)
        {
            List<CategoriaMDB> cates = Global.mdbService.GetCatesFill(cate.Id);

            StackPanel spfilles = new StackPanel();
            spfilles.Orientation = Orientation.Horizontal;

            if (spCatesFill.Children.Count == 1)
            {
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

                    string cate_an = breadcr.Text.Substring(breadcr.Text.LastIndexOf('/') + 1);

                    if (breadcr.Text.LastIndexOf('/') > 0 && sel_cate_fill && cate_an.Trim() != cate.Name.Trim())
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
        void carregar_window()
        {
            variants = Global.mdbService.GetAllVariants();

            slMinPreu.ValueChanged += PriceSlider_ValueChanged;
            slMaxPreu.ValueChanged += PriceSlider_ValueChanged;
            List<CategoriaMDB> cats = Global.mdbService.GetCatesPare();
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
                    breadcr.Text = inici + "/" + cate.Name;
                    carregar_prods();
                    spCatesFill.Children.Clear();
                    carregar_cates_fill(cate);

                };
                spCates.Children.Add(button);
            }
            List<int> num_prods = new List<int>();
            for (int i = 4; i <= 20; i += 4)
            {
                num_prods.Add(i);
            }
            cbNumProds.ItemsSource = num_prods;
            cbNumProds.SelectedIndex = 0;

            carregar_prods();
        }
        private void Prod_select_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            carregarQtProdsCis();
        }

        private void carregar_collectionChanged()
        {

            cistell = Global.cistellManager.GetCistell();
            cistell.Prod_select.CollectionChanged += Prod_select_CollectionChanged;

        }
        public MainWindow()
        {
            InitializeComponent();
            Global.Init();


            //Global.cistellManager.AddProducte()

            carregar_window();
            carregarQtProdsCis();
            carregar_collectionChanged();

            if (Global.Usuari != null)
            {
                tbNomUsu.Text = Global.Usuari.Nom;
            }
            else
            {
                tbNomUsu.Text = "";
            }
        }
        void carregarQtProdsCis()
        {

            /*if (Global.cistellManager != null)
            {*/

            if (Global.cistellManager.GetQtProd() > 0)
            {
                tbNumProds.Visibility = Visibility.Visible;
                tbNumProds.Text = Global.cistellManager.GetQtProd() + "";
            }
            else
            {
                tbNumProds.Visibility = Visibility.Collapsed;
            }
            /*}
            else
            {
                tbNumProds.Visibility = Visibility.Collapsed;
            }*/
        }
        /*public MainWindow(UsuariMDB usu, CistellMDB cistell)
        {
            InitializeComponent();

            carregar_window();
            usuari = usu;
            this.cistell = cistell;
            carregarQtProdsCis();


            if (usu != null)
            {
                tbNomUsu.Text = usu.Nom;
            }
            else
            {
                tbNomUsu.Text = "";
            }
        }*/

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
                wpProds.Children.Clear();
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
            tbMax.Text = slMaxPreu.Value + "€";
            tbMin.Text = slMinPreu.Value + "€";
            txbnum.Text = "0";
            carregar_prods();

        }

        private void spTalles_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lvTalles.Visibility == Visibility.Collapsed)
            {
                List<StockMDB> stocks = new List<StockMDB>();

                foreach (VariantMDB variant in variants)
                {
                    stocks.AddRange(variant.Stock);
                }


                lvTalles.Children.Clear();

                foreach (StockMDB stock in stocks.OrderBy(stock => stock.Talla).ToList())
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
                        VerticalAlignment = VerticalAlignment.Center,
                        FontFamily = new FontFamily("Agency FB"),
                        FontSize = 15
                    };

                    button.Content = textBlock;

                    button.Click += (s, e) =>
                    {
                        foreach (Button but in lvTalles.Children)
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
                        txbnum.Text = "0";
                        carregar_prods();

                    };

                    if (!lvTalles.Children.OfType<Button>().Any(b => (b.Content as TextBlock)?.Text == textBlock.Text))
                    {
                        lvTalles.Children.Add(button);
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
            txbnum.Text = "0";
            carregar_prods();
        }
        void carregar_prods()
        {
            wpProds.Children.Clear();
            int num_prods = (int)cbNumProds.SelectedValue;

            prods_act = Global.mdbService.ProdsFiltrats(Stock_select, Cate_select, ((int)slMinPreu.Value), ((int)slMaxPreu.Value), tbNom.Text);

            int prod_pos = 0;
            List<ProducteMDB> prods = new List<ProducteMDB>();
            if (prods_act.Count >= num_prods)
            {
                int num = int.Parse(txbnum.Text) * num_prods;
                if (prods_act.Count == num)
                {
                    num -= num_prods;
                    txbnum.Text = (int.Parse(txbnum.Text) - 1).ToString();
                }
                for (int i = num; i < ((int.Parse(txbnum.Text) * num_prods) + num_prods); i++)
                {
                    if (prods_act.Count <= i)
                    {
                        break;
                    }
                    prods.Add(prods_act[i]);

                }
                foreach (var prod in prods)
                {

                    UiProducte prod_ui = new UiProducte { Prod = prod };
                    prod_ui.MouseLeftButtonDown += (s, e) =>
                    {
                        ProdSelect(prod);
                    };
                    wpProds.Children.Add(prod_ui);

                    prod_pos++;
                }
            }
            else
            {
                foreach (var prod in prods_act)
                {

                    UiProducte prod_ui = new UiProducte { Prod = prod };
                    prod_ui.MouseLeftButtonDown += (s, e) =>
                    {
                        ProdSelect(prod);
                    };
                    wpProds.Children.Add(prod_ui);

                    prod_pos++;
                }
            }
        }

        private void ProdSelect(ProducteMDB product)
        {
            //var newWindow = new UIProducte_info(product, usuari, cistell);
            var newWindow = new UIProducte_info(product);

            newWindow.Closed += (s, args) =>
            {
                carregar_collectionChanged();
                carregarQtProdsCis();
            };
            //this.Close();

            newWindow.Show();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txbnum.Text = "0";
            carregar_prods();
        }

        private void Label_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Cate_select = null;
            breadcr.Text = inici;
            carregar_prods();
            spCatesFill.Children.Clear();
        }


        private void btnesq_Click(object sender, RoutedEventArgs e)
        {
            if (txbnum.Text != "0")
            {
                txbnum.Text = (int.Parse(txbnum.Text) - 1).ToString();
                carregar_prods();
            }

        }

        private void btnesq_top_Click(object sender, RoutedEventArgs e)
        {
            txbnum.Text = "0";
            carregar_prods();
        }
        private void btndret_Click(object sender, RoutedEventArgs e)
        {
            int num = prods_act.Count / (int)cbNumProds.SelectedValue;
            if (int.Parse(txbnum.Text) < num)
            {
                txbnum.Text = (int.Parse(txbnum.Text) + 1).ToString();
                carregar_prods();
            }
        }

        private void btndret_top_Click(object sender, RoutedEventArgs e)
        {
            int num = prods_act.Count / (int)cbNumProds.SelectedValue;
            txbnum.Text = num + "";
            carregar_prods();

        }

        private void btnCart_Click(object sender, RoutedEventArgs e)
        {
            //var newWindow = new UICarro(usuari, cistell);
            var newWindow = new UICarro();

            //this.Close();
            newWindow.Closed += (s, args) =>
            {
                carregar_collectionChanged();
                carregarQtProdsCis();
                if (Global.Usuari != null)
                {
                    tbNomUsu.Text = Global.Usuari.Nom;
                }
                else
                {
                    tbNomUsu.Text = "";
                }
            };

            newWindow.Show();

        }

        private void btLogin_Click(object sender, RoutedEventArgs e)
        {
            var winLogin = new UILogin();

            //this.Close();
            winLogin.Closed += (s, args) =>
            {
                //usuari = winLogin.usuari;
                /*
                if (winLogin.cistellWeb != null)
                {
                    if (cistell == null)
                    {
                        cistell = new CistellMDB();
                    }
                    if (cistell.Prod_select == null || cistell.Prod_select.Count == 0)
                    {
                        cistell.Prod_select = new System.Collections.ObjectModel.ObservableCollection<Prod_select>();
                    }
                    cistell.Id = winLogin.cistellWeb.Id;
                    cistell.Id_usu = winLogin.cistellWeb.Id_usu;
                    cistell.Cost_enviament = winLogin.cistellWeb.Cost_enviament;
                    cistell.Metode_enviament = winLogin.cistellWeb.Metode_enviament;

                    cistell.Prod_select.Clear();
                    foreach (Prod_select prod in winLogin.cistellWeb.Prod_select)
                    {
                        cistell.Prod_select.Add(prod);
                    }*/
                /*

                foreach (Prod_select prod in winLogin.cistell.Prod_select)
                {
                    if(!cistell.Prod_select.Contains(prod))
                    {
                        cistell.Prod_select.Add(prod);
                    }
                }
                 */

                carregar_collectionChanged();
                carregarQtProdsCis();
                //}
                if (Global.Usuari != null)
                {
                    tbNomUsu.Text = "Benvingut " + Global.Usuari.Nom + "!";
                }
                else
                {
                    tbNomUsu.Text = "";
                }
            };

            winLogin.Show();
        }
    }
}