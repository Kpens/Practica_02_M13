using Gestio_Botiga_Calcat.model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gestio_Botiga_Calcat.View
{
    /// <summary>
    /// ProducteMDBeraction logic for UiProducte.xaml
    /// </summary>
    public partial class UiProducte : UserControl
    {
        VariantMDB Variant_Sel { get; set; } = new VariantMDB();
        List<String> Images { get; set; } = new List<String>();
        List<VariantMDB> Variants { get; set; } = new List<VariantMDB>();

        public UiProducte()
        {
            InitializeComponent();

        }

        public ProducteMDB Prod
        {
            get { return (ProducteMDB)GetValue(ProdProperty); }
            set { SetValue(ProdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Prod.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProdProperty =
            DependencyProperty.Register("Prod", typeof(ProducteMDB), typeof(UiProducte), new PropertyMetadata(null, ProductChangedStatic));

        private static void ProductChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UiProducte p = (UiProducte)d;
            p.ProductChanged(d, e);
        }

        private void ProductChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Variants = Prod.Variants;
            Variant_Sel = Variants.First();


            foreach (VariantMDB variant in Variants) {
                Images.Add(variant.Fotos.First());
            }

            imgs_variants.ItemsSource= Images;
            HtmlDocument desc_html = new HtmlDocument();
            desc_html.LoadHtml(Prod.Desc);

            var body_desc = desc_html.DocumentNode.SelectSingleNode("//body");

            if (body_desc != null)
            {

                Desc = body_desc.InnerText.Substring(0, 30);

            }
            else
            {
                Desc = " ";
            }

            carregar_info();
        }

        public string Desc
        {
            get { return (string)GetValue(DescProperty); }
            set { SetValue(DescProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Desc.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescProperty =
            DependencyProperty.Register("Desc", typeof(string), typeof(UiProducte), new PropertyMetadata(""));


        private void carregar_info()
        {
            imgSel.Source = new BitmapImage(new Uri(Variant_Sel.Fotos.First()));
            double descompte = (Variant_Sel.Preu * Variant_Sel.DescomptePercent) / 100;
            tbDesc.Text = (Variant_Sel.Preu - descompte).ToString("F4");
            tbBase.Text = Variant_Sel.Preu + "";

        }

        private void imgs_variants_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if((imgs_variants.SelectedIndex <= Images.Count)&& (imgs_variants.SelectedIndex >= 0))
            {
                Variant_Sel = Variants[imgs_variants.SelectedIndex];
                carregar_info();
            }
        }
    }
}
