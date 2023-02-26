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
        public IEnumerable<Client> Clients { get; set; }
        public IEnumerable<Client> ClientsForFilters { get; set; }
        public List<Gender> Genders { get; set; }
        public int PageNumber { get; set; } = 1;
        public Dictionary<string, Func<Client, object>> Sortings { get; set; }
        public ClientsListPage()
        {
            InitializeComponent();
            Clients = DataAccess.GetClients();
            ClientsForFilters = Clients;

            Sortings = new Dictionary<string, Func<Client, object>>
            {
                {"Фамилия по убыванию", x => x.LastName },
                {"Фамилия по возрастанию", x => x.LastName },
                {"Дата регистрации по убыванию", x => x.RegistrationDate },
                {"Дата регистрации по возрастанию", x => x.RegistrationDate },
            };

            DataAccess.RefreshList += DataAccess_RefreshList;
            Genders = DataAccess.GetGenders();
            Genders.Insert(0, new Gender { Name = "Все" });

            DataContext = this;
        }

        private void SetPageNumbers()
        {
            PageNumbersPanel.Children.Clear();
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

                PageNumbersPanel.Children.Add(textBlock);
            }
        }

        private void Filter(bool filtersChanged)
        {

            var gender = cbGender.SelectedItem as Gender;
            var sorting = Sortings[cbSort.SelectedItem as string];
            var text = tbSearch.Text.ToLower();
            if (filtersChanged)
                PageNumber = 1;

            if (gender != null && sorting != null)
            {
                if (gender.Name != "Все")
                    ClientsForFilters = Clients.Where(x => x.Gender == gender).ToList();
                else
                    ClientsForFilters = Clients;

                ClientsForFilters = ClientsForFilters.OrderBy(sorting).ToList();
                if ((cbSort.SelectedItem as string).Contains("убыванию"))
                    ClientsForFilters = ClientsForFilters.Reverse();

                ClientsForFilters = ClientsForFilters.Where(x => x.LastName.ToLower().Contains(text)
                                                    || x.Email.Contains(text)
                                                    || x.Phone.Contains(text)).ToList();

                lvClients.ItemsSource = new ObservableCollection<Client>(ClientsForFilters.Skip((PageNumber - 1) * 10).Take(10).ToList());
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
            Clients = DataAccess.GetClients();
            lvClients.ItemsSource = Clients;
            lvClients.Items.Refresh();
            SetPageNumbers();
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
            int pagesCount = ClientsForFilters.Count() % 10 == 0 ? ClientsForFilters.Count() / 10 : ClientsForFilters.Count() / 10 + 1;
            if (PageNumber < pagesCount)
            {
                PageNumber++;
                Filter(false);
            }
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter(true);
        }

        private void FiltersChanged(object sender, SelectionChangedEventArgs e)
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
