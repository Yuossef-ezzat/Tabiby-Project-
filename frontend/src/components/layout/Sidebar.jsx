import { NavLink } from 'react-router-dom'
import {
  LayoutGrid,
  Stethoscope,
  CalendarDays,
  CalendarClock,
  MessageCircle,
  Pill,
  Boxes,
  ShoppingBag,
  HeartPulse,
  Bell,
  UserCog,
  UserRound,
  LogOut,
  X,
} from 'lucide-react'
import Logo from '../common/Logo'
import { useAuth } from '../../context/AuthContext'

const NAV_BY_ROLE = {
  Patient: [
    { to: '/app/dashboard', label: 'Dashboard', icon: LayoutGrid },
    { to: '/app/doctors', label: 'Find Doctors', icon: Stethoscope },
    { to: '/app/appointments', label: 'Appointments', icon: CalendarDays },
    { to: '/app/consultations', label: 'Consultations', icon: MessageCircle },
    { to: '/app/pharmacy', label: 'Pharmacy', icon: Pill },
    { to: '/app/orders', label: 'Orders', icon: ShoppingBag },
    { to: '/app/nursing', label: 'Nursing Care', icon: HeartPulse },
    { to: '/app/notifications', label: 'Notifications', icon: Bell },
    { to: '/app/profile', label: 'Profile', icon: UserRound },
  ],
  Doctor: [
    { to: '/app/doctor/dashboard', label: 'Dashboard', icon: LayoutGrid },
    { to: '/app/doctor/schedule', label: 'Schedule', icon: CalendarClock },
    { to: '/app/doctor/appointments', label: 'Appointments', icon: CalendarDays },
    { to: '/app/doctor/chat', label: 'Chat Room', icon: MessageCircle },
    { to: '/app/notifications', label: 'Notifications', icon: Bell },
    { to: '/app/profile', label: 'Profile', icon: UserRound },
  ],
  Pharmacist: [
    { to: '/app/pharmacist/inventory', label: 'Inventory', icon: Boxes },
    { to: '/app/pharmacist/orders', label: 'Orders', icon: ShoppingBag },
    { to: '/app/profile', label: 'Profile', icon: UserRound },
  ],
  Nurse: [
    { to: '/app/nurse/requests', label: 'Requests', icon: HeartPulse },
    { to: '/app/notifications', label: 'Notifications', icon: Bell },
    { to: '/app/profile', label: 'Profile', icon: UserRound },
  ],
  
  Admin: [{ to: '/app/admin/accounts', label: 'Professional Accounts', icon: UserCog }],
}

export default function Sidebar({ open, onClose }) {
  const { user, logout } = useAuth()
  const navItems = NAV_BY_ROLE[user?.role] || NAV_BY_ROLE.Patient

  return (
    <>
      {}
      {open && (
        <div
          className="fixed inset-0 z-30 bg-ink-950/60 backdrop-blur-sm lg:hidden"
          onClick={onClose}
        />
      )}

      <aside
        className={`fixed inset-y-0 left-0 z-40 flex w-72 flex-col bg-ink-900 transition-transform duration-300 lg:sticky lg:top-0 lg:z-0 lg:h-screen lg:translate-x-0
        ${open ? 'translate-x-0' : '-translate-x-full'}`}
      >
        <div className="flex items-center justify-between px-6 pt-7 pb-1">
          <Logo variant="light" />
          <button onClick={onClose} className="text-slate-400 hover:text-white lg:hidden" aria-label="Close menu">
            <X size={20} />
          </button>
        </div>
        <p className="px-6 pb-5 text-xs font-semibold uppercase tracking-[0.2em] text-vital-400">
          {user?.role || 'Patient'} Portal
        </p>

        <nav className="flex-1 space-y-1 overflow-y-auto px-4 py-2">
          {navItems.map(({ to, label, icon: Icon }) => (
            <NavLink
              key={to}
              to={to}
              onClick={onClose}
              className={({ isActive }) =>
                `flex items-center gap-3 rounded-xl px-3.5 py-2.5 text-sm font-medium transition-colors duration-150 ${
                  isActive
                    ? 'bg-vital-500/15 text-vital-300'
                    : 'text-slate-300 hover:bg-white/5 hover:text-white'
                }`
              }
            >
              <Icon size={19} />
              {label}
            </NavLink>
          ))}
        </nav>

        <div className="border-t border-white/10 px-4 py-4">
          <button
            onClick={logout}
            className="flex w-full items-center gap-3 rounded-xl px-3.5 py-2.5 text-sm font-medium text-slate-300 transition-colors duration-150 hover:bg-pulse-500/10 hover:text-pulse-400"
          >
            <LogOut size={19} />
            Log out
          </button>
        </div>
      </aside>
    </>
  )
}
