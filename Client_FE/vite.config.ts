import { fileURLToPath, URL } from "node:url"
import { defineConfig } from "vite"
import vue from "@vitejs/plugin-vue"
import Unimport from "unimport/unplugin"

// https://vitejs.dev/config/

export default defineConfig({
  plugins: [
    vue(),
    Unimport.vite({
      addons: {
        vueTemplate: true,
      },
      imports: [{ name: "push", from: "notivue" }],
    }),
  ],
  resolve: {
    dedupe: ["vue"],
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
  optimizeDeps: {
    include: ["vue"],
  },
  define: {
    "process.env": {},
  },
  build: {
    emptyOutDir: false,
    sourcemap: false,
    modulePreload: { polyfill: false },
    cssCodeSplit: true,
    minify: "esbuild",
    rollupOptions: {
      input: "src/main.ts",
      output: {
        globals: {
          vue: "Vue",
        },
        assetFileNames: (chunkInfo: any) => {
          if (chunkInfo.name == "main.css") return "print-management.css"
          return chunkInfo.name
        },
        entryFileNames: "print-management.js",
        chunkFileNames: "print-management.js",
      },
    },
    chunkSizeWarningLimit: 512,
  },
})
