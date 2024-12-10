import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter, Route, Routes } from 'react-router'
import './index.css'
import Home from './pages/Home.tsx'
import Layout from './pages/Layout.tsx'
import { SignalRProvider } from './SignalR/SignalRProvider.tsx'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <SignalRProvider>
      <BrowserRouter>
        <Routes>
          <Route element={<Layout />}>
            <Route index element={<Home />} />
            <Route path="/login" element={<p>Login</p>} />
          </Route>
        </Routes>
      </BrowserRouter>
    </SignalRProvider>
  </StrictMode>,
)
