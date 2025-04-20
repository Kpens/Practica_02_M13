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
        private List<Metode_enviamentMDB> metodes_Enviament = new List<Metode_enviamentMDB>();
        private double total = 0;
        public void carregar_vista()
        {
            
            if (Global.cistellManager.GetLlistaProds() != new ExtendedObservableCollection<Prod_select>() && Global.cistellManager.GetLlistaProds().Count >0)
            {
                Cistell.Prod_select = Global.cistellManager.GetLlistaProds();
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
                total = 0;
                double[] bases = new double[3];
                ObjectId[] ives = new ObjectId[3];
                foreach (Prod_select prod in Global.cistellManager.GetLlistaProds())
                {
                    VariantMDB variant = Global.mdbService.GetVariantByStockId(prod.Estoc_id);
                    double descompte = ((variant.Preu * prod.Quantitat) * variant.DescomptePercent) / 100;
                    ProducteMDB producte = Global.mdbService.GetProd(prod.Id);
                    if (ives[0] != ObjectId.Empty && ives[0] == producte.Tipus_IVA)
                    {
                        bases[0] += (variant.Preu * prod.Quantitat) - descompte;
                    }
                    else if (ives[1] != ObjectId.Empty && ives[1] == producte.Tipus_IVA)
                    {
                        bases[1] += (variant.Preu * prod.Quantitat) - descompte;
                    }
                    else if (ives[2] != ObjectId.Empty && ives[2] == producte.Tipus_IVA)
                    {
                        bases[2] += (variant.Preu * prod.Quantitat) - descompte;
                    }
                    else
                    {
                        if (ives[0] == ObjectId.Empty)
                        {
                            ives[0] = producte.Tipus_IVA;
                            bases[0] += (variant.Preu * prod.Quantitat) - descompte;
                        }
                        else if (ives[1] == ObjectId.Empty)
                        {
                            ives[1] = producte.Tipus_IVA;
                            bases[1] += (variant.Preu * prod.Quantitat) - descompte;
                        }
                        else if (ives[2] == ObjectId.Empty)
                        {
                            ives[2] = producte.Tipus_IVA;
                            bases[2] += (variant.Preu * prod.Quantitat) - descompte;
                        }
                    }

                }
                tbBasesImp1.Text = bases[0].ToString("F2") + "€";
                tbPercentatge1.Text = Global.mdbService.GetIVA(ives[0]).Percentatge.ToString("F2") + "%:";

                total += bases[0];

                if (ives[1] != ObjectId.Empty)
                {
                    tbBasesImp2.Text = bases[1].ToString("F2") + "€";
                    total += bases[1];
                    tbPercentatge2.Text = Global.mdbService.GetIVA(ives[1]).Percentatge.ToString("F2") + "%:";
                    spBases2.Visibility = Visibility.Visible;
                }
                else
                {
                    tbBasesImp2.Text = "0.00€";
                    tbPercentatge2.Text = "0.00%:";
                    spBases3.Visibility = Visibility.Collapsed;
                }

                if (ives[2] != ObjectId.Empty)
                {
                    tbBasesImp3.Text = bases[2].ToString("F2") + "€";
                    total += bases[2];
                    tbPercentatge3.Text = Global.mdbService.GetIVA(ives[2]).Percentatge.ToString("F2") + "%:";
                    spBases3.Visibility = Visibility.Visible;
                }
                else
                {
                    tbBasesImp3.Text = "0.00€";
                    tbPercentatge3.Text = "0.00%:";
                    spBases3.Visibility = Visibility.Collapsed;
                }   
                    

                List<string> metodes = new List<string>();
                int i = 0;
                bool trobat = false;
                cbMetEnv.ItemsSource=null;
                if(metodes_Enviament.Count ==0)
                {
                    metodes_Enviament = Global.mdbService.GetMetodes_enviament();
                }
                foreach (Metode_enviamentMDB metode in metodes_Enviament)
                {
                    double preu_met = metode.Preu_base;
                    if (metode.Preu_min_compra <= total)
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

                    tbTotal.Text = (total + metodes_Enviament[i].Preu_base).ToString("F2") + "€";
                }
                else
                {
                    tbTotal.Text = total.ToString("F2") + "€";

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
        private void carregar_collectionChanged()
        {

            this.Cistell.Prod_select.ListChanged += Prod_select_ListChanged;


            if (Cistell != null)
            {
                Cistell.Prod_select.CollectionChanged += Prod_select_CollectionChanged;

                foreach (var prod in Cistell.Prod_select)
                {
                    prod.PropertyChanged += Prod_PropertyChanged;
                }

            }

        }
        public UICarro()
        {
            InitializeComponent();
            DataContext = this;
            //this.usu = usu;
            this.Cistell = Global.cistellManager.GetCistell();
            carregar_collectionChanged();
            carregar_vista();

            if (Global.Usuari != null)
            {
                tbNomUsu.Text = Global.Usuari.Nom;
            }
            else
            {
                tbNomUsu.Text = "";
            }
            if (cbMetEnv.SelectedIndex != -1)
            {
                Global.cistellManager.Metode_enviament = metodes_Enviament[cbMetEnv.SelectedIndex];
                btnComprar.IsEnabled = true;
            }
            else
            {
                Global.cistellManager.Metode_enviament = null;
                btnComprar.IsEnabled = false;
            }
        }

        private void Prod_select_ListChanged(object? sender, PropertyChangedEventArgs e)
        {
            foreach (Prod_select prod in Global.cistellManager.GetLlistaProds())
            {
                if (prod.Quantitat == 0)
                {
                    Global.cistellManager.RemoveProd(prod);
                    Global.mdbService.ActualizarCistell();
                    carregar_vista();
                    break;
                }
            }
        }

        private void Prod_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Prod_select.Quantitat))
            {
                carregar_vista();
            }
        }

        private void Prod_select_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            ////Global.mdbService.ActualizarCistell();
            /*foreach(Prod_select prod in Global.cistellManager.GetLlistaProds())
            {
                if(prod.Quantitat == 0)
                {
                    Global.cistellManager.RemoveProd(prod);
                    Global.mdbService.ActualizarCistell();
                }
            }*/
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
                carregar_collectionChanged();
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
            if (Global.Usuari != null)
            {
                Global.cistellManager.ab_logged = true;
            }
            else
            {
                Global.cistellManager.ab_logged = false;
            }

        var newWindow = new WinFactura();
           
            
            this.Close();

            newWindow.Show();
        }

        private void cbMetEnv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbMetEnv.SelectedIndex != -1)
            {
                if (metodes_Enviament[cbMetEnv.SelectedIndex].Preu_min_compra <= total)
                {
                    tbTotal.Text = total.ToString("F2") + "€";
                }
                else
                {
                    tbTotal.Text = (total + metodes_Enviament[cbMetEnv.SelectedIndex].Preu_base).ToString("F2") + "€";
                }
                Global.cistellManager.Metode_enviament = metodes_Enviament[cbMetEnv.SelectedIndex];
                btnComprar.IsEnabled = true;

            }
        }
    }
}
