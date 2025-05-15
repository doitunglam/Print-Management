import { ref, computed } from "vue"
import localStorageHelper from "@/helpers/localStorage"
import type { ProductType } from "@/types/main"

const WISHLIST_KEY = "product-wishlist"
const wishlist = ref<Array<ProductType>>(
  localStorageHelper.getItem(WISHLIST_KEY) || []
)
export default function useAddToWishList() {
  const addToWishList = (product: ProductType) => {
    if (!isInWishlist(product.id)) {
      wishlist.value.push(product)
      saveWishlist()
    }
  }

  const removeFromWishList = (productId: string | number) => {
    wishlist.value = wishlist.value.filter((item) => item.id !== productId)
    saveWishlist()
  }

  const isInWishlist = (productId: string | number) => {
    return wishlist.value.some((item) => item.id === productId)
  }

  const clearWishlist = () => {
    wishlist.value = []
    saveWishlist()
  }

  const saveWishlist = () => {
    localStorageHelper.setItem(WISHLIST_KEY, wishlist.value, null)
  }

  const getWishlist = computed(() => wishlist.value)

  return {
    addToWishList,
    removeFromWishList,
    isInWishlist,
    clearWishlist,
    getWishlist,
  }
}
