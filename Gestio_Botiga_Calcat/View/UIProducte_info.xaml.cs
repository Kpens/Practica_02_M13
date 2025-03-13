using Gestio_Botiga_Calcat.model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
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
        ProducteMDB prod_select;
        VariantMDB Variant_Sel;
        List<VariantMDB> Variants { get; set; } = new List<VariantMDB>();
        List<String> Images { get; set; } = new List<String>();
        public UIProducte_info(ProducteMDB prod)
        {
            InitializeComponent();
            prod_select = prod;
            Variants= prod.Variants;
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
                tbDescrip.Text = body_desc.InnerText;

            }
            else
            {
                tbDescrip.Text = " ";
            }

            carregar_info();
        }
        void carregar_info()
        {
            tbBase.Text = Variant_Sel.Preu + "";
            double descompte = (Variant_Sel.Preu * Variant_Sel.DescomptePercent) / 100;
            tbDesc.Text = (Variant_Sel.Preu - descompte).ToString("F4");
            tbBase.Text = Variant_Sel.Preu + "";
            List<string> stocks = new List<string>();
            foreach(StockMDB stock in Variant_Sel.Stock)
            {
                if (stock.Quantitat > 0)
                {
                    stocks.Add(stock.Talla+"");
                }
                
            }
            lvTalles.ItemsSource = stocks;

            grFotos.ColumnDefinitions.Clear();
            grFotos.RowDefinitions.Clear();

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
            if (imgs_variants.SelectedIndex is int selected)
            {
                Variant_Sel = Variants[selected];
                carregar_info();
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {

            var newWindow = new MainWindow();

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

    }
}
