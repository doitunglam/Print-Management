import axiosInstance from './axiosConfig';

export const createTeam = async (teamData) => {
  try {
    const response = await axiosInstance.post('/Team/CreateTeam', teamData);
    return response.data;
  } catch (error) {
    console.error('Error creating team:', error);
    throw error;
  }
};

export const getTeamById = async (id) => {
  try {
    const response = await axiosInstance.get(`/Team/GetTeamById/${id}`);
    return response.data;
  } catch (error) {
    console.error('Error fetching team by ID:', error);
    throw error;
  }
};

export const updateTeam = async (id, teamData) => {
  try {
    const response = await axiosInstance.put(`/Team/UpdateTeam/${id}`, teamData);
    return response.data;
  } catch (error) {
    console.error('Error updating team:', error);
    throw error;
  }
};

export const deleteTeam = async (id) => {
  try {
    const response = await axiosInstance.delete(`/Team/DeleteTeam/${id}`);
    return response.data;
  } catch (error) {
    console.error('Error deleting team:', error);
    throw error;
  }
};

export const addUserToTeam = async (teamId, userId) => {
  try {
    const response = await axiosInstance.post(`/Team/team/${teamId}/user/${userId}`);
    return response.data;
  } catch (error) {
    console.error('Error adding user to team:', error);
    throw error;
  }
};

export const getAllTeams = async () => {
  try {
    const response = await axiosInstance.get('/Team/GetAllTeams');
    return response.data;
  } catch (error) {
    console.error('Error fetching all teams:', error);
    throw error;
  }
};

// Thêm các hàm khác tương tự cho các endpoint còn lại
// ... 