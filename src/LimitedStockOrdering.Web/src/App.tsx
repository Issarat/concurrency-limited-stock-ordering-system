import { Link, Outlet, Route, Routes } from 'react-router-dom'
import './App.css'
import { HomePage } from './pages/HomePage'
import { LoginPage } from './pages/LoginPage'
import { NotFoundPage } from './pages/NotFoundPage'
import { PurchaseConfirmationPage } from './pages/PurchaseConfirmationPage'

function AppLayout() {
  return (
    <div className="app-shell">
      <header className="site-header">
        <Link className="brand" to="/dashboard">
          Limited Stock Lab
        </Link>
        <span className="environment-badge">Development</span>
      </header>

      <main>
        <Outlet />
      </main>
    </div>
  )
}

function App() {
  return (
    <Routes>
      <Route path="/" element={<HomePage />} />
      <Route element={<AppLayout />}>
          <Route path="/home" element={<HomePage />} />
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
