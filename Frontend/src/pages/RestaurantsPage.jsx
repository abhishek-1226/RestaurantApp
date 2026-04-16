import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { restaurantAPI } from '../api';

export default function RestaurantsPage() {
  const [restaurants, setRestaurants] = useState([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    fetchRestaurants();
  }, []);

  const fetchRestaurants = async () => {
    try {
      const res = await restaurantAPI.getAll();
      setRestaurants(res.data);
    } catch (err) {
      setError('Failed to load restaurants.');
    } finally {
      setLoading(false);
    }
  };

  const filtered = restaurants.filter(r =>
    r.name.toLowerCase().includes(search.toLowerCase()) ||
    r.city.toLowerCase().includes(search.toLowerCase()) ||
    r.state.toLowerCase().includes(search.toLowerCase())
  );

  if (loading) return <div className="loading">Loading restaurants...</div>;

  return (
    <div className="page">
      <div className="page-header">
        <h1>🍽️ Restaurants</h1>
        <p>Discover and order from the best restaurants near you</p>
      </div>

      <div className="search-bar">
        <input
          type="text"
          placeholder="Search by name, city, or state..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="search-input"
        />
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      {filtered.length === 0 ? (
        <div className="empty-state">
          <p>No restaurants found.</p>
        </div>
      ) : (
        <div className="card-grid">
          {filtered.map(restaurant => (
            <Link to={`/restaurant/${restaurant.id}`} key={restaurant.id} className="restaurant-card">
              <div className="restaurant-card-header">
                <h3>{restaurant.name}</h3>
                <span className={`status-badge ${restaurant.isActive ? 'active' : 'inactive'}`}>
                  {restaurant.isActive ? 'Open' : 'Closed'}
                </span>
              </div>
              <p className="restaurant-description">{restaurant.description || 'No description available'}</p>
              <div className="restaurant-meta">
                <span>📍 {restaurant.city}, {restaurant.state}</span>
                <span>📞 {restaurant.phoneNumber}</span>
              </div>
              <div className="restaurant-meta">
                <span>📧 {restaurant.email || 'N/A'}</span>
                <span>📮 {restaurant.pinCode}</span>
              </div>
              {restaurant.menuItems && (
                <div className="restaurant-items-count">
                  {restaurant.menuItems.length} items on menu
                </div>
              )}
            </Link>
          ))}
        </div>
      )}
    </div>
  );
}
