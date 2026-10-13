import { useEffect, useMemo, useState, type SVGProps } from 'react'
import { Link, useParams } from 'react-router-dom'
import {
  getProductById,
  type ProductDetail,
  type ProductOption,
  type ProductVariant,
} from '../lib/api'
import { resolveProductImage } from '../lib/productImages'

type IconProps = SVGProps<SVGSVGElement>
type SelectedOptions = Record<string, string>

const priceFormatter = new Intl.NumberFormat('th-TH', {
  style: 'currency',
  currency: 'THB',
  maximumFractionDigits: 0,
})

function ArrowLeftIcon(props: IconProps) {
  return (
    <svg viewBox="0 0 24 24" fill="none" aria-hidden="true" {...props}>
      <path d="m15 18-6-6 6-6" />
    </svg>
  )
}

function ShieldIcon(props: IconProps) {
  return (
    <svg viewBox="0 0 24 24" fill="none" aria-hidden="true" {...props}>
      <path d="M12 3 5 6v5c0 4.7 2.7 8.3 7 10 4.3-1.7 7-5.3 7-10V6l-7-3Z" />
      <path d="m9 12 2 2 4-4" />
    </svg>
  )
}

function BoxIcon(props: IconProps) {
  return (
    <svg viewBox="0 0 24 24" fill="none" aria-hidden="true" {...props}>
      <path d="m4 7 8-4 8 4v10l-8 4-8-4V7Z" />
      <path d="m4 7 8 4 8-4M12 11v10" />
    </svg>
  )
}

function ShirtIcon({ color, ...props }: IconProps & { color: string }) {
  return (
    <svg viewBox="0 0 28 28" aria-hidden="true" {...props}>
      <path
        d="M9.2 5.2 12 4.1c.6 1.1 3.4 1.1 4 0l2.8 1.1 5 4.3-3.2 4-2.2-1.7v12.1H9.6V11.8l-2.2 1.7-3.2-4 5-4.3Z"
        fill={color}
        stroke="rgba(20, 30, 45, .28)"
        strokeWidth=".8"
        strokeLinejoin="round"
      />
      <path d="M11.8 4.4c.3 1.2 4.1 1.2 4.4 0" fill="none" stroke="rgba(255,255,255,.72)" strokeWidth="1" />
    </svg>
  )
}

function ColorValueIcon({ productName, color }: { productName: string; color: string }) {
  const isTop = /polo|jersey|shirt/i.test(productName)

  if (isTop) {
    return <ShirtIcon className="option-shirt-icon" color={color} />
  }

  return <span className="option-color-swatch" style={{ backgroundColor: color }} />
}

function findSelectedVariant(
  product: ProductDetail,
  selectedOptions: SelectedOptions,
): ProductVariant | undefined {
  if (product.options.length === 0) return product.variants[0]

  const selectedValueIds = Object.values(selectedOptions)
  if (selectedValueIds.length !== product.options.length) return undefined

  return product.variants.find((variant) =>
    selectedValueIds.every((valueId) => variant.optionValueIds.includes(valueId)),
  )
}

function getVariantSelection(product: ProductDetail, variant: ProductVariant): SelectedOptions {
  return Object.fromEntries(
    product.options.flatMap((option) => {
      const selectedValue = option.values.find((value) =>
        variant.optionValueIds.includes(value.optionValueId),
      )

      return selectedValue ? [[option.optionId, selectedValue.optionValueId]] : []
    }),
  )
}

function findAvailableVariantForValue(
  product: ProductDetail,
  option: ProductOption,
  optionValueId: string,
  selectedOptions: SelectedOptions,
): ProductVariant | undefined {
  const otherSelectedValues = Object.entries(selectedOptions)
    .filter(([optionId]) => optionId !== option.optionId)
    .map(([, valueId]) => valueId)

  const availableVariants = product.variants.filter(
    (variant) => variant.availableQuantity > 0 && variant.optionValueIds.includes(optionValueId),
  )

  return (
    availableVariants.find((variant) =>
      otherSelectedValues.every((valueId) => variant.optionValueIds.includes(valueId)),
    ) ?? availableVariants[0]
  )
}

function DetailSkeleton() {
  return (
    <div className="product-detail product-detail-skeleton" aria-busy="true">
      <div className="detail-skeleton-image" />
      <div className="detail-skeleton-copy"><span /><span /><span /><span /></div>
    </div>
  )
}

