using Breaking.DataBase;
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

namespace Breaking
{
    /// <summary>
    /// Логика взаимодействия для ChangeServiceWindow.xaml
    /// </summary>
    public partial class EditServiceWindow : Window
    {
        public ClientService ClientService { get; set; }
        public EditServiceWindow(ClientService service)
        {
            InitializeComponent();
            ClientService = service;
            DataContext = ClientService;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить?", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ClientService.Client.ClientServices.Remove(ClientService);
                DialogResult = false;
                Close();
            }
        }
    }
}
