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
        private bool enfactura = false;
        void carregar_login(UsuariMDB usuari, CistellMDB cistell)
        {
            this.cistellWeb = cistell;
            this.usuari = usuari;

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


        /*void tencar()
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
        }*/
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
            this.enfactura = false;
        }
        public UILogin(bool en_factura)
        {
            InitializeComponent();
            carregar_login();
            this.enfactura = true;
        }


        private void btEntrar_Click(object sender, RoutedEventArgs e)
        {
            if(Global.Usuari == null)
            {
                Global.Usuari = Global.mdbService.GetUsuari(tbLogin.Text, tbContra.Password);
            }
            Global.cistellManager.loginOk(enfactura);

            this.Close();
        }

        private void btSortir_Click(object sender, RoutedEventArgs e)
        {

            Global.cistellManager = null;
            Global.Init();
            this.Close();

        }
    }
}
