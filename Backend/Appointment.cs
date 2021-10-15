using System;

namespace Backend
{
    /// <summary>
    /// Klasse, welches ein Terminobjekt repräsentiert.
    /// </summary>
    public class Appointment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public DateTime Date { get; set; }
    }
}
