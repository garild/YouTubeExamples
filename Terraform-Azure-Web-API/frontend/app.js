// Configuration options for Auth0 and Web API
// domain must be the bare Auth0 host (no https://). Match Products.Api Auth0:Domain.
const auth0Config = {
  domain: "dev-youtube-gt.eu.auth0.com",
  clientId: "5UTKivQaqWf1lbpiX5DRQchn13v2aotb", // Auth0 SPA Client ID (AUTH0_UI_CLIENT_ID)
  audience: "https://api.products.local"
};

// API Base URL (updates dynamically if deployed in App Service or points to localhost)
const apiBaseUrl = window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1'
  ? "http://localhost:62254"  // Products.Api launchSettings HTTP port
  : "https://app-yt-examples-api-dev.azurewebsites.net"; // Default provisioned backend host

let auth0Client = null;
// Cache of the last successful catalog fetch so search can filter without re-hitting the API
let allProducts = [];

// Initialize the application and client sdk
window.onload = async () => {
  setupEventHandlers();

  try {
    // Auth0 SPA JS v2 CDN exposes createAuth0Client on the global `auth0` object.
    // Options use camelCase (clientId), not the v1 snake_case names.
    auth0Client = await auth0.createAuth0Client({
      domain: auth0Config.domain,
      clientId: auth0Config.clientId,
      authorizationParams: {
        audience: auth0Config.audience,
        redirect_uri: window.location.origin
      },
      cacheLocation: 'localstorage',
      useRefreshTokens: true
    });

    // Check if redirect callback needs processing
    const query = window.location.search;
    if (query.includes("code=") && query.includes("state=")) {
      await auth0Client.handleRedirectCallback();
      window.history.replaceState({}, document.title, "/");
    }

    const isAuthenticated = await auth0Client.isAuthenticated();
    await updateUIState(isAuthenticated);

    await fetchProducts();

  } catch (error) {
    console.error("Initialization failure:", error);
    // Keep Sign In visible even when Auth0 fails to initialize
    document.getElementById("btn-login").classList.remove("hidden");
    document.getElementById("anon-alert").classList.remove("hidden");
    showError("Could not initialize authentication client. Check auth0Config (domain without https://, SPA clientId, audience).");
    await fetchProducts();
  }
};

// Bind HTML events to DOM actions
function setupEventHandlers() {
  document.getElementById("btn-login").onclick = login;
  document.getElementById("btn-logout").onclick = logout;
  document.getElementById("btn-add-product").onclick = () => toggleModal(true);
  document.getElementById("product-form").onsubmit = submitProduct;
  document.getElementById("search-input").addEventListener("input", (e) => renderProducts(e.target.value));
}

// Returns a bearer access token when the user is authenticated, otherwise null.
// The GET catalog endpoint is public, so we attach the token only when available.
async function getAccessTokenOrNull() {
  try {
    if (auth0Client && (await auth0Client.isAuthenticated())) {
      return await auth0Client.getTokenSilently();
    }
  } catch (error) {
    console.warn("Could not acquire access token silently:", error);
  }
  return null;
}

// Trigger Auth0 Login Redirection
async function login() {
  if (!auth0Client) {
    showError("Auth0 client is not ready. Check domain (no https://), clientId, and audience in app.js.");
    return;
  }
  try {
    await auth0Client.loginWithRedirect();
  } catch (error) {
    console.error("Login redirect failure:", error);
    showError("Auth0 login redirection failed. Confirm Allowed Callback / Logout / Web Origins include this origin.");
  }
}

// Terminate Session and Log Out
function logout() {
  if (!auth0Client) {
    return;
  }
  auth0Client.logout({
    logoutParams: {
      returnTo: window.location.origin
    }
  });
}

// Alter client UI display state based on Authentication
async function updateUIState(isAuthenticated) {
  const loginBtn = document.getElementById("btn-login");
  const userPanel = document.getElementById("user-panel");
  const addProductBtn = document.getElementById("btn-add-product");
  const anonAlert = document.getElementById("anon-alert");

  if (isAuthenticated) {
    loginBtn.classList.add("hidden");
    userPanel.classList.remove("hidden");
    addProductBtn.classList.remove("hidden");
    anonAlert.classList.add("hidden");

    // Fetch user details from claims
    const user = await auth0Client.getUser();
    document.getElementById("user-name").textContent = user.name || user.nickname;
    document.getElementById("user-email").textContent = user.email || "";
    document.getElementById("user-avatar").src = user.picture || "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=80&h=80&fit=crop&crop=faces";
  } else {
    loginBtn.classList.remove("hidden");
    userPanel.classList.add("hidden");
    addProductBtn.classList.add("hidden");
    anonAlert.classList.remove("hidden");
  }
}

// Fetch products from the Web API (sends the bearer token when signed in)
async function fetchProducts() {
  const loadingSkeleton = document.getElementById("catalog-loading");
  const emptyState = document.getElementById("catalog-empty");
  const gridContainer = document.getElementById("catalog-grid");

  loadingSkeleton.classList.remove("hidden");
  emptyState.classList.add("hidden");
  gridContainer.innerHTML = "";

  try {
    const headers = { "Accept": "application/json" };
    const token = await getAccessTokenOrNull();
    if (token) {
      headers["Authorization"] = `Bearer ${token}`;
    }

    const response = await fetch(`${apiBaseUrl}/api/products`, { headers });

    if (!response.ok) {
      throw new Error(`API returned HTTP status ${response.status}`);
    }

    allProducts = await response.json();
    loadingSkeleton.classList.add("hidden");
    renderProducts(document.getElementById("search-input").value);

  } catch (error) {
    console.error("Failed to load catalog products:", error);
    loadingSkeleton.classList.add("hidden");
    showError("Could not retrieve catalog data from Web API server.");
  }
}

