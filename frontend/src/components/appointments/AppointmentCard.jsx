import { CalendarDays, Clock } from 'lucide-react'
import StatusBadge from '../common/StatusBadge'
import { formatDate, formatTime } from '../../utils/format'

export default function AppointmentCard({ appointment, onCancel, onReschedule, onReview }) {
  const initials = appointment.doctorName
    .replace('Dr. ', '')
    .split(' ')
    .map((p) => p[0])
    .slice(0, 2)
    .join('')

  return (
    <div className="card flex flex-col gap-4 p-5 sm:flex-row sm:items-center sm:justify-between">
      <div className="flex items-center gap-3.5">
        <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-full bg-ink-900 font-display text-sm font-bold text-vital-400">
          {initials}
        </div>
        <div>
          <h3 className="font-display text-sm font-semibold text-ink-900">{appointment.doctorName}</h3>
          <p className="text-xs font-medium text-vital-600">{appointment.specialty}</p>
        </div>
      </div>

      <div className="flex flex-wrap items-center gap-4 text-sm text-slate-500 sm:gap-6">
        <div className="flex items-center gap-1.5">
          <CalendarDays size={15} className="text-slate-400" />
          {formatDate(appointment.appointmentDate)}
        </div>
        <div className="flex items-center gap-1.5">
          <Clock size={15} className="text-slate-400" />
          {formatTime(appointment.appointmentTime)}
        </div>
        <StatusBadge status={appointment.status} />
      </div>

      <div className="flex gap-2 sm:justify-end">
        {appointment.status === 'Pending' && onCancel && (
          <button onClick={() => onCancel(appointment)} className="btn-secondary !py-2 text-sm">
            Cancel
          </button>
        )}
        {appointment.status === 'Confirmed' && onReschedule && (
          <button onClick={() => onReschedule(appointment)} className="btn-secondary !py-2 text-sm">
            Reschedule
          </button>
        )}

      </div>
    </div>
  )
}
