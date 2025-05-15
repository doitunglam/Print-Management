<template>
  <div class="product-management">
    <a-breadcrumb style="margin: 16px 0">
      <a-breadcrumb-item>Trang chủ</a-breadcrumb-item>
      <a-breadcrumb-item>Quản lý sản phẩm</a-breadcrumb-item>
    </a-breadcrumb>
    <div class="actions-bar">
      <a-button type="primary" @click="showCreateModal">Thêm sản phẩm</a-button>
    </div>
    <a-table
      :columns="columns"
      :dataSource="products"
      :pagination="{ pageSize: 10 }"
      rowKey="id"
    >
      <template #bodyCell="{ column, record }">
        <span v-if="column.key === 'imageUrl'">
          <img :src="record.imageUrl" alt="" class="product-image" />
        </span>
        <span v-else>{{ record[column.dataIndex] || "-" }}</span>
      </template>
    </a-table>

    <!-- Add Product Modal -->
    <a-modal
      v-model:visible="isModalVisible"
      title="Thêm sản phẩm"
      @ok="submitProduct"
      @cancel="handleCancel"
    >
      <a-form
        :model="newProduct"
        :rules="rules"
        ref="productForm"
        layout="vertical"
      >
        <a-form-item
          label="Tên sản phẩm"
          name="name"
          style="margin-bottom: 10px"
        >
          <a-input
            v-model:value="newProduct.name"
            placeholder="Nhập tên sản phẩm"
            required
          />
        </a-form-item>
        <a-form-item
          label="Mô tả sản phẩm"
          name="description"
          style="margin-bottom: 10px"
        >
          <a-input
            v-model:value="newProduct.description"
            placeholder="Nhập mô tả sản phẩm"
            required
          />
        </a-form-item>
        <a-form-item
          label="Giá sản phẩm"
          name="price"
          style="margin-bottom: 10px"
        >
          <a-input
            v-model:value="newProduct.price"
            placeholder="Nhập giá sản phẩm"
            required
          />
        </a-form-item>
        <a-form-item
          label="Ảnh sản phẩm"
          name="imageUrl"
          style="margin-bottom: 10px"
        >
          <a-upload :before-upload="handleFileUpload" :show-upload-list="false">
            <a-button icon="upload">Nhấn để Tải lên</a-button>
          </a-upload>
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script>
import { getAllProducts, addProduct } from "@/apis/productApi"
import { ref, uploadBytes, getDownloadURL } from "firebase/storage"
import { storage } from "@/firebaseConfig"
import { message } from "ant-design-vue"

export default {
  name: "ProductManagement",
  data() {
    return {
      products: [],
      newProduct: {
        name: "",
        description: "",
        price: "",
        imageUrl: "",
      },
      columns: [
        {
          title: "Tên sản phẩm",
          dataIndex: "name",
          key: "name",
        },
        {
          title: "Mô tả sản phẩm",
          dataIndex: "description",
          key: "description",
        },
        {
          title: "Hình ảnh sản phẩm",
          dataIndex: "imageUrl",
          key: "imageUrl",
        },
        {
          title: "Giá",
          dataIndex: "price",
          key: "price",
        },
        {
          title: "Thời gian tạo",
          dataIndex: "createdAt",
          key: "createdAt",
        },
      ],
      isModalVisible: false,
      isEditModalVisible: false,
      rules: {
        name: [
          {
            required: true,
            message: "Vui lòng nhập tên sản phẩm!",
            trigger: "change",
          },
        ],
        description: [
          { required: true, message: "Vui lòng nhập mô tả sản phẩm!" },
        ],
        price: [{ required: true, message: "Vui lòng nhập giá sản phẩm!" }],
      },
    }
  },
  methods: {
    async fetchProducts() {
      try {
        this.products = await getAllProducts()
        console.log("this.products", this.products)
      } catch (error) {
        console.error("Lỗi khi lấy danh sách sản phẩm:", error)
      }
    },
    async handleFileUpload(file) {
      try {
        const storageRef = ref(storage, `products/${file.name}`)
        const snapshot = await uploadBytes(storageRef, file)
        const filePath = await getDownloadURL(snapshot.ref)
        this.newProduct.imageUrl = filePath
        return false
      } catch (error) {
        console.error("Lỗi khi tải lên tệp:", error)
        return false
      }
    },
    async submitProduct() {
      this.$refs.productForm
        .validate()
        .then(async () => {
          try {
            console.log("Submitting product:", this.newProduct)
            await addProduct(this.newProduct)
            this.fetchProducts()
            this.isModalVisible = false
          } catch (error) {
            console.error("Lỗi khi thêm sản phẩm:", error)
          }
        })
        .catch(() => {
          message.error("Vui lòng điền đầy đủ thông tin!")
        })
    },
    showCreateModal() {
      this.isModalVisible = true
      this.newProduct = {
        name: "",
        description: "",
        price: 0,
        imageUrl: "",
      }
    },
    handleCancel() {
      this.isModalVisible = false
    },
  },
  mounted() {
    this.fetchProducts()
  },
}
</script>

<style scoped>
.actions-bar {
  display: flex;
  justify-content: space-between;
  margin-bottom: 16px;
}
.product-image {
  height: 100px;
  width: auto;
}
</style>
