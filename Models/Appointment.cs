using System.ComponentModel.DataAnnotations;

namespace EHRMvcCleanDemo.Models
{
    // Maps to Healthcare.Appointments
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        public DateTime AppointmentDate { get; set; }
        public int DurationMinutes { get; set; }

        // Required medical info with validation
        [Required(ErrorMessage = "Reason for visit is required.")]
        [MinLength(10, ErrorMessage = "Reason for visit must be at least 10 characters long.")]
        public string ReasonForVisit { get; set; } = string.Empty;

        // Scheduled / Completed / Cancelled
        public string Status { get; set; } = "Scheduled";

        public string? Notes { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
