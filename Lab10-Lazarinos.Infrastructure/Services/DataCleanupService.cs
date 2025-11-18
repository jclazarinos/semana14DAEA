using System;

namespace Lab10_Lazarinos.Infrastructure.Services
{
    public class DataCleanupService
    {
        public void PerformDatabaseCleanup()
        {
            // Tarea personalizada: Simular la eliminación de datos antiguos
            Console.WriteLine("--- INICIANDO TAREA RECURRENTE DE LIMPIEZA DE DATOS ---");
            int recordsDeleted = new Random().Next(100, 500);
            Console.WriteLine($"[Limpieza] {recordsDeleted} registros antiguos eliminados en {DateTime.Now}");
            Console.WriteLine("--- TAREA DE LIMPIEZA FINALIZADA ---");
        }
    }
}