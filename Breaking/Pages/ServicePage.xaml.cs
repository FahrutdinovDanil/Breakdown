using Breaking.DataBase;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Breaking.Pages
{
    /// <summary>
    /// Логика взаимодействия для ServicePage.xaml
    /// </summary>
    public partial class ServicePage : Page
    {
        public Service Service { get; set; }
        public List<Service> Services { get; set; }
        public ServicePage(Service service)
        {
            InitializeComponent();
            Service = service;
            if (service.ID == 0)
                btnDelete.Visibility = Visibility.Collapsed;

            DataContext = this;
        }

        private void btnChangePhoto_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog().Value == true)
            {
                Service.MainImagePath = File.ReadAllBytes(openFileDialog.FileName);
                imgClient.Source = new BitmapImage(new Uri(openFileDialog.FileName));

            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var stringBuilder = new StringBuilder();
            if (string.IsNullOrEmpty(Service.Title))
                stringBuilder.AppendLine("Введите название");
            if (string.IsNullOrEmpty(Convert.ToString(Service.Cost)))
                stringBuilder.AppendLine("Введите стоимость");
            if (string.IsNullOrEmpty(Convert.ToString(Service.DurationInSeconds)))
                stringBuilder.AppendLine("Введите длительность");
            if (stringBuilder.Length > 0)
            {
                MessageBox.Show(stringBuilder.ToString());
                return;
            }
            DataAccess.SaveService(Service);
            NavigationService.GoBack();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            DataAccess.DeleteService(Service);
            NavigationService.GoBack();
        }
    }
}
