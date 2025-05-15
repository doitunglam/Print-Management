<script setup lang="ts">
import PageLayout from "@/layouts/PageLayout.vue"
import OrderDetail from "@/components/order/OrderDetail.vue"
import { useRoute } from 'vue-router'

const route = useRoute()
</script>

<template>
  <PageLayout>
    <OrderDetail :id="Number.parseInt(route.params.id as string)"></OrderDetail>
  </PageLayout>
</template>
