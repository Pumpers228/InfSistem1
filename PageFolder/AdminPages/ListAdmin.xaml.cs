using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UPZhukov.ClassFolder;
using UPZhukov.DataFolder;

namespace UPZhukov.PageFolder.AdminPages
{
    public partial class ListAdmin : Page
    {
        private DBEntities dbEntities = new DBEntities();

        public ListAdmin()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                string query = @"SELECT 
                                u.UserId,
                                u.Login,
                                u.LastName + ' ' + u.FirstName + 
                                CASE WHEN u.MiddleName IS NOT NULL 
                                     THEN ' ' + u.MiddleName 
                                     ELSE '' END AS FullName,
                                r.RoleName,
                                u.Phone
                                FROM [User] u
                                JOIN Role r ON u.RoleId = r.RoleId
                                ORDER BY u.UserId";

                using (SqlConnection connection = ConnectionClass.connection)
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    UsersDataGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Ошибка при загрузке данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void AddUserBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddAdmin());
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

        private void UsersDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (UsersDataGrid.SelectedItem != null)
            {
                try
                {
                    DataRowView row = (DataRowView)UsersDataGrid.SelectedItem;
                    int userId = Convert.ToInt32(row["UserId"]);
                    
                    // Загружаем полный объект пользователя
                    var user = dbEntities.User.FirstOrDefault(u => u.UserId == userId);
                    if (user != null)
                    {
                        NavigationService?.Navigate(new EditAdmin(user));
                    }
                    else
                    {
                        MBClass.ErrorMB("Пользователь не найден");
                    }
                }
                catch (Exception ex)
                {
                    MBClass.ErrorMB(ex);
                }
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var row = button.DataContext as DataRowView;
            if (row != null)
            {
                int userId = Convert.ToInt32(row["UserId"]);
                
                // Проверяем, не пытается ли админ удалить сам себя
                if (userId == UserClass.UserId)
                {
                    MessageBox.Show("Вы не можете удалить свою учетную запись",
                        "Предупреждение",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show("Вы действительно хотите удалить этого пользователя?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (SqlConnection connection = ConnectionClass.connection)
                        {
                            connection.Open();
                            string query = "DELETE FROM [User] WHERE UserId = @userId";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@userId", userId);
                            command.ExecuteNonQuery();

                            MessageBox.Show("Пользователь успешно удален",
                                "Успех",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                            LoadUsers();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message,
                            "Ошибка при удалении пользователя",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
        }
    }
} 