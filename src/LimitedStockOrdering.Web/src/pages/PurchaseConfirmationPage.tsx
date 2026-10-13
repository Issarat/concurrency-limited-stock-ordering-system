import { useEffect, useState, type SVGProps } from 'react'
import { Link, useParams, useSearchParams } from 'react-router-dom'
import { getOrderPreview, type OrderPreview } from '../lib/api'
import { resolveProductImage } from '../lib/productImages'

type IconProps = SVGProps<SVGSVGElement>

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

function CheckIcon(props: IconProps) {
  return (
    <svg viewBox="0 0 24 24" fill="none" aria-hidden="true" {...props}>
      <path d="M20 6 9 17l-5-5" />
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

export function PurchaseConfirmationPage() {
  const { flashSaleId, productId } = useParams()
  const [searchParams] = useSearchParams()
  const [preview, setPreview] = useState<OrderPreview | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [isReviewed, setIsReviewed] = useState(false)

  const variantId = searchParams.get('variantId')
  const requestedQuantity = Number(searchParams.get('quantity') ?? 1)

  useEffect(() => {
    if (!productId || !variantId) {
      setError('ไม่พบรหัสสินค้าหรือ Variant ที่เลือก')
      setIsLoading(false)
      return
    }

    const controller = new AbortController()
    const quantity = Number.isInteger(requestedQuantity) ? requestedQuantity : 0
    const orderType = flashSaleId === 'regular' ? 'Regular' : 'FlashSale'

    getOrderPreview(
      { productId, variantId, quantity, orderType },
      controller.signal,
    )
      .then(setPreview)
      .catch((requestError: unknown) => {
        if (requestError instanceof DOMException && requestError.name === 'AbortError') return
        setError('โหลดรายละเอียดรายการไม่สำเร็จ')
      })
      .finally(() => setIsLoading(false))

    return () => controller.abort()
  }, [flashSaleId, productId, requestedQuantity, variantId])

  if (isLoading) {
    return (
      <section className="order-review-page" aria-busy="true">
        <div className="order-review-skeleton" />
      </section>
    )
  }

  const displayError =
    error ??
    (!variantId ? 'ไม่พบ Variant ที่เลือก' : null) ??
    (!preview ? 'Variant นี้ไม่มีอยู่หรือปิดการขายแล้ว' : null)

  if (displayError || !preview) {
    return (
      <section className="order-review-page">
        <div className="detail-error" role="alert">
          <BoxIcon />
          <h1>เปิดรายละเอียดรายการไม่ได้</h1>
          <p>{displayError ?? 'ไม่พบข้อมูลสินค้า'}</p>
          <Link to={productId ? `/products/${productId}` : '/dashboard'}>
            <ArrowLeftIcon />กลับไปเลือกสินค้า
          </Link>
        </div>
      </section>
    )
  }

  const imageSource = resolveProductImage(preview.imageUrl)
  const saleType = preview.isOnFlashSale ? 'Flash Sale' : 'สินค้าราคาปกติ'

  return (
    <section className="order-review-page">
      <nav className="product-breadcrumb" aria-label="Breadcrumb">
        <Link to={`/products/${preview.productId}`}><ArrowLeftIcon />แก้ไขตัวเลือก</Link>
        <span>/</span>
        <span>Order details</span>
      </nav>

      <header className="order-review-header">
        <div>
          <span className="detail-kicker">Purchase confirmation</span>
          <h1>ตรวจสอบรายการสั่งซื้อ</h1>
          <p>ตรวจสอบสินค้าและจำนวนก่อนยืนยัน รายการนี้ยังไม่สร้าง Order และยังไม่ตัดสต๊อก</p>
        </div>
        <span className="order-type-badge">{saleType}</span>
      </header>

      <div className="order-review-layout">
        <article className="order-item-card">
          <div className="order-item-image">
            {imageSource ? <img src={imageSource} alt={preview.productName} /> : <BoxIcon />}
          </div>

          <div className="order-item-content">
            <span>สินค้า 1 รายการ</span>
            <h2>{preview.productName}</h2>
            <p>{preview.description}</p>

            <dl className="order-item-options">
              {preview.options.map((option) => (
                <div key={option.name}>
                  <dt>{option.name}</dt>
                  <dd>{option.value}</dd>
                </div>
              ))}
              <div><dt>SKU</dt><dd>{preview.sku}</dd></div>
              <div><dt>จำนวน</dt><dd>{preview.quantity} ชิ้น</dd></div>
            </dl>
          </div>

          <strong className="order-item-price">{priceFormatter.format(preview.unitPrice)}</strong>
        </article>

        <aside className="order-summary-card">
          <div className="order-summary-title">
            <div><span>Order details</span><h2>สรุปยอด</h2></div>
            <BoxIcon />
          </div>

          <dl className="order-totals">
            <div><dt>ราคาต่อชิ้น</dt><dd>{priceFormatter.format(preview.unitPrice)}</dd></div>
            <div><dt>จำนวน</dt><dd>{preview.quantity} ชิ้น</dd></div>
            <div><dt>ค่าจัดส่ง</dt><dd>ฟรี</dd></div>
            <div className="order-grand-total"><dt>ยอดรวม</dt><dd>{priceFormatter.format(preview.totalAmount)}</dd></div>
          </dl>

          <div className={`order-stock-note ${preview.hasSufficientStock ? '' : 'is-error'}`}>
            <CheckIcon />
            <span>
              {!preview.hasSufficientStock
                ? 'สต๊อกไม่เพียงพอ กรุณากลับไปเลือกใหม่'
                : `มีสินค้าเพียงพอ · คงเหลือ ${preview.availableQuantity.toLocaleString('th-TH')} ชิ้น`}
            </span>
          </div>

          <button
            className="order-confirm-button"
            type="button"
            disabled={!preview.hasSufficientStock || isReviewed}
            onClick={() => setIsReviewed(true)}
          >
            {isReviewed ? <><CheckIcon />ตรวจสอบข้อมูลแล้ว</> : 'ยืนยันรายละเอียดรายการ'}
          </button>

          <p className="order-summary-footnote">
            ปุ่มนี้ยืนยันเฉพาะข้อมูลบนหน้าจอ ยังไม่มีการชำระเงินหรือสร้าง Order
          </p>
        </aside>
      </div>
    </section>
  )
}
