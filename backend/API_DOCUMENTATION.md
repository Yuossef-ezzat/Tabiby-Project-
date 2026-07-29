
# 🏥 Tabiby — Complete API Documentation

> **Version:** 1.0  
> **Last Updated:** July 6, 2026  
> **Generated From:** Source Code Analysis  
> **Base URL (Development):** `https://localhost:7292/api`

---

## Table of Contents

1. [Project Overview](#1-project-overview)
2. [Authentication Documentation](#2-authentication-documentation)
3. [Auth Module](#3-auth-module)
4. [Appointment Module](#4-appointment-module)
5. [Doctor Schedule Module](#5-doctor-schedule-module)
6. [Consultation Module](#6-consultation-module)
7. [Consultation Chat Module](#7-consultation-chat-module)
8. [Consultation Review Module](#8-consultation-review-module)
9. [Medication Module](#9-medication-module)
10. [Basket Module](#10-basket-module)
11. [Order Module](#11-order-module)
12. [Nursing Module](#12-nursing-module)
13. [Notification Module](#13-notification-module)
14. [Enum Documentation](#14-enum-documentation)
15. [DTO Documentation](#15-dto-documentation)
16. [Validation Documentation](#16-validation-documentation)
17. [AutoMapper Documentation](#17-automapper-documentation)
18. [Specification Documentation](#18-specification-documentation)
19. [Error Handling Documentation](#19-error-handling-documentation)
20. [SignalR Documentation](#20-signalr-documentation)
21. [File Upload Documentation](#21-file-upload-documentation)
22. [TypeScript Interfaces](#22-typescript-interfaces)
23. [Axios & React Query Examples](#23-axios--react-query-examples)
24. [Complete API Summary](#24-complete-api-summary)

---

# 1. Project Overview

## Architecture Overview

Tabiby is a **healthcare platform** API built with ASP.NET Core following a **3-Layer Architecture**:

| Layer | Project | Responsibility |
|-------|---------|---------------|
| **Presentation Layer (PL)** | `PL` | Controllers, Middleware, Extensions, Program.cs |
| **Business Logic Layer (BLL)** | `BLL` | Services, DTOs, AutoMapper, SignalR Hubs, Utilities |
| **Data Access Layer (DAL)** | `DAL` | Models, DbContext, Repository, Specifications, Exceptions |

## Main Modules

| Module | Description |
|--------|-------------|
| **Auth** | Registration, Login, Forgot/Reset Password (JWT + ASP.NET Identity) |
| **Appointment** | Patient books appointments with doctors, doctor confirms/completes |
| **Doctor Schedule** | Doctor manages weekly availability schedules |
| **Consultation** | Online consultation between patient and doctor |
| **Consultation Chat** | Real-time messaging within consultations (SignalR) |
| **Consultation Review** | Patient reviews completed consultations |
| **Medication** | Pharmacist manages medication catalog |
| **Basket** | Patient shopping cart for medications |
| **Order** | Patient places medication orders, pharmacist manages status |
| **Nursing** | Patient requests nursing services, nurse manages requests |
| **Notification** | Real-time notifications via SignalR |

## Database Overview

- **Database Engine:** SQL Server
- **ORM:** Entity Framework Core
- **Identity:** ASP.NET Identity with `int` as primary key type
- **Database Name:** `Tabibydb`

## User Roles

| Role | Description |
|------|-------------|
| `Patient` | Registered via the Register endpoint. Books appointments, requests consultations, orders medications, requests nursing |
| `Doctor` | Seeded. Manages schedule, confirms appointments, handles consultations |
| `Nurse` | Seeded. Handles nursing requests |
| `Pharmacist` | Seeded. Manages medications and order statuses |
| `Admin` | Seeded. Has Pharmacist-level access to medications and orders |

## CORS Configuration

Allowed Origins:
- `http://localhost:3000`
- `http://localhost:3001`

---

# 2. Authentication Documentation

## Authentication Method

**JWT Bearer Token** — passed in the `Authorization` header.

## Login Flow

```
POST /api/Auth/Login
    ↓ (email + password)
Response: { email, fullName, token }
    ↓
Store token in localStorage/cookie
    ↓
Include in all subsequent requests:
  Authorization: Bearer <token>
```

## Register Flow

```
POST /api/Auth/Register
    ↓ (email, password, displayName, ...)
Patient user is created
    ↓
User is assigned "Patient" role
    ↓
Response: { email, fullName, token }
```

> [!IMPORTANT]
> Registration **always** creates a `Patient` user. Doctors, Nurses, Pharmacists, and Admins are **seeded** into the database — there is no public registration endpoint for these roles.

## JWT Token Structure

| Claim | Source |
|-------|--------|
| `ClaimTypes.Email` | `user.Email` |
| `ClaimTypes.Name` | `user.UserName` |
| `ClaimTypes.NameIdentifier` | `user.Id` |
| `ClaimTypes.Role` | User roles (e.g. "Patient", "Doctor") |

## Token Configuration

| Property | Value |
|----------|-------|
| **Algorithm** | HMAC-SHA256 |
| **Lifetime** | 1 hour |
| **Issuer** | `https://localhost:7292/` |
| **Audience** | `https://localhost:7292/` |
| **Validate Issuer** | Yes |
| **Validate Audience** | Yes |
| **Validate Lifetime** | Yes |
| **Validate Signing Key** | Yes |

## Refresh Token

> [!WARNING]
> Refresh token is **NOT implemented**. The frontend must re-authenticate after token expiration (1 hour).

## Password Reset Flow

```
POST /api/Auth/forget-password  { email }
    ↓
Email sent with reset link
    ↓
POST /api/Auth/reset-password  { email, token, newPassword, confirmPassword }
    ↓
Password updated
```

---

# 3. Auth Module

---

## 3.1 Login

### Endpoint Information

| Field | Value |
|-------|-------|
| **Module** | Auth |
| **Endpoint Name** | Login |
| **HTTP Method** | `POST` |
| **URL** | `/api/Auth/Login` |
| **Description** | Authenticate a user and receive a JWT token |
| **Authentication Required** | No |
| **Authorization Roles** | None |

### Request

#### Headers

| Header | Value | Required |
|--------|-------|----------|
| Content-Type | `application/json` | Yes |
| Accept | `application/json` | No |

#### Request Body

```json
{
  "email": "patient1@tabiby.com",
  "password": "P@ssword123"
}
```

| Property | Data Type | Required | Nullable | Validation Rules | Description |
|----------|-----------|----------|----------|-----------------|-------------|
| `email` | `string` | Yes | No | `[EmailAddress]` — must be valid email format | User's email address |
| `password` | `string` | Yes | No | None in DTO | User's password |

### Success Response

**Status Code:** `200 OK`

```json
{
  "email": "patient1@tabiby.com",
  "fullName": "Ahmed Hassan",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

| Property | Data Type | Description |
|----------|-----------|-------------|
| `email` | `string` | The authenticated user's email |
| `fullName` | `string` | User's display name |
| `token` | `string` | JWT Bearer token (expires in 1 hour) |

### Error Responses

| Status | Reason | Example Response | When |
|--------|--------|-----------------|------|
| `404` | User not found | `{"stutsCode":404,"errorMsg":"User With Email: patient1@tabiby.com Not Found"}` | Email does not exist in the system |
| `401` | Invalid password | `{"stutsCode":401,"errorMsg":"Invalid email or password."}` | Password does not match |
| `400` | Validation error | `{"stutsCode":400,"errorMsg":"Validation Failed","errors":[...]}` | Invalid email format |

### Business Rules

- Looks up user by email using ASP.NET Identity `UserManager`
- Throws `UserNotFoundException` (→ 404) if email not found
- Throws `InvalidCredentialsException` (→ 401) if password is wrong
- Returns JWT token with Email, Name, NameIdentifier, and Role claims
- Token is valid for **1 hour**

### Frontend Notes

- Store the `token` value securely (localStorage, httpOnly cookie, etc.)
- Include token in all authenticated requests as `Authorization: Bearer <token>`
- Token expires in 1 hour — handle 401 responses by redirecting to login

### cURL Example

```bash
curl -X POST https://localhost:7292/api/Auth/Login \
  -H "Content-Type: application/json" \
  -d '{"email":"patient1@tabiby.com","password":"P@ssword123"}'
```

### Postman Example

- **Method:** POST
- **URL:** `https://localhost:7292/api/Auth/Login`
- **Body → Raw → JSON:**
```json
{
  "email": "patient1@tabiby.com",
  "password": "P@ssword123"
}
```

---

## 3.2 Register

### Endpoint Information

| Field | Value |
|-------|-------|
| **Module** | Auth |
| **Endpoint Name** | Register |
| **HTTP Method** | `POST` |
| **URL** | `/api/Auth/Register` |
| **Description** | Register a new Patient user |
| **Authentication Required** | No |
| **Authorization Roles** | None |

### Request

#### Headers

| Header | Value | Required |
|--------|-------|----------|
| Content-Type | `application/json` | Yes |

#### Request Body

```json
{
  "email": "newpatient@example.com",
  "password": "StrongP@ss1",
  "userName": "newpatient",
  "displayName": "New Patient",
  "phoneNumber": "+201234567890"
}
```

| Property | Data Type | Required | Nullable | Validation Rules | Description |
|----------|-----------|----------|----------|-----------------|-------------|
| `email` | `string` | Yes | No | None in DTO (Identity validates uniqueness & format) | New user's email |
| `password` | `string` | Yes | No | ASP.NET Identity defaults (uppercase, lowercase, digit, 6+ chars) | Account password |
| `userName` | `string` | No | Yes | If null, derived from email (before @) | Username |
| `displayName` | `string` | Yes | No | None | User's full display name |
| `phoneNumber` | `string` | No | Yes | `[Phone]` — valid phone format | Phone number |

### Success Response

**Status Code:** `200 OK`

```json
{
  "email": "newpatient@example.com",
  "fullName": "New Patient",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

| Property | Data Type | Description |
|----------|-----------|-------------|
| `email` | `string` | Registered email |
| `fullName` | `string` | Display name |
| `token` | `string` | JWT token — user is logged in immediately after registration |

### Error Responses

| Status | Reason | Example Response | When |
|--------|--------|-----------------|------|
| `400` | Validation/Identity errors | `{"stutsCode":400,"errorMsg":"Validation Failed","errors":["Passwords must have at least one uppercase."]}` | ASP.NET Identity validation fails (duplicate email, weak password, etc.) |

### Business Rules

- **Always creates a `Patient` user** — no role selection available
- `UserType` is set to `"Patient"` and role `"Patient"` is assigned
- If `userName` is null, it is auto-generated from email prefix (before `@`)
- User is logged in immediately — JWT token is returned in the response
- Duplicate email results in a `BadRequestException` with Identity error descriptions

### Workflow

```
Register → Receive JWT → Start using authenticated endpoints
```

### cURL Example

```bash
curl -X POST https://localhost:7292/api/Auth/Register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "newpatient@example.com",
    "password": "StrongP@ss1",
    "displayName": "New Patient",
    "phoneNumber": "+201234567890"
  }'
```

---

## 3.3 Forget Password

### Endpoint Information

| Field | Value |
|-------|-------|
| **Module** | Auth |
| **Endpoint Name** | ForgetPassword |
| **HTTP Method** | `POST` |
| **URL** | `/api/Auth/forget-password` |
| **Description** | Request a password reset link via email |
| **Authentication Required** | No |

### Request Body

```json
{
  "email": "patient1@tabiby.com"
}
```

| Property | Data Type | Required | Nullable | Validation Rules | Description |
|----------|-----------|----------|----------|-----------------|-------------|
| `email` | `string` | Yes | No | `[Required]`, `[EmailAddress]` | The email to send the reset link to |

### Success Response

**Status Code:** `200 OK`

```json
{
  "message": "Reset link sent, please check your inbox."
}
```

### Error Responses

| Status | Reason | Example Response | When |
|--------|--------|-----------------|------|
| `400` | Invalid email / not found | `{"message":"Invalid Email Address"}` | Email doesn't exist in the system |
| `400` | Validation error | ModelState errors | Invalid email format |
| `500` | Email sending failed | `{"message":"Failed to send email"}` | SMTP error |

### Business Rules

- Generates a password reset token via ASP.NET Identity
- Sends a reset link email using Gmail SMTP
- The reset link points to `Auth/ResetPassword` action

---

## 3.4 Reset Password

### Endpoint Information

| Field | Value |
|-------|-------|
| **Module** | Auth |
| **Endpoint Name** | ResetPassword |
| **HTTP Method** | `POST` |
| **URL** | `/api/Auth/reset-password` |
| **Description** | Reset password using token received via email |
| **Authentication Required** | No |

### Request Body

```json
{
  "email": "patient1@tabiby.com",
  "token": "CfDJ8N...encoded-token...",
  "newPassword": "NewStr0ng!",
  "confirmPassword": "NewStr0ng!"
}
```

| Property | Data Type | Required | Nullable | Validation Rules | Description |
|----------|-----------|----------|----------|-----------------|-------------|
| `email` | `string` | Yes | No | `[Required]`, `[EmailAddress]` | User's email |
| `token` | `string` | Yes | No | `[Required]` | Reset token from the email link |
| `newPassword` | `string` | Yes | No | `[Required]`, `[MinLength(6)]` | New password |
| `confirmPassword` | `string` | Yes | No | `[Compare("NewPassword")]` | Must match `newPassword` |

### Success Response

**Status Code:** `200 OK`

```json
{
  "message": "Password reset successfully."
}
```

### Error Responses

| Status | Reason | Example Response | When |
|--------|--------|-----------------|------|
| `400` | Validation / Token errors | `["Invalid token."]` | Expired or invalid token, password policy failure |

---

## Auth Module Summary

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `POST` | `/api/Auth/Login` | No | Login and get JWT |
| `POST` | `/api/Auth/Register` | No | Register as Patient |
| `POST` | `/api/Auth/forget-password` | No | Request password reset email |
| `POST` | `/api/Auth/reset-password` | No | Reset password with token |

---

# 4. Appointment Module

> [!NOTE]
> All Appointment endpoints (except `AvailableSlots`) require authentication. The controller has `[Authorize]` at the class level.

---

## 4.1 Book Appointment

### Endpoint Information

| Field | Value |
|-------|-------|
| **Module** | Appointment |
| **Endpoint Name** | BookAppointment |
| **HTTP Method** | `POST` |
| **URL** | `/api/Appointment/Book` |
| **Description** | Patient books an appointment with a doctor |
| **Authentication Required** | Yes |
| **Authorization Roles** | `Patient` |

### Request

#### Headers

| Header | Value | Required |
|--------|-------|----------|
| Authorization | `Bearer <token>` | Yes |
| Content-Type | `application/json` | Yes |

#### Request Body

```json
{
  "doctorId": 2,
  "scheduleId": 1,
  "appointmentDate": "2026-07-10T00:00:00",
  "notes": "Follow-up checkup"
}
```

| Property | Data Type | Required | Nullable | Validation Rules | Description |
|----------|-----------|----------|----------|-----------------|-------------|
| `doctorId` | `int` | Yes | No | `[Required]` | Doctor's user ID |
| `scheduleId` | `int` | Yes | No | `[Required]` | Doctor's schedule slot ID |
| `appointmentDate` | `DateTime` | Yes | No | `[Required]` | Desired appointment date |
| `notes` | `string` | No | Yes | None | Optional notes for the doctor |

### Success Response

**Status Code:** `200 OK`

```json
{
  "id": 1,
  "patientId": 5,
  "patientName": "Ahmed Hassan",
  "doctorId": 2,
  "doctorName": "Dr. Salma Youssef",
  "scheduleId": 1,
  "appointmentDate": "2026-07-10T00:00:00",
  "appointmentTime": "09:00:00",
  "status": 0,
  "statusText": "Pending",
  "notes": "Follow-up checkup",
  "createdAt": "2026-07-06T19:00:00",
  "updatedAt": null
}
```

| Property | Data Type | Description |
|----------|-----------|-------------|
| `id` | `int` | Appointment ID |
| `patientId` | `int` | Patient's user ID |
| `patientName` | `string` | Patient's full name |
| `doctorId` | `int` | Doctor's user ID |
| `doctorName` | `string` | Doctor's full name |
| `scheduleId` | `int` | Selected schedule slot ID |
| `appointmentDate` | `DateTime` | Appointment date |
| `appointmentTime` | `TimeSpan` | Appointment time (from schedule StartTime) |
| `status` | `int` (enum) | `AppointmentStatus` enum value |
| `statusText` | `string` | Human-readable status |
| `notes` | `string?` | Patient's notes |
| `createdAt` | `DateTime` | Creation timestamp |
| `updatedAt` | `DateTime?` | Last update timestamp |

### Error Responses

| Status | Reason | When |
|--------|--------|------|
| `400` | Validation error | Invalid model state |
| `401` | Unauthorized | Missing or expired JWT |
| `403` | Forbidden | Non-Patient role |
| `404` | Not found | Doctor or schedule not found |

### Business Rules

- Patient ID is extracted from the JWT token (`User.GetUserId()`)
- Initial status is `Pending`
- AppointmentTime is derived from the schedule's StartTime

### cURL Example

```bash
curl -X POST https://localhost:7292/api/Appointment/Book \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{"doctorId":2,"scheduleId":1,"appointmentDate":"2026-07-10T00:00:00","notes":"Follow-up checkup"}'
```

---

## 4.2 Get My Appointments (Patient)

### Endpoint Information

| Field | Value |
|-------|-------|
| **HTTP Method** | `GET` |
| **URL** | `/api/Appointment/MyAppointments` |
| **Description** | Get all appointments for the logged-in patient |
| **Authorization Roles** | `Patient` |

### Request

#### Headers

| Header | Value | Required |
|--------|-------|----------|
| Authorization | `Bearer <token>` | Yes |

### Success Response

**Status Code:** `200 OK`

```json
[
  {
    "id": 1,
    "patientId": 5,
    "patientName": "Ahmed Hassan",
    "doctorId": 2,
    "doctorName": "Dr. Salma Youssef",
    "scheduleId": 1,
    "appointmentDate": "2026-07-10T00:00:00",
    "appointmentTime": "09:00:00",
    "status": 0,
    "statusText": "Pending",
    "notes": "Follow-up checkup",
    "createdAt": "2026-07-06T19:00:00",
    "updatedAt": null
  }
]
```

---

## 4.3 Update Appointment

### Endpoint Information

| Field | Value |
|-------|-------|
| **HTTP Method** | `PUT` |
| **URL** | `/api/Appointment/Update/{appointmentId}` |
| **Description** | Patient updates an existing appointment |
| **Authorization Roles** | `Patient` |

### Path Parameters

| Name | Type | Required | Description |
|------|------|----------|-------------|
| `appointmentId` | `int` | Yes | Appointment ID to update |

### Request Body

```json
{
  "scheduleId": 2,
  "appointmentDate": "2026-07-12T00:00:00",
  "notes": "Updated notes"
}
```

| Property | Data Type | Required | Nullable | Description |
|----------|-----------|----------|----------|-------------|
| `scheduleId` | `int?` | No | Yes | New schedule slot (only applied if provided) |
| `appointmentDate` | `DateTime?` | No | Yes | New date (only applied if provided) |
| `notes` | `string?` | No | Yes | Updated notes (only applied if not null) |

### Business Rules

- Only the patient who owns the appointment can update it
- Uses conditional mapping: only non-null fields are updated
- Returns updated `AppointmentDto`

---

## 4.4 Get Appointment by ID

### Endpoint Information

| Field | Value |
|-------|-------|
| **HTTP Method** | `GET` |
| **URL** | `/api/Appointment/GetAppointment/{appointmentId}` |
| **Description** | Get a specific appointment (Patient or Doctor) |
| **Authorization Roles** | Any authenticated user (Patient or Doctor) |

### Path Parameters

| Name | Type | Required | Description |
|------|------|----------|-------------|
| `appointmentId` | `int` | Yes | Appointment ID |

---

## 4.5 Cancel Appointment

### Endpoint Information

| Field | Value |
|-------|-------|
| **HTTP Method** | `POST` |
| **URL** | `/api/Appointment/Cancel/{appointmentId}` |
| **Description** | Cancel an appointment (Patient or Doctor) |
| **Authorization Roles** | Any authenticated user |

### Path Parameters

| Name | Type | Required | Description |
|------|------|----------|-------------|
| `appointmentId` | `int` | Yes | Appointment ID to cancel |

### Business Rules

- Both patient and doctor can cancel
- Sets status to `Cancelled`

---

## 4.6 Get Doctor Appointments

### Endpoint Information

| Field | Value |
|-------|-------|
| **HTTP Method** | `GET` |
| **URL** | `/api/Appointment/DoctorAppointments` |
| **Description** | Get all appointments for the logged-in doctor |
| **Authorization Roles** | `Doctor` |

---

## 4.7 Confirm Appointment

### Endpoint Information

| Field | Value |
|-------|-------|
| **HTTP Method** | `POST` |
| **URL** | `/api/Appointment/Confirm/{appointmentId}` |
| **Description** | Doctor confirms a pending appointment |
| **Authorization Roles** | `Doctor` |

### Path Parameters

| Name | Type | Required | Description |
|------|------|----------|-------------|
| `appointmentId` | `int` | Yes | Appointment ID |

### Business Rules

- Sets status from `Pending` → `Confirmed`
- Only the assigned doctor can confirm

---

## 4.8 Complete Appointment

### Endpoint Information

| Field | Value |
|-------|-------|
| **HTTP Method** | `POST` |
| **URL** | `/api/Appointment/Complete/{appointmentId}` |
| **Description** | Doctor marks an appointment as completed |
| **Authorization Roles** | `Doctor` |

### Business Rules

- Sets status from `Confirmed` → `Completed`
- Only the assigned doctor can complete

---

## 4.9 Get Available Slots

### Endpoint Information

| Field | Value |
|-------|-------|
| **HTTP Method** | `GET` |
| **URL** | `/api/Appointment/AvailableSlots` |
| **Description** | Get available appointment slots for a doctor on a specific date |
| **Authentication Required** | **No** (`[AllowAnonymous]`) |

### Query Parameters

| Name | Type | Required | Default | Description |
|------|------|----------|---------|-------------|
| `doctorId` | `int` | Yes | — | Doctor's user ID |
| `date` | `DateTime` | Yes | — | Date to check availability |

### Success Response

**Status Code:** `200 OK`

```json
[
  {
    "scheduleId": 1,
    "dayOfWeek": 1,
    "startTime": "09:00:00",
    "endTime": "12:00:00",
    "isAvailable": true,
    "availableDates": ["2026-07-10T00:00:00"]
  }
]
```

### cURL Example

```bash
curl "https://localhost:7292/api/Appointment/AvailableSlots?doctorId=2&date=2026-07-10"
```

---

## Appointment Module Summary

| Method | Endpoint | Auth | Roles | Description |
|--------|----------|------|-------|-------------|
| `POST` | `/api/Appointment/Book` | Yes | Patient | Book appointment |
| `GET` | `/api/Appointment/MyAppointments` | Yes | Patient | Get patient's appointments |
| `PUT` | `/api/Appointment/Update/{id}` | Yes | Patient | Update appointment |
| `GET` | `/api/Appointment/GetAppointment/{id}` | Yes | Any | Get single appointment |
| `POST` | `/api/Appointment/Cancel/{id}` | Yes | Any | Cancel appointment |
| `GET` | `/api/Appointment/DoctorAppointments` | Yes | Doctor | Get doctor's appointments |
| `POST` | `/api/Appointment/Confirm/{id}` | Yes | Doctor | Confirm appointment |
| `POST` | `/api/Appointment/Complete/{id}` | Yes | Doctor | Complete appointment |
| `GET` | `/api/Appointment/AvailableSlots` | **No** | — | Get available slots |

---

# 5. Doctor Schedule Module

> [!NOTE]
> All endpoints require the `Doctor` role.

---

## 5.1 Create Schedule

### Endpoint Information

| Field | Value |
|-------|-------|
| **HTTP Method** | `POST` |
| **URL** | `/api/DoctorSchedule` |
| **Authorization Roles** | `Doctor` |

### Request Body

```json
{
  "dayOfWeek": 1,
  "startTime": "09:00:00",
  "endTime": "12:00:00",
  "isAvailable": true
}
```

| Property | Data Type | Required | Nullable | Validation | Description |
|----------|-----------|----------|----------|------------|-------------|
| `dayOfWeek` | `DayOfWeek` (int 0-6) | Yes | No | `[Required]` | Day of week (0=Sunday, 1=Monday, ..., 6=Saturday) |
| `startTime` | `TimeSpan` | Yes | No | `[Required]` | Slot start time (e.g. `"09:00:00"`) |
| `endTime` | `TimeSpan` | Yes | No | `[Required]` | Slot end time (e.g. `"12:00:00"`) |
| `isAvailable` | `bool` | No | No | Default: `true` | Whether the slot is available |

### Success Response

**Status Code:** `200 OK`

```json
{
  "id": 1,
  "doctorId": 2,
  "dayOfWeek": 1,
  "startTime": "09:00:00",
  "endTime": "12:00:00",
  "isAvailable": true,
  "createdAt": "2026-07-06T19:00:00",
  "updatedAt": null
}
```

---

## 5.2 Get Doctor Schedules

| Field | Value |
|-------|-------|
| **HTTP Method** | `GET` |
| **URL** | `/api/DoctorSchedule/Doctor` |
| **Authorization Roles** | `Doctor` |

Returns an array of `DoctorScheduleDto`.

---

## 5.3 Update Schedule

| Field | Value |
|-------|-------|
| **HTTP Method** | `PUT` |
| **URL** | `/api/DoctorSchedule/{scheduleId}` |
| **Authorization Roles** | `Doctor` |

### Request Body

```json
{
  "id": 1,
  "dayOfWeek": 2,
  "startTime": "10:00:00",
  "endTime": "14:00:00",
  "isAvailable": true
}
```

---

## 5.4 Delete Schedule

| Field | Value |
|-------|-------|
| **HTTP Method** | `DELETE` |
| **URL** | `/api/DoctorSchedule/{scheduleId}` |
| **Authorization Roles** | `Doctor` |

**Success:** `204 No Content`
**Error:** `404 Not Found` — "Schedule not found or you are not authorized to delete it."

---

## Doctor Schedule Module Summary

| Method | Endpoint | Auth | Roles | Description |
|--------|----------|------|-------|-------------|
| `POST` | `/api/DoctorSchedule` | Yes | Doctor | Create schedule |
| `GET` | `/api/DoctorSchedule/Doctor` | Yes | Doctor | Get my schedules |
| `PUT` | `/api/DoctorSchedule/{id}` | Yes | Doctor | Update schedule |
| `DELETE` | `/api/DoctorSchedule/{id}` | Yes | Doctor | Delete schedule |

---

# 6. Consultation Module

---

## 6.1 Get All Doctors (Search)

### Endpoint Information

| Field | Value |
|-------|-------|
| **HTTP Method** | `GET` |
| **URL** | `/api/Consultation/GetAllDoctors` |
| **Description** | Search and browse available doctors |
| **Authentication Required** | **No** |

### Query Parameters

| Name | Type | Required | Default | Description |
|------|------|----------|---------|-------------|
| `name` | `string?` | No | `null` | Filter by doctor name |
| `specialization` | `string?` | No | `null` | Filter by specialty |
| `location` | `string?` | No | `null` | Filter by location |
| `pageNumber` | `int` | No | `1` | Page number |
| `pageSize` | `int` | No | `10` | Items per page |

### Success Response

**Status Code:** `200 OK`

```json
[
  {
    "id": 2,
    "name": "Dr. Salma Youssef",
    "phone": "+201000000001",
    "specialty": "Cardiology",
    "location": "Cairo"
  }
]
```

### cURL Example

```bash
curl "https://localhost:7292/api/Consultation/GetAllDoctors?specialization=Cardiology&pageNumber=1&pageSize=10"
```

---

## 6.2 Request Consultation

### Endpoint Information

| Field | Value |
|-------|-------|
| **HTTP Method** | `POST` |
| **URL** | `/api/Consultation/RequestConsultation` |
| **Authorization Roles** | `Patient` |

### Request Body

```json
{
  "doctorId": 2
}
```

| Property | Data Type | Required | Description |
|----------|-----------|----------|-------------|
| `doctorId` | `int` | Yes | The doctor to consult with |

### Success Response

**Status Code:** `200 OK`

```json
{
  "id": 1,
  "patientId": 5,
  "doctorId": 2,
  "status": "Pending",
  "requestedAt": "2026-07-06T19:00:00",
  "createdAt": "2026-07-06T19:00:00",
  "review": null
}
```

---

## 6.3 My Consultations

| Field | Value |
|-------|-------|
| **HTTP Method** | `GET` |
| **URL** | `/api/Consultation/MyConsultations` |
| **Authorization Roles** | `Patient`, `Doctor` |

Returns all consultations for the current user.

---

## 6.4 Get My Consultation By ID

| Field | Value |
|-------|-------|
| **HTTP Method** | `GET` |
| **URL** | `/api/Consultation/GetMyConsultationById/{id}` |
| **Authorization Roles** | `Patient`, `Doctor` |

---

## 6.5 Delete Consultation

| Field | Value |
|-------|-------|
| **HTTP Method** | `DELETE` |
| **URL** | `/api/Consultation/DeleteConsultation/{id}` |
| **Authorization Roles** | `Doctor` |

**Success:** `204 No Content`

---

## 6.6 Update Consultation Status

### Endpoint Information

| Field | Value |
|-------|-------|
| **HTTP Method** | `PUT` |
| **URL** | `/api/Consultation/UpdateConsultationStatus/{id}` |
| **Authorization Roles** | `Patient`, `Doctor` |

### Request Body

```json
{
  "status": "Accepted"
}
```

| Property | Data Type | Required | Description |
|----------|-----------|----------|-------------|
| `status` | `string` | Yes | New status: `"Pending"`, `"Accepted"`, `"Completed"`, `"Rejected"` |

---

## Consultation Module Summary

| Method | Endpoint | Auth | Roles | Description |
|--------|----------|------|-------|-------------|
| `GET` | `/api/Consultation/GetAllDoctors` | No | — | Search doctors |
| `POST` | `/api/Consultation/RequestConsultation` | Yes | Patient | Request consultation |
| `GET` | `/api/Consultation/MyConsultations` | Yes | Patient, Doctor | Get my consultations |
| `GET` | `/api/Consultation/GetMyConsultationById/{id}` | Yes | Patient, Doctor | Get consultation by ID |
| `DELETE` | `/api/Consultation/DeleteConsultation/{id}` | Yes | Doctor | Delete consultation |
| `PUT` | `/api/Consultation/UpdateConsultationStatus/{id}` | Yes | Patient, Doctor | Update status |

---

# 7. Consultation Chat Module

> [!NOTE]
> All endpoints require `Patient` or `Doctor` role.

---

## 7.1 Get Messages

| Field | Value |
|-------|-------|
| **HTTP Method** | `GET` |
| **URL** | `/api/ConsultationChat/{consultationId}/messages` |
| **Authorization Roles** | `Patient`, `Doctor` |

### Success Response

```json
[
  {
    "id": 1,
    "consultationId": 1,
    "senderUserId": 5,
    "senderName": "Ahmed Hassan",
    "content": "Hello doctor, I have a question.",
    "isRead": false,
    "createdAt": "2026-07-06T19:05:00"
  }
]
```

---

## 7.2 Send Message

| Field | Value |
|-------|-------|
| **HTTP Method** | `POST` |
| **URL** | `/api/ConsultationChat/{consultationId}/messages` |
| **Authorization Roles** | `Patient`, `Doctor` |

### Request Body

```json
{
  "content": "Hello doctor, I have a question."
}
```

| Property | Data Type | Required | Description |
|----------|-----------|----------|-------------|
| `content` | `string` | Yes | Message text |

---

## 7.3 Mark Messages as Read

| Field | Value |
|-------|-------|
| **HTTP Method** | `POST` |
| **URL** | `/api/ConsultationChat/{consultationId}/read` |

**Success:** `204 No Content`

---

## 7.4 Get Unread Count

| Field | Value |
|-------|-------|
| **HTTP Method** | `GET` |
| **URL** | `/api/ConsultationChat/{consultationId}/unread-count` |

Returns: `int` (number of unread messages)

---

## Chat Module Summary

| Method | Endpoint | Auth | Roles | Description |
|--------|----------|------|-------|-------------|
| `GET` | `/api/ConsultationChat/{id}/messages` | Yes | Patient, Doctor | Get chat messages |
| `POST` | `/api/ConsultationChat/{id}/messages` | Yes | Patient, Doctor | Send message |
| `POST` | `/api/ConsultationChat/{id}/read` | Yes | Patient, Doctor | Mark as read |
| `GET` | `/api/ConsultationChat/{id}/unread-count` | Yes | Patient, Doctor | Unread count |

---

# 8. Consultation Review Module

> [!NOTE]
> All endpoints require authentication. The controller has `[Authorize]` at the class level.

---

## 8.1 Add Review

| Field | Value |
|-------|-------|
| **HTTP Method** | `POST` |
| **URL** | `/api/ConsultationReview/{consultationId}` |
| **Authorization Roles** | `Patient` |

### Request Body

```json
{
  "rating": 5,
  "comment": "Excellent consultation, very helpful!"
}
```

| Property | Data Type | Required | Description |
|----------|-----------|----------|-------------|
| `rating` | `int` | Yes | Rating score |
| `comment` | `string` | Yes | Review text |

### Success Response

```json
{
  "id": 1,
  "consultationId": 1,
  "rating": 5,
  "comment": "Excellent consultation, very helpful!",
  "createdAt": "2026-07-06T20:00:00"
}
```

---

## 8.2 Get Review

| Field | Value |
|-------|-------|
| **HTTP Method** | `GET` |
| **URL** | `/api/ConsultationReview/{consultationId}` |
| **Authorization Roles** | Any authenticated user |

**404 Error:** `"Review not found for this consultation."`

---

## Review Module Summary

| Method | Endpoint | Auth | Roles | Description |
|--------|----------|------|-------|-------------|
| `POST` | `/api/ConsultationReview/{id}` | Yes | Patient | Add review |
| `GET` | `/api/ConsultationReview/{id}` | Yes | Any | Get review |

---

# 9. Medication Module

---

## 9.1 Get All Medications

### Endpoint Information

| Field | Value |
|-------|-------|
| **HTTP Method** | `GET` |
| **URL** | `/api/Medication/GetAll` |
| **Authentication Required** | **No** |

### Query Parameters

| Name | Type | Required | Default | Description |
|------|------|----------|---------|-------------|
| `SearchName` | `string?` | No | `null` | Filter medications by name |

### Success Response

**Status Code:** `200 OK`

```json
[
  {
    "id": 1,
    "createdAt": "2026-07-01T00:00:00",
    "name": "Paracetamol 500mg",
    "price": 25.50,
    "stock": 100,
    "is_available": true
  }
]
```

### cURL Example

```bash
curl "https://localhost:7292/api/Medication/GetAll?SearchName=Paracetamol"
```

---

## 9.2 Get Medication by ID

| Field | Value |
|-------|-------|
| **HTTP Method** | `GET` |
| **URL** | `/api/Medication/GetById/{id}` |
| **Authentication Required** | **No** |

### Success Response

```json
{
  "id": 1,
  "name": "Paracetamol 500mg",
  "price": 25.50,
  "stock": 100,
  "isAvailable": true,
  "pictureUrl": "/files/medications/guid_image.jpg"
}
```

---

## 9.3 Create Medication

### Endpoint Information

| Field | Value |
|-------|-------|
| **HTTP Method** | `POST` |
| **URL** | `/api/Medication/CreateMedication` |
| **Authorization Roles** | `Pharmacist`, `Admin` |
| **Content-Type** | `multipart/form-data` |

### Request Body (Form Data)

| Property | Data Type | Required | Description |
|----------|-----------|----------|-------------|
| `name` | `string` | Yes | Medication name |
| `price` | `decimal` | Yes | Price |
| `image` | `IFormFile` | No | Product image |
| `stock` | `int` | Yes | Stock quantity |
| `isAvailable` | `bool` | Yes | Availability flag |

### cURL Example

```bash
curl -X POST https://localhost:7292/api/Medication/CreateMedication \
  -H "Authorization: Bearer <token>" \
  -F "name=Ibuprofen 400mg" \
  -F "price=35.00" \
  -F "stock=50" \
  -F "isAvailable=true" \
  -F "image=@/path/to/image.jpg"
```

---

## 9.4 Update Medication

| Field | Value |
|-------|-------|
| **HTTP Method** | `POST` |
| **URL** | `/api/Medication/UpdateMedication/{Id}` |
| **Authorization Roles** | `Pharmacist`, `Admin` |

### Request Body

```json
{
  "name": "Paracetamol 1000mg",
  "price": 30.00,
  "stock": 80,
  "isAvailable": true
}
```

---

## 9.5 Delete Medication

| Field | Value |
|-------|-------|
| **HTTP Method** | `DELETE` |
| **URL** | `/api/Medication/DeleteMedication/{id}` |
| **Authorization Roles** | `Pharmacist`, `Admin` |

**Success:** `204 No Content`

---

## Medication Module Summary

| Method | Endpoint | Auth | Roles | Description |
|--------|----------|------|-------|-------------|
| `GET` | `/api/Medication/GetAll` | No | — | Get all medications |
| `GET` | `/api/Medication/GetById/{id}` | No | — | Get by ID |
| `POST` | `/api/Medication/CreateMedication` | Yes | Pharmacist, Admin | Create (multipart/form-data) |
| `POST` | `/api/Medication/UpdateMedication/{id}` | Yes | Pharmacist, Admin | Update |
| `DELETE` | `/api/Medication/DeleteMedication/{id}` | Yes | Pharmacist, Admin | Delete |

---

# 10. Basket Module

> [!NOTE]
> All endpoints require the `Patient` role.

---

## 10.1 Get Basket

| Field | Value |
|-------|-------|
| **HTTP Method** | `GET` |
| **URL** | `/api/Basket` |
| **Authorization Roles** | `Patient` |

### Success Response

```json
{
  "id": 1,
  "items": [
    {
      "medicationId": 1,
      "productName": "Paracetamol 500mg",
      "pictureUrl": "/files/medications/guid_image.jpg",
      "price": 25.50,
      "quantity": 2,
      "subTotal": 51.00
    }
  ],
  "subTotal": 51.00,
  "shippingPrice": 20.00,
  "total": 71.00
}
```

---

## 10.2 Add Item to Basket

| Field | Value |
|-------|-------|
| **HTTP Method** | `POST` |
| **URL** | `/api/Basket/AddItem` |
| **Authorization Roles** | `Patient` |

### Request Body

```json
{
  "medicationId": 1,
  "quantity": 2
}
```

| Property | Data Type | Required | Validation | Description |
|----------|-----------|----------|------------|-------------|
| `medicationId` | `int` | Yes | `[Required]` | Medication ID |
| `quantity` | `int` | Yes | `[Range(1, 100)]` | Quantity (1–100) |

---

## 10.3 Update Item Quantity

| Field | Value |
|-------|-------|
| **HTTP Method** | `PUT` |
| **URL** | `/api/Basket/UpdateItem/{medicationId}` |
| **Authorization Roles** | `Patient` |

### Request Body

Raw integer value:

```json
3
```

---

## 10.4 Remove Item

| Field | Value |
|-------|-------|
| **HTTP Method** | `DELETE` |
| **URL** | `/api/Basket/RemoveItem/{medicationId}` |
| **Authorization Roles** | `Patient` |

**Success:** `204 No Content`

---

## 10.5 Clear Basket

| Field | Value |
|-------|-------|
| **HTTP Method** | `DELETE` |
| **URL** | `/api/Basket/Clear` |
| **Authorization Roles** | `Patient` |

**Success:** `204 No Content`

---

## Basket Module Summary

| Method | Endpoint | Auth | Roles | Description |
|--------|----------|------|-------|-------------|
| `GET` | `/api/Basket` | Yes | Patient | Get basket |
| `POST` | `/api/Basket/AddItem` | Yes | Patient | Add item |
| `PUT` | `/api/Basket/UpdateItem/{medicationId}` | Yes | Patient | Update quantity |
| `DELETE` | `/api/Basket/RemoveItem/{medicationId}` | Yes | Patient | Remove item |
| `DELETE` | `/api/Basket/Clear` | Yes | Patient | Clear basket |

---

# 11. Order Module

---

## 11.1 Create Order

### Endpoint Information

| Field | Value |
|-------|-------|
| **HTTP Method** | `POST` |
| **URL** | `/api/Order/Create` |
| **Authorization Roles** | `Patient` |

### Request Body

```json
{
  "address": {
    "firstName": "Ahmed",
    "lastName": "Hassan",
    "city": "Cairo",
    "country": "Egypt",
    "street": "123 Main St"
  }
}
```

| Property | Data Type | Required | Description |
|----------|-----------|----------|-------------|
| `address.firstName` | `string` | Yes | Recipient first name |
| `address.lastName` | `string` | Yes | Recipient last name |
| `address.city` | `string` | Yes | Delivery city |
| `address.country` | `string` | Yes | Delivery country |
| `address.street` | `string` | Yes | Delivery street address |

### Success Response

```json
{
  "id": 1,
  "status": "Pending",
  "total": 71.00,
  "orderDate": "2026-07-06T19:30:00",
  "items": [
    {
      "id": 1,
      "medicationId": 1,
      "medicationName": "Paracetamol 500mg",
      "pictureUrl": "/files/medications/guid_image.jpg",
      "quantity": 2,
      "unitPrice": 25.50,
      "subtotal": 51.00
    }
  ]
}
```

### Business Rules

- Creates order from the patient's current basket
- Default shipping price is **20.00 EGP**
- Total = SubTotal + ShippingPrice
- Initial status is `Pending`

---

## 11.2 Get My Orders

| Field | Value |
|-------|-------|
| **HTTP Method** | `GET` |
| **URL** | `/api/Order/MyOrders` |
| **Authorization Roles** | `Patient` |

---

## 11.3 Get My Order by ID

| Field | Value |
|-------|-------|
| **HTTP Method** | `GET` |
| **URL** | `/api/Order/GetMyOrder/{orderId}` |
| **Authorization Roles** | `Patient` |

---

## 11.4 Cancel Order

| Field | Value |
|-------|-------|
| **HTTP Method** | `POST` |
| **URL** | `/api/Order/Cancel/{orderId}` |
| **Authorization Roles** | `Patient` |

---

## 11.5 Delete Order

| Field | Value |
|-------|-------|
| **HTTP Method** | `DELETE` |
| **URL** | `/api/Order/Delete` |
| **Authorization Roles** | `Patient` |

### Query Parameters

| Name | Type | Required | Description |
|------|------|----------|-------------|
| `OrderId` | `int` | Yes | Order ID to delete |

**Success:** `204 No Content`

---

## 11.6 Get All Orders (Pharmacist)

| Field | Value |
|-------|-------|
| **HTTP Method** | `GET` |
| **URL** | `/api/Order/Orders` |
| **Authorization Roles** | `Pharmacist`, `Admin` |

---

## 11.7 Get Order by ID (Pharmacist)

| Field | Value |
|-------|-------|
| **HTTP Method** | `GET` |
| **URL** | `/api/Order/GetOrder/{id}` |
| **Authorization Roles** | `Pharmacist`, `Admin` |

---

## 11.8 Update Order Status

### Endpoint Information

| Field | Value |
|-------|-------|
| **HTTP Method** | `PUT` |
| **URL** | `/api/Order/Orders/{orderId}/Status` |
| **Authorization Roles** | `Pharmacist`, `Admin` |

### Request Body

```json
{
  "status": 1
}
```

| Property | Data Type | Required | Description |
|----------|-----------|----------|-------------|
| `status` | `int` (OrderStatus enum) | Yes | `0`=Pending, `1`=Processing, `2`=Shipped, `3`=Delivered, `4`=Cancelled, `5`=Rejected |

---

## Order Module Summary

| Method | Endpoint | Auth | Roles | Description |
|--------|----------|------|-------|-------------|
| `POST` | `/api/Order/Create` | Yes | Patient | Create order from basket |
| `GET` | `/api/Order/MyOrders` | Yes | Patient | Get my orders |
| `GET` | `/api/Order/GetMyOrder/{id}` | Yes | Patient | Get my order by ID |
| `POST` | `/api/Order/Cancel/{id}` | Yes | Patient | Cancel order |
| `DELETE` | `/api/Order/Delete` | Yes | Patient | Delete order |
| `GET` | `/api/Order/Orders` | Yes | Pharmacist, Admin | Get all orders |
| `GET` | `/api/Order/GetOrder/{id}` | Yes | Pharmacist, Admin | Get order by ID |
| `PUT` | `/api/Order/Orders/{id}/Status` | Yes | Pharmacist, Admin | Update order status |

---

# 12. Nursing Module

---

## 12.1 Search Nurses

### Endpoint Information

| Field | Value |
|-------|-------|
| **HTTP Method** | `GET` |
| **URL** | `/api/Nursing/Search` |
| **Authentication Required** | **No** (`[AllowAnonymous]`) |

### Query Parameters

| Name | Type | Required | Default | Description |
|------|------|----------|---------|-------------|
| `name` | `string?` | No | `null` | Filter by nurse name |
| `specialization` | `string?` | No | `null` | Filter by specialization |
| `pageNumber` | `int` | No | `1` | Page number |
| `pageSize` | `int` | No | `10` | Items per page |

### Success Response

```json
[
  {
    "id": 4,
    "name": "Fatma Ali",
    "phone": "+201000000003",
    "specialization": "Home Care"
  }
]
```

---

## 12.2 Request Nursing

| Field | Value |
|-------|-------|
| **HTTP Method** | `POST` |
| **URL** | `/api/Nursing/Request` |
| **Authorization Roles** | `Patient` |

### Request Body

```json
{
  "patientId": 5,
  "nurseId": 4,
  "careType": "Home Care"
}
```

| Property | Data Type | Required | Description |
|----------|-----------|----------|-------------|
| `patientId` | `int` | Yes | Patient ID |
| `nurseId` | `int` | Yes | Nurse ID |
| `careType` | `string` | Yes | Type of care needed |

### Success Response

```json
{
  "patientId": 5,
  "nurseId": 4,
  "careType": "Home Care",
  "status": "Pending",
  "review": null
}
```

---

## 12.3 Get My Nursing Requests

| Field | Value |
|-------|-------|
| **HTTP Method** | `GET` |
| **URL** | `/api/Nursing/MyRequests` |
| **Authorization Roles** | `Patient`, `Nurse` |

---

## 12.4 Update Nursing Status

| Field | Value |
|-------|-------|
| **HTTP Method** | `PUT` |
| **URL** | `/api/Nursing/UpdateStatus/{requestId}` |
| **Authorization Roles** | `Patient`, `Nurse`, `Admin` |

### Request Body

```json
{
  "status": "Accepted"
}
```

---

## 12.5 Cancel Nursing Request

| Field | Value |
|-------|-------|
| **HTTP Method** | `POST` |
| **URL** | `/api/Nursing/Cancel/{requestId}` |
| **Authorization Roles** | `Patient`, `Nurse` |

---

## 12.6 Add Nursing Review

| Field | Value |
|-------|-------|
| **HTTP Method** | `POST` |
| **URL** | `/api/Nursing/Review/{requestId}` |
| **Authorization Roles** | `Patient` |

### Request Body

```json
{
  "rating": 4,
  "comment": "Very professional nurse."
}
```

| Property | Data Type | Required | Description |
|----------|-----------|----------|-------------|
| `rating` | `int` | Yes | Rating score |
| `comment` | `string` | Yes | Review comment |

---

## 12.7 Get Nursing Review

| Field | Value |
|-------|-------|
| **HTTP Method** | `GET` |
| **URL** | `/api/Nursing/Review/{requestId}` |
| **Authorization Roles** | Any authenticated user |

**404 Error:** `"Review not found for this request."`

---

## Nursing Module Summary

| Method | Endpoint | Auth | Roles | Description |
|--------|----------|------|-------|-------------|
| `GET` | `/api/Nursing/Search` | **No** | — | Search nurses |
| `POST` | `/api/Nursing/Request` | Yes | Patient | Request nursing |
| `GET` | `/api/Nursing/MyRequests` | Yes | Patient, Nurse | Get my requests |
| `PUT` | `/api/Nursing/UpdateStatus/{id}` | Yes | Patient, Nurse, Admin | Update status |
| `POST` | `/api/Nursing/Cancel/{id}` | Yes | Patient, Nurse | Cancel request |
| `POST` | `/api/Nursing/Review/{id}` | Yes | Patient | Add review |
| `GET` | `/api/Nursing/Review/{id}` | Yes | Any | Get review |

---

# 13. Notification Module

> [!NOTE]
> The `NotificationController` has **no** `[Authorize]` attribute at the class level, but uses `User.GetUserId()` which throws `UnauthorizedAccessException` if no JWT is present. **Authentication is effectively required**.

---

## 13.1 Get Notifications

| Field | Value |
|-------|-------|
| **HTTP Method** | `GET` |
| **URL** | `/api/Notification/GetNotifications` |

Returns: `string[]` (array of notification messages)

```json
[
  "Your appointment has been confirmed.",
  "New consultation request from Ahmed."
]
```

---

## 13.2 Delete Notification

| Field | Value |
|-------|-------|
| **HTTP Method** | `DELETE` |
| **URL** | `/api/Notification/DeleteNotification` |

### Query Parameters

| Name | Type | Required | Description |
|------|------|----------|-------------|
| `notificationId` | `int` | Yes | Notification ID |

**Success:** `204 No Content`

---

## 13.3 Get Unread Count

| Field | Value |
|-------|-------|
| **HTTP Method** | `GET` |
| **URL** | `/api/Notification/GetUnreadCount` |

Returns: `int`

---

## 13.4 Mark as Read

| Field | Value |
|-------|-------|
| **HTTP Method** | `POST` |
| **URL** | `/api/Notification/MarkAsRead` |

### Query Parameters

| Name | Type | Required | Description |
|------|------|----------|-------------|
| `notificationId` | `int` | Yes | Notification ID |

**Success:** `204 No Content`

---

## Notification Module Summary

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `GET` | `/api/Notification/GetNotifications` | Yes* | Get user's notifications |
| `DELETE` | `/api/Notification/DeleteNotification` | Yes* | Delete notification |
| `GET` | `/api/Notification/GetUnreadCount` | Yes* | Get unread count |
| `POST` | `/api/Notification/MarkAsRead` | Yes* | Mark as read |

*Authentication required implicitly via `User.GetUserId()`

---

# 14. Enum Documentation

## AppointmentStatus

| Value | Name | Description |
|-------|------|-------------|
| `0` | `Pending` | Appointment booked, waiting for doctor confirmation |
| `1` | `Confirmed` | Doctor has confirmed the appointment |
| `2` | `Cancelled` | Appointment cancelled by patient or doctor |
| `3` | `Completed` | Appointment visit has been completed |

## ConsultationStatus

| Value | Name | Description |
|-------|------|-------------|
| `0` | `Pending` | Consultation requested, waiting for doctor response |
| `1` | `Accepted` | Doctor accepted the consultation |
| `2` | `Completed` | Consultation session completed |
| `3` | `Rejected` | Doctor rejected the consultation request |

## OrderStatus

| Value | Name | Description |
|-------|------|-------------|
| `0` | `Pending` | Order placed, not yet processed |
| `1` | `Processing` | Pharmacist is preparing the order |
| `2` | `Shipped` | Order shipped to delivery |
| `3` | `Delivered` | Order delivered to patient |
| `4` | `Cancelled` | Order cancelled |
| `5` | `Rejected` | Order rejected by pharmacist |

## NotificationType

| Value | Name | Description |
|-------|------|-------------|
| `1` | `AppointmentReminder` | Appointment-related notification |
| `2` | `OrderUpdate` | Order status change notification |
| `3` | `ConsultationUpdate` | Consultation status change |
| `4` | `ConsultationRequest` | New consultation request |
| `5` | `ConsultationReview` | New consultation review |
| `6` | `Message` | New chat message |
| `7` | `System` | System notification |

## DayOfWeek (System.DayOfWeek)

| Value | Name |
|-------|------|
| `0` | Sunday |
| `1` | Monday |
| `2` | Tuesday |
| `3` | Wednesday |
| `4` | Thursday |
| `5` | Friday |
| `6` | Saturday |

---

# 15. DTO Documentation

## Request DTOs

| DTO | Module | Fields |
|-----|--------|--------|
| `LoginDto` | Auth | `email`, `password` |
| `RegisterDto` | Auth | `email`, `password`, `userName?`, `displayName`, `phoneNumber?` |
| `ForgetPasswordDto` | Auth | `email` |
| `ResetPasswordDto` | Auth | `email`, `token`, `newPassword`, `confirmPassword` |
| `CreateAppointmentDto` | Appointment | `doctorId`, `scheduleId`, `appointmentDate`, `notes?` |
| `UpdateAppointmentDto` | Appointment | `scheduleId?`, `appointmentDate?`, `notes?` |
| `CreateDoctorScheduleDto` | Schedule | `dayOfWeek`, `startTime`, `endTime`, `isAvailable` |
| `UpdateDoctorScheduleDto` | Schedule | `id`, `dayOfWeek`, `startTime`, `endTime`, `isAvailable` |
| `CreateConsultationDto` | Consultation | `doctorId` |
| `UpdateConsultionStatusDto` | Consultation | `status` |
| `SendMessageDto` | Chat | `content` |
| `CreateConsultationReviewDto` | Review | `rating`, `comment` |
| `CreateMedicationDto` | Medication | `name`, `price`, `image?`, `stock`, `isAvailable` |
| `UpdateMedicationModel` | Medication | `name`, `price`, `stock`, `isAvailable` |
| `BasketItemInputDto` | Basket | `medicationId`, `quantity` |
| `CreateOrderDto` | Order | `address` (nested `OrderAddressDto`) |
| `UpdateOrderStatus` | Order | `status` (enum) |
| `CreateNursingRequestDto` | Nursing | `patientId`, `nurseId`, `careType` |
| `UpdateNursingStatusDto` | Nursing | `status` |
| `CreateNursingReviewDto` | Nursing | `rating`, `comment` |
| `SearchDoctorDto` | Consultation | `name?`, `specialization?`, `location?`, `pageNumber`, `pageSize` |
| `SearchNurseDto` | Nursing | `name?`, `specialization?`, `pageNumber`, `pageSize` |

## Response DTOs

| DTO | Module | Fields |
|-----|--------|--------|
| `UserDto` | Auth | `email`, `token`, `fullName` |
| `AppointmentDto` | Appointment | `id`, `patientId`, `patientName`, `doctorId`, `doctorName`, `scheduleId`, `appointmentDate`, `appointmentTime`, `status`, `statusText`, `notes?`, `createdAt`, `updatedAt?` |
| `AvailableDoctorSlotDto` | Appointment | `scheduleId`, `dayOfWeek`, `startTime`, `endTime`, `isAvailable`, `availableDates[]` |
| `DoctorScheduleDto` | Schedule | `id`, `doctorId`, `dayOfWeek`, `startTime`, `endTime`, `isAvailable`, `createdAt`, `updatedAt?` |
| `ConsultationDto` | Consultation | `id`, `patientId`, `doctorId`, `status`, `requestedAt`, `createdAt`, `review?` |
| `ConsultationMessageDto` | Chat | `id`, `consultationId`, `senderUserId`, `senderName`, `content`, `isRead`, `createdAt` |
| `ConsultationReviewDto` | Review | `id`, `consultationId`, `rating`, `comment`, `createdAt` |
| `DoctorInfoDto` | Doctor | `id`, `name`, `phone`, `specialty`, `location` |
| `NurseInfoDto` | Nurse | `id`, `name`, `phone`, `specialization` |
| `MedicationDto` | Medication | `id`, `name`, `price`, `stock`, `isAvailable`, `pictureUrl?` |
| `AllMedicationDto` | Medication | `id`, `createdAt`, `name`, `price`, `stock`, `is_available` |
| `BasketDto` | Basket | `id`, `items[]`, `subTotal`, `shippingPrice`, `total` |
| `BasketItemDto` | Basket | `medicationId`, `productName`, `pictureUrl`, `price`, `quantity`, `subTotal` |
| `OrderDto` | Order | `id`, `status`, `total`, `orderDate`, `items[]` |
| `OrderItemDto` | Order | `id`, `medicationId`, `medicationName`, `pictureUrl`, `quantity`, `unitPrice`, `subtotal` |
| `NursingRequestDto` | Nursing | `patientId`, `nurseId`, `careType`, `status`, `review?` |
| `NursingReviewDto` | Nursing | `nursingRequestId`, `rating`, `comment` |
| `NotificationDto` | Notification | `id`, `message`, `isRead`, `createdAt` |

---

# 16. Validation Documentation

## DataAnnotation Validations

| DTO | Property | Rule |
|-----|----------|------|
| `LoginDto` | `email` | `[EmailAddress]` |
| `RegisterDto` | `phoneNumber` | `[Phone]` |
| `ForgetPasswordDto` | `email` | `[Required]`, `[EmailAddress]` |
| `ResetPasswordDto` | `email` | `[Required]`, `[EmailAddress]` |
| `ResetPasswordDto` | `token` | `[Required]` |
| `ResetPasswordDto` | `newPassword` | `[Required]`, `[MinLength(6)]` |
| `ResetPasswordDto` | `confirmPassword` | `[Compare("NewPassword")]` |
| `CreateAppointmentDto` | `doctorId` | `[Required]` |
| `CreateAppointmentDto` | `scheduleId` | `[Required]` |
| `CreateAppointmentDto` | `appointmentDate` | `[Required]` |
| `CreateDoctorScheduleDto` | `dayOfWeek` | `[Required]` |
| `CreateDoctorScheduleDto` | `startTime` | `[Required]` |
| `CreateDoctorScheduleDto` | `endTime` | `[Required]` |
| `BasketItemInputDto` | `medicationId` | `[Required]` |
| `BasketItemInputDto` | `quantity` | `[Range(1, 100)]` |

## ASP.NET Identity Password Validations (Defaults)

| Rule | Default |
|------|---------|
| Require uppercase | Yes |
| Require lowercase | Yes |
| Require digit | Yes |
| Require non-alphanumeric | Yes |
| Minimum length | 6 characters |

> [!NOTE]
> **FluentValidation is NOT used** in this project. All validation is done via DataAnnotations and ASP.NET Identity defaults.

---

# 17. AutoMapper Documentation

## Mapping Profiles

| Source Entity | Destination DTO | Special Mappings |
|---------------|----------------|-----------------|
| `Appointment` → | `AppointmentDto` | `DoctorName` ← `Doctor.Fullname`, `PatientName` ← `Patient.Fullname`, `StatusText` ← `Status.ToString()` |
| `CreateAppointmentDto` → | `Appointment` | `AppointmentTime` ignored |
| `UpdateAppointmentDto` → | `Appointment` | Conditional: only maps non-null fields |
| `DoctorSchedule` ↔ | `DoctorScheduleDto` | ReverseMap |
| `DoctorSchedule` ↔ | `CreateDoctorScheduleDto` | ReverseMap |
| `DoctorSchedule` → | `UpdateDoctorScheduleDto` | ReverseMap, `Id` ignored on reverse |
| `DoctorSchedule` → | `AvailableDoctorSlotDto` | `ScheduleId` ← `Id`, `IsAvailable` = true |
| `Medication` ↔ | `MedicationDto` | ReverseMap |
| `Medication` ↔ | `CreateMedicationDto` | `Image` ignored |
| `Medication` ↔ | `UpdateMedicationDto` | `Image` ignored |
| `Medication` ↔ | `AllMedicationDto` | ReverseMap |
| `Consultation` ↔ | `ConsultationDto` | ReverseMap |
| `ConsultationMessage` ↔ | `ConsultationMessageDto` | ReverseMap |
| `ConsultationReview` ↔ | `ConsultationReviewDto` | ReverseMap |
| `Doctor` → | `DoctorInfoDto` | `Name` ← `Fullname`, `Phone` ← `PhoneNumber` |
| `Nurse` → | `NurseInfoDto` | `Name` ← `Fullname`, `Phone` ← `PhoneNumber` |
| `Order` → | `OrderDto` | `Items` ← `OrderItem` |
| `OrderItem` ↔ | `OrderItemDto` | ReverseMap |
| `OrderAddress` ↔ | `OrderAddressDto` | ReverseMap |
| `BasketItem` → | `BasketItemDto` | `ProductName` ← `Medication.Name`, `PictureUrl` ← `Medication.PictureUrl` |
| `CustomerBasket` → | `BasketDto` | `Items` ← `BasketItems`, `SubTotal` and `Total` calculated |
| `NursingRequest` ↔ | `NursingRequestDto` | ReverseMap |
| `NursingReview` ↔ | `NursingReviewDto` | ReverseMap |

---

# 18. Specification Documentation

The project uses the **Specification Pattern** for querying.

## Base Specification

| Feature | Implementation |
|---------|---------------|
| `Criteria` | LINQ `Where` expression |
| `OrderByExpression` | Sort ascending |
| `OrderByDescExpression` | Sort descending |
| `Includes` | EF Core eager loading |

## Specifications Used

| Specification | Module | Description |
|---------------|--------|-------------|
| `NotificationsByUserIdSpecs` | Notification | Filters notifications by user ID |
| `UnreadNotificationsForUserSpecification` | Notification | Filters unread notifications by user ID |
| Various `AppointmentSpecs` | Appointment | Appointment-related queries |
| Various `ConsultationSpecs` | Consultation | Consultation-related queries |
| Various `NursingRequestSpecs` | Nursing | Nursing request queries |
| Various `OrderSpecs` | Order | Order-related queries |

---

# 19. Error Handling Documentation

## Global Exception Handler Middleware

All exceptions are caught by `CustomExceptionHandlerMiddleWare` and mapped to HTTP status codes.

### Error Response Format

```json
{
  "stutsCode": 404,
  "errorMsg": "User With Email: test@example.com Not Found",
  "errors": null
}
```

| Property | Type | Description |
|----------|------|-------------|
| `stutsCode` | `int` | HTTP status code |
| `errorMsg` | `string` | Error message |
| `errors` | `string[]?` | List of specific errors (only for `BadRequestException`) |

### Exception-to-Status Mapping

| Exception | HTTP Status |
|-----------|-------------|
| `NotFoundException` (abstract) | `404 Not Found` |
| `UserNotFoundException` | `404 Not Found` |
| `DoctorNotFoundException` | `404 Not Found` |
| `PatientNotFoundException` | `404 Not Found` |
| `PharmacistNotFoundException` | `404 Not Found` |
| `NotificationNotFoundException` | `404 Not Found` |
| `UnauthorizedAccessException` | `401 Unauthorized` |
| `InvalidCredentialsException` | `401 Unauthorized` |
| `ConflictException` (abstract) | `409 Conflict` |
| `BusinessRuleException` (abstract) | `422 Unprocessable Entity` |
| `BadRequestException` | `400 Bad Request` (with `errors` list) |
| Any other exception | `500 Internal Server Error` |

### Not Found Endpoint Response

When a URL path doesn't match any controller:

```json
{
  "stutsCode": 404,
  "errorMsg": "The Requested Url '/api/unknown' is not found"
}
```

### Validation Error Response (Model State)

```json
{
  "stutsCode": 400,
  "msg": "One or more validation errors occurred.",
  "errors": [
    {
      "field": "Email",
      "errorMsg": ["The Email field is required."]
    }
  ]
}
```

---

# 20. SignalR Documentation

## Notification Hub

| Property | Value |
|----------|-------|
| **Hub URL** | `/notificationHub` |
| **Hub Class** | `NotificationHub` |

### Client Events (Listen)

| Event | Payload | Description |
|-------|---------|-------------|
| `NewNotification` | `NotificationDto` | Receives real-time notifications |

### Connection Flow

```
1. Connect to wss://localhost:7292/notificationHub
2. Listen for "NewNotification" event
3. Receive: { id, message, isRead, createdAt }
```

### NotificationDto Payload

```json
{
  "id": 1,
  "message": "Your appointment has been confirmed.",
  "isRead": false,
  "createdAt": "2026-07-06T19:00:00"
}
```

---

## Chat Hub

| Property | Value |
|----------|-------|
| **Hub URL** | `/chatHub` |
| **Hub Class** | `ChatHub` |

### Server Methods (Invoke)

| Method | Parameters | Description |
|--------|------------|-------------|
| `JoinConsultation` | `consultationId: int` | Join a consultation chat group |

### Connection Flow

```
1. Connect to wss://localhost:7292/chatHub
2. Invoke "JoinConsultation" with consultationId
3. User is added to SignalR group "consultation_{consultationId}"
4. Receive real-time messages in the group
```

---

# 21. File Upload Documentation

## Accepted File Formats

| Extension | Accepted |
|-----------|----------|
| `.jpg` | ✅ |
| `.jpeg` | ✅ |
| `.png` | ✅ |
| Others | ❌ |

## Constraints

| Property | Value |
|----------|-------|
| **Maximum File Size** | 2 MB (2,097,152 bytes) |
| **Minimum File Size** | > 0 bytes |
| **Storage Location** | `wwwroot/files/{folderName}/` |
| **File Naming** | `{GUID}_{originalFilename}` |
| **Content-Type** | `multipart/form-data` |

## Upload Endpoint

Used in `POST /api/Medication/CreateMedication` (with `[FromForm]`).

**Returned URL format:** `/files/{folderName}/{guid}_{filename}`

---

# 22. TypeScript Interfaces

```typescript
// ===================== AUTH =====================

interface LoginRequest {
  email: string;
  password: string;
}

interface RegisterRequest {
  email: string;
  password: string;
  userName?: string;
  displayName: string;
  phoneNumber?: string;
}

interface ForgetPasswordRequest {
  email: string;
}

interface ResetPasswordRequest {
  email: string;
  token: string;
  newPassword: string;
  confirmPassword: string;
}

interface UserResponse {
  email: string;
  fullName: string;
  token: string;
}

// ===================== APPOINTMENT =====================

interface CreateAppointmentRequest {
  doctorId: number;
  scheduleId: number;
  appointmentDate: string; // ISO 8601
  notes?: string;
}

interface UpdateAppointmentRequest {
  scheduleId?: number;
  appointmentDate?: string;
  notes?: string;
}

interface AppointmentResponse {
  id: number;
  patientId: number;
  patientName: string;
  doctorId: number;
  doctorName: string;
  scheduleId: number;
  appointmentDate: string;
  appointmentTime: string; // "HH:mm:ss"
  status: AppointmentStatus;
  statusText: string;
  notes?: string;
  createdAt: string;
  updatedAt?: string;
}

interface AvailableDoctorSlotResponse {
  scheduleId: number;
  dayOfWeek: number;
  startTime: string;
  endTime: string;
  isAvailable: boolean;
  availableDates: string[];
}

// ===================== DOCTOR SCHEDULE =====================

interface CreateDoctorScheduleRequest {
  dayOfWeek: number; // 0-6
  startTime: string; // "HH:mm:ss"
  endTime: string;
  isAvailable?: boolean;
}

interface UpdateDoctorScheduleRequest {
  id: number;
  dayOfWeek: number;
  startTime: string;
  endTime: string;
  isAvailable: boolean;
}

interface DoctorScheduleResponse {
  id: number;
  doctorId: number;
  dayOfWeek: number;
  startTime: string;
  endTime: string;
  isAvailable: boolean;
  createdAt: string;
  updatedAt?: string;
}

// ===================== CONSULTATION =====================

interface SearchDoctorRequest {
  name?: string;
  specialization?: string;
  location?: string;
  pageNumber?: number;
  pageSize?: number;
}

interface DoctorInfoResponse {
  id: number;
  name: string;
  phone: string;
  specialty: string;
  location: string;
}

interface CreateConsultationRequest {
  doctorId: number;
}

interface UpdateConsultationStatusRequest {
  status: string; // "Pending" | "Accepted" | "Completed" | "Rejected"
}

interface ConsultationResponse {
  id: number;
  patientId: number;
  doctorId: number;
  status: string;
  requestedAt: string;
  createdAt: string;
  review?: ConsultationReviewResponse;
}

// ===================== CONSULTATION CHAT =====================

interface SendMessageRequest {
  content: string;
}

interface ConsultationMessageResponse {
  id: number;
  consultationId: number;
  senderUserId: number;
  senderName: string;
  content: string;
  isRead: boolean;
  createdAt: string;
}

// ===================== CONSULTATION REVIEW =====================

interface CreateConsultationReviewRequest {
  rating: number;
  comment: string;
}

interface ConsultationReviewResponse {
  id: number;
  consultationId: number;
  rating: number;
  comment: string;
  createdAt: string;
}

// ===================== MEDICATION =====================

interface MedicationResponse {
  id: number;
  name: string;
  price: number;
  stock: number;
  isAvailable: boolean;
  pictureUrl?: string;
}

interface AllMedicationResponse {
  id: number;
  createdAt: string;
  name: string;
  price: number;
  stock: number;
  is_available: boolean;
}

interface UpdateMedicationRequest {
  name: string;
  price: number;
  stock: number;
  isAvailable: boolean;
}

// ===================== BASKET =====================

interface BasketItemInputRequest {
  medicationId: number;
  quantity: number; // 1-100
}

interface BasketItemResponse {
  medicationId: number;
  productName: string;
  pictureUrl: string;
  price: number;
  quantity: number;
  subTotal: number;
}

interface BasketResponse {
  id: number;
  items: BasketItemResponse[];
  subTotal: number;
  shippingPrice: number;
  total: number;
}

// ===================== ORDER =====================

interface OrderAddressRequest {
  firstName: string;
  lastName: string;
  city: string;
  country: string;
  street: string;
}

interface CreateOrderRequest {
  address: OrderAddressRequest;
}

interface UpdateOrderStatusRequest {
  status: OrderStatus;
}

interface OrderItemResponse {
  id: number;
  medicationId: number;
  medicationName: string;
  pictureUrl: string;
  quantity: number;
  unitPrice: number;
  subtotal: number;
}

interface OrderResponse {
  id: number;
  status: string;
  total: number;
  orderDate: string;
  items: OrderItemResponse[];
}

// ===================== NURSING =====================

interface SearchNurseRequest {
  name?: string;
  specialization?: string;
  pageNumber?: number;
  pageSize?: number;
}

interface NurseInfoResponse {
  id: number;
  name: string;
  phone: string;
  specialization: string;
}

interface CreateNursingRequestRequest {
  patientId: number;
  nurseId: number;
  careType: string;
}

interface UpdateNursingStatusRequest {
  status: string;
}

interface CreateNursingReviewRequest {
  rating: number;
  comment: string;
}

interface NursingRequestResponse {
  patientId: number;
  nurseId: number;
  careType: string;
  status: string;
  review?: NursingReviewResponse;
}

interface NursingReviewResponse {
  nursingRequestId: number;
  rating: number;
  comment: string;
}

// ===================== NOTIFICATION =====================

interface NotificationResponse {
  id: number;
  message: string;
  isRead: boolean;
  createdAt: string;
}

// ===================== ERROR =====================

interface ErrorResponse {
  stutsCode: number;
  errorMsg: string;
  errors?: string[];
}

interface ValidationErrorResponse {
  stutsCode: number;
  msg: string;
  errors: ValidationError[];
}

interface ValidationError {
  field: string;
  errorMsg: string[];
}

// ===================== ENUMS =====================

enum AppointmentStatus {
  Pending = 0,
  Confirmed = 1,
  Cancelled = 2,
  Completed = 3,
}

enum ConsultationStatus {
  Pending = 0,
  Accepted = 1,
  Completed = 2,
  Rejected = 3,
}

enum OrderStatus {
  Pending = 0,
  Processing = 1,
  Shipped = 2,
  Delivered = 3,
  Cancelled = 4,
  Rejected = 5,
}

enum NotificationType {
  AppointmentReminder = 1,
  OrderUpdate = 2,
  ConsultationUpdate = 3,
  ConsultationRequest = 4,
  ConsultationReview = 5,
  Message = 6,
  System = 7,
}
```

---

# 23. Axios & React Query Examples

## Axios Setup

```typescript
import axios from "axios";

const API_BASE_URL = "https://localhost:7292/api";

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

// Request interceptor — attach JWT token
api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Response interceptor — handle 401
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem("token");
      window.location.href = "/login";
    }
    return Promise.reject(error);
  }
);

export default api;
```

## Auth API

```typescript
// Login
export const login = async (data: LoginRequest): Promise<UserResponse> => {
  const response = await api.post<UserResponse>("/Auth/Login", data);
  localStorage.setItem("token", response.data.token);
  return response.data;
};

// Register
export const register = async (data: RegisterRequest): Promise<UserResponse> => {
  const response = await api.post<UserResponse>("/Auth/Register", data);
  localStorage.setItem("token", response.data.token);
  return response.data;
};

// Forget Password
export const forgetPassword = async (email: string) => {
  return api.post("/Auth/forget-password", { email });
};

// Reset Password
export const resetPassword = async (data: ResetPasswordRequest) => {
  return api.post("/Auth/reset-password", data);
};
```

## Appointment API

```typescript
export const bookAppointment = (data: CreateAppointmentRequest) =>
  api.post<AppointmentResponse>("/Appointment/Book", data);

export const getMyAppointments = () =>
  api.get<AppointmentResponse[]>("/Appointment/MyAppointments");

export const updateAppointment = (id: number, data: UpdateAppointmentRequest) =>
  api.put<AppointmentResponse>(`/Appointment/Update/${id}`, data);

export const cancelAppointment = (id: number) =>
  api.post<AppointmentResponse>(`/Appointment/Cancel/${id}`);

export const getAvailableSlots = (doctorId: number, date: string) =>
  api.get<AvailableDoctorSlotResponse[]>(
    `/Appointment/AvailableSlots?doctorId=${doctorId}&date=${date}`
  );
```

## Medication API

```typescript
export const getAllMedications = (searchName?: string) =>
  api.get<AllMedicationResponse[]>("/Medication/GetAll", {
    params: { SearchName: searchName },
  });

export const getMedicationById = (id: number) =>
  api.get<MedicationResponse>(`/Medication/GetById/${id}`);

export const createMedication = (data: FormData) =>
  api.post<MedicationResponse>("/Medication/CreateMedication", data, {
    headers: { "Content-Type": "multipart/form-data" },
  });
```

## Basket & Order API

```typescript
export const getBasket = () => api.get<BasketResponse>("/Basket");

export const addToBasket = (data: BasketItemInputRequest) =>
  api.post<BasketResponse>("/Basket/AddItem", data);

export const createOrder = (data: CreateOrderRequest) =>
  api.post<OrderResponse>("/Order/Create", data);

export const getMyOrders = () =>
  api.get<OrderResponse[]>("/Order/MyOrders");
```

## React Query Examples

```typescript
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";

// Hook: Login
export const useLogin = () => {
  return useMutation({
    mutationFn: login,
    onSuccess: (data) => {
      localStorage.setItem("token", data.token);
    },
  });
};

// Hook: Get My Appointments
export const useMyAppointments = () => {
  return useQuery({
    queryKey: ["appointments", "my"],
    queryFn: async () => {
      const { data } = await getMyAppointments();
      return data;
    },
  });
};

// Hook: Book Appointment
export const useBookAppointment = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: CreateAppointmentRequest) =>
      bookAppointment(data).then((res) => res.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["appointments"] });
    },
  });
};

// Hook: Get All Medications
export const useMedications = (searchName?: string) => {
  return useQuery({
    queryKey: ["medications", searchName],
    queryFn: async () => {
      const { data } = await getAllMedications(searchName);
      return data;
    },
  });
};

// Hook: Get Basket
export const useBasket = () => {
  return useQuery({
    queryKey: ["basket"],
    queryFn: async () => {
      const { data } = await getBasket();
      return data;
    },
  });
};

// Hook: Add to Basket
export const useAddToBasket = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: BasketItemInputRequest) =>
      addToBasket(data).then((res) => res.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["basket"] });
    },
  });
};

// Hook: Create Order
export const useCreateOrder = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: CreateOrderRequest) =>
      createOrder(data).then((res) => res.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["orders"] });
      queryClient.invalidateQueries({ queryKey: ["basket"] });
    },
  });
};

// Hook: Search Doctors
export const useSearchDoctors = (params: SearchDoctorRequest) => {
  return useQuery({
    queryKey: ["doctors", params],
    queryFn: async () => {
      const { data } = await api.get<DoctorInfoResponse[]>(
        "/Consultation/GetAllDoctors",
        { params }
      );
      return data;
    },
  });
};
```

---

# 24. Complete API Summary

## All Endpoints at a Glance

| # | Method | Endpoint | Auth | Roles | Module |
|---|--------|----------|------|-------|--------|
| 1 | `POST` | `/api/Auth/Login` | No | — | Auth |
| 2 | `POST` | `/api/Auth/Register` | No | — | Auth |
| 3 | `POST` | `/api/Auth/forget-password` | No | — | Auth |
| 4 | `POST` | `/api/Auth/reset-password` | No | — | Auth |
| 5 | `POST` | `/api/Appointment/Book` | Yes | Patient | Appointment |
| 6 | `GET` | `/api/Appointment/MyAppointments` | Yes | Patient | Appointment |
| 7 | `PUT` | `/api/Appointment/Update/{id}` | Yes | Patient | Appointment |
| 8 | `GET` | `/api/Appointment/GetAppointment/{id}` | Yes | Any | Appointment |
| 9 | `POST` | `/api/Appointment/Cancel/{id}` | Yes | Any | Appointment |
| 10 | `GET` | `/api/Appointment/DoctorAppointments` | Yes | Doctor | Appointment |
| 11 | `POST` | `/api/Appointment/Confirm/{id}` | Yes | Doctor | Appointment |
| 12 | `POST` | `/api/Appointment/Complete/{id}` | Yes | Doctor | Appointment |
| 13 | `GET` | `/api/Appointment/AvailableSlots` | No | — | Appointment |
| 14 | `POST` | `/api/DoctorSchedule` | Yes | Doctor | Schedule |
| 15 | `GET` | `/api/DoctorSchedule/Doctor` | Yes | Doctor | Schedule |
| 16 | `PUT` | `/api/DoctorSchedule/{id}` | Yes | Doctor | Schedule |
| 17 | `DELETE` | `/api/DoctorSchedule/{id}` | Yes | Doctor | Schedule |
| 18 | `GET` | `/api/Consultation/GetAllDoctors` | No | — | Consultation |
| 19 | `POST` | `/api/Consultation/RequestConsultation` | Yes | Patient | Consultation |
| 20 | `GET` | `/api/Consultation/MyConsultations` | Yes | Patient, Doctor | Consultation |
| 21 | `GET` | `/api/Consultation/GetMyConsultationById/{id}` | Yes | Patient, Doctor | Consultation |
| 22 | `DELETE` | `/api/Consultation/DeleteConsultation/{id}` | Yes | Doctor | Consultation |
| 23 | `PUT` | `/api/Consultation/UpdateConsultationStatus/{id}` | Yes | Patient, Doctor | Consultation |
| 24 | `GET` | `/api/ConsultationChat/{id}/messages` | Yes | Patient, Doctor | Chat |
| 25 | `POST` | `/api/ConsultationChat/{id}/messages` | Yes | Patient, Doctor | Chat |
| 26 | `POST` | `/api/ConsultationChat/{id}/read` | Yes | Patient, Doctor | Chat |
| 27 | `GET` | `/api/ConsultationChat/{id}/unread-count` | Yes | Patient, Doctor | Chat |
| 28 | `POST` | `/api/ConsultationReview/{id}` | Yes | Patient | Review |
| 29 | `GET` | `/api/ConsultationReview/{id}` | Yes | Any | Review |
| 30 | `GET` | `/api/Medication/GetAll` | No | — | Medication |
| 31 | `GET` | `/api/Medication/GetById/{id}` | No | — | Medication |
| 32 | `POST` | `/api/Medication/CreateMedication` | Yes | Pharmacist, Admin | Medication |
| 33 | `POST` | `/api/Medication/UpdateMedication/{id}` | Yes | Pharmacist, Admin | Medication |
| 34 | `DELETE` | `/api/Medication/DeleteMedication/{id}` | Yes | Pharmacist, Admin | Medication |
| 35 | `GET` | `/api/Basket` | Yes | Patient | Basket |
| 36 | `POST` | `/api/Basket/AddItem` | Yes | Patient | Basket |
| 37 | `PUT` | `/api/Basket/UpdateItem/{medicationId}` | Yes | Patient | Basket |
| 38 | `DELETE` | `/api/Basket/RemoveItem/{medicationId}` | Yes | Patient | Basket |
| 39 | `DELETE` | `/api/Basket/Clear` | Yes | Patient | Basket |
| 40 | `POST` | `/api/Order/Create` | Yes | Patient | Order |
| 41 | `GET` | `/api/Order/MyOrders` | Yes | Patient | Order |
| 42 | `GET` | `/api/Order/GetMyOrder/{id}` | Yes | Patient | Order |
| 43 | `POST` | `/api/Order/Cancel/{id}` | Yes | Patient | Order |
| 44 | `DELETE` | `/api/Order/Delete` | Yes | Patient | Order |
| 45 | `GET` | `/api/Order/Orders` | Yes | Pharmacist, Admin | Order |
| 46 | `GET` | `/api/Order/GetOrder/{id}` | Yes | Pharmacist, Admin | Order |
| 47 | `PUT` | `/api/Order/Orders/{id}/Status` | Yes | Pharmacist, Admin | Order |
| 48 | `GET` | `/api/Nursing/Search` | No | — | Nursing |
| 49 | `POST` | `/api/Nursing/Request` | Yes | Patient | Nursing |
| 50 | `GET` | `/api/Nursing/MyRequests` | Yes | Patient, Nurse | Nursing |
| 51 | `PUT` | `/api/Nursing/UpdateStatus/{id}` | Yes | Patient, Nurse, Admin | Nursing |
| 52 | `POST` | `/api/Nursing/Cancel/{id}` | Yes | Patient, Nurse | Nursing |
| 53 | `POST` | `/api/Nursing/Review/{id}` | Yes | Patient | Nursing |
| 54 | `GET` | `/api/Nursing/Review/{id}` | Yes | Any | Nursing |
| 55 | `GET` | `/api/Notification/GetNotifications` | Yes* | Any | Notification |
| 56 | `DELETE` | `/api/Notification/DeleteNotification` | Yes* | Any | Notification |
| 57 | `GET` | `/api/Notification/GetUnreadCount` | Yes* | Any | Notification |
| 58 | `POST` | `/api/Notification/MarkAsRead` | Yes* | Any | Notification |

**Total: 58 Endpoints**

---

## Workflow Diagrams

### Patient Flow

```
Register/Login → Get JWT Token
    ↓
Browse Doctors (GetAllDoctors) → Request Consultation
    ↓
Chat with Doctor (SendMessage) → Complete Consultation → Add Review
    ↓
Check Available Slots → Book Appointment → Wait for Confirmation
    ↓
Browse Medications → Add to Basket → Create Order → Track Order
    ↓
Search Nurses → Request Nursing → Review Nurse
```

### Doctor Flow

```
Login → Get JWT Token
    ↓
View Doctor Appointments → Confirm/Complete Appointments
    ↓
Manage Doctor Schedules (CRUD)
    ↓
View My Consultations → Accept/Reject → Chat with Patient
```

### Pharmacist Flow

```
Login → Get JWT Token
    ↓
Manage Medications (Create/Update/Delete)
    ↓
View All Orders → Update Order Status (Processing → Shipped → Delivered)
```

### Nurse Flow

```
Login → Get JWT Token
    ↓
View My Nursing Requests → Accept/Complete/Cancel Requests
```

---

> **End of Documentation**
>
> This documentation was extracted entirely from the source code of the Tabiby ASP.NET Core Web API project. All information accurately reflects the implemented codebase.
