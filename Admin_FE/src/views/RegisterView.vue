<template>
  <div class="register-container">
    <div class="register-background">
      <a-card class="register-card">
        <h2>Đăng ký tài khoản mới</h2>
        <p class="register-description">Vui lòng điền thông tin dưới đây để tạo tài khoản mới</p>
        <a-form :model="formData" @submit.prevent="handleRegister" layout="vertical">
          <a-form-item label="Tên người dùng" name="userName" :rules="[ { required: true, message: 'Vui lòng nhập tên người dùng!' } ]">
            <a-input v-model:value="formData.userName" placeholder="Nhập tên người dùng của bạn" />
          </a-form-item>
          <a-form-item label="Mật khẩu" name="password" :rules="[ { required: true, message: 'Vui lòng nhập mật khẩu!' } ]">
            <a-input type="password" v-model:value="formData.password" placeholder="Nhập mật khẩu của bạn" />
          </a-form-item>
          <a-form-item label="Họ và tên" name="fullName" :rules="[ { required: true, message: 'Vui lòng nhập họ và tên!' } ]">
            <a-input v-model:value="formData.fullName" placeholder="Nhập họ và tên của bạn" />
          </a-form-item>
          <a-form-item label="Ngày sinh" name="dateOfBirth" :rules="[ { required: true, message: 'Vui lòng nhập ngày sinh!' } ]" style="width: 100%;">
            <a-date-picker v-model:value="formData.dateOfBirth" placeholder="Chọn ngày sinh của bạn" style="width: 100%;" />
          </a-form-item>
          <a-form-item label="Email" name="email" :rules="[ { required: true, message: 'Vui lòng nhập email!' } ]">
            <a-input v-model:value="formData.email" placeholder="Nhập email của bạn" />
          </a-form-item>
          <a-form-item label="Số điện thoại" name="phoneNumber" :rules="[ { required: true, message: 'Vui lòng nhập số điện thoại!' } ]">
            <a-input v-model:value="formData.phoneNumber" placeholder="Nhập số điện thoại của bạn" />
          </a-form-item>
          <a-button type="primary" html-type="submit" block>Đăng ký</a-button>
        </a-form>
        <p class="login-link">Đã có tài khoản? <router-link to="/login">Đăng nhập ngay</router-link></p>
      </a-card>
    </div>
  </div>
</template>

<script>
import { registerUser } from '@/apis/authApi';
import { message } from 'ant-design-vue';

export default {
  name: 'RegisterView',
  data() {
    return {
      formData: {
        userName: '',
        password: '',
        fullName: '',
        dateOfBirth: '',
        email: '',
        phoneNumber: '',
      },
    };
  },
  methods: {
    async handleRegister() {
      try {
        const response = await registerUser(this.formData);
        if (response) {
          message.success('Đăng ký thành công! Vui lòng kiểm tra email để xem thông tin tài khoản.');
          this.$router.push('/login');
        } else {
          message.error(response.message || 'Đăng ký thất bại!');
        }
      } catch (error) {
        message.error(error.message || 'Có lỗi xảy ra!');
      }
    },
  },
};
</script>

<style scoped>
.register-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background: url('https://source.unsplash.com/random/1920x1080') no-repeat center center;
  background-size: cover;
}

.register-background {
  background-color: rgba(255, 255, 255, 0.9);
  padding: 30px;
  border-radius: 15px;
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.3);
}

.register-card {
  width: 400px;
  text-align: center;
}

.register-card h2 {
  margin-bottom: 10px;
  color: #333;
}

.register-description {
  margin-bottom: 20px;
  color: #666;
  font-size: 14px;
}

a-input, a-date-picker {
  margin-bottom: 15px;
}

a-button {
  margin-top: 20px;
}

.login-link {
  margin-top: 15px;
  color: #007bff;
}

.login-link a {
  color: #007bff;
  text-decoration: none;
}

.login-link a:hover {
  text-decoration: underline;
}

.ant-card-bordered {
  border: 0px solid #f0f0f0;
}
</style> 