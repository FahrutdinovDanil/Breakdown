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
    /// Логика взаимодействия для ClientsListPage.xaml
    /// </summary>
    public partial class ClientsListPage : Page
    {
        public List<Client> Clients { get; set; }
        public IEnumerable<Client> ClientsForFilters { get; set; }
        public List<Gender> Genders { get; set; }
        public int PageNumber { get; set; } = 1;
        public Dictionary<string, Func<Client, object>> Sortings { get; set; }
        public ClientsListPage()
        {
            InitializeComponent();
            Clients = DataAccess.GetClients();
            Genders = DataAccess.GetGenders();
            Genders.Insert(0, new Gender { Clients = Clients, Name = "Все" });
            Sortings = new Dictionary<string, Func<Client, object>>
            {
                {"Фамилия по убыванию", x => x.LastName },
                {"Фамилия по возрастанию", y => y.LastName},
                {"Дата регистрации по убыванию", x => x.RegistrationDate},
                {"Дата регистрации по возрастанию", x => x.RegistrationDate}
            };
            DataAccess.RefreshList += DataAccess_RefreshList;
            DataContext = this;
        }

        private void SetPageNumbers()
        {
            spPageNumbers.Children.Clear();
            int pagesCount = ClientsForFilters.Count() % 10 == 0 ? ClientsForFilters.Count() / 10 : ClientsForFilters.Count() / 10 + 1;
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

                spPageNumbers.Children.Add(textBlock);
            }

        }

        private void Filter(bool filtersChanged)
        {
            if (filtersChanged)
                PageNumber = 0;
            int pagesCount = ClientsForFilters.Count() % 10 == 0 ? ClientsForFilters.Count() / 10 : ClientsForFilters.Count() / 10 + 1;
            var gender = (cbGender.SelectedItem as Gender);
            var searchText = tbSearch.Text.ToLower();
            var sort = cbSort.SelectedItem as string;
            if (gender != null && sort != null)
            {
                ClientsForFilters = gender.Clients.Where(x => x.ToString().ToLower().Contains(searchText))
                                            .OrderBy(Sortings[sort]).ToList();
                if (sort.Contains("убыванию"))
                    ClientsForFilters.Reverse();

                lvClients.ItemsSource = ClientsForFilters.Skip(PageNumber * pagesCount).Take(pagesCount);
                lvClients.Items.Refresh();
                SetPageNumbers();
            }

        }
        private void DataAccess_RefreshList()
        {
            Clients = DataAccess.GetClients();
            lvClients.ItemsSource = Clients;
            lvClients.Items.Refresh();
            SetPageNumbers();
        }

        private void NavigateToPage(object sender, RoutedEventArgs e)
        {
            PageNumber = int.Parse(((sender as Hyperlink).Inlines.FirstOrDefault() as Run).Text);
            (sender as Hyperlink).TextDecorations = TextDecorations.Underline;
            Filter(false);
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter(true);
        }

        private void cbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter(true);
        }

        private void cbGender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter(true);
        }

        private void lvClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var client = (Client)lvClients.SelectedItem;
            if (client != null)
                NavigationService.Navigate(new ClientPage(client));
        }

        private void btnAddClient_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ClientPage(new Client { RegistrationDate = DateTime.Now }));
        }
    }
}
