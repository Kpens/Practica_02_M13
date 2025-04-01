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
        public CistellMDB cistell;
        public UsuariMDB usuari;
        private Service mdbService;
        public UILogin(UsuariMDB Usuari, CistellMDB cistell)
        {
            InitializeComponent();
            mdbService = new Service("Botiga");
            this.cistell = cistell;
            this.usuari = Usuari;
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
                List<Prod_select> aux = new List<Prod_select>();
                if(cistell != null && cistellUsuari !=null)
                {
                    foreach (Prod_select prod in cistell.Prod_select)
                    {
                        foreach(Prod_select prodUsuari in cistellUsuari.Prod_select)
                        {
                            if (!(prod.Id == prodUsuari.Id && prod.Estoc_id == prodUsuari.Estoc_id))
                            {
                                aux.Add(prod);
                            }
                        }
                    }
                    foreach (Prod_select prod in aux)
                    {
                        cistellUsuari.Prod_select.Add(prod);
                    }
                    cistell.Id = cistellUsuari.Id;
                    cistell.Id_usu = cistellUsuari.Id_usu;
                    cistell.Cost_enviament = cistellUsuari.Cost_enviament;
                    cistell.Metode_enviament = cistellUsuari.Metode_enviament;

                }
                if (cistellUsuari != null)
                {
                    cistell = cistellUsuari;
                    cistell.Id_usu = usuari.Id;
                }
                
                var newWindow = new MainWindow(usuari, cistell);
                this.Close();
                //newWindow.Show();
            }
            else
            {
                MessageBox.Show("Usuari o contrasenya incorrectes");
            }
        }

        private void btSortir_Click(object sender, RoutedEventArgs e)
        {

            //var newWindow = new MainWindow();
            usuari = null;
            this.Close();

            //newWindow.Show();
        }
    }
}
