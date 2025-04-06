string connectionString = "Host=db.dzrhyyhptitzewolflga.supabase.co;Port=5432;Username=postgres;Password=Spartanpbch2!;Database=postgres;SSL Mode=Require;Trust Server Certificate=true;";

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
