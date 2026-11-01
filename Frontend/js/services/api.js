// js/services/api.js

const BASE_URL = 'https://localhost:8000/api';

/**
 * Log out user by clearing storage and redirecting to the login page.
 */
export function logout() {
    localStorage.removeItem('authToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('userID');

    sessionStorage.removeItem('authToken');
    sessionStorage.removeItem('refreshToken');
    sessionStorage.removeItem('userID');

    if (!window.location.pathname.includes('login.html')) {
        window.location.href = '/pages/login.html';
    }
}

/**
 * Determine active storage (localStorage or sessionStorage) where tokens are stored.
 */
function getActiveStorage() {
    if (localStorage.getItem('authToken') || localStorage.getItem('refreshToken')) {
        return localStorage;
    }
    if (sessionStorage.getItem('authToken') || sessionStorage.getItem('refreshToken')) {
        return sessionStorage;
    }
    return localStorage; // Default fallback
}

/**
 * Handle refreshing the expired JWT token silently in the background.
 */
async function handleRefreshToken() {
    const storage = getActiveStorage();
    const refreshToken = storage.getItem('refreshToken');
    const userID = storage.getItem('userID');

    if (!refreshToken || !userID) {
        return false;
    }

    try {
        const response = await fetch(`${BASE_URL}/Auth/refresh`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                userID: parseInt(userID, 10),
                refreshToken: refreshToken
            })
        });

        if (response.ok) {
            const data = await response.json();
            
            // Flexible token retrieval matching API DTO
            const newToken = data.token || data.accessToken;
            const newRefreshToken = data.refreshToken || refreshToken;

            if (newToken) {
                storage.setItem('authToken', newToken);
            }
            if (newRefreshToken) {
                storage.setItem('refreshToken', newRefreshToken);
            }

            return true;
        } else {
            return false;
        }
    } catch (error) {
        console.error('Error refreshing token:', error);
        return false;
    }
}

/**
 * Centralized Fetch API Wrapper with auto JWT attachment & silent token refresh.
 * 
 * @param {string} endpoint - Path relative to BASE_URL (e.g., '/properties' or '/favorites/toggle')
 * @param {RequestInit} options - Standard Fetch parameters
 * @param {boolean} isRetry - Internal flag to prevent infinite loops on repeated 401s
 */
export async function request(endpoint, options = {}, isRetry = false) {
    const storage = getActiveStorage();
    const token = storage.getItem('authToken');

    const headers = {
        'Content-Type': 'application/json',
        ...(token ? { 'Authorization': `Bearer ${token}` } : {}),
        ...options.headers,
    };

    const config = { ...options, headers };

    // Format endpoint URL cleanly (prevents redundant / or api/ prefixes)
    let url = endpoint;
    if (!endpoint.startsWith('http')) {
        const cleanEndpoint = endpoint.replace(/^\/?(api\/)?/, '');
        url = `${BASE_URL}/${cleanEndpoint}`;
    }

    try {
        const response = await fetch(url, config);

        // Handle Token Expiration (401 Unauthorized)
        if (response.status === 401) {
            const isAuthEndpoint = endpoint.includes('/Auth/login') || 
                                   endpoint.includes('/Auth/refresh') || 
                                   endpoint.includes('/login');

            if (!isAuthEndpoint && !isRetry) {
                const refreshSuccess = await handleRefreshToken();

                if (refreshSuccess) {
                    // Retry original request with the newly acquired token
                    return await request(endpoint, options, true);
                } else {
                    logout();
                    throw new Error('Session expired. Please log in again.');
                }
            }
        }

        // Parse JSON safely (prevents crashes on empty 204 No Content responses)
        const data = await response.json().catch(() => ({}));

        if (!response.ok) {
            throw new Error(data.message || data.error || 'An error occurred during the request.');
        }

        return data;

    } catch (error) {
        console.error(`API Request Error [${endpoint}]:`, error);
        throw error;
    }
}