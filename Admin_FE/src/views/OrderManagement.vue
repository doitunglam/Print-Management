<template>
  <div class="order-management">
    <a-breadcrumb style="margin: 16px 0">
      <a-breadcrumb-item>Trang chủ</a-breadcrumb-item>
      <a-breadcrumb-item>Quản lý đơn hàng</a-breadcrumb-item>
    </a-breadcrumb>
    <a-table :columns="columns" :dataSource="orders" :pagination="{ pageSize: 10 }" rowKey="id">
      <template #bodyCell="{ column, record }">
        <span v-if="column.key === 'productImage'">
          <img :src="record.productImage" alt="" class="product-image" />
        </span>
        <span v-else-if="column.key === 'designImage'">
          <img :src="record.designImage" alt="" class="product-image" />
        </span>
        <span v-else-if="column.key === 'actions'">
          <a @click="showEditStepPopUp(record)">Đổi trạng thái</a>
        </span>
        <span v-else>{{ record[column.dataIndex] || "-" }}</span>
      </template>
    </a-table>
  </div>

  <!-- Modal Thay Đổi Trạng Thái-->
  <a-modal v-model:visible="isModalVisible" title="Xác nhận luồng xử lý" @ok="submitAddFlowTemplateModal">
    <a-form :model="editOrder" ref="editOrderStepForm" layout="vertical">
      <div v-for="step in editOrder.steps" :key="step.position">
        <p style="margin-bottom: 8px;">
          Bước: {{ step.name }}
        </p>
        <a-form-item style="flex-direction: row;">
          <p style="margin-bottom: 2px;">Trạng thái</p>
          <a-select v-model:value="step.status" placeholder="Nhập trạng thái của bước xử lý">
            <a-select-option v-for="orderStepStatus in orderStepStatues" :key="orderStepStatus.value"
              :value="orderStepStatus.value">
              {{ orderStepStatus.name }}
            </a-select-option>
          </a-select>
          <div v-if="step.status == 'Others'" style="margin-top: 8px;">
            <p style="margin-bottom: 2px;">Chi tiết</p>
            <a-input v-model:value="step.note" :disabled="step.status !== 'Others'"
              placeholder='Nếu chọn "Khác", vui lòng nhập chi tiết' />
          </div>
        </a-form-item>
      </div>
    </a-form>
  </a-modal>
</template>

<script>
import { getAllOrders, getOrder, setSteps } from "@/apis/orderApi"
import { getAllProducts } from "@/apis/productApi"
import { getAllDesigns } from "@/apis/projectApi"
import { message } from "ant-design-vue"
export default {
  name: "OrderManagement",
  data() {
    return {
      orders: [],
      products: [],
      designs: [],
      columns: [
        {
          title: "Tên sản phẩm",
          dataIndex: "productName",
          key: "productName",
        },
        {
          title: "Hình ảnh sản phẩm",
          dataIndex: "productImage",
          key: "productImage",
        },
        {
          title: "Thiết kế",
          dataIndex: "designImage",
          key: "designImage",
        },
        {
          title: "trạng thái",
          dataIndex: "status",
          key: "status",
        },
        {
          title: "Tên người nhận",
          dataIndex: "customerName",
          key: "customerName",
        },
        {
          title: "SĐT người nhận",
          dataIndex: "phoneNumber",
          key: "phoneNumber",
        },
        {
          title: "Địa chỉ người nhận",
          dataIndex: "deliveryAddress",
          key: "deliveryAddress",
        },
        {
          title: "Thời gian tạo đơn",
          dataIndex: "orderDate",
          key: "orderDate",
        },
        {
          title: "Hành động",
          key: "actions",
          scopedSlots: { customRender: "actions" },
        },
      ],
      editOrder: null,
      isModalVisible: false,
      orderStepStatues: [
        {
          name: "<Không xác định>",
          value: ""
        },
        {
          name: "Đang xử lý",
          value: "Processing"
        },
        {
          name: "Đã hoàn thành",
          value: "Completed"
        },
        {
          name: "Đã hủy",
          value: "Aborted"
        },
        {
          name: "Khác",
          value: "Others"
        }
      ]
    }
  },
  methods: {
    async fetchOrders() {
      try {
        const result = await getAllOrders()
        this.transformOrders(result)
      } catch (error) {
        console.error("Lỗi khi lấy danh sách đơn hàng:", error)
      }
    },
    async fetchProducts() {
      try {
        this.products = await getAllProducts()
        console.log("this.products", this.products)
      } catch (error) {
        console.error("Lỗi khi lấy danh sách sản phẩm:", error)
      }
    },
    async fetchDesigns() {
      try {
        this.designs = await getAllDesigns()
      } catch (error) {
        console.error("Lỗi khi lấy danh sách thiết kế:", error)
      }
    },

    transformOrders(orders) {
      this.orders = orders.map((order) => {
        const product = this.products.find(
          (product) => product.id === order.productID
        )
        const design = this.designs.find(
          (design) => design.id === order.designID
        )

        return {
          productName: product?.name ?? "",
          productImage: product?.imageUrl ?? "",
          designImage: design?.filePath ?? "",
          ...order,
        }
      })
    },
    async showEditStepPopUp(record) {
      const order = await getOrder(record.id)
      this.editOrder = order
      this.isModalVisible = true
    },
    async submitAddFlowTemplateModal() {
      try {
        const steps = { steps: this.editOrder.steps }
        await setSteps(steps)
        message.success("Cập nhật luồng xử lý thành công!")
        this.isModalVisible = false
      } catch (error) {
        console.error("Lỗi khi sửa luồng xử lý:", error)
      }

    }
  },
  async mounted() {
    await Promise.all([this.fetchProducts(), this.fetchDesigns(), this.fetchOrders()])
  },
}
</script>

<style scoped>
.actions-bar {
  display: flex;
  justify-content: space-between;
  margin-bottom: 16px;
}

.product-image {
  height: 100px;
  width: auto;
}

p {
  margin: 0;
}
</style>
