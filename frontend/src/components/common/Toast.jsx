import { CheckCircle2, XCircle, AlertTriangle, Info, X } from 'lucide-react'

const STYLES = {
  success: { icon: CheckCircle2, border: 'border-vital-500/40', iconClass: 'text-vital-400' },
  error: { icon: XCircle, border: 'border-pulse-500/40', iconClass: 'text-pulse-400' },
  warning: { icon: AlertTriangle, border: 'border-amber-400/40', iconClass: 'text-amber-400' },
  info: { icon: Info, border: 'border-white/15', iconClass: 'text-slate-300' },
}

export default function Toast({ type = 'info', message, onClose }) {
  const { icon: Icon, border, iconClass } = STYLES[type] || STYLES.info

  return (
    <div
      role="status"
      className={`pointer-events-auto flex w-[calc(100vw-2rem)] max-w-sm items-start gap-3 rounded-xl border bg-ink-900 px-4 py-3 text-white shadow-panel animate-fadeUp ${border}`}
    >
      <Icon size={20} className={`mt-0.5 shrink-0 ${iconClass}`} />
      <p className="flex-1 text-sm font-medium leading-snug">{message}</p>
      <button onClick={onClose} className="shrink-0 text-slate-400 hover:text-white" aria-label="Dismiss notification">
        <X size={16} />
      </button>
    </div>
  )
}
