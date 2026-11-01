import { request } from '../services/api.js';
import { isAuthenticated } from '../services/auth.js';

// Base backend URL for static assets (e.g., 360 panorama images)
const BACKEND_URL = 'https://localhost:8000'; 

document.addEventListener('DOMContentLoaded', async () => {
    // 1. Extract property ID from URL query params
    const urlParams = new URLSearchParams(window.location.search);
    const propertyId = urlParams.get('id');

    if (!propertyId) {
        alert('Property ID not found!');
        window.location.href = 'home.html';
        return;
    }

    try {
        // 2. Fetch property details from API
        const property = await request(`/properties/${propertyId}`, { method: 'GET' });

        const pId = property.propertyID || property.propertyId || propertyId;

        // 3. Render property details on page
        renderPropertyDetails(property);

        // 4. Initialize controls and event listeners
        setupViewerControls();
        setupFavorites(pId, property.isFavorite);
        setupRequestModal(pId);

    } catch (error) {
        console.error('Error loading property details:', error);
        const titleEl = document.getElementById('property-title');
        if (titleEl) titleEl.textContent = 'An error occurred while loading property details.';
    }
});

/* ==========================================================================
   1. Populate Property Details
   ========================================================================== */
function renderPropertyDetails(property) {
    // Titles
    const titleEl = document.getElementById('property-title');
    const viewerTitleEl = document.getElementById('viewer-title');
    if (titleEl) titleEl.textContent = property.title || 'No Title Available';
    if (viewerTitleEl) viewerTitleEl.textContent = property.title || 'Property Experience';

    // Location
    const displayLocation = property.location || property.city || 'Location not specified';
    const locationEl = document.getElementById('property-location');
    if (locationEl) locationEl.textContent = displayLocation;

    // Price
    const priceEl = document.getElementById('property-price-value');
    if (priceEl) {
        priceEl.textContent = property.price ? `$${property.price.toLocaleString()}` : '--';
    }

    // Specifications
    const propTypeEl = document.getElementById('property-type');
    const featTypeEl = document.getElementById('feature-type');
    const featAreaEl = document.getElementById('feature-area');
    const featBedEl = document.getElementById('feature-bedrooms');
    const featBathEl = document.getElementById('feature-bathrooms');

    if (propTypeEl) propTypeEl.textContent = property.propertyType || 'Property';
    if (featTypeEl) featTypeEl.textContent = property.propertyType || '--';
    if (featAreaEl) featAreaEl.textContent = property.areaSQM ? `${property.areaSQM} m²` : '--';
    if (featBedEl) featBedEl.textContent = property.bedrooms ?? '0';
    if (featBathEl) featBathEl.textContent = property.bathrooms ?? '0';

    // Description
    const descEl = document.getElementById('property-description');
    if (descEl) descEl.textContent = property.description || 'No description available.';

    // 360° Panorama Image
    const panorama = document.getElementById('property-panorama');
    if (panorama && property.panoramas && property.panoramas.length > 0) {
        let panoramaPath = property.panoramas[0].panoramaURL || property.panoramas[0].panoramaUrl || property.panoramas[0];
        
        if (typeof panoramaPath === 'string') {
            if (panoramaPath.startsWith('/')) {
                panoramaPath = `${BACKEND_URL}${panoramaPath}`;
            }
            panorama.setAttribute('src', panoramaPath);
        }
    }
}

/* ==========================================================================
   2. 360° Panorama Controls (Reset & Fullscreen)
   ========================================================================== */
function setupViewerControls() {
    const resetBtn = document.getElementById('reset-view-btn');
    const camera = document.getElementById('property-camera');
    if (resetBtn && camera) {
        resetBtn.addEventListener('click', () => {
            camera.setAttribute('rotation', '0 -90 0');
        });
    }

    const fullscreenBtn = document.getElementById('fullscreen-btn');
    const container = document.getElementById('aframe-container');
    if (fullscreenBtn && container) {
        fullscreenBtn.addEventListener('click', () => {
            if (!document.fullscreenElement) {
                container.requestFullscreen().catch(err => console.error('Fullscreen error:', err));
            } else {
                document.exitFullscreen();
            }
        });
    }
}

