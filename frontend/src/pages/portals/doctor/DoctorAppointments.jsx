import { useEffect, useState } from 'react'
import { CalendarX2 } from 'lucide-react'
import StatusBadge from '../../../components/common/StatusBadge'
import EmptyState from '../../../components/common/EmptyState'
import { useToast } from '../../../context/ToastContext'
import { appointmentsApi } from '../../../api/endpoints/appointments'
import { formatDate, formatTime } from '../../../utils/format'

export default function DoctorAppointments() {
  const toast = useToast()
  const [appointments, setAppointments] = useState([])
  const [isLoading, setIsLoading] = useState(true)

  const load = () => {
    appointmentsApi
      .doctorAppointments()
      .then(setAppointments)
      .catch((err) => toast.error(err.message || 'Could not load appointments'))
      .finally(() => setIsLoading(false))
  }
  useEffect(load, [])

  const act = async (fn, id, successMsg) => {
    try {
      await fn(id)
      toast.success(successMsg)
      load()
    } catch (err) {
      toast.error(err.message || 'Action failed')
    }
  }

  return (
    <div className="space-y-6">
      <div>
        <h2 className="font-display text-2xl font-bold text-ink-900">Appointments</h2>
        <p className="mt-1 text-sm text-slate-500">Manage your patient appointments.</p>
      </div>

      {isLoading ? (
        <p className="text-sm text-slate-500">Loading…</p>
      ) : appointments.length === 0 ? (
        <EmptyState icon={CalendarX2} title="No appointments" message="Booked appointments will appear here." />
      ) : (
        <div className="space-y-3">
          {appointments.map((a) => (
            <div key={a.id} className="card flex flex-col gap-3 p-5 sm:flex-row sm:items-center sm:justify-between">
              <div>
                <p className="font-display text-sm font-semibold text-ink-900">{a.patientName}</p>
                <p className="text-sm text-slate-500">
                  {formatDate(a.appointmentDate)} · {formatTime(a.appointmentTime?.slice(0, 5))}
                </p>
                {a.notes && <p className="mt-1 text-xs text-slate-400">{a.notes}</p>}
              </div>
              <div className="flex items-center gap-3">
                <StatusBadge status={a.statusText} />
                {a.statusText === 'Pending' && (
                  <button onClick={() => act(appointmentsApi.confirm, a.id, 'Appointment confirmed')} className="btn-secondary !py-2 text-sm">
                    Confirm
                  </button>
                )}
                {a.statusText === 'Confirmed' && (
                  <button onClick={() => act(appointmentsApi.complete, a.id, 'Marked as completed')} className="btn-secondary !py-2 text-sm">
                    Mark Completed
                  </button>
                )}
                {(a.statusText === 'Pending' || a.statusText === 'Confirmed') && (
                  <button onClick={() => act(appointmentsApi.cancel, a.id, 'Appointment cancelled')} className="btn-ghost text-sm text-pulse-600">
                    Cancel
                  </button>
                )}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
