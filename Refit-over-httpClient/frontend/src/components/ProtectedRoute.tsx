import React from 'react';
import { withAuthenticationRequired } from '@auth0/auth0-react';

interface ProtectedRouteProps {
  children: React.ReactNode;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
  const Component = withAuthenticationRequired(() => <>{children}</>, {
    onRedirecting: () => (
      <div className="loading-container">
        <div className="spinner"></div>
      </div>
    ),
  });

  return <Component />;
};

export default ProtectedRoute;
