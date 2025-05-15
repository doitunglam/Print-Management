<script lang="ts" setup>
import { ref, onMounted, type Ref } from "vue"
import { useGetAllProducts } from "@/composables/useApi"
import ProductItem from "@/components/products/ProductItem.vue"
import type { ProductType } from "@/types/main"

// get all products
const listProducts: Ref<ProductType[]> = ref([])
onMounted(async () => {
  try {
    const response = await useGetAllProducts()
    listProducts.value = response.slice(0, 8)
  } catch (e) {
    console.error(e)
  }
})
</script>

<template>
  <div class="font-[sans-serif] p-4 mx-auto max-w-[1400px] my-20">
    <h2
      class="text-xl text-center sm:text-3xl font-extrabold text-gray-800 mb-6 sm:mb-8"
    >
      Sản phẩm nổi bật
    </h2>

    <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 sm:gap-6">
      <template v-for="product in listProducts" :key="product.id">
        <ProductItem :product="product"></ProductItem>
      </template>
    </div>
    <div class="w-full flex justify-end items-center">
      <router-link
        to="/products"
        class="cursor-pointer font-semibold overflow-hidden relative z-100 border border-sky-500 group px-2 py-1"
      >
        <span
          class="relative z-10 text-sky-500 group-hover:text-white text-sm duration-500"
          >Xem thêm ➜</span
        >
        <span
          class="absolute w-full h-full bg-sky-500 -left-32 top-0 -rotate-45 group-hover:rotate-0 group-hover:left-0 duration-500"
        ></span>
        <span
          class="absolute w-full h-full bg-sky-500 -right-32 top-0 -rotate-45 group-hover:rotate-0 group-hover:right-0 duration-500"
        ></span>
      </router-link>
    </div>
  </div>
</template>
