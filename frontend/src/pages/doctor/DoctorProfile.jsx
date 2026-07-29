import { useEffect, useState } from 'react'
import { useParams, Link, useNavigate } from 'react-router-dom'
import { MapPin, Phone, CalendarDays, CheckCircle2, ArrowLeft, MessageCircle } from 'lucide-react'
import { useToast } from '../../context/ToastContext'
import { doctorsApi } from '../../api/endpoints/doctors'
import { appointmentsApi } from '../../api/endpoints/appointments'
import { consultationsApi } from '../../api/endpoints/consultations'
import { formatTime } from '../../utils/format'

function todayIso() {
  return new Date().toISOString().slice(0, 10)
}

export default function DoctorProfile() {
  const { id } = useParams()
  const navigate = useNavigate()
  const toast = useToast()

  const [doctor, setDoctor] = useState(null)
  const [date, setDate] = useState(todayIso())
  const [slots, setSlots] = useState([])
  const [selectedSlot, setSelectedSlot] = useState(null)
  const [notes, setNotes] = useState('')
  const [confirmed, setConfirmed] = useState(false)
  const [isBooking, setIsBooking] = useState(false)
  const [isRequestingConsult, setIsRequestingConsult] = useState(false)

  // There's no single "get doctor by id" route exposed, so we search the
  
  useEffect(() => {
    doctorsApi
      .search({ pageSize: 200 })
      .then((list) => setDoctor((list || []).find((d) => String(d.id) === id) || null))
      .catch((err) => toast.error(err.message || 'Could not load doctor'))
    
  }, [id])

  useEffect(() => {
    if (!date) return
    setSelectedSlot(null)
    appointmentsApi
      .availableSlots(id, date)
      .then((data) => setSlots(data || []))
      .catch((err) => toast.error(err.message || 'Could not load available slots'))
    
  }, [id, date])

  const initials = doctor
    ? doctor.name.replace('Dr. ', '').split(' ').map((p) => p[0]).slice(0, 2).join('')
    : '..'

  const onConfirm = async () => {
    if (!selectedSlot) {
      toast.error('Please select a time slot')
      return
    }
    setIsBooking(true)
    try {
      await appointmentsApi.book({
        doctorId: Number(id),
        scheduleId: selectedSlot.scheduleId,
        appointmentDate: date,
        notes: notes || undefined,
      })
      setConfirmed(true)
      toast.success('Appointment requested!')
    } catch (err) {
      toast.error(err.message || 'Could not book this appointment')
    } finally {
      setIsBooking(false)
    }
  }

  const onRequestConsultation = async () => {
    setIsRequestingConsult(true)
    try {
      await consultationsApi.request(Number(id))
      toast.success('Consultation requested!')
      navigate('/app/consultations')
    } catch (err) {
      toast.error(err.message || 'Could not request a consultation')
    } finally {
      setIsRequestingConsult(false)
    }
  }

  if (!doctor) {
    return (
      <div className="space-y-6">
        <Link to="/app/doctors" className="inline-flex items-center gap-1.5 text-sm font-medium text-slate-500 hover:text-ink-900">
          <ArrowLeft size={16} /> Back to doctors
        </Link>
        <p className="text-sm text-slate-500">Loading doctor…</p>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <Link to="/app/doctors" className="inline-flex items-center gap-1.5 text-sm font-medium text-slate-500 hover:text-ink-900">
        <ArrowLeft size={16} /> Back to doctors
      </Link>

      <div className="grid gap-6 lg:grid-cols-3">
        <div className="card p-6 lg:col-span-2">
          <div className="flex flex-col gap-5 sm:flex-row sm:items-start">
            <div className="flex h-20 w-20 shrink-0 items-center justify-center rounded-2xl bg-ink-900 font-display text-2xl font-bold text-vital-400">
              {initials}
            </div>
            <div className="flex-1">
              <h2 className="font-display text-2xl font-bold text-ink-900">{doctor.name}</h2>
              <p className="font-medium text-vital-600">{doctor.specialty}</p>
              <div className="mt-2 flex flex-wrap items-center gap-4 text-sm text-slate-500">
                {doctor.location && <span className="flex items-center gap-1"><MapPin size={14} /> {doctor.location}</span>}
                {doctor.phone && <span className="flex items-center gap-1"><Phone size={14} /> {doctor.phone}</span>}
              </div>
            </div>
          </div>

          <div className="mt-6 border-t border-mist-200 pt-6">
            <h3 className="font-display text-base font-semibold text-ink-900">Message the doctor</h3>
            <p className="mt-2 text-sm leading-relaxed text-slate-600">
              Prefer to describe your symptoms first? Start a consultation thread and chat with{' '}
              {doctor.name} directly.
            </p>
            <button onClick={onRequestConsultation} disabled={isRequestingConsult} className="btn-secondary mt-3">
              <MessageCircle size={16} />
              {isRequestingConsult ? 'Requesting…' : 'Request a Consultation'}
            </button>
          </div>
        </div>

        <div className="card h-fit p-6">
          <h3 className="font-display text-lg font-bold text-ink-900">Book Appointment</h3>

          {confirmed ? (
            <div className="mt-5 flex flex-col items-center gap-3 rounded-xl bg-vital-500/10 px-4 py-8 text-center">
              <CheckCircle2 size={36} className="text-vital-600" />
              <p className="font-semibold text-ink-900">Appointment requested!</p>
              <p className="text-sm text-slate-500">
                {doctor.name} will confirm your appointment shortly.
              </p>
              <Link to="/app/appointments" className="btn-secondary mt-2 w-full">
                View My Appointments
              </Link>
            </div>
          ) : (
            <>
              <div className="mt-5">
                <label className="field-label-light" htmlFor="date">Date</label>
                <input
                  id="date"
                  type="date"
                  min={todayIso()}
                  className="field-input-light"
                  value={date}
                  onChange={(e) => setDate(e.target.value)}
                />
              </div>

              <div className="mt-4">
                <label className="field-label-light">Select a time slot</label>
                {slots.length === 0 ? (
                  <p className="text-sm text-slate-500">No open slots for this date.</p>
                ) : (
                  <div className="grid grid-cols-2 gap-2">
                    {slots.map((slot) => (
                      <button
                        key={slot.scheduleId}
                        onClick={() => setSelectedSlot(slot)}
                        className={`rounded-xl border px-3 py-2.5 text-sm font-medium transition-colors ${
                          selectedSlot?.scheduleId === slot.scheduleId
                            ? 'border-vital-500 bg-vital-500/10 text-vital-700'
                            : 'border-mist-200 text-slate-600 hover:border-vital-300'
                        }`}
                      >
                        {formatTime(slot.startTime.slice(0, 5))} – {formatTime(slot.endTime.slice(0, 5))}
                      </button>
                    ))}
                  </div>
                )}
              </div>

              <div className="mt-4">
                <label className="field-label-light" htmlFor="notes">Notes (optional)</label>
                <textarea
                  id="notes"
                  rows={3}
                  placeholder="Describe your symptoms…"
                  className="field-input-light resize-none"
                  value={notes}
                  onChange={(e) => setNotes(e.target.value)}
                />
              </div>

              <button onClick={onConfirm} disabled={!selectedSlot || isBooking} className="btn-primary mt-5 w-full">
                {isBooking ? 'Booking…' : 'Confirm Appointment'}
              </button>
            </>
          )}
        </div>
      </div>
    </div>
  )
}
