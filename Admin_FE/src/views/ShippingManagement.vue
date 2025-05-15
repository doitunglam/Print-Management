<template>
  <div>
    <a-breadcrumb style="margin: 16px 0">
      <a-breadcrumb-item>Trang chủ</a-breadcrumb-item>
      <a-breadcrumb-item>Quản lý giao hàng</a-breadcrumb-item>
    </a-breadcrumb>
    <div class="actions-bar">
      <a-button type="primary" @click="showCreateModal"
        >Thêm giao hàng</a-button
      >
    </div>
    <a-table
      :columns="columns"
      :dataSource="deliveries"
      :pagination="{ pageSize: 10 }"
      rowKey="id"
    >
      <template #bodyCell="{ column, record }">
        <span v-if="column.key === 'actions'">
          <div>
            <a @click="updateDeliveryStatus(record.id)">Cập nhật trạng thái</a>
          </div>
          <!-- <div><a @click="showEditModal(record)">Sửa</a>
            <a-divider type="vertical" />
          <a-popconfirm
            title="Bạn có chắc chắn muốn xóa tài nguyên này không?"
            ok-text="Có"
            cancel-text="Không"
            @confirm="handleDelete(record.id)"
          >
            <a>Xóa</a>
          </a-popconfirm>
        </div> -->
        </span>
        <span v-else>{{ record[column.dataIndex] || "-" }}</span>
      </template>
    </a-table>
    <a-modal
      v-model:visible="isModalVisible"
      title="Giao hàng"
      @ok="handleOk"
      @cancel="handleCancel"
    >
      <a-form
        :model="formData"
        :rules="rules"
        ref="deliveryForm"
        layout="vertical"
      >
        <a-form-item
          label="Phương thức giao hàng (1: Chuyển phát nhanh, 2: Chuyển phát qua bưu điện)"
          name="shippingMethodId"
          style="margin-bottom: 10px"
        >
          <a-input-number
            v-model:value="formData.shippingMethodId"
            style="width: 100%"
          />
        </a-form-item>
        <!-- <a-form-item
          label="ID Khách hàng"
          name="customerId"
          style="margin-bottom: 10px"
        >
          <a-input-number
            v-model:value="formData.customerId"
            style="width: 100%"
          />
        </a-form-item> -->
        <a-form-item
          label="Khách hàng"
          name="customerId"
          style="margin-bottom: 10px"
        >
          <a-select
            v-model:value="formData.customerId"
            placeholder="Chọn khách hàng"
          >
            <a-select-option
              v-for="cus in customers"
              :key="cus.id"
              :value="cus.id"
              >{{ cus.fullName }}</a-select-option
            >
          </a-select>
        </a-form-item>
        <!-- <a-form-item
          label="ID Dự án"
          name="projectId"
          style="margin-bottom: 10px"
        >
          <a-input-number
            v-model:value="formData.projectId"
            style="width: 100%"
          />
        </a-form-item> -->
        <a-form-item label="Dự án" name="projectId" style="margin-bottom: 10px">
          <a-select v-model:value="formData.projectId" placeholder="Chọn dự án">
            <a-select-option
              v-for="pro in projects"
              :key="pro.id"
              :value="pro.id"
              >{{ pro.projectName }}</a-select-option
            >
          </a-select>
        </a-form-item>
        <a-form-item
          label="Thời gian giao hàng dự kiến"
          name="estimateDeliveryTime"
          style="margin-bottom: 10px"
        >
          <a-date-picker
            v-model:value="formData.estimateDeliveryTime"
            style="width: 100%"
          />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script>
import {
  getAllDeliveries,
  createDelivery,
  updateDeliveryStatus,
} from "@/apis/shippingApi"
import { getAllProjects } from "@/apis/projectApi"
import { getAllCustomers } from "@/apis/projectApi"
import { message } from "ant-design-vue"

