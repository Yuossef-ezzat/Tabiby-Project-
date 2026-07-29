import { useEffect, useState } from 'react'
import { UserPlus, Search, Trash2, X, Power } from 'lucide-react'
import StatusBadge from '../../../components/common/StatusBadge'
import { useToast } from '../../../context/ToastContext'
import { adminApi } from '../../../api/endpoints/admin'

const ROLES = ['Doctor', 'Nurse', 'Pharmacist']
const emptyForm = {
  role: 'Doctor',
  fullName: '',
  email: '',
  phone: '',
  password: '',
  specialty: '',
  location: '',
  specialization: '',
  pharmacyName: '',
}

// NOTE: the backend has no "license ID" field and, more importantly, no
// suspend/activate toggle for accounts — AdminUserDto has an IsActive flag
// you can filter by, but the only account-level action exposed is a hard
// DELETE. So this page creates per-role accounts and can delete them, but
// can't suspend/reactivate.
export default function AdminAccounts() {
  const toast = useToast()
  const [accounts, setAccounts] = useState([])
  const [query, setQuery] = useState('')
  const [showForm, setShowForm] = useState(false)
  const [form, setForm] = useState(emptyForm)
  const [isSaving, setIsSaving] = useState(false)
  const [isLoading, setIsLoading] = useState(true)

  const load = () => {
    adminApi
      .getUsers({ name: query || undefined, pageSize: 100 })
      .then((res) => setAccounts(res.users || []))
      .catch((err) => toast.error(err.message || 'Could not load accounts'))
      .finally(() => setIsLoading(false))
  }

  useEffect(() => {
    const t = setTimeout(load, 250)
    return () => clearTimeout(t)
    
  }, [query])

  const onChange = (e) => setForm((f) => ({ ...f, [e.target.name]: e.target.value }))

  const onCreate = async (e) => {
    e.preventDefault()
    if (!form.fullName || !form.email || !form.password) {
      toast.error('Please fill required fields')
      return
    }
    setIsSaving(true)
    try {
      if (form.role === 'Doctor') await adminApi.createDoctor(form)
      else if (form.role === 'Nurse') await adminApi.createNurse(form)
      else await adminApi.createPharmacist(form)
      toast.success('Account created!')
      setForm(emptyForm)
      setShowForm(false)
      load()
    } catch (err) {
      toast.error(err.message || 'Could not create this account')
    } finally {
      setIsSaving(false)
    }
  }

  const onDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this account?')) return;
    try {
      await adminApi.deleteUser(id)
      toast.success('Account deleted')
      load()
    } catch (err) {
      toast.error(err.message || 'Could not delete this account')
    }
  }

  const onToggleActive = async (id) => {
    try {
      await adminApi.toggleUserActive(id)
      toast.success('Account status updated')
      load()
    } catch (err) {
      toast.error(err.message || 'Could not update status')
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h2 className="font-display text-2xl font-bold text-ink-900">Professional Accounts</h2>
          <p className="mt-1 text-sm text-slate-500">Manage doctors, pharmacists, and nurses.</p>
        </div>
        <button onClick={() => setShowForm((s) => !s)} className="btn-primary text-sm">
          {showForm ? <X size={16} /> : <UserPlus size={16} />}
          {showForm ? 'Cancel' : 'Create Account'}
        </button>
      </div>

      {showForm && (
        <form onSubmit={onCreate} className="card space-y-4 p-6">
          <h3 className="font-display text-base font-semibold text-ink-900">New Professional Account</h3>
          <div className="grid gap-4 sm:grid-cols-2">
            <select name="role" className="field-input-light" value={form.role} onChange={onChange}>
              {ROLES.map((r) => (
                <option key={r}>{r}</option>
              ))}
            </select>
            <input name="fullName" placeholder="Full Name *" className="field-input-light" value={form.fullName} onChange={onChange} />
            <input name="email" type="email" placeholder="Email *" className="field-input-light" value={form.email} onChange={onChange} />
            <input name="phone" placeholder="Phone" className="field-input-light" value={form.phone} onChange={onChange} />
            <input name="password" type="password" placeholder="Temporary Password *" className="field-input-light" value={form.password} onChange={onChange} />

            {form.role === 'Doctor' && (
              <>
                <input name="specialty" placeholder="Specialty *" className="field-input-light" value={form.specialty} onChange={onChange} />
                <input name="location" placeholder="Location" className="field-input-light" value={form.location} onChange={onChange} />
              </>
            )}
            {form.role === 'Nurse' && (
              <input name="specialization" placeholder="Specialization *" className="field-input-light" value={form.specialization} onChange={onChange} />
            )}

          </div>
          <div className="flex gap-2">
            <button type="submit" disabled={isSaving} className="btn-primary">
              {isSaving ? 'Creating…' : 'Create Account'}
            </button>
            <button type="button" onClick={() => setShowForm(false)} className="btn-secondary">Cancel</button>
          </div>
        </form>
      )}

      <div className="relative">
        <Search size={18} className="pointer-events-none absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" />
        <input
          type="search"
          placeholder="Search by name…"
          className="field-input-light pl-11"
          value={query}
          onChange={(e) => setQuery(e.target.value)}
        />
      </div>

      <div className="card overflow-x-auto">
        <table className="w-full min-w-[640px] text-left text-sm">
          <thead>
            <tr className="border-b border-mist-200 text-xs uppercase tracking-wide text-slate-400">
              <th className="px-5 py-3.5 font-semibold">Name</th>
              <th className="px-5 py-3.5 font-semibold">Email</th>
              <th className="px-5 py-3.5 font-semibold">Role</th>
              <th className="px-5 py-3.5 font-semibold">Status</th>
              <th className="px-5 py-3.5 font-semibold text-right">Action</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-mist-200">
            {isLoading && (
              <tr><td colSpan={6} className="px-5 py-6 text-center text-slate-500">Loading…</td></tr>
            )}
            {!isLoading && accounts.length === 0 && (
              <tr><td colSpan={6} className="px-5 py-6 text-center text-slate-500">No accounts found.</td></tr>
            )}
            {accounts.map((a) => (
              <tr key={a.id}>
                <td className="px-5 py-4 font-display font-semibold text-ink-900">{a.fullName}</td>
                <td className="px-5 py-4 text-slate-500">{a.email}</td>
                <td className="px-5 py-4">
                  <span className="badge bg-vital-500/10 text-vital-700">{a.userType}</span>
                </td>
                <td className="px-5 py-4"><StatusBadge status={a.isActive ? 'Active' : 'Inactive'} /></td>
                <td className="px-5 py-4 text-right">
                  <div className="flex justify-end gap-2">
                    <button
                      onClick={() => onToggleActive(a.id)}
                      className={`rounded-full p-2 transition-colors ${
                        a.isActive 
                          ? 'bg-amber-500/10 text-amber-600 hover:bg-amber-500/20' 
                          : 'bg-emerald-500/10 text-emerald-600 hover:bg-emerald-500/20'
                      }`}
                      title={a.isActive ? "Deactivate Account" : "Activate Account"}
                    >
                      <Power size={16} />
                    </button>
                    <button
                      onClick={() => onDelete(a.id)}
                      className="rounded-full bg-pulse-500/10 p-2 text-pulse-600 hover:bg-pulse-500/20 transition-colors"
                      title="Delete Account"
                    >
                      <Trash2 size={16} />
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}
