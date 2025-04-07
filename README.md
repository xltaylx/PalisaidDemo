# PalisaidDemo – Supabase Seeder (Node.js + .NET)

This project contains two implementations for seeding a Supabase PostgreSQL database with mock test data for a diabetes clinic dashboard:

**Node.js implementation** using Supabase JS SDK + Faker
**.NET Console App** using Dapper + Bogus

Both versions populate the same schema: `patient`, `location`, and `appointment`.

## 🔧 Supabase Setup
### `location`
| Column | Type               |
|--------|--------------------|
| id     | serial PRIMARY KEY |
| name   | text               |
| city   | text               |
| state  | text               |

### `patient`
| Column | Type               |
|--------|--------------------|
| id     | serial PRIMARY KEY |
| name   | text               |
| dob    | date               |
| email  | text               |
| phone  | text               |

### `appointment`
| Column     | Type               |
|------------|--------------------|
| id         | serial PRIMARY KEY |
| patientid  | int REFERENCES patient(id) |
| locationid | int REFERENCES location(id) |
| date       | timestamp          |
| reason     | text               |
---
## Front-End Environment Setup

Create a `.env.local` file at the root of the Next.js project with:

```env
NEXT_PUBLIC_SUPABASE_URL=https://dzrhyyhptitzewolflga.supabase.co
NEXT_PUBLIC_SUPABASE_ANON_KEY=anon_key_here
````
---

## Node.js Seeder Instructions

### Install & Run

```bash
cd node
npm install
node seed.js --reset --records=50
```
### CLI Options

- `--reset` → Drops all existing tables and recreates them  
- `--records=n` → Number of patients to create (default: 10)

---
## .NET Seeder Environment Setup

To run the .NET seeder, copy `.env.local.template` to `.env.local` inside the `/dotnet` folder and provide the actual connection string:

```env
CONNECTION_STRING=Host=your-db.supabase.co;Port=5432;Username=postgres;Password=db_password;Database=postgres;SSL Mode=Require;Trust Server Certificate=true;
```
---

## .NET Seeder Instructions

### Run

```bash
cd dotnet
dotnet run -- --reset --records=50
```
### CLI Options

- `--reset` → Drops and recreates the tables  
- `--records=n` → Number of patients to generate (default: 10)

---

## Output Behavior

Both seeders will:

- Insert 5 locations  
- Insert N patients  
- Create 1 appointment per patient using a random location  

The tables will be dropped and recreated if `--reset` is passed.
