// ============================================================
//  wwwroot/js/site.js
//  Client-side enhancements
// ============================================================

// Auto-dismiss success alerts after 4 seconds
document.addEventListener('DOMContentLoaded', () => {
    const alerts = document.querySelectorAll('.alert-success');
    alerts.forEach(alert => {
        setTimeout(() => {
            const bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
            bsAlert.close();
        }, 4000);
    });

    // Highlight active nav link
    const currentPath = window.location.pathname.toLowerCase();
    document.querySelectorAll('.nav-link').forEach(link => {
        const href = link.getAttribute('href')?.toLowerCase();
        if (href && currentPath.startsWith(href) && href !== '/') {
            link.classList.add('active', 'fw-semibold');
        }
    });
});