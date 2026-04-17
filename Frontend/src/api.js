import axios from 'axios';

const API = axios.create({
  baseURL: 'http://localhost:5000/api',
});

// Attach JWT token to every request if available
API.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Handle 401 responses globally
API.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response && error.response.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      if (window.location.pathname !== '/login') {
        window.location.href = '/login';
      }
    }
    return Promise.reject(error);
  }
);

// Auth endpoints
export const authAPI = {
  register: (data) => API.post('/auth/register', data),
  login: (data) => API.post('/auth/login', data),
  verifyOtp: (data) => API.post('/auth/verify-otp', data),
  resendOtp: (data) => API.post('/auth/resend-otp', data),
};

// User endpoints
export const userAPI = {
  getMe: () => API.get('/users/me'),
  getAll: () => API.get('/users'),
  getById: (id) => API.get(`/users/${id}`),
  assignRole: (id, roleId) => API.put(`/users/${id}/role`, { roleId }),
  updateStatus: (id, status) => API.put(`/users/${id}/status`, { status }),
};

// Restaurant endpoints
export const restaurantAPI = {
  getAll: () => API.get('/restaurants'),
  getById: (id) => API.get(`/restaurants/${id}`),
  create: (data) => API.post('/restaurants', data),
  update: (id, data) => API.put(`/restaurants/${id}`, data),
  delete: (id) => API.delete(`/restaurants/${id}`),
  assignManager: (id, managerId) => API.patch(`/restaurants/${id}/manager`, { managerId }),
  setStatus: (id, isActive) => API.patch(`/restaurants/${id}/status`, { isActive }),
  getByCity: (city) => API.get(`/restaurants/city/${city}`),
  getByState: (state) => API.get(`/restaurants/state/${state}`),
};

// Menu endpoints
export const menuAPI = {
  getByRestaurant: (restaurantId) => API.get(`/menu/restaurant/${restaurantId}`),
  getById: (id) => API.get(`/menu/${id}`),
  create: (data) => API.post('/menu', data),
  update: (id, data) => API.put(`/menu/${id}`, data),
  delete: (id) => API.delete(`/menu/${id}`),
  setAvailability: (id, isAvailable) => API.patch(`/menu/${id}/availability`, { isAvailable }),
  updatePrice: (id, newPrice) => API.patch(`/menu/${id}/price`, { newPrice }),
  search: (restaurantId, query) => API.get(`/menu/restaurant/${restaurantId}/search?query=${query}`),
};

// Order endpoints
export const orderAPI = {
  create: (data) => API.post('/orders', data),
  getMyOrders: () => API.get('/orders/my'),
  getById: (id) => API.get(`/orders/${id}`),
  getByRestaurant: (restaurantId) => API.get(`/orders/restaurant/${restaurantId}`),
  updateStatus: (id, status) => API.patch(`/orders/${id}/status`, { status }),
  updatePaymentStatus: (id, paymentStatus) => API.patch(`/orders/${id}/payment-status`, { paymentStatus }),
  setPaymentMethod: (id, paymentMethod) => API.patch(`/orders/${id}/payment-method`, { paymentMethod }),
  cancel: (id) => API.delete(`/orders/${id}`),
};

export default API;
