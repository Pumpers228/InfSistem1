using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using UPZhukov.ClassFolder;

namespace UPZhukov.PageFolder.RegistratorPages
{
    public partial class AddAppealPage : Page
    {
        Frame mainFrame;

        public AddAppealPage(Frame frame = null)
        {
            InitializeComponent();
            mainFrame = frame;
            LoadAppealTypes();
        }

        private void LoadAppealTypes()
        {
            try
            {
                using (var connection = ConnectionClass.GetConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SELECT TypeId, TypeName FROM AppealType", connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ComboBoxItem item = new ComboBoxItem
                            {
                                Content = reader["TypeName"].ToString(),
                                Tag = reader["TypeId"]
                            };
                            AppealTypeCb.Items.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MBClass.ErrorMB("Ошибка при загрузке типов обращений: " + ex.Message);
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(LastNameTb.Text) ||
                    string.IsNullOrWhiteSpace(FirstNameTb.Text) ||
                    string.IsNullOrWhiteSpace(AddressTb.Text) ||
                    string.IsNullOrWhiteSpace(AppealTextTb.Text) ||
                    AppealTypeCb.SelectedItem == null)
                {
                    MBClass.ErrorMB("Пожалуйста, заполните все обязательные поля");
                    return;
                }

                string query = @"INSERT INTO Appeal 
                    (CitizenLastName, CitizenFirstName, CitizenMiddleName,
                     CitizenAddress, CitizenPhone, CitizenEmail,
                     AppealText, TypeId, StatusId, RegistratorId,
                     RegistrationDate)
                    VALUES 
                    (@LastName, @FirstName, @MiddleName,
                     @Address, @Phone, @Email,
                     @AppealText, @TypeId, @StatusId, @RegistratorId,
                     @RegistrationDate)";

                using (var connection = ConnectionClass.GetConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@LastName", LastNameTb.Text.Trim());
                        command.Parameters.AddWithValue("@FirstName", FirstNameTb.Text.Trim());
                        command.Parameters.AddWithValue("@MiddleName", 
                            string.IsNullOrWhiteSpace(MiddleNameTb.Text) ? DBNull.Value : (object)MiddleNameTb.Text.Trim());
                        command.Parameters.AddWithValue("@Address", AddressTb.Text.Trim());
                        command.Parameters.AddWithValue("@Phone",
                            string.IsNullOrWhiteSpace(PhoneTb.Text) ? DBNull.Value : (object)PhoneTb.Text.Trim());
                        command.Parameters.AddWithValue("@Email",
                            string.IsNullOrWhiteSpace(EmailTb.Text) ? DBNull.Value : (object)EmailTb.Text.Trim());
                        command.Parameters.AddWithValue("@AppealText", AppealTextTb.Text.Trim());
                        command.Parameters.AddWithValue("@TypeId", 
                            ((ComboBoxItem)AppealTypeCb.SelectedItem).Tag);
                        command.Parameters.AddWithValue("@StatusId", 1); // Статус "Зарегистрировано"
                        command.Parameters.AddWithValue("@RegistratorId", UserClass.UserId);
                        command.Parameters.AddWithValue("@RegistrationDate", DateTime.Now);

                        command.ExecuteNonQuery();
                    }
                }

                MBClass.InfoMB("Обращение успешно зарегистрировано");

                // Если страница была открыта из фрейма сотрудника, возвращаемся назад
                if (mainFrame != null)
                {
                    mainFrame.GoBack();
                }
                else
                {
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MBClass.ErrorMB("Ошибка при сохранении обращения: " + ex.Message);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (mainFrame != null)
            {
                mainFrame.GoBack();
            }
            else
            {
                ClearFields();
            }
        }

        private void ClearFields()
        {
            LastNameTb.Text = "";
            FirstNameTb.Text = "";
            MiddleNameTb.Text = "";
            AddressTb.Text = "";
            PhoneTb.Text = "";
            EmailTb.Text = "";
            AppealTextTb.Text = "";
            AppealTypeCb.SelectedIndex = -1;
        }
    }
} 