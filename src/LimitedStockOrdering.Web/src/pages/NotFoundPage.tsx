import { Link } from 'react-router-dom'

export function NotFoundPage() {
  return (
    <section className="page">
      <div className="page-copy">
        <div className="eyebrow">404</div>
        <h1>Page not found.</h1>
        <div className="actions">
          <Link className="button" to="/">
            Return home
          </Link>
        </div>
      </div>
    </section>
  )
}
