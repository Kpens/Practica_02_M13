using Gestio_Botiga_Calcat.model;
using HtmlAgilityPack;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace Gestio_Botiga_Calcat.View
{
    /// <summary>
    /// Interaction logic for UIProducte_info.xaml
    /// </summary>
    public partial class UIProducte_info : Window
    {
        private ProducteMDB prod_select;
        private VariantMDB Variant_Sel;
        private StockMDB Stock_select;
        List<VariantMDB> Variants { get; set; } = new List<VariantMDB>();
        List<String> Images { get; set; } = new List<String>();
        private CistellMDB cistell = new CistellMDB();
        private UsuariMDB usuari = new UsuariMDB();
        void carregar_window(ProducteMDB prod)
        {
            prod_select = prod;
            Variants = prod.Variants;
            Variant_Sel = Variants.First();
            tbMarca.Text = prod.Marca;
            tbNom.Text = prod.Nom;

            foreach (VariantMDB variant in Variants)
            {
                Images.Add(variant.Fotos.First());
            }

            imgs_variants.ItemsSource = Images;


            HtmlDocument desc_html = new HtmlDocument();
            desc_html.LoadHtml(prod.Desc);

            var body_desc = desc_html.DocumentNode.SelectSingleNode("//body");

            if (body_desc != null)
            {
                spDesc.Visibility = Visibility.Visible;
                tbDescrip.Text = body_desc.InnerText.Trim();

            }
            else
            {
                tbDescrip.Text = " ";
            }

            carregar_info();
        }
        /*public UIProducte_info(ProducteMDB prod)
        {
            InitializeComponent();
            carregar_window(prod);

        }*/
        public UIProducte_info(ProducteMDB prod, UsuariMDB usu, CistellMDB cistell)
        {
            InitializeComponent();
            carregar_window(prod);
            this.usuari = usu;
            this.cistell = cistell;
            if (cistell != null)
            {
                tbNumProds.Visibility = Visibility.Visible;
                tbNumProds.Text = cistell.Prod_select.Count + "";
            }
            else
            {
                tbNumProds.Visibility = Visibility.Collapsed;
            }

        }
        void carregar_info()
        {
            double descompte = (Variant_Sel.Preu * Variant_Sel.DescomptePercent) / 100;
            tbDesc.Text = (Variant_Sel.Preu - descompte).ToString("F2") + "€";
            if (Variant_Sel.DescomptePercent != 0)
            {
                tbBase.Text = Variant_Sel.Preu + "€";
            }
            else
            {
                tbBase.Text = "";
            }


            lvTalles.Children.Clear();
            tbNom_var.Text = Variant_Sel.Color;
            foreach (StockMDB stock in Variant_Sel.Stock)
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
                    FontSize = 18
                };
                if (stock.Quantitat <= 0)
                { 
                    textBlock.Foreground = new SolidColorBrush(Colors.Gray);
                    textBlock.TextDecorations = TextDecorations.Strikethrough;
                    button.IsEnabled = false;

                }

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

                };
                button.Content = textBlock;
                lvTalles.Children.Add(button);

            }

            grFotos.ColumnDefinitions.Clear();
            grFotos.RowDefinitions.Clear();
            grFotos.Children.Clear();

            grFotos.ColumnDefinitions.Add(new ColumnDefinition());
            if (Variant_Sel.Fotos.Count > 2) {

                grFotos.ColumnDefinitions.Add(new ColumnDefinition());
            }
            grFotos.RowDefinitions.Add(new RowDefinition());
            if (Variant_Sel.Fotos.Count > 1) { 
                grFotos.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < Variant_Sel.Fotos.Count; i++)
            {
                System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                img.Source = new BitmapImage(new Uri(Variant_Sel.Fotos[i]));

                Grid.SetColumn(img, i % 2);
                Grid.SetRow(img, i / 2);

                grFotos.Children.Add(img);
            }

        }

        private void imgs_variants_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((imgs_variants.SelectedIndex <= Images.Count) && (imgs_variants.SelectedIndex >= 0))
            {
                Variant_Sel = Variants[imgs_variants.SelectedIndex];
                carregar_info();
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {

            var newWindow = new MainWindow(usuari, cistell);

            this.Close();

            newWindow.Show();
        }
        private void spDesc_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (tbDescrip.Visibility == Visibility.Collapsed)
            {
                tbMostrar.Text = "-";
                tbDescrip.Visibility = Visibility.Visible;
            }
            else
            {
                tbMostrar.Text = "+";
                tbDescrip.Visibility = Visibility.Collapsed;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Stock_select != null && Variant_Sel != null)
            {
                Prod_select prod_sel = new Prod_select
                {
                    Id = prod_select.Id,
                    Estoc_id = Stock_select.Id,
                    Quantitat = 1
                };
                if (cistell == null)
                {
                    cistell = new CistellMDB();
                }
                if (cistell.Prod_select == null)
                {
                    cistell.Prod_select = new ObservableCollection<Prod_select>();
                }

                foreach(Prod_select prod in cistell.Prod_select)
                {
                    if (prod.Id == prod_sel.Id && prod.Estoc_id == prod_sel.Estoc_id)
                    {
                        return;
                    }
                }

                cistell.Prod_select.Add(prod_sel);
                tbNumProds.Visibility = Visibility.Visible;
                tbNumProds.Text = cistell.Prod_select.Count + "";
                
            }

        }


        private void btnCart_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new UICarro(usuari, cistell);

            this.Close();

            newWindow.Show();

        }
        private void btLogin_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new UILogin(usuari, cistell);

            this.Close();

            newWindow.Show();

        }
    }
}
