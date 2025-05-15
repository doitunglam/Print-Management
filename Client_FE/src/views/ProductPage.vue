<script setup lang="ts">
import PageLayout from "@/layouts/PageLayout.vue"
import ProductBanner from "@/components/products/ProductBanner.vue"
import ProductList from "@/components/products/ProductList.vue"
</script>

<template>
  <PageLayout>
    <ProductBanner></ProductBanner>
    <ProductList></ProductList>
  </PageLayout>
</template>
