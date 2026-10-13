import { Link, useParams } from 'react-router-dom'

export function PurchaseConfirmationPage() {
  const { flashSaleId, productId } = useParams()

  return (
    <section className="page">
      <div className="page-copy">
        <div className="eyebrow">Purchase confirmation</div>
        <h1>Review before confirming.</h1>
        <p>
          This route is ready for the product summary, quantity selector, and
          idempotent confirmation request. No payment gateway is part of the MVP.
        </p>
        <div className="metadata" aria-label="Route parameters">
          <div>
            <strong>Flash sale</strong>
            <span>{flashSaleId}</span>
          </div>
          <div>
            <strong>Product</strong>
            <span>{productId}</span>
          </div>
          <div>
            <strong>Status</strong>
            <span>Awaiting implementation</span>
          </div>
        </div>
        <div className="actions">
          <Link className="button secondary" to="/">
            Back to products
          </Link>
        </div>
      </div>
    </section>
  )
}
