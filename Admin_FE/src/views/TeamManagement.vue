<template>
  <div>
    <a-breadcrumb style="margin: 16px 0">
      <a-breadcrumb-item>Trang chủ</a-breadcrumb-item>
      <a-breadcrumb-item>Quản lý đội nhóm</a-breadcrumb-item>
    </a-breadcrumb>
    <div class="actions-bar">
      
      <a-button type="primary" @click="showCreateModal">Thêm đội nhóm</a-button>
    </div>
    <a-table
      :columns="columns"
      :dataSource="teams"
      :pagination="{ pageSize: 10 }"
      rowKey="id"
    >
      <template #bodyCell="{ column, record }">
        <span v-if="column.key === 'actions'">
          <a @click="showEditModal(record)">Sửa</a>
          <a-divider type="vertical" />
          <a-popconfirm
            title="Bạn có chắc chắn muốn xóa đội nhóm này không?"
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
    <a-modal
      v-model:visible="isModalVisible"
      title="Đội nhóm"
      @ok="handleOk"
      @cancel="handleCancel"
    >
      <a-form :model="formData" :rules="rules" ref="teamForm" layout="vertical">
        <a-form-item label="Tên đội nhóm" name="name" style="margin-bottom: 10px;">
          <a-input v-model:value="formData.name" />
        </a-form-item>
        <a-form-item label="Mô tả" name="description" style="margin-bottom: 10px;">
          <a-input v-model:value="formData.description" />
        </a-form-item>
        <a-form-item label="Số thành viên" name="numberOfMember" style="margin-bottom: 10px;">
          <a-input-number v-model:value="formData.numberOfMember" />
        </a-form-item>
        <a-form-item label="ID Quản lý" name="managerId" style="margin-bottom: 10px;">
          <a-input v-model:value="formData.managerId" />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script>
import { getAllTeams, createTeam, updateTeam, deleteTeam } from '@/apis/teamApi';
import { message } from 'ant-design-vue';

export default {
  name: 'TeamManagement',
  data() {
    return {
      teams: [],
      columns: [
        {
          title: 'Tên',
          dataIndex: 'name',
          key: 'name',
        },
        {
          title: 'Mô tả',
          dataIndex: 'description',
          key: 'description',
        },
        {
          title: 'Số thành viên',
          dataIndex: 'numberOfMember',
          key: 'numberOfMember',
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
          title: 'ID Quản lý',
          dataIndex: 'managerId',
          key: 'managerId',
        },
        {
          title: 'Hành động',
          key: 'actions',
        },
      ],
      isModalVisible: false,
      formData: {
        id: null,
        name: '',
        description: '',
        numberOfMember: 0,
        createTime: new Date().toISOString(),
        updateTime: new Date().toISOString(),
        managerId: 0,
      },
      isEditing: false,
      rules: {
        name: [{ required: true, message: 'Vui lòng nhập tên đội nhóm!' }],
        description: [{ required: true, message: 'Vui lòng nhập mô tả!' }],
      },
    };
  },
  async created() {
    this.fetchTeams();
  },
  methods: {
    async fetchTeams() {
      try {
        const data = await getAllTeams();
        console.log(data.data.data)
        this.teams = Array.isArray(data.data.data) ? data.data.data : [];
      } catch (error) {
        message.error(error.message || 'Có lỗi xảy ra khi tải đội nhóm!');
      }
    },
    // async handleSearch(query) {
    //   try {
    //     const data = await searchTeams(query);
    //     this.teams = Array.isArray(data) ? data : [];
    //   } catch (error) {
    //     message.error(error.message || 'Có lỗi xảy ra khi tìm kiếm đội nhóm!');
    //   }
    // },
    showCreateModal() {
      this.isEditing = false;
      this.formData = { id: null, name: '', description: '', numberOfMember: 0, createTime: new Date().toISOString(), updateTime: new Date().toISOString(), managerId: 0 };
      this.isModalVisible = true;
    },
    showEditModal(record) {
      this.isEditing = true;
      this.formData = { ...record };
      this.isModalVisible = true;
    },
    async handleOk() {
      this.$refs.teamForm.validate().then(async () => {
        try {
          const payload = {
            name: this.formData.name,
            description: this.formData.description,
            numberOfMember: this.formData.numberOfMember,
            createTime: this.formData.createTime,
            updateTime: this.formData.updateTime,
            managerId: this.formData.managerId,
          };

          if (this.isEditing) {
            await updateTeam(this.formData.id, payload);
            message.success('Cập nhật đội nhóm thành công!');
          } else {
            await createTeam(payload);
            message.success('Thêm đội nhóm thành công!');
          }
          this.isModalVisible = false;
          this.fetchTeams();
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
        await deleteTeam(id);
        message.success('Xóa đội nhóm thành công!');
        this.fetchTeams();
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