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
    /// Interaction logic for UICarro.xaml
    /// </summary>
    public partial class UICarro : Window
    {
        private UsuariMDB usu;
        private CistellMDB cistell;
        private Service mdbService;
        public UICarro(UsuariMDB usu, CistellMDB cistell)
        {
            InitializeComponent();
            mdbService = new Service("Botiga");
            if (usu.Login == null && cistell.Prod_select == null) {
                lvProds_cist.Visibility = Visibility.Collapsed;
                spNoProds.Visibility = Visibility.Visible;
                tbTit.Visibility = Visibility.Collapsed;
                grDetalls.Visibility = Visibility.Collapsed;
            }
            else if(cistell.Prod_select != null)
            {
                lvProds_cist.ItemsSource = null;
                lvProds_cist.ItemsSource = cistell.Prod_select;
                grDetalls.Visibility = Visibility.Visible;
            }
            this.usu = usu;
            this.cistell = cistell;

            List<string> metodes = new List<string>();
            foreach (Metode_enviamentMDB metode in mdbService.GetMetodes_enviament())
            {
                metodes.Add(metode.Nom+" (De "+metode.MinTemps_en_dies+" a "+metode.MaxTemps_en_dies+" dies laborals) "+metode.Preu_base+"€");
            }

            cbMetEnv.ItemsSource = metodes;
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {

            var newWindow = new MainWindow(usu, cistell);

            this.Close();

            newWindow.Show();
        }

        private void lvProds_cist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lvProds_cist.SelectedValue is Prod_select selected)
            {
                
                ProducteMDB prod =mdbService.GetProd(selected.Id);
                var newWindow = new UIProducte_info(prod,usu, cistell);

                this.Close();

                newWindow.Show();
            }
        }
    }
}
