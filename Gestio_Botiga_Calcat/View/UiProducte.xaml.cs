using Gestio_Botiga_Calcat.model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
    /// ProducteMDBeraction logic for UiProducte.xaml
    /// </summary>
    public partial class UiProducte : UserControl
    {
        VariantMDB variant_select = new VariantMDB();
        public string Desc { get; set; } // Propiedad normal

        public UiProducte()
        {
            InitializeComponent();
            //List<VariantMDB> variants = Prod.Variants;
            //variant_select = variants.FirstOrDefault();

        }



        public ProducteMDB Prod
        {
            get { return (ProducteMDB)GetValue(ProdProperty); }
            set { SetValue(ProdProperty, value);
                cargDesc();
            }
        }

        // Using a DependencyProperty as the backing store for Prod.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProdProperty =
            DependencyProperty.Register("Prod", typeof(ProducteMDB), typeof(UiProducte), new PropertyMetadata(null));
       
        private void cargDesc()
        {
            Desc = "ddd";
            if (Prod != null)
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(Prod.Desc);

                var bodyContent = htmlDoc.DocumentNode.SelectSingleNode("//body");

                if (bodyContent != null)
                {
                    Desc = bodyContent.InnerText;
                }
                else
                {
                    Desc = " m";
                }
            }
            else
            {
                Desc = "nopp";
            }
        }

    }
}
