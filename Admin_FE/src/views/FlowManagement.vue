<template>
  <div class="flow-management">
    <a-breadcrumb style="margin: 16px 0">
      <a-breadcrumb-item>Trang chủ</a-breadcrumb-item>
      <a-breadcrumb-item>Quản lý luồng đặt hàng</a-breadcrumb-item>
    </a-breadcrumb>
    <a-row :gutter="24">
      <a-col :span="8">
        <div class="actions-bar">
          <a-button type="primary" @click="() => showModal('addStepTemplate')">Thêm bước xử lý</a-button>
        </div>

        <a-table :columns="stepTemplateColumn" :dataSource="stepTemplates" :pagination="{ pageSize: 4 }" rowKey="id">
          <template #bodyCell="{ column, record }">
            <span v-if="column.key === 'actions'">
              <a @click="showEditStepTemplate(record)">Sửa</a>
              <a-divider type="vertical" />
              <a-popconfirm title="Bạn có chắc chắn muốn xóa bước xử lý này không?" ok-text="Có" cancel-text="Không"
                @confirm="handleStepTemplateDelete(record.id)">
                <a>Xóa</a>
              </a-popconfirm>
            </span>
          </template>
        </a-table>
      </a-col>
      <a-col :span="16">
        <div class="actions-bar">
          <a-button type="primary" @click="() => showModal('addFlowTemplate')">Thêm luồng xử lý</a-button>
        </div>

        <a-table :columns="flowTemplateColumn" :dataSource="flowTemplates" :pagination="{ pageSize: 4 }" rowKey="id">
          <template #bodyCell="{ column, record }">
            <span v-if="column.key == 'productName'">
              {{ products.find((product) => product.id == record.productId).name }}
            </span>
            <span v-if="column.key === 'actions'">
              <a @click="showEditFlowTemplate(record)">Sửa</a>
              <a-divider type="vertical" />
              <a-popconfirm title="Bạn có chắc chắn muốn xóa bước xử lý này không?" ok-text="Có" cancel-text="Không"
                @confirm="handleFlowTemplateDelete(record.id)">
                <a>Xóa</a>
              </a-popconfirm>
            </span>
          </template>
        </a-table>
      </a-col>
    </a-row>

    <!-- Add Step Template Modal -->
    <a-modal v-model:visible="modalVisibility['addStepTemplate']" title="Thêm bước xử lý"
      @ok="submitAddStepTemplateModal" @cancel="handleCancel">
      <a-form :model="newStepTemplate" ref="addStepTemplateForm" layout="vertical">
        <a-form-item label="Tên bước xử lý" name="name" style="margin-bottom: 10px">
          <a-input v-model:value="newStepTemplate.name" placeholder="Ví dụ: Xác nhận đơn hàng, Đóng gói đơn hàng"
            required />
        </a-form-item>
      </a-form>
    </a-modal>

    <!-- Edit Step Template Modal -->
    <a-modal v-model:visible="modalVisibility['editStepTemplate']" title="Sửa bước xử lý"
      @ok="submitEditStepTemplateModal" @cancel="handleCancel">
      <a-form :model="editStepTemplate" ref="addStepTemplateForm" layout="vertical">
        <a-form-item label="Tên bước xử lý" name="name" style="margin-bottom: 10px">
          <a-input v-model:value="editStepTemplate.name" placeholder="Ví dụ: Xác nhận đơn hàng, Đóng gói đơn hàng"
            required />
        </a-form-item>
      </a-form>
    </a-modal>

    <!-- Add Flow Template Modal -->
    <a-modal v-model:visible="modalVisibility['addFlowTemplate']" title="Thêm luồng xử lý"
      @ok="submitAddFlowTemplateModal" @cancel="handleCancel">
      <a-form :model="newFlowTemplate" ref="addStepTemplateForm" layout="vertical">
        <a-form-item label="Tên luồng xử lý" name="name" style="margin-bottom: 10px">
          <a-input v-model:value="newFlowTemplate.name" placeholder="Nhập tên luồng xử lý" required />
        </a-form-item>

        <a-form-item label="Tên sản phẩm" name="productName" style="margin-bottom: 10px">
          <a-select v-model:value="newFlowTemplate.productId" placeholder="Nhập tên sản phẩm áp dụng luồng xử lý"
            required>
            <a-select-option v-for="product in products" :key="product.id" :value="product.id">
              {{ product.name }}
            </a-select-option>
          </a-select>
        </a-form-item>

        <a-form-item v-for="step in newFlowTemplate.stepFlowTemplates" :key="step.position">
          <a-row :gutter="12">
            <a-col flex="auto">
              <a-select v-model:value="step.stepTemplateId" placeholder="Chọn một bước xử lý" style="width: 100%"
                required>
                <a-select-option v-for="stepTemplate in stepTemplates" :key="stepTemplate.id" :value="stepTemplate.id">
                  {{ stepTemplate.name }}
                </a-select-option>
              </a-select>
            </a-col>
            <a-col flex="none">
              <a-button @click="() => removeFlowTemplateStep(newFlowTemplate, step.position)">
                Xóa
              </a-button>
            </a-col>
          </a-row>
        </a-form-item>

        <a-button type="dashed" @click="() => { flowTemplateAddStep(newFlowTemplate) }">
          Thêm bước xử lý
        </a-button>
      </a-form>
    </a-modal>

    <!-- Edit Flow Template Modal -->
    <a-modal v-model:visible="modalVisibility['editFlowTemplate']" title="Thêm luồng xử lý"
      @ok="submitEditFlowTemplateModal" @cancel="handleCancel">
      <a-form :model="editFlowTemplate" ref="editFlowTemplateForm" layout="vertical">
        <a-form-item label="Tên luồng xử lý" name="name" style="margin-bottom: 10px">
          <a-input v-model:value="editFlowTemplate.name" placeholder="Nhập tên luồng xử lý" required />
        </a-form-item>

        <a-form-item label="Tên sản phẩm" name="productName" style="margin-bottom: 10px">
          <a-select v-model:value="editFlowTemplate.productId" placeholder="Nhập tên sản phẩm áp dụng luồng xử lý"
            required>
            <a-select-option v-for="product in products" :key="product.id" :value="product.id">
              {{ product.name }}
            </a-select-option>
          </a-select>
        </a-form-item>

        <a-form-item style="margin-bottom: 2px !important;" v-for="step in editFlowTemplate.stepFlowTemplates"
          :key="step.position">
          <a-row :gutter="12">
            <a-col flex="auto">
              <a-select v-model:value="step.stepTemplateId" placeholder="Chọn một bước xử lý" style="width: 100%"
                required>
                <a-select-option v-for="stepTemplate in stepTemplates" :key="stepTemplate.id" :value="stepTemplate.id">
                  {{ stepTemplate.name }}
                </a-select-option>
              </a-select>
            </a-col>
            <a-col flex="none">
              <a-button @click="() => removeFlowTemplateStep(editFlowTemplate, step.position)">
                Xóa
              </a-button>
            </a-col>
          </a-row>
        </a-form-item>

        <a-button type="dashed" @click="() => { flowTemplateAddStep(editFlowTemplate) }">
          Thêm bước xử lý
        </a-button>
      </a-form>
    </a-modal>
  </div>
