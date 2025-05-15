import axios from "axios"
import type { AxiosInstance } from "axios"

const timeout = 20000
const baseUrl = import.meta.env.VITE_PRINT_MANAGEMENT_BASE_URL

const apiClient: AxiosInstance = axios.create({
  baseURL: baseUrl,
  headers: {},
  timeout: timeout,
})

export default apiClient
