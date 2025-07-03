<template>
  <div class="design-management">
    <a-breadcrumb style="margin: 16px 0">
      <a-breadcrumb-item>Trang chủ</a-breadcrumb-item>
      <a-breadcrumb-item>Quản lý thiết kế</a-breadcrumb-item>
    </a-breadcrumb>
    <div class="actions-bar" v-if="checkPermission('Design') > 0">
      <a-button type="primary" @click="showCreateModal">Thêm thiết kế</a-button>
    </div>
    <a-table :columns="columns" :dataSource="designs" :pagination="{ pageSize: 10 }" rowKey="id">
      <template #bodyCell="{ column, record }">
        <span v-if="column.key === 'actions'">
          <div v-if="checkPermission('Design') > 0">
            <a @click="approveDesign(record.id)" v-if="record.designStatus != 'Approved'">Phê duyệt</a>
            <a-divider type="vertical" v-if="record.designStatus != 'Approved'" />
            <a @click="showEditModal(record)">Sửa</a>
            <a-divider type="vertical" />
            <a-popconfirm title="Bạn có chắc chắn muốn xóa thiết kế này không?" ok-text="Có" cancel-text="Không"
              @confirm="handleDelete(record.id)">
              <a>Xóa</a>
            </a-popconfirm>
          </div>
        </span>
        <span v-else-if="column.key === 'filePath'">
          <img :src="record.filePath" alt="" class="design-image" />
        </span>
        <span v-else-if="column.key === 'projectId'">
          {{projects.findLast(project => project.id == record.projectId)?.projectName ?? '-'}}
        </span>
        <span v-else-if="column.key === 'designerId'">
          {{users.findLast(user => user.id == record.designerId)?.fullName ?? '-'}}
        </span>
        <span v-else-if="column.key === 'approverId'">
          {{users.findLast(user => user.id == record.approverId)?.fullName ?? '-'}}
        </span>
        <span v-else>{{ record[column.dataIndex] || "-" }}</span>
      </template>
    </a-table>

    <!-- Add Design Modal -->
    <a-modal v-model:visible="isModalVisible" title="Thêm Thiết kế" @ok="submitDesign" @cancel="handleCancel">
      <a-form :model="newDesign" :rules="rules" ref="designForm" layout="vertical">
        <a-form-item label="Dự án" name="projectId" style="margin-bottom: 10px">
          <a-select v-model:value="newDesign.projectId" placeholder="Chọn một dự án" required>
            <a-select-option v-for="project in projects" :key="project.id" :value="project.id">
              {{ project.projectName }}
            </a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="Nhà thiết kế" name="designerId" style="margin-bottom: 10px">
          <a-select v-model:value="newDesign.designerId" placeholder="Chọn nhà thiết kế" required>
            <a-select-option v-for="user in usersFiltered" :key="user.id" :value="user.id">
              {{ user.id }} - {{ user.fullName }}
            </a-select-option>
          </a-select> 
        </a-form-item>
        <a-form-item label="Tệp Thiết kế" name="filePath" style="margin-bottom: 10px">
          <a-upload :before-upload="handleFileUpload" :show-upload-list="false">
            <a-button icon="upload">Nhấn để Tải lên</a-button>
          </a-upload>
        </a-form-item>
      </a-form>
    </a-modal>

    <!-- Edit Design Modal -->
    <a-modal v-model:visible="isEditModalVisible" title="Sửa Thiết kế" @ok="handleUpdate" @cancel="handleCancelEdit">
      <a-form :model="editDesign" :rules="rules" ref="editDesignForm" layout="vertical">
        <a-form-item label="Dự án" name="projectId" style="margin-bottom: 10px">
          <a-select v-model:value="editDesign.projectId" placeholder="Chọn một dự án" required>
            <a-select-option v-for="project in projects" :key="project.id" :value="project.id">
              {{ project.projectName }}
            </a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="ID Nhà thiết kế" name="designerId" style="margin-bottom: 10px">
          <a-select v-model:value="editDesign.designerId" placeholder="Chọn nhà thiết kế" required>
            <a-select-option v-for="user in usersFiltered" :key="user.id" :value="user.id">
              {{ user.id }} - {{ user.fullName }}
            </a-select-option>
          </a-select>           
        </a-form-item>
        <a-form-item label="Tệp Thiết kế" name="filePath" style="margin-bottom: 10px">
          <a-upload :before-upload="handleFileUpload" :show-upload-list="false">
            <a-button icon="upload">Nhấn để Tải lên</a-button>
          </a-upload>
        </a-form-item>
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
  deleteDesign,
} from "@/apis/projectApi"
import { getAllProjects } from "@/apis/projectApi"
import { checkPermission } from '@/utils/index'
import { ref, uploadBytes, getDownloadURL } from "firebase/storage"
import { getAllUsersFromAllUsersApi } from '@/apis/userApi';
import { storage } from "@/firebaseConfig"
import { message } from "ant-design-vue"

