import { useEffect, useState } from 'react'
import { Plus, Trash2, X } from 'lucide-react'
import StatusBadge from '../../../components/common/StatusBadge'
import { useToast } from '../../../context/ToastContext'
import { doctorScheduleApi } from '../../../api/endpoints/doctorSchedule'
import { formatTime } from '../../../utils/format'



const DAYS = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday']

export default function DoctorSchedule() {
  const toast = useToast()
  const [slots, setSlots] = useState([])
  const [isLoading, setIsLoading] = useState(true)
  const [showForm, setShowForm] = useState(false)
  const [form, setForm] = useState({ dayOfWeek: 1, startTime: '09:00', endTime: '17:00' })
  const [isSaving, setIsSaving] = useState(false)

  const load = () => {
    doctorScheduleApi
      .getMine()
      .then(setSlots)
      .catch((err) => toast.error(err.message || 'Could not load your schedule'))
      .finally(() => setIsLoading(false))
  }
  useEffect(load, [])

  const addSlot = async (e) => {
    e.preventDefault()
    setIsSaving(true)
    try {
      await doctorScheduleApi.create({
        dayOfWeek: Number(form.dayOfWeek),
        startTime: `${form.startTime}:00`,
        endTime: `${form.endTime}:00`,
        isAvailable: true,
      })
      toast.success('Slot added!')
      setShowForm(false)
      load()
    } catch (err) {
      toast.error(err.message || 'Could not add this slot')
    } finally {
      setIsSaving(false)
    }
  }

  const toggleAvailable = async (slot) => {
    try {
      await doctorScheduleApi.update(slot.id, { ...slot, isAvailable: !slot.isAvailable })
      load()
    } catch (err) {
      toast.error(err.message || 'Could not update this slot')
    }
  }

  const removeSlot = async (id) => {
    try {
      await doctorScheduleApi.remove(id)
      toast.info('Slot removed')
      load()
    } catch (err) {
      toast.error(err.message || 'Could not remove this slot')
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="font-display text-2xl font-bold text-ink-900">My Schedule</h2>
          <p className="mt-1 text-sm text-slate-500">Manage your weekly availability.</p>
        </div>
        <button onClick={() => setShowForm((s) => !s)} className="btn-primary text-sm">
          {showForm ? <X size={16} /> : <Plus size={16} />}
          {showForm ? 'Cancel' : 'Add Slot'}
        </button>
      </div>

      {showForm && (
        <form onSubmit={addSlot} className="card grid gap-4 p-5 sm:grid-cols-4">
          <div>
            <label className="field-label-light">Day</label>
            <select
              className="field-input-light"
              value={form.dayOfWeek}
              onChange={(e) => setForm((f) => ({ ...f, dayOfWeek: e.target.value }))}
            >
              {DAYS.map((d, idx) => (
                <option key={d} value={idx}>{d}</option>
              ))}
            </select>
          </div>
          <div>
            <label className="field-label-light">Start time</label>
            <input
              type="time"
              className="field-input-light"
              value={form.startTime}
              onChange={(e) => setForm((f) => ({ ...f, startTime: e.target.value }))}
            />
          </div>
          <div>
            <label className="field-label-light">End time</label>
            <input
              type="time"
              className="field-input-light"
              value={form.endTime}
              onChange={(e) => setForm((f) => ({ ...f, endTime: e.target.value }))}
            />
          </div>
          <div className="flex items-end">
            <button type="submit" disabled={isSaving} className="btn-primary w-full">
              {isSaving ? 'Saving…' : 'Save Slot'}
            </button>
          </div>
        </form>
      )}

      <div className="card divide-y divide-mist-200">
        {isLoading && <p className="p-5 text-sm text-slate-500">Loading…</p>}
        {!isLoading && slots.length === 0 && <p className="p-5 text-sm text-slate-500">No schedule slots yet.</p>}
        {slots.map((slot) => (
          <div key={slot.id} className="flex items-center justify-between gap-4 p-5">
            <div className="flex items-center gap-4">
              <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-mist-100 font-display text-xs font-bold text-slate-600">
                {DAYS[slot.dayOfWeek]?.slice(0, 3)}
              </div>
              <div>
                <p className="font-display text-sm font-semibold text-ink-900">{DAYS[slot.dayOfWeek]}</p>
                <p className="text-sm text-slate-500">
                  {formatTime(slot.startTime?.slice(0, 5))} – {formatTime(slot.endTime?.slice(0, 5))}
                </p>
              </div>
            </div>
            <div className="flex items-center gap-3">
              <button onClick={() => toggleAvailable(slot)}>
                <StatusBadge status={slot.isAvailable ? 'Available' : 'Off'} />
              </button>
              <button
                onClick={() => removeSlot(slot.id)}
                className="rounded-lg p-1.5 text-slate-400 hover:bg-mist-100 hover:text-pulse-500"
                aria-label="Delete"
              >
                <Trash2 size={15} />
              </button>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}
