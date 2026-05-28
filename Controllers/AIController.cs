using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PatientManagementSystem.Models;

namespace PatientManagementSystem.Controllers
{
    public class AIController : Controller
    {
        private readonly GroqService _groq;
        private readonly DatabaseHelper _db;

        public AIController(GroqService groq, DatabaseHelper db)
        {
            _groq = groq;
            _db = db;
        }

        // Symptom Checker page
        public IActionResult SymptomChecker()
        {
            return View();
        }

        // Process symptoms and return doctor suggestion
        [HttpPost]
        public async Task<IActionResult> SymptomChecker(string symptoms)
        {
            var prompt = $@"You are a medical assistant. 
            A patient describes these symptoms: {symptoms}
            
            Based on these symptoms, suggest ONE doctor specialization 
            from this list only:
            Cardiology, Neurology, Orthopedics, General Medicine, 
            Dermatology, Pediatrics, Psychiatry
            
            Reply with ONLY the specialization name, nothing else.
            No explanations, no punctuation, just the single word 
            or two word specialization name.";

            // Get AI response
            var specialization = await _groq.GetResponseAsync(prompt);
            specialization = specialization.Trim();

            // Find matching doctors from DB
            var doctors = new List<Doctor>();
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT * FROM Doctors WHERE Specialization LIKE @Spec",
                    conn);
                cmd.Parameters.AddWithValue("@Spec", $"%{specialization}%");
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

            ViewBag.Symptoms = symptoms;
            ViewBag.Specialization = specialization;
            ViewBag.Doctors = doctors;

            return View();
        }
    }
}