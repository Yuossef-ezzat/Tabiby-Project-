import { useState } from 'react'
import { Pencil, Trash2, Plus, X } from 'lucide-react'
import StatusBadge from '../common/StatusBadge'
import { useToast } from '../../context/ToastContext'
import { medicationsApi } from '../../api/endpoints/medications'
import { formatEGP } from '../../utils/format'





export default function InventoryTable({ items, onChanged, readOnly = false }) {
  const toast = useToast()
  const [showAddForm, setShowAddForm] = useState(false)
  const [newMed, setNewMed] = useState({ name: '', price: '', stock: '', image: null })
  const [editingId, setEditingId] = useState(null)
  const [editDraft, setEditDraft] = useState({ name: '', price: '', stock: '' })
  const [isSaving, setIsSaving] = useState(false)

  const onChangeNewMed = (e) => {
    const { name, value, files } = e.target
    setNewMed((f) => ({ ...f, [name]: files ? files[0] : value }))
  }

  const onSaveMedication = async () => {
    if (!newMed.name || !newMed.price || !newMed.stock) {
      toast.error('Please fill required fields')
      return
    }
    setIsSaving(true)
    try {
      await medicationsApi.create({
        name: newMed.name,
        price: Number(newMed.price),
        stock: Number(newMed.stock),
        isAvailable: Number(newMed.stock) > 0,
        image: newMed.image,
      })
      setNewMed({ name: '', price: '', stock: '', image: null })
      setShowAddForm(false)
      toast.success('Medication added!')
      onChanged?.()
    } catch (err) {
      toast.error(err.message || 'Could not add this medication')
    } finally {
      setIsSaving(false)
    }
  }

  const startEdit = (m) => {
    setEditingId(m.id)
    setEditDraft({ name: m.name, price: m.price, stock: m.stock, isAvailable: m.isAvailable })
  }

  const saveEdit = async (m) => {
    try {
      await medicationsApi.update(m.id, {
        name: editDraft.name,
        price: Number(editDraft.price),
        stock: Number(editDraft.stock),
        isAvailable: editDraft.isAvailable,
      })
      toast.success('Medication updated!')
      setEditingId(null)
      onChanged?.()
    } catch (err) {
      toast.error(err.message || 'Could not update this medication')
    }
  }

  const onDelete = async (id) => {
    try {
      await medicationsApi.remove(id)
      toast.success('Medication removed!')
      onChanged?.()
    } catch (err) {
      const msg = err.message || ''
      if (msg.includes('entity changes') || msg.includes('constraint')) {
        toast.error('Cannot delete this medication because it is linked to a patient order. Try setting it to Out of Stock instead.')
      } else {
        toast.error(msg || 'Could not remove this medication')
      }
    }
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <p className="text-sm text-slate-500">{items.length} medications</p>
        {!readOnly && (
          <button onClick={() => setShowAddForm((s) => !s)} className="btn-primary text-sm">
            {showAddForm ? <X size={16} /> : <Plus size={16} />}
            {showAddForm ? 'Cancel' : 'Add Medication'}
          </button>
        )}
      </div>

      {!readOnly && showAddForm && (
        <div className="card grid gap-3 p-5 sm:grid-cols-2">
          <input name="name" placeholder="Medication name" className="field-input-light" value={newMed.name} onChange={onChangeNewMed} />
          <input name="price" placeholder="Price (EGP)" type="number" className="field-input-light" value={newMed.price} onChange={onChangeNewMed} />
          <input name="stock" placeholder="Stock quantity" type="number" className="field-input-light" value={newMed.stock} onChange={onChangeNewMed} />
          <input name="image" type="file" accept="image/*" className="field-input-light" onChange={onChangeNewMed} />
          <div className="flex gap-2 sm:col-span-2">
            <button className="btn-primary" disabled={isSaving} onClick={onSaveMedication}>
              {isSaving ? 'Saving…' : 'Save Medication'}
            </button>
            <button className="btn-secondary" onClick={() => setShowAddForm(false)}>Cancel</button>
          </div>
        </div>
      )}

      <div className="card overflow-x-auto">
        <table className="w-full min-w-[640px] text-left text-sm">
          <thead>
            <tr className="border-b border-mist-200 text-xs uppercase tracking-wide text-slate-400">
              <th className="px-5 py-3.5 font-semibold">Medication</th>
              <th className="px-5 py-3.5 font-semibold">Price</th>
              <th className="px-5 py-3.5 font-semibold">Stock</th>
              <th className="px-5 py-3.5 font-semibold">Status</th>
              {!readOnly && <th className="px-5 py-3.5 font-semibold text-right">Action</th>}
            </tr>
          </thead>
          <tbody className="divide-y divide-mist-200">
            {items.map((m) => {
              const isEditing = editingId === m.id
              return (
                <tr key={m.id}>
                  <td className="px-5 py-4 font-display font-semibold text-ink-900">
                    {isEditing ? (
                      <input className="field-input-light !py-1.5" value={editDraft.name} onChange={(e) => setEditDraft((d) => ({ ...d, name: e.target.value }))} />
                    ) : (
                      m.name
                    )}
                  </td>
                  <td className="px-5 py-4 text-slate-700">
                    {isEditing ? (
                      <input type="number" className="field-input-light !py-1.5" value={editDraft.price} onChange={(e) => setEditDraft((d) => ({ ...d, price: e.target.value }))} />
                    ) : (
                      formatEGP(m.price)
                    )}
                  </td>
                  <td className="px-5 py-4 text-slate-700">
                    {isEditing ? (
                      <input type="number" className="field-input-light !py-1.5" value={editDraft.stock} onChange={(e) => setEditDraft((d) => ({ ...d, stock: e.target.value }))} />
                    ) : (
                      m.stock
                    )}
                  </td>
                  <td className="px-5 py-4">
                    {isEditing ? (
                      <label className="flex items-center gap-2 text-sm text-slate-700 cursor-pointer">
                        <input type="checkbox" checked={editDraft.isAvailable || false} onChange={(e) => setEditDraft((d) => ({ ...d, isAvailable: e.target.checked }))} className="rounded border-slate-300 text-vital-500 focus:ring-vital-500" />
                        Available
                      </label>
                    ) : (
                      <StatusBadge status={m.isAvailable ? 'Available' : 'Out of Stock'} />
                    )}
                  </td>
                  {!readOnly && (
                    <td className="px-5 py-4">
                      <div className="flex justify-end gap-1.5">
                        {isEditing ? (
                          <>
                            <button onClick={() => saveEdit(m)} className="rounded-lg px-2 py-1 text-xs font-semibold text-vital-600 hover:bg-mist-100">Save</button>
                            <button onClick={() => setEditingId(null)} className="rounded-lg px-2 py-1 text-xs text-slate-500 hover:bg-mist-100">Cancel</button>
                          </>
                        ) : (
                          <>
                            <button onClick={() => startEdit(m)} className="rounded-lg p-1.5 text-slate-400 hover:bg-mist-100 hover:text-vital-600" aria-label="Edit">
                              <Pencil size={15} />
                            </button>
                            <button onClick={() => onDelete(m.id)} className="rounded-lg p-1.5 text-slate-400 hover:bg-mist-100 hover:text-pulse-500" aria-label="Delete">
                              <Trash2 size={15} />
                            </button>
                          </>
                        )}
                      </div>
                    </td>
                  )}
                </tr>
              )
            })}
          </tbody>
        </table>
      </div>
    </div>
  )
}
