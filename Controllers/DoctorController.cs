using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EHRMvcCleanDemo.Data;

namespace EHRMvcCleanDemo.Controllers
{
    // Handles Doctor directory
    public class DoctorController : Controller
    {
        private readonly EHRDbContext _context;

        public DoctorController(EHRDbContext context)
        {
            _context = context;
        }

        // GET: /Doctor
        public async Task<IActionResult> Index()
        {
            // Minimum necessary rule
            var doctors = await _context.Doctors
                .Where(d => d.IsActive == true)
                .ToListAsync();

            return View(doctors);
        }

        // GET: /Doctor/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.DoctorId == id);

            if (doctor == null)
                return NotFound();

            return View(doctor);
        }

        // GET: /Doctor/AppointmentsOverview
        /// <summary>
        /// Displays a summary of all active doctors with their appointment statistics
        /// HIPAA Compliance: This view is SAFE because:
        /// 1. NO patient names or identifiable information is displayed
        /// 2. Only aggregate counts (total appointments) are shown
        /// 3. Only appointment dates are shown (no patient-specific details)
        /// 4. Follows "minimum necessary" standard - only summary data needed for scheduling
        /// </summary>
        public async Task<IActionResult> AppointmentsOverview()
        {
            // Business logic is placed HERE in the controller (not in the view) because:
            // 1. Separation of Concerns - Controllers handle data retrieval and business logic
            // 2. Testability - Logic in controllers can be unit tested
            // 3. Reusability - This logic could be reused in APIs or other actions
            // 4. Performance - LINQ queries should execute at the data layer, not in views
            // 5. Maintainability - Complex queries in views make them hard to read and maintain

            var today = DateTime.Today;

            // Use GroupJoin to include doctors with zero appointments
            var summary = await _context.Doctors
                .Where(d => d.IsActive == true) // Only active doctors
                .GroupJoin(
                    _context.Appointments,
                    doctor => doctor.DoctorId,
                    appointment => appointment.DoctorId,
                    (doctor, appointments) => new Models.DoctorAppointmentSummaryDTO
                    {
                        DoctorId = doctor.DoctorId,
                        FullName = doctor.FirstName + " " + doctor.LastName,
                        Specialty = doctor.Specialty,

                        // Count all appointments for this doctor
                        TotalAppointments = appointments.Count(),

                        // Find the earliest upcoming appointment date
                        NextAppointmentDate = appointments
                            .Where(a => a.AppointmentDate >= today)
                            .OrderBy(a => a.AppointmentDate)
                            .Select(a => (DateTime?)a.AppointmentDate)
                            .FirstOrDefault()
                    })
                .OrderBy(d => d.FullName) // Sort alphabetically by name
                .ToListAsync();

            return View(summary);
        }
    }
}
