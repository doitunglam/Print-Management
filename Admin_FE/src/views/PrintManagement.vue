<template>
  <div>
    <a-breadcrumb style="margin: 16px 0">
      <a-breadcrumb-item>Trang chủ</a-breadcrumb-item>
      <a-breadcrumb-item>Quản lý in ấn</a-breadcrumb-item>
    </a-breadcrumb>

    <!-- Đặt hai nút trên cùng một dòng với chiều dài bằng nhau -->
    <div style="display: flex; justify-content: space-between; margin-bottom: 20px;">
      <a-button type="primary" @click="showModal" style="flex: 1; margin-right: 10px;">
        Xác nhận thiết kế cho công việc in
      </a-button>
      <a-button type="primary" @click="showFinishModal" style="flex: 1; margin-left: 10px;">
        Hoàn thành công việc in và kết thúc dự án
      </a-button>
    </div>

    <!-- Bảng công việc in -->
    <a-table
      :dataSource="printJobs"
      :columns="columns"
      :pagination="{ pageSize: 10 }"
      rowKey="id"
      class="design-table"
    >
    <template #bodyCell="{ column, record }">
        <span v-if="column.key === 'actions'">
          <a @click="showEditModal(record)">Sửa</a>
          <a-divider type="vertical" />
          <a-popconfirm
            title="Bạn có chắc chắn muốn xóa công việc in này không?"
            ok-text="Có"
            cancel-text="Không"
            @confirm="handleDelete(record.id)"
          >
            <a>Xóa</a>
          </a-popconfirm>
        </span>
        <span v-else>{{ record[column.dataIndex] || '-' }}</span>
      </template>
  </a-table>

    <!-- Modal xác nhận thiết kế -->
    <a-modal
      v-model:visible="isModalVisible"
      title="Xác nhận thiết kế cho công việc in"
      @ok="handleConfirm"
      @cancel="handleCancel"
    >
      <a-form :model="formData" ref="designForm" layout="vertical">
        <a-form-item label="ID Thiết kế" name="designId" style="margin-bottom: 10px;">
          <a-select v-model:value="formData.designId" placeholder="Chọn ID Thiết kế" required>
            <a-select-option v-for="design in designs" :key="design.id" :value="design.id">
              ID Thiết kế: {{ design.id }} - ID Dự án: {{ design.projectId }}
            </a-select-option>
          </a-select>
        </a-form-item>
      </a-form>
    </a-modal>

    <!-- Modal hoàn thành công việc in -->
    <a-modal
      v-model:visible="isFinishModalVisible"
      title="Hoàn thành công việc in và kết thúc dự án"
      @ok="handleFinishOk"
      @cancel="handleFinishCancel"
    >
      <a-form :model="finishFormData" ref="finishForm" layout="vertical">
        <p>Bạn có chắc muốn hoàn thành công việc in và kết thúc dự án trên không?</p>

        <!-- Sử dụng a-select cho Print Job ID -->
        <a-form-item label="Print Job ID" name="printJobId" style="margin-bottom: 10px;">
          <a-select v-model:value="finishFormData.printJobId" placeholder="Chọn công việc in" required>
            <a-select-option v-for="job in printJobs" :key="job.id" :value="job.id">
              {{ job.id }}
            </a-select-option>
          </a-select>
        </a-form-item>

        <!-- Sử dụng a-select cho Project ID -->
        <a-form-item label="Project ID" name="projectId" style="margin-bottom: 10px;">
          <a-select v-model:value="finishFormData.projectId" placeholder="Chọn dự án" required>
            <a-select-option v-for="project in projects" :key="project.id" :value="project.id">
              {{ project.id }} - {{ project.projectName }}
            </a-select-option>
          </a-select>
        </a-form-item>
      </a-form>
    </a-modal>

    <a-modal
  v-model:visible="isEditPrintJobModalVisible"
  title="Sửa Công Việc In"
  @ok="handlePrintJobUpdate"
  @cancel="handleCancelPrintJobEdit"
>
  <a-form :model="editPrintJob" ref="editPrintJobForm" layout="vertical">
    <!-- Thiết kế -->
    <a-form-item label="Thiết kế" name="designId" style="margin-bottom: 10px;">
      <a-select v-model:value="editPrintJob.designId" placeholder="Chọn thiết kế" required>
        <a-select-option v-for="design in designs" :key="design.id" :value="design.id">
          ID Thiết kế: {{ design.id }} - ID Dự án: {{ design.projectId }}
        </a-select-option>
      </a-select>
    </a-form-item>

    <!-- Trạng thái công việc in -->
    <!-- <a-form-item label="Trạng Thái Công Việc In" name="printJobStatus" style="margin-bottom: 10px;">
      <a-select v-model:value="editPrintJob.printJobStatus" placeholder="Chọn trạng thái công việc in" required>
        <a-select-option v-for="status in printJobStatuses" :key="status.value" :value="status.value">
          {{ status.label }}
        </a-select-option>
      </a-select>
    </a-form-item> -->
  </a-form>
