import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter, Route, Routes } from 'react-router'
import './index.css'
import Game from './pages/Game.tsx'
import Home from './pages/Home.tsx'
import { SignalRProvider } from './SignalR/SignalRProvider.tsx'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <SignalRProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/game/:gameCode" element={<Game />} />
        </Routes>
      </BrowserRouter>
    </SignalRProvider>
  </StrictMode>,
)
