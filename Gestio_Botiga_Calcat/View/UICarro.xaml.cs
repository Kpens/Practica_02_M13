using Gestio_Botiga_Calcat.model;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace Gestio_Botiga_Calcat.View
{
    /// <summary>
    /// Interaction logic for UICarro.xaml
    /// </summary>
    public partial class UICarro : Window
    {
        private UsuariMDB usu;
        private Service mdbService;

        public void carregar_vista()
        {

            if (Cistell.Prod_select.Count ==0)
            {
                lvProds_cist.Visibility = Visibility.Collapsed;
                spNoProds.Visibility = Visibility.Visible;
                tbTit.Visibility = Visibility.Collapsed;
                grDetalls.Visibility = Visibility.Collapsed;
            }
            else if (Cistell != null)
            {
                lvProds_cist.ItemsSource = null;
                lvProds_cist.ItemsSource = Cistell.Prod_select;
                grDetalls.Visibility = Visibility.Visible;
                double bases= 0;
                foreach (Prod_select prod in Cistell.Prod_select)
                {
                    VariantMDB variant = mdbService.GetVariantByStockId(prod.Estoc_id);
                    double descompte = ((variant.Preu * prod.Quantitat) * variant.DescomptePercent) / 100;
                    bases += (variant.Preu * prod.Quantitat) - descompte;
                }
                tbBasesImp.Text = bases.ToString("F2") + "€";
            }
        }

        public UICarro(UsuariMDB usu, CistellMDB Cistell)
        {
            DataContext = this;
            InitializeComponent();
            mdbService = new Service("Botiga");
            this.usu = usu;
            this.Cistell = Cistell;
            carregar_vista();

            List<string> metodes = new List<string>();
            int i = 0;
            bool trobat = false;
            foreach (Metode_enviamentMDB metode in mdbService.GetMetodes_enviament())
            {

                metodes.Add(metode.Nom + " (De " + metode.MinTemps_en_dies + " a " + metode.MaxTemps_en_dies + " dies laborals) " + metode.Preu_base + "€");
                if (Cistell != null && Cistell.Metode_enviament != ObjectId.Empty && metode.Id == Cistell.Metode_enviament)
                {
                    trobat = true;
                }
                if (trobat == false) { 
                    i++;
                }
                
            }

            cbMetEnv.ItemsSource = metodes;
            if (trobat)
            {
                cbMetEnv.SelectedIndex = i;
            }
            Cistell.Prod_select.CollectionChanged += Prod_select_CollectionChanged;
        }

        private void Prod_select_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            carregar_vista();
        }

        public CistellMDB Cistell
        {
            get { return (CistellMDB)GetValue(CistellProperty); }
            set { SetValue(CistellProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Cistell.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CistellProperty =
            DependencyProperty.Register("Cistell", typeof(CistellMDB), typeof(UICarro), new PropertyMetadata(null));

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {

            var newWindow = new MainWindow(usu, Cistell);

            this.Close();

            newWindow.Show();
        }

        private void lvProds_cist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lvProds_cist.SelectedValue is Prod_select selected)
            {
                
                ProducteMDB prod =mdbService.GetProd(selected.Id);
                var newWindow = new UIProducte_info(prod,usu, Cistell);

                this.Close();

                newWindow.Show();
            }
        }
        private void btLogin_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new UILogin(usu, Cistell);

            this.Close();

            newWindow.Show();

        }
    }
}
