using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gestio_Botiga_Calcat;

public partial class MainWindow : Window
{
    private Service mdbService;
    public MainWindow()
    {
        InitializeComponent();
        mdbService = new Service("Botiga");

        var cates = mdbService.GetCatesPare();
        spCates.Children.Clear();

        foreach (var cate in cates)
        {
            var nom = cate["nom"].ToString();

            var button = new Button
            {
                Content = nom,
                Background = new SolidColorBrush(Colors.White),
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Left
            };

            spCates.Children.Add(button);
        }


    }

}
