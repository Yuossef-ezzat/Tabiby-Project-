import { useState } from 'react'
import { User, MapPin, Banknote } from 'lucide-react'
import Modal from '../common/Modal'
import { useToast } from '../../context/ToastContext'
import { ordersApi } from '../../api/endpoints/orders'
import { formatEGP } from '../../utils/format'




export default function CheckoutModal({ total, onClose, onSuccess }) {
  const toast = useToast()
  const [form, setForm] = useState({ firstName: '', lastName: '', street: '', city: '', country: '' })
  const [errors, setErrors] = useState({})
  const [isPlacing, setIsPlacing] = useState(false)

  const onChange = (e) => setForm((f) => ({ ...f, [e.target.name]: e.target.value }))

  const validate = () => {
    const next = {}
    if (!form.firstName) next.firstName = 'First name is required'
    if (!form.lastName) next.lastName = 'Last name is required'
    if (!form.street) next.street = 'Street address is required'
    if (!form.city) next.city = 'City is required'
    if (!form.country) next.country = 'Country is required'
    setErrors(next)
    return Object.keys(next).length === 0
  }

  const onSubmit = async (e) => {
    e.preventDefault()
    if (!validate()) {
      toast.error('Please fill required fields')
      return
    }
    setIsPlacing(true)
    try {
      await ordersApi.create(form)
      toast.success('Order placed!')
      onSuccess()
    } catch (err) {
      toast.error(err.message || 'Could not place your order')
    } finally {
      setIsPlacing(false)
    }
  }

  return (
    <Modal title="Checkout" onClose={onClose}>
      <form onSubmit={onSubmit} className="space-y-4" noValidate>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className="field-label-light" htmlFor="firstName">First name</label>
            <div className="relative">
              <User size={18} className="pointer-events-none absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-400" />
              <input id="firstName" name="firstName" className="field-input-light pl-11" value={form.firstName} onChange={onChange} />
            </div>
            {errors.firstName && <p className="mt-1.5 text-sm text-pulse-500">{errors.firstName}</p>}
          </div>
          <div>
            <label className="field-label-light" htmlFor="lastName">Last name</label>
            <input id="lastName" name="lastName" className="field-input-light" value={form.lastName} onChange={onChange} />
            {errors.lastName && <p className="mt-1.5 text-sm text-pulse-500">{errors.lastName}</p>}
          </div>
        </div>

        <div>
          <label className="field-label-light" htmlFor="street">Street address</label>
          <div className="relative">
            <MapPin size={18} className="pointer-events-none absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-400" />
            <input id="street" name="street" className="field-input-light pl-11" value={form.street} onChange={onChange} />
          </div>
          {errors.street && <p className="mt-1.5 text-sm text-pulse-500">{errors.street}</p>}
        </div>

        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className="field-label-light" htmlFor="city">City</label>
            <input id="city" name="city" className="field-input-light" value={form.city} onChange={onChange} />
            {errors.city && <p className="mt-1.5 text-sm text-pulse-500">{errors.city}</p>}
          </div>
          <div>
            <label className="field-label-light" htmlFor="country">Country</label>
            <input id="country" name="country" className="field-input-light" value={form.country} onChange={onChange} />
            {errors.country && <p className="mt-1.5 text-sm text-pulse-500">{errors.country}</p>}
          </div>
        </div>

        <div>
          <label className="field-label-light">Payment method</label>
          <div className="flex items-center gap-2.5 rounded-xl border border-vital-500 bg-vital-500/10 px-4 py-3 text-sm font-medium text-vital-700">
            <Banknote size={18} />
            Cash on Delivery
          </div>
        </div>

        <div className="flex justify-between border-t border-mist-200 pt-3 font-display text-base font-bold text-ink-900">
          <span>Total</span>
          <span>{formatEGP(total)}</span>
        </div>

        <button type="submit" disabled={isPlacing} className="btn-primary w-full">
          {isPlacing ? 'Placing order…' : 'Place Order'}
        </button>
      </form>
    </Modal>
  )
}
