using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UPZhukov.ClassFolder;
using UPZhukov.WindowFolder;
using UPZhukov.PageFolder.RegistratorPages;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Navigation;

namespace UPZhukov.PageFolder.EmployeePages
{
    public partial class EmployeePage : Page
    {
        Frame mainFrame;
        private string _currentSearchText = "";
        private DataView _currentDataView;

        public EmployeePage(Frame frame)
        {
            InitializeComponent();
            mainFrame = frame;
            LoadFilters();
            LoadAppeals();
        }

        private void LoadFilters()
        {
            try
            {
                DataTable statusTable = new DataTable();
                DataTable typeTable = new DataTable();

                using (var connection = ConnectionClass.GetConnection())
                {
                    connection.Open();
                    
                    // Загрузка статусов
                    using (SqlCommand statusCommand = new SqlCommand("SELECT StatusId, StatusName FROM AppealStatus", connection))
                    using (SqlDataAdapter statusAdapter = new SqlDataAdapter(statusCommand))
                    {
                        statusAdapter.Fill(statusTable);
                    }

                    // Загрузка типов обращений
                    using (SqlCommand typeCommand = new SqlCommand("SELECT TypeId, TypeName FROM AppealType", connection))
                    using (SqlDataAdapter typeAdapter = new SqlDataAdapter(typeCommand))
                    {
                        typeAdapter.Fill(typeTable);
                    }
                }

                // Добавление элемента "Все статусы"
                ComboBoxItem allStatusItem = new ComboBoxItem
                {
                    Content = "Все статусы",
                    Tag = null
                };
                StatusFilterCb.Items.Add(allStatusItem);

                // Добавление статусов
                foreach (DataRow row in statusTable.Rows)
                {
                    ComboBoxItem item = new ComboBoxItem
                    {
                        Content = row["StatusName"].ToString(),
                        Tag = row["StatusId"]
                    };
                    StatusFilterCb.Items.Add(item);
                }
                StatusFilterCb.SelectedItem = allStatusItem;

                // Добавление элемента "Все типы"
                ComboBoxItem allTypeItem = new ComboBoxItem
                {
                    Content = "Все типы",
                    Tag = null
                };
                TypeFilterCb.Items.Add(allTypeItem);

                // Добавление типов обращений
                foreach (DataRow row in typeTable.Rows)
                {
                    ComboBoxItem item = new ComboBoxItem
                    {
                        Content = row["TypeName"].ToString(),
                        Tag = row["TypeId"]
                    };
                    TypeFilterCb.Items.Add(item);
                }
                TypeFilterCb.SelectedItem = allTypeItem;
            }
            catch (Exception ex)
            {
                MBClass.ErrorMB("Ошибка при загрузке фильтров: " + ex.Message);
            }
        }

