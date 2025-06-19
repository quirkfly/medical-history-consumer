using System.Text.Json;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using PatientMedicalHistory.Data; 
using PatientMedicalHistory.Models;
using PatientMedicalHistory.Models.DTOs;
using Microsoft.Extensions.Configuration;


class Program
{
    private const string KafkaTopic = "patient-medical-history";
    private const string KafkaGroupId = "patient-consumer-group";

    static async Task Main(string[] args)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "192.168.101.167:9092", // change as needed
            GroupId = KafkaGroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(KafkaTopic);

        Console.WriteLine("Kafka consumer started. Listening to topic: " + KafkaTopic);

        var cancellationToken = new CancellationTokenSource();

        // Run consumption loop
        try
        {   
            while (!cancellationToken.Token.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(cancellationToken.Token);

                Console.WriteLine($"Received raw message: {consumeResult.Message.Value}");

                var patientDto = JsonSerializer.Deserialize<PatientDto>(consumeResult.Message.Value);
                if (patientDto == null)
                {
                    Console.WriteLine("Received null patient DTO, skipping...");
                    continue;
                } 
                Console.WriteLine($"Received patient: {patientDto.FirstName} {patientDto.LastName}");

                await SaveOrUpdatePatientAsync(patientDto);
            }
        }
        catch (OperationCanceledException)
        {
            // Graceful shutdown
            Console.WriteLine("Consumer stopping...");
        }
        finally
        {
            consumer.Close();
        }
    }

    private static async Task SaveOrUpdatePatientAsync(PatientDto dto)
    {
        Console.WriteLine(JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true }));

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<MedicalHistoryContext>();
        var connectionString = configuration.GetConnectionString("PatientMedicalHistoryDatabase");
        optionsBuilder.UseNpgsql(connectionString);

        using var dbContext = new MedicalHistoryContext(optionsBuilder.Options);

        // Look for existing patient
        var patient = await dbContext.Patients
            .Include(p => p.MedicalHistory)
            .FirstOrDefaultAsync(p => p.Id == dto.Id);

        if (patient == null)
        {
            // Create new patient
            patient = new Patient
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Age = dto.Age,
                MedicalHistory = new List<MedicalHistoryEntry>()
            };
            dbContext.Patients.Add(patient);
        }
        else
        {
            // Update fields
            patient.FirstName = dto.FirstName;
            patient.LastName = dto.LastName;
            patient.Age = dto.Age;
        }

        // Update or add medical history entries
        foreach (var entryDto in dto.MedicalHistory)
        {
            if (entryDto.Date.Kind == DateTimeKind.Unspecified)
            {
                entryDto.Date = DateTime.SpecifyKind(entryDto.Date, DateTimeKind.Utc);
            }   

            var entry = patient.MedicalHistory.Find(e => e.Id == entryDto.Id);
            if (entry == null)
            {
                entry = new MedicalHistoryEntry
                {
                    Id = entryDto.Id,
                    Date = entryDto.Date,
                    Diagnosis = entryDto.Diagnosis,
                    PatientId = patient.Id
                };
                patient.MedicalHistory.Add(entry);
            }
            else
            {
                entry.Date = entryDto.Date;
                entry.Diagnosis = entryDto.Diagnosis;
            }
        }

        await dbContext.SaveChangesAsync();

        Console.WriteLine($"Patient {patient.Id} saved/updated.");
    }
}
