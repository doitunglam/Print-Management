<template>
  <div>
    <a-breadcrumb style="margin: 16px 0">
      <a-breadcrumb-item>Trang chủ</a-breadcrumb-item>
      <a-breadcrumb-item>Quản lý người dùng</a-breadcrumb-item>
    </a-breadcrumb>
    <div class="actions-bar">
      <a-button type="primary" @click="showCreateModal">Thêm người dùng</a-button>
    </div>
    <a-table
      :columns="columns"
      :dataSource="users"
      :pagination="{ pageSize: 10 }"
      rowKey="id"
    >
    <template #bodyCell="{ column, record }">
  <span v-if="column.key === 'actions'">
    <!-- "Thêm vai trò" above "Sửa" and "Xóa" -->
    <a @click="showRoleModal(record)" style="display: block; margin-bottom: 8px;">Thêm vai trò</a>
    <a @click="showEditModal(record)" style="display: block; margin-bottom: 8px;">Sửa</a>
    <a-popconfirm
      title="Bạn có chắc chắn muốn xóa người dùng này không?"
      ok-text="Có"
      cancel-text="Không"
      @confirm="() => handleDelete(record.id)"
    >
      <a>Xóa</a>
    </a-popconfirm>
  </span>
  <span v-else>{{ record[column.dataIndex] || '-' }}</span>
</template>
    </a-table>
    <a-modal
      v-model:visible="isModalVisible"
      title="Quản lý người dùng"
      @ok="handleOk"
      @cancel="handleCancel"
    >
    <a-form :model="formData" :rules="rules" ref="userForm" layout="vertical">
  <a-form-item label="Tên tài khoản" name="userName" style="margin-bottom: 10px;">
    <a-input v-model:value="formData.userName" />
  </a-form-item>
  <a-form-item label="Tên người dùng" name="fullName" style="margin-bottom: 10px;">
    <a-input v-model:value="formData.fullName" />
  </a-form-item>
  <a-form-item label="Email" name="email" style="margin-bottom: 10px;">
    <a-input v-model:value="formData.email" />
  </a-form-item>
  <a-form-item label="Số điện thoại" name="phoneNumber" style="margin-bottom: 10px;">
    <a-input v-model:value="formData.phoneNumber" />
  </a-form-item>
  <a-form-item label="Vai trò" name="roles" style="margin-bottom: 10px;">
    <a-select
      mode="tags"
      style="width: 100%"
      placeholder="Nhập vai trò"
      v-model:value="selectedRoles"
    >
      <a-select-option v-for="role in availableRoles" :key="role">
        {{ role }}
      </a-select-option>
    </a-select>
  </a-form-item>
</a-form>

    </a-modal>
    <a-modal
      v-model:visible="isRoleModalVisible"
      title="Thêm vai trò"
      @ok="handleAddRoles"
      @cancel="handleCancel"
    >
      <a-select
        mode="tags"
        style="width: 100%"
        placeholder="Nhập vai trò"
        v-model:value="selectedRoles"
      >
        <a-select-option v-for="role in availableRoles.filter(role => !users.find(user => user.id === currentUserId)?.roles.includes(role))" :key="role">
          {{ role }}
        </a-select-option>
      </a-select>
    </a-modal>
  </div>
</template>

<script>
import { createUser, updateUser, deleteUser } from '@/apis/userApi';
import { getAllUsersFromAllUsersApi, addRolesToUser } from '@/apis/userApi';
import { message } from 'ant-design-vue';
// import dayjs from 'dayjs';

