# Seeds the exact doctors + medications used in the original UI design,
# through the REAL API (Admin creates each doctor account, then each new
# doctor logs in and sets up their own weekly schedule, then Admin adds the
# medication catalog) -- not raw SQL, since Identity password hashing can't
# safely be replicated that way.
#
# REQUIREMENTS before running:
#   1. Backend must be running (.\run-backend.ps1 in another terminal)
#   2. You need an existing Admin account (edit $adminEmail/$adminPassword below)

$baseUrl = "https://localhost:7292/api"
$adminEmail = "mariam2020@gmail.com"
$adminPassword = "Mariam2004@@"

# All seeded doctors get this shared temporary password so the script can
# log in as each one immediately after creating them (only needed to set up
# their schedule) -- tell real users to change it via Profile afterwards.
$doctorPassword = "Doctor@12345"

function Invoke-Api {
    param($Method, $Path, $Token, $Body)
    $headers = @{}
    if ($Token) { $headers["Authorization"] = "Bearer $Token" }
    if ($Body) {
        return Invoke-RestMethod -Method $Method -Uri "$baseUrl$Path" -Headers $headers -Body ($Body | ConvertTo-Json) -ContentType "application/json"
    } else {
        return Invoke-RestMethod -Method $Method -Uri "$baseUrl$Path" -Headers $headers
    }
}

Write-Host "Logging in as admin..." -ForegroundColor Cyan
$adminLogin = Invoke-Api -Method Post -Path "/Auth/Login" -Body @{ email = $adminEmail; password = $adminPassword }
$adminToken = $adminLogin.token
Write-Host "Logged in as admin." -ForegroundColor Green

# ---- Doctors (exact list from the original UI design) ----
$doctors = @(
    @{ fullName = "Dr. Mona Khalil";    email = "mona.khalil@tabiby.health";    phone = "01012345678"; specialty = "Cardiology";        location = "Nasr City, Cairo" }
    @{ fullName = "Dr. Youssef Adel";   email = "youssef.adel@tabiby.health";   phone = "01098765432"; specialty = "Dermatology";       location = "Zamalek, Cairo" }
    @{ fullName = "Dr. Nourhan Samir";  email = "nourhan.samir@tabiby.health";  phone = "01234567890"; specialty = "Pediatrics";        location = "Maadi, Cairo" }
    @{ fullName = "Dr. Karim Fathy";    email = "karim.fathy@tabiby.health";    phone = "01122334455"; specialty = "Orthopedics";       location = "Heliopolis, Cairo" }
    @{ fullName = "Dr. Laila Hassan";   email = "laila.hassan@tabiby.health";   phone = "01555667788"; specialty = "Neurology";         location = "Dokki, Giza" }
    @{ fullName = "Dr. Omar Reda";      email = "omar.reda@tabiby.health";      phone = "01099887766"; specialty = "Dentistry";         location = "6th of October, Giza" }
)

# A generic Mon-Fri 9am-5pm schedule for every seeded doctor.
$weekdaySlots = @(
    @{ dayOfWeek = 1; startTime = "09:00:00"; endTime = "17:00:00" } # Monday
    @{ dayOfWeek = 2; startTime = "09:00:00"; endTime = "17:00:00" } # Tuesday
    @{ dayOfWeek = 3; startTime = "09:00:00"; endTime = "17:00:00" } # Wednesday
    @{ dayOfWeek = 4; startTime = "09:00:00"; endTime = "17:00:00" } # Thursday
    @{ dayOfWeek = 5; startTime = "09:00:00"; endTime = "17:00:00" } # Friday
)

foreach ($doc in $doctors) {
    Write-Host "`nCreating doctor: $($doc.fullName)..." -ForegroundColor Cyan
    try {
        Invoke-Api -Method Post -Path "/Admin/Doctors" -Token $adminToken -Body @{
            email        = $doc.email
            password     = $doctorPassword
            displayName  = $doc.fullName
            phoneNumber  = $doc.phone
            specialty    = $doc.specialty
            location     = $doc.location
        } | Out-Null
        Write-Host "  Created." -ForegroundColor Green
    } catch {
        Write-Host "  Skipped (likely already exists): $($_.Exception.Message)" -ForegroundColor Yellow
    }

    Write-Host "  Logging in as $($doc.fullName) to set up their schedule..." -ForegroundColor Cyan
    try {
        $docLogin = Invoke-Api -Method Post -Path "/Auth/Login" -Body @{ email = $doc.email; password = $doctorPassword }
        $docToken = $docLogin.token

        foreach ($slot in $weekdaySlots) {
            try {
                Invoke-Api -Method Post -Path "/DoctorSchedule" -Token $docToken -Body @{
                    dayOfWeek   = $slot.dayOfWeek
                    startTime   = $slot.startTime
                    endTime     = $slot.endTime
                    isAvailable = $true
                } | Out-Null
            } catch {
                Write-Host "    Slot skipped (likely already exists)" -ForegroundColor Yellow
            }
        }
        Write-Host "  Schedule set up." -ForegroundColor Green
    } catch {
        Write-Host "  Could not log in as this doctor: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# ---- Medications (exact list from the original UI design) ----
$medications = @(
    @{ name = "Paracetamol 500mg";              price = 15; stock = 240; isAvailable = $true }
    @{ name = "Amoxicillin 250mg Capsules";      price = 42; stock = 120; isAvailable = $true }
    @{ name = "Vitamin C Effervescent";          price = 60; stock = 0;   isAvailable = $false }
    @{ name = "Omeprazole 20mg";                 price = 35; stock = 80;  isAvailable = $true }
    @{ name = "Cetirizine 10mg";                 price = 22; stock = 150; isAvailable = $true }
    @{ name = "Ibuprofen 400mg";                 price = 18; stock = 200; isAvailable = $true }
)

Write-Host "`nCreating medications..." -ForegroundColor Cyan
foreach ($med in $medications) {
    Write-Host "  $($med.name)..." -NoNewline
    try {
        # CreateMedication expects multipart/form-data (it accepts an optional
        # image file), so this uses form fields rather than JSON.
        $form = @{
            Name        = $med.name
            Price       = $med.price
            Stock       = $med.stock
            IsAvailable = $med.isAvailable
        }
        Invoke-RestMethod -Method Post -Uri "$baseUrl/Medication/CreateMedication" `
            -Headers @{ Authorization = "Bearer $adminToken" } `
            -Form $form | Out-Null
        Write-Host " created." -ForegroundColor Green
    } catch {
        Write-Host " skipped (likely already exists): $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

Write-Host "`nDone! Refresh the frontend and check Find a Doctor / Pharmacy." -ForegroundColor Green
