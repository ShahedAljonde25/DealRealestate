const apiBaseUrl = "https://localhost:8000/api"; 

async function fetchUserFavorites() {
    const container = document.getElementById("favoritesContainer");
    const alertContainer = document.getElementById("alertContainer");
    
    const token = localStorage.getItem("authToken") || sessionStorage.getItem("authToken");

    if (!token) {
        container.innerHTML = "";
        alertContainer.innerHTML = `
            <div class="alert alert-warning text-center" role="alert">
                Please <a href="login.html" class="alert-link">login</a> first to view your favorites.
            </div>
        `;
        return;
    }

    try {
        const response = await fetch(`${apiBaseUrl}/Favorites/my-favorites`, {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            }
        });

        if (response.status === 404) {
            container.innerHTML = "";
            alertContainer.innerHTML = `
                <div class="alert alert-info text-center" role="alert">
                    <i class="fa-solid fa-info-circle"></i> You have no favorite properties yet.
                </div>
            `;
            return;
        }

        if (!response.ok) throw new Error("Failed to fetch favorites.");

        const favorites = await response.json();
        container.innerHTML = "";

        favorites.forEach(fav => {
            const propertyTitle = fav.title || `Property #${fav.propertyID}`;
            const propertyPrice = fav.price ? `$${fav.price.toLocaleString()}` : "Price on request";
            
            const propertyImage = fav.imageUrl || "https://via.placeholder.com/400x200?text=DealRealState";

            container.innerHTML += `
                <div class="col-md-4 mb-4" id="fav-card-${fav.favoriteID}">
                    <div class="card property-card h-100">
                        <div class="property-img-wrapper">
                            <img src="${propertyImage}" alt="Property Image">
                            <button class="btn-remove-favorite" onclick="removeFavorite(${fav.propertyID})" title="Remove">
                                <i class="fa-solid fa-heart"></i>
                            </button>
                        </div>
                        <div class="card-body d-flex flex-column">
                            <h5 class="card-title fw-bold text-dark">${propertyTitle}</h5>
                            <p class="text-primary fw-semibold fs-5 mb-2">${propertyPrice}</p>
                            <p class="card-text text-muted small">Location: ${fav.city || 'N/A'}</p>
                            <p class="card-text text-muted small mb-4">Added: ${new Date(fav.createdAt).toLocaleDateString()}</p>
                            <div class="mt-auto">
                                <a href="property-details.html?id=${fav.propertyID}" class="btn btn-outline-primary w-100">View Details</a>
                            </div>
                        </div>
                    </div>
                </div>
            `;
        });

    } catch (error) {
        console.error("Error:", error);
        container.innerHTML = `<div class="alert alert-danger text-center">Failed to load favorites.</div>`;
    }
}

async function removeFavorite(propertyID) {
    const token = localStorage.getItem("authToken") || sessionStorage.getItem("authToken");
    try {
        const response = await fetch(`${apiBaseUrl}/Favorites/toggle`, {
            method: "POST",
            headers: {
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ propertyID: propertyID })
        });

        if (response.ok) {
            fetchUserFavorites(); // تحديث القائمة فوراً
        }
    } catch (err) {
        console.error("Error:", err);
    }
}

document.addEventListener("DOMContentLoaded", fetchUserFavorites);