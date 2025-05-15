<template>
  <div>
    <a-breadcrumb style="margin: 16px 0">
      <a-breadcrumb-item>Trang chủ</a-breadcrumb-item>
      <a-breadcrumb-item>Quản lý tài nguyên</a-breadcrumb-item>
    </a-breadcrumb>
    <div class="actions-bar">
      <a-button v-if="isAdmin" type="primary" @click="showCreateModal">Thêm tài nguyên</a-button>
    </div>
    <a-table
      :columns="columns"
      :dataSource="resources"
      :pagination="{ pageSize: 10 }"
      rowKey="id"
    >
      <template #bodyCell="{ column, record }">
        <span v-if="column.key === 'actions'">
          <a @click="showEditModal(record)">Sửa</a>
          <a-divider type="vertical" />
          <a-popconfirm
            title="Bạn có chắc chắn muốn xóa tài nguyên này không?"
            ok-text="Có"
            cancel-text="Không"
            @confirm="handleDelete(record.id)"
          >
            <a>Xóa</a>
          </a-popconfirm>
        </span>
        <span v-else-if="column.key === 'image'">
          <img :src="record[column.dataIndex]" alt="Image" style="width: 50px; height: 50px;" />
        </span>
        <span v-else>{{ record[column.dataIndex] || '-' }}</span>
      </template>
    </a-table>
    <a-modal
      v-model:visible="isModalVisible"
      title="Tài nguyên"
      @ok="handleOk"
      @cancel="handleCancel"
    >
      <a-form :model="formData" :rules="rules" ref="resourceForm" layout="vertical">
        <a-form-item label="Tên tài nguyên" name="resourceName" style="margin-bottom: 10px;">
          <a-input v-model:value="formData.resourceName" />
        </a-form-item>
        <!-- <a-form-item label="Hình ảnh" name="image" style="margin-bottom: 10px;">
          <a-upload
            name="file"
            :customRequest="handleCustomRequest"
            list-type="picture-card"
            :show-upload-list="false"
          >
            <div>
              <a-icon type="plus" />
              <div style="margin-top: 8px">Upload</div>
            </div>
          </a-upload>
          <img v-if="formData.image" :src="formData.image" alt="Image" style="width: 100px; margin-top: 8px;" />
        </a-form-item> -->
        <a-form-item label="Loại tài nguyên (0: MayIn, 1: Giay, 2: Muc)" name="resourceType" style="margin-bottom: 10px;">
          <a-input-number v-model:value="formData.resourceType" />
        </a-form-item>
        <a-form-item label="Số lượng có sẵn" name="availableQuantity" style="margin-bottom: 10px;">
          <a-input-number v-model:value="formData.availableQuantity" />
        </a-form-item>
        <a-form-item label="Trạng thái tài nguyên (0: SanSangSuDung, 1: CanBaoTri, 2: CanNhapThem)" name="resourceStatus" style="margin-bottom: 10px;"> 
          <a-input-number v-model:value="formData.resourceStatus" />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script>
import { getAllResources, createResources} from '@/apis/projectApi';
import { uploadImage } from '@/apis/uploadApi';
import { message } from 'ant-design-vue';

export default {
  name: 'ResourceManagement',
  data() {
    return {
      resources: [],
      columns: [
        {
          title: 'Tên tài nguyên',
          dataIndex: 'resourceName',
          key: 'resourceName',
        },
        {
          title: 'ID',
          dataIndex: 'id',
          key: 'id',
        },
        // {
        //   title: 'Hình ảnh',
        //   dataIndex: 'image',
        //   key: 'image',
        // },
        {
          title: 'Loại tài nguyên',
          dataIndex: 'resourceType',
          key: 'resourceType',
        },
        {
          title: 'Số lượng có sẵn',
          dataIndex: 'availableQuantity',
          key: 'availableQuantity',
        },
        {
          title: 'Trạng thái tài nguyên',
          dataIndex: 'resourceStatus',
          key: 'resourceStatus',
        },
        {
          title: 'Hành động',
          key: 'actions',
        },
      ],
      isModalVisible: false,
      formData: {
        id: null,
        resourceName: '',
        image: '',
        resourceType: 0,
        availableQuantity: 0,
        resourceStatus: 0,
      },
      isEditing: false,
      rules: {
        resourceName: [{ required: true, message: 'Vui lòng nhập tên tài nguyên!' }],
        image: [{ required: true, message: 'Vui lòng tải lên hình ảnh!' }],
        resourceType: [{ required: true, message: 'Vui lòng nhập loại tài nguyên! (0: MayIn, 1: Giay, 2: Muc)'}],
        availableQuantity: [{ required: true, message: 'Vui lòng nhập số lượng có sẵn!' }],
        resourceStatus: [{ required: true, message: 'Vui lòng nhập trạng thái tài nguyên! (0: SanSangSuDung, 1: CanBaoTri, 2: CanNhapThem)'}],
      },
    };
  },
  computed: {
    isAdmin() {
      const roles = JSON.parse(localStorage.getItem('roles') || '[]');
      return roles.includes('Admin');
    }
  },
  async created() {
    this.fetchResources();
  },
  methods: {
    async fetchResources() {
      try {
        const data = await getAllResources();
        this.resources = data.data;
      } catch (error) {
        message.error(error.message || 'Có lỗi xảy ra khi tải tài nguyên!');
      }
    },
    // async handleSearch(query) {
    //   try {
    //     const data = await searchResources(query);
    //     this.resources = data;
    //   } catch (error) {
    //     message.error(error.message || 'Có lỗi xảy ra khi tìm kiếm tài nguyên!');
    //   }
    // },
    showCreateModal() {
      this.isEditing = false;
      this.formData = { id: null, resourceName: '', image: '', resourceType: 0, availableQuantity: 0, resourceStatus: 0 };
      this.isModalVisible = true;
    },
    showEditModal(record) {
      this.isEditing = true;
      this.formData = { ...record };
      this.isModalVisible = true;
    },
    async handleOk() {
      this.$refs.resourceForm.validate().then(async () => {
        try {
          const payload = {
            resourceName: this.formData.resourceName,
            image: this.formData.image,
            resourceType: this.formData.resourceType,
            availableQuantity: this.formData.availableQuantity,
            resourceStatus: this.formData.resourceStatus,
          };

          if (this.isEditing) {
            // await updateResource(this.formData.id, payload);
            message.success('Cập nhật tài nguyên thành công!');
          } else {
            await createResources(payload);
            message.success('Thêm tài nguyên thành công!');
          }
          this.isModalVisible = false;
          this.fetchResources();
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
        // await deleteResource(id);
        message.success('Xóa tài nguyên thành công!' + id);
        this.fetchResources();
      } catch (error) {
        message.error(error.message || 'Có lỗi xảy ra!');
      }
    },
    async handleCustomRequest({ file, onSuccess, onError }) {
      try {
        const response = await uploadImage(file);
        this.formData.image = response.data.url; 
        onSuccess(response, file);
        message.success(`${file.name} file uploaded successfully`);
      } catch (error) {
        onError(error);
        message.error(`${file.name} file upload failed.`);
      }
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