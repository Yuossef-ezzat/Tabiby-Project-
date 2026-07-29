export default function StatCard({ icon: Icon, label, value, accent = 'vital' }) {
  const accents = {
    vital: 'bg-vital-500/10 text-vital-600',
    pulse: 'bg-pulse-500/10 text-pulse-600',
    ink: 'bg-ink-900/5 text-ink-900',
  }

  return (
    <div className="card flex items-center gap-4 p-5">
      <div className={`flex h-11 w-11 shrink-0 items-center justify-center rounded-xl ${accents[accent]}`}>
        <Icon size={20} />
      </div>
      <div>
        <p className="font-display text-2xl font-bold text-ink-900">{value}</p>
        <p className="text-sm text-slate-500">{label}</p>
      </div>
    </div>
  )
}
