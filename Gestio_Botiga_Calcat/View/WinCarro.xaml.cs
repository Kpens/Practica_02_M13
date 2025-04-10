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
        //private UsuariMDB usu;

        public void carregar_vista()
        {
            
            if (Cistell != null && Cistell.Prod_select.Count >0)
            {
                lvProds_cist.ItemsSource = null;
                lvProds_cist.ItemsSource = Cistell.Prod_select;
                grDetalls.Visibility = Visibility.Visible;
                spNoProds.Visibility = Visibility.Collapsed;
                lvProds_cist.Visibility = Visibility.Visible;
                tbTit.Visibility = Visibility.Visible;

                if (Global.Usuari != null)
                {
                    Global.mdbService.ActualizarCistell();
                }

                double bases = 0;
                foreach (Prod_select prod in Cistell.Prod_select)
                {
                    VariantMDB variant = Global.mdbService.GetVariantByStockId(prod.Estoc_id);
                    double descompte = ((variant.Preu * prod.Quantitat) * variant.DescomptePercent) / 100;
                    bases += (variant.Preu * prod.Quantitat) - descompte;
                }
                tbBasesImp.Text = bases.ToString("F2") + "€";

                List<string> metodes = new List<string>();
                int i = 0;
                bool trobat = false;
                cbMetEnv.ItemsSource=null;
                foreach (Metode_enviamentMDB metode in Global.mdbService.GetMetodes_enviament())
                {
                    double preu_met = metode.Preu_base;
                    if (metode.Preu_min_compra <= bases)
                    {
                        preu_met = 0;
                    }
                    metodes.Add(metode.Nom + " (De " + metode.MinTemps_en_dies + " a " + metode.MaxTemps_en_dies + " dies laborals) " + preu_met  + "€");
                    if (Cistell != null && Cistell.Metode_enviament != ObjectId.Empty && metode.Id == Cistell.Metode_enviament)
                    {
                        trobat = true;
                    }
                    if (trobat == false)
                    {
                        i++;
                    }
                }
                cbMetEnv.ItemsSource = metodes;
                if (trobat)
                {
                    cbMetEnv.SelectedIndex = i;
                }
            }
            else
            {
                lvProds_cist.Visibility = Visibility.Collapsed;
                spNoProds.Visibility = Visibility.Visible;
                tbTit.Visibility = Visibility.Collapsed;
                grDetalls.Visibility = Visibility.Collapsed;
            }
        }

        public UICarro()
        {
            InitializeComponent();
            DataContext = this;
            //this.usu = usu;
            this.Cistell = Global.cistellManager.GetCistell();
            this.Cistell.Prod_select.ListChanged += Prod_select_ListChanged; 

            carregar_vista();

            if (Cistell != null)
            {
                Cistell.Prod_select.CollectionChanged += Prod_select_CollectionChanged;

            }
           
            if (Global.Usuari != null)
            {
                if(Cistell != null)
                {
                    foreach (var prod in Cistell.Prod_select)
                    {
                        prod.PropertyChanged += Prod_PropertyChanged;
                    }
                }
                
            }

            if (Global.Usuari != null)
            {
                tbNomUsu.Text = Global.Usuari.Nom;
            }
            else
            {
                tbNomUsu.Text = "";
            }
        }

        private void Prod_select_ListChanged(object? sender, PropertyChangedEventArgs e)
        {
            Global.mdbService.ActualizarCistell();
            carregar_vista();
        }

        private void Prod_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Prod_select.Quantitat))
            {
                Global.mdbService.ActualizarCistell();
            }
        }

        private void Prod_select_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            ////Global.mdbService.ActualizarCistell();
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

            //var newWindow = new MainWindow(usu, Cistell);
            //var newWindow = new MainWindow();

            this.Close();

            //newWindow.Show();
        }

        private void lvProds_cist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lvProds_cist.SelectedValue is Prod_select selected)
            {
                
                ProducteMDB prod =Global.mdbService.GetProd(selected.Id);
                //var newWindow = new UIProducte_info(prod,usu, Cistell);

                var newWindow = new UIProducte_info(prod);
                this.Close();

                newWindow.Show();
            }
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
                //carregarQtProdsCis();
                //}
                this.Cistell = Global.cistellManager.GetCistell();
                carregar_vista();
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

        private void btnComprar_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new WinFactura();

            this.Close();

            newWindow.Show();
        }
    }
}
