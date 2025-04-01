using Gestio_Botiga_Calcat.model;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Intrinsics.X86;
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
            
            if (Cistell != null && Cistell.Prod_select.Count >0)
            {
                lvProds_cist.ItemsSource = null;
                lvProds_cist.ItemsSource = Cistell.Prod_select;
                grDetalls.Visibility = Visibility.Visible;
                spNoProds.Visibility = Visibility.Collapsed;

                if (usu != null)
                {
                    mdbService.ActualizarCistell(Cistell);
                }

                double bases = 0;
                foreach (Prod_select prod in Cistell.Prod_select)
                {
                    VariantMDB variant = mdbService.GetVariantByStockId(prod.Estoc_id);
                    double descompte = ((variant.Preu * prod.Quantitat) * variant.DescomptePercent) / 100;
                    bases += (variant.Preu * prod.Quantitat) - descompte;
                }
                tbBasesImp.Text = bases.ToString("F2") + "€";
            }else
            {
                lvProds_cist.Visibility = Visibility.Collapsed;
                spNoProds.Visibility = Visibility.Visible;
                tbTit.Visibility = Visibility.Collapsed;
                grDetalls.Visibility = Visibility.Collapsed;
            }
        }

        public UICarro(UsuariMDB usu, CistellMDB Cistell)
        {
            InitializeComponent();
            DataContext = this;
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
            if (Cistell != null)
            {
                Cistell.Prod_select.CollectionChanged += Prod_select_CollectionChanged;

            }
           
            if (usu != null)
            {
                if(Cistell != null)
                {
                    foreach (var prod in Cistell.Prod_select)
                    {
                        prod.PropertyChanged += Prod_PropertyChanged;
                    }
                }
                
            }

            if (usu != null)
            {
                tbNomUsu.Text = "Benvingut " + usu.Nom + "!";
            }
            else
            {
                tbNomUsu.Text = "";
            }
        }

        private void Prod_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Prod_select.Quantitat))
            {
                mdbService.ActualizarCistell(Cistell);
            }
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
            var winLogin = new UILogin(usu, Cistell);

            winLogin.Closed += (s, args) =>
            {
                usu = winLogin.usuari;
                
                if (winLogin.cistell != null)
                {
                    if (Cistell == null)
                    {
                        Cistell = new CistellMDB();
                    }
                    if(Cistell.Prod_select == null)
                    {
                        Cistell.Prod_select = new System.Collections.ObjectModel.ObservableCollection<Prod_select>();
                    }
                    Cistell.Id = winLogin.cistell.Id;
                    Cistell.Id_usu = winLogin.cistell.Id_usu;
                    Cistell.Cost_enviament = winLogin.cistell.Cost_enviament;
                    Cistell.Metode_enviament = winLogin.cistell.Metode_enviament;

                    Cistell.Prod_select.Clear();
                    foreach (Prod_select prod in winLogin.cistell.Prod_select)
                    {
                        Cistell.Prod_select.Add(prod);
                    }
                }


                if (usu != null)
                {
                    tbNomUsu.Text = "Benvingut " + usu.Nom + "!";
                }
                else
                {
                    tbNomUsu.Text = "";
                }
                var newWindow = new UICarro(usu, Cistell);

                this.Close();

                newWindow.Show();
            };

            winLogin.Show();
        }
    }
}
