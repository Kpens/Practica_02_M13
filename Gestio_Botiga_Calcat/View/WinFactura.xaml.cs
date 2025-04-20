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
using ZstdSharp.Unsafe;
using System.Diagnostics.Contracts;

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
                factura.Codi = "FAC"+DateTime.Now.Year.ToString() + (Global.QtFactures+1);
                factura.Emissor = Global.DadesEmpresa.Id;
                factura.Destinatari = Global.Usuari.Id;


                factura.Data_factura = DateTime.Now;
                factura.Data_venciment = DateTime.Now.AddDays(30);
                double total = 0, desc = 0;
                double[] bases = new double[3];
                int[] ives = new int[3];
                ives[0] = -1;
                ives[1] = -1;
                ives[2] = -1;
                if (factura.Linies_compra == null)
                {
                    factura.Linies_compra = new List<Linies>();  
                }
                foreach(Prod_select prod in Global.cistellManager.Prod_select_comprar)
                {
                    VariantMDB variant = Global.mdbService.GetVariantByStockId(prod.Estoc_id);
                    ProducteMDB producte = Global.mdbService.GetProd(prod.Id);
                    IVA_MDB iva = Global.mdbService.GetIVA(producte.Tipus_IVA);
                    Linies linia = new Linies();
                    linia.Nom = producte.Nom;
                    linia.Descompte = variant.DescomptePercent;
                    linia.Preu_base = variant.Preu;
                    linia.Estoc = prod.Estoc_id;
                    linia.Quantitat = prod.Quantitat;
                    linia.tipus_IVA = producte.Tipus_IVA;
                    linia.perc_IVA = ((int)iva.Percentatge);

                    double descompte = ((variant.Preu * prod.Quantitat) * variant.DescomptePercent) / 100;
                    linia.Bases_imposables = descompte;
                    linia.Total = (linia.Preu_base -descompte)*((iva.Percentatge/100.0)+1);
                    factura.Linies_compra.Add(linia);
                    desc += descompte;
                    //total += linia.Total;

                    if (ives[0] != -1 &&ives[0] == ((int)iva.Percentatge))
                    {
                        bases[0] += (variant.Preu * prod.Quantitat) - descompte;
                    }
                    else if (ives[1] != -1 && ives[1] == ((int)iva.Percentatge))
                    {
                        bases[1] += (variant.Preu * prod.Quantitat) - descompte;
                    }
                    else if (ives[2] != -1 && ives[2] == ((int)iva.Percentatge))
                    {
                        bases[2] += (variant.Preu * prod.Quantitat) - descompte;
                    }
                    else
                    {
                        if (ives[0] == -1)
                        {
                            ives[0] = (int)iva.Percentatge;
                            bases[0] += (variant.Preu * prod.Quantitat) - descompte;
                        }
                        else if (ives[1] == -1)
                        {
                            ives[1] = (int)iva.Percentatge;
                            bases[1] += (variant.Preu * prod.Quantitat) - descompte;
                        }
                        else if (ives[2] == -1)
                        {
                            ives[2] = (int)iva.Percentatge;
                            bases[2] += (variant.Preu * prod.Quantitat) - descompte;
                        }
                    }
                }

                total += bases[0];
                factura.iva1 = ives[0];
                factura.Preu_base1 = bases[0];

                if (ives[1] != -1)
                {
                    total += bases[1];
                    factura.iva2 = ives[1];
                    factura.Preu_base2 = bases[1];
                }
                else{

                    factura.iva2 = -1;
                    factura.Preu_base2 = -1;
                }

                if (ives[2] != -1)
                {
                    total += bases[2];
                    factura.iva3 = -1;
                    factura.Preu_base3 = -1;
                }

                if (total >= Global.cistellManager.Metode_enviament.Preu_min_compra)
                {
                    factura.Cost_enviament = 0;
                }
                else
                {
                    factura.Cost_enviament = Global.cistellManager.Metode_enviament.Preu_base;
                }
                factura.Metode_pagament = Global.cistellManager.Metode_enviament.Nom + " (De " + Global.cistellManager.Metode_enviament.MinTemps_en_dies + " a " + Global.cistellManager.Metode_enviament.MaxTemps_en_dies + " dies laborals) " + Global.cistellManager.Metode_enviament.Preu_base + "€";
                //factura.Preu_base = desc;

                

                factura.Total = total + factura.Cost_enviament;

                Global.mdbService.ActualitzarQtProdsFactura(Global.cistellManager.Prod_select_comprar);

                tbNom.Text = Global.Usuari.Nom;
                tbNomTar.Text = Global.Usuari.Nom;
                tbMail.Text = Global.Usuari.Mail;
                tbTelf.Text = Global.Usuari.Telf;
                tbAdreca.Text = Global.Usuari.Carrer + ", " + Global.Usuari.CodiPostal + ", " + Global.Usuari.Ciutat + ", " + Global.Usuari.Pais;
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
            if (winLogin != null)
            {
                winLogin.Close();
            }
           
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

            Global.mdbService.RetornarQtProdsFactura(Global.cistellManager.Prod_select_comprar);
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
            Global.mdbService.crear_factura(factura);


            string pwd_b64 = Global.DadesEmpresa.Pwd_Mail;
            string contra_bbdd = Encoding.UTF8.GetString(Convert.FromBase64String(pwd_b64));


            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(Global.DadesEmpresa.Mail, "ijonjtbfymibtbqm"),
                EnableSsl = true
            };

            var mail = new MailMessage(Global.DadesEmpresa.Mail, Global.Usuari.Mail, "Factura", "Aquí tens la teva factura");
            smtpClient.Send(mail);
            es_pot_tencar = true;
            grUsu.Visibility = Visibility.Collapsed;
            spNoProds.Visibility = Visibility.Visible;
            tbTitNoProds.Text = "Gràcies per la vostra compra!";
            lbNoProds.Visibility = Visibility.Hidden;

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