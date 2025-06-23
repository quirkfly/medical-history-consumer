
# Medical History Consumer

A .NET-based application designed to consume and process patient medical history data from a Kafka topic and persist it into a PostgreSQL database.

## Features

- **Kafka Consumer**: Listens to a Kafka topic (`patient-medical-history`) for incoming patient data.
- **Data Processing**: Deserializes JSON patient data and updates the local database.
- **Database Persistence**: Saves or updates patient records and their medical history using Entity Framework Core and PostgreSQL.
- **Error Handling**: Skips null or invalid patient data messages gracefully.

## Technologies Used

- **C#** and **.NET**
- **Confluent.Kafka** client for Kafka consumption
- **Entity Framework Core** with Npgsql provider for PostgreSQL integration
- **System.Text.Json** for JSON serialization/deserialization

## Overview

The application starts a Kafka consumer subscribed to the `patient-medical-history` topic. It continuously listens for new messages containing patient medical history in JSON format. Upon receiving a message, it deserializes the data into a `PatientDto` object and either inserts or updates the patient record along with their medical history entries in the PostgreSQL database.

## Kafka Consumer Configuration

- **Kafka Topic**: `patient-medical-history`
- **Consumer Group**: `patient-consumer-group`
- **Bootstrap Server**: Configured at `192.168.101.167:9092` (update as needed)
- **Auto Offset Reset**: Earliest

## Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/quirkfly/medical-history-consumer.git
   cd medical-history-consumer
   ```

2. Build the application:

   ```bash
   dotnet build
   ```

3. Configure your PostgreSQL connection string in `appsettings.json` under `ConnectionStrings:PatientMedicalHistoryDatabase`.

4. Run the application:

   ```bash
   dotnet run
   ```

## Usage

Run the consumer application to start listening for patient medical history messages from the Kafka topic. The consumer will print logs about the received messages and database updates.

## Contributing

Contributions are welcome! Please fork the repository, make your changes, and submit a pull request. Ensure that your code adheres to the project's coding standards and includes appropriate tests.

## License

This project is licensed under the MIT License.
