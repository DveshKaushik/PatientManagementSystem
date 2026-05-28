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

        // ─── Symptom Checker ───────────────────────────────────────

        public IActionResult SymptomChecker()
        {
            return View();
        }

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

            var specialization = await _groq.GetResponseAsync(prompt);
            specialization = specialization.Trim();

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

        // ─── Chatbot ───────────────────────────────────────────────

        public IActionResult Chatbot()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Chatbot(string message)
        {
            var totalPatients = 0;
            var totalDoctors = 0;
            var totalAppointments = 0;
            var doctorsList = new List<string>();
            var patientsList = new List<string>();
            var appointmentsList = new List<string>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();

                // Get counts
                totalPatients = (int)new SqlCommand(
                    "SELECT COUNT(*) FROM Patients", conn).ExecuteScalar();
                totalDoctors = (int)new SqlCommand(
                    "SELECT COUNT(*) FROM Doctors", conn).ExecuteScalar();
                totalAppointments = (int)new SqlCommand(
                    "SELECT COUNT(*) FROM Appointments", conn).ExecuteScalar();

                // Get doctors list
                var docReader = new SqlCommand(
                    "SELECT Name, Specialization FROM Doctors", conn)
                    .ExecuteReader();
                while (docReader.Read())
                    doctorsList.Add(
                        $"{docReader["Name"]} ({docReader["Specialization"]})");
                docReader.Close();

                // Get patients list
                var patReader = new SqlCommand(
                    "SELECT Name, Age, Gender FROM Patients", conn)
                    .ExecuteReader();
                while (patReader.Read())
                    patientsList.Add(
                        $"{patReader["Name"]}, Age {patReader["Age"]}, {patReader["Gender"]}");
                patReader.Close();

                // Get appointments list
                var apptReader = new SqlCommand(@"
                    SELECT p.Name AS Patient, d.Name AS Doctor,
                           a.AppointmentDate, a.Status
                    FROM Appointments a
                    INNER JOIN Patients p ON a.PatientId = p.PatientId
                    INNER JOIN Doctors d ON a.DoctorId = d.DoctorId",
                    conn).ExecuteReader();
                while (apptReader.Read())
                    appointmentsList.Add(
                        $"{apptReader["Patient"]} with {apptReader["Doctor"]} " +
                        $"on {Convert.ToDateTime(apptReader["AppointmentDate"]).ToString("dd MMM yyyy")} " +
                        $"({apptReader["Status"]})");
                apptReader.Close();
            }

            // Build context for AI
            var context = $@"
                You are a helpful assistant for a Patient Management System.
                Here is the current data:

                Total Patients: {totalPatients}
                Total Doctors: {totalDoctors}
                Total Appointments: {totalAppointments}

                Doctors: {string.Join(", ", doctorsList)}
                Patients: {string.Join(", ", patientsList)}
                Appointments: {string.Join(", ", appointmentsList)}

                Answer the following question based on this data only.
                Be concise and helpful. Question: {message}";

            var response = await _groq.GetResponseAsync(context);

            ViewBag.Message = message;
            ViewBag.Response = response;

            return View();
        }
    }
}