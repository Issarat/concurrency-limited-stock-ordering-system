import { Link } from 'react-router-dom'

export function HomePage() {
  return (
    <section className="page">
      <div className="page-copy">
        <div className="eyebrow">Project scaffold ready</div>
        <h1>Correct stock under pressure.</h1>
        <p>
          A flash-sale system designed to prove atomic stock updates, per-user
          limits, idempotent retries, and real-time committed inventory.
        </p>
        <div className="actions">
          <Link className="button" to="/purchase/demo-sale/demo-product">
            Open purchase confirmation
          </Link>
          <a className="button secondary" href="/health">
            API health
          </a>
        </div>
        <div className="metadata" aria-label="Technology overview">
          <div>
            <strong>ASP.NET Core 10</strong>
            <span>API and SignalR</span>
          </div>
          <div>
            <strong>PostgreSQL</strong>
            <span>Transactional source of truth</span>
          </div>
          <div>
            <strong>React 19</strong>
            <span>Focused demo interface</span>
          </div>
        </div>
      </div>
    </section>
  )
}
