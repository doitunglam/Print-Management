export type ProductType = {
  id: number
  name: string
  description: string
  price: number
  imageUrl: string
  createdAt: string
  updatedAt: string
  orderCount: number
  rating: number
}

export type DesignType = {
  projectId: number
  designerId: number
  filePath: string
  designTime: string
  designStatus: string
  approverId: number
  id: number
}

export type SortOptionType = {
  name: string
  value: string
  sort: Function
}

export type OrderType = {
  id: number
  productID: number
  designID: number
  orderDate: string
  rating: number
  status: string
  deliveryAddress: string
  phoneNumber: string
  customerID: number
  customerName: string
}
