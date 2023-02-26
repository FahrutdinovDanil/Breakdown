using Breaking.DataBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для ServicesListPage.xaml
    /// </summary>
    public partial class ServicesListPage : Page
    {
        public IEnumerable<Service> Services { get; set; }
        public IEnumerable<Service> ServicesForFilters { get; set; }
        public int PageNumber { get; set; } = 1;
        public Dictionary<string, Func<Service, object>> Sortings { get; set; }
        public ServicesListPage()
        {
            InitializeComponent();
            Services = DataAccess.GetServices();
            ServicesForFilters = Services;

            Sortings = new Dictionary<string, Func<Service, object>>()
            {
                {"Стоимость по убыванию", x => x.Cost },
                {"Стоимость по возрастанию", x => x.Cost },
                {"Длительность по убыванию", x => x.DurationInSeconds },
                {"Длительность по возрастанию", x => x.DurationInSeconds },
            };
            DataAccess.RefreshList += DataAccess_RefreshList;
            DataContext = this;
        }

        private void SetPageNumbers()
        {
            PageNumbersPanel.Children.Clear();
            int pagesCount = ServicesForFilters.Count() % 10 == 0 ? ServicesForFilters.Count() / 10 : ServicesForFilters.Count() / 10 + 1;
            for (int i = 0; i < pagesCount; i++)
            {
                var hyperlink = new Hyperlink()
                {
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 25,
                    TextDecorations = null
                };
                hyperlink.Inlines.Add($"{i + 1}");
                hyperlink.Click += NavigateToPage;

                var textBlock = new TextBlock() { Margin = new Thickness(5, 0, 5, 0) };

                if (i == PageNumber - 1)
                    hyperlink.TextDecorations = TextDecorations.Underline;

                textBlock.Inlines.Add(hyperlink);

                PageNumbersPanel.Children.Add(textBlock);
            }
        }
        private void Filter(bool filtersChanged)
        {

            var sorting = Sortings[cbSort.SelectedItem as string];
            var text = tbSearch.Text.ToLower();
            if (filtersChanged)
                PageNumber = 1;

            if (sorting != null)
            {

                ServicesForFilters = ServicesForFilters.OrderBy(sorting).ToList();
                if ((cbSort.SelectedItem as string).Contains("убыванию"))
                    ServicesForFilters = ServicesForFilters.Reverse();

                ServicesForFilters = ServicesForFilters.Where(x => x.Title.ToLower().Contains(text)
                                                    || x.Description.Contains(text)).ToList();

                lvServices.ItemsSource = new ObservableCollection<Service>(ServicesForFilters.Skip((PageNumber - 1) * 10).Take(10).ToList());
            }
            SetPageNumbers();
        }
        private void NavigateToPage(object sender, RoutedEventArgs e)
        {
            PageNumber = int.Parse(((sender as Hyperlink).Inlines.FirstOrDefault() as Run).Text);
            (sender as Hyperlink).TextDecorations = TextDecorations.Underline;
            Filter(false);
        }


        private void DataAccess_RefreshList()
        {
            Services = DataAccess.GetServices();
            lvServices.ItemsSource = Services;
            lvServices.Items.Refresh();
            SetPageNumbers();
        }

        private void btnClients_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ClientsListPage());
        }

        private void btnAddServices_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ServicePage(new Service()));
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter(true);
        }

        private void lvServices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var service = (Service)lvServices.SelectedItem;
            if (service != null)
                NavigationService.Navigate(new ServicePage(service));
        }

        private void cbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter(true);
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (PageNumber > 1)
            {
                PageNumber--;
                Filter(false);
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            int pagesCount = ServicesForFilters.Count() % 10 == 0 ? ServicesForFilters.Count() / 10 : ServicesForFilters.Count() / 10 + 1;
            if (PageNumber < pagesCount)
            {
                PageNumber++;
                Filter(false);
            }
        }
    }
}
