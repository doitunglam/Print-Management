<template>
    <div>
      <a-breadcrumb style="margin: 16px 0">
        <a-breadcrumb-item>Trang chủ</a-breadcrumb-item>
        <a-breadcrumb-item>Quản lý khách hàng</a-breadcrumb-item>
      </a-breadcrumb>
      <a-table
        :columns="columns"
        :dataSource="customers"
        :pagination="{ pageSize: 10 }"
        rowKey="id"
      >
        <template #bodyCell="{ column, record }">
          <span>{{ record[column.dataIndex] || '-' }}</span>
        </template>
      </a-table>
    </div>
  </template>
  
  <script>
  import { getAllCustomers } from '@/apis/projectApi';
  import { message } from 'ant-design-vue';
  
  export default {
    name: 'CustomerManagement',
    data() {
      return {
        customers: [],
        columns: [
          {
            title: 'ID',
            dataIndex: 'id',
            key: 'id',
          },
          {
            title: 'Tên khách hàng',
            dataIndex: 'fullName',
            key: 'fullName',
          },
          {
            title: 'Số điện thoại',
            dataIndex: 'phoneNumber',
            key: 'phoneNumber',
          },
          {
            title: 'Địa chỉ',
            dataIndex: 'address',
            key: 'address',
          },
          {
            title: 'Email',
            dataIndex: 'email',
            key: 'email',
          },
        ],
      };
    },
    async created() {
      this.fetchCustomers();
    },
    methods: {
      async fetchCustomers() {
        try {
          const data = await getAllCustomers();
          this.customers = data.data;
        } catch (error) {
          message.error(error.message || 'Có lỗi xảy ra khi tải danh sách khách hàng!');
        }
      },
    },
  };
  </script>
  
  <style scoped>
  </style>
  