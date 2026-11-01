// js/services/auth.js

import { request, logout as apiLogout } from './api.js';

/**
 * Authenticates user credentials and stores tokens/userID in storage.
 * @param {Object} credentials - { email, password, remember }
 */
export async function loginUser(credentials) {
    const data = await request('/Auth/login', {
        method: 'POST',
        body: JSON.stringify({
            email: credentials.email,
            password: credentials.password
        }),
    });

    const accessToken = data.accessToken || data.AccessToken || data.token;
    const refreshToken = data.refreshToken || data.RefreshToken;
    const userID = data.userID || data.userId || data.UserID || data.Id || data.id;

    if (!accessToken) {
        throw new Error('No token received from the server.');
    }

    const storage = credentials.remember ? localStorage : sessionStorage;

    // Clear previous sessions across both storages to avoid conflicting tokens
    localStorage.removeItem('authToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('userID');
    sessionStorage.removeItem('authToken');
    sessionStorage.removeItem('refreshToken');
    sessionStorage.removeItem('userID');

    // Save newly issued credentials
    storage.setItem('authToken', accessToken);

    if (refreshToken) {
        storage.setItem('refreshToken', refreshToken);
    }

    if (userID) {
        storage.setItem('userID', userID);
    } else {
        console.warn('Warning: userID not found in login response. The token refresh process may fail.');
    }

    return data;
}

/**
 * Checks whether an active authentication token exists.
 * @returns {boolean}
 */
export function isAuthenticated() {
    return !!(localStorage.getItem('authToken') || sessionStorage.getItem('authToken'));
}

/**
 * Retrieves the currently active JWT access token.
 * @returns {string|null}
 */
export function getAuthToken() {
    return localStorage.getItem('authToken') || sessionStorage.getItem('authToken') || null;
}

/**
 * Retrieves the stored User ID.
 * @returns {string|null}
 */
export function getUserID() {
    return localStorage.getItem('userID') || sessionStorage.getItem('userID') || null;
}

/**
 * Logs out the user and redirects to login page.
 */
export function logoutUser() {
    apiLogout();
}