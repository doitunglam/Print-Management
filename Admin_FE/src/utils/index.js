const ROLE = {
  USER: "User",
  EMPLOYEE: "Employee",
  DESIGNER: "Designer",
  SHIPPER: "Shipper",
  ADMIN: "Admin",
};

const PAGE = {
  ACCOUNT: "Account",
  TEAM: "Team",
  RESOURCE: "Resource",
  RESOURCE_PROPERTY: "Resource Property",
  SHIPPING: "Shipping",
  USER: "User",
  PROJECT: "Project",
  PROJECT_PRODUCT: "Project Product",
  PROJECT_ORDER: "Project Order",
  PROJECT_DESIGN: "Project Design",
  PROJECT_PRINT: "Project Print",
  PROJECT_DETAIL: "Project Detail",
  PROJECT_ORDER_FLOW: "Project Order Flow",
};

// Permission code: 0 (Read); 1: (Write)
const translatePermission = (role, page) => {
  // debugger
  switch (role) {
    case ROLE.USER:
      switch (page) {
        case PAGE.PROJECT:
        case PAGE.PROJECT_DESIGN:
        case PAGE.PROJECT_PRINT:
          return 0;
        default:
          return 1;
      }
    case ROLE.SHIPPER:
      switch (page) {
        case PAGE.PROJECT:
          return 0;
        default:
          return 1;
      }
    case ROLE.DESIGNER:
      switch (page) {
        case PAGE.PROJECT:
          return 0;
        default:
          return 1;
      }
    default:
      return 1;
  }
};

export const checkPermission = (page) => {
  const role = JSON.parse(localStorage.getItem("roles"));
  console.log(translatePermission(role, page), page);
  return translatePermission(role, page);
  //   const roles = JSON.parse(localStorage.getItem("roles"));
  //   console.log(localStorage.getItem("roles"))
  //   return roles
  //     .map((role) => translatePermission(role, page))
  //     .reduce((max, current) => Math.max(max, current), 0);
};
