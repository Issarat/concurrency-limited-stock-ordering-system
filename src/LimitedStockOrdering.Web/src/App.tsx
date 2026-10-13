import { Route, Routes } from 'react-router-dom'
import './App.css'
import { AppLayout } from './layouts/AppLayout'
import { LoginPage } from './pages/LoginPage'
import { NotFoundPage } from './pages/NotFoundPage'
import { ProductDetailPage } from './pages/ProductDetailPage'
import { PurchaseConfirmationPage } from './pages/PurchaseConfirmationPage'
import { ShopPage } from './pages/Shop'

function App() {
  return (
    <Routes>
      <Route path="/" element={<LoginPage />} />
      <Route element={<AppLayout />}>
        <Route path="/dashboard" element={<ShopPage />} />
        <Route path="/products/:productId" element={<ProductDetailPage />} />
        <Route
          path="/purchase/:flashSaleId/:productId"
          element={<PurchaseConfirmationPage />}
        />
        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  )
}

export default App
