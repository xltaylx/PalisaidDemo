using DotNetEnv;

Env.Load(".env.local");

string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("CONNECTION_STRING is not defined in .env.local.");
    return;
}
int patientCount = 10;
bool reset = false;

// Parse CLI arguments first
foreach (var arg in args)
{
    if (arg.StartsWith("--records="))
    {
        patientCount = int.Parse(arg.Split("=")[1]);
    }
    else if (arg == "--reset")
    {
        reset = true;
    }
}

// Now instantiate the Seeder *once*
var seeder = new SeederService(connectionString);

if (reset)
{
    Console.WriteLine("Resetting database...");
    seeder.ResetDatabase();
}

Console.WriteLine($"Seeding database with {patientCount} patients...");
seeder.Seed(patientCount);
Console.WriteLine("Seeding complete");
