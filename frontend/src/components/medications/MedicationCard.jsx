import { ShoppingCart, Plus, Minus } from 'lucide-react'
import { useState } from 'react'
import { resolveMedicationImage } from '../../utils/images'
import { formatEGP } from '../../utils/format'

export default function MedicationCard({ medication, onAdd }) {
  const [qty, setQty] = useState(1)

  return (
    <div className="card flex flex-col overflow-hidden transition-shadow duration-200 hover:shadow-panel">
      <div className="relative aspect-[4/3] w-full overflow-hidden bg-mist-100">
        <img
          src={resolveMedicationImage(medication.pictureUrl)}
          alt={medication.name}
          className="h-full w-full object-cover transition-transform duration-300 group-hover:scale-105"
        />
        {!medication.isAvailable && (
          <div className="absolute inset-0 flex items-center justify-center bg-ink-950/50">
            <span className="badge bg-white/90 text-slate-700">Out of Stock</span>
          </div>
        )}
      </div>

      <div className="flex flex-1 flex-col p-4">
        <h3 className="font-display text-sm font-semibold leading-snug text-ink-900">{medication.name}</h3>
        <p className="mt-1 text-xs text-slate-400">{medication.stock} in stock</p>

        <div className="mt-3 flex items-center justify-between">
          <span className="font-display text-base font-bold text-ink-900">{formatEGP(medication.price)}</span>

          {medication.isAvailable && (
            <div className="flex items-center gap-1.5 rounded-full border border-mist-200 px-1 py-1">
              <button
                onClick={() => setQty((q) => Math.max(1, q - 1))}
                className="flex h-6 w-6 items-center justify-center rounded-full text-slate-500 hover:bg-mist-100"
                aria-label="Decrease quantity"
              >
                <Minus size={13} />
              </button>
              <span className="w-4 text-center text-sm font-semibold">{qty}</span>
              <button
                onClick={() => setQty((q) => q + 1)}
                className="flex h-6 w-6 items-center justify-center rounded-full text-slate-500 hover:bg-mist-100"
                aria-label="Increase quantity"
              >
                <Plus size={13} />
              </button>
            </div>
          )}
        </div>

        <button
          onClick={() => onAdd?.(medication, qty)}
          disabled={!medication.isAvailable}
          className="btn-primary mt-3 w-full !py-2.5 text-sm"
        >
          <ShoppingCart size={16} />
          Add to cart
        </button>
      </div>
    </div>
  )
}
