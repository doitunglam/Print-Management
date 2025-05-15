import axiosInstance from './axiosConfig';

export const getAllUsers = async () => {
  try {
    const response = await axiosInstance.get('/user');
    if (!response.data.success) {
      throw new Error(response.data.message || 'Error fetching users');
    }
    return response.data.data;
  } catch (error) {
    console.error('Error fetching users:', error);
    throw error;
  }
};

export const getAllUsersFromAllUsersApi = async () => {
  try {
    const response = await axiosInstance.get('/Auth/GetAllUsers/all-users');
    if (!response.data) {
      throw new Error(response.data.message || 'Error fetching all users from all-users API');
    }
    return response.data;
  } catch (error) {
    console.error('Error fetching all users from all-users API:', error);
    throw error;
  }
};



export const getUserProfile = async () => {
  try {
    const response = await axiosInstance.get('/user/profile');
    if (!response.data.success) {
      throw new Error(response.data.message || 'Error fetching user profile');
    }
    return response.data.data;
  } catch (error) {
    console.error('Error fetching user profile:', error);
    throw error;
  }
};

export const searchUserByEmail = async (email) => {
  try {
    const response = await axiosInstance.get(`/user/searchByEmail`, { params: { email } });
    if (!response.data.success) {
      throw new Error(response.data.message || 'Error searching user by email');
    }
    return response.data.data;
  } catch (error) {
    console.error('Error searching user by email:', error);
    throw error;
  }
};

export const createUser = async (userData) => {
  try {
    const response = await axiosInstance.post('/user', userData);
    if (!response.data.success) {
      throw new Error(response.data.message || 'Error creating user');
    }
    return response.data.data;
  } catch (error) {
    console.error('Error creating user:', error);
    throw error;
  }
};

export const updateUser = async (id, userData) => {
  try {
    const response = await axiosInstance.put(`/Auth/UpdateUser/update-user/${id}`, userData);
    if (!response.data) {
      throw new Error(response.data.message || 'Error updating user');
    }
    return response.data;
  } catch (error) {
    console.error('Error updating user:', error);
    throw error; // Re-throwing the error will allow the calling code to handle it.
  }
};

// Delete user function
export const deleteUser = async (id) => {
  try {
    const response = await axiosInstance.delete(`/Auth/DeleteUser/delete-user/${id}`);  
    if (!response.data) {
      throw new Error(response.data.message || 'Error deleting user');
    }
    return response.data;
  } catch (error) {
    console.error('Error deleting user:', error);
    throw error; // Re-throwing the error allows handling in the calling code
  }
};

export const addRolesToUser = async (userId, roles) => {
  try {
    const response = await axiosInstance.post(`/Auth/AddRolesToUser/${userId}`, roles);
    return response.data; // Trả về toàn bộ phản hồi
  } catch (error) {
    console.error('Error adding roles to user:', error);
    throw error;
  }
};


