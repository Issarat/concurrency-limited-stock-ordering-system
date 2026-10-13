import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api': 'http://localhost:5171',
      '/health': 'http://localhost:5171',
      '/hubs': {
        target: 'http://localhost:5171',
        ws: true,
      },
    },
  },
})
