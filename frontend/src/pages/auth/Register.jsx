import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { User, Mail, Lock, Phone, Eye, EyeOff, ArrowRight } from 'lucide-react'
import AuthLayout from '../../layouts/AuthLayout'
import { useAuth } from '../../context/AuthContext'
import { useToast } from '../../context/ToastContext'

export default function Register() {
  const navigate = useNavigate()
  const { register, isLoading } = useAuth()
  const toast = useToast()
  const [showPassword, setShowPassword] = useState(false)
  const [form, setForm] = useState({ fullName: '', email: '', phone: '', password: '' })
  const [errors, setErrors] = useState({})

  const onChange = (e) => setForm((f) => ({ ...f, [e.target.name]: e.target.value }))

  const validate = () => {
    const next = {}
    if (!form.fullName) next.fullName = 'Full name is required'
    if (!form.email) next.email = 'Email is required'
    else if (!/\S+@\S+\.\S+/.test(form.email)) next.email = 'Enter a valid email address'
    if (!form.phone) next.phone = 'Phone number is required'
    if (!form.password) next.password = 'Password is required'
    else if (form.password.length < 6) next.password = 'At least 6 characters'
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
      await register(form)
      toast.success('Account created!')
      navigate('/app/dashboard')
    } catch (err) {
      toast.error(err.message || 'Could not create your account. Please try again.')
    }
  }

  return (
    <AuthLayout
      eyebrow="Get started"
      title="Create your patient account"
      subtitle="Takes less than a minute — no paperwork."
      footer={
        <>
          Already have an account?{' '}
          <Link to="/login" className="font-semibold text-vital-400 hover:text-vital-300">
            Log in
          </Link>
        </>
      }
    >
      <form className="space-y-5" onSubmit={onSubmit} noValidate>
        <div>
          <label className="field-label" htmlFor="fullName">Full name</label>
          <div className="relative">
            <User size={18} className="pointer-events-none absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-400" />
            <input
              id="fullName"
              name="fullName"
              type="text"
              autoComplete="name"
              placeholder="Salma Ahmed"
              className="field-input pl-11"
              value={form.fullName}
              onChange={onChange}
            />
          </div>
          {errors.fullName && <p className="mt-1.5 text-sm text-pulse-400">{errors.fullName}</p>}
        </div>

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
          <label className="field-label" htmlFor="phone">Phone number</label>
          <div className="relative">
            <Phone size={18} className="pointer-events-none absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-400" />
            <input
              id="phone"
              name="phone"
              type="tel"
              autoComplete="tel"
              placeholder="01012345678"
              className="field-input pl-11"
              value={form.phone}
              onChange={onChange}
            />
          </div>
          {errors.phone && <p className="mt-1.5 text-sm text-pulse-400">{errors.phone}</p>}
        </div>

        <div>
          <label className="field-label" htmlFor="password">Password</label>
          <div className="relative">
            <Lock size={18} className="pointer-events-none absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-400" />
            <input
              id="password"
              name="password"
              type={showPassword ? 'text' : 'password'}
              autoComplete="new-password"
              placeholder="At least 6 characters"
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

        <p className="text-xs leading-relaxed text-slate-400">
          By creating an account you agree to Tabiby's Terms of Service and
          acknowledge our Privacy Policy.
        </p>

        <button type="submit" disabled={isLoading} className="btn-primary w-full">
          {isLoading ? 'Creating account…' : 'Create account'}
          {!isLoading && <ArrowRight size={18} />}
        </button>
      </form>
    </AuthLayout>
  )
}
