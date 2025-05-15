import axiosInstance from './axiosConfig';

export const createDelivery = async (deliveryData) => {
  try {
    const response = await axiosInstance.post('/Shipping/CreateDelivery', deliveryData);
    return response.data;
  } catch (error) {
    console.error('Error creating delivery:', error);
    throw error;
  }
};

export const getAllDeliveries = async () => {
  try {
    const response = await axiosInstance.get('/Shipping/GetAllDeliveries');
    return response.data;
  } catch (error) {
    console.error('Error fetching all deliveries:', error);
    throw error;
  }
};

export const updateDeliveryStatus = async (deliveryId) => {
  try {
    const response = await axiosInstance.post(`/Shipping/UpdateDelivery-status?deliveryId=${deliveryId}`);
    return response.data;
  } catch (error) {
    console.error('Error updating delivery status:', error);
    throw error;
  }
};

// Thêm các hàm khác tương tự cho các endpoint còn lại
// ... 