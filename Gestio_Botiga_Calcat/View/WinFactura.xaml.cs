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
                var winLogin = new UILogin(false);

                winLogin.Closed += (s, args) =>
                {

                    tbNomUsu.Text = "Benvingut " + Global.Usuari.Nom + "!";
                    
                };

                winLogin.Show();
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {

            //var newWindow = new MainWindow(Global.Usuari, cistell);
            //var newWindow = new MainWindow();

            this.Close();

            //newWindow.Show();
        }
        private void btLogin_Click(object sender, RoutedEventArgs e)
        {
            var winLogin = new UILogin();

            //this.Close();
            winLogin.Closed += (s, args) =>
            {
                //Global.Usuari = winLogin.Global.Usuari;
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
    }
}
