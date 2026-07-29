export default function Logo({ variant = 'light', size = 'md', showWordmark = true }) {
  const dims = size === 'sm' ? 28 : size === 'lg' ? 44 : 34
  const textColor = variant === 'light' ? 'text-white' : 'text-ink-900'

  return (
    <div className="flex items-center gap-2.5 select-none">
      <div
        className="flex shrink-0 items-center justify-center rounded-xl bg-ink-900"
        style={{ width: dims, height: dims }}
      >
        <svg viewBox="0 0 32 32" width={dims * 0.62} height={dims * 0.62}>
          <path
            d="M4 17h5l2-6 4 12 3-9 2 3h8"
            fill="none"
            stroke="#4DE8DC"
            strokeWidth="2.4"
            strokeLinecap="round"
            strokeLinejoin="round"
          />
        </svg>
      </div>
      {showWordmark && (
        <span className={`font-display text-xl font-bold tracking-tight ${textColor}`}>
          Tabiby
        </span>
      )}
    </div>
  )
}
