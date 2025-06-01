import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    port: 5277,
    proxy: {
      '/api': {
        target: 'http://localhost:5276',
        changeOrigin: true,
        secure: false
      }
    }
  }
})
