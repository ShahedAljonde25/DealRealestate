document.addEventListener("DOMContentLoaded", function() {
    fetch('../components/footer.html')
        .then(response => {
            if (!response.ok) throw new Error('Failed to load footer');
            return response.text();
        })
        .then(data => {
            const placeholder = document.getElementById('footer-placeholder');
            if (placeholder) {
                placeholder.innerHTML = data;
            }
        })
        .catch(error => console.error('Error loading footer:', error));
});

document.addEventListener("DOMContentLoaded", function() {
    
    // دالة لجلب المكونات
    function loadComponent(url, placeholderId) {
        fetch(url)
            .then(response => {
                if (!response.ok) throw new Error(`Failed to load ${url}`);
                return response.text();
            })
            .then(data => {
                const placeholder = document.getElementById(placeholderId);
                if (placeholder) {
                    placeholder.innerHTML = data;
                }
            })
            .catch(error => console.error(`Error loading ${url}:`, error));
    }

    // استدعاء المكونات
    loadComponent('../components/navbar.html', 'navbar-placeholder');
    loadComponent('../components/footer.html', 'footer-placeholder');
});