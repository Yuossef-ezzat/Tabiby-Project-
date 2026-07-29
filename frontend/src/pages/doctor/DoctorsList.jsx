import { useEffect, useMemo, useState } from 'react'
import { Search } from 'lucide-react'
import DoctorCard from '../../components/doctors/DoctorCard'
import EmptyState from '../../components/common/EmptyState'
import { useToast } from '../../context/ToastContext'
import { doctorsApi } from '../../api/endpoints/doctors'

export default function DoctorsList() {
  const toast = useToast()
  const [query, setQuery] = useState('')
  const [specialty, setSpecialty] = useState('All Specialties')
  const [doctors, setDoctors] = useState([])
  const [isLoading, setIsLoading] = useState(true)

  const [dynamicSpecialties, setDynamicSpecialties] = useState([])

  useEffect(() => {
    
    doctorsApi.search({ pageSize: 200 }).then(data => {
      const unique = Array.from(new Set((data || []).map(d => d.specialty).filter(Boolean)))
      setDynamicSpecialties(unique)
    }).catch(() => {})
  }, [])

  useEffect(() => {
    let cancelled = false
    setIsLoading(true)
    doctorsApi
      .search({
        name: query || undefined,
        specialization: specialty === 'All Specialties' ? undefined : specialty,
        pageSize: 50,
      })
      .then((data) => {
        if (!cancelled) setDoctors(data || [])
      })
      .catch((err) => !cancelled && toast.error(err.message || 'Could not load doctors'))
      .finally(() => !cancelled && setIsLoading(false))
    return () => {
      cancelled = true
    }
    
  }, [query, specialty])

  const filtered = useMemo(() => doctors, [doctors])

  return (
    <div className="space-y-6">
      <div>
        <h2 className="font-display text-2xl font-bold text-ink-900">Find a Doctor</h2>
        <p className="mt-1 text-sm text-slate-500">Search specialists and book an appointment in minutes.</p>
      </div>

      <div className="flex flex-col gap-3 sm:flex-row">
        <div className="relative flex-1">
          <Search size={18} className="pointer-events-none absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" />
          <input
            type="search"
            placeholder="Search doctors by name…"
            className="field-input-light pl-11"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
          />
        </div>
        <select
          className="field-input-light sm:w-56"
          value={specialty}
          onChange={(e) => setSpecialty(e.target.value)}
        >
          <option>All Specialties</option>
          {dynamicSpecialties.map((s) => (
            <option key={s}>{s}</option>
          ))}
        </select>
      </div>

      {isLoading ? (
        <p className="text-sm text-slate-500">Loading doctors…</p>
      ) : filtered.length === 0 ? (
        <EmptyState
          icon={Search}
          title="No doctors found"
          message="Try a different name, specialty, or clear your filters."
        />
      ) : (
        <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-3">
          {filtered.map((doctor) => (
            <DoctorCard key={doctor.id} doctor={doctor} />
          ))}
        </div>
      )}
    </div>
  )
}
