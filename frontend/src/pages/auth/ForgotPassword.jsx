import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Mail, ArrowRight, CheckCircle2 } from 'lucide-react'
import AuthLayout from '../../layouts/AuthLayout'
import { useToast } from '../../context/ToastContext'
import { authApi } from '../../api/endpoints/auth'

export default function ForgotPassword() {
  const toast = useToast()
  const [email, setEmail] = useState('')
  const [sent, setSent] = useState(false)
  const [isLoading, setIsLoading] = useState(false)

  const onSubmit = async (e) => {
    e.preventDefault()
    if (!email) {
      toast.error('Please fill required fields')
      return
    }
    setIsLoading(true)
    try {
      await authApi.forgetPassword(email)
      setSent(true)
      toast.success('Reset link sent!')
    } catch (err) {
      toast.error(err.message || 'Could not send the reset link.')
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <AuthLayout
      eyebrow="Account recovery"
      title="Reset your password"
      subtitle="We'll send a reset link to your email."
      footer={
        <Link to="/login" className="font-semibold text-vital-400 hover:text-vital-300">
          ← Back to log in
        </Link>
      }
    >
      {sent ? (
        <div className="flex flex-col items-center gap-3 py-4 text-center">
          <CheckCircle2 size={40} className="text-vital-400" />
          <p className="text-white">
            If an account exists for <span className="font-semibold">{email}</span>, a reset link is on its way.
          </p>
        </div>
      ) : (
        <form className="space-y-5" onSubmit={onSubmit} noValidate>
          <div>
            <label className="field-label" htmlFor="email">Email address</label>
            <div className="relative">
              <Mail size={18} className="pointer-events-none absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-400" />
              <input
                id="email"
                type="email"
                placeholder="you@example.com"
                className="field-input pl-11"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
              />
            </div>
          </div>
          <button type="submit" disabled={isLoading} className="btn-primary w-full">
            {isLoading ? 'Sending…' : 'Send reset link'}
            {!isLoading && <ArrowRight size={18} />}
          </button>
        </form>
      )}
    </AuthLayout>
  )
}
