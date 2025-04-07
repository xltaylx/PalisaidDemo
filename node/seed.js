import { createClient } from "@supabase/supabase-js";
import { faker } from "@faker-js/faker";
import minimist from "minimist";
import dotenv from 'dotenv'

dotenv.config({ path: '.env.local' })
const args = minimist(process.argv.slice(2))
const reset = args.reset || false
const recordCount = parseInt(args.records) || 10

const supabase = createClient(
    process.env.NEXT_PUBLIC_SUPABASE_URL,
    process.env.NEXT_PUBLIC_SUPABASE_ANON_KEY
)

async function resetTables()
{
    console.log("Resetting tables...")
    await supabase.from('appointment').delete().neq('id', 0)
    await supabase.from('patient').delete().neq('id', 0)
    await supabase.from('location').delete().neq('id', 0)
    console.log("All tables cleared.")
}

async function seedData(count = 10) {
    console.log(`Seeding ${count} patients...`)

    const locations = Array.from({ length: 5 }).map(() => ({
        name: faker.company.name(),
        city: faker.location.city(),
        state: faker.location.state(),
    }))

    const { data: insertedLocations, error: locationError } = await supabase.from('location').insert(locations).select()
    if (locationError) {
        console.error("Error inserting locations:", locationError)
        return
    }

    const patients = Array.from({ length: count }).map(() => ({
        name: faker.person.fullName(),
        dateofbirth: faker.date.birthdate({ min: 18, max: 90, mode: 'age' }),
        email: faker.internet.email(),
        phone: faker.phone.number()
    }))

    const { data: insertedPatients, error: patientError } = await supabase.from('patient').insert(patients).select()
    if (patientError) {
        console.error("Error inserting patients:", patientError)
        return
    }

    const appointments = []

    for (let i = 0; i < count; i++) {
        appointments.push({
            patientid: insertedPatients[i].id,
            locationid: faker.helpers.arrayElement(insertedLocations).id,
            date: faker.date.soon({ days: 60 }),
            reason: faker.helpers.arrayElement(['Check-up', 'Consultation', 'Follow-up']),
        })
    }

    const { error: appointmentError } = await supabase.from('appointment').insert(appointments)
    if (appointmentError) {
        console.error("Error inserting appointments:", appointmentError)
        return
    }

    console.log("Seeding complete.")
}

async function main() {
    if (reset) await resetTables()
    await seedData(recordCount)
}

main()
