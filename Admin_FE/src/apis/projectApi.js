import axiosInstance from "./axiosConfig";

export const createProject = async (projectData) => {
  try {
    const response = await axiosInstance.post(
      "/Project/CreateProject",
      projectData
    );
    return response.data;
  } catch (error) {
    console.error("Error creating project:", error);
    throw error;
  }
};

export const updateProject = async (id, projectData) => {
  try {
    const response = await axiosInstance.put(
      `/Project/UpdateProject/${id}`,
      projectData
    );
    return response.data;
  } catch (error) {
    console.error("Error updating project:", error);
    throw error;
  }
};

export const deleteProject = async (id) => {
  try {
    const response = await axiosInstance.delete(`/Project/DeleteProject/${id}`);
    return response.data;
  } catch (error) {
    console.error("Error deleting project:", error);
    throw error;
  }
};

export const addDesign = async (designData) => {
  try {
    const response = await axiosInstance.post("/Project/AddDesign", designData);
    return response.data;
  } catch (error) {
    console.error("Error adding design:", error);
    throw error;
  }
};

export const approveDesign = async (designId) => {
  try {
    const response = await axiosInstance.post(
      `/Project/ApproveDesign/${designId}`
    );
    return response.data;
  } catch (error) {
    console.error("Error approving design:", error);
    throw error;
  }
};

export const updateDesign = async (designId, designData) => {
  try {
    const response = await axiosInstance.put(
      `/Project/UpdateDesign/${designId}`,
      designData
    );
    return response.data;
  } catch (error) {
    console.error("Error updating design:", error);
    throw error;
  }
};

export const deleteDesign = async (designId) => {
  try {
    const response = await axiosInstance.delete(
      `/Project/DeleteDesign/${designId}`
    );
    return response.data;
  } catch (error) {
    console.error("Error deleting design:", error);
    throw error;
  }
};

export const rejectDesign = async (designId) => {
  try {
    const response = await axiosInstance.post(
      `/Project/RejectDesign/${designId}`
    );
    return response.data;
  } catch (error) {
    console.error("Error rejecting design:", error);
    throw error;
  }
};

export const confirmDesignForPrinting = async (body) => {
  try {
    console.log("Sending request payload:", { designId: body });
    const response = await axiosInstance.post(
      "/Project/ConfirmDesign-for-printing",
      { designId: body }
    );
    return response.data;
  } catch (error) {
    console.error(
      "Error confirming design for printing:",
      error.response ? error.response.data : error.message
    );
    throw error;
  }
};

export const updatePrintJob = async (printJobId, printJobData) => {
  try {
    const response = await axiosInstance.put(
      `/Project/UpdatePrintJob/${printJobId}`,
      printJobData
    );
    return response.data;
  } catch (error) {
    console.error("Error updating print job:", error);
    throw error;
  }
};

export const deletePrintJob = async (printJobId) => {
  try {
    const response = await axiosInstance.delete(
      `/Project/DeletePrintJob/${printJobId}`
    );
    return response.data;
  } catch (error) {
    console.error("Error deleting print job:", error);
    throw error;
  }
};

export const createResources = async (resourceData) => {
  try {
    const response = await axiosInstance.post(
      "/Project/CreateResources",
      resourceData
    );
    return response.data;
  } catch (error) {
    console.error("Error creating resources:", error);
    throw error;
  }
};

export const createResourceProperty = async (propertyData) => {
  try {
    const response = await axiosInstance.post(
      "/Project/CreateResource-property",
      propertyData
    );
    return response.data;
  } catch (error) {
    console.error("Error creating resource property:", error);
    throw error;
  }
};

// New API for creating resource property detail
export const createResourcePropertyDetail = async (detailData) => {
  try {
    const response = await axiosInstance.post(
      "/Project/CreateResource-property-detail",
      detailData
    );
    return response.data;
  } catch (error) {
    console.error("Error creating resource property detail:", error);
    throw error;
  }
};

