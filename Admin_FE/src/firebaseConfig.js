import { initializeApp } from 'firebase/app';
import { getStorage } from 'firebase/storage';

const firebaseConfig = {
    apiKey: "AIzaSyDSSPBo1xMEqjPlsmscoXvAPat2rNR1s-M",
    authDomain: "zalo-app-66612.firebaseapp.com",
    databaseURL: "https://zalo-app-66612-default-rtdb.firebaseio.com",
    projectId: "zalo-app-66612",
    storageBucket: "zalo-app-66612.appspot.com",
    messagingSenderId: "1075698897426",
    appId: "1:1075698897426:web:4e8536e451ed77a0767ecb",
    measurementId: "G-3C42XLGJ3E"
};

// Khởi tạo Firebase
const app = initializeApp(firebaseConfig);

// Khởi tạo Firebase Storage
const storage = getStorage(app);

export { storage }; 