export default function VitalDivider({ className = '' }) {
  return (
    <svg
      className={`h-6 w-full ${className}`}
      viewBox="0 0 400 24"
      preserveAspectRatio="none"
      fill="none"
    >
      <path
        d="M0 12H150L162 3L174 21L186 12H400"
        stroke="currentColor"
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
        strokeDasharray="6 4"
        className="animate-pulseLine"
      />
    </svg>
  )
}
