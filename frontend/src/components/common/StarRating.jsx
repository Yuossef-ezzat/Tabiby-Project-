import { Star } from 'lucide-react'

export default function StarRating({ rating, reviews, size = 14 }) {
  return (
    <div className="flex items-center gap-1 text-sm">
      <Star size={size} className="fill-amber-400 text-amber-400" />
      <span className="font-semibold text-slate-800">{rating.toFixed(1)}</span>
      {reviews != null && <span className="text-slate-400">({reviews})</span>}
    </div>
  )
}
