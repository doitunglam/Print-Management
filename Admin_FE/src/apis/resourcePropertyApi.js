import axiosInstance from './axiosConfig';

export const getAllResourceProperties = async () => {
    try {
      const response = await axiosInstance.get('/Project/GetAllResourceProperties');
      return response.data.data;
    } catch (error) {
      console.error('Error getting all resource properties:', error);
      throw error;
    }
  };

  export const createResourceProperty = async (propertyData) => {
    try {
      const response = await axiosInstance.post('/Project/CreateResource-property', propertyData);
      return response.data;
    } catch (error) {
      console.error('Error creating resource property:', error);
      throw error;
    }
  };
  
  // New API for creating resource property detail
  export const createResourcePropertyDetail = async (detailData) => {
    try {
      const response = await axiosInstance.post('/Project/CreateResource-property-detail', detailData);
      return response.data;
    } catch (error) {
      console.error('Error creating resource property detail:', error);
      throw error;
    }
  };

  export const getAllResourcePropertyDetails = async () => {
    try {
        const response = await axiosInstance.get('/Project/GetAllResourcePropertyDetails');
        return response.data.data;
    } catch (error) {
        console.error('Error getting all resource property details:', error);
        throw error;
    }
};