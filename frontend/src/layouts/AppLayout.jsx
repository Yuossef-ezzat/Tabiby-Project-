import { useState } from 'react'
import { Outlet, useLocation } from 'react-router-dom'
import Sidebar from '../components/layout/Sidebar'
import Topbar from '../components/layout/Topbar'

const TITLES = {
  
  '/app/dashboard': 'Dashboard',
  '/app/doctors': 'Find Doctors',
  '/app/appointments': 'Appointments',
  '/app/consultations': 'Consultations',
  '/app/pharmacy': 'Pharmacy',
  '/app/cart': 'Your Cart',
  '/app/orders': 'Orders',
  '/app/nursing': 'Nursing Care',
  '/app/notifications': 'Notifications',
  '/app/profile': 'My Profile',
  
  '/app/doctor/dashboard': 'Doctor Dashboard',
  '/app/doctor/schedule': 'My Schedule',
  '/app/doctor/appointments': 'Appointments',
  '/app/doctor/chat': 'Chat Room',
  
  '/app/pharmacist/inventory': 'Inventory',
  '/app/pharmacist/orders': 'Orders',
  
  '/app/nurse/requests': 'Home Care Requests',
  
  '/app/admin/accounts': 'Professional Accounts',
}

export default function AppLayout() {
  const [sidebarOpen, setSidebarOpen] = useState(false)
  const location = useLocation()

  const title =
    TITLES[location.pathname] ||
    (location.pathname.startsWith('/app/doctors/') ? 'Doctor Profile' : 'Tabiby')

  return (
    <div className="flex min-h-screen bg-mist-50">
      <Sidebar open={sidebarOpen} onClose={() => setSidebarOpen(false)} />

      <div className="flex min-h-screen flex-1 flex-col">
        <Topbar onMenuClick={() => setSidebarOpen(true)} title={title} />
        <main className="flex-1 px-5 py-6 sm:px-8 sm:py-8">
          <Outlet />
        </main>
      </div>
    </div>
  )
}
