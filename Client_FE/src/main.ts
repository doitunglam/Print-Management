import { createApp } from "vue";
import { createPinia } from "pinia";
import { createNotivue } from "notivue";
import "notivue/notification.css";
import "notivue/animations.css";
import App from "./App.vue";
import router from "./router";
import Antd from "ant-design-vue";
import "./assets/css/main.css";

const notivue = createNotivue({
  position: "top-center",
  limit: 4,
  notifications: {
    global: {
      duration: 3000,
    },
  },
});
const app = createApp(App);

app.use(createPinia());
app.use(router);
app.use(notivue);
app.use(Antd);

app.mount("#app");
