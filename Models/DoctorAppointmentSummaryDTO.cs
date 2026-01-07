namespace EHRMvcCleanDemo.Models
{
    /// <summary>
    /// Data Transfer Object for Doctor Appointment Summary
    /// Used to display aggregated appointment data in a read-only view
    /// HIPAA-Safe: Contains NO patient information, only aggregate counts and dates
    /// </summary>
    public class DoctorAppointmentSummaryDTO
    {
        public int DoctorId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Specialty { get; set; } = string.Empty;

        /// <summary>
        /// Total count of all appointments for this doctor
        /// </summary>
        public int TotalAppointments { get; set; }

        /// <summary>
        /// Date of the next upcoming appointment (null if no upcoming appointments)
        /// </summary>
        public DateTime? NextAppointmentDate { get; set; }
    }
}
