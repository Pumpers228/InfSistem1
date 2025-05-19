using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using UPZhukov.ClassFolder;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace UPZhukov.PageFolder.EmployeePages
{
    public partial class StatisticsPage : Page
    {
        public SeriesCollection TypeSeries { get; set; }
        public SeriesCollection StatusSeries { get; set; }
        public SeriesCollection DynamicSeries { get; set; }
        public string[] DateLabels { get; set; }
        public Func<double, string> ValueFormatter { get; set; }

        public StatisticsPage()
        {
            InitializeComponent();
            
            // Инициализация коллекций
            TypeSeries = new SeriesCollection();
            StatusSeries = new SeriesCollection();
            DynamicSeries = new SeriesCollection();
            
            // Установка контекста данных
            DataContext = this;

            // Загружаем статистику после полной инициализации компонентов
            Dispatcher.BeginInvoke(new Action(() => LoadStatistics()), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void LoadStatistics()
        {
            try
            {
                if (PeriodCb == null || PeriodCb.SelectedItem == null)
                {
                    MBClass.ErrorMB("Ошибка инициализации компонентов");
                    return;
                }

                var period = ((ComboBoxItem)PeriodCb.SelectedItem).Content.ToString();
                DateTime startDate = GetStartDate(period);

                using (var connection = ConnectionClass.GetConnection())
                {
                    connection.Open();

                    // Загрузка статистики по типам
                    LoadTypeStatistics(connection, startDate);

                    // Загрузка статистики по статусам
                    LoadStatusStatistics(connection, startDate);

                    // Загрузка динамики
                    LoadDynamicStatistics(connection, startDate);

                    // Загрузка общей статистики
                    LoadGeneralStatistics(connection, startDate);
                }
            }
            catch (Exception ex)
            {
                MBClass.ErrorMB("Ошибка при загрузке статистики: " + ex.Message);
            }
        }

        private DateTime GetStartDate(string period)
        {
            var now = DateTime.Now;
            switch (period)
            {
                case "За день":
                    return now.Date;
                case "За неделю":
                    return now.AddDays(-7);
                case "За месяц":
                    return now.AddMonths(-1);
                case "За год":
                    return now.AddYears(-1);
                default:
                    return now.Date;
            }
        }

        private void LoadTypeStatistics(SqlConnection connection, DateTime startDate)
        {
            try
            {
                string query = @"SELECT T.TypeName, COUNT(*) as Count
                           FROM Appeal A
                           JOIN AppealType T ON A.TypeId = T.TypeId
                           WHERE A.RegistrationDate >= @StartDate
                           GROUP BY T.TypeName";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    using (var reader = command.ExecuteReader())
                    {
                        var values = new List<double>();
                        var labels = new List<string>();

                        while (reader.Read())
                        {
                            values.Add(Convert.ToDouble(reader["Count"]));
                            labels.Add(reader["TypeName"].ToString());
                        }

                        if (TypeSeries != null)
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                try
                                {
                                    TypeSeries.Clear();
                                    if (values.Count > 0)
                                    {
                                        var series = new PieSeries
                                        {
                                            Title = "Типы обращений",
                                            Values = new ChartValues<double>(values),
                                            DataLabels = true,
                                            LabelPoint = point => $"{labels[(int)point.Participation]} ({point.Participation:P0})"
                                        };
                                        TypeSeries.Add(series);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MBClass.ErrorMB("Ошибка при обновлении графика типов: " + ex.Message);
                                }
                            }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MBClass.ErrorMB("Ошибка при загрузке статистики по типам: " + ex.Message);
            }
        }

        private void LoadStatusStatistics(SqlConnection connection, DateTime startDate)
        {
            try
            {
                string query = @"SELECT S.StatusName, COUNT(*) as Count
                           FROM Appeal A
                           JOIN AppealStatus S ON A.StatusId = S.StatusId
                           WHERE A.RegistrationDate >= @StartDate
                           GROUP BY S.StatusName";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    using (var reader = command.ExecuteReader())
                    {
                        var values = new List<double>();
                        var labels = new List<string>();

                        while (reader.Read())
                        {
                            values.Add(Convert.ToDouble(reader["Count"]));
                            labels.Add(reader["StatusName"].ToString());
                        }

                        if (StatusSeries != null)
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                try
                                {
                                    StatusSeries.Clear();
                                    if (values.Count > 0)
                                    {
                                        var series = new PieSeries
                                        {
                                            Title = "Статусы обращений",
                                            Values = new ChartValues<double>(values),
                                            DataLabels = true,
                                            LabelPoint = point => $"{labels[(int)point.Participation]} ({point.Participation:P0})"
                                        };
                                        StatusSeries.Add(series);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MBClass.ErrorMB("Ошибка при обновлении графика статусов: " + ex.Message);
                                }
                            }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MBClass.ErrorMB("Ошибка при загрузке статистики по статусам: " + ex.Message);
            }
        }

        private void LoadDynamicStatistics(SqlConnection connection, DateTime startDate)
        {
            try
            {
                string query = @"SELECT CAST(RegistrationDate AS DATE) as Date, COUNT(*) as Count
                           FROM Appeal
                           WHERE RegistrationDate >= @StartDate
                           GROUP BY CAST(RegistrationDate AS DATE)
                           ORDER BY Date";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    using (var reader = command.ExecuteReader())
                    {
                        var values = new List<double>();
                        var labels = new List<string>();

                        while (reader.Read())
                        {
                            values.Add(Convert.ToDouble(reader["Count"]));
                            labels.Add(Convert.ToDateTime(reader["Date"]).ToString("dd.MM.yyyy"));
                        }

                        if (DynamicSeries != null)
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                try
                                {
                                    DynamicSeries.Clear();
                                    if (values.Count > 0)
                                    {
                                        var series = new LineSeries
                                        {
                                            Title = "Количество обращений",
                                            Values = new ChartValues<double>(values),
                                            PointGeometry = DefaultGeometries.Circle,
                                            PointGeometrySize = 8
                                        };
                                        DynamicSeries.Add(series);
                                        DateLabels = labels.ToArray();
                                        ValueFormatter = value => value.ToString("N0");
                                    }
                                    else
                                    {
                                        DateLabels = new string[0];
                                        ValueFormatter = value => "0";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MBClass.ErrorMB("Ошибка при обновлении графика динамики: " + ex.Message);
                                }
                            }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MBClass.ErrorMB("Ошибка при загрузке динамики обращений: " + ex.Message);
            }
        }

        private void LoadGeneralStatistics(SqlConnection connection, DateTime startDate)
        {
            try
            {
                string query = @"SELECT 
                           (SELECT COUNT(*) FROM Appeal WHERE RegistrationDate >= @StartDate) as TotalAppeals,
                           (SELECT COUNT(*) FROM Appeal WHERE StatusId = 3 AND RegistrationDate >= @StartDate) as CompletedAppeals,
                           (SELECT COUNT(*) FROM Appeal WHERE StatusId = 1 AND RegistrationDate >= @StartDate) as NewAppeals,
                           (SELECT AVG(DATEDIFF(day, RegistrationDate, ResponseDate)) 
                            FROM Appeal 
                            WHERE ResponseDate IS NOT NULL AND RegistrationDate >= @StartDate) as AvgResponseTime";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    using (var reader = command.ExecuteReader())
                    {
                        var statistics = new List<StatisticsItem>();
                        
                        if (reader.Read())
                        {
                            statistics.Add(new StatisticsItem 
                            { 
                                Name = "Всего обращений", 
                                Value = reader["TotalAppeals"] == DBNull.Value ? "0" : reader["TotalAppeals"].ToString() 
                            });
                            statistics.Add(new StatisticsItem 
                            { 
                                Name = "Выполнено обращений", 
                                Value = reader["CompletedAppeals"] == DBNull.Value ? "0" : reader["CompletedAppeals"].ToString() 
                            });
                            statistics.Add(new StatisticsItem 
                            { 
                                Name = "Новых обращений", 
                                Value = reader["NewAppeals"] == DBNull.Value ? "0" : reader["NewAppeals"].ToString() 
                            });
                            statistics.Add(new StatisticsItem 
                            { 
                                Name = "Среднее время ответа (дней)", 
                                Value = reader["AvgResponseTime"] == DBNull.Value ? "0" : 
                                    Math.Round(Convert.ToDouble(reader["AvgResponseTime"]), 1).ToString() 
                            });
                        }
                        else
                        {
                            statistics.Add(new StatisticsItem { Name = "Всего обращений", Value = "0" });
                            statistics.Add(new StatisticsItem { Name = "Выполнено обращений", Value = "0" });
                            statistics.Add(new StatisticsItem { Name = "Новых обращений", Value = "0" });
                            statistics.Add(new StatisticsItem { Name = "Среднее время ответа (дней)", Value = "0" });
                        }

                        if (StatisticsGrid != null)
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                try
                                {
                                    StatisticsGrid.ItemsSource = statistics;
                                }
                                catch (Exception ex)
                                {
                                    MBClass.ErrorMB("Ошибка при обновлении таблицы статистики: " + ex.Message);
                                }
                            }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MBClass.ErrorMB("Ошибка при загрузке общей статистики: " + ex.Message);
            }
        }

        private void PeriodCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadStatistics();
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadStatistics();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void TypeChart_DataClick(object sender, ChartPoint chartPoint)
        {
            try
            {
                var series = chartPoint.SeriesView as PieSeries;
                if (series != null)
                {
                    var typeName = series.Title;
                    var value = chartPoint.Participation;
                    MBClass.InfoMB($"Тип обращения: {typeName}\nКоличество: {value}");
                }
            }
            catch (Exception ex)
            {
                MBClass.ErrorMB("Ошибка при обработке клика по диаграмме типов: " + ex.Message);
            }
        }

        private void StatusChart_DataClick(object sender, ChartPoint chartPoint)
        {
            try
            {
                var series = chartPoint.SeriesView as PieSeries;
                if (series != null)
                {
                    var statusName = series.Title;
                    var value = chartPoint.Participation;
                    MBClass.InfoMB($"Статус обращения: {statusName}\nКоличество: {value}");
                }
            }
            catch (Exception ex)
            {
                MBClass.ErrorMB("Ошибка при обработке клика по диаграмме статусов: " + ex.Message);
            }
        }
    }

    public class StatisticsItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
} 