export default {
  name: "ShippingManagement",
  data() {
    return {
      deliveries: [],
      customers: [],
      projects: [],
      columns: [
        {
          title: "Địa chỉ giao hàng",
          dataIndex: "deliveryAddress",
          key: "deliveryAddress",
        },
        {
          title: "Thời gian giao hàng dự kiến",
          dataIndex: "estimateDeliveryTime",
          key: "estimateDeliveryTime",
        },
        {
          title: "Thời gian giao hàng thực tế",
          dataIndex: "actualDeliveryTime",
          key: "actualDeliveryTime",
        },
        {
          title: "Trạng thái giao hàng",
          dataIndex: "deliveryStatus",
          key: "deliveryStatus",
        },
        {
          title: "ID Khách hàng",
          dataIndex: "customerId",
          key: "customerId",
        },
        {
          title: "ID Người giao hàng",
          dataIndex: "deliverId",
          key: "deliverId",
        },
        {
          title: "Hành động",
          key: "actions",
        },
      ],
      isModalVisible: false,
      formData: {
        shippingMethodId: 0,
        customerId: 0,
        projectId: 0,
        estimateDeliveryTime: null,
      },
      isEditing: false,
      rules: {
        shippingMethodId: [
          { required: true, message: "Vui lòng nhập phương thức giao hàng!" },
        ],
        customerId: [
          { required: true, message: "Vui lòng nhập ID khách hàng!" },
        ],
        projectId: [{ required: true, message: "Vui lòng nhập ID dự án!" }],
        estimateDeliveryTime: [
          {
            required: true,
            message: "Vui lòng nhập thời gian giao hàng dự kiến!",
          },
        ],
      },
    }
  },
  async created() {
    await Promise.all([this.fetchCustomers(), this.fetchProjects()])
    this.fetchDeliveries()
  },
  methods: {
    async fetchDeliveries() {
      try {
        const data = await getAllDeliveries()
        this.deliveries = Array.isArray(data) ? data : []
      } catch (error) {
        message.error(error.message || "Có lỗi xảy ra khi tải giao hàng!")
      }
    },
    async fetchCustomers() {
      try {
        const data = await getAllCustomers()
        this.customers = data.data
      } catch (error) {
        message.error(
          error.message || "Có lỗi xảy ra khi tải danh sách khách hàng!"
        )
      }
    },
    async fetchProjects() {
      try {
        const data = await getAllProjects()
        this.projects = data
      } catch (error) {
        message.error(error.message || "Có lỗi xảy ra khi tải dự án!")
      }
    },
    showCreateModal() {
      this.isEditing = false
      this.formData = {
        shippingMethodId: 1,
        customerId: null,
        projectId: null,
        estimateDeliveryTime: null,
      }
      this.isModalVisible = true
    },
    async handleOk() {
      this.$refs.deliveryForm
        .validate()
        .then(async () => {
          try {
            const payload = {
              shippingMethodId: this.formData.shippingMethodId,
              customerId: this.formData.customerId,
              projectId: this.formData.projectId,
              estimateDeliveryTime: this.formData.estimateDeliveryTime,
            }

            await createDelivery(payload)
            message.success("Thêm giao hàng thành công!")
            this.isModalVisible = false
            this.fetchDeliveries()
          } catch (error) {
            message.error(error.message || "Có lỗi xảy ra!")
          }
        })
        .catch(() => {
          message.error("Vui lòng điền đầy đủ thông tin!")
        })
    },
    handleCancel() {
      this.isModalVisible = false
    },
    async updateDeliveryStatus(deliveryId) {
      try {
        await updateDeliveryStatus(deliveryId)
        message.success("Cập nhật trạng thái giao hàng thành công!")
        this.fetchDeliveries()
      } catch (error) {
        message.error(error.message || "Có lỗi xảy ra khi cập nhật trạng thái!")
      }
    },
  },
}
</script>

<style scoped>
.actions-bar {
  display: flex;
  justify-content: space-between;
  margin-bottom: 16px;
}
</style>
