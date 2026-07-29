import { useEffect, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { Minus, Plus, X, ShoppingBag } from 'lucide-react'
import EmptyState from '../../components/common/EmptyState'
import CheckoutModal from '../../components/pharmacy/CheckoutModal'
import { useToast } from '../../context/ToastContext'
import { basketApi } from '../../api/endpoints/basket'
import { resolveMedicationImage } from '../../utils/images'
import { formatEGP } from '../../utils/format'

export default function Cart() {
  const toast = useToast()
  const navigate = useNavigate()
  const [basket, setBasket] = useState(null)
  const [showCheckout, setShowCheckout] = useState(false)

  const load = () => {
    basketApi
      .get()
      .then(setBasket)
      .catch((err) => toast.error(err.message || 'Could not load your cart'))
  }
  useEffect(load, [])

  const updateQty = async (medicationId, nextQty) => {
    if (nextQty < 1) return
    try {
      await basketApi.updateItem(medicationId, nextQty)
      load()
    } catch (err) {
      toast.error(err.message || 'Could not update quantity')
    }
  }

  const removeItem = async (medicationId) => {
    try {
      await basketApi.removeItem(medicationId)
      toast.info('Item removed from cart')
      load()
    } catch (err) {
      toast.error(err.message || 'Could not remove item')
    }
  }

  const onOrderPlaced = () => {
    setShowCheckout(false)
    navigate('/app/orders')
  }

  const items = basket?.items || []

  if (basket && items.length === 0) {
    return (
      <EmptyState
        icon={ShoppingBag}
        title="Your cart is empty"
        message="Add medications from the pharmacy to see them here."
        action={
          <Link to="/app/pharmacy" className="btn-primary">
            Browse Pharmacy
          </Link>
        }
      />
    )
  }

  return (
    <div className="grid gap-6 lg:grid-cols-3">
      <div className="space-y-3 lg:col-span-2">
        <h2 className="font-display text-2xl font-bold text-ink-900">Your Cart</h2>
        {!basket && <p className="text-sm text-slate-500">Loading…</p>}
        {items.map((item) => (
          <div key={item.medicationId} className="card flex items-center gap-4 p-4">
            <img
              src={resolveMedicationImage(item.pictureUrl)}
              alt={item.productName}
              className="h-16 w-16 shrink-0 rounded-lg object-cover"
            />
            <div className="min-w-0 flex-1">
              <p className="font-display text-sm font-semibold text-ink-900">{item.productName}</p>
              <p className="text-sm text-slate-500">{formatEGP(item.price)} each</p>
            </div>
            <div className="flex items-center gap-1.5 rounded-full border border-mist-200 px-1 py-1">
              <button
                onClick={() => updateQty(item.medicationId, item.quantity - 1)}
                className="flex h-7 w-7 items-center justify-center rounded-full text-slate-500 hover:bg-mist-100"
                aria-label="Decrease quantity"
              >
                <Minus size={14} />
              </button>
              <span className="w-5 text-center text-sm font-semibold">{item.quantity}</span>
              <button
                onClick={() => updateQty(item.medicationId, item.quantity + 1)}
                className="flex h-7 w-7 items-center justify-center rounded-full text-slate-500 hover:bg-mist-100"
                aria-label="Increase quantity"
              >
                <Plus size={14} />
              </button>
            </div>
            <p className="w-20 shrink-0 text-right font-display text-sm font-bold text-ink-900">
              {formatEGP(item.subTotal)}
            </p>
            <button
              onClick={() => removeItem(item.medicationId)}
              className="text-slate-400 hover:text-pulse-500"
              aria-label="Remove item"
            >
              <X size={18} />
            </button>
          </div>
        ))}
      </div>

      {}
      {basket && items.length > 0 && (
        <div className="card h-fit p-6">
          <h3 className="font-display text-lg font-bold text-ink-900">Order Summary</h3>
          <div className="mt-4 space-y-2.5 text-sm">
            <div className="flex justify-between text-slate-600">
              <span>Subtotal</span>
              <span>{formatEGP(basket.subTotal)}</span>
            </div>
            <div className="flex justify-between text-slate-600">
              <span>Delivery</span>
              <span>{formatEGP(basket.shippingPrice)}</span>
            </div>
            <div className="flex justify-between border-t border-mist-200 pt-2.5 font-display text-base font-bold text-ink-900">
              <span>Total</span>
              <span>{formatEGP(basket.total)}</span>
            </div>
          </div>
          <button onClick={() => setShowCheckout(true)} className="btn-primary mt-5 w-full">Proceed to Checkout</button>
        </div>
      )}

      {showCheckout && (
        <CheckoutModal total={basket.total} onClose={() => setShowCheckout(false)} onSuccess={onOrderPlaced} />
      )}
    </div>
  )
}
