import { X } from 'lucide-react'

export default function Modal({ title, onClose, children, maxWidth = 'max-w-md' }) {
  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-ink-950/60 p-4 backdrop-blur-sm animate-fadeUp"
      onClick={onClose}
    >
      <div
        className={`w-full ${maxWidth} rounded-xl2 bg-white p-6 shadow-panel sm:p-7`}
        onClick={(e) => e.stopPropagation()}
      >
        <div className="mb-5 flex items-center justify-between">
          <h3 className="font-display text-lg font-bold text-ink-900">{title}</h3>
          <button onClick={onClose} className="text-slate-400 hover:text-ink-900" aria-label="Close">
            <X size={20} />
          </button>
        </div>
        {children}
      </div>
    </div>
  )
}
