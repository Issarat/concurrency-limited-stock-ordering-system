const configuredApiBaseUrl = import.meta.env.VITE_API_BASE_URL as string | undefined

export const apiBaseUrl = configuredApiBaseUrl?.replace(/\/$/, '') ?? ''

const authStorageKey = 'limited-stock-auth'

export type AuthUser = {
  userId: string
  username: string
  firstName: string
  lastName: string
  displayName: string
  role: 'Admin' | 'Buyer'
  isAnonymous: boolean
}

export type AuthSession = {
  accessToken: string
  tokenType: 'Bearer'
  expiresAt: string
  user: AuthUser
}

type ApiProblem = {
  detail?: string
}

export function getStoredAuthSession(): AuthSession | null {
  // Authentication belongs to the current browser tab only.
  localStorage.removeItem(authStorageKey)
  const serialized = sessionStorage.getItem(authStorageKey)
  if (!serialized) {
    return null
  }

  try {
    const session = JSON.parse(serialized) as AuthSession
    const isLegacyGuest = session.user?.isAnonymous
      && (!session.user.firstName || !session.user.lastName)
    if (!session.accessToken
      || Date.parse(session.expiresAt) <= Date.now()
      || isLegacyGuest) {
      clearAuthSession()
      return null
    }

    return session
  } catch {
    clearAuthSession()
    return null
  }
}

export function clearAuthSession() {
  sessionStorage.removeItem(authStorageKey)
  localStorage.removeItem(authStorageKey)
}

function storeAuthSession(session: AuthSession) {
  sessionStorage.setItem(authStorageKey, JSON.stringify(session))
  return session
}

async function readAuthResponse(response: Response): Promise<AuthSession> {
  if (!response.ok) {
    const problem = await response.json().catch(() => null) as ApiProblem | null
    throw new Error(problem?.detail ?? `Authentication failed with ${response.status}`)
  }

  return storeAuthSession(await response.json() as AuthSession)
}

export async function loginAdmin(
  username: string,
  password: string,
): Promise<AuthSession> {
  const response = await fetch(`${apiBaseUrl}/api/auth/admin/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username, password }),
  })

  return readAuthResponse(response)
}

export async function loginAsGuest(): Promise<AuthSession> {
  const existingSession = getStoredAuthSession()
  if (existingSession?.user.isAnonymous) {
    return existingSession
  }

  const response = await fetch(`${apiBaseUrl}/api/auth/guest`, {
    method: 'POST',
  })

  return readAuthResponse(response)
}

export async function authenticatedFetch(
  input: string,
  init: RequestInit = {},
): Promise<Response> {
  const session = getStoredAuthSession()
  const headers = new Headers(init.headers)
  if (session) {
    headers.set('Authorization', `Bearer ${session.accessToken}`)
  }

  const response = await fetch(input, { ...init, headers })
  if (response.status === 401) {
    clearAuthSession()
  }

  return response
}

export type SystemStatus = {
  service: string
  status: string
  utcTime: string
}

export type ProductListItem = {
  productId: string
  name: string
  description: string | null
  imageUrl: string | null
  startingPrice: number
  flashSaleStartingPrice: number | null
  isOnFlashSale: boolean
  availableQuantity: number
  soldQuantity: number
}

export type ProductOptionValue = {
  optionValueId: string
  value: string
  swatchHex: string | null
  displayOrder: number
}

export type ProductOption = {
  optionId: string
  name: string
  displayOrder: number
  values: ProductOptionValue[]
}

export type ProductVariant = {
  variantId: string
  sku: string
  basePrice: number
  salePrice: number | null
  availableQuantity: number
  optionValueIds: string[]
}

export type ProductDetail = ProductListItem & {
  options: ProductOption[]
  variants: ProductVariant[]
}

export type OrderPreviewRequest = {
  productId: string
  variantId: string
  quantity: number
  orderType: 'Regular' | 'FlashSale'
}

export type OrderPreviewOption = {
  name: string
  value: string
}

export type OrderPreview = {
  productId: string
  variantId: string
  productName: string
  description: string | null
  imageUrl: string | null
  sku: string
  orderType: 'Regular' | 'FlashSale'
  unitPrice: number
  quantity: number
  totalAmount: number
  availableQuantity: number
  hasSufficientStock: boolean
  isOnFlashSale: boolean
  options: OrderPreviewOption[]
}

export async function getSystemStatus(signal?: AbortSignal): Promise<SystemStatus> {
  const response = await fetch(`${apiBaseUrl}/api/system/status`, { signal })

  if (!response.ok) {
    throw new Error(`System status request failed with ${response.status}`)
  }

  return response.json() as Promise<SystemStatus>
}

export async function getProducts(signal?: AbortSignal): Promise<ProductListItem[]> {
  const response = await fetch(`${apiBaseUrl}/api/products`, { signal })

  if (!response.ok) {
    throw new Error(`Products request failed with ${response.status}`)
  }

  return response.json() as Promise<ProductListItem[]>
}

export async function getProductById(
  productId: string,
  signal?: AbortSignal,
): Promise<ProductDetail> {
  const response = await fetch(`${apiBaseUrl}/api/products/${productId}`, { signal })

  if (!response.ok) {
    throw new Error(`Product request failed with ${response.status}`)
  }

  return response.json() as Promise<ProductDetail>
}

export async function getOrderPreview(
  request: OrderPreviewRequest,
  signal?: AbortSignal,
): Promise<OrderPreview> {
  const response = await fetch(`${apiBaseUrl}/api/orders/preview`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
    signal,
  })

  if (!response.ok) {
    throw new Error(`Order preview request failed with ${response.status}`)
  }

  return response.json() as Promise<OrderPreview>
}