export default {
  name: "DesignManagement",
  data() {
    return {
      designs: [],
      projects: [],
      users: [],
      usersFiltered: [],
      newDesign: {
        projectId: null,
        designerId: "",
        filePath: "",
      },
      editDesign: {
        projectId: null,
        designerId: "",
        filePath: "",
      },
      columns: [
        {
          title: "Mã thiết kế",
          dataIndex: "id",
          key: "id",
        },
        {
          title: "Dự án",
          dataIndex: "projectId",
          key: "projectId",
        },
        {
          title: "Nhà thiết kế",
          dataIndex: "designerId",
          key: "designerId",
        },
        {
          title: "Tệp",
          dataIndex: "filePath",
          key: "filePath",
        },
        {
          title: "Thời gian Thiết kế",
          dataIndex: "designTime",
          key: "designTime",
        },
        {
          title: "Trạng thái",
          dataIndex: "designStatus",
          key: "designStatus",
        },
        {
          title: "Người phê duyệt",
          dataIndex: "approverId",
          key: "approverId",
        },
        {
          title: "Hành động",
          key: "actions",
          scopedSlots: { customRender: "actions" },
        },
      ],
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
    async handleFileUpload(file) {
      try {
        const storageRef = ref(storage, `designs/${file.name}`)
        const snapshot = await uploadBytes(storageRef, file)
        const filePath = await getDownloadURL(snapshot.ref)
        this.newDesign.filePath = filePath
        return false
      } catch (error) {
        console.error("Lỗi khi tải lên tệp:", error)
        return false
      }
    },
    async submitDesign() {
      try {
        await addDesign(this.newDesign)
        this.fetchDesigns()
        this.isModalVisible = false
        message.success("Thiết kế đã được tạo thành công!")
      } catch (error) {
        console.error("Lỗi khi thêm thiết kế:", error)
        message.error("Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu")
      }
    },
    async approveDesign(designId) {
      try {
        await approveDesign(designId)
        this.fetchDesigns()
        message.success("Phê duyệt thiết kế thành công")
      } catch (error) {
        console.error("Lỗi khi chấp nhận thiết kế:", error)
        message.error("Lỗi khi phê duyệt thiết kế")
      }
    },
    showCreateModal() {
      this.isModalVisible = true
      this.newDesign = {
        projectId: null,
        designerId: "",
        filePath: "",
      }
    },
    showEditModal(record) {
      this.isEditModalVisible = true
      this.editDesign = { ...record }
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
    async handleDelete(designId) {
      try {
        await deleteDesign(designId)
        this.fetchDesigns()
        message.success("Thiết kế đã được xóa thành công!")
      } catch (error) {
        console.error("Lỗi khi xóa thiết kế:", error)
      }
    },
    handleCancel() {
      this.isModalVisible = false
    },
    handleCancelEdit() {
      this.isEditModalVisible = false
    },
    checkPermission(page) {
      return checkPermission(page)
    },
    async fetchUsers() {
      try {
        const data = await getAllUsersFromAllUsersApi();
        this.users = data;
        this.usersFiltered = data.filter(t => t.roles.includes("Employee"))
      } catch (error) {
        message.error(error.message || 'Có lỗi xảy ra khi tải danh sách người dùng!');
      }
    },
  },
  mounted() {
    this.fetchDesigns()
    this.fetchProjects()
    this.fetchUsers()
  },
}
</script>

<style scoped>
.design-table {
  margin-top: 10px;
}

.actions-bar {
  display: flex;
  justify-content: space-between;
  margin-bottom: 16px;
}

.design-image {
  height: 100px;
  width: auto;
}
</style>
