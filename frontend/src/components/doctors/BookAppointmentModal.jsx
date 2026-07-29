import { useState } from 'react'
import { CalendarDays } from 'lucide-react'
import Modal from '../common/Modal'
import { useToast } from '../../context/ToastContext'
import { formatEGP } from '../../utils/format'

const TIME_SLOTS = ['09:00 AM', '10:30 AM', '12:00 PM', '02:00 PM', '04:30 PM', '06:00 PM']

export default function BookAppointmentModal({ doctor, onClose, onSuccess }) {
  const toast = useToast()
  const [date, setDate] = useState('')
  const [slot, setSlot] = useState(null)
  const [notes, setNotes] = useState('')
  const [isBooking, setIsBooking] = useState(false)

  const onConfirm = async (e) => {
    e.preventDefault()
    if (!date || !slot) {
      toast.error('Please select a date and time slot')
      return
    }
    setIsBooking(true)
    
    await new Promise((r) => setTimeout(r, 600))
    setIsBooking(false)
    toast.success('Appointment requested!')
    onSuccess()
  }

  return (
    <Modal title="Book Appointment" onClose={onClose}>
      <div className="mb-5 flex items-center justify-between rounded-xl bg-mist-100 px-4 py-3">
        <div>
          <p className="text-xs text-slate-500">Booking with</p>
          <p className="font-display text-sm font-semibold text-ink-900">{doctor.name}</p>
        </div>
      </div>

      <form onSubmit={onConfirm} className="space-y-4" noValidate>
        <div>
          <label className="field-label-light" htmlFor="date">Appointment date</label>
          <div className="relative">
            <CalendarDays size={18} className="pointer-events-none absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-400" />
            <input
              id="date"
              type="date"
              className="field-input-light pl-11"
              value={date}
              onChange={(e) => setDate(e.target.value)}
            />
          </div>
        </div>

        <div>
          <label className="field-label-light">Select a time slot</label>
          <div className="grid grid-cols-2 gap-2">
            {TIME_SLOTS.map((s) => (
              <button
                type="button"
                key={s}
                onClick={() => setSlot(s)}
                className={`rounded-xl border px-3 py-2.5 text-sm font-medium transition-colors ${
                  slot === s
                    ? 'border-vital-500 bg-vital-500/10 text-vital-700'
                    : 'border-mist-200 text-slate-600 hover:border-vital-300'
                }`}
              >
                {s}
              </button>
            ))}
          </div>
        </div>

        <div>
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

        <button type="submit" disabled={isBooking} className="btn-primary w-full">
          {isBooking ? 'Booking…' : 'Confirm Appointment'}
        </button>
      </form>
    </Modal>
  )
}
