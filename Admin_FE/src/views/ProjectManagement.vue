<template>
  <div>
    <a-breadcrumb style="margin: 16px 0">
      <a-breadcrumb-item>Trang chủ</a-breadcrumb-item>
      <a-breadcrumb-item>Quản lý dự án</a-breadcrumb-item>
    </a-breadcrumb>
    <div class="actions-bar">
      <a-button type="primary" @click="showCreateModal">Thêm dự án</a-button>
    </div>
    <a-table
      :columns="columns"
      :dataSource="projects"
      :pagination="{ pageSize: 10 }"
      rowKey="id"
    >
    <template #bodyCell="{ column, record }">
        <span v-if="column.key === 'actions'">
          <a @click="showEditModal(record)">Sửa</a>
          <!-- <a-divider type="vertical" />
          <a-popconfirm
            title="Bạn có chắc chắn muốn xóa dự án này không?"
            ok-text="Có"
            cancel-text="Không"
            @confirm="handleDelete(record.id)"
          >
            <a>Xóa</a>
          </a-popconfirm> -->
        </span>
        <span v-else>{{ record[column.dataIndex] || '-' }}</span>
      </template>
    </a-table>
    <a-modal
      v-model:visible="isModalVisible"
      title="Dự án"
      @ok="handleOk"
      @cancel="handleCancel"
    >
      <a-form :model="formData" :rules="rules" ref="projectForm" layout="vertical">
        <a-form-item label="Tên dự án" name="projectName" style="margin-bottom: 10px;">
          <a-input v-model:value="formData.projectName" />
        </a-form-item>
        <a-form-item label="Mô tả" name="requestDescriptionFromCustomer" style="margin-bottom: 10px;">
          <a-input v-model:value="formData.requestDescriptionFromCustomer" />
        </a-form-item>
        <a-form-item label="Ngày bắt đầu" name="startDate" style="margin-bottom: 10px;">
          <a-date-picker v-model:value="formData.startDate" style="width: 100%;" />
        </a-form-item>
        <a-form-item label="Ngày kết thúc dự kiến" name="expectedEndDate" style="margin-bottom: 10px;">
          <a-date-picker v-model:value="formData.expectedEndDate" style="width: 100%;" />
        </a-form-item>
        <a-form-item label="ID nhân viên" name="employeeId" style="margin-bottom: 10px;">
          <a-input-number v-model:value="formData.employeeId" style="width: 100%;" />
        </a-form-item>
        <a-form-item label="Khách hàng" name="customerId" style="margin-bottom: 10px;">
  <a-select v-model:value="formData.customerId" placeholder="Chọn một khách hàng" required>
    <a-select-option v-for="customer in customers" :key="customer.id" :value="customer.id">
      {{ customer.id }} - {{ customer.fullName }} 
    </a-select-option>
  </a-select>
</a-form-item>

<a-form-item label="Trạng thái dự án (0: Đang Thiết Kế, 1: Đang In, 2: Đã Hoàn Thành)" name="projectStatus" style="margin-bottom: 10px;">
  <a-select v-model:value="formData.projectStatus" placeholder="Chọn trạng thái dự án" style="width: 100%;">
    <a-select-option value="0">Đang Thiết Kế</a-select-option>
    <a-select-option value="1">Đang In</a-select-option>
    <a-select-option value="2">Đã Hoàn Thành</a-select-option>
  </a-select>
</a-form-item>

      </a-form>
    </a-modal>
  </div>
</template>

<script>
import { getAllProjects, createProject, updateProject, deleteProject } from '@/apis/projectApi';
import { getAllCustomers } from '@/apis/projectApi';  // Giả sử bạn có một API như vậy.
import { message } from 'ant-design-vue';
import dayjs from 'dayjs';

