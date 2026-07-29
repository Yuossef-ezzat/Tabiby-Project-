# Tabiby — Frontend

A React + Tailwind CSS frontend for **Tabiby**, a healthcare platform (clinic
booking, online consultations, home nursing, and a medication pharmacy),
wired to the real ASP.NET Core backend (`ProjectDEPI`) — no mock data left
in the data path (a couple of static UI lists like specialty names remain in
`src/mock/mockData.js` since they aren't backed by any endpoint).

## Stack

- **React 18** + **React Router v6**
- **Tailwind CSS** (custom design tokens — see `tailwind.config.js`)
- **lucide-react** for icons
- **@microsoft/signalr** for the real-time Chat and Notification hubs
- **Vite** for tooling

## Getting started

1. Run the backend (`ProjectDEPI`) — by default it listens on
   `https://localhost:7292` and only accepts requests from
   `http://localhost:3000` / `:3001` (see its CORS policy), so keep this app
   on port 3000 (already pinned in `vite.config.js`).
2. Copy `.env.example` to `.env` if you haven't already, and point
   `VITE_API_BASE_URL` at your backend's `/api` root.
3. Install and run:

```bash
npm install
npm run dev
```

4. Register a new account (always created as a Patient) or use an account
   seeded by the backend's `DataSeeder` — note its seed file paths are
   hardcoded to the original developer's machine, so seeding may not run
   unless you adjust that.

## Folder structure

```
src/
├── api/
│   ├── client.js          # fetch wrapper: JWT header, multipart, error normalization
│   ├── signalr.js          # ChatHub / NotificationHub connection builders
│   └── endpoints/          # One file per backend module, mapped 1:1 to controllers
├── assets/images/
├── components/
├── context/
│   ├── AuthContext.jsx      # Real login/register/logout, JWT decode, session restore
│   └── ToastContext.jsx
├── layouts/
├── mock/mockData.js         # Only static UI lists left (e.g. specialty names)
├── pages/
├── routes/
├── styles/
├── utils/
│   ├── jwt.js               # JWT payload decoder (role/email/id claims)
│   └── format.js, images.js
├── App.jsx
└── main.jsx
```

## Known backend contract gaps (frontend adapted around these; not modified)

These come from actually reading the current backend implementation — see
the in-file comments (search "NOTE:") in the relevant `api/endpoints/*.js`
and pages for the full detail:

- **`NursingRequestDto` has no `Id` field.** The nurse-facing requests page
  can only show requests read-only — there's no way to target
  `UpdateStatus/{requestId}` or `Cancel/{requestId}` from that list.
- **No doctor/patient names on several DTOs** (`ConsultationDto`,
  `OrderDto`) — the UI falls back to "Patient #id" / "Doctor #id" or
  cross-references a separately fetched doctor list.
- **No suspend/reactivate for admin accounts** — only hard delete exists.
- **Medications have no category**, and `GetAll` doesn't return a picture
  (only `GetById` does) — category filtering was dropped; list images fall
  back to a placeholder.
- **Nursing requests take a `NurseId`, not an address/date** — patients
  search for and select a nurse instead of typing an address.
- Enum fields without a `JsonStringEnumConverter` (`OrderStatus`,
  `DayOfWeek`) are sent/received as plain numbers, not strings — handled in
  `orders.js` / `doctorSchedule.js`.

## Design system

- **Palette:** deep clinical navy (`ink`), a signal-teal accent (`vital`),
  and a warm coral (`pulse`) for destructive/alert states.
- **Type:** `Sora` for display/headings, `Inter` for body and UI text.
- Fully responsive: sidebar collapses to a slide-over on mobile, grids
  reflow to 1–2 columns, and the auth layout drops its left copy panel
  below `lg`.
