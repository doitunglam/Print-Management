<template>
  <a-layout-sider
    v-model:collapsed="collapsed"
    collapsible
    width="200"
    class="site-layout-background"
  >
    <a-menu
      mode="inline"
      :default-selected-keys="[selectedKey]"
      style="height: 100%; border-right: 0"
    >
      <template v-if="isAdmin">
        <a-sub-menu key="projectManagement" title="Quản lý dự án">
          <a-menu-item v-for="item in projectManagementItems" :key="item.key">
            <router-link :to="item.route">{{ item.label }}</router-link>
          </a-menu-item>
        </a-sub-menu>
        <a-menu-item v-for="item in otherMenuItems" :key="item.key">
          <router-link :to="item.route">{{ item.label }}</router-link>
        </a-menu-item>
      </template>
      <template v-else>
        <a-menu-item v-for="item in menuItems" :key="item.key">
          <router-link :to="item.route">{{ item.label }}</router-link>
        </a-menu-item>
      </template>
    </a-menu>
  </a-layout-sider>
</template>

<script>
export default {
  name: "AppSidebar",
  computed: {
    isAdmin() {
      const role = localStorage.getItem("roles") || ""
      return role.trim().replace(/"/g, "") === "Admin"
    },
    selectedKey() {
      const path = this.$route.path
      if (path === "/") return "1"
      if (path === "/categories") return "5"
      return "1"
    },
    menuItems() {
      const role = localStorage.getItem("roles") || ""

      const roleMenuMap = {
        User: [
          { key: "9", label: "Quản lý dự án", route: "/project-management" },
          { key: "10", label: "Quản lý in", route: "/print-management" },
          { key: "11", label: "Quản lý thiết kế", route: "/design-management" },
        ],
        Admin: [
          { key: "7", label: "Quản lý tài khoản", route: "/roles" },
          { key: "6", label: "Quản lý đội ngũ", route: "/team" },
          { key: "9", label: "Quản lý dự án", route: "/project-management" },
          { key: "11", label: "Quản lý thiết kế", route: "/design-management" },
          { key: "10", label: "Quản lý in", route: "/print-management" },
          { key: "8", label: "Quản lý tài nguyên", route: "/resources" },
          {
            key: "12",
            label: "Quản lý thuộc tính tài nguyên",
            route: "/resource-properties",
          },
          {
            key: "13",
            label: "Quản lý chi tiết thuộc tính tài nguyên",
            route: "/resource-property-details",
          },
          {
            key: "5",
            label: "Quản lý giao hàng",
            route: "/shipper-management",
          },
          { key: "14", label: "Danh sách khách hàng", route: "/customer" },
          { key: "15", label: "Quản lý luồng đặt hàng", route: "/flow-management" },
        ],
        Employee: [
          { key: "9", label: "Quản lý dự án", route: "/project-management" },
          { key: "11", label: "Quản lý thiết kế", route: "/design-management" },
          { key: "10", label: "Quản lý in", route: "/print-management" },
          {
            key: "12",
            label: "Quản lý thuộc tính tài nguyên",
            route: "/resource-properties",
          },
          {
            key: "13",
            label: "Quản lý chi tiết thuộc tính tài nguyên",
            route: "/resource-property-details",
          },
          { key: "14", label: "Danh sách khách hàng", route: "/customer" },
        ],
        Designer: [
          { key: "11", label: "Quản lý thiết kế", route: "/design-management" },
          { key: "9", label: "Quản lý dự án", route: "/project-management" },
        ],
        Shipper: [
          {
            key: "5",
            label: "Quản lý giao hàng",
            route: "/shipper-management",
          },
          { key: "9", label: "Quản lý dự án", route: "/project-management" },
          { key: "14", label: "Danh sách khách hàng", route: "/customer" },
        ],
      }

      return roleMenuMap[role.trim().replace(/"/g, "")] || []
    },
    projectManagementItems() {
      return [
        { key: "9", label: "Quản lý dự án", route: "/project-management" },
        {
          key: "15",
          label: "Quản lý sản phẩm",
          route: "/product-management",
        },
        {
          key: "16",
          label: "Quản lý đơn hàng",
          route: "/order-management",
        },
        { key: "11", label: "Quản lý thiết kế", route: "/design-management" },
        { key: "10", label: "Quản lý in", route: "/print-management" },
        {
          key: "13",
          label: "Quản lý chi tiết thuộc tính tài nguyên",
          route: "/resource-property-details",
        },
      ]
    },
    otherMenuItems() {
      return this.menuItems.filter(
        (item) => !["9", "11", "10", "13"].includes(item.key)
      )
    },
  },
  methods: {
    navigateTo(route) {
      this.$router.push(route)
    },
  },
}
</script>

<style scoped></style>
