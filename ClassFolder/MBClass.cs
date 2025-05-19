using System.Windows;

namespace UPZhukov.ClassFolder
{
    public static class MBClass
    {
        public static void ErrorMB(string text)
        {
            MessageBox.Show(text, "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        public static void InfoMB(string text)
        {
            MessageBox.Show(text, "Информация",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        public static bool QuestionMB(string text)
        {
            return MessageBoxResult.Yes == MessageBox.Show(text,
                "Вопрос", MessageBoxButton.YesNo,
                MessageBoxImage.Question);
        }

        public static void ExceptionMB()
        {
            MessageBox.Show("Ошибка подключения к БД",
                "Ошибка", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
} 