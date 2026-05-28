using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PatientManagementSystem.Models;

namespace PatientManagementSystem.Controllers
{
    public class PatientController : Controller
    {
        private readonly DatabaseHelper _db;

        public PatientController(DatabaseHelper db)
        {
            _db = db;
        }

        // LIST all patients
        public IActionResult Index()
        {
            var patients = new List<Patient>();
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Patients", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    patients.Add(new Patient
                    {
                        PatientId = (int)reader["PatientId"],
                        Name = reader["Name"].ToString(),
                        Age = (int)reader["Age"],
                        Gender = reader["Gender"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        Address = reader["Address"].ToString()
                    });
                }
            }
            return View(patients);
        }

        // SHOW add patient form
        public IActionResult Create()
        {
            return View();
        }

        // SAVE new patient to DB
        [HttpPost]
        public IActionResult Create(Patient patient)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "INSERT INTO Patients (Name, Age, Gender, Phone, Address) VALUES (@Name, @Age, @Gender, @Phone, @Address)",
                    conn);
                cmd.Parameters.AddWithValue("@Name", patient.Name);
                cmd.Parameters.AddWithValue("@Age", patient.Age);
                cmd.Parameters.AddWithValue("@Gender", patient.Gender);
                cmd.Parameters.AddWithValue("@Phone", patient.Phone);
                cmd.Parameters.AddWithValue("@Address", patient.Address);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // DELETE a patient
        public IActionResult Delete(int id)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "DELETE FROM Patients WHERE PatientId = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }
    }
}