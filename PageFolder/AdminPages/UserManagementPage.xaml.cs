using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using UPZhukov.ClassFolder;
using UPZhukov.WindowFolder;

namespace UPZhukov.PageFolder.AdminPages
{
    public partial class UserManagementPage : Page
    {
        public UserManagementPage()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                string query = @"SELECT U.UserId, U.Login, U.LastName, 
                                      U.FirstName, U.MiddleName, R.RoleName
                               FROM [User] U
                               JOIN Role R ON U.RoleId = R.RoleId";

                using (var connection = ConnectionClass.GetConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        UsersDataGrid.ItemsSource = dataTable.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MBClass.ErrorMB("Ошибка при загрузке пользователей: " + ex.Message);
            }
        }

        private void AddUserBtn_Click(object sender, RoutedEventArgs e)
        {
            AddEditUserWindow addUserWindow = new AddEditUserWindow(null);
            addUserWindow.ShowDialog();
            LoadUsers();
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            var row = (DataRowView)((Button)sender).DataContext;
            int userId = Convert.ToInt32(row["UserId"]);
            AddEditUserWindow editUserWindow = new AddEditUserWindow(userId);
            editUserWindow.ShowDialog();
            LoadUsers();
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var row = (DataRowView)((Button)sender).DataContext;
                int userId = Convert.ToInt32(row["UserId"]);

                if (MBClass.QuestionMB("Вы уверены, что хотите удалить этого пользователя?"))
                {
                    using (var connection = ConnectionClass.GetConnection())
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand("DELETE FROM [User] WHERE UserId = @UserId", connection))
                        {
                            command.Parameters.AddWithValue("@UserId", userId);
                            command.ExecuteNonQuery();
                        }
                    }
                    LoadUsers();
                }
            }
            catch (Exception ex)
            {
                MBClass.ErrorMB("Ошибка при удалении пользователя: " + ex.Message);
            }
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }
    }
} 