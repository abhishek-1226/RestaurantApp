import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { useCart } from '../context/CartContext';

export default function Navbar() {
  const { user, logout, isAuthenticated, isAdmin } = useAuth();
  const { getItemCount } = useCart();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav className="navbar">
      <div className="navbar-container">
        <Link to="/" className="navbar-brand">
          🍽️ RestaurantApp
        </Link>

        <div className="navbar-links">
          <Link to="/" className="nav-link">Restaurants</Link>

          {isAuthenticated() ? (
            <>
              <Link to="/orders" className="nav-link">My Orders</Link>
              <Link to="/cart" className="nav-link cart-link">
                🛒 Cart
                {getItemCount() > 0 && (
                  <span className="cart-badge">{getItemCount()}</span>
                )}
              </Link>
              {isAdmin() && (
                <Link to="/admin" className="nav-link admin-link">Admin</Link>
              )}
              <div className="user-menu">
                <span className="user-name">👤 {user?.name}</span>
                <span className="user-role">({user?.role})</span>
                <button onClick={handleLogout} className="btn btn-logout">Logout</button>
              </div>
            </>
          ) : (
            <>
              <Link to="/login" className="nav-link">Login</Link>
              <Link to="/register" className="nav-link btn btn-primary">Register</Link>
            </>
          )}
        </div>
      </div>
    </nav>
  );
}
