import React from 'react';
import { useAuth0 } from '@auth0/auth0-react';
import { LogIn, LogOut } from 'lucide-react';

export const LoginButton: React.FC = () => {
  const { loginWithRedirect } = useAuth0();

  return (
    <button className="btn btn-primary" onClick={() => loginWithRedirect()}>
      <LogIn size={18} />
      <span>Log In</span>
    </button>
  );
};

export const LogoutButton: React.FC = () => {
  const { logout } = useAuth0();

  return (
    <button
      className="btn btn-secondary"
      onClick={() => logout({ logoutParams: { returnTo: window.location.origin } })}
    >
      <LogOut size={18} />
      <span>Log Out</span>
    </button>
  );
};
