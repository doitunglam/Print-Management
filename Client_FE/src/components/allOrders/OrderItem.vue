<script lang="ts" setup>
import type { ProductType, DesignType, OrderType } from "@/types/main"
import { useMainStore } from "@/stores/main"
const store = useMainStore()
import Loading from "@/components/Loading.vue"
import router from "@/router"

import { useGetAllDesigns, useGetAllOrders, useGetProductById } from "@/composables/useApi"
import { ref, onMounted } from "vue"

const props = defineProps<{
  order: OrderType
}>()

// get product
const loading = ref(false)
const product = ref<ProductType | null>(null)

onMounted(async () => {
  try {
    loading.value = true
    const response = await useGetProductById(props.order.productID)
    product.value = response
    console.log(product.value)
    loading.value = false
  } catch (e) {
    loading.value = false
    console.error(e)
  }
})

function onClick(orderId: any)  {
  router.push(`order/${orderId}`)
  // console.log(router, orderId)
}
</script>

<template>
  <div class="bg-white shadow-[0_4px_12px_-5px_rgba(0,0,0,0.4)] mt-4 p-4 flex-grow">
    <div v-if="loading">
      <Loading></Loading>
    </div>
    <div v-else>
      <a-flex gap="8">
        <img :src="product?.imageUrl" class="h-32">

        <a-flex vertical gap="32" class="flex-grow">
          <a-flex gap="32" justify="space-between">
            <p>
              Ngày tạo đơn hàng: {{ new Date(order.orderDate).toDateString() }}
            </p>
            <p>
              Địa chỉ nhận hàng: {{ order.deliveryAddress }}
            </p>
            <p>
              Tên khách hàng: {{ order.customerName }}
            </p>
          </a-flex>
          <a-flex justify="flex-end">
            <a-button type="primary" @click="() => onClick(order.id)">
              Xem chi tiết
            </a-button>
          </a-flex>
        </a-flex>
      </a-flex>
    </div>
  </div>
</template>
