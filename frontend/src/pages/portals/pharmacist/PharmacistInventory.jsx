import { useEffect, useState } from 'react'
import InventoryTable from '../../../components/tables/InventoryTable'
import { useToast } from '../../../context/ToastContext'
import { medicationsApi } from '../../../api/endpoints/medications'

export default function PharmacistInventory() {
  const toast = useToast()
  const [items, setItems] = useState([])
  const [isLoading, setIsLoading] = useState(true)

  const [searchQuery, setSearchQuery] = useState('')

  const load = () => {
    medicationsApi
      .getAll()
      .then((list) =>
        setItems(
          (list || []).map((m) => ({
            id: m.id,
            name: m.name,
            price: m.price,
            stock: m.stock,
            isAvailable: m.isAvailable ?? m.IsAvailable ?? m.is_available ?? false,
          })),
        ),
      )
      .catch((err) => toast.error(err.message || 'Could not load inventory'))
      .finally(() => setIsLoading(false))
  }
  useEffect(load, [])

  const filteredItems = items.filter(i => i.name.toLowerCase().includes(searchQuery.toLowerCase()))

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h2 className="font-display text-2xl font-bold text-ink-900">Inventory</h2>
          <p className="mt-1 text-sm text-slate-500">Manage stock levels and pricing for the pharmacy catalog.</p>
        </div>
        <div className="relative">
          <input
            type="search"
            placeholder="Search medications..."
            className="field-input-light pl-4 w-full sm:w-64"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
          />
        </div>
      </div>
      {isLoading ? <p className="text-sm text-slate-500">Loading…</p> : <InventoryTable items={filteredItems} onChanged={load} />}
    </div>
  )
}
