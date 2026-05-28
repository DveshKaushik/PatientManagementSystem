using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PatientManagementSystem.Models;

namespace PatientManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseHelper _db;

        public HomeController(DatabaseHelper db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var model = new DashboardViewModel();
            model.ChartData = new List<AppointmentChartData>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();

                // Total patients
                var cmd1 = new SqlCommand(
                    "SELECT COUNT(*) FROM Patients", conn);
                model.TotalPatients = (int)cmd1.ExecuteScalar();

                // Total doctors
                var cmd2 = new SqlCommand(
                    "SELECT COUNT(*) FROM Doctors", conn);
                model.TotalDoctors = (int)cmd2.ExecuteScalar();

                // Total appointments
                var cmd3 = new SqlCommand(
                    "SELECT COUNT(*) FROM Appointments", conn);
                model.TotalAppointments = (int)cmd3.ExecuteScalar();

                // Today's appointments
                var cmd4 = new SqlCommand(@"
                    SELECT COUNT(*) FROM Appointments 
                    WHERE CAST(AppointmentDate AS DATE) = CAST(GETDATE() AS DATE)", conn);
                model.TodayAppointments = (int)cmd4.ExecuteScalar();

                // Chart data - appointments per day (last 7 days)
                var cmd5 = new SqlCommand(@"
                    SELECT 
                        CAST(AppointmentDate AS DATE) AS AppDate,
                        COUNT(*) AS Total
                    FROM Appointments
                    WHERE AppointmentDate >= DATEADD(DAY, -7, GETDATE())
                    GROUP BY CAST(AppointmentDate AS DATE)
                    ORDER BY AppDate", conn);

                var reader = cmd5.ExecuteReader();
                while (reader.Read())
                {
                    model.ChartData.Add(new AppointmentChartData
                    {
                        Date = Convert.ToDateTime(reader["AppDate"])
                                      .ToString("dd MMM"),
                        Count = (int)reader["Total"]
                    });
                }
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}