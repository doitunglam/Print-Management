<script lang="ts" setup>
import type { DesignType } from "@/types/main"
import { useMainStore } from "@/stores/main"
const store = useMainStore()
import { Notivue, Notification } from "notivue"
import router from "@/router/index.ts"

defineProps<{
  design: DesignType
}>()

const selectDesign = (design: DesignType) => {
  const selectedProduct = store.getSelectedProduct
  store.setSelectedDesign(design)
  if (!selectedProduct.id) {
    push.success({
      title: "Thông báo",
      message: "Đã chọn thiết kế, tiếp tục chọn sản phẩm để in.",
    })
    router.push({ name: "ProductPage" })
  } else {
    router.push({ name: "OrderPage" })
  }
}
</script>

<template>
  <div
    class="bg-white shadow-[0_4px_12px_-5px_rgba(0,0,0,0.4)] w-full max-w-sm rounded-lg overflow-hidden mx-auto font-[sans-serif] mt-4 h-[430px]"
  >
    <div class="lg:h-[256px]">
      <img :src="design.filePath" class="w-full h-[256px] object-cover" />
    </div>

    <div class="p-6 flex flex-col">
      <p class="mt-4 text-sm text-gray-500 leading-relaxed">
        Thiết kế hiện đại, đậm chất tinh tế và tương lai, phù hợp với mọi chất
        liệu, mọi sản phẩm
      </p>
      <button
        type="button"
        @click="selectDesign(design)"
        class="mt-6 px-5 py-2.5 rounded-lg text-white text-sm tracking-wider border-none outline-none bg-blue-600 hover:bg-blue-700 active:bg-blue-600"
      >
        Sử dụng
      </button>
    </div>
  </div>
  <Notivue v-slot="item">
    <Notification :item="item" />
  </Notivue>
</template>
