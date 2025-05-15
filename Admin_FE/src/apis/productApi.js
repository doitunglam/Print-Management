import axiosInstance from "./axiosConfig"

export const addProduct = async (productData) => {
  try {
    const response = await axiosInstance.post(
      "/Project/CreateProduct",
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
    const response = await axiosInstance.get("/Project/getAllProduct")
    return response.data.data
  } catch (error) {
    console.error("Error getting all products:", error)
    throw error
  }
}
