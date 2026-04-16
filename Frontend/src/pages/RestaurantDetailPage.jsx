import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { restaurantAPI, menuAPI } from '../api';
import { useCart } from '../context/CartContext';
import { useAuth } from '../context/AuthContext';

export default function RestaurantDetailPage() {
  const { id } = useParams();
  const [restaurant, setRestaurant] = useState(null);
  const [menuItems, setMenuItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const { addToCart } = useCart();
  const { isAuthenticated } = useAuth();

  useEffect(() => {
    fetchData();
  }, [id]);

  const fetchData = async () => {
    try {
      const [restRes, menuRes] = await Promise.all([
        restaurantAPI.getById(id),
        menuAPI.getByRestaurant(id),
      ]);
      setRestaurant(restRes.data);
      setMenuItems(menuRes.data);
    } catch (err) {
      setError('Failed to load restaurant details.');
    } finally {
      setLoading(false);
    }
  };

  const handleAddToCart = (item) => {
    if (!isAuthenticated()) {
      alert('Please login to add items to cart.');
      return;
    }
    addToCart(item, restaurant.id, restaurant.name);
  };

  if (loading) return <div className="loading">Loading restaurant...</div>;
  if (error) return <div className="alert alert-error">{error}</div>;
  if (!restaurant) return <div className="empty-state">Restaurant not found.</div>;

  return (
    <div className="page">
      <Link to="/" className="back-link">← Back to Restaurants</Link>

      <div className="restaurant-detail-header">
        <div>
          <h1>{restaurant.name}</h1>
          <p className="restaurant-description">{restaurant.description}</p>
          <div className="restaurant-meta">
            <span>📍 {restaurant.address}, {restaurant.city}, {restaurant.state} - {restaurant.pinCode}</span>
          </div>
          <div className="restaurant-meta">
            <span>📞 {restaurant.phoneNumber}</span>
            {restaurant.email && <span>📧 {restaurant.email}</span>}
          </div>
        </div>
        <span className={`status-badge large ${restaurant.isActive ? 'active' : 'inactive'}`}>
          {restaurant.isActive ? 'Open' : 'Closed'}
        </span>
      </div>

      <h2 className="section-title">Menu ({menuItems.length} items)</h2>

      {menuItems.length === 0 ? (
        <div className="empty-state">
          <p>No menu items available yet.</p>
        </div>
      ) : (
        <div className="menu-grid">
          {menuItems.map(item => (
            <div key={item.id} className={`menu-item-card ${!item.isAvailable ? 'unavailable' : ''}`}>
              <div className="menu-item-info">
                <h3>{item.name}</h3>
                <p className="menu-item-desc">{item.description || 'No description'}</p>
                <div className="menu-item-price">₹{item.price.toFixed(2)}</div>
              </div>
              <div className="menu-item-actions">
                {item.isAvailable ? (
                  <button
                    onClick={() => handleAddToCart(item)}
                    className="btn btn-add-cart"
                  >
                    + Add
                  </button>
                ) : (
                  <span className="unavailable-text">Unavailable</span>
                )}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
