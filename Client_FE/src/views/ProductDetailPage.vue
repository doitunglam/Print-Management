<script setup lang="ts">
import PageLayout from "@/layouts/PageLayout.vue"
import ProductDetail from "@/components/products/ProductDetail.vue"

defineProps<{
  id: string
}>()
</script>

<template>
  <PageLayout>
    <ProductDetail :id="id"></ProductDetail>
  </PageLayout>
</template>
