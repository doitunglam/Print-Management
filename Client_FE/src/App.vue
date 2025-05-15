<script setup lang="ts">
import { StyleProvider } from 'ant-design-vue';
import { ConfigProvider } from 'ant-design-vue';
import { RouterLink, RouterView } from "vue-router"
</script>

<template>
  <StyleProvider layer>
    <ConfigProvider>
      <router-view v-slot="{ Component, route }">
        <transition :name="route.meta.transition || ''">
          <component :is="Component" />
        </transition>
      </router-view>
    </ConfigProvider>
  </StyleProvider>
</template>

<style scoped></style>
