<script lang="ts" setup>
import router from "@/router/index.ts"
import { formatMoney } from "@/helpers/common"
import type { ProductType } from "@/types/main"
import useAddToWishList from "@/composables/useAddToWishList"
import { Notivue, Notification } from "notivue"
import { useMainStore } from "@/stores/main"
const store = useMainStore()

defineProps<{
  product: ProductType
}>()

const { addToWishList, removeFromWishList, isInWishlist } = useAddToWishList()

const handleUpdateWishList = (product: ProductType) => {
  if (!isInWishlist(product.id)) {
    addToWishList(product)
  } else {
    removeFromWishList(product.id)
  }
}

const selectProductToApplyDesign = (product: ProductType) => {
  const selectedDesign = store.getSelectedDesign
  store.setSelectedProduct(product)
  if (!selectedDesign.id) {
    push.success({
      title: "Thông báo",
      message: "Đã chọn sản phẩm, tiếp tục chọn thiết kế để in.",
    })
    router.push({ name: "DesignPage" })
  } else {
    router.push({ name: "OrderPage" })
  }
}
</script>

<template>
  <div class="group overflow-hidden cursor-pointer relative">
    <div class="bg-gray-100 w-full overflow-hidden">
      <router-link :to="`/product/${product.id}`">
        <img
          :src="product.imageUrl"
          :alt="product.name"
          class="aspect-[3/4] w-full object-cover object-top hover:scale-110 transition-all duration-700"
        />
      </router-link>
    </div>

    <div class="p-4 relative">
      <div
        class="flex flex-wrap justify-between gap-2 w-full absolute px-4 py-3 z-10 transition-all duration-500 left-0 right-0 group-hover:bottom-20 lg:bottom-5 lg:opacity-0 lg:bg-white lg:group-hover:opacity-100 max-lg:bottom-20 max-lg:py-3 max-lg:bg-white/60"
      >
        <button
          type="button"
          title="Add to wishlist"
          class="bg-transparent outline-none border-none"
          @click="handleUpdateWishList(product)"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="fill-gray-800 w-5 h-5 inline-block"
            viewBox="0 0 64 64"
            :class="isInWishlist(product.id) ? 'fill-red-600' : ''"
          >
            <path
              d="M45.5 4A18.53 18.53 0 0 0 32 9.86 18.5 18.5 0 0 0 0 22.5C0 40.92 29.71 59 31 59.71a2 2 0 0 0 2.06 0C34.29 59 64 40.92 64 22.5A18.52 18.52 0 0 0 45.5 4ZM32 55.64C26.83 52.34 4 36.92 4 22.5a14.5 14.5 0 0 1 26.36-8.33 2 2 0 0 0 3.27 0A14.5 14.5 0 0 1 60 22.5c0 14.41-22.83 29.83-28 33.14Z"
            ></path>
          </svg>
        </button>
        <button
          @click="selectProductToApplyDesign(product)"
          type="button"
          title="apply design"
          class="bg-transparent outline-none border-none"
        >
          <svg
            width="24"
            height="24"
            viewBox="0 0 1024 1024"
            class="icon"
            version="1.1"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path d="M192 234.666667h640v64H192z" fill="#424242" />
            <path
              d="M85.333333 533.333333h853.333334v-149.333333c0-46.933333-38.4-85.333333-85.333334-85.333333H170.666667c-46.933333 0-85.333333 38.4-85.333334 85.333333v149.333333z"
              fill="#616161"
            />
            <path
              d="M170.666667 768h682.666666c46.933333 0 85.333333-38.4 85.333334-85.333333v-170.666667H85.333333v170.666667c0 46.933333 38.4 85.333333 85.333334 85.333333z"
              fill="#424242"
            />
            <path
              d="M853.333333 384m-21.333333 0a21.333333 21.333333 0 1 0 42.666667 0 21.333333 21.333333 0 1 0-42.666667 0Z"
              fill="#00E676"
            />
            <path
              d="M234.666667 85.333333h554.666666v213.333334H234.666667z"
              fill="#90CAF9"
            />
            <path
              d="M800 661.333333h-576c-17.066667 0-32-14.933333-32-32s14.933333-32 32-32h576c17.066667 0 32 14.933333 32 32s-14.933333 32-32 32z"
              fill="#242424"
            />
            <path
              d="M234.666667 661.333333h554.666666v234.666667H234.666667z"
              fill="#90CAF9"
            />
            <path
              d="M234.666667 618.666667h554.666666v42.666666H234.666667z"
              fill="#42A5F5"
            />
            <path
              d="M341.333333 704h362.666667v42.666667H341.333333zM341.333333 789.333333h277.333334v42.666667H341.333333z"
              fill="#1976D2"
            />
          </svg>
        </button>
      </div>
      <div class="z-20 relative">
        <h6 class="text-sm font-semibold text-gray-800 truncate">
          {{ product.name }}
        </h6>
        <h6 class="text-sm text-gray-600 mt-2">
          {{ formatMoney(product.price) }}
        </h6>
      </div>
    </div>
  </div>
  <Notivue v-slot="item">
    <Notification :item="item" />
  </Notivue>
</template>