export default {
  name: 'ProjectManagement',
  data() {
    return {
      projects: [],
      customers: [],
      columns: [
        {
          title: 'Tên dự án',
          dataIndex: 'projectName',
          key: 'projectName',
        },
        {
          title: 'ID',
          dataIndex: 'id',
          key: 'id',
        },
        {
          title: 'Mô tả',
          dataIndex: 'requestDescriptionFromCustomer',
          key: 'requestDescriptionFromCustomer',
        },
        {
          title: 'Ngày bắt đầu',
          dataIndex: 'startDate',
          key: 'startDate',
        },
        {
          title: 'Ngày kết thúc dự kiến',
          dataIndex: 'expectedEndDate',
          key: 'expectedEndDate',
        },
        {
          title: 'ID nhân viên',
          dataIndex: 'employeeId',
          key: 'employeeId',
        },
        {
          title: 'ID khách hàng',
          dataIndex: 'customerId',
          key: 'customerId',
        },
        {
          title: 'Trạng thái dự án',
          dataIndex: 'projectStatus',
          key: 'projectStatus',
        },
        {
          title: 'Hành động',
          key: 'actions',
        },
      ],
      isModalVisible: false,
      formData: {
        id: null,
        projectName: '',
        requestDescriptionFromCustomer: '',
        startDate: '',
        expectedEndDate: '',
        employeeId: 0,
        customerId: null,  // Lúc này sẽ là id của khách hàng đã chọn
        projectStatus: 0,
      },
      isEditing: false,
      rules: {
        projectName: [{ required: true, message: 'Vui lòng nhập tên dự án!' }],
        requestDescriptionFromCustomer: [{ required: true, message: 'Vui lòng nhập mô tả!' }],
        startDate: [{ required: true, message: 'Vui lòng nhập ngày bắt đầu!' }],
        expectedEndDate: [{ required: true, message: 'Vui lòng nhập ngày kết thúc dự kiến!' }],
        employeeId: [{ required: true, message: 'Vui lòng nhập ID nhân viên!' }],
        customerId: [{ required: true, message: 'Vui lòng chọn khách hàng!' }],
        projectStatus: [{ required: true, message: 'Vui lòng nhập trạng thái dự án!' }],
      },
    };
  },
  async created() {
    this.fetchProjects();
    this.fetchCustomers();  // Gọi API lấy danh sách khách hàng
  },
  methods: {
    async fetchProjects() {
      try {
        const data = await getAllProjects();
        this.projects = data;
      } catch (error) {
        message.error(error.message || 'Có lỗi xảy ra khi tải dự án!');
      }
    },
    async fetchCustomers() {
        try {
          const data = await getAllCustomers();
          console.log(data);
          this.customers = data.data;
        } catch (error) {
          message.error(error.message || 'Có lỗi xảy ra khi tải danh sách khách hàng!');
        }
      },
    showCreateModal() {
      this.isEditing = false;
      this.formData = {
        id: null,
        projectName: '',
        requestDescriptionFromCustomer: '',
        startDate: '',
        expectedEndDate: '',
        employeeId: 0,
        customerId: 0, // Không có khách hàng nào được chọn
        projectStatus: 0,
      };
      this.isModalVisible = true;
    },
    showEditModal(record) {
      this.isEditing = true;
      this.formData = {
        ...record,
        startDate: dayjs(record.startDate),
        expectedEndDate: dayjs(record.expectedEndDate),
        projectStatus: 0,
      };
      this.isModalVisible = true;
    },
    async handleOk() {
      this.$refs.projectForm.validate().then(async () => {
        try {
          const payload = {
            projectName: this.formData.projectName,
            requestDescriptionFromCustomer: this.formData.requestDescriptionFromCustomer,
            startDate: this.formData.startDate,
            expectedEndDate: this.formData.expectedEndDate,
            employeeId: this.formData.employeeId,
            customerId: this.formData.customerId,  // Sử dụng id khách hàng
            projectStatus: this.formData.projectStatus,
          };
          payload.startDate = this.formData.startDate.format('YYYY-MM-DD');
          payload.expectedEndDate = this.formData.expectedEndDate.format('YYYY-MM-DD');
          if (this.isEditing) {
            await updateProject(this.formData.id, payload);
            message.success('Cập nhật dự án thành công!');
          } else {
            await createProject(payload);
            message.success('Thêm dự án thành công!');
          }
          this.isModalVisible = false;
          this.fetchProjects();
        } catch (error) {
          message.error(error.message || 'Có lỗi xảy ra!');
        }
      }).catch(() => {
        message.error('Vui lòng điền đầy đủ thông tin!');
      });
    },
    handleCancel() {
      this.isModalVisible = false;
    },
    async handleDelete(id) {
      try {
        await deleteProject(id);
        message.success('Xóa dự án thành công!');
        this.fetchProjects();
      } catch (error) {
        message.error(error.message || 'Có lỗi xảy ra!');
      }
    },
    mounted() {
    this.fetchCustomers();
    this.fetchProjects();
  },
  },
};
</script>

<style scoped>
.actions-bar {
  display: flex;
  justify-content: space-between;
  margin-bottom: 16px;
}
</style>
