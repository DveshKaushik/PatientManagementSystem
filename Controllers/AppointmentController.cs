using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using PatientManagementSystem.Models;

namespace PatientManagementSystem.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly DatabaseHelper _db;

        public AppointmentController(DatabaseHelper db)
        {
            _db = db;
        }

        // LIST all appointments with patient and doctor names
        public IActionResult Index()
        {
            var appointments = new List<Appointment>();
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT 
                        a.AppointmentId,
                        p.Name AS PatientName,
                        d.Name AS DoctorName,
                        d.Specialization,
                        a.AppointmentDate,
                        a.Status
                    FROM Appointments a
                    INNER JOIN Patients p ON a.PatientId = p.PatientId
                    INNER JOIN Doctors d ON a.DoctorId = d.DoctorId", conn);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    appointments.Add(new Appointment
                    {
                        AppointmentId = (int)reader["AppointmentId"],
                        PatientName = reader["PatientName"].ToString(),
                        DoctorName = reader["DoctorName"].ToString(),
                        Specialization = reader["Specialization"].ToString(),
                        AppointmentDate = (DateTime)reader["AppointmentDate"],
                        Status = reader["Status"].ToString()
                    });
                }
            }
            return View(appointments);
        }

        // SHOW booking form with patient and doctor dropdowns
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        // SAVE new appointment to DB
        [HttpPost]
        public IActionResult Create(Appointment appointment)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    INSERT INTO Appointments 
                    (PatientId, DoctorId, AppointmentDate, Status) 
                    VALUES (@PatientId, @DoctorId, @AppointmentDate, @Status)", conn);
                cmd.Parameters.AddWithValue("@PatientId", appointment.PatientId);
                cmd.Parameters.AddWithValue("@DoctorId", appointment.DoctorId);
                cmd.Parameters.AddWithValue("@AppointmentDate", appointment.AppointmentDate);
                cmd.Parameters.AddWithValue("@Status", "Scheduled");
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // DELETE an appointment
        public IActionResult Delete(int id)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "DELETE FROM Appointments WHERE AppointmentId = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // Helper — loads patient and doctor dropdowns
        private void LoadDropdowns()
        {
            var patients = new List<SelectListItem>();
            var doctors = new List<SelectListItem>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();

                // Load patients
                var patCmd = new SqlCommand("SELECT PatientId, Name FROM Patients", conn);
                var patReader = patCmd.ExecuteReader();
                while (patReader.Read())
                {
                    patients.Add(new SelectListItem
                    {
                        Value = patReader["PatientId"].ToString(),
                        Text = patReader["Name"].ToString()
                    });
                }
                patReader.Close();

                // Load doctors
                var docCmd = new SqlCommand("SELECT DoctorId, Name, Specialization FROM Doctors", conn);
                var docReader = docCmd.ExecuteReader();
                while (docReader.Read())
                {
                    doctors.Add(new SelectListItem
                    {
                        Value = docReader["DoctorId"].ToString(),
                        Text = docReader["Name"].ToString() + " - " + docReader["Specialization"].ToString()
                    });
                }
                docReader.Close();
            }

            ViewBag.Patients = patients;
            ViewBag.Doctors = doctors;
        }
    }
}