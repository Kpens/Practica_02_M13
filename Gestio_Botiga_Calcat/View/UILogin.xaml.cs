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
        private CistellMDB cistell = new CistellMDB();
        private UsuariMDB usuari = new UsuariMDB();
        public UILogin(UsuariMDB Usuari, CistellMDB cistell)
        {
            InitializeComponent();
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
    }
}
