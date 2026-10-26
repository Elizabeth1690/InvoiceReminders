import '@/styles/main.css'

import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'

import App from '@/app/app'

const rootElement = document.getElementById('root')

if (!rootElement) {
  throw new Error('No se encontró el elemento #root en index.html')
}

createRoot(rootElement).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
