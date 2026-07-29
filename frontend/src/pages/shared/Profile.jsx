import { useEffect, useState } from 'react'
import { User, Mail, Phone, MapPin, Save } from 'lucide-react'
import { useAuth } from '../../context/AuthContext'
import { useToast } from '../../context/ToastContext'
import { profileApi } from '../../api/endpoints/profile'






export default function Profile() {
  const { user } = useAuth()
  const toast = useToast()
  const [form, setForm] = useState(null)
  const [errors, setErrors] = useState({})
  const [isSaving, setIsSaving] = useState(false)

  useEffect(() => {
    profileApi
      .getMyProfile()
      .then((p) =>
        setForm({
          fullname: p.fullname || '',
          email: p.email || '',
          phone: p.phone || '',
          address: p.address || '',
          dateOfBirth: p.dateOfBirth ? p.dateOfBirth.slice(0, 10) : '',
          specialty: p.specialty || '',
          location: p.location || '',
          specialization: p.specialization || '',
          pharmacyName: p.pharmacyName || '',
        }),
      )
      .catch((err) => toast.error(err.message || 'Could not load your profile'))
    
  }, [])

  const onChange = (e) => setForm((f) => ({ ...f, [e.target.name]: e.target.value }))

  const validate = () => {
    const next = {}
    if (!form.fullname) next.fullname = 'Full name is required'
    if (!form.email) next.email = 'Email is required'
    else if (!/\S+@\S+\.\S+/.test(form.email)) next.email = 'Enter a valid email address'
    if (!form.phone) next.phone = 'Phone number is required'
    setErrors(next)
    return Object.keys(next).length === 0
  }

  const onSubmit = async (e) => {
    e.preventDefault()
    if (!validate()) {
      toast.error('Please fill required fields')
      return
    }
    setIsSaving(true)

    const payload = { ...form }
    if (!payload.dateOfBirth) payload.dateOfBirth = null

    try {
      await profileApi.updateMyProfile(payload)
      toast.success('Profile updated!')
    } catch (err) {
      toast.error(err.message || 'Could not update your profile')
    } finally {
      setIsSaving(false)
    }
  }

  if (!form) {
    return <p className="text-sm text-slate-500">Loading…</p>
  }

  const initials = (form.fullname || 'U').split(' ').map((p) => p[0]).slice(0, 2).join('')

  return (
    <div className="mx-auto max-w-2xl space-y-6">
      <div>
        <h2 className="font-display text-2xl font-bold text-ink-900">My Profile</h2>
        <p className="mt-1 text-sm text-slate-500">Manage your personal information.</p>
      </div>

      <div className="card p-6 sm:p-8">
        <div className="mb-6 flex items-center gap-4">
          <div className="flex h-16 w-16 items-center justify-center rounded-full bg-ink-900 font-display text-xl font-bold text-vital-400">
            {initials}
          </div>
          <div>
            <p className="font-display text-lg font-semibold text-ink-900">{form.fullname || 'Your name'}</p>
            <p className="text-sm text-slate-500">{user?.role} account</p>
          </div>
        </div>

        <form onSubmit={onSubmit} className="space-y-4" noValidate>
          <div>
            <label className="field-label-light" htmlFor="fullname">Full name</label>
            <div className="relative">
              <User size={18} className="pointer-events-none absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-400" />
              <input id="fullname" name="fullname" className="field-input-light pl-11" value={form.fullname} onChange={onChange} />
            </div>
            {errors.fullname && <p className="mt-1.5 text-sm text-pulse-500">{errors.fullname}</p>}
          </div>

          <div>
            <label className="field-label-light" htmlFor="email">Email address</label>
            <div className="relative">
              <Mail size={18} className="pointer-events-none absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-400" />
              <input id="email" name="email" type="email" className="field-input-light pl-11" value={form.email} onChange={onChange} />
            </div>
            {errors.email && <p className="mt-1.5 text-sm text-pulse-500">{errors.email}</p>}
          </div>

          <div>
            <label className="field-label-light" htmlFor="phone">Phone number</label>
            <div className="relative">
              <Phone size={18} className="pointer-events-none absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-400" />
              <input id="phone" name="phone" type="tel" className="field-input-light pl-11" value={form.phone} onChange={onChange} />
            </div>
            {errors.phone && <p className="mt-1.5 text-sm text-pulse-500">{errors.phone}</p>}
          </div>

          {user?.role === 'Patient' && (
            <>
              <div>
                <label className="field-label-light" htmlFor="address">Address</label>
                <div className="relative">
                  <MapPin size={18} className="pointer-events-none absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-400" />
                  <input id="address" name="address" className="field-input-light pl-11" value={form.address} onChange={onChange} />
                </div>
              </div>
              <div>
                <label className="field-label-light" htmlFor="dateOfBirth">Date of birth</label>
                <input id="dateOfBirth" name="dateOfBirth" type="date" className="field-input-light" value={form.dateOfBirth} onChange={onChange} />
              </div>
            </>
          )}

          {user?.role === 'Doctor' && (
            <>
              <div>
                <label className="field-label-light" htmlFor="specialty">Specialty</label>
                <input id="specialty" name="specialty" className="field-input-light" value={form.specialty} onChange={onChange} />
              </div>
              <div>
                <label className="field-label-light" htmlFor="location">Location</label>
                <input id="location" name="location" className="field-input-light" value={form.location} onChange={onChange} />
              </div>
            </>
          )}

          {user?.role === 'Nurse' && (
            <div>
              <label className="field-label-light" htmlFor="specialization">Specialization</label>
              <input id="specialization" name="specialization" className="field-input-light" value={form.specialization} onChange={onChange} />
            </div>
          )}

          <button type="submit" disabled={isSaving} className="btn-primary">
            <Save size={18} />
            {isSaving ? 'Saving…' : 'Save changes'}
          </button>
        </form>
      </div>
    </div>
  )
}
