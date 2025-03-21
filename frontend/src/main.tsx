import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter, Route, Routes } from 'react-router'
import { OverlayProvider } from './components/Overlay.tsx'
import './index.css'
import Deck from './pages/Deck.tsx'
import Game from './pages/Game.tsx'
import Home from './pages/Home.tsx'
import { SignalRProvider } from './SignalR/SignalRProvider.tsx'

const queryClient = new QueryClient();

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <SignalRProvider>
        <OverlayProvider>
          <BrowserRouter>
            <Routes>
              <Route path="/" element={<Home />} />
              <Route path="/deck" element={<Deck />} />
              <Route path="/game/:gameCode/" element={<Game />} />
              <Route path="/game/:gameCode/deck" element={<Deck />} />
            </Routes>
          </BrowserRouter>
        </OverlayProvider>
      </SignalRProvider>
    </QueryClientProvider>
  </StrictMode>,
)
