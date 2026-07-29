import { useEffect, useRef, useState } from 'react'
import { Send, ArrowLeft } from 'lucide-react'
import StatusBadge from '../../../components/common/StatusBadge'
import { useToast } from '../../../context/ToastContext'
import { useAuth } from '../../../context/AuthContext'
import { consultationsApi } from '../../../api/endpoints/consultations'
import { createChatConnection } from '../../../api/signalr'
import { formatTime } from '../../../utils/format'

export default function DoctorChatRoom() {
  const toast = useToast()
  const { user } = useAuth()
  const [consultations, setConsultations] = useState([])
  const [activeId, setActiveId] = useState(null)
  const [messages, setMessages] = useState([])
  const [draft, setDraft] = useState('')
  const [showChatOnMobile, setShowChatOnMobile] = useState(false)
  const connectionRef = useRef(null)
  const chatContainerRef = useRef(null)

  const accepted = consultations.filter((c) => c.status === 'Accepted')
  const active = accepted.find((c) => c.id === activeId)

  useEffect(() => {
    consultationsApi
      .myConsultations()
      .then((list) => {
        setConsultations(list || [])
        const firstActive = (list || []).find((c) => c.status === 'Accepted')
        if (firstActive) setActiveId(firstActive.id)
      })
      .catch((err) => toast.error(err.message || 'Could not load consultations'))
    
  }, [])

  useEffect(() => {
    connectionRef.current?.stop()
    setMessages([])
    if (!active) return

    consultationsApi.getMessages(active.id).then(setMessages).catch(() => {})

    const connection = createChatConnection()
    connectionRef.current = connection
    connection.on('ReceiveMessage', (message) => {
      if (message.consultationId === active.id) setMessages((prev) => [...prev, message])
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

  const onEndConsultation = async () => {
    if (!window.confirm('Are you sure you want to end this consultation?')) return
    try {
      await consultationsApi.updateStatus(active.id, 'Completed')
      toast.success('Consultation ended successfully')
      setConsultations((prev) => prev.filter((c) => c.id !== active.id))
      setActiveId(null)
      setShowChatOnMobile(false)
    } catch (err) {
      toast.error(err.message || 'Could not end consultation')
    }
  }

  if (accepted.length === 0) {
    return (
      <div className="card flex h-[calc(100vh-11rem)] items-center justify-center">
        <p className="text-sm text-slate-500">No active consultations yet. Accept a request from your dashboard to start chatting.</p>
      </div>
    )
  }

  return (
    <div className="card grid h-[calc(100vh-11rem)] grid-cols-1 overflow-hidden lg:grid-cols-[320px_1fr]">
      <div className={`flex-col border-r border-mist-200 lg:flex ${showChatOnMobile ? 'hidden' : 'flex'}`}>
        <div className="border-b border-mist-200 p-4">
          <h2 className="font-display text-lg font-bold text-ink-900">Chat Room</h2>
        </div>
        <div className="flex-1 divide-y divide-mist-200 overflow-y-auto">
          {accepted.map((c) => (
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
                P{c.patientId}
              </div>
              <div className="min-w-0 flex-1">
                <p className="truncate font-display text-sm font-semibold text-ink-900">{c.patientName || `Patient #${c.patientId}`}</p>
                <div className="mt-1.5"><StatusBadge status={c.status} /></div>
              </div>
            </button>
          ))}
        </div>
      </div>

      <div className={`flex-col overflow-hidden lg:flex ${showChatOnMobile ? 'flex' : 'hidden'}`}>
        {active && (
          <>
            <div className="flex items-center gap-3 border-b border-mist-200 p-4">
              <button onClick={() => setShowChatOnMobile(false)} className="text-slate-500 lg:hidden">
                <ArrowLeft size={20} />
              </button>
              <div className="flex h-10 w-10 items-center justify-center rounded-full bg-ink-900 font-display text-sm font-bold text-vital-400">
                P{active.patientId}
              </div>
              <div className="flex-1">
                <p className="font-display text-sm font-semibold text-ink-900">{active.patientName || `Patient #${active.patientId}`}</p>
                <p className="text-xs text-slate-500">Patient</p>
              </div>
              <button onClick={onEndConsultation} className="btn-secondary !px-3 !py-1 text-xs text-pulse-600 hover:bg-pulse-50 hover:text-pulse-700 hover:border-pulse-200">
                End Chat
              </button>
            </div>

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
      </div>
    </div>
  )
}
