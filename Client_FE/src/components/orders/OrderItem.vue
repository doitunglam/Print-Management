<script lang="ts" setup>
import { formatMoney } from "@/helpers/common"
import { useMainStore } from "@/stores/main"
const store = useMainStore()
import { computed, reactive, ref } from "vue"
import { useCreateOrder } from "@/composables/useApi"
import { Notivue, Notification } from "notivue"
import router from "@/router"
import type { ProductType, DesignType } from "@/types/main"

const selectedProduct = computed(() => store.getSelectedProduct)
const selectedDesign = computed(() => store.getSelectedDesign)

const customerFirstName = ref("")
const customerLastName = ref("")
const customerAddress = ref("")
const customerProvince = ref("")
const customerDistrict = ref("")
const customerWard = ref("")
const order = reactive({
  productID: selectedProduct.value.id,
  designID: selectedDesign.value.id,
  orderDate: new Date(),
  status: "pending",
  deliveryAddress: "",
  phoneNumber: "",
  customerID: 0,
  customerName: "",
})

const submitOrder = async () => {
  order.deliveryAddress = `${customerAddress.value}, ${customerWard.value}, ${customerDistrict.value}, ${customerProvince.value}`
  order.customerName = `${customerLastName.value} ${customerFirstName.value}`

  try {
    await useCreateOrder(order)
    push.success({
      title: "Chúc mừng",
      message: "Tạo đơn thành công!",
    })
    store.setSelectedProduct({} as ProductType)
    store.setSelectedDesign({} as DesignType)
    router.push("/")
  } catch (e) {
    console.error(e)
  }
}
</script>

