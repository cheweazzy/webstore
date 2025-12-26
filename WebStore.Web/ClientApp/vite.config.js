import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react' 

export default defineConfig({
    plugins: [plugin()],
    server: {
        proxy: {
            '^/api': {
                target: 'https://localhost:7213', 
                secure: false
            },
            '^/swagger': {
                target: 'https://localhost:7213',
                secure: false
            }
        }
    }
})

