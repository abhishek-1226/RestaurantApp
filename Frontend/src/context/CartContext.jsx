import { createContext, useContext, useState } from 'react';

const CartContext = createContext(null);

export function CartProvider({ children }) {
  const [cartItems, setCartItems] = useState([]);
  const [restaurantId, setRestaurantId] = useState(null);
  const [restaurantName, setRestaurantName] = useState('');

  const addToCart = (item, restId, restName) => {
    // If adding from a different restaurant, clear cart first
    if (restaurantId && restaurantId !== restId) {
      if (!window.confirm('Adding items from a different restaurant will clear your current cart. Continue?')) {
        return;
      }
      setCartItems([]);
    }

    setRestaurantId(restId);
    setRestaurantName(restName);

    setCartItems(prev => {
      const existing = prev.find(ci => ci.menuItemId === item.id);
      if (existing) {
        return prev.map(ci =>
          ci.menuItemId === item.id
            ? { ...ci, quantity: ci.quantity + 1 }
            : ci
        );
      }
      return [...prev, {
        menuItemId: item.id,
        name: item.name,
        price: item.price,
        quantity: 1,
        specialInstructions: ''
      }];
    });
  };

  const removeFromCart = (menuItemId) => {
    setCartItems(prev => prev.filter(ci => ci.menuItemId !== menuItemId));
  };

  const updateQuantity = (menuItemId, quantity) => {
    if (quantity <= 0) {
      removeFromCart(menuItemId);
      return;
    }
    setCartItems(prev =>
      prev.map(ci => ci.menuItemId === menuItemId ? { ...ci, quantity } : ci)
    );
  };

  const updateInstructions = (menuItemId, specialInstructions) => {
    setCartItems(prev =>
      prev.map(ci => ci.menuItemId === menuItemId ? { ...ci, specialInstructions } : ci)
    );
  };

  const clearCart = () => {
    setCartItems([]);
    setRestaurantId(null);
    setRestaurantName('');
  };

  const getTotal = () => {
    return cartItems.reduce((sum, item) => sum + item.price * item.quantity, 0);
  };

  const getItemCount = () => {
    return cartItems.reduce((sum, item) => sum + item.quantity, 0);
  };

  return (
    <CartContext.Provider value={{
      cartItems, restaurantId, restaurantName,
      addToCart, removeFromCart, updateQuantity, updateInstructions,
      clearCart, getTotal, getItemCount
    }}>
      {children}
    </CartContext.Provider>
  );
}

export function useCart() {
  const context = useContext(CartContext);
  if (!context) {
    throw new Error('useCart must be used within a CartProvider');
  }
  return context;
}
