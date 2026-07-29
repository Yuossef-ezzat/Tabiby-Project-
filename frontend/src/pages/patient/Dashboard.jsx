import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { CalendarDays, MessageCircle, ShoppingBag, HeartPulse, ArrowRight, Stethoscope } from 'lucide-react'
import StatCard from '../../components/dashboard/StatCard'
import AppointmentCard from '../../components/appointments/AppointmentCard'
import VitalDivider from '../../components/common/VitalDivider'
import { useAuth } from '../../context/AuthContext'
import { useToast } from '../../context/ToastContext'
import { appointmentsApi } from '../../api/endpoints/appointments'
import { consultationsApi } from '../../api/endpoints/consultations'
import { doctorsApi } from '../../api/endpoints/doctors'
import { ordersApi } from '../../api/endpoints/orders'
import { nursingApi } from '../../api/endpoints/nursing'

export default function Dashboard() {
  const { user } = useAuth()
  const toast = useToast()
  const firstName = (user?.fullName || '').split(' ')[0] || 'there'

  const [appointments, setAppointments] = useState([])
  const [consultations, setConsultations] = useState([])
  const [doctorsById, setDoctorsById] = useState({})
  const [pendingOrders, setPendingOrders] = useState(0)
  const [pendingNursing, setPendingNursing] = useState(0)

  useEffect(() => {
    appointmentsApi.myAppointments().then(setAppointments).catch((e) => toast.error(e.message))
    consultationsApi.myConsultations().then(setConsultations).catch((e) => toast.error(e.message))
    doctorsApi
      .search({ pageSize: 200 })
      .then((list) => setDoctorsById(Object.fromEntries((list || []).map((d) => [d.id, d]))))
      .catch(() => {})
    ordersApi
      .myOrders()
      .then((list) => setPendingOrders((list || []).filter((o) => o.status !== 'Delivered' && o.status !== 'Cancelled').length))
      .catch(() => {})
    nursingApi
      .myRequests()
      .then((list) => setPendingNursing((list || []).filter((r) => r.status === 'Pending' || r.status === 'Accepted').length))
      .catch(() => {})
    
  }, [])

  const upcoming = appointments
    .filter((a) => a.statusText !== 'Completed' && a.statusText !== 'Cancelled')
    .slice(0, 2)
    .map((a) => ({
      id: a.id,
      doctorName: a.doctorName,
      specialty: doctorsById[a.doctorId]?.specialty || '',
      appointmentDate: a.appointmentDate,
      appointmentTime: a.appointmentTime?.slice(0, 5),
      status: a.statusText,
      notes: a.notes,
    }))

  const activeConsultations = consultations.filter((c) => c.status === 'Accepted')
  const topDoctors = Object.values(doctorsById).slice(0, 3)

  return (
    <div className="space-y-8">
      <div className="relative overflow-hidden rounded-xl2 bg-ink-900 p-7 sm:p-9">
        <div className="pointer-events-none absolute -right-10 -top-16 h-52 w-52 rounded-full bg-vital-500/20 blur-3xl" />
        <p className="font-body text-sm font-semibold uppercase tracking-[0.2em] text-vital-400">
          Good to see you
        </p>
        <h2 className="mt-1.5 font-display text-2xl font-bold text-white sm:text-3xl">
          Hi {firstName}, here's your health snapshot
        </h2>
        <VitalDivider className="my-4 max-w-xs text-vital-400/60" />
        <Link to="/app/doctors" className="btn-primary mt-1 inline-flex">
          <Stethoscope size={18} />
          Book a new appointment
        </Link>
      </div>

      <div className="grid grid-cols-2 gap-4 lg:grid-cols-4">
        <StatCard icon={CalendarDays} label="Upcoming appointments" value={upcoming.length} accent="vital" />
        <StatCard icon={MessageCircle} label="Active consultations" value={activeConsultations.length} accent="vital" />
        <StatCard icon={ShoppingBag} label="Pending orders" value={pendingOrders} accent="pulse" />
        <StatCard icon={HeartPulse} label="Nursing requests" value={pendingNursing} accent="pulse" />
      </div>

      <section>
        <div className="mb-4 flex items-center justify-between">
          <h3 className="font-display text-lg font-bold text-ink-900">Upcoming Appointments</h3>
          <Link to="/app/appointments" className="flex items-center gap-1 text-sm font-semibold text-vital-600 hover:text-vital-700">
            View all <ArrowRight size={15} />
          </Link>
        </div>
        {upcoming.length === 0 ? (
          <p className="text-sm text-slate-500">No upcoming appointments yet.</p>
        ) : (
          <div className="space-y-3">
            {upcoming.map((a) => (
              <AppointmentCard key={a.id} appointment={a} />
            ))}
          </div>
        )}
      </section>

      <div className="grid gap-6 lg:grid-cols-2">
        <section>
          <div className="mb-4 flex items-center justify-between">
            <h3 className="font-display text-lg font-bold text-ink-900">Active Consultations</h3>
            <Link to="/app/consultations" className="flex items-center gap-1 text-sm font-semibold text-vital-600 hover:text-vital-700">
              View all <ArrowRight size={15} />
            </Link>
          </div>
          <div className="card divide-y divide-mist-200">
            {activeConsultations.length === 0 && (
              <p className="p-4 text-sm text-slate-500">No active consultations.</p>
            )}
            {activeConsultations.map((c) => {
              const doc = doctorsById[c.doctorId]
              const name = doc?.name || c.doctorName || `Doctor #${c.doctorId}`
              return (
                <Link key={c.id} to="/app/consultations" className="flex items-center gap-3.5 p-4 transition-colors hover:bg-mist-50">
                  <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-full bg-ink-900 font-display text-sm font-bold text-vital-400">
                    {name.replace('Dr. ', '').split(' ').map((p) => p[0]).slice(0, 2).join('')}
                  </div>
                  <div className="min-w-0 flex-1">
                    <p className="font-display text-sm font-semibold text-ink-900">{name}</p>
                    <p className="truncate text-xs text-slate-500">{doc?.specialty}</p>
                  </div>
                </Link>
              )
            })}
          </div>
        </section>

        <section>
          <div className="mb-4 flex items-center justify-between">
            <h3 className="font-display text-lg font-bold text-ink-900">Doctors</h3>
            <Link to="/app/doctors" className="flex items-center gap-1 text-sm font-semibold text-vital-600 hover:text-vital-700">
              Browse all <ArrowRight size={15} />
            </Link>
          </div>
          <div className="card divide-y divide-mist-200">
            {topDoctors.length === 0 && <p className="p-4 text-sm text-slate-500">No doctors found.</p>}
            {topDoctors.map((d) => (
              <Link key={d.id} to={`/app/doctors/${d.id}`} className="flex items-center gap-3.5 p-4 transition-colors hover:bg-mist-50">
                <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-full bg-ink-900 font-display text-sm font-bold text-vital-400">
                  {d.name.replace('Dr. ', '').split(' ').map((p) => p[0]).slice(0, 2).join('')}
                </div>
                <div className="min-w-0 flex-1">
                  <p className="font-display text-sm font-semibold text-ink-900">{d.name}</p>
                  <p className="text-xs text-slate-500">{d.specialty}</p>
                </div>
              </Link>
            ))}
          </div>
        </section>
      </div>
    </div>
  )
}
