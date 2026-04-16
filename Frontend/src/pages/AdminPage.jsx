import { useState, useEffect } from 'react';
import { restaurantAPI, menuAPI, orderAPI, userAPI } from '../api';

export default function AdminPage() {
  const [tab, setTab] = useState('restaurants');
  const [restaurants, setRestaurants] = useState([]);
  const [users, setUsers] = useState([]);
  const [orders, setOrders] = useState([]);
  const [menuItems, setMenuItems] = useState([]);
  const [selectedRestaurant, setSelectedRestaurant] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  // Restaurant form
  const [restForm, setRestForm] = useState({
    name: '', address: '', city: '', state: '', pinCode: '', phoneNumber: '', description: '', email: ''
  });

  // Menu item form
  const [menuForm, setMenuForm] = useState({
    name: '', price: '', description: '', restaurantId: ''
  });

  useEffect(() => {
    if (tab === 'restaurants') fetchRestaurants();
    if (tab === 'users') fetchUsers();
  }, [tab]);

  const fetchRestaurants = async () => {
    setLoading(true);
    try {
      const res = await restaurantAPI.getAll();
      setRestaurants(res.data);
    } catch { setError('Failed to load restaurants.'); }
    finally { setLoading(false); }
  };

  const fetchUsers = async () => {
    setLoading(true);
    try {
      const res = await userAPI.getAll();
      setUsers(res.data);
    } catch { setError('Failed to load users.'); }
    finally { setLoading(false); }
  };

  const fetchOrders = async (restaurantId) => {
    setLoading(true);
    try {
      const res = await orderAPI.getByRestaurant(restaurantId);
      setOrders(res.data);
    } catch { setError('Failed to load orders.'); }
    finally { setLoading(false); }
  };

  const fetchMenu = async (restaurantId) => {
    try {
      const res = await menuAPI.getByRestaurant(restaurantId);
      setMenuItems(res.data);
    } catch { setError('Failed to load menu.'); }
  };

  const handleCreateRestaurant = async (e) => {
    e.preventDefault();
    setError(''); setSuccess('');
    try {
      await restaurantAPI.create({ ...restForm, pinCode: parseInt(restForm.pinCode) });
      setSuccess('Restaurant created!');
      setRestForm({ name: '', address: '', city: '', state: '', pinCode: '', phoneNumber: '', description: '', email: '' });
      fetchRestaurants();
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to create restaurant.');
    }
  };

  const handleDeleteRestaurant = async (id) => {
    if (!window.confirm('Delete this restaurant?')) return;
    try {
      await restaurantAPI.delete(id);
      fetchRestaurants();
      setSuccess('Restaurant deleted.');
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to delete.');
    }
  };

  const handleAddMenuItem = async (e) => {
    e.preventDefault();
    setError(''); setSuccess('');
    try {
      await menuAPI.create({
        name: menuForm.name,
        price: parseFloat(menuForm.price),
        description: menuForm.description,
        restaurantId: parseInt(menuForm.restaurantId),
      });
      setSuccess('Menu item added!');
      setMenuForm({ name: '', price: '', description: '', restaurantId: menuForm.restaurantId });
      if (menuForm.restaurantId) fetchMenu(menuForm.restaurantId);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to add menu item.');
    }
  };

  const handleDeleteMenuItem = async (id) => {
    if (!window.confirm('Delete this menu item?')) return;
    try {
      await menuAPI.delete(id);
      if (menuForm.restaurantId) fetchMenu(menuForm.restaurantId);
      setSuccess('Menu item deleted.');
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to delete.');
    }
  };

  const handleToggleAvailability = async (id, current) => {
    try {
      await menuAPI.setAvailability(id, !current);
      if (menuForm.restaurantId) fetchMenu(menuForm.restaurantId);
    } catch (err) {
      setError('Failed to update availability.');
    }
  };

  const handleUpdateOrderStatus = async (orderId, status) => {
    try {
      await orderAPI.updateStatus(orderId, status);
      if (selectedRestaurant) fetchOrders(selectedRestaurant);
      setSuccess('Order status updated.');
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to update.');
    }
  };

  const handleUpdatePaymentStatus = async (orderId, paymentStatus) => {
    try {
      await orderAPI.updatePaymentStatus(orderId, paymentStatus);
      if (selectedRestaurant) fetchOrders(selectedRestaurant);
      setSuccess('Payment status updated.');
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to update.');
    }
  };

  const handleRoleChange = async (userId, roleId) => {
    try {
      await userAPI.assignRole(userId, roleId);
      fetchUsers();
      setSuccess('Role updated.');
    } catch (err) {
      setError('Failed to update role.');
    }
  };

  const handleStatusChange = async (userId, status) => {
    try {
      await userAPI.updateStatus(userId, status);
      fetchUsers();
      setSuccess('User status updated.');
    } catch (err) {
      setError('Failed to update status.');
    }
  };

  const clearMessages = () => { setError(''); setSuccess(''); };

  return (
    <div className="page">
      <h1>⚙️ Admin Dashboard</h1>

      {error && <div className="alert alert-error">{error} <button onClick={clearMessages}>✕</button></div>}
      {success && <div className="alert alert-success">{success} <button onClick={clearMessages}>✕</button></div>}

      <div className="admin-tabs">
        <button className={`tab ${tab === 'restaurants' ? 'active' : ''}`} onClick={() => setTab('restaurants')}>Restaurants</button>
        <button className={`tab ${tab === 'menu' ? 'active' : ''}`} onClick={() => setTab('menu')}>Menu Items</button>
        <button className={`tab ${tab === 'orders' ? 'active' : ''}`} onClick={() => setTab('orders')}>Orders</button>
        <button className={`tab ${tab === 'users' ? 'active' : ''}`} onClick={() => setTab('users')}>Users</button>
      </div>

      {/* RESTAURANTS TAB */}
      {tab === 'restaurants' && (
        <div className="admin-section">
          <h2>Create Restaurant</h2>
          <form onSubmit={handleCreateRestaurant} className="admin-form">
            <div className="form-row">
              <input placeholder="Name" value={restForm.name} onChange={e => setRestForm({...restForm, name: e.target.value})} required />
              <input placeholder="Phone" value={restForm.phoneNumber} onChange={e => setRestForm({...restForm, phoneNumber: e.target.value})} required />
            </div>
            <input placeholder="Address" value={restForm.address} onChange={e => setRestForm({...restForm, address: e.target.value})} required />
            <div className="form-row">
              <input placeholder="City" value={restForm.city} onChange={e => setRestForm({...restForm, city: e.target.value})} required />
              <input placeholder="State" value={restForm.state} onChange={e => setRestForm({...restForm, state: e.target.value})} required />
              <input placeholder="PIN Code" value={restForm.pinCode} onChange={e => setRestForm({...restForm, pinCode: e.target.value})} required />
            </div>
            <div className="form-row">
              <input placeholder="Email" type="email" value={restForm.email} onChange={e => setRestForm({...restForm, email: e.target.value})} />
              <input placeholder="Description" value={restForm.description} onChange={e => setRestForm({...restForm, description: e.target.value})} />
            </div>
            <button type="submit" className="btn btn-primary">Create Restaurant</button>
          </form>

          <h2 style={{marginTop: '2rem'}}>All Restaurants ({restaurants.length})</h2>
          <div className="admin-table-wrapper">
            <table className="admin-table">
              <thead>
                <tr><th>ID</th><th>Name</th><th>City</th><th>State</th><th>Phone</th><th>Active</th><th>Actions</th></tr>
              </thead>
              <tbody>
                {restaurants.map(r => (
                  <tr key={r.id}>
                    <td>{r.id}</td>
                    <td>{r.name}</td>
                    <td>{r.city}</td>
                    <td>{r.state}</td>
                    <td>{r.phoneNumber}</td>
                    <td>{r.isActive ? '✅' : '❌'}</td>
                    <td>
                      <button className="btn btn-sm btn-danger" onClick={() => handleDeleteRestaurant(r.id)}>Delete</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* MENU TAB */}
      {tab === 'menu' && (
        <div className="admin-section">
          <h2>Manage Menu Items</h2>
          <div className="form-group">
            <label>Select Restaurant</label>
            <select
              value={menuForm.restaurantId}
              onChange={(e) => {
                setMenuForm({...menuForm, restaurantId: e.target.value});
                if (e.target.value) fetchMenu(e.target.value);
              }}
            >
              <option value="">-- Select --</option>
              {restaurants.map(r => <option key={r.id} value={r.id}>{r.name}</option>)}
            </select>
          </div>

          {menuForm.restaurantId && (
            <>
              <h3>Add Menu Item</h3>
              <form onSubmit={handleAddMenuItem} className="admin-form">
                <div className="form-row">
                  <input placeholder="Item Name" value={menuForm.name} onChange={e => setMenuForm({...menuForm, name: e.target.value})} required />
                  <input placeholder="Price" type="number" step="0.01" value={menuForm.price} onChange={e => setMenuForm({...menuForm, price: e.target.value})} required />
                </div>
                <input placeholder="Description" value={menuForm.description} onChange={e => setMenuForm({...menuForm, description: e.target.value})} />
                <button type="submit" className="btn btn-primary">Add Item</button>
              </form>

              <h3 style={{marginTop: '2rem'}}>Current Menu ({menuItems.length} items)</h3>
              <div className="admin-table-wrapper">
                <table className="admin-table">
                  <thead>
                    <tr><th>ID</th><th>Name</th><th>Price</th><th>Available</th><th>Actions</th></tr>
                  </thead>
                  <tbody>
                    {menuItems.map(item => (
                      <tr key={item.id}>
                        <td>{item.id}</td>
                        <td>{item.name}</td>
                        <td>₹{item.price.toFixed(2)}</td>
                        <td>
                          <button
                            className={`btn btn-sm ${item.isAvailable ? 'btn-success' : 'btn-outline'}`}
                            onClick={() => handleToggleAvailability(item.id, item.isAvailable)}
                          >
                            {item.isAvailable ? 'Available' : 'Unavailable'}
                          </button>
                        </td>
                        <td>
                          <button className="btn btn-sm btn-danger" onClick={() => handleDeleteMenuItem(item.id)}>Delete</button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </>
          )}
        </div>
      )}

      {/* ORDERS TAB */}
      {tab === 'orders' && (
        <div className="admin-section">
          <h2>Manage Orders</h2>
          <div className="form-group">
            <label>Select Restaurant</label>
            <select
              value={selectedRestaurant || ''}
              onChange={(e) => {
                setSelectedRestaurant(e.target.value);
                if (e.target.value) fetchOrders(e.target.value);
              }}
            >
              <option value="">-- Select --</option>
              {restaurants.map(r => <option key={r.id} value={r.id}>{r.name}</option>)}
            </select>
          </div>

          {orders.length > 0 && (
            <div className="admin-table-wrapper">
              <table className="admin-table">
                <thead>
                  <tr><th>Order #</th><th>Items</th><th>Total</th><th>Status</th><th>Payment</th><th>Actions</th></tr>
                </thead>
                <tbody>
                  {orders.map(order => (
                    <tr key={order.id}>
                      <td>#{order.id}</td>
                      <td>{order.orderItems?.map(i => `${i.itemName}(${i.quantity})`).join(', ')}</td>
                      <td>₹{order.totalAmount?.toFixed(2)}</td>
                      <td>
                        <select
                          value={order.orderStatus}
                          onChange={(e) => handleUpdateOrderStatus(order.id, e.target.value)}
                        >
                          <option value="Pending">Pending</option>
                          <option value="Confirmed">Confirmed</option>
                          <option value="Preparing">Preparing</option>
                          <option value="Out for Delivery">Out for Delivery</option>
                          <option value="Delivered">Delivered</option>
                          <option value="Cancelled">Cancelled</option>
                        </select>
                      </td>
                      <td>
                        <select
                          value={order.paymentStatus}
                          onChange={(e) => handleUpdatePaymentStatus(order.id, e.target.value)}
                        >
                          <option value="Pending">Pending</option>
                          <option value="Paid">Paid</option>
                          <option value="Refunded">Refunded</option>
                        </select>
                      </td>
                      <td>{new Date(order.createdAt).toLocaleDateString()}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {/* USERS TAB */}
      {tab === 'users' && (
        <div className="admin-section">
          <h2>Manage Users ({users.length})</h2>
          <div className="admin-table-wrapper">
            <table className="admin-table">
              <thead>
                <tr><th>ID</th><th>Name</th><th>Email</th><th>Role</th><th>Status</th><th>Verified</th><th>Actions</th></tr>
              </thead>
              <tbody>
                {users.map(user => (
                  <tr key={user.id}>
                    <td>{user.id}</td>
                    <td>{user.name}</td>
                    <td>{user.email}</td>
                    <td>
                      <select value={
                        user.roleName === 'Admin' ? 1 : user.roleName === 'Manager' ? 2 : user.roleName === 'Operator' ? 3 : 4
                      } onChange={(e) => handleRoleChange(user.id, parseInt(e.target.value))}>
                        <option value={1}>Admin</option>
                        <option value={2}>Manager</option>
                        <option value={3}>Operator</option>
                        <option value={4}>Customer</option>
                      </select>
                    </td>
                    <td>
                      <select value={user.status} onChange={(e) => handleStatusChange(user.id, e.target.value)}>
                        <option value="Active">Active</option>
                        <option value="Inactive">Inactive</option>
                        <option value="Banned">Banned</option>
                      </select>
                    </td>
                    <td>{user.isVerified ? '✅' : '❌'}</td>
                    <td>—</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
}
