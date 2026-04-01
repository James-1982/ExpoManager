import axios from 'axios';

const api = axios.create({
    baseURL: 'https://localhost:7017/api/v1', // punta direttamente alla versione v1
    withCredentials: true,                    // se nel backend AllowCredentials è true
  });

export default api;