<template>
  <div class="font-sans max-w-4xl max-md:max-w-xl mx-auto mt-6">
    <h1 class="text-2xl font-bold text-sky-600">Sản phẩm bạn đã chọn</h1>
    <div class="grid md:grid-cols-3 gap-4 mt-8">
      <div class="md:col-span-2 space-y-4">
        <div
          class="flex gap-4 bg-white px-4 py-6 rounded-md shadow-[0_2px_12px_-3px_rgba(61,63,68,0.3)]"
        >
          <div class="flex gap-4">
            <div class="w-28 h-28 max-sm:w-24 max-sm:h-24 shrink-0">
              <img
                :src="selectedProduct.imageUrl"
                class="w-full h-full object-contain"
              />
            </div>

            <div class="flex flex-col gap-4">
              <div>
                <h3 class="text-sm sm:text-base font-bold text-gray-800">
                  {{ selectedProduct.name }}
                </h3>
                <p
                  class="text-sm font-semibold text-gray-500 mt-2 flex items-center gap-2"
                >
                  Màu sắc:
                  <span
                    class="inline-block w-5 h-5 rounded-md bg-white border border-gray-200"
                  ></span>
                </p>
              </div>
            </div>
          </div>
          <div class="ml-auto flex flex-col">
            <h3 class="text-sm sm:text-base font-bold text-gray-800 mt-auto">
              {{ formatMoney(selectedProduct.price) }}
            </h3>
          </div>
        </div>
        <div
          class="flex gap-4 bg-white px-4 py-6 rounded-md shadow-[0_2px_12px_-3px_rgba(61,63,68,0.3)]"
        >
          <div class="flex gap-4">
            <div class="w-28 h-28 max-sm:w-24 max-sm:h-24 shrink-0">
              <img
                :src="selectedDesign.filePath"
                class="w-full h-full object-contain"
              />
            </div>

            <div class="flex flex-col gap-4">
              <div>
                <h3 class="text-sm sm:text-base font-bold text-gray-800">
                  Mẫu thiết kế
                </h3>
              </div>
            </div>
          </div>
          <div class="ml-auto flex flex-col">
            <h3 class="text-sm sm:text-base font-bold text-gray-800 mt-auto">
              50.000 đ
            </h3>
          </div>
        </div>
      </div>

      <div
        class="bg-white rounded-md px-4 py-6 h-max shadow-[0_2px_12px_-3px_rgba(61,63,68,0.3)]"
      >
        <ul class="text-gray-800 space-y-4">
          <li class="flex flex-wrap gap-4 text-sm">
            Tổng phí
            <span class="ml-auto font-bold">{{
              formatMoney(selectedProduct.price + 50000)
            }}</span>
          </li>
          <li class="flex flex-wrap gap-4 text-sm">
            Phí vận chuyển <span class="ml-auto font-bold">25.000 đ</span>
          </li>
          <li class="flex flex-wrap gap-4 text-sm">
            VAT
            <span class="ml-auto font-bold">{{
              formatMoney((selectedProduct.price + 50000) * 0.1)
            }}</span>
          </li>
          <hr class="border-gray-300" />
          <li class="flex flex-wrap gap-4 text-sm font-bold">
            Thành tiền
            <span class="ml-auto">{{
              formatMoney(
                selectedProduct.price +
                  50000 +
                  (selectedProduct.price + 50000) * 0.1 +
                  25000
              )
            }}</span>
          </li>
        </ul>

        <div class="mt-8 space-y-2">
          <router-link to="/">
            <button
              type="button"
              class="text-sm px-4 py-2.5 w-full font-semibold tracking-wide bg-transparent hover:bg-gray-100 text-gray-800 border border-gray-300 rounded-md"
            >
              Tiếp tục mua sắm
            </button>
          </router-link>
        </div>

        <div class="mt-4 flex flex-wrap justify-center gap-4">
          <img
            src="https://readymadeui.com/images/master.webp"
            alt="card1"
            class="w-10 object-contain"
          />
          <img
            src="https://readymadeui.com/images/visa.webp"
            alt="card2"
            class="w-10 object-contain"
          />
          <img
            src="https://readymadeui.com/images/american-express.webp"
            alt="card3"
            class="w-10 object-contain"
          />
        </div>
      </div>
    </div>
  </div>
  <div class="font-[sans-serif] gap-10 mx-auto max-w-4xl mt-6 mb-20">
    <div class="font-[sans-serif] bg-white">
      <div class="flex justify-center items-center">
        <div class="max-w-4xl w-full h-max rounded-md px-4 py-8 sticky top-0">
          <h2 class="text-2xl font-bold text-gray-800">Hoàn thành đơn hàng</h2>
          <form class="mt-8">
            <div>
              <h3 class="text-sm lg:text-base text-gray-800 mb-4">
                Thông tin cá nhân
              </h3>
              <div class="grid md:grid-cols-2 gap-4">
                <div>
                  <input
                    type="text"
                    v-model="customerFirstName"
                    placeholder="Tên"
                    class="px-4 py-3 bg-gray-100 focus:bg-transparent text-gray-800 w-full text-sm rounded-md focus:outline-blue-600"
                  />
                </div>

                <div>
                  <input
                    type="text"
                    v-model="customerLastName"
                    placeholder="Họ"
                    class="px-4 py-3 bg-gray-100 focus:bg-transparent text-gray-800 w-full text-sm rounded-md focus:outline-blue-600"
                  />
                </div>

                <div>
                  <input
                    type="email"
                    placeholder="Email"
                    class="px-4 py-3 bg-gray-100 focus:bg-transparent text-gray-800 w-full text-sm rounded-md focus:outline-blue-600"
                  />
                </div>

                <div>
                  <input
                    type="text"
                    v-model="order.phoneNumber"
                    placeholder="Số điện thoại"
                    class="px-4 py-3 bg-gray-100 focus:bg-transparent text-gray-800 w-full text-sm rounded-md focus:outline-blue-600"
                  />
                </div>
              </div>
            </div>

            <div class="mt-8">
              <h3 class="text-sm lg:text-base text-gray-800 mb-4">
                Địa chỉ giao hàng
              </h3>
              <div class="grid md:grid-cols-2 gap-4">
                <div>
                  <input
                    type="text"
                    v-model="customerAddress"
                    placeholder="Tên đường, số nhà"
                    class="px-4 py-3 bg-gray-100 focus:bg-transparent text-gray-800 w-full text-sm rounded-md focus:outline-blue-600"
                  />
                </div>
                <div>
                  <input
                    type="text"
                    v-model="customerProvince"
                    placeholder="Tỉnh/Thành Phố"
                    class="px-4 py-3 bg-gray-100 focus:bg-transparent text-gray-800 w-full text-sm rounded-md focus:outline-blue-600"
                  />
                </div>
                <div>
                  <input
                    type="text"
                    v-model="customerDistrict"
                    placeholder="Quận/Huyện"
                    class="px-4 py-3 bg-gray-100 focus:bg-transparent text-gray-800 w-full text-sm rounded-md focus:outline-blue-600"
                  />
                </div>
                <div>
                  <input
                    type="text"
                    v-model="customerWard"
                    placeholder="Phường/Xã"
                    class="px-4 py-3 bg-gray-100 focus:bg-transparent text-gray-800 w-full text-sm rounded-md focus:outline-blue-600"
                  />
                </div>
              </div>

              <div class="flex gap-4 max-md:flex-col mt-8">
                <button
                  type="button"
                  @click="submitOrder"
                  class="rounded-md px-4 py-2.5 w-full text-sm tracking-wide bg-blue-600 hover:bg-blue-700 text-white"
                >
                  Đặt hàng
                </button>
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
  <Notivue v-slot="item">
    <Notification :item="item" />
  </Notivue>
</template>
