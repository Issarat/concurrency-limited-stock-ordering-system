import { useState, type FormEvent, type SVGProps } from 'react'
import { useNavigate } from 'react-router-dom'

type IconProps = SVGProps<SVGSVGElement>

function BrandMarkIcon(props: IconProps) {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" {...props}>
      <path d="M12 2.75 14.58 9l6.67 3-6.67 3L12 21.25 9.42 15l-6.67-3 6.67-3L12 2.75Z" />
    </svg>
  )
}

function UserIcon(props: IconProps) {
  return (
    <svg viewBox="0 0 24 24" fill="none" aria-hidden="true" {...props}>
      <circle cx="12" cy="8" r="4" />
      <path d="M4.5 20c.8-4 3.3-6 7.5-6s6.7 2 7.5 6" />
    </svg>
  )
}

function LockIcon(props: IconProps) {
  return (
    <svg viewBox="0 0 24 24" fill="none" aria-hidden="true" {...props}>
      <rect x="5" y="10" width="14" height="11" rx="3" />
      <path d="M8 10V7a4 4 0 0 1 8 0v3" />
    </svg>
  )
}

function EyeIcon({ isOpen, ...props }: IconProps & { isOpen: boolean }) {
  return (
    <svg viewBox="0 0 24 24" fill="none" aria-hidden="true" {...props}>
      <path d="M2.7 12s3.4-5.4 9.3-5.4 9.3 5.4 9.3 5.4-3.4 5.4-9.3 5.4S2.7 12 2.7 12Z" />
      <circle cx="12" cy="12" r="2.5" />
      {!isOpen && <path d="m4 4 16 16" />}
    </svg>
  )
}

function CheckIcon(props: IconProps) {
  return (
    <svg viewBox="0 0 24 24" fill="none" aria-hidden="true" {...props}>
      <path d="m5 12.5 4.2 4.2L19 7" />
    </svg>
  )
}

function ArrowIcon(props: IconProps) {
  return (
    <svg viewBox="0 0 24 24" fill="none" aria-hidden="true" {...props}>
      <path d="M5 12h14M14 7l5 5-5 5" />
    </svg>
  )
}

export function LoginPage() {
  const navigate = useNavigate()
  const [showPassword, setShowPassword] = useState(false)

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    navigate('/dashboard')
  }

  return (
    <main className="auth-page">
      <section className="auth-shell" aria-label="เข้าสู่ระบบ Limited Stock Lab">
        <aside className="auth-story">
          <div className="orb orb-one" aria-hidden="true" />
          <div className="orb orb-two" aria-hidden="true" />

          <div className="auth-story-content">
            <div className="auth-brand">
              <span className="auth-brand-mark">
                <BrandMarkIcon />
              </span>
              <span>Limited Stock Lab</span>
            </div>

            <div className="auth-intro">
              <span className="auth-kicker">
                <span className="pulse-dot" /> Flash sale simulation
              </span>
              <h1>
                ทุกคำสั่งซื้อ
                <span>ถูกต้อง แม้เกิดพร้อมกัน</span>
              </h1>
              <p>
                ทดลองระบบสั่งซื้อสินค้าจำนวนจำกัดที่ออกแบบให้สต๊อก โควตา
                และคำขอซ้ำถูกจัดการอย่างปลอดภัยใน transaction เดียว
              </p>
            </div>

            <div className="reliability-card">
              <div className="reliability-item">
                <span className="feature-icon blue">
                  <CheckIcon />
                </span>
                <div>
                  <strong>ไม่ขายเกินสต๊อก</strong>
                  <span>ฐานข้อมูลเป็นผู้รับประกันความถูกต้อง</span>
                </div>
              </div>
              <div className="reliability-divider" />
              <div className="reliability-item">
                <span className="feature-icon mint">10</span>
                <div>
                  <strong>โควตาชัดเจนต่อผู้ใช้</strong>
                  <span>จำกัดสูงสุด 10 ชิ้นในแต่ละรอบขาย</span>
                </div>
              </div>
              <div className="reliability-tags" aria-label="คุณสมบัติของระบบ">
                <span><CheckIcon /> Atomic stock</span>
                <span><CheckIcon /> Idempotent request</span>
                <span><CheckIcon /> Live inventory</span>
              </div>
            </div>
          </div>

          <div className="auth-story-footer">
            <span>ASP.NET Core</span>
            <span className="footer-separator" />
            <span>PostgreSQL</span>
            <span className="footer-separator" />
            <span>SignalR</span>
          </div>
        </aside>

        <section className="auth-form-panel">
          <div className="auth-form-wrap">
            <div className="auth-form-topline">
              <span>ต้องการดูระบบก่อน?</span>
              <button
                type="button"
                className="text-button"
                onClick={() => navigate('/dashboard')}
              >
                เปิด Demo
              </button>
            </div>

            <div className="auth-form-heading">
              <span className="form-kicker">ยินดีต้อนรับกลับ</span>
              <h2>เข้าสู่ระบบ</h2>
              <p>ใช้บัญชีสำหรับทดลอง Flash Sale dashboard</p>
            </div>

            <form className="login-form" onSubmit={handleSubmit}>
              <label htmlFor="username">ชื่อผู้ใช้</label>
              <div className="input-control">
                <UserIcon className="input-icon" />
                <input
                  id="username"
                  name="username"
                  type="text"
                  autoComplete="username"
                  placeholder="กรอกชื่อผู้ใช้"
                  required
                />
              </div>

              <label htmlFor="password">รหัสผ่าน</label>
              <div className="input-control">
                <LockIcon className="input-icon" />
                <input
                  id="password"
                  name="password"
                  type={showPassword ? 'text' : 'password'}
                  autoComplete="current-password"
                  placeholder="กรอกรหัสผ่าน"
                  minLength={8}
                  required
                />
                <button
                  type="button"
                  className="password-toggle"
                  onClick={() => setShowPassword((current) => !current)}
                  aria-label={showPassword ? 'ซ่อนรหัสผ่าน' : 'แสดงรหัสผ่าน'}
                >
                  <EyeIcon isOpen={showPassword} />
                </button>
              </div>

              <div className="form-options">
                <label className="remember-control">
                  <input type="checkbox" name="remember" />
                  <span>จดจำฉันไว้</span>
                </label>
                <span className="form-mode-note">บัญชีทดสอบเท่านั้น</span>
              </div>

              <button className="primary-login-button" type="submit">
                <span>เข้าสู่ระบบ</span>
                <ArrowIcon />
              </button>
            </form>

            <div className="auth-divider"><span>หรือ</span></div>

            <button
              className="demo-login-button"
              type="button"
              onClick={() => navigate('/dashboard')}
            >
              <span className="demo-avatar">D</span>
              เข้าสู่ระบบด้วยบัญชี Demo
            </button>

            <p className="auth-footnote">
              ระบบนี้เป็น Portfolio demo และไม่มีการชำระเงินจริง
            </p>
          </div>
        </section>
      </section>
    </main>
  )
}