</template>

<script>
import {
  getAllDesigns,
  addDesign,
  approveDesign,
  updateDesign,
  getAllStepTemplate,
  addStepTemplate,
  editStepTemplate,
  deleteStepTemplate,
  getAllFlowTemplate,
  addFlowTemplate,
  editFlowTemplate,
  deleteFlowTemplate,
} from "@/apis/projectApi"
import { getAllProjects } from "@/apis/projectApi"
import { ref, uploadBytes, getDownloadURL } from "firebase/storage"
import { storage } from "@/firebaseConfig"
import { message } from "ant-design-vue"
import { getAllProducts } from "@/apis/productApi"

export default {
  name: "FlowManagement",
  data() {
    return {
      products: [],
      projects: [],
      stepTemplates: [],
      flowTemplates: [],
      newStepTemplate: {
        projectId: null,
        name: "",
        filePath: "",
      },
      newFlowTemplate: {
        name: "",
        productId: "",
        stepFlowTemplates: []
      },
      editDesign: {
        projectId: null,
        designerId: "",
        filePath: "",
      },
      editStepTemplate: null,
      editFlowTemplate: null,
      stepTemplateColumn: [
        {
          title: "ID",
          dataIndex: "id",
          key: "id",
        },
        {
          title: "Tên",
          dataIndex: "name",
          key: "name",
        }, {
          title: "Hành động",
          key: "actions",
          scopedSlots: { customRender: "actions" },
        },
      ],
      flowTemplateColumn: [
        {
          title: "ID",
          dataIndex: "id",
          key: "id",
        },
        {
          title: "Tên",
          dataIndex: "name",
          key: "name",
        },
        {
          title: "Tên sản phẩm",
          dataIndex: "productName",
          key: "productName"
        },
        {
          title: "Hành động",
          key: "actions",
          scopedSlots: { customRender: "actions" },
        },
      ],
      modalVisibility: {},
      isAddStepTemplateModalVisible: false,
      isModalVisible: false,
      isEditModalVisible: false,
      rules: {
        projectId: [{ required: true, message: "Vui lòng chọn dự án!" }],
        designerId: [
          { required: true, message: "Vui lòng nhập ID nhà thiết kế!" },
        ],
      },
    }
  },
  methods: {
    async fetchProducts() {
      try {
        this.products = await getAllProducts()
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
    async fetchProjects() {
      try {
        this.projects = await getAllProjects()
      } catch (error) {
        console.error("Lỗi khi lấy danh sách dự án:", error)
      }
    },
    async fetchStepTemplates() {
      try {
        this.stepTemplates = await getAllStepTemplate()
      } catch (error) {
        console.error("Lỗi khi lấy danh sách bước xử lý đơn hàng: ", error)
      }
    },
    async fetchFlowTemplates() {
      try {
        this.flowTemplates = await getAllFlowTemplate()
      } catch (error) {
        console.error("Lỗi khi lấy danh sách luồng xử lý đơn hàng: ", error)
      }
    },
    async handleFileUpload(file) {
      try {
        const storageRef = ref(storage, `designs/${file.name}`)
        const snapshot = await uploadBytes(storageRef, file)
        const filePath = await getDownloadURL(snapshot.ref)
        this.newStepTemplate.filePath = filePath
        return false
      } catch (error) {
        console.error("Lỗi khi tải lên tệp:", error)
        return false
      }
    },
    async submitStepModa() {
      try {
        console.log("Submitting design:", this.newStepTemplate)
        await addDesign(this.newStepTemplate)
        this.fetchDesigns()
      } catch (error) {
        console.error("Lỗi khi thêm thiết kế:", error)
      }
    },
    async approveDesign(designId) {
      try {
        await approveDesign(designId)
        this.fetchDesigns()
      } catch (error) {
        console.error("Lỗi khi chấp nhận thiết kế:", error)
      }
    },
    showModal(modalIdentity) {
      this.modalVisibility = {}
      this.modalVisibility[modalIdentity] = true;
    },
    showAddStepTemplateModal() {

    },

    showCreateModal() {
      this.isModalVisible = true
      this.newStepTemplate = {
        projectId: null,
        name: "",
        filePath: "",
      }
    },
    showEditStepTemplate(record) {
      this.showModal('editStepTemplate');
      this.editStepTemplate = { ...record }
    },
    showEditFlowTemplate(record) {
      this.showModal('editFlowTemplate');
      this.editFlowTemplate = { ...record }
    },
    async handleUpdate() {
      try {
        await updateDesign(this.editDesign.id, this.editDesign)
        this.fetchDesigns()
        this.isEditModalVisible = false
        message.success("Thiết kế đã được cập nhật thành công!")
      } catch (error) {
        console.error("Lỗi khi cập nhật thiết kế:", error)
      }
    },
    handleCancel() {
      this.isModalVisible = false
    },
    handleCancelEdit() {
      this.isEditModalVisible = false
    },
    async submitAddStepTemplateModal() {
      try {
        console.log("Submitting step template:", this.newStepTemplate)
        await addStepTemplate(this.newStepTemplate)
        this.fetchStepTemplates()
        this.modalVisibility = {}
        this.newStepTemplate.name = ""
        message.success("Tạo bước xử lý thành công!")
      } catch (error) {
        console.error("Lỗi khi thêm bước xử lý:", error)
      }
    },
    async submitEditStepTemplateModal() {
      try {
        console.log("Editing step template:", this.editStepTemplate)
        await editStepTemplate(this.editStepTemplate)
        this.fetchStepTemplates()
        this.modalVisibility = {}
        message.success("Sửa bước xử lý thành công!")
      } catch (error) {
        console.error("Lỗi khi sửa bước xử lý:", error)
      }
    },
    async handleStepTemplateDelete(stepTemplateId) {
      try {
        await deleteStepTemplate(stepTemplateId)
        this.fetchStepTemplates()
        message.success("Bước xử lý đã được xóa thành công!")
      } catch (error) {
        message.error("Có lỗi đã xảy ra!")

        console.error("Lỗi khi xóa bước xử lý:", error)
      }
    },
    flowTemplateAddStep(flowTemplate) {
      const highestPos = Math.max(...flowTemplate.stepFlowTemplates.map(item => item.position), 0) + 1
      flowTemplate.stepFlowTemplates.push({
        position: highestPos
      })
    },

    removeFlowTemplateStep(flowTemplate, position) {
      flowTemplate.stepFlowTemplates = flowTemplate.stepFlowTemplates.filter(step => step.position !== position);
    },
    async submitAddFlowTemplateModal() {
      try {
        await addFlowTemplate(this.newFlowTemplate)
        this.fetchFlowTemplates()
        this.modalVisibility = {}
        this.newFlowTemplate.name = ""
        this.newFlowTemplate.stepFlowTemplates = []
        message.success("Tạo luồng xử lý thành công!")
      } catch (error) {
        console.error("Lỗi khi thêm luồng xử lý:", error)
      }
    },
    async submitEditFlowTemplateModal() {
      try {
        await editFlowTemplate(this.editFlowTemplate)
        this.fetchFlowTemplates()
        this.modalVisibility = {}
        message.success("Sửa luồng xử lý thành công!")
      } catch (error) {
        console.error("Lỗi khi sửa luồng xử lý:", error)
      }
    },
    async handleFlowTemplateDelete(flowTemplateId) {
      try {
        await deleteFlowTemplate(flowTemplateId)
        setTimeout(() => {
          this.fetchFlowTemplates()
        }, 2000)
        message.success("Luồng xử lý đã được xóa thành công!")
      } catch (error) {
        message.error("Có lỗi đã xảy ra!")

        console.error("Lỗi khi xóa luồng xử lý:", error)
      }
    }
  },
  mounted() {
    this.fetchProducts()
    this.fetchDesigns()
    this.fetchProjects()
    this.fetchStepTemplates()
    this.fetchFlowTemplates()

  },
}
</script>

<style scoped>
.design-table {
  margin-top: 10px;
}

.actions-bar {
  display: flex;
  gap: 16px;
  margin-bottom: 16px;
}

.design-image {
  height: 100px;
  width: auto;
}
</style>
