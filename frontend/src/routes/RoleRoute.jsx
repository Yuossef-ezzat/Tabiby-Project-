import { Navigate } from 'react-router-dom'
import { useAuth, ROLE_HOME_ROUTE } from '../context/AuthContext'

export default function RoleRoute({ roles, children }) {
  const { user } = useAuth()
  if (!user) return <Navigate to="/login" replace />
  if (!roles.includes(user.role)) {
    return <Navigate to={ROLE_HOME_ROUTE[user.role] || '/login'} replace />
  }
  return children
}
