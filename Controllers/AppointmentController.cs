using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EHRMvcCleanDemo.Data;
using EHRMvcCleanDemo.Models;

namespace EHRMvcCleanDemo.Controllers
{
    // Handles appointment creation
    public class AppointmentController : Controller
    {
        private readonly EHRDbContext _context;

        public AppointmentController(EHRDbContext context)
        {
            _context = context;
        }

        // GET: Appointment/Create
        public IActionResult Create()
        {
            ViewBag.Doctors = new SelectList(_context.Doctors, "DoctorId", "FullName");
            ViewBag.Patients = new SelectList(_context.Patients, "PatientId", "FullName");
            return View();
        }

        // POST: Appointment/Create
        [HttpPost]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            // Custom validation: Prevent appointments in the past
            // This is a business rule that ensures data quality and prevents scheduling errors
            if (appointment.AppointmentDate < DateTime.Now)
            {
                ModelState.AddModelError("AppointmentDate",
                    "Appointment date cannot be in the past. Please select a future date and time.");
            }

            // Check if model validation passed (includes both data annotations and custom validation)
            if (!ModelState.IsValid)
            {
                // Re-populate dropdowns for the view
                ViewBag.Doctors = new SelectList(_context.Doctors, "DoctorId", "FullName");
                ViewBag.Patients = new SelectList(_context.Patients, "PatientId", "FullName");
                return View(appointment);
            }

            // Set metadata fields
            appointment.CreatedDate = DateTime.UtcNow;
            appointment.Status = "Scheduled";

            _context.Appointments.Add(appointment);

            // HIPAA audit with enhanced details
            // Including appointment date and DoctorId improves audit trail completeness
            // This helps track WHEN appointments were scheduled and WITH WHICH provider
            _context.AuditLogs.Add(new AuditLog
            {
                UserId = "DemoUser",
                Action = "Create",
                TableName = "Appointments",
                RecordId = appointment.AppointmentId,
                PatientId = appointment.PatientId,
                AccessDate = DateTime.UtcNow,
                // Enhanced details: includes appointment date and doctor information
                Details = $"Created appointment for {appointment.AppointmentDate:yyyy-MM-dd HH:mm} with Doctor ID {appointment.DoctorId}. Reason: {appointment.ReasonForVisit}"
            });

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Doctor");
        }
    }
}
