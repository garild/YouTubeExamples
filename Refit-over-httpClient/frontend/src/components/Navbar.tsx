import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth0 } from '@auth0/auth0-react';
import { LoginButton, LogoutButton } from './AuthButtons';
import { LayoutDashboard, Home } from 'lucide-react';

const Navbar: React.FC = () => {
  const { isAuthenticated, isLoading } = useAuth0();

  return (
    <nav className="navbar">
      <div className="navbar-brand">
        <span className="logo">WaylaApp</span>
      </div>
      <ul className="navbar-nav">
        <li className="nav-item">
          <Link to="/" className="nav-link">
            <Home size={18} />
            <span>Home</span>
          </Link>
        </li>
        {isAuthenticated && (
          <li className="nav-item">
            <Link to="/dashboard" className="nav-link">
              <LayoutDashboard size={18} />
              <span>Dashboard</span>
            </Link>
          </li>
        )}
      </ul>
      <div className="navbar-auth">
        {!isLoading && (
          isAuthenticated ? <LogoutButton /> : <LoginButton />
        )}
      </div>
    </nav>
  );
};

export default Navbar;
