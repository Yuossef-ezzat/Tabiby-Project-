import { useEffect, useMemo, useState } from 'react'
import { Search, ShoppingCart } from 'lucide-react'
import { Link } from 'react-router-dom'
import MedicationCard from '../../components/medications/MedicationCard'
import EmptyState from '../../components/common/EmptyState'
import { useToast } from '../../context/ToastContext'
import { medicationsApi } from '../../api/endpoints/medications'
import { basketApi } from '../../api/endpoints/basket'




export default function Pharmacy() {
  const toast = useToast()
  const [query, setQuery] = useState('')
  const [medications, setMedications] = useState([])
  const [cartCount, setCartCount] = useState(0)
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    basketApi
      .get()
      .then((b) => setCartCount((b?.items || []).reduce((sum, i) => sum + i.quantity, 0)))
      .catch(() => {})
  }, [])

  useEffect(() => {
    setIsLoading(true)
    const timeout = setTimeout(() => {
      medicationsApi
        .getAll(query || undefined)
        .then(setMedications)
        .catch((err) => toast.error(err.message || 'Could not load medications'))
        .finally(() => setIsLoading(false))
    }, 250)
    return () => clearTimeout(timeout)
    
  }, [query])

  const normalized = useMemo(
    () =>
      medications.map((m) => ({
        id: m.id,
        name: m.name,
        price: m.price,
        stock: m.stock,
        
        
        isAvailable: m.is_available ?? m.isAvailable,
      })),
    [medications],
  )

  const onAdd = async (medication, qty = 1) => {
    try {
      await basketApi.addItem(medication.id, qty)
      setCartCount((c) => c + qty)
      toast.success(`${medication.name} added to cart`)
    } catch (err) {
      toast.error(err.message || 'Could not add to cart')
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="font-display text-2xl font-bold text-ink-900">Medication Market</h2>
          <p className="mt-1 text-sm text-slate-500">Browse and order prescribed medications.</p>
        </div>
        <Link to="/app/cart" className="btn-secondary relative">
          <ShoppingCart size={18} />
          Cart
          {cartCount > 0 && (
            <span className="absolute -right-2 -top-2 flex h-5 w-5 items-center justify-center rounded-full bg-pulse-500 text-[11px] font-bold text-white">
              {cartCount}
            </span>
          )}
        </Link>
      </div>

      <div className="relative">
        <Search size={18} className="pointer-events-none absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" />
        <input
          type="search"
          placeholder="Search medications…"
          className="field-input-light pl-11"
          value={query}
          onChange={(e) => setQuery(e.target.value)}
        />
      </div>

      {isLoading ? (
        <p className="text-sm text-slate-500">Loading…</p>
      ) : normalized.length === 0 ? (
        <EmptyState icon={Search} title="No medications found" message="Try a different search term." />
      ) : (
        <div className="grid grid-cols-2 gap-5 sm:grid-cols-3 lg:grid-cols-4">
          {normalized.map((m) => (
            <MedicationCard key={m.id} medication={m} onAdd={onAdd} />
          ))}
        </div>
      )}
    </div>
  )
}
