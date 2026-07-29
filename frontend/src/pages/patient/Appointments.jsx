import { useEffect, useState } from 'react'
import { CalendarX2 } from 'lucide-react'
import AppointmentCard from '../../components/appointments/AppointmentCard'
import EmptyState from '../../components/common/EmptyState'
import Modal from '../../components/common/Modal'
import { useToast } from '../../context/ToastContext'
import { appointmentsApi } from '../../api/endpoints/appointments'
import { doctorsApi } from '../../api/endpoints/doctors'

const TABS = ['Upcoming', 'Past']

export default function Appointments() {
  const toast = useToast()
  const [tab, setTab] = useState('Upcoming')
  const [appointments, setAppointments] = useState([])
  const [doctorsById, setDoctorsById] = useState({})
  const [isLoading, setIsLoading] = useState(true)
  const [cancelTarget, setCancelTarget] = useState(null)
  
  const [rescheduleTarget, setRescheduleTarget] = useState(null)
  const [rescheduleDate, setRescheduleDate] = useState(new Date().toISOString().slice(0, 10))
  const [rescheduleSlots, setRescheduleSlots] = useState([])
  const [selectedRescheduleSlot, setSelectedRescheduleSlot] = useState(null)
  const [isRescheduling, setIsRescheduling] = useState(false)

  const load = () => {
    setIsLoading(true)
    Promise.all([appointmentsApi.myAppointments(), doctorsApi.search({ pageSize: 200 })])
      .then(([appts, doctors]) => {
        setAppointments(appts || [])
        setDoctorsById(Object.fromEntries((doctors || []).map((d) => [d.id, d])))
      })
      .catch((err) => toast.error(err.message || 'Could not load appointments'))
      .finally(() => setIsLoading(false))
  }

  useEffect(load, [])

  useEffect(() => {
    if (!rescheduleTarget || !rescheduleDate) return
    setSelectedRescheduleSlot(null)
    appointmentsApi
      .availableSlots(rescheduleTarget.doctorId, rescheduleDate)
      .then((data) => setRescheduleSlots(data || []))
      .catch((err) => toast.error(err.message || 'Could not load available slots'))
  }, [rescheduleTarget, rescheduleDate])

  const isPast = (a) => a.statusText === 'Completed' || a.statusText === 'Cancelled'
  const list = appointments
    .filter((a) => (tab === 'Upcoming' ? !isPast(a) : isPast(a)))
    .map((a) => ({
      id: a.id,
      doctorId: a.doctorId,
      doctorName: a.doctorName,
      specialty: doctorsById[a.doctorId]?.specialty || '',
      appointmentDate: a.appointmentDate,
      appointmentTime: a.appointmentTime?.slice(0, 5),
      status: a.statusText,
      notes: a.notes,
    }))

  const confirmCancel = async () => {
    try {
      await appointmentsApi.cancel(cancelTarget)
      toast.success('Appointment cancelled')
      setCancelTarget(null)
      load()
    } catch (err) {
      toast.error(err.message || 'Could not cancel this appointment')
    }
  }

  const confirmReschedule = async () => {
    if (!selectedRescheduleSlot) {
      toast.error('Please select a time slot')
      return
    }
    setIsRescheduling(true)
    try {
      await appointmentsApi.update(rescheduleTarget.id, {
        scheduleId: selectedRescheduleSlot.scheduleId,
        appointmentDate: rescheduleDate,
        notes: rescheduleTarget.notes
      })
      toast.success('Appointment rescheduled successfully!')
      setRescheduleTarget(null)
      load()
    } catch (err) {
      toast.error(err.message || 'Could not reschedule this appointment')
    } finally {
      setIsRescheduling(false)
    }
  }

  return (
    <div className="space-y-6">
      <div>
        <h2 className="font-display text-2xl font-bold text-ink-900">My Appointments</h2>
        <p className="mt-1 text-sm text-slate-500">Track, manage, and review your visits.</p>
      </div>

      <div className="flex gap-2 border-b border-mist-200">
        {TABS.map((t) => (
          <button
            key={t}
            onClick={() => setTab(t)}
            className={`px-4 py-2.5 text-sm font-semibold transition-colors ${
              tab === t ? 'border-b-2 border-vital-500 text-vital-700' : 'text-slate-500 hover:text-ink-900'
            }`}
          >
            {t}
          </button>
        ))}
      </div>

      {isLoading ? (
        <p className="text-sm text-slate-500">Loading…</p>
      ) : list.length === 0 ? (
        <EmptyState icon={CalendarX2} title={`No ${tab.toLowerCase()} appointments`} message="Book a new appointment to see it here." />
      ) : (
        <div className="space-y-3">
          {list.map((a) => (
            <AppointmentCard
              key={a.id}
              appointment={a}
              onCancel={tab === 'Upcoming' && a.status === 'Pending' ? () => setCancelTarget(a.id) : undefined}
              onReschedule={tab === 'Upcoming' && a.status === 'Confirmed' ? () => {
                setRescheduleTarget(a)
                setRescheduleDate(new Date().toISOString().slice(0, 10))
                setRescheduleSlots([])
                setSelectedRescheduleSlot(null)
              } : undefined}
            />
          ))}
        </div>
      )}

      {cancelTarget && (
        <Modal onClose={() => setCancelTarget(null)} title="Cancel appointment?">
          <p className="text-sm text-slate-600">This will cancel your appointment. This can't be undone.</p>
          <div className="mt-5 flex justify-end gap-3">
            <button className="btn-secondary" onClick={() => setCancelTarget(null)}>Keep it</button>
            <button className="btn-danger" onClick={confirmCancel}>Cancel Appointment</button>
          </div>
        </Modal>
      )}

      {rescheduleTarget && (
        <Modal onClose={() => setRescheduleTarget(null)} title="Reschedule Appointment">
          <div className="space-y-4">
            <div>
              <label className="field-label-light">Select New Date</label>
              <input
                type="date"
                min={new Date().toISOString().slice(0, 10)}
                className="field-input-light w-full"
                value={rescheduleDate}
                onChange={(e) => setRescheduleDate(e.target.value)}
              />
            </div>

            <div>
              <label className="field-label-light">Select Time Slot</label>
              {rescheduleSlots.length === 0 ? (
                <p className="text-sm text-slate-500">No open slots for this date.</p>
              ) : (
                <div className="grid grid-cols-2 gap-2 max-h-48 overflow-y-auto">
                  {rescheduleSlots.map((slot) => (
                    <button
                      key={slot.scheduleId}
                      onClick={() => setSelectedRescheduleSlot(slot)}
                      className={`rounded-xl border px-3 py-2.5 text-sm font-medium transition-colors ${
                        selectedRescheduleSlot?.scheduleId === slot.scheduleId
                          ? 'border-vital-500 bg-vital-500/10 text-vital-700'
                          : 'border-mist-200 text-slate-600 hover:border-vital-300'
                      }`}
                    >
                      {slot.startTime.slice(0, 5)} – {slot.endTime.slice(0, 5)}
                    </button>
                  ))}
                </div>
              )}
            </div>

            <div className="mt-6 flex justify-end gap-3">
              <button className="btn-secondary" onClick={() => setRescheduleTarget(null)}>Cancel</button>
              <button 
                className="btn-primary" 
                onClick={confirmReschedule} 
                disabled={!selectedRescheduleSlot || isRescheduling}
              >
                {isRescheduling ? 'Rescheduling...' : 'Confirm Reschedule'}
              </button>
            </div>
          </div>
        </Modal>
      )}
    </div>
  )
}