</a-modal>

  </div>
</template>



<script>
import {
  getAllPrintJobs,
  confirmDesignForPrinting as apiConfirmDesignForPrinting,
  confirmFinishingProject as apiConfirmFinishingProject,
  getAllProjects,
} from '@/apis/projectApi';
import {
  getAllDesigns,
  updatePrintJob,
  deletePrintJob,
} from '@/apis/projectApi';
import { message } from 'ant-design-vue';

export default {
  name: 'PrintManagement',
  data() {
    return {
      printJobs: [],
      designs: [],
      projects: [],
      columns: [
        { title: 'ID', dataIndex: 'id', key: 'id' },
        { title: 'ID Thiết kế', dataIndex: 'designId', key: 'designId' },
        { title: 'Trạng thái công việc in', dataIndex: 'printJobStatus', key: 'printJobStatus' },
        { title: 'Hành động', key: 'actions', scopedSlots: { customRender: 'actions' } },
      ],
      isModalVisible: false,
      isFinishModalVisible: false,
      isEditPrintJobModalVisible: false,
      formData: { designId: null },
      finishFormData: { printJobId: null, projectId: null },
      editPrintJob: { designId: null },
    };
  },
  async created() {
    this.fetchPrintJobs();
    this.fetchProjects();
    this.fetchDesigns();
  },
  methods: {
    async fetchPrintJobs() {
      try {
        const data = await getAllPrintJobs();
        this.printJobs = data;
      } catch (error) {
        message.error(error.message || 'Có lỗi xảy ra khi tải công việc in!');
      }
    },
    async fetchDesigns() {
      try {
        this.designs = await getAllDesigns();
      } catch (error) {
        console.error('Lỗi khi lấy danh sách thiết kế:', error);
      }
    },
    async fetchProjects() {
      try {
        this.projects = await getAllProjects();
      } catch (error) {
        console.error('Lỗi khi lấy danh sách dự án:', error);
      }
    },
    showModal() {
      this.isModalVisible = true;
    },
    async handleConfirm() {
      this.$refs.designForm.validate().then(async () => {
        try {
          const { designId } = this.formData;
          if (!designId) {
            message.error('Vui lòng chọn một thiết kế!');
            return;
          }
          await apiConfirmDesignForPrinting(designId);
          message.success('Dự án đã được xác nhận cho in ấn thành công!');
          this.fetchPrintJobs();
        } catch (error) {
          message.error(error.message || 'Có lỗi xảy ra khi xác nhận dự án!');
        } finally {
          this.isModalVisible = false;
          this.formData.designId = null;
        }
      }).catch(() => {
        message.error('Vui lòng điền đầy đủ thông tin!');
      });
    },
    handleCancel() {
      this.isModalVisible = false;
    },
    async handleFinishOk() {
      this.$refs.finishForm.validate().then(async () => {
        try {
          const payload = {
            printJobId: Number(this.finishFormData.printJobId),
            projectId: Number(this.finishFormData.projectId),
          };
          if (isNaN(payload.printJobId) || isNaN(payload.projectId)) {
            message.error('ID không hợp lệ!');
            return;
          }
          await apiConfirmFinishingProject(payload);
          message.success('Hoàn thành công việc in và kết thúc dự án thành công!');
          this.fetchPrintJobs();
          this.isFinishModalVisible = false;
        } catch (error) {
          message.error(error.message || 'Có lỗi xảy ra khi hoàn thành công việc in!');
        }
      }).catch(() => {
        message.error('Vui lòng điền đầy đủ thông tin!');
      });
    },
    handleFinishCancel() {
      this.isFinishModalVisible = false;
    },
    showFinishModal() {
      this.finishFormData = { printJobId: null, projectId: null };
      this.isFinishModalVisible = true;
    },
    showEditModal(record) {
      this.isEditPrintJobModalVisible = true;
      this.editPrintJob = { ...record };
    },
    async handleUpdate() {
      try {
        await updatePrintJob(this.editPrintJob.id, this.editPrintJob);
        this.fetchPrintJobs();
        this.isEditPrintJobModalVisible = false;
        message.success('Công việc in đã được cập nhật thành công!');
      } catch (error) {
        console.error('Lỗi khi cập nhật công việc in:', error);
      }
    },
    async handleDelete(printJobId) {
      try {
        await deletePrintJob(printJobId);
        this.fetchPrintJobs();
        message.success('Công việc in đã được xóa thành công!');
      } catch (error) {
        console.error('Lỗi khi xóa công việc:', error);
      }
    },
    handleCancelEdit() {
      this.isEditPrintJobModalVisible = false;
    },
  },
};
</script>

