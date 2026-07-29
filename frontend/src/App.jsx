import { Routes, Route, Navigate } from 'react-router-dom'
import Login from './pages/auth/Login'
import Register from './pages/auth/Register'
import ForgotPassword from './pages/auth/ForgotPassword'
import AppLayout from './layouts/AppLayout'
import ProtectedRoute from './routes/ProtectedRoute'
import RoleRoute from './routes/RoleRoute'


import Profile from './pages/shared/Profile'


import Dashboard from './pages/patient/Dashboard'
import DoctorsList from './pages/doctor/DoctorsList'
import DoctorProfile from './pages/doctor/DoctorProfile'
import Appointments from './pages/patient/Appointments'
import Consultations from './pages/patient/Consultations'
import Pharmacy from './pages/pharmacy/Pharmacy'
import Cart from './pages/pharmacy/Cart'
import Orders from './pages/pharmacy/Orders'
import Nursing from './pages/patient/Nursing'
import Notifications from './pages/patient/Notifications'


import DoctorDashboard from './pages/portals/doctor/DoctorDashboard'
import DoctorSchedule from './pages/portals/doctor/DoctorSchedule'
import DoctorAppointments from './pages/portals/doctor/DoctorAppointments'
import DoctorChatRoom from './pages/portals/doctor/DoctorChatRoom'


import PharmacistInventory from './pages/portals/pharmacist/PharmacistInventory'
import PharmacistOrders from './pages/portals/pharmacist/PharmacistOrders'


import NurseRequests from './pages/portals/nurse/NurseRequests'


import AdminAccounts from './pages/portals/admin/AdminAccounts'

const NON_ADMIN_ROLES = ['Patient', 'Doctor', 'Pharmacist', 'Nurse']

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<Navigate to="/login" replace />} />

      {}
      <Route path="/login" element={<Login />} />
      <Route path="/register" element={<Register />} />
      <Route path="/forgot-password" element={<ForgotPassword />} />

      {}
      <Route
        path="/app"
        element={
          <ProtectedRoute>
            <AppLayout />
          </ProtectedRoute>
        }
      >
        <Route index element={<Navigate to="dashboard" replace />} />

        {}
        <Route
          path="profile"
          element={
            <RoleRoute roles={NON_ADMIN_ROLES}>
              <Profile />
            </RoleRoute>
          }
        />

        {}
        <Route
          path="dashboard"
          element={
            <RoleRoute roles={['Patient']}>
              <Dashboard />
            </RoleRoute>
          }
        />
        <Route
          path="doctors"
          element={
            <RoleRoute roles={['Patient']}>
              <DoctorsList />
            </RoleRoute>
          }
        />
        <Route
          path="doctors/:id"
          element={
            <RoleRoute roles={['Patient']}>
              <DoctorProfile />
            </RoleRoute>
          }
        />
        <Route
          path="appointments"
          element={
            <RoleRoute roles={['Patient']}>
              <Appointments />
            </RoleRoute>
          }
        />
        <Route
          path="consultations"
          element={
            <RoleRoute roles={['Patient']}>
              <Consultations />
            </RoleRoute>
          }
        />
        <Route
          path="pharmacy"
          element={
            <RoleRoute roles={['Patient']}>
              <Pharmacy />
            </RoleRoute>
          }
        />
        <Route
          path="cart"
          element={
            <RoleRoute roles={['Patient']}>
              <Cart />
            </RoleRoute>
          }
        />
        <Route
          path="orders"
          element={
            <RoleRoute roles={['Patient']}>
              <Orders />
            </RoleRoute>
          }
        />
        <Route
          path="nursing"
          element={
            <RoleRoute roles={['Patient']}>
              <Nursing />
            </RoleRoute>
          }
        />
        <Route
          path="notifications"
          element={
            <RoleRoute roles={NON_ADMIN_ROLES}>
              <Notifications />
            </RoleRoute>
          }
        />

        {}
        <Route
          path="doctor/dashboard"
          element={
            <RoleRoute roles={['Doctor']}>
              <DoctorDashboard />
            </RoleRoute>
          }
        />
        <Route
          path="doctor/schedule"
          element={
            <RoleRoute roles={['Doctor']}>
              <DoctorSchedule />
            </RoleRoute>
          }
        />
        <Route
          path="doctor/appointments"
          element={
            <RoleRoute roles={['Doctor']}>
              <DoctorAppointments />
            </RoleRoute>
          }
        />
        <Route
          path="doctor/chat"
          element={
            <RoleRoute roles={['Doctor']}>
              <DoctorChatRoom />
            </RoleRoute>
          }
        />

        {}
        <Route
          path="pharmacist/inventory"
          element={
            <RoleRoute roles={['Pharmacist']}>
              <PharmacistInventory />
            </RoleRoute>
          }
        />
        <Route
          path="pharmacist/orders"
          element={
            <RoleRoute roles={['Pharmacist']}>
              <PharmacistOrders />
            </RoleRoute>
          }
        />

        {}
        <Route
          path="nurse/requests"
          element={
            <RoleRoute roles={['Nurse']}>
              <NurseRequests />
            </RoleRoute>
          }
        />

        {}
        <Route
          path="admin/accounts"
          element={
            <RoleRoute roles={['Admin']}>
              <AdminAccounts />
            </RoleRoute>
          }
        />
      </Route>

      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  )
}
