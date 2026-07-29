import { MapPin, Phone, MessageCircle, CalendarDays } from 'lucide-react'
import { Link } from 'react-router-dom'




export default function DoctorCard({ doctor }) {
  const initials = doctor.name
    .replace('Dr. ', '')
    .split(' ')
    .map((p) => p[0])
    .slice(0, 2)
    .join('')

  return (
    <div className="card group flex flex-col p-5 transition-shadow duration-200 hover:shadow-panel">
      <div className="flex items-start gap-3.5">
        <div className="flex h-14 w-14 shrink-0 items-center justify-center rounded-full bg-ink-900 font-display text-lg font-bold text-vital-400">
          {initials}
        </div>
        <div className="min-w-0 flex-1">
          <Link to={`/app/doctors/${doctor.id}`} className="truncate font-display text-base font-semibold text-ink-900 hover:text-vital-700">
            {doctor.name}
          </Link>
          <p className="text-sm font-medium text-vital-600">{doctor.specialty}</p>
          {doctor.location && (
            <div className="mt-1 flex items-center gap-1 text-xs text-slate-500">
              <MapPin size={13} />
              <span className="truncate">{doctor.location}</span>
            </div>
          )}
          {doctor.phone && (
            <div className="mt-0.5 flex items-center gap-1 text-xs text-slate-500">
              <Phone size={13} />
              <span className="truncate">{doctor.phone}</span>
            </div>
          )}
        </div>
      </div>

      <div className="mt-4 flex gap-2">
        <Link to={`/app/doctors/${doctor.id}`} className="btn-primary flex-1 !py-2.5 text-sm">
          <CalendarDays size={15} />
          Book Appointment
        </Link>
        <Link to={`/app/doctors/${doctor.id}?consult=1`} className="btn-secondary flex-1 !py-2.5 text-sm">
          <MessageCircle size={15} />
          Consult
        </Link>
      </div>
    </div>
  )
}
