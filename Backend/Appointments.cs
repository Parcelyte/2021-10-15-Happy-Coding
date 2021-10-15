using System.Collections.Generic;

namespace Backend
{
    public class Appointments
    {
        public static int IdCounter = 0;
        /// <summary>
        /// In-Memory Liste, welche als Fake-Datenbank fungiert als Beispiel.
        /// </summary>
        public static List<Appointment> List = new List<Appointment>();
    }
}
