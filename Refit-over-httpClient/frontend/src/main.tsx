import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { Auth0Provider } from '@auth0/auth0-react';
import { BrowserRouter } from 'react-router-dom';
import App from './App.tsx';
import './index.css';

// Ideally, these should come from environment variables.
// Using placeholders if env vars are not set.
const domain = import.meta.env.VITE_AUTH0_DOMAIN || "YOUR_AUTH0_DOMAIN";
const clientId = import.meta.env.VITE_AUTH0_CLIENT_ID || "YOUR_AUTH0_CLIENT_ID";
const callbackUrl = import.meta.env.VITE_AUTH0_CALLBACK_URL || window.location.origin;
const audience = import.meta.env.VITE_AUTH0_AUDIENCE;

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <Auth0Provider
      domain={domain}
      clientId={clientId}
      cacheLocation="localstorage"
      authorizationParams={{
        redirect_uri: callbackUrl,
        ...(audience && audience !== "YOUR_AUTH0_API_IDENTIFIER" ? { audience } : {})
      }}
    >
      <BrowserRouter>
        <App />
      </BrowserRouter>
    </Auth0Provider>
  </StrictMode>,
);
