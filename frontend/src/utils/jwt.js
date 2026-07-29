
const CLAIM_KEYS = {
  role: ['role', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'],
  email: ['email', 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
  name: ['name', 'unique_name', 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
  nameId: [
    'nameid',
    'sub',
    'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier',
  ],
}

function firstPresent(payload, keys) {
  for (const k of keys) {
    if (payload[k] !== undefined) return payload[k]
  }
  return undefined
}

export function decodeJwt(token) {
  try {
    const [, payloadB64] = token.split('.')
    const json = decodeURIComponent(
      atob(payloadB64.replace(/-/g, '+').replace(/_/g, '/'))
        .split('')
        .map((c) => '%' + c.charCodeAt(0).toString(16).padStart(2, '0'))
        .join(''),
    )
    const payload = JSON.parse(json)

    const rawRole = firstPresent(payload, CLAIM_KEYS.role)
    const roleValue = Array.isArray(rawRole) ? rawRole[0] : rawRole
    // Defense in depth: normalize casing here too, in case the backend's
    
    const role = roleValue
      ? roleValue.charAt(0).toUpperCase() + roleValue.slice(1).toLowerCase()
      : roleValue

    return {
      id: Number(firstPresent(payload, CLAIM_KEYS.nameId)),
      email: firstPresent(payload, CLAIM_KEYS.email),
      userName: firstPresent(payload, CLAIM_KEYS.name),
      role,
      exp: payload.exp, 
    }
  } catch {
    return null
  }
}

export function isTokenExpired(decoded) {
  if (!decoded?.exp) return true
  return Date.now() >= decoded.exp * 1000
}
