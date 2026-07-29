import { useEffect, useState } from 'react'
import { HeartPulse, X } from 'lucide-react'
import StatusBadge from '../../components/common/StatusBadge'
import { useToast } from '../../context/ToastContext'
import { nursingApi } from '../../api/endpoints/nursing'
import ReviewModal from '../../components/common/ReviewModal'





export default function Nursing() {
  const toast = useToast()
  const [careType, setCareType] = useState('')
  const [nurseQuery, setNurseQuery] = useState('')
  const [nurses, setNurses] = useState([])
  const [selectedNurse, setSelectedNurse] = useState(null)
  const [requests, setRequests] = useState([])
  const [nursesById, setNursesById] = useState({})
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [reviewModalOpen, setReviewModalOpen] = useState(false)
  const [activeRequest, setActiveRequest] = useState(null)

  const loadRequests = () => {
    nursingApi
      .myRequests()
      .then(setRequests)
      .catch((err) => toast.error(err.message || 'Could not load your requests'))
  }

  useEffect(() => {
    loadRequests()
    nursingApi
      .searchNurses({ pageSize: 200 })
      .then((list) => setNursesById(Object.fromEntries((list || []).map((n) => [n.id, n]))))
      .catch(() => { })
    
  }, [])

  useEffect(() => {
    if (!nurseQuery) {
      setNurses([])
      return
    }
    const timeout = setTimeout(() => {
      nursingApi
        .searchNurses({ name: nurseQuery, pageSize: 10 })
        .then(setNurses)
        .catch(() => { })
    }, 300)
    return () => clearTimeout(timeout)
  }, [nurseQuery])

  const onSubmit = async (e) => {
    e.preventDefault()
    if (!careType || !selectedNurse) {
      toast.error('Please fill required fields')
      return
    }
    setIsSubmitting(true)
    try {
      await nursingApi.request({ nurseId: selectedNurse.id, careType })
      toast.success('Nursing request sent!')
      setCareType('')
      setSelectedNurse(null)
      setNurseQuery('')
      loadRequests()
    } catch (err) {
      toast.error(err.message || 'Could not send this request')
    } finally {
      setIsSubmitting(false)
    }
  }

  const handleReviewSubmit = async (data) => {
    try {
      await nursingApi.addReview(activeRequest.id, data)
      toast.success('Review submitted successfully!')
      loadRequests()
    } catch (err) {
      toast.error(err.message || 'Could not submit review')
      throw err 
    }
  }

  const dynamicCareTypes = Array.from(new Set(Object.values(nursesById).map(n => n.specialization).filter(Boolean)))

  return (
    <div className="space-y-6">
      <div>
        <h2 className="font-display text-2xl font-bold text-ink-900">Home Nursing Care</h2>
        <p className="mt-1 text-sm text-slate-500">Request a qualified nurse to visit you at home.</p>
      </div>

      <div className="grid gap-6 lg:grid-cols-3">
        <form onSubmit={onSubmit} className="card space-y-4 p-6 lg:col-span-1">
          <h3 className="font-display text-base font-semibold text-ink-900">New Request</h3>

          <div>
            <label className="field-label-light" htmlFor="careType">Care type</label>
            <select
              id="careType"
              className="field-input-light"
              value={careType}
              onChange={(e) => setCareType(e.target.value)}
            >
              <option value="">Select care type…</option>
              {dynamicCareTypes.map((t) => (
                <option key={t}>{t}</option>
              ))}
            </select>
          </div>

          <div>
            <label className="field-label-light" htmlFor="nurse">Nurse</label>
            {selectedNurse ? (
              <div className="flex items-center justify-between rounded-xl border border-mist-300 bg-mist-50 px-3.5 py-2.5">
                <div>
                  <p className="text-sm font-semibold text-ink-900">{selectedNurse.name}</p>
                  <p className="text-xs text-slate-500">{selectedNurse.specialization}</p>
                </div>
                <button type="button" onClick={() => setSelectedNurse(null)} className="text-slate-400 hover:text-red-600">
                  <X size={16} />
                </button>
              </div>
            ) : (
              <div className="relative">
                <input
                  id="nurse"
                  type="text"
                  placeholder="Search nurse by name…"
                  className="field-input-light"
                  value={nurseQuery}
                  onChange={(e) => setNurseQuery(e.target.value)}
                />
                {nurses.length > 0 && (
                  <div className="absolute z-10 mt-1 w-full overflow-hidden rounded-xl border border-mist-200 bg-white shadow-panel">
                    {nurses.map((n) => (
                      <button
                        key={n.id}
                        type="button"
                        onClick={() => {
                          setSelectedNurse(n)
                          setNurseQuery('')
                          setNurses([])
                        }}
                        className="flex w-full flex-col items-start px-3.5 py-2.5 text-left hover:bg-mist-50"
                      >
                        <span className="text-sm font-semibold text-ink-900">{n.name}</span>
                        <span className="text-xs text-slate-500">{n.specialization}</span>
                      </button>
                    ))}
                  </div>
                )}
              </div>
            )}
          </div>

          <button type="submit" disabled={isSubmitting} className="btn-primary w-full">
            <HeartPulse size={18} />
            {isSubmitting ? 'Sending…' : 'Submit Request'}
          </button>
        </form>

        {}
        <div className="space-y-3 lg:col-span-2">
          <h3 className="font-display text-base font-semibold text-ink-900">My Requests</h3>
          {requests.length === 0 && <p className="text-sm text-slate-500">No nursing requests yet.</p>}
          {requests.map((r, idx) => (
            <div key={`${r.nurseId}-${r.careType}-${idx}`} className="card flex flex-col gap-3 p-5 sm:flex-row sm:items-center sm:justify-between">
              <div>
                <p className="font-display text-sm font-semibold text-ink-900">{r.careType}</p>
                <p className="mt-1.5 text-xs text-vital-600">
                  Assigned to {nursesById[r.nurseId]?.name || `Nurse #${r.nurseId}`}
                </p>
              </div>
              <div className="flex items-center gap-4">
                <StatusBadge status={r.status} />
                {r.status === 'Completed' && !r.review && (
                  <button
                    onClick={() => {
                      setActiveRequest(r)
                      setReviewModalOpen(true)
                    }}
                    className="text-xs font-semibold text-vital-500 hover:text-vital-600"
                  >
                    Leave Review
                  </button>
                )}
                {r.review && (
                  <span className="text-xs font-medium text-slate-500">
                    Reviewed ★{r.review.rating}
                  </span>
                )}
              </div>
            </div>
          ))}
        </div>
      </div>

      <ReviewModal 
        isOpen={reviewModalOpen} 
        onClose={() => {
          setReviewModalOpen(false)
          setActiveRequest(null)
        }} 
        onSubmit={handleReviewSubmit}
        title={`Review Nurse ${activeRequest ? nursesById[activeRequest.nurseId]?.name?.replace('Nurse ', '') || '' : ''}`}
      />
    </div>
  )
}
