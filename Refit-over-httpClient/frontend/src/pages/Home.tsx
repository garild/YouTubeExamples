import React from 'react';
import { useAuth0 } from '@auth0/auth0-react';
import { LoginButton } from '../components/AuthButtons';
import { ArrowRight } from 'lucide-react';
import { Link } from 'react-router-dom';

const Home: React.FC = () => {
  const { isAuthenticated, isLoading } = useAuth0();

  return (
    <div className="home-container fade-in">
      <div className="hero-section">
        <h1 className="hero-title">
          Welcome to <span className="highlight">WaylaApp</span>
        </h1>
        <p className="hero-subtitle">
          Your modern, secure, and blazingly fast application powered by React and Auth0.
        </p>
        <div className="hero-actions">
          {isLoading ? (
            <div className="spinner"></div>
          ) : isAuthenticated ? (
            <Link to="/dashboard" className="btn btn-primary btn-large">
              <span>Go to Dashboard</span>
              <ArrowRight size={20} />
            </Link>
          ) : (
            <LoginButton />
          )}
        </div>
      </div>
    </div>
  );
};

export default Home;
