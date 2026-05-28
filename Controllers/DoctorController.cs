using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PatientManagementSystem.Models;

namespace PatientManagementSystem.Controllers
{
    public class DoctorController : Controller
    {
        private readonly DatabaseHelper _db;

        public DoctorController(DatabaseHelper db)
        {
            _db = db;
        }

        // LIST all doctors
        public IActionResult Index()
        {
            var doctors = new List<Doctor>();
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Doctors", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    doctors.Add(new Doctor
                    {
                        DoctorId = (int)reader["DoctorId"],
                        Name = reader["Name"].ToString(),
                        Specialization = reader["Specialization"].ToString(),
                        Phone = reader["Phone"].ToString()
                    });
                }
            }
            return View(doctors);
        }

        // SHOW add doctor form
        public IActionResult Create()
        {
            return View();
        }

        // SAVE new doctor to DB
        [HttpPost]
        public IActionResult Create(Doctor doctor)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "INSERT INTO Doctors (Name, Specialization, Phone) VALUES (@Name, @Specialization, @Phone)",
                    conn);
                cmd.Parameters.AddWithValue("@Name", doctor.Name);
                cmd.Parameters.AddWithValue("@Specialization", doctor.Specialization);
                cmd.Parameters.AddWithValue("@Phone", doctor.Phone);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // DELETE a doctor
        public IActionResult Delete(int id)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "DELETE FROM Doctors WHERE DoctorId = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }
    }
}