export const getAllResourcePropertyDetails = async () => {
  try {
    const response = await axiosInstance.get(
      "/Project/GetAllResourcePropertyDetails"
    );
    return response.data.data; // Assuming the API response has a "data" field
  } catch (error) {
    console.error("Error getting resource property details:", error);
    throw error;
  }
};

export const createResourceForPrintJob = async (resourceData) => {
  try {
    const response = await axiosInstance.post(
      "/Project/CreateResource-for-print-job",
      resourceData
    );
    return response.data;
  } catch (error) {
    console.error("Error creating resource for print job:", error);
    throw error;
  }
};

export const usingResourceForPrintJob = async (resourceData) => {
  try {
    const response = await axiosInstance.post(
      "/Project/UsingResource-for-print-job",
      resourceData
    );
    return response.data;
  } catch (error) {
    console.error("Error using resource for print job:", error);
    throw error;
  }
};

export const confirmFinishingProject = async (data) => {
  try {
    const response = await axiosInstance.post(
      "/Project/ConfirmFinishing-project",
      data
    );
    return response.data;
  } catch (error) {
    console.error("Error confirming finishing project:", error);
    throw error;
  }
};

export const getAllResources = async () => {
  try {
    const response = await axiosInstance.get("/Project/get-all-resources");
    return response.data;
  } catch (error) {
    console.error("Error getting all resources:", error);
    throw error;
  }
};

export const getAllResourceProperties = async () => {
  try {
    const response = await axiosInstance.get(
      "/Project/GetAllResourceProperties"
    );
    return response.data;
  } catch (error) {
    console.error("Error getting all resource properties:", error);
    throw error;
  }
};

export const getAllProjects = async () => {
  try {
    const response = await axiosInstance.get("/Project/all-projects");
    return response.data;
  } catch (error) {
    console.error("Error getting all projects:", error);
    throw error;
  }
};

export const getAllDesigns = async () => {
  try {
    const response = await axiosInstance.get("/Project/all-designs");
    return response.data;
  } catch (error) {
    console.error("Error getting all designs:", error);
    throw error;
  }
};

export const getAllPrintJobs = async () => {
  try {
    const response = await axiosInstance.get("/Project/print-jobs");
    return response.data;
  } catch (error) {
    console.error("Error getting all print jobs:", error);
    throw error;
  }
};

export const getAllCustomers = async () => {
  try {
    const response = await axiosInstance.get("/Project/all-customers");
    return response.data;
  } catch (error) {
    console.error("Error getting all customers:", error);
    throw error;
  }
};

// ==== StepTemplate ====
export const getAllStepTemplate = async () => {
  try {
    const response = await axiosInstance.get("/Project/StepTemplate");
    return response.data;
  } catch (error) {
    console.error("Error getting all StepTemplate:", error);
  }
};

export const addStepTemplate = async (stepTemplateData) => {
  const response = await axiosInstance.post(
    "/Project/StepTemplate",
    stepTemplateData
  );
  return response.data;
};

export const editStepTemplate = async (stepTemplateData) => {
  const response = await axiosInstance.put(
    `/Project/StepTemplate/${stepTemplateData.id}`,
    stepTemplateData
  );
  return response.data;
};

export const deleteStepTemplate = async (stepTemplateId) => {
  const response = await axiosInstance.delete(
    `/Project/StepTemplate/${stepTemplateId}`
  );
  return response.data;
};

// ==== FlowTemplate ====
export const getAllFlowTemplate = async () => {
    const response = await axiosInstance.get("/Project/FlowTemplate");
    return response.data;
};

export const addFlowTemplate = async (stepTemplateData) => {
  const response = await axiosInstance.post(
    "/Project/FlowTemplate",
    stepTemplateData
  );
  return response.data;
};

export const editFlowTemplate = async (stepTemplateData) => {
  const response = await axiosInstance.put(
    `/Project/FlowTemplate/${stepTemplateData.id}`,
    stepTemplateData
  );
  return response.data;
};

export const deleteFlowTemplate = async (stepTemplateId) => {
  const response = await axiosInstance.delete(
    `/Project/FlowTemplate/${stepTemplateId}`
  );
  return response.data;
};
// Thêm các hàm khác tương tự cho các endpoint còn lại
// ...
