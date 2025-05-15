<script lang="ts" setup>
import { ref, onMounted, type Ref, watch } from "vue"
import useClickOutside from "@/composables/useClickOutside"
import { LIST_PRODUCT_CATEGORIES } from "@/constants/main"
import { useGetAllProducts } from "@/composables/useApi"
import ProductItem from "./ProductItem.vue"
import Loading from "@/components/Loading.vue"
import type { SortOptionType, ProductType } from "@/types/main"
import { debounce, formatNumber } from "@/helpers/common"

// get all products
const listProducts: Ref<ProductType[]> = ref([])
const listOriginalProducts: Ref<ProductType[]> = ref([])
const loading = ref(false)
onMounted(async () => {
  try {
    loading.value = true
    const response = await useGetAllProducts()
    listProducts.value = listOriginalProducts.value = response
    loading.value = false
  } catch (e) {
    loading.value = false
    console.error(e)
  }
})

// sort
const showDropdownOrder = ref(false)
const listOrderOptions = [
  {
    name: "Phổ biến nhất",
    value: "popular",
    sort: () => {
      listProducts.value.sort((a: ProductType, b: ProductType) => a.id - b.id)
    },
  },
  {
    name: "Mới nhất",
    value: "newest",
    sort: () => {
      listProducts.value.sort((a: ProductType, b: ProductType) => {
        return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
      })
    },
  },
  {
    name: "Cũ nhất",
    value: "oldest",
    sort: () => {
      listProducts.value.sort((a: ProductType, b: ProductType) => {
        return new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime()
      })
    },
  },
  {
    name: "Giá từ thấp đến cao",
    value: "price-asc",
    sort: () => {
      listProducts.value.sort(
        (a: ProductType, b: ProductType) => a.price - b.price
      )
    },
  },
  {
    name: "Giá từ cao đến thấp",
    value: "price-desc",
    sort: () => {
      listProducts.value.sort(
        (a: ProductType, b: ProductType) => b.price - a.price
      )
    },
  },
]
const selectedOrderOption: Ref<SortOptionType> = ref(listOrderOptions[0])
const updateSelectedOrderOption = (option: SortOptionType) => {
  selectedOrderOption.value = option
  showDropdownOrder.value = false
  option.sort()
}

const dropdownRef = ref()
const exceptionRef = ref()
useClickOutside(
  dropdownRef,
  () => {
    showDropdownOrder.value = false
  },
  exceptionRef
)

// filter
const priceRange = ref(1000000)
const debounceSortByRange = debounce((max: number) => {
  listProducts.value = listOriginalProducts.value
  listProducts.value = listProducts.value.filter((product: ProductType) => {
    return product.price <= max
  })
}, 300)
watch(
  () => priceRange,
  (newVal) => {
    debounceSortByRange(newVal.value)
  },
  { deep: true }
)
</script>

<template>
  <div
    class="font-[sans-serif] flex justify-between items-start gap-10 mx-auto max-w-[1400px] mt-6 mb-20"
  >
    <div class="p-4">
      <h2 class="text-md text-start font-semibold text-gray-800 mb-6 sm:mb-8">
        Phân loại
      </h2>
      <div class="w-[200px]">
        <div
          v-for="category in LIST_PRODUCT_CATEGORIES"
          :key="category.value"
          class="text-md text-start font-medium text-zinc-600 hover:text-sky-500 cursor-pointer py-2"
        >
          {{ category.name }}
        </div>
      </div>
      <h2
        class="text-md text-start font-semibold text-gray-800 mb-6 sm:mb-8 mt-16"
      >
        Lọc theo
      </h2>
      <div class="w-[200px]">
        <h1 class="mb-2">Mức giá</h1>
        <h2 class="mb-2 text-sm text-gray-600">
          Thấp hơn:
          <span class="text-sky-600 font-bold"
            >{{ formatNumber(priceRange) }} đ</span
          >
        </h2>
        <div class="relative">
          <input
            type="range"
            min="0"
            max="1000000"
            step="50000"
            class="no-design-slider w-full h-4 rounded-15 bg-[#d3d3d3] outline-none opacity-70"
            v-model="priceRange"
          />
          <span class="absolute top-full left-0 text-xs">0</span>
          <span class="absolute top-full right-0 text-xs">1,000,000</span>
        </div>
      </div>
    </div>
    <div class="p-4">
      <div
        class="text-md text-end font-semibold text-gray-800 mb-6 sm:mb-8 flex justify-end items-center"
      >
        Sắp xếp theo:
        <div class="relative font-[sans-serif] w-[180px]">
          <button
            ref="exceptionRef"
            @click="showDropdownOrder = !showDropdownOrder"
            type="button"
            id="dropdownToggle"
            class="px-4 py-2 flex items-center text-zinc-500 text-sm font-medium outline-none"
          >
            {{ selectedOrderOption.name }}
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="w-3 fill-gray-400 inline ml-3"
              viewBox="0 0 24 24"
            >
              <path
                fill-rule="evenodd"
                d="M11.99997 18.1669a2.38 2.38 0 0 1-1.68266-.69733l-9.52-9.52a2.38 2.38 0 1 1 3.36532-3.36532l7.83734 7.83734 7.83734-7.83734a2.38 2.38 0 1 1 3.36532 3.36532l-9.52 9.52a2.38 2.38 0 0 1-1.68266.69734z"
                clip-rule="evenodd"
                data-original="#000000"
              />
            </svg>
          </button>

          <ul
            ref="dropdownRef"
            v-if="showDropdownOrder"
            id="dropdownMenu"
            class="absolute block shadow-lg bg-white py-2 z-[1000] min-w-full w-max rounded-lg max-h-96 overflow-auto"
          >
            <li
              v-for="option in listOrderOptions"
              :key="option.value"
              class="py-2.5 px-5 flex items-center hover:bg-gray-100 text-[#333] text-sm cursor-pointer"
              :class="
                selectedOrderOption.value === option.value ? 'text-sky-500' : ''
              "
              @click="updateSelectedOrderOption(option)"
            >
              {{ option.name }}
            </li>
          </ul>
        </div>
      </div>

      <div v-if="loading" class="w-[1000px] h-[400px]">
        <Loading></Loading>
      </div>
      <div
        v-else
        class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 sm:gap-6"
      >
        <template v-for="product in listProducts" :key="product.id">
          <ProductItem :product="product"></ProductItem>
        </template>
      </div>
    </div>
  </div>
</template>
