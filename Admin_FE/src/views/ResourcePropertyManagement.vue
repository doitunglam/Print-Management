<template>
  <div>
    <a-breadcrumb style="margin: 16px 0">
      <a-breadcrumb-item>Trang chủ</a-breadcrumb-item>
      <a-breadcrumb-item>Quản lý thuộc tính tài nguyên</a-breadcrumb-item>
    </a-breadcrumb>
    <div class="actions-bar">
      <a-button type="primary" @click="showCreateModal" class="action-button"
        >Thêm thuộc tính</a-button
      >
      <!-- <a-button type="primary" @click="showCreateDetailModal" class="action-button">Thêm chi tiết thuộc tính</a-button> -->
    </div>
    <a-table
      :columns="columns"
      :dataSource="resourceProperties"
      :pagination="{ pageSize: 10 }"
      rowKey="id"
    >
      <template #bodyCell="{ column, record }">
        <span v-if="column.key === 'actions'">
          <a @click="showEditModal(record)">Sửa</a>
          <a-divider type="vertical" />
          <a-popconfirm
            title="Bạn có chắc chắn muốn xóa thuộc tính tài nguyên này không?"
            ok-text="Có"
            cancel-text="Không"
            @confirm="handleDelete(record.id)"
          >
            <a>Xóa</a>
          </a-popconfirm>
        </span>
        <span v-else-if="column.key === 'image'">
          <img
            :src="record[column.dataIndex]"
            alt="Image"
            style="width: 50px; height: 50px"
          />
        </span>
        <span v-else>{{ record[column.dataIndex] || "-" }}</span>
      </template>
    </a-table>
    <a-modal
      v-model:visible="isModalVisible"
      title="Thuộc tính tài nguyên"
      @ok="handleOk"
      @cancel="handleCancel"
    >
      <a-form
        :model="formData"
        :rules="rules"
        ref="resourcePropertyForm"
        layout="vertical"
      >
        <a-form-item
          label="Tên thuộc tính"
          name="resourcePropertyName"
          style="margin-bottom: 10px"
        >
          <a-input v-model:value="formData.resourcePropertyName" />
        </a-form-item>
        <a-form-item
          label="Tài nguyên"
          name="resourceId"
          style="margin-bottom: 10px"
        >
          <a-select
            v-model:value="formData.resourceId"
            placeholder="Chọn tài nguyên"
          >
            <a-select-option
              v-for="source in resources"
              :key="source.id"
              :value="source.id"
              >{{ source.resourceName }}</a-select-option
            >
          </a-select>
        </a-form-item>
        <a-form-item
          label="Số lượng"
          name="quantity"
          style="margin-bottom: 10px"
        >
          <a-input-number v-model:value="formData.quantity" />
        </a-form-item>
      </a-form>
    </a-modal>
    <!-- New Modal for ResourcePropertyDetail -->
    <a-modal
      v-model:visible="isDetailModalVisible"
      title="Chi tiết thuộc tính tài nguyên"
      @ok="handleDetailOk"
      @cancel="handleDetailCancel"
    >
      <a-form
        :model="detailFormData"
        :rules="detailRules"
        ref="resourcePropertyDetailForm"
        layout="vertical"
      >
        <a-form-item
          label="Property ID"
          name="propertyId"
          style="margin-bottom: 10px"
        >
          <a-input-number v-model:value="detailFormData.propertyId" />
        </a-form-item>
        <a-form-item
          label="Tên chi tiết thuộc tính"
          name="propertyDetailName"
          style="margin-bottom: 10px"
        >
          <a-input v-model:value="detailFormData.propertyDetailName" />
        </a-form-item>
        <a-form-item label="Giá" name="price" style="margin-bottom: 10px">
          <a-input-number v-model:value="detailFormData.price" />
        </a-form-item>
        <a-form-item
          label="Số lượng"
          name="quantity"
          style="margin-bottom: 10px"
        >
          <a-input-number v-model:value="detailFormData.quantity" />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script>
import {
  getAllResourceProperties,
  createResourceProperty,
} from "@/apis/resourcePropertyApi"
import { createResourcePropertyDetail } from "@/apis/projectApi" // Import the new API
import { message } from "ant-design-vue"
import { getAllResources } from "@/apis/projectApi"

