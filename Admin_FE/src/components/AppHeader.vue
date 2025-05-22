<template>
  <header>
    <a-layout-header style="background: #fff; padding: 0">
      <div class="header-content">
        <!-- <img style="width: 160px; objectFit: contain" src="https://cdn.skyltmax.se/vite/assets/nl-NL-764cbd98.svg" alt="Logo" class="logo" /> -->
        <div class="user-info">
          <span>Xin chào, {{ user ? user.userName : 'User'}} ID: {{ user ? user.id : ''}}</span>
          <a-dropdown>
            <a class="ant-dropdown-link" @click.prevent>
              <span>▼</span>
            </a>
            <template v-slot:overlay>
              <a-menu>
                <a-menu-item @click="handleLogout">Logout</a-menu-item>
              </a-menu>
            </template>
          </a-dropdown>
        </div>
      </div>
    </a-layout-header>
  </header>
</template>

<script>
import { Layout, Dropdown, Menu } from 'ant-design-vue';
import { isLoggedIn } from '@/store/authState';
import { getUserInfo } from '@/apis/authApi';

export default {
  name: 'AppHeader',
  components: {
    'a-layout-header': Layout.Header,
    'a-dropdown': Dropdown,
    'a-menu': Menu,
    'a-menu-item': Menu.Item,
  },
  data() {
    return {
      user: null,
    };
  },
  async created() {
    try {
      const userInfo = await getUserInfo();
      this.user = userInfo;
      const userRole = userInfo.roles.length > 1 ? userInfo.roles.find(role => role !== 'User') : userInfo.roles.find(role => role === 'User');
      console.log(userRole);
      localStorage.setItem('roles', JSON.stringify(userRole));
    } catch (error) {
      if (error.status === 401) {
        localStorage.removeItem('user');
        localStorage.removeItem('token');
        isLoggedIn.value = false;
        this.$router.push('/login');
      }
      console.error('Failed to fetch user info:', error);
    }
  },
  methods: {
    goToProfile() {
      console.log('Navigating to profile...');
    },
    handleLogout() {
      localStorage.removeItem('user');
      localStorage.removeItem('token');
      isLoggedIn.value = false;
      this.$router.push('/login');
    },
  },
};
</script>

<style scoped>
.header-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 20px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.logo {
  height: 40px;
}

.user-info {
  margin-left: auto;
  font-weight: bold;
  color: #333;
}
</style> 