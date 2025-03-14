using Gestio_Botiga_Calcat.model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Prod_selecteraction logic for UIProducte_carro.xaml
    /// </summary>
    public partial class UIProducte_carro : UserControl
    {
        private Service mdbService;
        private ProducteMDB Prod_act;
        private VariantMDB Variant_act;
        public UIProducte_carro()
        {
            InitializeComponent();
            mdbService = new Service("Botiga");
        }


        public Prod_select Prod_cist
        {
            get { return (Prod_select)GetValue(Prod_cistProperty); }
            set { SetValue(Prod_cistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Prod_cist.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Prod_cistProperty =
            DependencyProperty.Register("Prod_cist", typeof(Prod_select), typeof(UIProducte_carro), new PropertyMetadata(null, ProductChangedStatic));

        private static void ProductChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIProducte_carro p = (UIProducte_carro)d;
            p.ProductChanged(d, e);
        }

        private void ProductChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Prod_act = mdbService.GetProd(Prod_cist.Id);
            Variant_act = mdbService.GetVariantByStockId(Prod_cist.Estoc_id);
            imgSel.Source = new BitmapImage(new Uri(Variant_act.Fotos.First()));
            tbNom.Text = Prod_act.Nom;
            HtmlDocument desc_html = new HtmlDocument();
            desc_html.LoadHtml(Prod_act.Desc);

            var body_desc = desc_html.DocumentNode.SelectSingleNode("//body");

            if (body_desc != null)
            {
                tbDescr.Text = body_desc.InnerText;
            }
            else
            {
                tbDescr.Text = " ";
            }
            carregar_info();
        }

        private void carregar_info()
        {
            imgSel.Source = new BitmapImage(new Uri(Variant_act.Fotos.First()));
            double descompte = (Variant_act.Preu * Variant_act.DescomptePercent) / 100;
            tbDesc.Text = (Variant_act.Preu - descompte).ToString("F2");
            tbBase.Text = Variant_act.Preu + "";

        }
    }
}
