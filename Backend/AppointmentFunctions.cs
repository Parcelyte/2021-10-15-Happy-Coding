using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Backend
{
    public class AppointmentFunctions
    {
        [FunctionName(nameof(PostAppointment))]
        public static async Task<IActionResult> PostAppointment(
            [HttpTrigger(AuthorizationLevel.Anonymous,
            "post", Route = "appointment")]
            HttpRequest req)
        {
            try
            {
                string json = await new StreamReader(req.Body).ReadToEndAsync();
                Appointment newApmt = JsonConvert.DeserializeObject<Appointment>(json);

                newApmt.Id = Appointments.IdCounter + 1;
                Appointments.IdCounter++;

                if (Appointments.List.FirstOrDefault(a => a.Id == newApmt.Id) != null)
                {
                    return new BadRequestObjectResult($"Ein Termin mit der angegebenen ID existiert bereits!");
                }

                Appointments.List.Add(newApmt);

                return new OkObjectResult(newApmt);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Fehler bei der Verarbeitung Ihrer Anfrage. Details: {ex.Message}");
            }
        }

        [FunctionName(nameof(GetAppointments))]
        public static async Task<IActionResult> GetAppointments(
            [HttpTrigger(AuthorizationLevel.Anonymous,
            "get", Route = "appointments/{date:DateTime}")]
            HttpRequest req, DateTime date)
        {
            try
            {
                List<Appointment> exApmts = Appointments.List.FindAll(a => a.Date.Date == date.Date);

                if (exApmts.Count < 1)
                {
                    return new NotFoundObjectResult($"Für das angegebene Datum existieren keine Termine!");
                }

                return new OkObjectResult(exApmts);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Fehler bei der Verarbeitung Ihrer Anfrage. Details: {ex.Message}");
            }
        }

        [FunctionName(nameof(GetAppointment))]
        public static async Task<IActionResult> GetAppointment(
            [HttpTrigger(AuthorizationLevel.Anonymous,
            "get", Route = "appointment/{id:int}")]
            HttpRequest req, int id)
        {
            try
            {
                Appointment exApmt = Appointments.List.FirstOrDefault(a => a.Id == id);

                if (exApmt == null)
                {
                    return new NotFoundObjectResult($"Termin mit der angegebenen ID existiert nicht!");
                }

                return new OkObjectResult(exApmt);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Fehler bei der Verarbeitung Ihrer Anfrage. Details: {ex.Message}");
            }
        }     

        [FunctionName(nameof(PutAppointment))]
        public static async Task<IActionResult> PutAppointment(
            [HttpTrigger(AuthorizationLevel.Anonymous,
            "put", Route = "appointment/{id:int}")]
            HttpRequest req, int id)
        {
            try
            {
                string json = await new StreamReader(req.Body).ReadToEndAsync();
                Appointment updApmt = JsonConvert.DeserializeObject<Appointment>(json);
                Appointment exApmt = Appointments.List.FirstOrDefault(a => a.Id == id);

                if (exApmt == null)
                {
                    return new NotFoundObjectResult($"Termin mit der angegebenen ID existiert nicht!");
                }

                exApmt.Name = !string.IsNullOrEmpty(updApmt.Name) ? updApmt.Name : exApmt.Name;
                exApmt.Note = !string.IsNullOrEmpty(updApmt.Note) ? updApmt.Note : exApmt.Note;
                exApmt.Date = updApmt.Date != null ? updApmt.Date : exApmt.Date;

                return new OkObjectResult(exApmt);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Fehler bei der Verarbeitung Ihrer Anfrage. Details: {ex.Message}");
            }
        }

        [FunctionName(nameof(DeleteAppointment))]
        public static async Task<IActionResult> DeleteAppointment(
            [HttpTrigger(AuthorizationLevel.Anonymous,
            "delete", Route = "appointment/{id:int}")]
            HttpRequest req, int id)
        {
            try
            {
                Appointment exApmt = Appointments.List.FirstOrDefault(a => a.Id == id);

                if (exApmt == null)
                {
                    return new BadRequestObjectResult($"Ein Termin mit der angegebenen ID existiert nicht!");
                }

                Appointments.List.Remove(exApmt);

                return new OkObjectResult($"Termin mit der ID {id} wurde erfolgreich gelöscht!");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Fehler bei der Verarbeitung Ihrer Anfrage. Details: {ex.Message}");
            }
        }
    }
}
