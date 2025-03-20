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
        private CistellMDB cistell;
        private UsuariMDB usuari;
        private Service mdbService;
        public UILogin(UsuariMDB Usuari, CistellMDB cistell)
        {
            InitializeComponent();
            mdbService = new Service("Botiga");
            this.cistell = cistell;
            this.usuari = Usuari;
            if (cistell != null)
            {
                tbNumProds.Visibility = Visibility.Visible;
                tbNumProds.Text = cistell.Prod_select.Count + "";
            }
            else
            {
                tbNumProds.Visibility = Visibility.Collapsed;
            }

            if(Usuari != null)
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


        }

        private void btEntrar_Click(object sender, RoutedEventArgs e)
        {
            usuari = mdbService.GetUsuari(tbLogin.Text, tbContra.Password);
            if (usuari != null)
            {
                CistellMDB cistellUsuari = mdbService.GetCistell(usuari.Id);
                if(cistell != null && cistellUsuari !=null)
                {
                    foreach (Prod_select prod in cistell.Prod_select)
                    {
                        cistellUsuari.Prod_select.Add(prod);
                    }
                }
                cistell = cistellUsuari;
                cistell.Id_usu = usuari.Id;
                var newWindow = new MainWindow(usuari, cistell);
                this.Close();
                newWindow.Show();
            }
            else
            {
                MessageBox.Show("Usuari o contrasenya incorrectes");
            }
        }

        private void btSortir_Click(object sender, RoutedEventArgs e)
        {

            var newWindow = new MainWindow();

            this.Close();

            newWindow.Show();
        }
    }
}
