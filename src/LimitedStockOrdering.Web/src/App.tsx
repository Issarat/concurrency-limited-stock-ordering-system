import { Navigate, Outlet, Route, Routes } from 'react-router-dom'
import './App.css'
import { AppLayout } from './layouts/AppLayout'
import { getStoredAuthSession } from './lib/api'
import { LoginPage } from './pages/LoginPage'
import { NotFoundPage } from './pages/NotFoundPage'
import { ProductDetailPage } from './pages/ProductDetailPage'
import { PurchaseConfirmationPage } from './pages/PurchaseConfirmationPage'
import { ShopPage } from './pages/Shop'

function RequireAuthentication() {
  return getStoredAuthSession()
    ? <Outlet />
    : <Navigate to="/login" replace />
}

function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route element={<AppLayout />}>
        <Route path="/" element={<ShopPage />} />
        <Route path="/dashboard" element={<Navigate to="/" replace />} />
        <Route path="/products/:productId" element={<ProductDetailPage />} />
        <Route element={<RequireAuthentication />}>
          <Route
            path="/purchase/:flashSaleId/:productId"
            element={<PurchaseConfirmationPage />}
          />
        </Route>
        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  )
}

export default App
