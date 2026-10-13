import { type SVGProps } from 'react'
import { Link, Outlet } from 'react-router-dom'

type IconProps = SVGProps<SVGSVGElement>

function UserIcon(props: IconProps) {
  return (
    <svg viewBox="0 0 24 24" fill="none" aria-hidden="true" {...props}>
      <circle cx="12" cy="8" r="3.5" />
      <path d="M5.5 20c.7-4 2.9-6 6.5-6s5.8 2 6.5 6" />
    </svg>
  )
}

function CartIcon(props: IconProps) {
  return (
    <svg viewBox="0 0 24 24" fill="none" aria-hidden="true" {...props}>
      <path d="M3 5h2l1.6 9.5h10.9l1.5-7H6" />
      <circle cx="9" cy="19" r="1.2" />
      <circle cx="17" cy="19" r="1.2" />
    </svg>
  )
}

function SparkIcon(props: IconProps) {
  return (
    <svg viewBox="0 0 24 24" fill="none" aria-hidden="true" {...props}>
      <path d="m12 3 1.5 4.5L18 9l-4.5 1.5L12 15l-1.5-4.5L6 9l4.5-1.5L12 3ZM18.5 15l.8 2.2 2.2.8-2.2.8-.8 2.2-.8-2.2-2.2-.8 2.2-.8.8-2.2Z" />
    </svg>
  )
}

export function AppLayout() {
  return (
    <div className="storefront-shell">
      <div className="utility-bar">
        <div className="storefront-container utility-inner">
          <p>เว็บไซต์นี้เป็นผลงาน Portfolio / Resume</p>
          <nav className="utility-links" aria-label="Quick links">
            <span>พร้อมทดลองใช้งาน</span>
            <Link to="https://github.com/Issarat/concurrency-limited-stock-ordering-system"><SparkIcon />GitHub</Link>
          </nav>
        </div>
      </div>

      <header className="commerce-header">
        <div className="storefront-container commerce-nav">
          <Link className="commerce-brand" to="/dashboard">
            <span>Limited</span>Stock
          </Link>

          <nav className="commerce-actions" aria-label="Account and order">
            <Link to="/" className="commerce-action">
              <UserIcon />
              <span>เข้าสู่ระบบ</span>
            </Link>
            <span className="nav-divider" aria-hidden="true" />
            <Link
              to="/purchase/demo-sale/demo-product"
              className="commerce-action"
            >
              <CartIcon />
              <span>ตะกร้า</span>
            </Link>
          </nav>
        </div>
      </header>

      <main className="storefront-main">
        <div className="storefront-container storefront-content">
          <Outlet />
        </div>
      </main>
    </div>
  )
}
