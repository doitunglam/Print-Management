import { createRouter, createWebHistory } from "vue-router"
import HomePage from "@/views/HomePage.vue"
import NotFoundPage from "@/views/NotFoundPage.vue"
import ProductPage from "@/views/ProductPage.vue"
import DesignPage from "@/views/DesignPage.vue"
import ProductDetailPage from "@/views/ProductDetailPage.vue"
import OrderPage from "@/views/OrderPage.vue"
import OrderDetailPage from "@/views/OrderDetailPage.vue"
import AllOrdersPage from "@/views/AllOrdersPage.vue"

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: "/",
      name: "HomePage",
      component: HomePage,
    },
    {
      path: "/products",
      name: "ProductPage",
      component: ProductPage,
    },
    {
      path: "/product/:id",
      name: "ProductDetailPage",
      component: ProductDetailPage,
      props: true,
    },
    {
      path: "/designs",
      name: "DesignPage",
      component: DesignPage,
    },
    {
      path: "/order",
      name: "OrderPage",
      component: OrderPage,
    },
    {
      path: "/order/:id",
      name: "OrderDetailPage",
      component: OrderDetailPage,
    },
    {
      path: "/allOrders",
      name: "AllOrdersPage",
      component: AllOrdersPage,
    },

    { path: `/:notFound(.*)`, component: NotFoundPage },
  ],
})

export default router
