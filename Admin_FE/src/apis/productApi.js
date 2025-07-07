import axiosInstance from "./axiosConfig"

export const addProduct = async (productData) => {
  try {
    const response = await axiosInstance.post(
      "/Project/Product",
      productData
    )
    return response.data
  } catch (error) {
    console.error("Error adding product:", error)
    throw error
  }
}

export const getAllProducts = async () => {
  try {
    const response = await axiosInstance.get("/Project/Product")
    return response.data.data
  } catch (error) {
    console.error("Error getting all products:", error)
    throw error
  }
}

export const updateProduct = async (id, productData) => {
  try {
    const response = await axiosInstance.put(
      `/Project/Product/${id}`,
      productData
    );
    return response.data;
  } catch (error) {
    console.error("Error updating product:", error);
    throw error;
  }
};

export const deleteProduct = async (id) => {
  try {
    const response = await axiosInstance.delete(`/Project/Product/${id}`);
    return response.data;
  } catch (error) {
    console.error("Error deleting product:", error);
    throw error;
  }
};