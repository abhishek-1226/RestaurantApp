import { useState, useEffect } from 'react';
import { orderAPI } from '../api';

export default function OrdersPage() {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    fetchOrders();
  }, []);

  const fetchOrders = async () => {
    try {
      const res = await orderAPI.getMyOrders();
      setOrders(res.data);
    } catch (err) {
      setError('Failed to load orders.');
    } finally {
      setLoading(false);
    }
  };

  const handleCancel = async (orderId) => {
    if (!window.confirm('Are you sure you want to cancel this order?')) return;
    try {
      await orderAPI.cancel(orderId);
      fetchOrders();
    } catch (err) {
      alert(err.response?.data?.message || 'Failed to cancel order.');
    }
  };

  const getStatusColor = (status) => {
    const colors = {
      'Pending': '#f59e0b',
      'Confirmed': '#3b82f6',
      'Preparing': '#8b5cf6',
      'Out for Delivery': '#06b6d4',
      'Delivered': '#10b981',
      'Cancelled': '#ef4444',
    };
    return colors[status] || '#6b7280';
  };

  if (loading) return <div className="loading">Loading orders...</div>;

  return (
    <div className="page">
      <h1>📋 My Orders</h1>

      {error && <div className="alert alert-error">{error}</div>}

      {orders.length === 0 ? (
        <div className="empty-state">
          <h2>No orders yet</h2>
          <p>Place your first order from our restaurants.</p>
        </div>
      ) : (
        <div className="orders-list">
          {orders.map(order => (
            <div key={order.id} className="order-card">
              <div className="order-card-header">
                <h3>Order #{order.id}</h3>
                <span
                  className="order-status"
                  style={{ backgroundColor: getStatusColor(order.orderStatus) }}
                >
                  {order.orderStatus}
                </span>
              </div>

              <div className="order-details-grid">
                <div>
                  <p><strong>Payment:</strong> {order.paymentMethod} — <span className="payment-status">{order.paymentStatus}</span></p>
                  <p><strong>Delivery:</strong> {order.deliveryAddress}</p>
                  <p><strong>Contact:</strong> {order.contactNumber}</p>
                  <p><strong>Placed:</strong> {new Date(order.createdAt).toLocaleString()}</p>
                </div>
                <div className="order-pricing">
                  <p>Subtotal: ₹{order.subtotal?.toFixed(2)}</p>
                  <p>Tax: ₹{order.taxAmount?.toFixed(2)}</p>
                  {order.deliveryFee > 0 && <p>Delivery: ₹{order.deliveryFee?.toFixed(2)}</p>}
                  <p className="order-total"><strong>Total: ₹{order.totalAmount?.toFixed(2)}</strong></p>
                </div>
              </div>

              <div className="order-items">
                <h4>Items</h4>
                {order.orderItems?.map((item, idx) => (
                  <div key={idx} className="order-item-row">
                    <span>{item.itemName} × {item.quantity}</span>
                    <span>₹{item.totalPrice?.toFixed(2)}</span>
                  </div>
                ))}
              </div>

              {order.orderStatus === 'Pending' && (
                <div className="order-actions">
                  <button
                    onClick={() => handleCancel(order.id)}
                    className="btn btn-danger"
                  >
                    Cancel Order
                  </button>
                </div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