        private void LoadAppeals()
        {
            try
            {
                if (StatusFilterCb == null || TypeFilterCb == null)
                    return;

                string query = @"SELECT A.AppealId, A.RegistrationDate,
                                      A.CitizenLastName + ' ' + A.CitizenFirstName + 
                                      CASE WHEN A.CitizenMiddleName IS NOT NULL 
                                           THEN ' ' + A.CitizenMiddleName 
                                           ELSE '' END AS CitizenFullName,
                                      T.TypeName, S.StatusName,
                                      CASE WHEN S.StatusId IN (1, 2) THEN 'Visible' ELSE 'Collapsed' END AS CanRespond,
                                      A.AppealText
                               FROM Appeal A
                               JOIN AppealType T ON A.TypeId = T.TypeId
                               JOIN AppealStatus S ON A.StatusId = S.StatusId
                               WHERE (@StatusId IS NULL OR A.StatusId = @StatusId)
                               AND (@TypeId IS NULL OR A.TypeId = @TypeId)
                               AND (@SearchText IS NULL OR 
                                   A.CitizenLastName + ' ' + A.CitizenFirstName + 
                                   CASE WHEN A.CitizenMiddleName IS NOT NULL 
                                        THEN ' ' + A.CitizenMiddleName 
                                        ELSE '' END LIKE @SearchPattern
                                   OR A.AppealText LIKE @SearchPattern)
                               ORDER BY A.RegistrationDate DESC";

                using (var connection = ConnectionClass.GetConnection())
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        var selectedStatus = (ComboBoxItem)StatusFilterCb.SelectedItem;
                        var selectedType = (ComboBoxItem)TypeFilterCb.SelectedItem;

                        cmd.Parameters.AddWithValue("@StatusId", selectedStatus?.Tag ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TypeId", selectedType?.Tag ?? DBNull.Value);
                        
                        if (string.IsNullOrWhiteSpace(_currentSearchText))
                        {
                            cmd.Parameters.AddWithValue("@SearchText", DBNull.Value);
                            cmd.Parameters.AddWithValue("@SearchPattern", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@SearchText", _currentSearchText);
                            cmd.Parameters.AddWithValue("@SearchPattern", $"%{_currentSearchText}%");
                        }

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            
                            _currentDataView = dataTable.DefaultView;
                            _currentDataView.Sort = "RegistrationDate DESC";
                            AppealsDataGrid.ItemsSource = _currentDataView;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MBClass.ErrorMB("Ошибка при загрузке обращений: " + ex.Message);
            }
        }

        private void NewAppealBtn_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new AddAppealPage(mainFrame));
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadAppeals();
        }

        private void ViewAppeal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var row = (DataRowView)((Button)sender).DataContext;
                int appealId = Convert.ToInt32(row["AppealId"]);
                
                var viewWindow = new ViewAppealWindow(appealId, true);
                viewWindow.ShowDialog();
                
                // Обновляем данные после закрытия окна
                LoadAppeals();
            }
            catch (Exception ex)
            {
                MBClass.ErrorMB("Ошибка при открытии обращения: " + ex.Message);
            }
        }

        private void RespondToAppeal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var row = (DataRowView)((Button)sender).DataContext;
                int appealId = Convert.ToInt32(row["AppealId"]);
                
                var responseWindow = new ResponseAppealWindow(appealId);
                if (responseWindow.ShowDialog() == true)
                {
                    // Обновляем данные после успешного ответа
                    LoadAppeals();
                }
            }
            catch (Exception ex)
            {
                MBClass.ErrorMB("Ошибка при ответе на обращение: " + ex.Message);
            }
        }

        private void StatusFilterCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                LoadAppeals();
            }
        }

        private void TypeFilterCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                LoadAppeals();
            }
        }

        private void SearchTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            _currentSearchText = SearchTb.Text.Trim();
            LoadAppeals();
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadAppeals();
        }

        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentDataView == null || _currentDataView.Count == 0)
                {
                    MBClass.ErrorMB("Нет данных для экспорта");
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV файлы (*.csv)|*.csv",
                    Title = "Экспорт в Excel",
                    FileName = $"Обращения_{DateTime.Now:yyyy-MM-dd}"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    StringBuilder csv = new StringBuilder();

                    // Добавляем заголовки
                    csv.AppendLine("№;Дата регистрации;ФИО гражданина;Тип обращения;Статус");

                    // Добавляем данные
                    foreach (DataRowView row in _currentDataView)
                    {
                        csv.AppendLine($"{row["AppealId"]};" +
                                     $"{Convert.ToDateTime(row["RegistrationDate"]):dd.MM.yyyy HH:mm};" +
                                     $"{row["CitizenFullName"]};" +
                                     $"{row["TypeName"]};" +
                                     $"{row["StatusName"]}");
                    }

                    File.WriteAllText(saveFileDialog.FileName, csv.ToString(), Encoding.UTF8);
                    MBClass.InfoMB("Данные успешно экспортированы");
                }
            }
            catch (Exception ex)
            {
                MBClass.ErrorMB("Ошибка при экспорте данных: " + ex.Message);
            }
        }

        private void StatisticsBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new StatisticsPage());
        }
    }
} 