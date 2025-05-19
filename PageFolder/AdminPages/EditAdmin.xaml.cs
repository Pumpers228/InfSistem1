using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UPZhukov.ClassFolder;
using UPZhukov.DataFolder;

namespace UPZhukov.PageFolder.AdminPages
{
    public partial class EditAdmin : Page
    {
        private User editUser;
        private DBEntities dbEntities = new DBEntities();

        public EditAdmin(User user)
        {
            InitializeComponent();
            editUser = user;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Загрузка ролей в ComboBox
                RoleCb.ItemsSource = dbEntities.Role.ToList();

                // Заполнение полей данными пользователя
                LoginTb.Text = editUser.Login;
                RoleCb.SelectedValue = editUser.RoleId;
                LastNameTb.Text = editUser.LastName;
                FirstNameTb.Text = editUser.FirstName;
                MiddleNameTb.Text = editUser.MiddleName;
                PhoneTb.Text = editUser.Phone;
            }
            catch (Exception ex)
            {
                MBClass.ErrorMB(ex);
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка обязательных полей
                if (string.IsNullOrWhiteSpace(LastNameTb.Text) ||
                    string.IsNullOrWhiteSpace(FirstNameTb.Text) ||
                    RoleCb.SelectedValue == null)
                {
                    MBClass.ErrorMB("Заполните обязательные поля!");
                    return;
                }

                // Проверка паролей
                if (!string.IsNullOrWhiteSpace(PasswordPb.Password))
                {
                    if (PasswordPb.Password != ConfirmPasswordPb.Password)
                    {
                        MBClass.ErrorMB("Пароли не совпадают!");
                        return;
                    }
                    editUser.Password = PasswordPb.Password;
                }

                // Обновление данных пользователя
                editUser.RoleId = (int)RoleCb.SelectedValue;
                editUser.LastName = LastNameTb.Text;
                editUser.FirstName = FirstNameTb.Text;
                editUser.MiddleName = MiddleNameTb.Text;
                editUser.Phone = PhoneTb.Text;

                dbEntities.SaveChanges();
                MBClass.InfoMB("Пользователь успешно обновлен!");
                NavigationService.Navigate(new ListAdmin());
            }
            catch (Exception ex)
            {
                MBClass.ErrorMB(ex);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ListAdmin());
        }
    }
} 