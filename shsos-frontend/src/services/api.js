import axios from 'axios';

const api = axios.create({
    baseURL: 'http://localhost:5129', // Using HTTP to avoid localhost cert issues
    withCredentials: true,
});

export default api;
