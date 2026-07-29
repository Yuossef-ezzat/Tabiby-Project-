import { CheckCircle2, XCircle } from 'lucide-react'
import StatusBadge from '../common/StatusBadge'

export default function PendingRequestCard({ name, meta, status = 'Pending', onAccept, onReject }) {
  return (
    <div className="card p-5">
      <div className="flex items-start justify-between gap-3">
        <div>
          <h3 className="font-display text-sm font-semibold text-ink-900">{name}</h3>
          <p className="mt-0.5 text-xs text-slate-500">{meta}</p>
        </div>
        <StatusBadge status={status} />
      </div>

      {status === 'Pending' && (
        <div className="mt-4 flex gap-2">
          <button onClick={onAccept} className="btn-primary flex-1 !py-2 text-sm">
            <CheckCircle2 size={16} />
            Accept
          </button>
          <button onClick={onReject} className="btn-secondary flex-1 !py-2 text-sm !text-pulse-600">
            <XCircle size={16} />
            Reject
          </button>
        </div>
      )}
    </div>
  )
}
