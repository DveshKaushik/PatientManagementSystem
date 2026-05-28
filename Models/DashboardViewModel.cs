namespace PatientManagementSystem.Models
{
    public class DashboardViewModel
    {
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalAppointments { get; set; }
        public int TodayAppointments { get; set; }
        public List<AppointmentChartData> ChartData { get; set; }
    }

    public class AppointmentChartData
    {
        public string Date { get; set; }
        public int Count { get; set; }
    }
}