using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Backend;

namespace Frontend
{
    class Program
    {
        static readonly HttpClient httpClient = new HttpClient();
        static string appointmentUrl = "http://localhost:7071/api/appointment";
        static string appointmentsUrl = "http://localhost:7071/api/appointments";

        static void Main(string[] args)
        {
            // Da Main nicht asynchron sein kann, wird diese Zeile als Workaround genutzt.
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            while(true)
            {
                Console.Clear();
                Console.WriteLine("1. Post Appointment");
                Console.WriteLine("2. Get Appointments By Date");
                Console.WriteLine("3. Get Appointment By Id");
                Console.WriteLine("4. Edit Appointment");
                Console.WriteLine("5. Delete Appointment");
                Console.WriteLine("6. Exit Application");
                Console.Write("\nWählen Sie einen Menüpunkt aus: ");

                int input = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine();

                switch(input)
                {
                    case 1:
                        Appointment apmtToCreate = new Appointment();
                        Console.WriteLine("Bitte geben Sie folgende Termindaten ein:");
                        Console.Write("Name: ");
                        apmtToCreate.Name = Console.ReadLine();
                        Console.Write("Note: ");
                        apmtToCreate.Note = Console.ReadLine();
                        Console.Write("Date: ");
                        apmtToCreate.Date = Convert.ToDateTime(Console.ReadLine());
                        Appointment apmtCreated = await PostAppointment(apmtToCreate);
                        Console.WriteLine("\nTermin erstellt!");
                        PrintAppointment(apmtCreated);
                        break;
                    case 2:
                        Console.Write("Geben Sie ein Datum ein und ggf. eine Uhrzeit: ");
                        DateTime dateToSearch = Convert.ToDateTime(Console.ReadLine());
                        List<Appointment> appointmentList = await GetAppointmentsByDate(dateToSearch);
                        Console.WriteLine();

                        foreach(Appointment apmt in appointmentList)
                        {
                            PrintSeparatorLine();
                            Console.WriteLine($"ID: {apmt.Id}");
                            Console.WriteLine($"Name: {apmt.Name}");
                            Console.WriteLine($"Note: {apmt.Note}");
                            Console.WriteLine($"Date: {apmt.Date}");
                        }

                        PrintSeparatorLine();
                        break;
                    case 3:
                        Console.Write("Geben Sie die ID des zu suchenden Termins ein: ");
                        int idToSearch = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine();
                        Appointment apmtToSearch = await GetAppointmentById(idToSearch);
                        PrintAppointment(apmtToSearch);
                        break;
                    case 4:
                        Console.Write("Geben Sie die ID des zu bearbeitenden Termins ein: ");
                        int idToEdit = Convert.ToInt32(Console.ReadLine());
                        Appointment apmtToEdit = new Appointment();
                        Console.WriteLine("Bitte geben Sie folgende Termindaten ein:");
                        Console.Write("Name: ");
                        apmtToEdit.Name = Console.ReadLine();
                        Console.Write("Note: ");
                        apmtToEdit.Note = Console.ReadLine();
                        Console.Write("Date: ");
                        apmtToEdit.Date = Convert.ToDateTime(Console.ReadLine());
                        Appointment apmtEdited = await PutAppointment(apmtToEdit, idToEdit);
                        Console.WriteLine("\nTermin bearbeitet!");
                        PrintAppointment(apmtEdited);
                        break;
                    case 5:
                        Console.Write("Geben Sie die ID des zu löschenden Termins ein: ");
                        int idToDelete = Convert.ToInt32(Console.ReadLine());
                        string responseDelete = await DeleteAppointment(idToDelete);
                        Console.WriteLine();
                        Console.WriteLine(responseDelete);
                        break;
                    case 6:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("\nUngültige Eingabe.");
                        Console.WriteLine("Drücken Sie eine beliebige Taste, um zum Menü zurückzukehren...");
                        Console.ReadKey();
                        break;
                }

                Console.WriteLine("\nDrücken Sie eine beliebige Taste, um zum Menü zurückzukehren...");
                Console.ReadKey();
            }
        }        

        public static async Task<Appointment> PostAppointment(Appointment appointment)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(appointmentUrl, appointment);
                string responseString = await response.Content.ReadAsStringAsync();
                Appointment createdAppointment = JsonConvert.DeserializeObject<Appointment>(responseString);
                return createdAppointment;
            }
            catch(Exception ex)
            {
                HandleErrorMessage(ex);
                return new Appointment();
            }
        }

        public static async Task<List<Appointment>> GetAppointmentsByDate(DateTime dateToSearch)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(appointmentsUrl + $"/{dateToSearch}");
                string responseString = await response.Content.ReadAsStringAsync();
                List<Appointment> appointments = JsonConvert.DeserializeObject<List<Appointment>>(responseString);
                return appointments;
            }
            catch(Exception ex)
            {
                HandleErrorMessage(ex);
                return new List<Appointment>();
            }
        }

        public static async Task<Appointment> GetAppointmentById(int id)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(appointmentUrl + $"/{id}");
                string responseString = await response.Content.ReadAsStringAsync();
                Appointment appointment = JsonConvert.DeserializeObject<Appointment>(responseString);
                return appointment;
            }
            catch(Exception ex)
            {
                HandleErrorMessage(ex);
                return new Appointment();
            }
        }

        public static async Task<Appointment> PutAppointment(Appointment appointment, int id)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PutAsJsonAsync(appointmentUrl + $"/{id}", appointment);
                string responseString = await response.Content.ReadAsStringAsync();
                Appointment editedAppointment = JsonConvert.DeserializeObject<Appointment>(responseString);
                return editedAppointment;
            }
            catch (Exception ex)
            {
                HandleErrorMessage(ex);
                return new Appointment();
            }
        }

        public static async Task<string> DeleteAppointment(int id)
        {
            try
            {
                HttpResponseMessage response = await httpClient.DeleteAsync(appointmentUrl + $"/{id}");
                string responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
            catch(Exception ex)
            {
                HandleErrorMessage(ex);
                return "";
            }
        }

        public static void PrintSeparatorLine()
        {
            Console.WriteLine("--------------------------------------");
        }

        public static void PrintAppointment(Appointment appointment)
        {
            PrintSeparatorLine();
            Console.WriteLine($"ID: {appointment.Id}");
            Console.WriteLine($"Name: {appointment.Name}");
            Console.WriteLine($"Note: {appointment.Note}");
            Console.WriteLine($"Date: {appointment.Date}");
            PrintSeparatorLine();
        }

        public static void HandleErrorMessage(Exception ex)
        {
            Console.WriteLine($"\nIrgendetwas lief da schief! Details: {ex.Message}\n");
        }
    }
}
