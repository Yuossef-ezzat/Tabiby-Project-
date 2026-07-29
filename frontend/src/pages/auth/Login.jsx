import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { Mail, Lock, Eye, EyeOff, ArrowRight } from 'lucide-react'
import AuthLayout from '../../layouts/AuthLayout'
import { useAuth, ROLE_HOME_ROUTE } from '../../context/AuthContext'
import { useToast } from '../../context/ToastContext'

export default function Login() {
  const navigate = useNavigate()
  const { login, isLoading } = useAuth()
  const toast = useToast()
  const [showPassword, setShowPassword] = useState(false)
  const [form, setForm] = useState({ email: '', password: '' })
  const [errors, setErrors] = useState({})

  const onChange = (e) => setForm((f) => ({ ...f, [e.target.name]: e.target.value }))

  const validate = () => {
    const next = {}
    if (!form.email) next.email = 'Email is required'
    else if (!/\S+@\S+\.\S+/.test(form.email)) next.email = 'Enter a valid email address'
    if (!form.password) next.password = 'Password is required'
    setErrors(next)
    return Object.keys(next).length === 0
  }

  const onSubmit = async (e) => {
    e.preventDefault()
    if (!validate()) {
      toast.error('Please fill required fields')
      return
    }
    try {
      const loggedInUser = await login(form)
      toast.success('Welcome back!')
      navigate(ROLE_HOME_ROUTE[loggedInUser.role] || '/app/dashboard')
    } catch (err) {
      toast.error(err.message || 'Could not log in. Please try again.')
    }
  }

  return (
    <AuthLayout
      eyebrow="Welcome back"
      title="Log in to your account"
      subtitle="Enter your details to continue your care journey."
      footer={
        <>
          New to Tabiby?{' '}
          <Link to="/register" className="font-semibold text-vital-400 hover:text-vital-300">
            Create an account
          </Link>
        </>
      }
    >
      <form className="space-y-5" onSubmit={onSubmit} noValidate>
        <div>
          <label className="field-label" htmlFor="email">Email address</label>
          <div className="relative">
            <Mail size={18} className="pointer-events-none absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-400" />
            <input
              id="email"
              name="email"
              type="email"
              autoComplete="email"
              placeholder="you@example.com"
              className="field-input pl-11"
              value={form.email}
              onChange={onChange}
            />
          </div>
          {errors.email && <p className="mt-1.5 text-sm text-pulse-400">{errors.email}</p>}
        </div>

        <div>
          <div className="flex items-center justify-between">
            <label className="field-label" htmlFor="password">Password</label>
            <Link to="/forgot-password" className="mb-1.5 text-sm font-medium text-vital-400 hover:text-vital-300">
              Forgot password?
            </Link>
          </div>
          <div className="relative">
            <Lock size={18} className="pointer-events-none absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-400" />
            <input
              id="password"
              name="password"
              type={showPassword ? 'text' : 'password'}
              autoComplete="current-password"
              placeholder="••••••••"
              className="field-input pl-11 pr-11"
              value={form.password}
              onChange={onChange}
            />
            <button
              type="button"
              onClick={() => setShowPassword((s) => !s)}
              className="absolute right-3.5 top-1/2 -translate-y-1/2 text-slate-400 hover:text-white"
              aria-label={showPassword ? 'Hide password' : 'Show password'}
            >
              {showPassword ? <EyeOff size={18} /> : <Eye size={18} />}
            </button>
          </div>
          {errors.password && <p className="mt-1.5 text-sm text-pulse-400">{errors.password}</p>}
        </div>

        <label className="flex select-none items-center gap-2 text-sm text-slate-300">
          <input type="checkbox" className="h-4 w-4 rounded border-white/20 bg-white/5 text-vital-500 focus:ring-vital-500" />
          Keep me signed in
        </label>

        <button type="submit" disabled={isLoading} className="btn-primary w-full">
          {isLoading ? 'Signing in…' : 'Log in'}
          {!isLoading && <ArrowRight size={18} />}
        </button>
      </form>
    </AuthLayout>
  )
}
