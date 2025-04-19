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
using System.Net.Mail;
using System.Net;
using MongoDB.Bson;

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
        private FacturaMDB factura = new FacturaMDB();
        private void carregar_info()
        {

            if (Global.Usuari != null)
            {
                Global.mdbService.GetNumFactura();
                if(Global.DadesEmpresa == null)
                {
                    Global.DadesEmpresa = Global.mdbService.GetEmpresa();
                }

                factura.Id = ObjectId.GenerateNewId();
                factura.Codi = DateTime.Now.Year.ToString() + (Global.QtFactures+1);
                factura.Emissor = Global.DadesEmpresa.Id;
                factura.Destinatari = Global.Usuari.Id;

                factura.Data_factura = DateTime.Now;
                factura.Data_venciment = DateTime.Now.AddDays(30);
                //factura.Cost_enviament = Global.cistellManager.Metode_enviament.;


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
            /*revisar_targeta();
            if (tbNumTar.Text.All(char.IsDigit) && tbNumTar.Text.Length >0)
            {
                imgTar.Visibility = Visibility.Visible;
            }
            else
            {
                imgTar.Visibility = Visibility.Hidden;
            }
            imgTar.Source = new BitmapImage(new Uri(imgs + "mastercard.png", UriKind.Relative));*/

            if (IsValidCardNumber(tbNumTar.Text.Replace(" ", "").Trim()))
            {
                imgTar.Visibility = Visibility.Visible;
            }
            else
            {
                imgTar.Visibility = Visibility.Hidden;
            }
            comprar();
        }
        //StackOverflow
        private bool IsValidCardNumber(string cardNumber)
        {

            if (!long.TryParse(cardNumber, out _) || cardNumber.Length < 13 || cardNumber.Length > 19)
            {
                return false;
            }

            if (!IsLuhnValid(cardNumber))
            {
                return false;
            }

            if (cardNumber.StartsWith("4"))
            {
                imgTar.Source = new BitmapImage(new Uri(imgs + "visa.png", UriKind.Relative));
            }
            else if (cardNumber.StartsWith("51") || cardNumber.StartsWith("52") || cardNumber.StartsWith("53") ||
                     cardNumber.StartsWith("54") || cardNumber.StartsWith("55"))
            {
                imgTar.Source = new BitmapImage(new Uri(imgs + "mastercard.png", UriKind.Relative));
            }
            else
            {
                return false;
            }

            return true;
        }
        private bool IsLuhnValid(string cardNumber)
        {
            int sum = 0;
            bool alternate = false;

            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                int n = int.Parse(cardNumber[i].ToString());

                if (alternate)
                {
                    n *= 2;
                    if (n > 9)
                    {
                        n -= 9;
                    }
                }

                sum += n;
                alternate = !alternate;
            }

            return (sum % 10 == 0);
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
            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("tuemail@gmail.com", "tu_contraseña"),
                EnableSsl = true
            };

            var mensaje = new MailMessage("tuemail@gmail.com", "destinatario@correo.com", "Hola!", "Este es un correo enviado desde C#");

            smtp.Send(mensaje);


            MessageBox.Show("Gràcies per la seva compra!", "Compra realitzada", MessageBoxButton.OK, MessageBoxImage.Information);


        }

        private bool CVV_es_correcte()
        {
            return (tbCSVTar.Text.Trim().Length == 3 || tbCSVTar.Text.Trim().Length == 4) && tbCSVTar.Text.Trim().All(char.IsDigit);
        }
        private bool data_caducitat_correcte()
        {
            string data_cad = tbDataTar.Text.Replace(" ", "").Trim();
            if (tbDataTar.Text.Length != 5 || tbDataTar.Text[2] != '/')
            {
                return false;
            }

            if (!int.TryParse(tbDataTar.Text.Substring(0, 2), out int mes) ||
                !int.TryParse(tbDataTar.Text.Substring(3, 2), out int anny))
            {
                return false;
            }

            anny += 2000;

            DateTime expiration = new DateTime(anny, mes, 1).AddMonths(1).AddDays(-1);
            return expiration >= DateTime.Now && mes >= 1 && mes <= 12;
        }
        private void comprar()
        {
            if (data_caducitat_correcte() && CVV_es_correcte() && IsValidCardNumber(tbNumTar.Text.Replace(" ", "").Trim()))
            {
                btnComprar.IsEnabled = true;
            }
            else
            {
                btnComprar.IsEnabled = false;
            }
        }
        private void tbDataTar_TextChanged(object sender, TextChangedEventArgs e)
        {

            comprar();
        }
    }
}