// js/pages/login.js

import { loginUser } from '../services/auth.js';

const loginForm = document.getElementById('loginForm');
const emailInput = document.getElementById('email');
const passwordInput = document.getElementById('password');
const rememberMeCheckbox = document.getElementById('rememberMe');
const errorDiv = document.getElementById('errorMessage');

if (loginForm) {
    loginForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        if (errorDiv) errorDiv.style.display = 'none';

        const email = emailInput.value.trim();
        const password = passwordInput.value;
        const remember = rememberMeCheckbox ? rememberMeCheckbox.checked : false;

        try {
            // Delegate auth logic to auth.js
            await loginUser({ email, password, remember });

            // Redirect back to return URL or Home
            const urlParams = new URLSearchParams(window.location.search);
            const redirectUrl = urlParams.get('redirect');

            window.location.href = redirectUrl ? decodeURIComponent(redirectUrl) : '../pages/home.html';

        } catch (error) {
            if (errorDiv) {
                errorDiv.textContent = error.message || 'Invalid email or password.';
                errorDiv.style.display = 'block';
            }
        }
    });
}