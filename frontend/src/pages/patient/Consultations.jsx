import { useEffect, useMemo, useRef, useState } from 'react'
import { Send, ArrowLeft } from 'lucide-react'
import StatusBadge from '../../components/common/StatusBadge'
import { useToast } from '../../context/ToastContext'
import { useAuth } from '../../context/AuthContext'
import { consultationsApi } from '../../api/endpoints/consultations'
import { doctorsApi } from '../../api/endpoints/doctors'
import { createChatConnection } from '../../api/signalr'
import { formatTime } from '../../utils/format'
import ReviewModal from '../../components/common/ReviewModal'

export default function Consultations() {
  const toast = useToast()
  const { user } = useAuth()
  const [consultations, setConsultations] = useState([])
  const [doctorsById, setDoctorsById] = useState({})
  const [activeId, setActiveId] = useState(null)
  const [messages, setMessages] = useState([])
  const [draft, setDraft] = useState('')
  const [showChatOnMobile, setShowChatOnMobile] = useState(false)
  const [reviewModalOpen, setReviewModalOpen] = useState(false)
  const connectionRef = useRef(null)
  const chatContainerRef = useRef(null)

  const active = consultations.find((c) => c.id === activeId)
  const activeDoctor = active ? doctorsById[active.doctorId] : null

  useEffect(() => {
    Promise.all([consultationsApi.myConsultations(), doctorsApi.search({ pageSize: 200 })])
      .then(([list, doctors]) => {
        setConsultations(list || [])
        setDoctorsById(Object.fromEntries((doctors || []).map((d) => [d.id, d])))
        if (list?.length) setActiveId(list[0].id)
      })
      .catch((err) => toast.error(err.message || 'Could not load consultations'))
    
  }, [])

  
  
  useEffect(() => {
    connectionRef.current?.stop()
    setMessages([])
    if (!active || active.status !== 'Accepted') return

    consultationsApi
      .getMessages(active.id)
      .then(setMessages)
      .catch((err) => toast.error(err.message || 'Could not load messages'))

    const connection = createChatConnection()
    connectionRef.current = connection
    connection.on('ReceiveMessage', (message) => {
      if (message.consultationId === active.id) {
        setMessages((prev) => [...prev, message])
      }
    })
    connection
      .start()
      .then(() => connection.invoke('JoinConsultation', active.id))
      .catch(() => toast.error('Could not connect to live chat'))

    return () => connection.stop()
    
  }, [activeId])

  useEffect(() => {
    if (chatContainerRef.current) {
      chatContainerRef.current.scrollTop = chatContainerRef.current.scrollHeight
    }
  }, [messages])

  const onSend = async (e) => {
    e.preventDefault()
    if (!draft.trim()) {
      toast.error('Please enter a message')
      return
    }
    try {
      await consultationsApi.sendMessage(active.id, draft.trim())
      setDraft('')
    } catch (err) {
      toast.error(err.message || 'Could not send message')
    }
  }

  const handleReviewSubmit = async (data) => {
    try {
      await consultationsApi.addReview(active.id, data)
      toast.success('Review submitted successfully!')
      consultationsApi.myConsultations().then(list => setConsultations(list || []))
    } catch (err) {
      toast.error(err.message || 'Could not submit review')
      throw err 
    }
  }

  const list = useMemo(
    () =>
      consultations.map((c) => ({
        ...c,
        doctorName: doctorsById[c.doctorId]?.name || c.doctorName || `Doctor #${c.doctorId}`,
        specialty: doctorsById[c.doctorId]?.specialty || '',
      })),
    [consultations, doctorsById],
  )

  if (consultations.length === 0) {
    return (
      <div className="card flex h-[calc(100vh-11rem)] items-center justify-center">
        <p className="text-sm text-slate-500">No consultations yet. Start one from a doctor's profile.</p>
      </div>
    )
  }

  return (
    <div className="card grid h-[calc(100vh-11rem)] grid-cols-1 overflow-hidden lg:grid-cols-[320px_1fr]">
      {}
      <div className={`flex-col border-r border-mist-200 lg:flex ${showChatOnMobile ? 'hidden' : 'flex'}`}>
        <div className="border-b border-mist-200 p-4">
          <h2 className="font-display text-lg font-bold text-ink-900">Consultations</h2>
        </div>
        <div className="flex-1 divide-y divide-mist-200 overflow-y-auto">
          {list.map((c) => {
            const initials = c.doctorName.replace('Dr. ', '').split(' ').map((p) => p[0]).slice(0, 2).join('')
            return (
              <button
                key={c.id}
                onClick={() => {
                  setActiveId(c.id)
                  setShowChatOnMobile(true)
                }}
                className={`flex w-full items-start gap-3 p-4 text-left transition-colors hover:bg-mist-50 ${
                  activeId === c.id ? 'bg-vital-500/5' : ''
                }`}
              >
                <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-full bg-ink-900 font-display text-sm font-bold text-vital-400">
                  {initials}
                </div>
                <div className="min-w-0 flex-1">
                  <p className="truncate font-display text-sm font-semibold text-ink-900">{c.doctorName}</p>
                  <p className="truncate text-xs text-slate-500">{c.specialty}</p>
                  <div className="mt-1.5">
                    <StatusBadge status={c.status} />
                  </div>
                </div>
              </button>
            )
          })}
        </div>
      </div>

      {/* Chat window */}
      <div className={`flex-col overflow-hidden lg:flex ${showChatOnMobile ? 'flex' : 'hidden'}`}>
        {active && (
          <>
            <div className="flex items-center gap-3 border-b border-mist-200 p-4">
              <button onClick={() => setShowChatOnMobile(false)} className="text-slate-500 lg:hidden">
                <ArrowLeft size={20} />
              </button>
              <div className="flex h-10 w-10 items-center justify-center rounded-full bg-ink-900 font-display text-sm font-bold text-vital-400">
                {(activeDoctor?.name || '?').replace('Dr. ', '').split(' ').map((p) => p[0]).slice(0, 2).join('')}
              </div>
              <div>
                <p className="font-display text-sm font-semibold text-ink-900">{activeDoctor?.name || active.doctorName || `Doctor #${active.doctorId}`}</p>
                <p className="text-xs text-slate-500">{activeDoctor?.specialty}</p>
              </div>
              <StatusBadge status={active.status} />
            </div>

            {active.status !== 'Accepted' ? (
              <div className="flex flex-1 flex-col items-center justify-center bg-mist-50 p-5 text-center">
                <p className="text-sm text-slate-500 mb-4">
                  {active.status === 'Pending'
                    ? 'Chat opens once the doctor accepts your consultation request.'
                    : `This consultation is ${active.status.toLowerCase()}.`}
                </p>
                {active.status === 'Completed' && !active.review && (
                  <button 
                    onClick={() => setReviewModalOpen(true)}
                    className="btn-primary"
                  >
                    Leave a Review
                  </button>
                )}
                {active.review && (
                  <div className="mt-4 rounded-lg bg-white p-4 text-left shadow-sm w-full max-w-sm">
                    <div className="flex items-center justify-between">
                      <span className="font-semibold text-ink-900">Your Review</span>
                      <span className="font-medium text-vital-600">★ {active.review.rating}/5</span>
                    </div>
                    <p className="mt-2 text-slate-600 text-sm">{active.review.comment}</p>
                  </div>
                )}
              </div>
            ) : (
              <>
                <div ref={chatContainerRef} className="flex-1 space-y-3 overflow-y-auto bg-mist-50 p-5">
                  {messages.map((m) => {
                    const mine = m.senderUserId === user?.id
                    return (
                      <div key={m.id} className={`flex ${mine ? 'justify-end' : 'justify-start'}`}>
                        <div
                          className={`max-w-[75%] rounded-2xl px-4 py-2.5 text-sm ${
                            mine ? 'rounded-br-sm bg-ink-900 text-white' : 'rounded-bl-sm bg-white text-slate-700 shadow-card'
                          }`}
                        >
                          <p>{m.content}</p>
                          <p className={`mt-1 text-[11px] ${mine ? 'text-slate-300' : 'text-slate-400'}`}>
                            {formatTime(new Date(m.createdAt).toTimeString().slice(0, 5))}
                          </p>
                        </div>
                      </div>
                    )
                  })}
                </div>

                <form onSubmit={onSend} className="flex items-center gap-2 border-t border-mist-200 p-4">
                  <input
                    type="text"
                    value={draft}
                    onChange={(e) => setDraft(e.target.value)}
                    placeholder="Type a message…"
                    className="field-input-light flex-1"
                  />
                  <button type="submit" className="btn-primary !px-4">
                    <Send size={18} />
                  </button>
                </form>
              </>
            )}
          </>
        )}
      </div>

      <ReviewModal 
        isOpen={reviewModalOpen} 
        onClose={() => setReviewModalOpen(false)} 
        onSubmit={handleReviewSubmit}
        title={`Review Dr. ${activeDoctor?.name?.replace('Dr. ', '') || ''}`}
      />
    </div>
  )
}
