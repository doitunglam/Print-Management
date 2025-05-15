<script lang="ts" setup>
import { type Ref, ref, onMounted } from "vue"
import { useGetAllDesigns } from "@/composables/useApi"
import DesignItem from "./DesignItem.vue"
import Loading from "@/components/Loading.vue"

// get all designs
const listDesigns: Ref<any> = ref([])
const loading = ref(false)
onMounted(async () => {
  try {
    loading.value = true
    const response = await useGetAllDesigns()
    listDesigns.value = response
    loading.value = false
  } catch (e) {
    loading.value = false
    console.error(e)
  }
})
</script>

<template>
  <div
    class="flex justify-center items-center text-2xl font-semibold text-emerald-500 mt-8"
  >
    Lựa chọn thiết kế phù hợp
  </div>
  <div
    class="font-[sans-serif] flex justify-between items-start gap-10 mx-auto max-w-[1400px] mt-6 mb-20"
  >
    <div v-if="loading" class="w-[1400px] h-[400px]">
      <Loading></Loading>
    </div>
    <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 sm:gap-6">
      <template v-for="design in listDesigns" :key="design.id">
        <DesignItem :design="design"></DesignItem>
      </template>
    </div>
  </div>
</template>
