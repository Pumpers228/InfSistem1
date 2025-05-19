using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using UPZhukov.ClassFolder;

namespace UPZhukov.PageFolder.AdminPages
{
    public partial class AddAdmin : Page
    {
        public AddAdmin()
        {
            InitializeComponent();
            LoadRoles();
        }

        private void LoadRoles()
        {
            try
            {
                string query = "SELECT RoleId, RoleName FROM Role";
                using (SqlConnection connection = ConnectionClass.connection)
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    RoleCb.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Ошибка при загрузке ролей",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginTb.Text) ||
                string.IsNullOrWhiteSpace(PasswordPb.Password) ||
                string.IsNullOrWhiteSpace(ConfirmPasswordPb.Password) ||
                string.IsNullOrWhiteSpace(LastNameTb.Text) ||
                string.IsNullOrWhiteSpace(FirstNameTb.Text) ||
                RoleCb.SelectedValue == null)
            {
                MessageBox.Show("Заполните все обязательные поля",
                    "Предупреждение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (PasswordPb.Password != ConfirmPasswordPb.Password)
            {
                MessageBox.Show("Пароли не совпадают",
                    "Предупреждение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Проверяем, не существует ли уже пользователь с таким логином
                using (SqlConnection connection = ConnectionClass.connection)
                {
                    connection.Open();
                    string checkQuery = "SELECT COUNT(*) FROM [User] WHERE Login = @login";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@login", LoginTb.Text);
                    int count = (int)checkCommand.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует",
                            "Предупреждение",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }

                    // Добавляем нового пользователя
                    string query = @"INSERT INTO [User] 
                                   (Login, Password, RoleId, LastName, FirstName, MiddleName, Phone)
                                   VALUES 
                                   (@login, @password, @roleId, @lastName, @firstName, @middleName, @phone)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@login", LoginTb.Text);
                    command.Parameters.AddWithValue("@password", PasswordPb.Password);
                    command.Parameters.AddWithValue("@roleId", RoleCb.SelectedValue);
                    command.Parameters.AddWithValue("@lastName", LastNameTb.Text);
                    command.Parameters.AddWithValue("@firstName", FirstNameTb.Text);
                    command.Parameters.AddWithValue("@middleName", 
                        string.IsNullOrWhiteSpace(MiddleNameTb.Text) ? DBNull.Value : (object)MiddleNameTb.Text);
                    command.Parameters.AddWithValue("@phone", 
                        string.IsNullOrWhiteSpace(PhoneTb.Text) ? DBNull.Value : (object)PhoneTb.Text);

                    command.ExecuteNonQuery();

                    MessageBox.Show("Пользователь успешно добавлен",
                        "Успех",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    NavigationService?.Navigate(new ListAdmin());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Ошибка при добавлении пользователя",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ListAdmin());
        }
    }
} 