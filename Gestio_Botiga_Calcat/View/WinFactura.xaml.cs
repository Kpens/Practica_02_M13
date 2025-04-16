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
    /// Interaction logic for WinFactura.xaml
    /// </summary>
    public partial class WinFactura : Window
    {
        public WinFactura()
        {
            InitializeComponent();
            if (Global.Usuari != null)
            {
                tbNomUsu.Text = Global.Usuari.Nom;
            }
            else
            {
                /*
                  var winLogin = new UILogin(false);

                  winLogin.Closed += (s, args) =>
                  {

                      tbNomUsu.Text = "Benvingut " + Global.Usuari.Nom + "!";

                  };

                  winLogin.Show();*/
                win_login();
                tbNom.Text = Global.Usuari.Nom;
                tbNomTar.Text = Global.Usuari.Nom;
                tbMail.Text = Global.Usuari.Mail;
                tbTelf.Text = Global.Usuari.Telf;
                tbAdreca.Text = "C/ " + Global.Usuari.Carrer + ", " + Global.Usuari.CodiPostal + ", " + Global.Usuari.Municipi + ", " + Global.Usuari.Pais;
            }


        }

        private void tbNumTar_TextChanged(object sender, TextChangedEventArgs e)
        {


        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {

            //var newWindow = new MainWindow(Global.Usuari, cistell);
            //var newWindow = new MainWindow();

            this.Close();

            //newWindow.Show();
        }

        private void win_login()
        {

            var winLogin = new UILogin();

            //this.Close();
            winLogin.Closed += (s, args) =>
            {
                if (Global.Usuari != null)
                {
                    tbNomUsu.Text = "Benvingut " + Global.Usuari.Nom + "!";
                }
                else
                {
                    tbNomUsu.Text = "";
                    win_login();
                }
            };

            winLogin.Show();

        }

        private void btLogin_Click(object sender, RoutedEventArgs e)
        {
            win_login();
        }
    }
}