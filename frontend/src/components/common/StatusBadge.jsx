const STYLES = {
  Confirmed: 'bg-vital-500/10 text-vital-700',
  Active: 'bg-vital-500/10 text-vital-700',
  Assigned: 'bg-vital-500/10 text-vital-700',
  Shipped: 'bg-vital-500/10 text-vital-700',
  Available: 'bg-vital-500/10 text-vital-700',
  Processing: 'bg-vital-500/10 text-vital-700',
  Pending: 'bg-amber-100 text-amber-700',
  Completed: 'bg-slate-100 text-slate-600',
  Delivered: 'bg-slate-100 text-slate-600',
  Closed: 'bg-slate-100 text-slate-600',
  Off: 'bg-slate-100 text-slate-500',
  Cancelled: 'bg-pulse-500/10 text-pulse-600',
  Rejected: 'bg-pulse-500/10 text-pulse-600',
  'Out of Stock': 'bg-pulse-500/10 text-pulse-600',
}

export default function StatusBadge({ status }) {
  const style = STYLES[status] || 'bg-slate-100 text-slate-600'
  return <span className={`badge ${style}`}>{status}</span>
}
