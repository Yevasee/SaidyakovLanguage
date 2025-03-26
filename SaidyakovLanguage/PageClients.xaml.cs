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
        int countRecordsOfClients;
        int countOfPages;
        int currentPage = 0;
        List<Client> listClientsForPage = new List<Client>();
        List<Client> listCurrentClients;
        public PageClients()
        {
            InitializeComponent();

            UpdateClients();
        }

        private void UpdateClients()
        {
            var listClients = SaidyakovLanguageEntities.GetContext().Client.ToList();

            listViewClients.ItemsSource = listClients;
            listCurrentClients = listClients;
            
            ChangePage(0, 0);
        }
        private void ChangePage(int direction, int? selectedPage)
        {
            listClientsForPage.Clear();
            countRecordsOfClients = listCurrentClients.Count;

            countOfPages = countRecordsOfClients / 10;
            if (countRecordsOfClients % 10 > 0)
            {
                countOfPages += 1;
            }

            Boolean isUpdate = false;

            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage <= countOfPages)
                {
                    currentPage = (int)selectedPage;
                }
                isUpdate = true;
            }
            else
            {
                switch (direction)
                {
                    case 1:
                        if (currentPage > 0)
                        {
                            currentPage--;
                            isUpdate = true;
                        }
                        break;
                    case 2:
                        if (currentPage < countOfPages - 1)
                        {
                            currentPage++;
                            isUpdate = true;
                        }
                        break;
                }
            }

            int countRecordsOnPage = currentPage * 10 + 10 < countRecordsOfClients ? currentPage * 10 + 10 : countRecordsOfClients;
            for (int i = currentPage * 10; i < countRecordsOnPage; i++)
            {
                listClientsForPage.Add(listCurrentClients[i]);
            }

            if (isUpdate)
            {
                listBoxPages.Items.Clear();

                for (int i = 1; i <= countOfPages; i++)
                {
                    listBoxPages.Items.Add(i);
                }

                listBoxPages.SelectedIndex = currentPage;
                textBlockCountRecords.Text = countRecordsOnPage.ToString();
                textBlockCountAllRecords.Text = " из " + countRecordsOfClients.ToString();
                listViewClients.ItemsSource = listClientsForPage;

                listViewClients.Items.Refresh();
            }
        }
        private void btnLeftDir_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void listBoxPages_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(listBoxPages.SelectedItem.ToString()) - 1);
        }

        private void btnRightDir_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }
    }
}
