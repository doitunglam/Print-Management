import { createRouter, createWebHistory } from "vue-router";
import LoginView from "@/views/LoginView.vue";
import UserManagement from "@/views/UserManagement.vue";
import TeamManagement from "@/views/TeamManagement.vue";
import RoleManagement from "@/views/RoleManagement.vue";
import ResourceManagement from "@/views/ResourceManagement.vue";
import ProjectManagement from "@/views/ProjectManagement.vue";
import PrintManagement from "@/views/PrintManagement.vue";
import DesignManagement from "@/views/DesignManagement.vue";
import ProductManagement from "@/views/ProductManagement.vue";
import OrderManagement from "@/views/OrderManagement.vue";
import ShippingManagement from "@/views/ShippingManagement.vue";
import ResourcePropertyManagement from "@/views/ResourcePropertyManagement.vue";
import ResourcePropertyDetailsManagement from "@/views/ResourcePropertyDetailsManagement.vue";
import FlowManagement from "@/views/FlowManagement.vue";
import RegisterView from "@/views/RegisterView.vue";
import CustomerManagement from "@/views/Customer.vue";

const routes = [
  { path: "/login", name: "Login", component: LoginView },
  { path: "/users", name: "Users", component: UserManagement },
  { path: "/team", name: "Team", component: TeamManagement },
  { path: "/roles", name: "Roles", component: RoleManagement },
  { path: "/resources", name: "Resources", component: ResourceManagement },
  {
    path: "/project-management",
    name: "ProjectManagement",
    component: ProjectManagement,
  },
  {
    path: "/print-management",
    name: "PrintManagement",
    component: PrintManagement,
  },
  {
    path: "/design-management",
    name: "DesignManagement",
    component: DesignManagement,
  },
  {
    path: "/product-management",
    name: "ProductManagement",
    component: ProductManagement,
  },
  {
    path: "/order-management",
    name: "OrderManagement",
    component: OrderManagement,
  },
  {
    path: "/shipper-management",
    name: "ShipperManagement",
    component: ShippingManagement,
  },
  {
    path: "/resource-properties",
    name: "ResourceProperties",
    component: ResourcePropertyManagement,
  },
  {
    path: "/resource-property-details",
    name: "ResourcePropertyDetails",
    component: ResourcePropertyDetailsManagement,
  },
  {
    path: "/flow-management",
    name: "FlowManagement",
    component: FlowManagement,
  },
  {
    path: "/register",
    name: "Register",
    component: RegisterView,
    meta: { requiresAuth: false },
  },
  {
    path: "/customer",
    name: "CustomerManagement",
    component: CustomerManagement,
  },
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,
});

router.beforeEach((to, from, next) => {
  const publicPages = ["/login", "/register"];
  const authRequired = !publicPages.includes(to.path);
  const loggedIn = localStorage.getItem("user");

  console.log("Checking authentication...");
  console.log("Auth required:", authRequired);
  console.log("Logged in:", loggedIn);

  if (authRequired && !loggedIn) {
    console.log("Redirecting to login...");
    return next("/login");
  }

  next();
});

export default router;
