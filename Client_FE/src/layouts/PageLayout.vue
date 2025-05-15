<script setup lang="ts">
import Footer from "@/layouts/Footer.vue"
import Header from "@/layouts/Header.vue"
</script>

<template>
  <div class="flex h-screen bg-gray-200 font-roboto">
    <div class="flex-1 flex flex-col overflow-x-hidden overflow-y-auto">
      <Header />

      <main class="flex-1 bg-gray-200 mt-[150px]">
        <slot />
      </main>

      <Footer />
    </div>
  </div>
</template>
