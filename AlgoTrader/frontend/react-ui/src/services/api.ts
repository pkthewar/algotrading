import axios from 'axios'

const backendOrigin =
  import.meta.env.VITE_BACKEND_ORIGIN?.trim() || 'http://localhost:5219'

export const api = axios.create({
  baseURL: `${backendOrigin}/api`,
})

export const backendUrls = {
  apiBaseUrl: `${backendOrigin}/api`,
  hubUrl: `${backendOrigin}/marketHub`,
}
