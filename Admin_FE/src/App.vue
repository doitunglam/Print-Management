<template>
  <div>
    <AppHeader v-if="isLoggedIn" />
    <a-layout style="min-height: 100vh">
      <AppSidebar v-if="isLoggedIn" />
      <a-layout>
        <a-layout-content style="margin: 0 16px">
          <router-view />
        </a-layout-content>
        <a-layout-footer style="text-align: center">
          Ant Design ©2024 Created by PrintPM
        </a-layout-footer>
      </a-layout>
    </a-layout>
  </div>
</template>


<script>

import AppHeader from './components/AppHeader.vue';
import AppSidebar from './components/AppSidebar.vue';
import { isLoggedIn } from './store/authState';

export default {
  name: 'App',
  components: {
    AppHeader,
    AppSidebar,
  },
  setup() {
    return { isLoggedIn };
  },
};
</script>

<style>
#app {
  display: flex;
  flex-direction: column;
}

.main-content {
  display: flex;
}

#components-layout-demo-side .logo {
  height: 32px;
  margin: 16px;
  background: rgba(255, 255, 255, 0.3);
}

.site-layout .site-layout-background {
  background: #fff;
}

[data-theme='dark'] .site-layout .site-layout-background {
  background: #141414;
}
</style>
