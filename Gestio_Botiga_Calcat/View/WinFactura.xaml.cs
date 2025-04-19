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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gestio_Botiga_Calcat.View
{
    /// <summary>
    /// Interaction logic for WinFactura.xaml
    /// </summary>
    public partial class WinFactura : Window
    {
        private string imgs = "/imgs/";
        private bool es_pot_tencar = false;
        UILogin winLogin;
        private void carregar_info()
        {

            if (Global.Usuari != null)
            {
                tbNom.Text = Global.Usuari.Nom;
                tbNomTar.Text = Global.Usuari.Nom;
                tbMail.Text = Global.Usuari.Mail;
                tbTelf.Text = Global.Usuari.Telf;
                tbAdreca.Text = Global.Usuari.Carrer + ", " + Global.Usuari.CodiPostal + ", " + Global.Usuari.Municipi + ", " + Global.Usuari.Pais;
                grUsu.Visibility = Visibility.Visible;
                spNoProds.Visibility = Visibility.Collapsed;
            }
            else
            {
                grUsu.Visibility = Visibility.Collapsed;
                spNoProds.Visibility = Visibility.Visible;
                win_login();

            }
        }
        public WinFactura()
        {
            InitializeComponent();
            carregar_info();

        }

        private void tbNumTar_TextChanged(object sender, TextChangedEventArgs e)
        {
            imgTar.Source = new BitmapImage(new Uri(imgs + "mastercard.png", UriKind.Relative));

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {

            //var newWindow = new MainWindow(Global.Usuari, cistell);
            //var newWindow = new MainWindow();
            es_pot_tencar = true;
            winLogin.Close();
            this.Close();

            //newWindow.Show();
        }

        private void win_login()
        {

            winLogin = new UILogin();

            //this.Close();
            winLogin.Closed += (s, args) =>
            {
                if(es_pot_tencar)
                {
                    this.Close();
                }
                else
                {
                    carregar_info();
                }
            };

            winLogin.Show();

        }

        private void btLogin_Click(object sender, RoutedEventArgs e)
        {
            win_login();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            //Global.mdbService.CancelarCompra(factura);

            //var newWindow = new UICarro();
            es_pot_tencar = true;
            this.Close();

            //newWindow.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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
        private void btnComprar_Click(object sender, RoutedEventArgs e)
        {
            //Global.mdbService.CrearFactura(factura);
            MessageBox.Show("Gràcies per la seva compra!", "Compra realitzada", MessageBoxButton.OK, MessageBoxImage.Information);

        }
    }
}