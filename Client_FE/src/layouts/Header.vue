<script setup lang="ts">
import { ref, computed } from "vue"
import router from "@/router/index.ts"
import useAddToWishList from "@/composables/useAddToWishList"
import useClickOutside from "@/composables/useClickOutside"
import { formatMoney } from "@/helpers/common"
import { Notivue, Notification } from "notivue"
import { useMainStore } from "@/stores/main"
const store = useMainStore()
import type { ProductType } from "@/types/main"

const toggleOpen = ref(null)
const toggleClose = ref(null)
const collapseMenu = ref()

const handleClick = () => {
  if (collapseMenu.value.style.display === "block") {
    collapseMenu.value.style.display = "none"
  } else {
    collapseMenu.value.style.display = "block"
  }
}
const currentPath = computed(() => {
  return router.options.history.state.current
})

// wish list
const { getWishlist, removeFromWishList } = useAddToWishList()
const dropdownRef = ref()
const exceptionRef = ref()
const showDropdownWishList = ref(false)

useClickOutside(
  dropdownRef,
  () => {
    showDropdownWishList.value = false
  },
  exceptionRef
)

// select product
const selectProductToApplyDesign = (product: ProductType) => {
  showDropdownWishList.value = false
  const selectedDesign = store.getSelectedDesign
  store.setSelectedProduct(product)
  if (!selectedDesign.id) {
    push.success({
      title: "Thông báo",
      message: "Đã chọn sản phẩm, tiếp tục chọn thiết kế để in.",
    })
    router.push({ name: "DesignPage" })
  } else {
    router.push({ name: "OrderPage" })
  }
}
</script>

