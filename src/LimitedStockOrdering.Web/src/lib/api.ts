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