export default {
  name: "ResourcePropertyManagement",
  data() {
    return {
      resourceProperties: [],
      columns: [
        {
          title: "ID",
          dataIndex: "id",
          key: "id",
        },
        {
          title: "Tên thuộc tính",
          dataIndex: "resourcePropertyName",
          key: "resourcePropertyName",
        },

        {
          title: "Tài nguyên",
          dataIndex: "resourceName",
          key: "resourceName",
        },
        {
          title: "Số lượng",
          dataIndex: "quantity",
          key: "quantity",
        },
        {
          title: "Hành động",
          key: "actions",
        },
      ],
      isModalVisible: false,
      isDetailModalVisible: false, // State for the new detail modal
      formData: {
        id: null,
        resourcePropertyName: "",
        resourceId: null,
        quantity: null,
      },
      detailFormData: {
        // Data for the new detail form
        propertyId: null,
        propertyDetailName: "",
        image: "",
        price: null,
        quantity: null,
      },
      isEditing: false,
      rules: {
        resourcePropertyName: [
          { required: true, message: "Vui lòng nhập tên thuộc tính!" },
        ],
        resourceId: [
          { required: true, message: "Vui lòng nhập ID tài nguyên!" },
        ],
        quantity: [{ required: true, message: "Vui lòng nhập số lượng!" }],
      },
      detailRules: {
        // Rules for the new detail form
        propertyId: [{ required: true, message: "Vui lòng nhập Property ID!" }],
        propertyDetailName: [
          { required: true, message: "Vui lòng nhập tên chi tiết thuộc tính!" },
        ],
        price: [{ required: true, message: "Vui lòng nhập giá!" }],
        quantity: [{ required: true, message: "Vui lòng nhập số lượng!" }],
      },
      resources: [],
    }
  },
  async created() {
    await this.fetchResources()
    this.fetchResourceProperties()
  },
  methods: {
    async fetchResources() {
      try {
        const data = await getAllResources()
        this.resources = data.data
      } catch (error) {
        message.error(error.message || "Có lỗi xảy ra khi tải tài nguyên!")
      }
    },
    async fetchResourceProperties() {
      try {
        const data = await getAllResourceProperties()
        this.resourceProperties = Array.isArray(data)
          ? this.transformDataResourceProperties(data)
          : []
      } catch (error) {
        message.error(
          error.message || "Có lỗi xảy ra khi tải thuộc tính tài nguyên!"
        )
      }
    },
    transformDataResourceProperties(data) {
      return data.map((item) => {
        item.resourceName =
          this.resources.find((resource) => resource.id === item.resourceId)
            ?.resourceName ?? ""
        return item
      })
    },
    showCreateModal() {
      this.isEditing = false
      this.formData = {
        id: null,
        resourcePropertyName: "",
        resourceId: null,
        quantity: null,
      }
      this.isModalVisible = true
    },
    showCreateDetailModal() {
      this.detailFormData = {
        propertyId: null,
        propertyDetailName: "",
        image: "",
        price: null,
        quantity: null,
      }
      this.isDetailModalVisible = true
    },
    showEditModal(record) {
      this.isEditing = true
      this.formData = { ...record }
      this.isModalVisible = true
    },
    async handleOk() {
      this.$refs.resourcePropertyForm
        .validate()
        .then(async () => {
          try {
            const payload = {
              resourcePropertyName: this.formData.resourcePropertyName,
              resourceId: this.formData.resourceId,
              quantity: this.formData.quantity,
            }

            if (this.isEditing) {
              // Assuming update logic is handled elsewhere or not needed
              message.success("Cập nhật thuộc tính thành công!")
            } else {
              await createResourceProperty(payload)
              message.success("Thêm thuộc tính thành công!")
            }
            this.isModalVisible = false
            this.fetchResourceProperties()
          } catch (error) {
            message.error(error.message || "Có lỗi xảy ra!")
          }
        })
        .catch(() => {
          message.error("Vui lòng điền đầy đủ thông tin!")
        })
    },
    async handleDetailOk() {
      this.$refs.resourcePropertyDetailForm
        .validate()
        .then(async () => {
          try {
            const payload = {
              propertyId: this.detailFormData.propertyId,
              propertyDetailName: this.detailFormData.propertyDetailName,
              price: this.detailFormData.price,
              quantity: this.detailFormData.quantity,
            }
            await createResourcePropertyDetail(payload)
            message.success("Thêm chi tiết thuộc tính thành công!")
            this.isDetailModalVisible = false
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
    handleDetailCancel() {
      this.isDetailModalVisible = false
    },
  },
}
</script>

<style scoped>
.actions-bar {
  display: flex;
  justify-content: flex-start;
  margin-bottom: 16px;
}

.action-button {
  margin-right: 20px;
}
</style>