/* ==========================================================================
   3. Helper: Robust Update Favorite Button UI
   ========================================================================== */
function updateFavoriteButtonUI(btn, isFavorite) {
    if (!btn) return;

    const icon = btn.querySelector('i') || btn.querySelector('svg');

    if (isFavorite) {
        btn.classList.add('active', 'is-favorite');
        if (icon) {
            icon.classList.remove('far', 'fa-regular');
            icon.classList.add('fas', 'fa-solid', 'fa-heart');
        }
    } else {
        btn.classList.remove('active', 'is-favorite');
        if (icon) {
            icon.classList.remove('fas', 'fa-solid');
            icon.classList.add('far', 'fa-regular', 'fa-heart');
        }
    }
}

/* ==========================================================================
   4. Add / Remove Favorites Toggle
   ========================================================================== */
function setupFavorites(propertyId, initialIsFavorite = false) {
    const favoriteBtn = document.getElementById('favorite-btn');
    if (!favoriteBtn) return;

    let currentIsFavorite = Boolean(initialIsFavorite);
    updateFavoriteButtonUI(favoriteBtn, currentIsFavorite);

    favoriteBtn.addEventListener('click', async (e) => {
        e.preventDefault();
        e.stopPropagation();

        if (!isAuthenticated()) {
            alert('Please log in to manage your favorites.');
            const currentUrl = encodeURIComponent(window.location.href);
            window.location.href = `login.html?redirect=${currentUrl}`;
            return;
        }

        favoriteBtn.disabled = true;

        // FavoriteMode Enum: 0 = Remove, 1 = Add
        const targetMode = currentIsFavorite ? 0 : 1;

        try {
            await request('/favorites/toggle', {
                method: 'POST',
                body: JSON.stringify({
                    propertyID: parseInt(propertyId, 10),
                    mode: targetMode
                })
            });

            // تحديث الحالة والواجهة بعد نجاح الاستجابة
            currentIsFavorite = !currentIsFavorite;
            updateFavoriteButtonUI(favoriteBtn, currentIsFavorite);

        } catch (err) {
            console.error('Error updating favorite:', err);
            alert(err.message || 'An error occurred while updating favorites.');
        } finally {
            favoriteBtn.disabled = false;
        }
    });
}

/* ==========================================================================
   5. Request Property Modal
   ========================================================================== */
function setupRequestModal(propertyId) {
    const modal = document.getElementById('request-modal');
    const requestBtn = document.getElementById('request-btn');
    const closeBtn = document.getElementById('close-modal-btn');
    const form = document.getElementById('property-request-form');

    if (!modal) return;

    if (requestBtn) {
        requestBtn.addEventListener('click', () => {
            if (!isAuthenticated()) {
                alert('Please log in to submit a request for this property.');
                const currentUrl = encodeURIComponent(window.location.href);
                window.location.href = `login.html?redirect=${currentUrl}`;
                return;
            }
            modal.setAttribute('aria-hidden', 'false');
            modal.style.display = 'flex';
        });
    }

    if (closeBtn) {
        closeBtn.addEventListener('click', () => {
            modal.setAttribute('aria-hidden', 'true');
            modal.style.display = 'none';
        });
    }

    window.addEventListener('click', (e) => {
        if (e.target === modal) {
            modal.setAttribute('aria-hidden', 'true');
            modal.style.display = 'none';
        }
    });

    if (form) {
        form.addEventListener('submit', async (e) => {
            e.preventDefault();
            const messageInput = document.getElementById('request-message');

            try {
                await request('/Requests', {
                    method: 'POST',
                    body: JSON.stringify({
                        propertyId: parseInt(propertyId, 10),
                        message: messageInput.value
                    })
                });

                alert('Request submitted successfully!');
                modal.setAttribute('aria-hidden', 'true');
                modal.style.display = 'none';
                form.reset();
            } catch (err) {
                console.error('Error submitting request:', err);
                alert(err.message || 'Failed to submit property request.');
            }
        });
    }
}