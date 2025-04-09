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

namespace SaidyakovLanguage
{
    /// <summary>
    /// Логика взаимодействия для PageClients.xaml
    /// </summary>
    public partial class PageClients : Page
    {
        List<Client> listClientsForPage = new List<Client>();
        public PageClients()
        {
            InitializeComponent();

            listClientsForPage = SaidyakovLanguageEntities.GetContext().Client.ToList();
            listViewClients.ItemsSource = listClientsForPage;

            comboBoxGender.SelectedIndex = 0;
            comboBoxSort.SelectedIndex = 0;

            comboBoxLimitCountOfRecordsForPage.SelectedIndex = 0;
            listBoxPages.SelectedIndex = 0;

            UpdateClients();
        }

        private int GetLimitCountOfRecordsForPage()
        {
            int currentCountOfClients = listClientsForPage.Count();
            int limitCountOfRecordsForPage = currentCountOfClients;

            switch (comboBoxLimitCountOfRecordsForPage.SelectedIndex)
            {
                case 0: limitCountOfRecordsForPage = 10; break;
                case 1: limitCountOfRecordsForPage = 50; break;
                case 2: limitCountOfRecordsForPage = 200; break;
            }

            if (limitCountOfRecordsForPage > currentCountOfClients)
            {
                limitCountOfRecordsForPage = currentCountOfClients;
            }

            return limitCountOfRecordsForPage;
        }
        
        private void UpdateListView()
        {
            int selectedPage = listBoxPages.SelectedIndex;
            var currentCountOfClients = listClientsForPage.Count();
            int limitCountOfRecordsForPage = GetLimitCountOfRecordsForPage();
            int firstIndexOfListClientsForPageOnLimit = limitCountOfRecordsForPage * selectedPage;

            if (listBoxPages.Items.Count - 1 == listBoxPages.SelectedIndex && currentCountOfClients % limitCountOfRecordsForPage != 0)
            {
                limitCountOfRecordsForPage = currentCountOfClients % limitCountOfRecordsForPage;
            }

            var listClientsForPageOnLimit = listClientsForPage.
                GetRange(firstIndexOfListClientsForPageOnLimit,
                limitCountOfRecordsForPage);
            listViewClients.ItemsSource = listClientsForPageOnLimit;
            textBlockCountOfPages.Text = currentCountOfClients.ToString() + " из " + SaidyakovLanguageEntities.GetContext().Client.Count().ToString();
        }

        private void UpdateListBoxPages()
        {
            int currentCountOfClients = listClientsForPage.Count();
            int limitCountOfRecordsForPage = GetLimitCountOfRecordsForPage();

            int countOfPages = currentCountOfClients / limitCountOfRecordsForPage;
            if (currentCountOfClients % limitCountOfRecordsForPage > 0)
            {
                ++countOfPages;
            }

            listBoxPages.Items.Clear();
            for (int i = 1; i <= countOfPages; ++i) { 
                listBoxPages.Items.Add(i);
            }
            listBoxPages.SelectedIndex = 0;
        }

        public void UpdateClients()
        {
            listClientsForPage = SaidyakovLanguageEntities.GetContext().Client.ToList();

            switch (comboBoxGender.SelectedIndex)
            {
                case 1: listClientsForPage = listClientsForPage.Where(cl => cl.Gender.Code.Equals("2")).ToList(); break;
                case 2: listClientsForPage = listClientsForPage.Where(cl => cl.Gender.Code.Equals("1")).ToList(); break;
            }

            switch (comboBoxSort.SelectedIndex)
            {
                case 1: listClientsForPage = listClientsForPage.OrderBy(cl => cl.FirstName).ToList(); break;
                case 2: listClientsForPage = listClientsForPage.OrderByDescending(cl => cl.StartTimeDT).ToList(); break;
                case 3: listClientsForPage = listClientsForPage.OrderByDescending(cl => cl.VisitCount).ToList(); break;
            }

            listClientsForPage = listClientsForPage.Where(cl => cl.FIO.ToLower().Contains(textBoxSearch.Text.ToLower())
                                                       || cl.Email.ToLower().Contains(textBoxSearch.Text.ToLower())
                                                       || cl.Phone.Replace("-", "").Replace("(", "").Replace(")", "").
                                                       Contains(textBoxSearch.Text.ToLower())).ToList();
            
            listViewClients.ItemsSource = listClientsForPage;

            UpdateListBoxPages();
            UpdateListView();
        }

        private void PageList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            UpdateListView();
        }

        private void PageCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateListBoxPages();
            UpdateListView();
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxPages.SelectedIndex > 0)
                listBoxPages.SelectedIndex--;
            UpdateListView();
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxPages.SelectedIndex < listBoxPages.Items.Count - 1)
                listBoxPages.SelectedIndex++;
            UpdateListView();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var CurrentClient = (sender as Button).DataContext as Client;
            if (CurrentClient.VisitCount != 0) MessageBox.Show("Невозможно удалить этого клиента!");
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    SaidyakovLanguageEntities.GetContext().Client.Remove(CurrentClient);
                    SaidyakovLanguageEntities.GetContext().SaveChanges();
                    UpdateClients();
                }
            }
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateClients();
        }

        private void SortCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClients();
        }

        private void GenderCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClients();
        }

        private void BtnAddClient_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new PageAddEditClient(null));
        }

        private void BtnEditClient_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new PageAddEditClient((sender as Button).DataContext as Client));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                SaidyakovLanguageEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                listViewClients.ItemsSource = SaidyakovLanguageEntities.GetContext().Client.ToList();

                UpdateClients();
            }
        }
    }
}
