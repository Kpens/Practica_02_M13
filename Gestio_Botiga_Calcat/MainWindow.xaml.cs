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

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Service _mongoService;
    public MainWindow()
    {
        InitializeComponent();
        _mongoService = new Service("Botiga");
    }

    private void CargarUsuariosButton_Click(object sender, RoutedEventArgs e)
    {
        var usuarios = _mongoService.GetAllUsers();

        UsuariosListBox.Items.Clear();

        foreach (var usuario in usuarios)
        {
            
            var nombre = usuario["login"].ToString(); 

            var userString = $"Nombre: {nombre}";

            UsuariosListBox.Items.Add(userString);
        }
    }
}
