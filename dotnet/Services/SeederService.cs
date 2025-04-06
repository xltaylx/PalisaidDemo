using Bogus;
using Dapper;
using Npgsql;

public class SeederService
{
    private readonly string _connectionString;
    
    public SeederService(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public void ResetDatabase()
{
    using var connection = new NpgsqlConnection(_connectionString);
    connection.Open();

    Console.WriteLine("Dropping tables if they exist...");
    connection.Execute("DROP TABLE IF EXISTS appointment CASCADE");
    connection.Execute("DROP TABLE IF EXISTS patient CASCADE");
    connection.Execute("DROP TABLE IF EXISTS location CASCADE");

    Console.WriteLine("Creating tables...");
    connection.Execute(@"
        CREATE TABLE IF NOT EXISTS location (
            id SERIAL PRIMARY KEY,
            name TEXT,
            city TEXT,
            state TEXT
        );

        CREATE TABLE IF NOT EXISTS patient (
            id SERIAL PRIMARY KEY,
            name TEXT,
            dateofbirth DATE,
            email TEXT,
            phone TEXT
        );

        CREATE TABLE IF NOT EXISTS appointment (
            id SERIAL PRIMARY KEY,
            patientid INT REFERENCES patient(id),
            locationid INT REFERENCES location(id),
            date TIMESTAMP,
            reason TEXT
        );
    ");

    Console.WriteLine("Tables created.");
}


    public void Seed(int patientCount = 10, int locationCount = 3, int appointmentsPerPatient = 3)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        var faker = new Faker();

        var locations = new Faker<Location>()
            .RuleFor(l => l.Name, f => f.Company.CompanyName())
            .RuleFor(l => l.City, f => f.Address.City())
            .RuleFor(l => l.State, f => f.Address.StateAbbr())
            .Generate(locationCount);
        
        foreach(var loc in locations)
        {
            connection.Execute("INSERT INTO Location (Name, City, State) VALUES (@Name, @City, @State)", loc);
        }
        var locationIds = connection.Query<int>("SELECT Id from Location").ToList();

        var patients = new Faker<Patient>()
            .RuleFor(p => p.Name, f => f.Name.FullName())
            .RuleFor(p => p.DateOfBirth, f => f.Date.Past(60, DateTime.Today.AddYears(-20)))
            .RuleFor(p => p.Email, f => f.Internet.Email())
            .RuleFor(p => p.Phone, f => f.Phone.PhoneNumber())
            .Generate(patientCount);
            
        foreach(var pat in patients)
        {
            connection.Execute("INSERT INTO Patient (Name, DateOfBirth, Email, Phone) VALUES (@Name, @DateOfBirth, @Email, @Phone)", pat);
        }
        var patientIds = connection.Query<int>("SELECT Id from Patient").ToList();

        var appointmentReasons = new[] {"Check-up", "Consultation", "Follow-up", "Lab work", "Screening" };

        var rnd = new Random();
        foreach(var pid in patientIds)
        {
            for(int i = 0; i < appointmentsPerPatient; i++)
            {
                var app = new Appointment 
                {
                    PatientId = pid,
                    LocationId = locationIds[rnd.Next(locationIds.Count)],
                    Date = faker.Date.Soon(60),
                    Reason = faker.PickRandom(appointmentReasons)
                };

                connection.Execute("INSERT INTO Appointment (PatientId, LocationId, Date, Reason) VALUES (@PatientId, @LocationId, @Date, @Reason)", app);
            }
        }
    }
}