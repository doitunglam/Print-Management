import { defineStore } from "pinia"
import type { ProductType, DesignType } from "@/types/main"

export const useMainStore = defineStore("main", {
  state: () => ({
    selectedProduct: {} as ProductType,
    selectedDesign: {} as DesignType,
  }),

  getters: {
    getSelectedProduct: (state) => state.selectedProduct,
    getSelectedDesign: (state) => state.selectedDesign,
  },

  actions: {
    setSelectedProduct(selectedProduct: ProductType) {
      this.selectedProduct = selectedProduct
    },
    setSelectedDesign(selectedDesign: DesignType) {
      this.selectedDesign = selectedDesign
    },
  },
})
