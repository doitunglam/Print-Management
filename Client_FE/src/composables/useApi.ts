import {
  getAllProducts,
  getAllDesigns,
  getProductById,
  createOrder,
  getAllOrders,
  updateOrder,
} from "@/services/axios/api"
import type { OrderType } from "@/types/main"

export const useGetAllProducts = async () => {
  try {
    const response = await getAllProducts()
    const data = response.data.data
    return data
  } catch (e) {
    console.error(e)
    return null
  }
}

export const useGetProductById = async (id: number) => {
  try {
    const response = await getProductById(id)
    const data = response.data.data
    return data
  } catch (e) {
    console.error(e)
    return null
  }
}

export const useGetAllDesigns = async () => {
  try {
    const response = await getAllDesigns()
    return response.data
  } catch (e) {
    console.error(e)
    return null
  }
}

export const useGetAllOrders = async () => {
  try {
    const response = await getAllOrders()
    return response.data
  } catch (e) {
    console.error(e)
    return null
  }
}

export const useCreateOrder = async (order: OrderType) => {
  try {
    const response = await createOrder(order)
    return response.data
  } catch (e) {
    console.error(e)
    return null
  }
}

export const useUpdateOrder = async (order: OrderType) => {
    try {
    await updateOrder(order)
  } catch (e) {
    console.error(e)
    return null
  }
}
