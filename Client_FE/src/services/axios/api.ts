import Http from "@/helpers/http";
import type { OrderType } from "@/types/main";

// MOCK CUSTOMER ID
const CUSTOMER_ID = "0";
Http.defaults.timeout = 20000;

const urlGetAllProducts = "/api/Project/getAllProduct";
const urlGetProductById = "/api/Project/getProductBy";
const urlGetAllDesigns = "/api/Project/all-designs";
const urlCreateOrder = "/api/Project/createOrder";
const urlGetAllOrder = `/api/Project/getAllOrder?customerId=${CUSTOMER_ID}`;
const genOrderUrl = (id: number) => `/api/Project/Order/${id}`;

export const getAllProducts = (): Promise<any> => {
  const config = {
    header: {},
    params: {},
  };
  return Http.get(`${urlGetAllProducts}`, config);
};

export const getProductById = (id: number): Promise<any> => {
  const config = {
    header: {},
    params: {},
  };
  return Http.get(`${urlGetProductById}/${id}`, config);
};

export const getAllDesigns = (): Promise<any> => {
  const config = {
    header: {},
    params: {},
  };
  return Http.get(`${urlGetAllDesigns}`, config);
};

export const createOrder = (order: OrderType): Promise<any> => {
  const config = {
    header: {},
    params: {},
  };
  return Http.post(`${urlCreateOrder}`, order, config);
};

export const getAllOrders = (): Promise<any> => {
  const config = {
    header: {},
    params: {},
  };
  return Http.get(`${urlGetAllOrder}`, config);
};

export const getOrder = async (id: number): Promise<any> => {
  try {
    const config = {
      header: {},
      params: {},
    };
    const repsonse = await Http.get(genOrderUrl(id), config);

    return repsonse.data.data;
  } catch (exception) {
    console.error("Get Order Error", exception);

    return null;
  }
};

export const updateOrder = async (
  order: OrderType
): Promise<any> => {
  try {
    const config = {
      header: {},
      params: {},
    };
    const repsonse = await Http.post(genOrderUrl(order.id), order, config);

    return repsonse;
  } catch (exception) {
    console.error("Update Order Error", exception);

    return null;
  }
};