// Render (and optionally filter) the cached products into the grid
function renderProducts(searchTerm = "") {
  const gridContainer = document.getElementById("catalog-grid");
  const emptyState = document.getElementById("catalog-empty");
  const countBadge = document.getElementById("product-count");

  const term = searchTerm.trim().toLowerCase();
  const filtered = term
    ? allProducts.filter(p =>
        (p.name || "").toLowerCase().includes(term) ||
        (p.sku || "").toLowerCase().includes(term))
    : allProducts;

  countBadge.textContent = `${filtered.length} item${filtered.length === 1 ? "" : "s"}`;
  countBadge.classList.remove("hidden");

  gridContainer.innerHTML = "";

  if (filtered.length === 0) {
    emptyState.classList.remove("hidden");
    return;
  }
  emptyState.classList.add("hidden");

  filtered.forEach((product, index) => {
    const formattedPrice = new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(product.price);

    const createdDate = new Date(product.createdAt).toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric'
    });

    const initials = (product.name || "?").trim().charAt(0).toUpperCase();

    const card = document.createElement("div");
    card.className = "group relative rounded-3xl border border-white/5 bg-gradient-to-br from-slate-900/70 to-slate-900/30 p-6 backdrop-blur-xl hover:border-indigo-500/40 transition-all shadow-md hover:shadow-xl hover:shadow-indigo-500/10 hover:-translate-y-1 flex flex-col justify-between animate-fade-in-up";
    card.style.animationDelay = `${Math.min(index * 60, 480)}ms`;
    card.innerHTML = `
      <div>
        <div class="flex items-center justify-between gap-2">
          <div class="flex items-center gap-3">
            <div class="flex h-10 w-10 items-center justify-center rounded-xl bg-gradient-to-tr from-indigo-500/20 to-violet-500/20 text-indigo-300 font-bold border border-white/10">${initials}</div>
            <span class="inline-flex items-center rounded-md bg-indigo-400/10 px-2 py-1 text-xs font-medium text-indigo-300 ring-1 ring-inset ring-indigo-400/20">${escapeHtml(product.sku)}</span>
          </div>
          <span class="text-xs text-slate-500">${createdDate}</span>
        </div>
        <h3 class="mt-4 text-lg font-semibold text-white leading-6">${escapeHtml(product.name)}</h3>
      </div>
      <div class="mt-6 flex items-center justify-between border-t border-white/5 pt-4">
        <span class="text-sm text-slate-400">Price</span>
        <span class="text-xl font-bold bg-gradient-to-r from-white to-slate-300 bg-clip-text text-transparent">${formattedPrice}</span>
      </div>
    `;
    gridContainer.appendChild(card);
  });
}

// Basic HTML escaping to keep user-provided strings safe in innerHTML
function escapeHtml(value) {
  return String(value ?? "").replace(/[&<>"']/g, (ch) => ({
    "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;"
  }[ch]));
}

// Create Product Form Submission
async function submitProduct(event) {
  event.preventDefault();
  closeValidationErrors();

  const nameInput = document.getElementById("input-name").value;
  const priceInput = parseFloat(document.getElementById("input-price").value);
  const skuInput = document.getElementById("input-sku").value;

  const payload = {
    name: nameInput,
    price: priceInput,
    sku: skuInput
  };

  try {
    const isAuthenticated = await auth0Client.isAuthenticated();
    if (!isAuthenticated) {
      throw new Error("You must log in to submit products.");
    }

    // Retrieve active access token silently
    const token = await auth0Client.getTokenSilently();
    
    const submitBtn = document.getElementById("btn-submit-product");
    submitBtn.disabled = true;
    submitBtn.textContent = "Saving...";

    const response = await fetch(`${apiBaseUrl}/api/products`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
      },
      body: JSON.stringify(payload)
    });

    submitBtn.disabled = false;
    submitBtn.textContent = "Save Product";

    if (!response.ok) {
      if (response.status === 400) {
        const errorData = await response.json();
        if (errorData.errors) {
          showFormValidationError(errorData.errors);
          return;
        }
      }
      throw new Error(`Server returned status code ${response.status}`);
    }

    // Clear form inputs
    document.getElementById("product-form").reset();
    toggleModal(false);
    
    // Refresh product list grid
    await fetchProducts();

  } catch (error) {
    console.error("Submission failed:", error);
    showError(`Product creation failed: ${error.message}`);
  }
}

// Modal open/close state toggler
function toggleModal(show) {
  const modal = document.getElementById("product-modal");
  if (show) {
    modal.classList.remove("hidden");
  } else {
    modal.classList.add("hidden");
    closeValidationErrors();
  }
}

// Form validation error displayer
function showFormValidationError(errors) {
  const errorContainer = document.getElementById("modal-validation-error");
  errorContainer.innerHTML = "";
  
  const list = document.createElement("ul");
  list.className = "list-disc pl-4 space-y-1";
  
  Object.keys(errors).forEach(key => {
    errors[key].forEach(errMessage => {
      const item = document.createElement("li");
      item.textContent = errMessage;
      list.appendChild(item);
    });
  });
  
  errorContainer.appendChild(list);
  errorContainer.classList.remove("hidden");
}

function closeValidationErrors() {
  document.getElementById("modal-validation-error").classList.add("hidden");
}

// Toast banner error alerts
function showError(message) {
  const toast = document.getElementById("error-toast");
  document.getElementById("error-message").textContent = message;
  toast.classList.remove("hidden");
  
  // Auto-close after 8 seconds
  setTimeout(() => {
    closeError();
  }, 8000);
}

function closeError() {
  document.getElementById("error-toast").classList.add("hidden");
}
