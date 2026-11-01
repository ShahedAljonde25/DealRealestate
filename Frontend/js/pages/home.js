const API_BASE_URL = 'https://localhost:8000/api/properties';

document.addEventListener('DOMContentLoaded', () => {
    fetchFeaturedProperties();

    // ربط نموذج البحث (Search Form) بحدث الإرسال للفلترة
    const searchForm = document.getElementById('searchForm');
    if (searchForm) {
        searchForm.addEventListener('submit', (e) => {
            e.preventDefault(); // منع إعادة تحميل الصفحة
            
            const selectedType = document.getElementById('searchType').value;
            
            // إعادة جلب العقارات بناءً على النوع المختار
            fetchFeaturedProperties(selectedType);
        });
    }
});


// ==========================================
// FETCH PROPERTIES (مع دعم الفلترة الاختيارية)
// ==========================================

async function fetchFeaturedProperties(propertyType = '') {
    const gridContainer = document.getElementById('propertiesGrid');

    // إظهار حالة التحميل
    gridContainer.innerHTML = `
        <div class="loading-state" style="grid-column: 1 / -1; text-align: center; padding: 50px; color: #64748b;">
            <i class="fa-solid fa-spinner fa-spin fa-2x"></i>
            <p style="margin-top: 10px;">Loading properties from database...</p>
        </div>
    `;

    try {
        let url = API_BASE_URL;

        // إذا تم اختيار نوع عقار، نضيفه كـ Query Parameter للرابط
        if (propertyType && propertyType.trim() !== '') {
            url += `?propertyType=${encodeURIComponent(propertyType)}`;
        }

        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'Accept': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error('No properties found');
        }

        const properties = await response.json();

        // Clear loading state
        gridContainer.innerHTML = '';

        // Check empty result
        if (!properties || properties.length === 0) {
            gridContainer.innerHTML = `
                <p class="empty-properties" style="grid-column: 1 / -1; text-align: center; padding: 40px; color: #64748b;">
                    No properties available matching this filter.
                </p>
            `;
            return;
        }

        // ==========================================
        // CREATE PROPERTY CARDS
        // ==========================================

        properties.forEach(property => {
            const cardHTML = `
                <article
                    class="property-card"
                    data-property-id="${property.propertyID}"
                >
                    <div class="property-img">
                        <img
                            src="${property.imageURL || '../assets/images/hero.jpg'}"
                            alt="${property.title || 'Property'}"
                            loading="lazy"
                        >
                        <span class="property-tag">
                            ${property.propertyType || 'Property'}
                        </span>
                    </div>

                    <div class="property-details">
                        <h3>
                            ${property.title || 'Untitled Property'}
                        </h3>

                        <p>
                            <i class="fa-solid fa-location-dot"></i>
                            ${
                                property.location ||
                                property.city ||
                                'Location unavailable'
                            }
                        </p>

                        <div class="property-footer">
                            <span class="price">
                                ${
                                    property.price
                                        ? property.price.toLocaleString() + ' JD'
                                        : 'Price on request'
                                }
                            </span>

                            <a
                                href="property-details.html?id=${property.propertyID}"
                                class="details-btn"
                            >
                                View
                            </a>
                        </div>
                    </div>
                </article>
            `;

            gridContainer.insertAdjacentHTML(
                'beforeend',
                cardHTML
            );
        });

    } catch (error) {
        console.error('Error fetching properties:', error);

        gridContainer.innerHTML = `
            <div class="properties-error" style="grid-column: 1 / -1; text-align: center; padding: 40px; color: #ef4444;">
                <i class="fa-solid fa-triangle-exclamation fa-2x"></i>
                <p style="margin-top: 10px;">
                    Could not load properties. Please check your connection or backend API.
                </p>
            </div>
        `;
    }
}