export default {
  name: 'UserManagement',
  data() {
    return {
      users: [],
      columns: [
      {
          title: 'Id',
          dataIndex: 'id',
          key: 'id',
        },
        {
          title: 'Tài khoản',
          dataIndex: 'userName',
          key: 'userName',
        },
        {
          title: 'Tên người dùng',
          dataIndex: 'fullName',
          key: 'fullName',
        },
        {
          title: 'Email',
          dataIndex: 'email',
          key: 'email',
        },
        {
          title: 'Số điện thoại',
          dataIndex: 'phoneNumber',
          key: 'phoneNumber',
        },
        {
          title: 'Thời gian tạo',
          dataIndex: 'createTime',
          key: 'createTime',
        },
        {
          title: 'Thời gian cập nhật',
          dataIndex: 'updateTime',
          key: 'updateTime',
        },
        {
          title: 'ID Đội nhóm',
          dataIndex: 'teamId',
          key: 'teamId',
        },
        {
          title: 'Vai trò',
          dataIndex: 'roles',
          key: 'roles',
          customRender: ({ text }) => text.join(', '),
        },
        {
          title: 'Hành động',
          key: 'actions',
        },
      ],

      isModalVisible: false,
      isRoleModalVisible: false,
      formData: {
        id: null,
        userName: '',
        fullName: '',
        email: '',
        phoneNumber: '',
        roles: [],
      },
      selectedRoles: [],
      isEditing: false,
      availableRoles: ['Admin', 'User', 'Employee', 'Designer', 'Shipper'], 
      rules: {
        fullName: [{ required: true, message: 'Vui lòng nhập tên người dùng!' }],
        email: [{ required: true, message: 'Vui lòng nhập email!' }],
        phoneNumber: [{ required: true, message: 'Vui lòng nhập số điện thoại!' }],
      },
    };
  },
  
  async created() {
    this.fetchUsers();
  },
  methods: {
    async fetchUsers() {
      try {
        const data = await getAllUsersFromAllUsersApi();
        this.users = Array.isArray(data) ? data : [];
      } catch (error) {
        message.error(error.message || 'Có lỗi xảy ra khi tải người dùng!');
      }
    },
    showRoleModal(record) {
      this.currentUserId = record.id;
      this.selectedRoles = []; // Khởi tạo selectedRoles rỗng
      this.isRoleModalVisible = true;
    },
    async handleAddRoles() {
  try {
    console.log('User ID:', this.currentUserId);
    console.log('Selected Roles:', this.selectedRoles);

    if (typeof this.currentUserId !== 'number' || !Number.isInteger(this.currentUserId)) {
      throw new Error('User ID is not a long integer.');
    }

    if (!Array.isArray(this.selectedRoles)) {
      throw new Error('Selected Roles is not an array.');
    }

    this.selectedRoles.forEach(role => {
      if (typeof role !== 'string') {
        throw new Error(`Role ${role} is not a string.`);
      }
    });

    // Gọi API và nhận phản hồi
    const response = await addRolesToUser(this.currentUserId, this.selectedRoles);

    // Kiểm tra phản hồi và hiển thị thông báo phù hợp
    if (response.message === 'Add roles successfully.') {
      message.success('Thêm vai trò thành công!');
    } else if (response.error) {
      message.error(response.error);
    } else {
      message.error('Có lỗi xảy ra khi thêm vai trò!');
    }

    this.isRoleModalVisible = false;
    this.fetchUsers();
  } catch (error) {
    console.error('Error in handleAddRoles:', error);
    message.error(error.message || 'Có lỗi xảy ra khi thêm vai trò!');
  }
},


    showCreateModal() {
      this.isEditing = false;
      this.formData = { id: null, fullName: '', email: '', phoneNumber: '', roles: [] };
      this.isModalVisible = true;
    },
    showEditModal(record) {
      this.isEditing = true;
      this.formData = { ...record };
      this.selectedRoles = [...record.roles];  // set roles to the current user's roles
      this.isModalVisible = true;
    },
    

    async handleOk() {
      this.$refs.userForm.validate().then(async () => {
        try {
          const payload = {
            fullName: this.formData.fullName,
            userName: this.formData.userName,
            email: this.formData.email,
            phoneNumber: this.formData.phoneNumber,
            roles: this.selectedRoles,
          };

          if (this.isEditing) {
            await updateUser(this.formData.id, payload);
            message.success('Cập nhật người dùng thành công!');
          } else {
            await createUser(payload);
            message.success('Thêm người dùng thành công!');
          }
          this.isModalVisible = false;
          this.fetchUsers();
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
        await deleteUser(id);
        message.success('Xóa người dùng thành công!');
        this.fetchUsers();
      } catch (error) {
        message.error(error.message || 'Có lỗi xảy ra!');
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
