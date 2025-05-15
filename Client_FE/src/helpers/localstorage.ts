const localStorageHelper = {
  setItem(key: string, value: any, ttl: number | null) {
    const item = {
      value: value,
      timestamp: Date.now(),
      ttl: ttl || null, // time to live
    }
    localStorage.setItem(key, JSON.stringify(item))
  },

  getItem(key: string) {
    const item = localStorage.getItem(key)
    if (!item) return null

    try {
      const parsed = JSON.parse(item)
      if (parsed.ttl && Date.now() - parsed.timestamp > parsed.ttl) {
        localStorage.removeItem(key)
        return null
      }
      return parsed.value
    } catch (error) {
      console.error('Error get localStorage item:', error)
      return null
    }
  },

  removeItem(key: string) {
    localStorage.removeItem(key)
  },

  clear() {
    localStorage.clear()
  },

  hasKey(key: string) {
    return localStorage.getItem(key) !== null
  },

  getAllKeys() {
    return Object.keys(localStorage)
  },

  getAllItems() {
    const items: any = {}
    Object.keys(localStorage).forEach(key => {
      items[key] = this.getItem(key)
    })
    return items
  },
}

export default localStorageHelper