<template>
  <header
    class="shadow-md bg-white font-[sans-serif] tracking-wide z-50 fixed top-0 w-full"
  >
    <section
      class="flex items-center flex-wrap lg:justify-center gap-4 py-2.5 sm:px-10 px-4 border-gray-200 border-b min-h-[70px] relative"
    >
      <div
        class="left-10 z-50 flex flex-col px-4 py-2.5 pt-6 rounded max-lg:hidden font-oi bg-gradient-to-r from-yellow-200 to-sky-500 bg-clip-text text-transparent"
      >
        <div>Print Your Vision,</div>
        <div class="ml-20">Share Your Style</div>
      </div>

      <a href="javascript:void(0)" class="max-sm:hidden absolute left-10"
        ><img src="@/assets/images/print.svg" alt="logo" class="w-14" />
      </a>
      <a href="javascript:void(0)" class="hidden max-sm:block"
        ><img
          src="https://readymadeui.com/readymadeui-short.svg"
          alt="logo"
          class="w-9"
        />
      </a>

      <div class="lg:absolute lg:right-10 flex items-center ml-auto space-x-8">
        <span class="relative">
          <svg
            ref="exceptionRef"
            @click="showDropdownWishList = !showDropdownWishList"
            xmlns="http://www.w3.org/2000/svg"
            width="20px"
            class="cursor-pointer fill-gray-800 hover:fill-[#007bff] inline-block"
            viewBox="0 0 64 64"
          >
            <path
              d="M45.5 4A18.53 18.53 0 0 0 32 9.86 18.5 18.5 0 0 0 0 22.5C0 40.92 29.71 59 31 59.71a2 2 0 0 0 2.06 0C34.29 59 64 40.92 64 22.5A18.52 18.52 0 0 0 45.5 4ZM32 55.64C26.83 52.34 4 36.92 4 22.5a14.5 14.5 0 0 1 26.36-8.33 2 2 0 0 0 3.27 0A14.5 14.5 0 0 1 60 22.5c0 14.41-22.83 29.83-28 33.14Z"
              data-original="#000000"
            />
          </svg>
          <span
            class="absolute left-auto -ml-1 top-0 rounded-full bg-blue-600 px-1 py-0 text-xs text-white"
            >{{ getWishlist.length }}</span
          >
          <div
            ref="dropdownRef"
            v-if="showDropdownWishList && getWishlist.length"
            id="dropdownMenu"
            class="absolute block shadow-modal bg-white py-2 z-[999999999] min-w-full w-max rounded-lg max-h-[500px] overflow-auto top-full right-0 p-4"
          >
            <div
              class="font-sans max-w-5xl max-md:max-w-xl mx-auto bg-white py-4"
            >
              <h1 class="text-2xl font-bold text-rose-500 text-center">
                Danh sách yêu thích
              </h1>

              <div class="grid md:grid-cols-1 gap-8 mt-16">
                <template v-for="item in getWishlist" :key="item.id">
                  <div class="md:col-span-2 space-y-4 min-w-96">
                    <div class="grid grid-cols-3 items-start gap-4">
                      <div class="col-span-2 flex items-start gap-4">
                        <div
                          class="w-28 h-28 max-sm:w-24 max-sm:h-24 shrink-0 bg-gray-100 p-2 rounded-md"
                        >
                          <img
                            :src="item.imageUrl"
                            class="w-full h-full object-contain"
                          />
                        </div>

                        <div class="flex flex-col">
                          <h3 class="text-base font-bold text-gray-800">
                            {{ item.name }}
                          </h3>
                        </div>
                      </div>

                      <div class="ml-auto">
                        <h4
                          class="text-md max-sm:text-base font-semibold text-sky-600"
                        >
                          {{ formatMoney(item.price) }}
                        </h4>
                        <div class="flex gap-4 justify-end">
                          <button
                            type="button"
                            @click="removeFromWishList(item.id)"
                            class="mt-16 font-semibold text-red-500 text-xs flex items-center gap-1 shrink-0"
                          >
                            <svg
                              xmlns="http://www.w3.org/2000/svg"
                              class="w-4 fill-current inline"
                              viewBox="0 0 24 24"
                            >
                              <path
                                d="M19 7a1 1 0 0 0-1 1v11.191A1.92 1.92 0 0 1 15.99 21H8.01A1.92 1.92 0 0 1 6 19.191V8a1 1 0 0 0-2 0v11.191A3.918 3.918 0 0 0 8.01 23h7.98A3.918 3.918 0 0 0 20 19.191V8a1 1 0 0 0-1-1Zm1-3h-4V2a1 1 0 0 0-1-1H9a1 1 0 0 0-1 1v2H4a1 1 0 0 0 0 2h16a1 1 0 0 0 0-2ZM10 4V3h4v1Z"
                                data-original="#000000"
                              ></path>
                              <path
                                d="M11 17v-7a1 1 0 0 0-2 0v7a1 1 0 0 0 2 0Zm4 0v-7a1 1 0 0 0-2 0v7a1 1 0 0 0 2 0Z"
                                data-original="#000000"
                              ></path>
                            </svg>
                          </button>
                          <button
                            @click="selectProductToApplyDesign(item)"
                            type="button"
                            class="mt-16 font-semibold text-red-500 text-xs flex items-center gap-1 shrink-0"
                          >
                            <svg
                              width="20"
                              height="20"
                              viewBox="0 0 1024 1024"
                              class="icon"
                              version="1.1"
                              xmlns="http://www.w3.org/2000/svg"
                            >
                              <path
                                d="M192 234.666667h640v64H192z"
                                fill="#424242"
                              />
                              <path
                                d="M85.333333 533.333333h853.333334v-149.333333c0-46.933333-38.4-85.333333-85.333334-85.333333H170.666667c-46.933333 0-85.333333 38.4-85.333334 85.333333v149.333333z"
                                fill="#616161"
                              />
                              <path
                                d="M170.666667 768h682.666666c46.933333 0 85.333333-38.4 85.333334-85.333333v-170.666667H85.333333v170.666667c0 46.933333 38.4 85.333333 85.333334 85.333333z"
                                fill="#424242"
                              />
                              <path
                                d="M853.333333 384m-21.333333 0a21.333333 21.333333 0 1 0 42.666667 0 21.333333 21.333333 0 1 0-42.666667 0Z"
                                fill="#00E676"
                              />
                              <path
                                d="M234.666667 85.333333h554.666666v213.333334H234.666667z"
                                fill="#90CAF9"
                              />
                              <path
                                d="M800 661.333333h-576c-17.066667 0-32-14.933333-32-32s14.933333-32 32-32h576c17.066667 0 32 14.933333 32 32s-14.933333 32-32 32z"
                                fill="#242424"
                              />
                              <path
                                d="M234.666667 661.333333h554.666666v234.666667H234.666667z"
                                fill="#90CAF9"
                              />
                              <path
                                d="M234.666667 618.666667h554.666666v42.666666H234.666667z"
                                fill="#42A5F5"
                              />
                              <path
                                d="M341.333333 704h362.666667v42.666667H341.333333zM341.333333 789.333333h277.333334v42.666667H341.333333z"
                                fill="#1976D2"
                              />
                            </svg>
                          </button>
                        </div>
                      </div>
                    </div>
                    <hr class="border-gray-300" />
                  </div>
                </template>
              </div>
            </div>
          </div>
        </span>
      </div>
    </section>

    <div class="flex flex-wrap justify-center px-10 py-3 relative">
      <div
        ref="collapseMenu"
        class="max-lg:hidden lg:!block max-lg:before:fixed max-lg:before:bg-black max-lg:before:opacity-40 max-lg:before:inset-0 max-lg:before:z-50"
      >
        <button
          ref="toggleClose"
          @click="handleClick"
          class="lg:hidden fixed top-2 right-4 z-[100] rounded-full bg-white w-9 h-9 flex items-center justify-center border"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="w-3.5 h-3.5 fill-black"
            viewBox="0 0 320.591 320.591"
          >
            <path
              d="M30.391 318.583a30.37 30.37 0 0 1-21.56-7.288c-11.774-11.844-11.774-30.973 0-42.817L266.643 10.665c12.246-11.459 31.462-10.822 42.921 1.424 10.362 11.074 10.966 28.095 1.414 39.875L51.647 311.295a30.366 30.366 0 0 1-21.256 7.288z"
              data-original="#000000"
            ></path>
            <path
              d="M287.9 318.583a30.37 30.37 0 0 1-21.257-8.806L8.83 51.963C-2.078 39.225-.595 20.055 12.143 9.146c11.369-9.736 28.136-9.736 39.504 0l259.331 257.813c12.243 11.462 12.876 30.679 1.414 42.922-.456.487-.927.958-1.414 1.414a30.368 30.368 0 0 1-23.078 7.288z"
              data-original="#000000"
            ></path>
          </svg>
        </button>

        <ul
          class="lg:flex lg:gap-x-10 max-lg:space-y-3 max-lg:fixed max-lg:bg-white max-lg:w-2/3 max-lg:min-w-[280px] max-lg:top-0 max-lg:left-0 max-lg:p-4 max-lg:h-full max-lg:shadow-md max-lg:overflow-auto z-50"
        >
          <li class="max-lg:border-b max-lg:pb-4 px-3 lg:hidden">
            <a href="javascript:void(0)"
              ><img src="@/assets/images/print.svg" alt="logo" class="w-20" />
            </a>
          </li>
          <li class="max-lg:border-b max-lg:px-3 max-lg:py-3">
            <router-link
              to="/"
              href="javascript:void(0)"
              class="hover:text-[#007bff] block text-[15px]"
              :class="
                currentPath === '/'
                  ? 'hover:text-[#007bff] !text-[#007bff] '
                  : ''
              "
              >Trang chủ</router-link
            >
          </li>
          <li class="group max-lg:border-b max-lg:px-3 max-lg:py-3 relative">
            <router-link
              to="/products"
              href="javascript:void(0)"
              class="hover:text-[#007bff] hover:fill-[#007bff] text-gray-800 text-[15px] flex items-center"
              :class="currentPath === '/products' ? ' !text-[#007bff] ' : ''"
              >Sản phẩm<svg
                xmlns="http://www.w3.org/2000/svg"
                width="16px"
                height="16px"
                class="ml-1 inline-block"
                viewBox="0 0 24 24"
              >
                <path
                  d="M12 16a1 1 0 0 1-.71-.29l-6-6a1 1 0 0 1 1.42-1.42l5.29 5.3 5.29-5.29a1 1 0 0 1 1.41 1.41l-6 6a1 1 0 0 1-.7.29z"
                  data-name="16"
                  data-original="#000000"
                />
              </svg>
            </router-link>
            <ul
              class="absolute top-5 max-lg:top-8 left-0 z-50 block space-y-2 shadow-lg bg-white max-h-0 overflow-hidden min-w-[230px] group-hover:opacity-100 group-hover:max-h-[700px] px-6 group-hover:pb-4 group-hover:pt-6 transition-all duration-[400ms]"
            >
              <li class="border-b py-3">
                <a
                  href="javascript:void(0)"
                  class="hover:text-[#007bff] hover:fill-[#007bff] text-gray-800 text-[15px] flex items-center"
                >
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    width="20px"
                    height="20px"
                    class="mr-4 inline-block"
                    viewBox="0 0 64 64"
                  >
                    <path
                      d="M61.92 30.93a7.076 7.076 0 0 0-6.05-5.88 8.442 8.442 0 0 0-.87-.04V22A15.018 15.018 0 0 0 40 7H24A15.018 15.018 0 0 0 9 22v3.01a8.442 8.442 0 0 0-.87.04 7.076 7.076 0 0 0-6.05 5.88A6.95 6.95 0 0 0 7 38.7V52a3.009 3.009 0 0 0 3 3v6a1 1 0 0 0 1 1h3a1 1 0 0 0 .96-.73L16.75 55h30.5l1.79 6.27A1 1 0 0 0 50 62h3a1 1 0 0 0 1-1v-6a3.009 3.009 0 0 0 3-3V38.7a6.95 6.95 0 0 0 4.92-7.77ZM11 22A13.012 13.012 0 0 1 24 9h16a13.012 13.012 0 0 1 13 13v3.3a6.976 6.976 0 0 0-5 6.7v3.18a3 3 0 0 0-1-.18H17a3 3 0 0 0-1 .18V32a6.976 6.976 0 0 0-5-6.7Zm37 16v5H16v-5a1 1 0 0 1 1-1h30a1 1 0 0 1 1 1ZM13.25 60H12v-5h2.67ZM52 60h-1.25l-1.42-5H52Zm3.83-23.08a1.008 1.008 0 0 0-.83.99V52a1 1 0 0 1-1 1H10a1 1 0 0 1-1-1V37.91a1.008 1.008 0 0 0-.83-.99 4.994 4.994 0 0 1 .2-9.88A4.442 4.442 0 0 1 9 27h.01a4.928 4.928 0 0 1 3.3 1.26A5.007 5.007 0 0 1 14 32v12a1 1 0 0 0 1 1h34a1 1 0 0 0 1-1V32a5.007 5.007 0 0 1 1.69-3.74 4.932 4.932 0 0 1 3.94-1.22 5.018 5.018 0 0 1 4.31 4.18v.01a4.974 4.974 0 0 1-4.11 5.69Z"
                      data-original="#000000"
                    />
                  </svg>
                  Nội thất
                </a>
              </li>
              <li class="border-b py-3">
                <a
                  href="javascript:void(0)"
                  class="hover:text-[#007bff] hover:fill-[#007bff] text-gray-800 text-[15px] flex items-center"
                >
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    width="20px"
                    height="20px"
                    class="mr-4 inline-block"
                    viewBox="0 0 407.7 407.7"
                  >
                    <path
                      d="M405.5 118.021a7.93 7.93 0 0 0-.29-.29l-84.16-74.8a7.994 7.994 0 0 0-2.64-1.6l-60.88-21.76a8 8 0 0 0-10.72 7.12c0 1.76-2.64 42.32-43.2 42.96-40.8-.64-43.36-41.2-43.44-42.96a8 8 0 0 0-10.64-7.12l-60.8 22c-.976.357-1.872.9-2.64 1.6l-83.6 74.56a8 8 0 0 0 0 11.6l66.56 67.28v184a8 8 0 0 0 8 8h253.6a8 8 0 0 0 8-8v-184l66.56-67.28a8 8 0 0 0 .29-11.31zm-67.09 55.79v-37.12a8 8 0 0 0-16 0v235.52H84.73v-235.52a8 8 0 0 0-16 0v37.2l-49.2-49.84 76.16-68.16 50.08-18.08c6.204 31.966 37.147 52.851 69.113 46.647 23.607-4.582 42.065-23.04 46.647-46.647l50.08 18.08 75.92 68.16-49.12 49.76z"
                      data-original="#000000"
                    />
                  </svg>
                  Quần áo
                </a>
              </li>
              <li class="border-b py-3">
                <a
                  href="javascript:void(0)"
                  class="hover:text-[#007bff] hover:fill-[#007bff] text-gray-800 text-[15px] flex items-center"
                >
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    width="20px"
                    height="20px"
                    class="mr-4 inline-block"
                    viewBox="0 0 512 512"
                  >
                    <path
                      d="M434.1 243.904h-5.955a95.572 95.572 0 0 1-61.022-22.072l-117.812-98.055a49.716 49.716 0 0 0-31.743-11.481c-27.361 0-49.621 22.26-49.621 49.621v11.586c0 22.572-18.364 40.937-40.937 40.937-15.844 0-30.407-9.279-37.102-23.639l-3.261-6.995c-7.434-15.944-23.604-26.246-41.195-26.246C20.39 157.56 0 177.949 0 203.012v118.792c0 42.954 34.946 77.9 77.9 77.9h356.2c42.954 0 77.9-34.946 77.9-77.9 0-42.954-34.946-77.9-77.9-77.9zm0 125.8H77.9c-17.829 0-33.403-9.799-41.65-24.287h439.5c-8.247 14.488-23.821 24.287-41.65 24.287zM30 315.419V203.012c0-8.521 6.932-15.452 15.452-15.452 5.98 0 11.478 3.503 14.005 8.923l3.261 6.994c11.601 24.884 36.837 40.963 64.293 40.963 39.115 0 70.937-31.822 70.937-70.937v-11.586c0-10.819 8.802-19.621 19.621-19.621a19.66 19.66 0 0 1 12.552 4.54l28.901 24.055-32.93 32.93 21.213 21.213 34.872-34.871 13.031 10.846-31.444 31.444 21.213 21.213 33.386-33.385 13.031 10.846-29.958 29.958 21.213 21.213 32.115-32.115c21.284 15.35 47.024 23.723 73.383 23.723h5.955c24.246 0 44.328 18.112 47.461 41.513H30z"
                      data-original="#000000"
                    />
                  </svg>
                  Giày dép
                </a>
              </li>
            </ul>
          </li>
          <li class="max-lg:border-b max-lg:px-3 max-lg:py-3">
            <router-link
              to="/allOrders"
              href="javascript:void(0)"
              class="hover:text-[#007bff] text-gray-800 text-[15px] block"
              :class="currentPath === '/allOrders' ? ' !text-[#007bff] ' : ''"
              >Đơn hàng
            </router-link>
          </li>
          <li class="max-lg:border-b max-lg:px-3 max-lg:py-3">
            <router-link
              to="/designs"
              href="javascript:void(0)"
              class="hover:text-[#007bff] text-gray-800 text-[15px] block"
              :class="currentPath === '/designs' ? ' !text-[#007bff] ' : ''"
              >Thiết kế
            </router-link>
          </li>
          <li class="max-lg:border-b max-lg:px-3 max-lg:py-3">
            <a
              href="javascript:void(0)"
              class="hover:text-[#007bff] text-gray-800 text-[15px] block"
              >Blog</a
            >
          </li>
          <li class="max-lg:border-b max-lg:px-3 max-lg:py-3">
            <a
              href="javascript:void(0)"
              class="hover:text-[#007bff] text-gray-800 text-[15px] block"
              >About</a
            >
          </li>
          <li class="max-lg:border-b max-lg:px-3 max-lg:py-3">
            <a
              href="javascript:void(0)"
              class="hover:text-[#007bff] text-gray-800 text-[15px] block"
              >Liên hệ</a
            >
          </li>
          <!-- <li class="max-lg:border-b max-lg:px-3 max-lg:py-3">
            <a
              href="javascript:void(0)"
              class="hover:text-[#007bff] text-gray-800 text-[15px] block"
              >Đối tác</a
            >
          </li>
          <li class="max-lg:border-b max-lg:px-3 max-lg:py-3">
            <a
              href="javascript:void(0)"
              class="hover:text-[#007bff] text-gray-800 text-[15px] block"
              >Xem thêm</a
            >
          </li> -->
        </ul>
      </div>

      <div ref="toggleOpen" class="flex ml-auto lg:hidden" @click="handleClick">
        <button>
          <svg
            class="w-7 h-7"
            fill="#000"
            viewBox="0 0 20 20"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              fill-rule="evenodd"
              d="M3 5a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM3 10a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM3 15a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1z"
              clip-rule="evenodd"
            ></path>
          </svg>
        </button>
      </div>
    </div>
    <Notivue v-slot="item">
      <Notification :item="item" />
    </Notivue>
  </header>
</template>
