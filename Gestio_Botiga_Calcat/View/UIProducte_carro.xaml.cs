using Gestio_Botiga_Calcat.model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private StockMDB Stock_act;
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
            tbNum.Text = Prod_cist.Quantitat.ToString();

            Stock_act = mdbService.GetStockById(Prod_cist.Estoc_id);

            tbDescr.Text = "Color: "+Variant_act.Color+", Talla: "+Stock_act.Talla;


            if(tbNum.Text == "1")           
            {
                btRes.Foreground = new SolidColorBrush(Colors.LightGray);
            }
            else if(tbNum.Text == Stock_act.Quantitat.ToString())
            {
                tbSum.Foreground = new SolidColorBrush(Colors.LightGray);
            }
            carregar_info();
        }

        private void carregar_info()
        {
            imgSel.Source = new BitmapImage(new Uri(Variant_act.Fotos.First()));
            double descompte = ((Variant_act.Preu* int.Parse( tbNum.Text)) * Variant_act.DescomptePercent) / 100;
            tbDesc.Text = ((Variant_act.Preu * int.Parse(tbNum.Text)) - descompte).ToString("F2") + "€";
            if (Variant_act.DescomptePercent != 0)
            {
                tbBase.Text = (Variant_act.Preu * int.Parse(tbNum.Text)).ToString("F2") + "€";
            }
            else
            {
                tbBase.Text = "";
            }

        }

        private void btRes_Click(object sender, RoutedEventArgs e)
        {
            tbSum.Foreground = new SolidColorBrush(Colors.Black);
            if (int.Parse(tbNum.Text) > 1)
            {
                tbNum.Text = (int.Parse(tbNum.Text) - 1).ToString();
                if (tbNum.Text == "1")
                {
                    btRes.Foreground = new SolidColorBrush(Colors.LightGray);
                }
                else
                {
                    btRes.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
            else
            {
                btRes.Foreground = new SolidColorBrush(Colors.LightGray);
            }
            carregar_info();
        }

        private void tbSum_Click(object sender, RoutedEventArgs e)
        {
            btRes.Foreground = new SolidColorBrush(Colors.Black);
            if (tbNum.Text != Stock_act.Quantitat.ToString())
            {
                tbNum.Text = (int.Parse(tbNum.Text) + 1).ToString();
                if (tbNum.Text == Stock_act.Quantitat.ToString())
                {
                    tbSum.Foreground = new SolidColorBrush(Colors.LightGray);
                }
                else
                {
                    tbSum.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
            else
            {
                tbSum.Foreground = new SolidColorBrush(Colors.LightGray);
            }
            carregar_info();

        }

        private void btDel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Estas segur que vols eliminar aquest producte de la cistella?", "Eliminar", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show("Eliminat!");
            }
        }
    }
}
