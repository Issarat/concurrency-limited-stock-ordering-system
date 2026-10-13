import { useEffect, useMemo, useState, type SVGProps } from 'react'
import { Link } from 'react-router-dom'
import { getProducts, type ProductListItem } from '../lib/api'
import { resolveProductImage } from '../lib/productImages'

type IconProps = SVGProps<SVGSVGElement>

const priceFormatter = new Intl.NumberFormat('th-TH', {
  style: 'currency',
  currency: 'THB',
  maximumFractionDigits: 0,
})

function SearchIcon(props: IconProps) {
  return (
    <svg viewBox="0 0 24 24" fill="none" aria-hidden="true" {...props}>
      <circle cx="11" cy="11" r="6.5" />
      <path d="m16 16 4 4" />
    </svg>
  )
}

function BagIcon(props: IconProps) {
  return (
    <svg viewBox="0 0 24 24" fill="none" aria-hidden="true" {...props}>
      <path d="M5 8h14l-1 12H6L5 8Z" />
      <path d="M9 9V6a3 3 0 0 1 6 0v3" />
    </svg>
  )
}

function BoltIcon(props: IconProps) {
  return (
    <svg viewBox="0 0 24 24" fill="none" aria-hidden="true" {...props}>
      <path d="m13 2-8 12h7l-1 8 8-12h-7l1-8Z" />
    </svg>
  )
}

function ProductCard({ product }: { product: ProductListItem }) {
  const imageSource = resolveProductImage(product.imageUrl)
  const isSoldOut = product.availableQuantity <= 0
  const displayPrice = product.flashSaleStartingPrice ?? product.startingPrice

  return (
    <article className="product-card">
      <div className="product-image-wrap">
        {imageSource ? (
          <img
            src={imageSource}
            alt={product.name}
            className="product-image"
            loading="lazy"
            decoding="async"
          />
        ) : (
          <div className="product-image-placeholder" aria-label="ไม่มีรูปสินค้า">
            <BagIcon />
          </div>
        )}

        {product.isOnFlashSale && (
          <span className="sale-badge"><BoltIcon />Flash Sale</span>
        )}
        {isSoldOut && <span className="sold-out-badge">สินค้าหมด</span>}
      </div>

      <div className="product-card-body">
        <div className="product-card-copy">
          <h2>{product.name}</h2>
          <p>{product.description || 'สินค้าคุณภาพพร้อมจัดส่ง'}</p>
        </div>

        <div className="product-price-row">
          <div>
            <span className="price-label">ราคาเริ่มต้น</span>
            <strong>{priceFormatter.format(displayPrice)}</strong>
            {product.flashSaleStartingPrice !== null && (
              <del>{priceFormatter.format(product.startingPrice)}</del>
            )}
          </div>
          <span className={`stock-pill ${isSoldOut ? 'is-empty' : ''}`}>
            เหลือ {product.availableQuantity.toLocaleString('th-TH')}
          </span>
        </div>

        <div className="product-card-footer">
          <span>ขายแล้ว {product.soldQuantity.toLocaleString('th-TH')} ชิ้น</span>
          {isSoldOut ? (
            <button className="product-buy-button" type="button" disabled>
              สินค้าหมด
            </button>
          ) : (
            <Link className="product-buy-button" to={`/products/${product.productId}`}>
              ดูรายละเอียด
            </Link>
          )}
        </div>
      </div>
    </article>
  )
}

function ProductGridSkeleton() {
  return (
    <div className="product-grid" aria-label="กำลังโหลดรายการสินค้า" aria-busy="true">
      {Array.from({ length: 8 }, (_, index) => (
        <div className="product-card product-card-skeleton" key={index}>
          <div className="skeleton-image" />
          <div className="skeleton-body"><span /><span /><span /></div>
        </div>
      ))}
    </div>
  )
}

export function ShopPage() {
  const [products, setProducts] = useState<ProductListItem[]>([])
  const [query, setQuery] = useState('')
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const controller = new AbortController()

    getProducts(controller.signal)
      .then(setProducts)
      .catch((requestError: unknown) => {
        if (requestError instanceof DOMException && requestError.name === 'AbortError') return
        setError('โหลดรายการสินค้าไม่สำเร็จ กรุณาตรวจสอบว่า API และฐานข้อมูลกำลังทำงาน')
      })
      .finally(() => setIsLoading(false))

    return () => controller.abort()
  }, [])

  const filteredProducts = useMemo(() => {
    const normalizedQuery = query.trim().toLocaleLowerCase('th-TH')
    if (!normalizedQuery) return products

    return products.filter((product) =>
      `${product.name} ${product.description ?? ''}`
        .toLocaleLowerCase('th-TH')
        .includes(normalizedQuery),
    )
  }, [products, query])

  const availableProductCount = products.filter((product) => product.availableQuantity > 0).length
  const flashSaleCount = products.filter((product) => product.isOnFlashSale).length

  return (
    <section className="product-catalog">
      <div className="catalog-hero">
        <div>
          <span className="catalog-kicker">Limited Stock Collection</span>
          <h1>สินค้าพร้อมช้อป</h1>
          <p>เลือกสินค้า เช็กสต๊อกจริง และทดลองขั้นตอนยืนยันคำสั่งซื้อได้ทันที</p>
        </div>
        <div className="catalog-stats" aria-label="สรุปรายการสินค้า">
          <div><strong>{products.length}</strong><span>สินค้าทั้งหมด</span></div>
          <div><strong>{availableProductCount}</strong><span>พร้อมจำหน่าย</span></div>
          <div><strong>{flashSaleCount}</strong><span>Flash Sale</span></div>
        </div>
      </div>

      <div className="catalog-toolbar">
        <div><h2>รายการสินค้า</h2><span>{filteredProducts.length} รายการ</span></div>
        <label className="catalog-search">
          <SearchIcon />
          <span className="visually-hidden">ค้นหาสินค้า</span>
          <input
            type="search"
            value={query}
            onChange={(event) => setQuery(event.target.value)}
            placeholder="ค้นหาชื่อสินค้า..."
          />
        </label>
      </div>

      {isLoading && <ProductGridSkeleton />}

      {!isLoading && error && (
        <div className="catalog-state catalog-error" role="alert">
          <strong>เชื่อมต่อระบบไม่ได้</strong>
          <p>{error}</p>
          <button type="button" onClick={() => window.location.reload()}>ลองอีกครั้ง</button>
        </div>
      )}

      {!isLoading && !error && filteredProducts.length === 0 && (
        <div className="catalog-state">
          <SearchIcon />
          <strong>ไม่พบสินค้าที่ค้นหา</strong>
          <p>ลองค้นหาด้วยชื่อหรือคำอื่นอีกครั้ง</p>
        </div>
      )}

      {!isLoading && !error && filteredProducts.length > 0 && (
        <div className="product-grid">
          {filteredProducts.map((product) => (
            <ProductCard product={product} key={product.productId} />
          ))}
        </div>
      )}
    </section>
  )
}