export function ProductDetailPage() {
  const { productId } = useParams()
  const [product, setProduct] = useState<ProductDetail | null>(null)
  const [selectedOptions, setSelectedOptions] = useState<SelectedOptions>({})
  const [quantity, setQuantity] = useState(1)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    if (!productId) {
      setError('ไม่พบรหัสสินค้า')
      setIsLoading(false)
      return
    }

    const controller = new AbortController()

    getProductById(productId, controller.signal)
      .then((result) => {
        setProduct(result)
        const initialVariant =
          result.variants.find((variant) => variant.availableQuantity > 0) ?? result.variants[0]

        setSelectedOptions(initialVariant ? getVariantSelection(result, initialVariant) : {})
      })
      .catch((requestError: unknown) => {
        if (requestError instanceof DOMException && requestError.name === 'AbortError') return
        setError('โหลดรายละเอียดสินค้าไม่สำเร็จ หรือไม่พบสินค้านี้')
      })
      .finally(() => setIsLoading(false))

    return () => controller.abort()
  }, [productId])

  const selectedVariant = useMemo(
    () => (product ? findSelectedVariant(product, selectedOptions) : undefined),
    [product, selectedOptions],
  )

  if (isLoading) {
    return <section className="product-detail-page"><DetailSkeleton /></section>
  }

  if (error || !product) {
    return (
      <section className="product-detail-page">
        <div className="detail-error" role="alert">
          <BoxIcon />
          <h1>ไม่พบสินค้า</h1>
          <p>{error ?? 'สินค้านี้อาจถูกปิดการขายแล้ว'}</p>
          <Link to="/dashboard"><ArrowLeftIcon />กลับไปหน้าสินค้า</Link>
        </div>
      </section>
    )
  }

  const imageSource = resolveProductImage(product.imageUrl)
  const currentPrice = selectedVariant?.salePrice ?? selectedVariant?.basePrice ?? product.startingPrice
  const regularPrice = selectedVariant?.basePrice ?? product.startingPrice
  const stockQuantity = selectedVariant?.availableQuantity ?? product.availableQuantity
  const maxQuantity = Math.min(10, stockQuantity)
  const isSoldOut = stockQuantity <= 0
  const hasSalePrice = selectedVariant?.salePrice !== null && selectedVariant?.salePrice !== undefined

  return (
    <section className="product-detail-page">
      <nav className="product-breadcrumb" aria-label="Breadcrumb">
        <Link to="/dashboard"><ArrowLeftIcon />สินค้าทั้งหมด</Link>
        <span>/</span>
        <span>{product.name}</span>
      </nav>

      <div className="product-detail">
        <div className="product-detail-media">
          {imageSource ? (
            <img src={imageSource} alt={product.name} decoding="async" />
          ) : (
            <div className="detail-image-placeholder"><BoxIcon /></div>
          )}
          {product.isOnFlashSale && <span className="detail-sale-badge">Flash Sale</span>}
        </div>

        <div className="product-detail-info">
          <span className="detail-kicker">Limited Stock Product</span>
          <h1>{product.name}</h1>
          <p className="detail-description">{product.description || 'สินค้าคุณภาพพร้อมจัดส่ง'}</p>

          <div className="detail-price-block">
            <span>ราคาสำหรับตัวเลือกนี้</span>
            <div>
              <strong>{priceFormatter.format(currentPrice)}</strong>
              {hasSalePrice && <del>{priceFormatter.format(regularPrice)}</del>}
            </div>
          </div>

          {product.options.map((option) => (
            <fieldset className="product-option-group" key={option.optionId}>
              <legend>{option.name}</legend>
              <div className="product-option-values">
                {option.values.map((value) => {
                  const isSelected = selectedOptions[option.optionId] === value.optionValueId
                  const availableVariant = findAvailableVariantForValue(
                    product,
                    option,
                    value.optionValueId,
                    selectedOptions,
                  )
                  const isAvailable = Boolean(availableVariant)

                  return (
                    <button
                      type="button"
                      className={`option-value ${isSelected ? 'is-selected' : ''}`}
                      disabled={!isAvailable}
                      aria-pressed={isSelected}
                      onClick={() => {
                        if (!availableVariant) return
                        setSelectedOptions(getVariantSelection(product, availableVariant))
                        setQuantity(1)
                      }}
                      key={value.optionValueId}
                    >
                      {value.swatchHex && (
                        <ColorValueIcon productName={product.name} color={value.swatchHex} />
                      )}
                      {value.value}
                    </button>
                  )
                })}
              </div>
            </fieldset>
          ))}

          <div className="variant-summary">
            <div><span>SKU</span><strong>{selectedVariant?.sku ?? 'กรุณาเลือกตัวเลือก'}</strong></div>
            <div><span>สถานะสต๊อก</span><strong className={isSoldOut ? 'is-out' : 'is-ready'}>{isSoldOut ? 'สินค้าหมด' : `พร้อมส่ง · เหลือ ${stockQuantity.toLocaleString('th-TH')} ชิ้น`}</strong></div>
          </div>

          <div className="purchase-panel">
            <div className="quantity-picker" aria-label="จำนวนสินค้า">
              <button type="button" disabled={quantity <= 1} onClick={() => setQuantity((value) => value - 1)}>−</button>
              <span>{quantity}</span>
              <button type="button" disabled={quantity >= maxQuantity} onClick={() => setQuantity((value) => value + 1)}>+</button>
            </div>

            {selectedVariant && !isSoldOut ? (
              <Link
                className="detail-buy-button"
                to={`/purchase/${product.isOnFlashSale ? 'current-sale' : 'regular'}/${product.productId}?variantId=${selectedVariant.variantId}&quantity=${quantity}`}
              >
                ยืนยันตัวเลือก
              </Link>
            ) : (
              <button className="detail-buy-button" type="button" disabled>
                {selectedVariant ? 'สินค้าหมด' : 'กรุณาเลือกตัวเลือก'}
              </button>
            )}
          </div>

          <div className="detail-assurance">
            <ShieldIcon />
            <div><strong>ตัดสต๊อกอย่างถูกต้อง</strong><span>ระบบตรวจสต๊อกและโควตาอีกครั้งเมื่อยืนยันคำสั่งซื้อ</span></div>
          </div>
        </div>
      </div>
    </section>
  )
}
