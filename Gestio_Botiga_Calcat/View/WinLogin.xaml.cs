using Gestio_Botiga_Calcat.model;
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
using System.Windows.Shapes;

namespace Gestio_Botiga_Calcat.View
{
    /// <summary>
    /// Interaction logic for UILogin.xaml
    /// </summary>
    public partial class UILogin : Window
    {
        public CistellMDB cistellWeb;
        public CistellMDB cistellUsuari;
        public UsuariMDB usuari;
        private bool es_pot_tencar = true;
        void carregar_login(UsuariMDB usuari, CistellMDB cistell)
        {
            this.cistellWeb = cistell;
            this.usuari = usuari;
            /*
                        if (cistell != null)
                        {
                            if (this.cistell == null)
                            {
                                this.cistell = new CistellMDB();
                            }
                            if (this.cistell.Prod_select == null)
                            {
                                this.cistell.Prod_select = new System.Collections.ObjectModel.ObservableCollection<Prod_select>();
                            }
                            this.cistell.Id = cistell.Id;
                            this.cistell.Id_usu = cistell.Id_usu;
                            this.cistell.Cost_enviament = cistell.Cost_enviament;
                            this.cistell.Metode_enviament = cistell.Metode_enviament;

                            this.cistell.Prod_select.Clear();
                            foreach (Prod_select prod in cistell.Prod_select)
                            {
                                this.cistell.Prod_select.Add(prod);
                            }
                        }*/

            /*if (cistell != null)
            {
                tbNumProds.Visibility = Visibility.Visible;
                if(cistell.Prod_select.Count > 0)
                {
                    tbNumProds.Text = cistell.Prod_select.Count + "";
                }
                else
                {
                    tbNumProds.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                tbNumProds.Visibility = Visibility.Collapsed;
            }*/

            if (usuari != null)
            {
                spNoProds.Visibility = Visibility.Visible;
                grIniSes.Visibility = Visibility.Collapsed;
            }
            else
            {
                spNoProds.Visibility = Visibility.Collapsed;
                grIniSes.Visibility = Visibility.Visible;
            }
        }


        void tencar()
        {
            if (es_pot_tencar)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                this.Close();
            }
            else
            {
                WindowStyle = WindowStyle.None;
            }
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (es_pot_tencar)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }
        void carregar_login()
        {

            if (Global.Usuari != null)
            {
                spNoProds.Visibility = Visibility.Visible;
                grIniSes.Visibility = Visibility.Collapsed;
            }
            else
            {
                spNoProds.Visibility = Visibility.Collapsed;
                grIniSes.Visibility = Visibility.Visible;
            }
        }
        public UILogin()
        {
            InitializeComponent();
            carregar_login();
        }
        /*
        public UILogin(UsuariMDB usuari, CistellMDB cistell)
        {
            InitializeComponent();
            carregar_login(usuari, cistell);
        }*/
        public UILogin(UsuariMDB usuari, CistellMDB cistell, bool en_factura)
        {
            InitializeComponent();
            carregar_login(usuari, cistell);
            this.es_pot_tencar = false;
        }
        /*
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {

            var newWindow = new MainWindow(usuari, cistell);

            this.Close();

            newWindow.Show();
        }
        private void btnCart_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new UICarro(usuari, cistell);

            this.Close();

            newWindow.Show();


        }*/



        private void btEntrar_Click(object sender, RoutedEventArgs e)
        {
            if(Global.Usuari == null)
            {
                Global.Usuari = Global.mdbService.GetUsuari(tbLogin.Text, tbContra.Password);
            }
            Global.cistellManager.loginOk();

            //usuari = Global.mdbService.GetUsuari(tbLogin.Text, tbContra.Password);
            //if (usuari == null)
            //{
            //    MessageBox.Show("Usuari o contrasenya incorrectes");
            //    return;
            //}

            //CistellMDB cistellBd = Global.mdbService.GetCistell(usuari.Id);

            //if (cistellWeb != null && cistellBd != null)
            //{
            //    foreach (Prod_select prodWeb in cistellWeb.Prod_select)
            //    {
            //        int productesIguals = cistellBd.Prod_select.Where(prodBd => prodWeb.Id == prodBd.Id && prodWeb.Estoc_id == prodBd.Estoc_id).Count();

            //        if(productesIguals == 0)
            //        {
            //            cistellBd.Prod_select.Add(prodWeb);
            //        }

            //    }

            //    cistellWeb.Id = cistellBd.Id;
            //    cistellWeb.Id_usu = cistellBd.Id_usu;
            //    cistellWeb.Cost_enviament = cistellBd.Cost_enviament;
            //    cistellWeb.Metode_enviament = cistellBd.Metode_enviament;
            //    Global.mdbService.ActualizarCistell(cistellBd);
            //}

            //if (cistellBd != null)
            //{
            //    cistellWeb = cistellBd;
            //    cistellWeb.Id_usu = usuari.Id;
            //}

            //var newWindow = new MainWindow(usuari, cistellWeb);
            //var newWindow = new MainWindow();
            this.Close();
        }

        private void btSortir_Click(object sender, RoutedEventArgs e)
        {

            //var newWindow = new MainWindow();
            //Global.Usuari = null;
            Global.cistellManager = null;
            Global.Init();
            this.Close();

            //newWindow.Show();
        }
    }
}
