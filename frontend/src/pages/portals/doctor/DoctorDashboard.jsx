import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { Inbox, Activity, CheckCircle2, ListChecks } from 'lucide-react'
import StatCard from '../../../components/dashboard/StatCard'
import PendingRequestCard from '../../../components/requests/PendingRequestCard'
import { useToast } from '../../../context/ToastContext'
import { consultationsApi } from '../../../api/endpoints/consultations'






export default function DoctorDashboard() {
  const toast = useToast()
  const [consultations, setConsultations] = useState([])

  const load = () => {
    consultationsApi
      .myConsultations()
      .then(setConsultations)
      .catch((err) => toast.error(err.message || 'Could not load consultations'))
  }
  useEffect(load, [])

  const pending = consultations.filter((c) => c.status === 'Pending')
  const active = consultations.filter((c) => c.status === 'Accepted')
  const completed = consultations.filter((c) => c.status === 'Completed')

  const respond = async (id, status, successMsg) => {
    try {
      await consultationsApi.updateStatus(id, status)
      toast.success(successMsg)
      load()
    } catch (err) {
      toast.error(err.message || 'Could not update this request')
    }
  }

  return (
    <div className="space-y-8">
      <div>
        <h2 className="font-display text-2xl font-bold text-ink-900">Doctor Dashboard</h2>
        <p className="mt-1 text-sm text-slate-500">Review incoming requests and manage your active consultations.</p>
      </div>

      <div className="grid grid-cols-2 gap-4 lg:grid-cols-4">
        <StatCard icon={Inbox} label="Pending Requests" value={pending.length} accent="pulse" />
        <StatCard icon={Activity} label="Active Consultations" value={active.length} accent="vital" />
        <StatCard icon={CheckCircle2} label="Completed" value={completed.length} accent="ink" />
        <StatCard icon={ListChecks} label="Total Requests" value={consultations.length} accent="ink" />
      </div>

      <section>
        <h3 className="mb-4 font-display text-lg font-bold text-ink-900">Pending Requests</h3>
        {pending.length === 0 ? (
          <p className="text-sm text-slate-500">No pending requests right now.</p>
        ) : (
          <div className="grid gap-4 sm:grid-cols-2">
            {pending.map((r) => (
              <PendingRequestCard
                key={r.id}
                name={r.patientName || `Patient #${r.patientId}`}
                meta="New consultation request"
                onAccept={() => respond(r.id, 'Accepted', 'Request accepted')}
                onReject={() => respond(r.id, 'Rejected', 'Request rejected')}
              />
            ))}
          </div>
        )}
      </section>

      <section>
        <h3 className="mb-4 font-display text-lg font-bold text-ink-900">Active Consultations</h3>
        <div className="card divide-y divide-mist-200">
          {active.length === 0 && <p className="p-4 text-sm text-slate-500">No active consultations.</p>}
          {active.map((c) => (
            <Link key={c.id} to="/app/doctor/chat" className="flex items-center gap-3.5 p-4 transition-colors hover:bg-mist-50">
              <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-full bg-ink-900 font-display text-sm font-bold text-vital-400">
                P{c.patientId}
              </div>
              <div className="min-w-0 flex-1">
                <p className="font-display text-sm font-semibold text-ink-900">{c.patientName || `Patient #${c.patientId}`}</p>
                <p className="truncate text-xs text-slate-500">Open the chat to continue the conversation</p>
              </div>
            </Link>
          ))}
        </div>
      </section>

      <section>
        <h3 className="mb-4 font-display text-lg font-bold text-ink-900">Completed Consultations</h3>
        <div className="space-y-3">
          {completed.length === 0 && <p className="text-sm text-slate-500">No completed consultations.</p>}
          {completed.map((c) => (
            <div key={c.id} className="card flex flex-col p-5">
              <div className="flex items-center gap-3.5">
                <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-full bg-ink-900 font-display text-sm font-bold text-vital-400">
                  P{c.patientId}
                </div>
                <div className="min-w-0 flex-1">
                  <p className="font-display text-sm font-semibold text-ink-900">{c.patientName || `Patient #${c.patientId}`}</p>
                  <p className="truncate text-xs text-slate-500">Completed</p>
                </div>
              </div>
              {c.review && (
                <div className="mt-4 w-full rounded-lg bg-mist-50 p-4 text-sm">
                  <div className="flex items-center justify-between">
                    <span className="font-semibold text-ink-900">{c.patientName || `Patient #${c.patientId}`}</span>
                    <span className="font-medium text-vital-600">★ {c.review.rating}/5</span>
                  </div>
                  <p className="mt-1 text-slate-600">{c.review.comment}</p>
                </div>
              )}
            </div>
          ))}
        </div>
      </section>
    </div>
  )
}
