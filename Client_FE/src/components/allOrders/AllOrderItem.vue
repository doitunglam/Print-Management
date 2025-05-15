<script lang="ts" setup>
import { type Ref, ref, onMounted } from "vue"
import OrderItem from "./OrderItem.vue"
import { useGetAllOrders } from "@/composables/useApi"
import Loading from "@/components/Loading.vue"

// get all designs
const listOrders: Ref<any> = ref([])
const phone: Ref<string> = ref("")
const loading = ref(false)
onMounted(async () => {

})

async function onSearch(params: type) {
  try {
    loading.value = true
    const response = await useGetAllOrders()
    listOrders.value = response.data
    loading.value = false
  } catch (e) {
    loading.value = false
    console.error(e)
  }
}
</script>

<template>
  <div class="flex justify-center items-center text-2xl font-semibold text-emerald-500 mt-8">
    Đơn hàng của tôi
  </div>
  <div class="w-[400px] mx-auto my-2">
    <a-input-search v-model:value="phone" placeholder="Nhập số điện thoại" enter-button :loading="loading"
      @search="onSearch" />
  </div>

  <div class="font-[sans-serif] gap-10 mx-auto mb-20 p-8">
    <div v-if="loading" class="w-[1400px] h-[400px]">
      <Loading></Loading>
    </div>
    <a-flex>
      <template v-for="order in listOrders" :key="order.id">
        <OrderItem :order="order"> </OrderItem>
      </template>
    </a-flex>
  </div>
</template>
