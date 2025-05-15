<template>
  <div>
    <a-breadcrumb style="margin: 16px 0">
      <a-breadcrumb-item>Trang chủ</a-breadcrumb-item>
      <a-breadcrumb-item>Quản lý chi tiết thuộc tính tài nguyên</a-breadcrumb-item>
    </a-breadcrumb>
    <div class="actions-bar">
      <a-button type="primary" @click="showCreateDetailModal" class="action-button">Thêm chi tiết thuộc tính tài nguyên</a-button>
      <a-button type="primary" @click="showUsageModal" class="action-button">Sử dụng chi tiết tài nguyên cho công việc in</a-button>
    </div>
    <a-table
      :columns="columns"
      :dataSource="resourcePropertyDetails"
      :pagination="{ pageSize: 10 }"
      rowKey="id"
    >
    <template #bodyCell="{ column, record }">
        <span v-if="column.key === 'actions'">
          <a @click="showEditModal(record)">Sửa</a>
          <a-divider type="vertical" />
          <a-popconfirm
            title="Bạn có chắc chắn muốn xóa chi tiết thuộc tính tài nguyên này không?"
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
    <!-- Modal for Usage -->
    <a-modal
      v-model:visible="isUsageModalVisible"
      title="Sử dụng chi tiết tài nguyên cho công việc in"
      @ok="handleUsageOk"
      @cancel="handleUsageCancel"
    >
      <a-form :model="usageFormData" ref="usageForm" layout="vertical">
        <a-form-item label="Resource Property Detail" name="resourcePropertyDetailId" style="margin-bottom: 10px;">
          <a-select
            v-model:value="usageFormData.resourcePropertyDetailId"
            placeholder="Chọn chi tiết thuộc tính tài nguyên"
            :options="resourcePropertyDetailOptions"
            style="width: 100%"
          />
        </a-form-item>
        <a-form-item label="Print Job" name="printJobId" style="margin-bottom: 10px;">
          <a-select
            v-model:value="usageFormData.printJobId"
            placeholder="Chọn công việc in"
            :options="printJobOptions"
            style="width: 100%"
          />
        </a-form-item>
      </a-form>
    </a-modal>

    <!-- Modal for Adding Resource Property Detail -->
    <a-modal
      v-model:visible="isDetailModalVisible"
      title="Chi tiết thuộc tính tài nguyên"
      @ok="handleDetailOk"
      @cancel="handleDetailCancel"
    >
      <a-form :model="detailFormData" :rules="detailRules" ref="resourcePropertyDetailForm" layout="vertical">
        <a-form-item label="Property ID" name="propertyId" style="margin-bottom: 10px;">
          <a-input v-model:value="detailFormData.propertyId" />
        </a-form-item>
        <a-form-item label="Tên chi tiết thuộc tính" name="propertyDetailName" style="margin-bottom: 10px;">
          <a-input v-model:value="detailFormData.propertyDetailName" />
        </a-form-item>
        <a-form-item label="Giá" name="price" style="margin-bottom: 10px;">
          <a-input-number v-model:value="detailFormData.price" />
        </a-form-item>
        <a-form-item label="Số lượng" name="quantity" style="margin-bottom: 10px;">
          <a-input-number v-model:value="detailFormData.quantity" />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script>
import { getAllResourcePropertyDetails, usingResourceForPrintJob, getAllPrintJobs } from '@/apis/projectApi';
import { createResourcePropertyDetail} from '@/apis/resourcePropertyApi'; // Import the new API
import { message } from 'ant-design-vue';

export default {
  name: 'ResourcePropertyDetails',
  data() {
    return {
      resourcePropertyDetails: [],
      columns: [
        {
          title: 'Property ID',
          dataIndex: 'propertyId',
          key: 'propertyId',
        },
        {
          title: 'Tên chi tiết thuộc tính',
          dataIndex: 'propertyDetailName',
          key: 'propertyDetailName',
        },
        {
          title: 'ID',
          dataIndex: 'id',
          key: 'id',
        },
        {
          title: 'Giá',
          dataIndex: 'price',
          key: 'price',
        },
        {
          title: 'Số lượng',
          dataIndex: 'quantity',
          key: 'quantity',
        },
        {
          title: 'Hành động',
          key: 'actions',
        },
      ],
      isUsageModalVisible: false, // State for the usage modal
      isDetailModalVisible: false, // State for the new detail modal
      usageFormData: { // Data for the usage form
        resourcePropertyDetailId: null,
        printJobId: null,
      },
      detailFormData: { // Data for the new detail form
        propertyId: null,
        propertyDetailName: '',
        image: '',
        price: null,
        quantity: null,
      },
      resourcePropertyDetailOptions: [], // Options for the Resource Property select
      printJobOptions: [], // Options for Print Job select
    };
  },
  async created() {
    await this.fetchResourcePropertyDetails();
    await this.fetchPrintJobOptions(); 
  },
  methods: {
    async fetchResourcePropertyDetails() {
      try {
        const data = await getAllResourcePropertyDetails();
        this.resourcePropertyDetails = Array.isArray(data) ? data : [];
        this.resourcePropertyDetailOptions = this.resourcePropertyDetails.map(detail => ({
          label: detail.propertyDetailName,
          value: detail.id
        }));
      } catch (error) {
        message.error(error.message || 'Có lỗi xảy ra khi tải chi tiết thuộc tính tài nguyên!');
      }
    },
    async fetchPrintJobOptions() {
      try {
        const data = await getAllPrintJobs(); // Replace with actual print job API
        this.printJobOptions = data.map(job => ({
          value: job.id
        }));
      } catch (error) {
        message.error(error.message || 'Có lỗi xảy ra khi tải công việc in!');
      }
    },
    showUsageModal() {
      this.usageFormData = { resourcePropertyDetailId: null, printJobId: null }; // Reset the form data
      this.isUsageModalVisible = true;
    },
    showCreateDetailModal() {
      this.detailFormData = { propertyId: null, propertyDetailName: '', image: '', price: null, quantity: null }; // Reset data
      this.isDetailModalVisible = true;
    },
    async handleUsageOk() {
      this.$refs.usageForm.validate().then(async () => {
        try {
          const payload = {
            resourcePropertyDetailId: this.usageFormData.resourcePropertyDetailId,
            printJobId: this.usageFormData.printJobId,
          };
          await usingResourceForPrintJob(payload);
          message.success('Sử dụng chi tiết tài nguyên cho công việc in thành công!');
          this.isUsageModalVisible = false;
        } catch (error) {
          message.error(error.message || 'Có lỗi xảy ra khi sử dụng chi tiết tài nguyên!');
        }
      }).catch(() => {
        message.error('Vui lòng điền đầy đủ thông tin!');
      });
    },
    async handleDetailOk() {
      this.$refs.resourcePropertyDetailForm.validate().then(async () => {
        try {
          const payload = {
            propertyId: this.detailFormData.propertyId,
            propertyDetailName: this.detailFormData.propertyDetailName,
            price: this.detailFormData.price,
            quantity: this.detailFormData.quantity,
          };
          await createResourcePropertyDetail(payload);
          message.success('Thêm chi tiết thuộc tính thành công!');
          this.isDetailModalVisible = false;
        } catch (error) {
          message.error(error.message || 'Có lỗi xảy ra khi thêm chi tiết thuộc tính!');
        }
      }).catch(() => {
        message.error('Vui lòng điền đầy đủ thông tin!');
      });
    },
    handleUsageCancel() {
      this.isUsageModalVisible = false;
    },
    handleDetailCancel() {
      this.isDetailModalVisible = false;
    },
  },
};
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
