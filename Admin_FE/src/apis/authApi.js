import axiosInstance from "./axiosConfig"

export const registerUser = async (userData) => {
  try {
    const response = await axiosInstance.post("/auth/register", userData)
    return response.data
  } catch (error) {
    console.error("Error registering user:", error)
    throw error
  }
}

export const confirmRegister = async (confirmCode) => {
  try {
    const response = await axiosInstance.post(
      `/auth/confirmRegisterAccount?confirmCode=${confirmCode}`
    )
    return response.data
  } catch (error) {
    console.error("Error confirming user registration:", error)
    throw error
  }
}

export const loginUser = async (credentials) => {
  try {
    const response = await axiosInstance.post("/Auth/Login", credentials)
    return response.data
  } catch (error) {
    console.error("Error logging in:", error)
    throw error
  }
}

export const getUserInfo = async () => {
  try {
    const response = await axiosInstance.get("/Auth/GetUserInfo/userinfo")
    return response.data
  } catch (error) {
    console.error("Error fetching user info:", error)
    throw error
  }
}

export const confirmRegisterAccount = async (data) => {
  try {
    const response = await axiosInstance.post(
      "/api/Auth/ConfirmRegisterAccount",
      data
    )
    return response.data
  } catch (error) {
    console.error("Error confirming registration:", error)
    throw error
  }
}

export const changePassword = async (data) => {
  try {
    const response = await axiosInstance.put("/api/Auth/ChangePassword", data)
    return response.data
  } catch (error) {
    console.error("Error changing password:", error)
    throw error
  }
}

export const addRolesToUser = async (userId, roles) => {
  try {
    const response = await axiosInstance.post(`/api/Auth/AddRolesToUser`, {
      userId: userId,
      roles: roles,
    })
    return response.data
  } catch (error) {
    console.error("Error adding roles to user:", error)
    throw error
  }
}
