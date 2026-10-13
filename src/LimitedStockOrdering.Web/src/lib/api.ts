const configuredApiBaseUrl = import.meta.env.VITE_API_BASE_URL as string | undefined

export const apiBaseUrl = configuredApiBaseUrl?.replace(/\/$/, '') ?? ''

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
